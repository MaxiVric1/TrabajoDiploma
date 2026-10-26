using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // PN3 - Arquitectura: datos técnicos de los proyectos En Análisis (lote, superficie edificable en planta, altura
    // máxima, costo de obra y valor de venta por m² en USD). En CABA, "Consultar CABA" los trae del Código Urbanístico
    // (USIG + EPOK) y Arquitectura puede corregirlos; en otras localidades se cargan a mano. Los totales se ven al
    // instante y los guarda calculados la base.
    public partial class FormEvaluacionArquitectonica : Form, IObservador
    {
        private class FilaProyecto
        {
            public int NroProyecto { get; set; }
            public string Direccion { get; set; }
            public string Localidad { get; set; }
            public decimal? M2Terreno { get; set; }
            public string Operacion { get; set; }
            public decimal Canje { get; set; }
            public string DatosTecnicos { get; set; }
            [Browsable(false)] public ProyectoInversion Proyecto { get; set; }
        }

        private ProyectoInversionBLL proyectoBLL = new ProyectoInversionBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _grilla;
        private bool _cargando;
        private string _smp, _distrito; // de la última consulta a EPOK (o los ya guardados), como referencia

        public FormEvaluacionArquitectonica()
        {
            InitializeComponent();
            PN3Comun.EstiloGrilla(dgvProyectos);
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormEvaluacionArquitectonica"));
            foreach (var (control, clave) in new (Control, string)[] {
                (grpInmueble, BE.NombreControl.FormEvaluacionArquitectonica_grpInmueble),
                (lblDireccion, BE.NombreControl.FormEvaluacionArquitectonica_lblDireccion),
                (lblLocalidad, BE.NombreControl.FormEvaluacionArquitectonica_lblLocalidad),
                (lblTipo, BE.NombreControl.FormEvaluacionArquitectonica_lblTipo),
                (lblSuperficie, BE.NombreControl.FormEvaluacionArquitectonica_lblSuperficie),
                (lblCanje, BE.NombreControl.FormEvaluacionArquitectonica_lblCanje),
                (grpPropietario, BE.NombreControl.FormEvaluacionArquitectonica_grpPropietario),
                (lblPropietario, BE.NombreControl.FormEvaluacionArquitectonica_lblPropietario),
                (lblDni, BE.NombreControl.FormEvaluacionArquitectonica_lblDni),
                (lblTelefono, BE.NombreControl.FormEvaluacionArquitectonica_lblTelefono),
                (lblEmail, BE.NombreControl.FormEvaluacionArquitectonica_lblEmail),
                (grpDatosTecnicos, BE.NombreControl.FormEvaluacionArquitectonica_grpDatosTecnicos),
                (lblLote, BE.NombreControl.FormEvaluacionArquitectonica_lblLote),
                (lblSupEdificable, BE.NombreControl.FormEvaluacionArquitectonica_lblSupEdificable),
                (lblAltura, BE.NombreControl.FormEvaluacionArquitectonica_lblAltura),
                (btnConsultarCaba, BE.NombreControl.FormEvaluacionArquitectonica_btnConsultarCaba),
                (lblCostoM2, BE.NombreControl.FormEvaluacionArquitectonica_lblCostoM2),
                (lblValorM2, BE.NombreControl.FormEvaluacionArquitectonica_lblValorM2),
                (lblMetros, BE.NombreControl.FormEvaluacionArquitectonica_lblMetros),
                (lblCostoTotal, BE.NombreControl.FormEvaluacionArquitectonica_lblCostoTotal),
                (lblValorTotal, BE.NombreControl.FormEvaluacionArquitectonica_lblValorTotal),
                (lblMargen, BE.NombreControl.FormEvaluacionArquitectonica_lblMargen),
                (lblIcc, BE.NombreControl.FormEvaluacionArquitectonica_lblIcc),
                (btnGuardar, BE.NombreControl.FormEvaluacionArquitectonica_btnGuardar) })
                _controlesTraducibles.Add(new EtiquetaTraducible(control, clave));
            _grilla = new DataGridTraducible(dgvProyectos, "FormEvaluacionArquitectonica.dgvProyectos")
                .ConColumna("NroProyecto", BE.NombreControl.FormEvaluacionArquitectonica_dgvProyectos_nroProyecto)
                .ConColumna("Direccion", BE.NombreControl.FormEvaluacionArquitectonica_dgvProyectos_direccion)
                .ConColumna("Localidad", BE.NombreControl.FormEvaluacionArquitectonica_dgvProyectos_localidad)
                .ConColumna("M2Terreno", BE.NombreControl.FormEvaluacionArquitectonica_dgvProyectos_m2Terreno)
                .ConColumna("Operacion", BE.NombreControl.FormEvaluacionArquitectonica_dgvProyectos_operacion)
                .ConColumna("Canje", BE.NombreControl.FormEvaluacionArquitectonica_dgvProyectos_canje)
                .ConColumna("DatosTecnicos", BE.NombreControl.FormEvaluacionArquitectonica_dgvProyectos_datosTecnicos);
            _controlesTraducibles.Add(_grilla);
            GestorIdioma.Instancia.Adjuntar(this);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles) control.Traducir(traducciones);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormEvaluacionArquitectonica_Load(object sender, EventArgs e)
        {
            nudSupEdificable.Increment = 10;
            nudAltura.Increment = 0.5m;
            nudCostoM2.Increment = nudValorM2.Increment = 50;
            MostrarIcc();
            CargarProyectos();
        }

        // Último ICC publicado por el INDEC (referencia para el costo de obra)
        private void MostrarIcc()
        {
            try
            {
                IccBLL icc = new IccBLL();
                ValorICC ultimo = icc.Ultimo();
                lblIccValor.Text = ultimo == null ? "Sin datos del INDEC" + (icc.UltimoError != null ? $" ({icc.UltimoError})" : "")
                    : $"{ultimo.Periodo:MM/yyyy}: {ultimo.ValorIndice:N2} ({ultimo.VariacionMensual:+0.##;-0.##}% mensual)" +
                      (icc.UltimoError != null ? " - sin conexión, dato guardado" : "");
            }
            catch (Exception ex)
            {
                lblIccValor.Text = "Sin datos del INDEC: " + ex.Message;
            }
        }

        private void CargarProyectos(int? idASeleccionar = null)
        {
            _cargando = true;
            try
            {
                List<FilaProyecto> filas = proyectoBLL.LeerTodos(EstadoProyecto.EnAnalisis).Select(p => new FilaProyecto
                {
                    NroProyecto = p.IdProyecto,
                    Direccion = p.Direccion,
                    Localidad = p.Localidad,
                    M2Terreno = p.SuperficieLote,
                    Operacion = p.TipoPublicacion.ToString(),
                    Canje = p.PorcentajeCanje,
                    DatosTecnicos = p.DatosTecnicosCompletos ? "Completos" : "Pendientes",
                    Proyecto = p
                }).ToList();
                dgvProyectos.DataSource = null;
                dgvProyectos.DataSource = filas;
                PN3Comun.FormatoColumnas(dgvProyectos, new[] { "M2Terreno", "Canje" });
                _grilla.Traducir(GestorIdioma.Instancia.ObtenerTraduccionesActuales());
                foreach (DataGridViewRow fila in dgvProyectos.Rows)
                {
                    FilaProyecto f = (FilaProyecto)fila.DataBoundItem;
                    if (!f.Proyecto.DatosTecnicosCompletos) fila.DefaultCellStyle.ForeColor = PN3Comun.AMARILLO;
                    if (idASeleccionar == f.NroProyecto) dgvProyectos.CurrentCell = fila.Cells["NroProyecto"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los proyectos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { _cargando = false; }
            MostrarProyecto();
        }

        private ProyectoInversion Seleccionado() => (dgvProyectos.CurrentRow?.DataBoundItem as FilaProyecto)?.Proyecto;

        private void dgvProyectos_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarProyecto();
        }

        private void MostrarProyecto()
        {
            ProyectoInversion p = Seleccionado();
            grpDatosTecnicos.Enabled = btnGuardar.Enabled = p != null;
            _smp = p?.Smp;
            _distrito = p?.DistritoUrbanistico;
            MostrarParcela(p);
            foreach (Label l in new[] { lblDireccionValor, lblLocalidadValor, lblTipoValor, lblSuperficieValor, lblCanjeValor,
                                        lblPropietarioValor, lblDniValor, lblTelefonoValor, lblEmailValor })
                l.Text = "-";
            if (p == null)
            {
                DatosTecnicos_Changed(this, EventArgs.Empty);
                return;
            }
            try
            {
                Inmueble inmueble = new InmuebleBLL().BuscarPorId(p.IdInmueble);
                lblDireccionValor.Text = p.Direccion;
                lblLocalidadValor.Text = p.Localidad ?? "-";
                lblTipoValor.Text = $"{p.TipoInmueble} en {p.TipoPublicacion.ToString().ToLower()}";
                lblSuperficieValor.Text = $"{inmueble.MetrosCuadrados:N2} m² cubiertos, {inmueble.CantidadAmbientes} ambientes" +
                                          (inmueble.SuperficieLote.HasValue ? $", lote {inmueble.SuperficieLote:N2} m²" : "");
                lblCanjeValor.Text = $"{p.PorcentajeCanje:0.##}% de las unidades";
                Cliente propietario = new ClienteBLL().BuscarPorId(inmueble.IdCliente);
                if (propietario != null)
                {
                    lblPropietarioValor.Text = $"{propietario.Nombre} {propietario.Apellido}";
                    lblDniValor.Text = propietario.Dni;
                    lblTelefonoValor.Text = string.IsNullOrEmpty(propietario.Telefono) ? "-" : propietario.Telefono;
                    lblEmailValor.Text = string.IsNullOrEmpty(propietario.Email) ? "-" : propietario.Email;
                }

                _cargando = true;
                nudLote.Value = Math.Min(p.SuperficieLote ?? inmueble.SuperficieLote ?? 0, nudLote.Maximum);
                nudSupEdificable.Value = Math.Min(p.SuperficieEdificablePlanta ?? 0, nudSupEdificable.Maximum);
                nudAltura.Value = Math.Min(p.AlturaMaxima ?? 0, nudAltura.Maximum);
                nudCostoM2.Value = p.CostoObraM2 ?? 0;
                nudValorM2.Value = p.ValorVentaM2Unidades ?? 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el proyecto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { _cargando = false; }
            DatosTecnicos_Changed(this, EventArgs.Empty);
        }

        // CABA: botón de consulta a EPOK y la parcela de referencia; otras localidades: carga manual
        private void MostrarParcela(ProyectoInversion p)
        {
            bool caba = p != null && ProyectoInversionBLL.EsCaba(p.Localidad);
            btnConsultarCaba.Visible = caba;
            btnConsultarCaba.Enabled = true;
            lblParcela.ForeColor = System.Drawing.Color.White;
            if (p == null)
                lblParcela.Text = "";
            else if (!caba)
            {
                lblParcela.Text = $"Carga manual requerida para esta localidad ({p.Localidad ?? "sin localidad"}).";
                lblParcela.ForeColor = PN3Comun.AMARILLO;
            }
            else
                lblParcela.Text = _smp != null || _distrito != null
                    ? $"SMP {_smp ?? "-"} - distrito {_distrito ?? "-"}"
                    : "Inmueble de CABA: \"Consultar CABA\" trae los parámetros del Código Urbanístico.";
        }

        // Consulta a USIG + EPOK fuera del hilo de la interfaz (los servicios del GCBA pueden tardar)
        private async void btnConsultarCaba_Click(object sender, EventArgs e)
        {
            ProyectoInversion p = Seleccionado();
            if (p == null) return;
            btnConsultarCaba.Enabled = btnGuardar.Enabled = false;
            lblParcela.ForeColor = System.Drawing.Color.White;
            lblParcela.Text = $"Consultando USIG y EPOK para {p.Direccion}...";
            Cursor = Cursors.WaitCursor;
            try
            {
                ParcelaCaba parcela = await Task.Run(() => proyectoBLL.ConsultarParametrosCaba(p.IdProyecto));
                if (Seleccionado()?.IdProyecto != p.IdProyecto) return; // cambió la selección mientras consultaba
                _cargando = true;
                if (parcela.SuperficieParcela.HasValue) nudLote.Value = Math.Min(parcela.SuperficieParcela.Value, nudLote.Maximum);
                nudSupEdificable.Value = Math.Min(parcela.SuperficieEdificablePlanta.Value, nudSupEdificable.Maximum);
                nudAltura.Value = Math.Min(parcela.AlturaMaxima.Value, nudAltura.Maximum);
                _cargando = false;
                _smp = parcela.Smp;
                _distrito = parcela.Distrito;
                lblParcela.ForeColor = PN3Comun.VERDE;
                lblParcela.Text = $"SMP {parcela.Smp} - distrito {parcela.Distrito} - parcela {parcela.SuperficieParcela:N2} m² " +
                                  $"(catastro: {parcela.DireccionCatastral}; {parcela.Busqueda}). Puede corregir los valores antes de guardar.";
                DatosTecnicos_Changed(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _cargando = false;
                lblParcela.ForeColor = PN3Comun.ROJO;
                lblParcela.Text = "Sin datos de EPOK: cargue los valores a mano.";
                MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + "Puede cargar los datos técnicos a mano.",
                    "Consultar CABA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnConsultarCaba.Enabled = true;
                btnGuardar.Enabled = Seleccionado() != null;
            }
        }

        // Mismo cálculo que las columnas calculadas de ProyectoInversion: pisos (altura / 3 m) x superficie en planta
        private void DatosTecnicos_Changed(object sender, EventArgs e)
        {
            if (_cargando) return;
            int pisos = (int)Math.Floor(nudAltura.Value / ParcelaCaba.ALTURA_POR_PISO);
            decimal metros = Math.Round(pisos * nudSupEdificable.Value, 2);
            decimal costo = Math.Round(metros * nudCostoM2.Value, 2);
            decimal valor = Math.Round(metros * nudValorM2.Value, 2);
            lblMetrosValor.Text = $"{metros:N2} m² ({pisos} pisos en {nudAltura.Value:0.##} m x {nudSupEdificable.Value:N2} m² por planta)";
            lblCostoTotalValor.Text = $"{PN3Comun.Usd(costo)} ({metros:N2} m² x USD {nudCostoM2.Value:N2})";
            lblValorTotalValor.Text = $"{PN3Comun.Usd(valor)} ({metros:N2} m² x USD {nudValorM2.Value:N2})";
            lblMargenValor.Text = $"{PN3Comun.Usd(valor - costo)} (valor de las unidades menos la obra)";
            lblMargenValor.ForeColor = valor - costo >= 0 ? System.Drawing.Color.White : PN3Comun.ROJO;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ProyectoInversion p = Seleccionado();
            if (p == null) return;
            try
            {
                proyectoBLL.ActualizarDatosTecnicos(new ProyectoInversion
                {
                    IdProyecto = p.IdProyecto,
                    SuperficieLote = nudLote.Value,
                    SuperficieEdificablePlanta = nudSupEdificable.Value,
                    AlturaMaxima = nudAltura.Value,
                    Smp = _smp,
                    DistritoUrbanistico = _distrito,
                    CostoObraM2 = nudCostoM2.Value,
                    ValorVentaM2Unidades = nudValorM2.Value
                });
                MessageBox.Show($"Datos técnicos del Proyecto N° {p.IdProyecto} guardados. Ya puede evaluarlo el Directorio.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarProyectos(p.IdProyecto);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos técnicos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
