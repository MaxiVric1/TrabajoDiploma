using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // PN3 - Directorio: proyectos En Análisis con los datos técnicos completos; para cada uno, el análisis comparativo
    // con los datos de mercado del día y el mapa. Aprueba o rechaza (con motivo).
    public partial class FormDecisionDirectorio : Form, IObservador
    {
        private class FilaProyecto
        {
            public int NroProyecto { get; set; }
            public string Direccion { get; set; }
            public string Localidad { get; set; }
            public string Operacion { get; set; }
            public decimal? MetrosConstruibles { get; set; }
            public decimal? CostoTotalObra { get; set; }
            public decimal? ValorTotalUnidades { get; set; }
            public decimal Canje { get; set; }
            [Browsable(false)] public ProyectoInversion Proyecto { get; set; }
        }

        private ProyectoInversionBLL proyectoBLL = new ProyectoInversionBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _grilla;
        private readonly Dictionary<int, AnalisisComparativo> _analisis = new Dictionary<int, AnalisisComparativo>(); // ya calculados
        private ValorICC _icc;
        private bool _cargando;

        public FormDecisionDirectorio()
        {
            InitializeComponent();
            PN3Comun.EstiloGrilla(dgvProyectos);
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormDecisionDirectorio"));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnAprobar, BE.NombreControl.FormDecisionDirectorio_btnAprobar));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRechazar, BE.NombreControl.FormDecisionDirectorio_btnRechazar));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnComisiones, BE.NombreControl.FormDecisionDirectorio_btnComisiones));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnInforme, BE.NombreControl.FormDecisionDirectorio_btnInforme));
            _controlesTraducibles.AddRange(panelAnalisis.Traducibles());
            _grilla = new DataGridTraducible(dgvProyectos, "FormDecisionDirectorio.dgvProyectos")
                .ConColumna("NroProyecto", BE.NombreControl.FormDecisionDirectorio_dgvProyectos_nroProyecto)
                .ConColumna("Direccion", BE.NombreControl.FormDecisionDirectorio_dgvProyectos_direccion)
                .ConColumna("Localidad", BE.NombreControl.FormDecisionDirectorio_dgvProyectos_localidad)
                .ConColumna("Operacion", BE.NombreControl.FormDecisionDirectorio_dgvProyectos_operacion)
                .ConColumna("MetrosConstruibles", BE.NombreControl.FormDecisionDirectorio_dgvProyectos_metrosConstruibles)
                .ConColumna("CostoTotalObra", BE.NombreControl.FormDecisionDirectorio_dgvProyectos_costoTotalObra)
                .ConColumna("ValorTotalUnidades", BE.NombreControl.FormDecisionDirectorio_dgvProyectos_valorTotalUnidades)
                .ConColumna("Canje", BE.NombreControl.FormDecisionDirectorio_dgvProyectos_canje);
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

        private void FormDecisionDirectorio_Load(object sender, EventArgs e)
        {
            CargarProyectos();
        }

        private void CargarProyectos()
        {
            _cargando = true;
            try
            {
                List<FilaProyecto> filas = proyectoBLL.LeerTodos(EstadoProyecto.EnAnalisis).Where(p => p.DatosTecnicosCompletos).Select(p => new FilaProyecto
                {
                    NroProyecto = p.IdProyecto,
                    Direccion = p.Direccion,
                    Localidad = p.Localidad,
                    Operacion = p.TipoPublicacion.ToString(),
                    MetrosConstruibles = p.MetrosConstruibles,
                    CostoTotalObra = p.CostoTotalObra,
                    ValorTotalUnidades = p.ValorTotalUnidades,
                    Canje = p.PorcentajeCanje,
                    Proyecto = p
                }).ToList();
                dgvProyectos.DataSource = null;
                dgvProyectos.DataSource = filas;
                PN3Comun.FormatoColumnas(dgvProyectos, new[] { "MetrosConstruibles", "CostoTotalObra", "ValorTotalUnidades", "Canje" });
                _grilla.Traducir(GestorIdioma.Instancia.ObtenerTraduccionesActuales());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los proyectos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { _cargando = false; }
            MostrarAnalisis();
        }

        private ProyectoInversion Seleccionado() => (dgvProyectos.CurrentRow?.DataBoundItem as FilaProyecto)?.Proyecto;

        private void dgvProyectos_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarAnalisis();
        }

        private void MostrarAnalisis()
        {
            ProyectoInversion p = Seleccionado();
            btnAprobar.Enabled = btnRechazar.Enabled = btnInforme.Enabled = false;
            btnComisiones.Enabled = p != null;
            if (p == null)
            {
                panelAnalisis.Limpiar(dgvProyectos.Rows.Count == 0 ? "No hay proyectos con datos técnicos esperando la decisión del Directorio." : "-");
                lblEstado.Text = "";
                return;
            }
            if (_analisis.TryGetValue(p.IdProyecto, out AnalisisComparativo calculado))
            {
                Mostrar(calculado);
                return;
            }
            panelAnalisis.Limpiar("Calculando el análisis con los datos de mercado...");
            lblEstado.Text = $"Proyecto N° {p.IdProyecto}: consultando DolarAPI, BCRA e INDEC...";
            BeginInvoke(new Action(() => Calcular(p.IdProyecto)));
        }

        private void Calcular(int idProyecto)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (_icc == null) _icc = new IccBLL().Ultimo();
                _analisis[idProyecto] = proyectoBLL.ObtenerAnalisisComparativo(idProyecto);
                if (Seleccionado()?.IdProyecto == idProyecto) Mostrar(_analisis[idProyecto]);
            }
            catch (Exception ex)
            {
                panelAnalisis.Limpiar("No se pudo calcular el análisis: " + ex.Message);
                lblEstado.Text = "";
            }
            finally { Cursor = Cursors.Default; }
        }

        private void Mostrar(AnalisisComparativo a)
        {
            panelAnalisis.Mostrar(a, _icc);
            lblEstado.Text = $"Proyecto N° {a.Proyecto.IdProyecto}: análisis al {a.Fecha:dd/MM/yyyy HH:mm}." +
                             (a.Advertencias.Count > 0 ? $" {a.Advertencias.Count} advertencia(s) en los supuestos." : "");
            btnAprobar.Enabled = btnRechazar.Enabled = btnInforme.Enabled = true;
        }

        // Comisiones de LandSmart del proyecto: al cambiarlas se recalcula el análisis
        private void btnComisiones_Click(object sender, EventArgs e)
        {
            ProyectoInversion p = Seleccionado();
            if (p == null) return;
            decimal[] valores = PN3Comun.PedirNumeros(this, "Comisiones de LandSmart",
                $"Comisiones sobre el fideicomiso del Proyecto N° {p.IdProyecto} ({p.Direccion}):",
                new[] { ("Estructuración (sobre el costo de obra):", p.PorcentajeEstructuracion),
                        ("Administración mensual (por mes de obra):", p.PorcentajeAdministracionMensual),
                        ("Venta de unidades de los inversores:", p.PorcentajeVentaUnidades) }, 0, 99, 2);
            if (valores == null) return;
            try
            {
                proyectoBLL.ActualizarComisiones(p.IdProyecto, valores[0], valores[1], valores[2]);
                _analisis.Remove(p.IdProyecto);
                CargarProyectos();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar las comisiones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Informe interno del Directorio con el análisis que se está viendo (mismos datos de mercado)
        private void btnInforme_Click(object sender, EventArgs e)
        {
            ProyectoInversion p = Seleccionado();
            if (p == null || !_analisis.TryGetValue(p.IdProyecto, out AnalisisComparativo a)) return;
            PN3Comun.GuardarPdf(this, $"Informe_Proyecto_{p.IdProyecto}.pdf", "Guardar informe de evaluación en PDF",
                ruta => new InformeInversionBLL().GenerarInformeDirectorio(a, GraficosPN3.GananciaPorEscenario(a), GraficosPN3.DistribucionGanancia(a), ruta));
        }

        private void btnAprobar_Click(object sender, EventArgs e)
        {
            ProyectoInversion p = Seleccionado();
            if (p == null) return;
            string observaciones = PN3Comun.PedirTexto(this, "Aprobar proyecto",
                $"Aprobar el Proyecto N° {p.IdProyecto} ({p.Direccion}). Observaciones (opcional):", 1000, multilinea: true);
            if (observaciones == null) return;
            Decidir(() => proyectoBLL.AprobarProyecto(p.IdProyecto, observaciones),
                $"Proyecto N° {p.IdProyecto} aprobado. Inversiones ya puede cargar los inversores del fideicomiso.");
        }

        private void btnRechazar_Click(object sender, EventArgs e)
        {
            ProyectoInversion p = Seleccionado();
            if (p == null) return;
            string motivo = PN3Comun.PedirTexto(this, "Rechazar proyecto",
                $"Motivo del rechazo del Proyecto N° {p.IdProyecto} ({p.Direccion}):", 1000, multilinea: true, obligatorio: true);
            if (motivo == null) return;
            Decidir(() => proyectoBLL.RechazarProyecto(p.IdProyecto, motivo), $"Proyecto N° {p.IdProyecto} rechazado.");
        }

        private void Decidir(Action decision, string exito)
        {
            try
            {
                decision();
                MessageBox.Show(exito, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarProyectos();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la decisión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
