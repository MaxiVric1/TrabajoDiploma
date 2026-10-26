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
    // PN3 - Inversiones: fideicomisos de los proyectos Aprobados. Tablero del fondeo (capital suscripto contra el
    // necesario, inversores y su participación) que se refresca solo cada 30 segundos, con el dólar del día.
    public partial class FormGestionFideicomiso : Form, IObservador
    {
        public const int SEGUNDOS_REFRESCO = 30;

        private class FilaProyecto
        {
            public int NroProyecto { get; set; }
            public string Direccion { get; set; }
            public string Localidad { get; set; }
            public decimal CapitalNecesario { get; set; }
            public decimal CapitalSuscripto { get; set; }
            public decimal Fondeado { get; set; }
            public int Inversores { get; set; }
        }

        private class FilaInversor
        {
            public string Inversor { get; set; }
            public string Cuit { get; set; }
            public decimal Monto { get; set; }
            public decimal? Participacion { get; set; }
            public int Unidades { get; set; }
            public DateTime Fecha { get; set; }
        }

        private ProyectoInversionBLL proyectoBLL = new ProyectoInversionBLL();
        private SuscripcionFideicomisoBLL suscripcionBLL = new SuscripcionFideicomisoBLL();
        private CotizacionesBLL cotizacionesBLL = new CotizacionesBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _grillaProyectos, _grillaInversores;
        private readonly Timer _refresco = new Timer { Interval = SEGUNDOS_REFRESCO * 1000 };
        private ResumenFideicomiso _resumen;
        private bool _cargando;

        public FormGestionFideicomiso()
        {
            InitializeComponent();
            PN3Comun.EstiloGrilla(dgvProyectos);
            PN3Comun.EstiloGrilla(dgvInversores);
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormGestionFideicomiso"));
            foreach (var (control, clave) in new (Control, string)[] {
                (grpTablero, BE.NombreControl.FormGestionFideicomiso_grpTablero),
                (lblNecesario, BE.NombreControl.FormGestionFideicomiso_lblNecesario),
                (lblSuscripto, BE.NombreControl.FormGestionFideicomiso_lblSuscripto),
                (lblRestante, BE.NombreControl.FormGestionFideicomiso_lblRestante),
                (lblDolarMep, BE.NombreControl.FormGestionFideicomiso_lblDolarMep),
                (lblDolarBlue, BE.NombreControl.FormGestionFideicomiso_lblDolarBlue),
                (lblRestantePesos, BE.NombreControl.FormGestionFideicomiso_lblRestantePesos),
                (grpInversores, BE.NombreControl.FormGestionFideicomiso_grpInversores),
                (btnAgregarInversor, BE.NombreControl.FormGestionFideicomiso_btnAgregarInversor) })
                _controlesTraducibles.Add(new EtiquetaTraducible(control, clave));
            _grillaProyectos = new DataGridTraducible(dgvProyectos, "FormGestionFideicomiso.dgvProyectos")
                .ConColumna("NroProyecto", BE.NombreControl.FormGestionFideicomiso_dgvProyectos_nroProyecto)
                .ConColumna("Direccion", BE.NombreControl.FormGestionFideicomiso_dgvProyectos_direccion)
                .ConColumna("Localidad", BE.NombreControl.FormGestionFideicomiso_dgvProyectos_localidad)
                .ConColumna("CapitalNecesario", BE.NombreControl.FormGestionFideicomiso_dgvProyectos_capitalNecesario)
                .ConColumna("CapitalSuscripto", BE.NombreControl.FormGestionFideicomiso_dgvProyectos_capitalSuscripto)
                .ConColumna("Fondeado", BE.NombreControl.FormGestionFideicomiso_dgvProyectos_fondeado)
                .ConColumna("Inversores", BE.NombreControl.FormGestionFideicomiso_dgvProyectos_inversores);
            _grillaInversores = new DataGridTraducible(dgvInversores, "FormGestionFideicomiso.dgvInversores")
                .ConColumna("Inversor", BE.NombreControl.FormGestionFideicomiso_dgvInversores_inversor)
                .ConColumna("Cuit", BE.NombreControl.FormGestionFideicomiso_dgvInversores_cuit)
                .ConColumna("Monto", BE.NombreControl.FormGestionFideicomiso_dgvInversores_monto)
                .ConColumna("Participacion", BE.NombreControl.FormGestionFideicomiso_dgvInversores_participacion)
                .ConColumna("Unidades", BE.NombreControl.FormGestionFideicomiso_dgvInversores_unidades)
                .ConColumna("Fecha", BE.NombreControl.FormGestionFideicomiso_dgvInversores_fecha);
            _controlesTraducibles.Add(_grillaProyectos);
            _controlesTraducibles.Add(_grillaInversores);
            _refresco.Tick += (s, e) => Refrescar();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles) control.Traducir(traducciones);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _refresco.Stop();
            _refresco.Dispose();
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormGestionFideicomiso_Load(object sender, EventArgs e)
        {
            barFondeo.Maximum = 10000; // centésimas de punto
            CargarProyectos();
            _refresco.Start();
        }

        // Refresco automático: otro usuario puede haber cargado aportes; el dólar se renueva cada pocos minutos
        private void Refrescar()
        {
            CargarProyectos(Seleccionado());
        }

        private void CargarProyectos(int? idASeleccionar = null)
        {
            _cargando = true;
            try
            {
                List<FilaProyecto> filas = proyectoBLL.LeerTodos(EstadoProyecto.Aprobado).Select(p => new FilaProyecto
                {
                    NroProyecto = p.IdProyecto,
                    Direccion = p.Direccion,
                    Localidad = p.Localidad,
                    CapitalNecesario = p.CapitalNecesario,
                    CapitalSuscripto = p.CapitalSuscripto,
                    Fondeado = p.CapitalNecesario > 0 ? Math.Round(p.CapitalSuscripto * 100 / p.CapitalNecesario, 2) : 0,
                    Inversores = p.CantidadInversores
                }).ToList();
                dgvProyectos.DataSource = null;
                dgvProyectos.DataSource = filas;
                PN3Comun.FormatoColumnas(dgvProyectos, new[] { "CapitalNecesario", "CapitalSuscripto", "Fondeado" });
                _grillaProyectos.Traducir(GestorIdioma.Instancia.ObtenerTraduccionesActuales());
                foreach (DataGridViewRow fila in dgvProyectos.Rows)
                {
                    FilaProyecto f = (FilaProyecto)fila.DataBoundItem;
                    if (f.Fondeado >= 100) fila.DefaultCellStyle.ForeColor = PN3Comun.VERDE;
                    if (idASeleccionar == f.NroProyecto) dgvProyectos.CurrentCell = fila.Cells["NroProyecto"];
                }
            }
            catch (Exception ex)
            {
                lblActualizacion.Text = "Error al actualizar: " + ex.Message;
            }
            finally { _cargando = false; }
            MostrarTablero();
        }

        private int? Seleccionado() => (dgvProyectos.CurrentRow?.DataBoundItem as FilaProyecto)?.NroProyecto;

        private void dgvProyectos_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarTablero();
        }

        private void MostrarTablero()
        {
            int? id = Seleccionado();
            btnAgregarInversor.Enabled = id.HasValue;
            dgvInversores.DataSource = null;
            if (!id.HasValue)
            {
                _resumen = null;
                lblProyectoValor.Text = dgvProyectos.Rows.Count == 0 ? "No hay proyectos aprobados por el Directorio." : "-";
                barFondeo.Value = 0;
                foreach (Label l in new[] { lblPorcentajeValor, lblNecesarioValor, lblSuscriptoValor, lblRestanteValor, lblDolarMepValor, lblDolarBlueValor, lblRestantePesosValor })
                    l.Text = "-";
                return;
            }
            try
            {
                _resumen = suscripcionBLL.ObtenerResumenFideicomiso(id.Value);
                ProyectoInversion p = _resumen.Proyecto;
                lblProyectoValor.Text = $"{p} - {p.MetrosConstruibles:N2} m² construibles - unidades {PN3Comun.Usd(p.ValorTotalUnidades)}";
                barFondeo.Value = (int)Math.Min(barFondeo.Maximum, _resumen.PorcentajeFondeado * 100);
                lblPorcentajeValor.Text = $"{_resumen.PorcentajeFondeado:N2}%";
                lblPorcentajeValor.ForeColor = _resumen.Fondeado ? PN3Comun.VERDE : System.Drawing.Color.White;
                lblNecesarioValor.Text = PN3Comun.Usd(_resumen.CapitalNecesario);
                lblSuscriptoValor.Text = $"{PN3Comun.Usd(_resumen.CapitalSuscripto)} ({_resumen.Suscripciones.Count} aporte(s))";
                lblRestanteValor.Text = PN3Comun.Usd(_resumen.CapitalRestante);

                CotizacionDolar mep = cotizacionesBLL.Mep(), blue = cotizacionesBLL.Blue();
                lblDolarMepValor.Text = mep?.Venta != null ? $"$ {mep.Venta:N2} ({mep.FechaActualizacion:dd/MM HH:mm})" : "Sin datos de DolarAPI";
                lblDolarBlueValor.Text = blue?.Venta != null ? $"$ {blue.Venta:N2} ({blue.FechaActualizacion:dd/MM HH:mm})" : "Sin datos de DolarAPI";
                lblRestantePesosValor.Text = mep?.Venta != null ? $"{PN3Comun.Pesos(_resumen.CapitalRestante * mep.Venta.Value)} al MEP" : "-";

                dgvInversores.DataSource = _resumen.Suscripciones.Select(s => new FilaInversor
                {
                    Inversor = s.NombreInversor,
                    Cuit = s.Cuit,
                    Monto = s.MontoAportado,
                    Participacion = s.PorcentajeParticipacion,
                    Unidades = s.UnidadesAsignadas,
                    Fecha = s.FechaSuscripcion
                }).ToList();
                PN3Comun.FormatoColumnas(dgvInversores, new[] { "Monto", "Participacion" }, new[] { "Fecha" });
                _grillaInversores.Traducir(GestorIdioma.Instancia.ObtenerTraduccionesActuales());
                lblActualizacion.Text = $"Actualizado a las {DateTime.Now:HH:mm:ss} (se refresca cada {SEGUNDOS_REFRESCO} segundos)" +
                                        (cotizacionesBLL.UltimoError != null ? " - DolarAPI sin conexión" : "");
            }
            catch (Exception ex)
            {
                lblActualizacion.Text = "Error al actualizar el tablero: " + ex.Message;
            }
        }

        private void btnAgregarInversor_Click(object sender, EventArgs e)
        {
            if (_resumen == null) return;
            if (_resumen.Fondeado)
            {
                MessageBox.Show("El fideicomiso ya tiene suscripto todo el capital necesario.", "Gestión del Fideicomiso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            _refresco.Stop();
            try
            {
                using (FormAgregarSuscripcion frm = new FormAgregarSuscripcion(_resumen))
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                        CargarProyectos(_resumen.Proyecto.IdProyecto);
                }
            }
            finally { _refresco.Start(); }
        }
    }
}
