using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // Ranking de agentes comerciales (Admin y Directorio): los 5 que más o menos vendieron en un período, por
    // cantidad de propiedades o por valor total en USD, con exportación a PDF
    public partial class FormRankingAgentes : Form, IObservador
    {
        private class FilaRanking
        {
            public int Posicion { get; set; }
            public string Dni { get; set; }
            public string Agente { get; set; }
            public int Cantidad { get; set; }
            public decimal TotalUsd { get; set; }
        }

        // Opción de un combo: el texto se traduce con la clave (Formulario.control)
        private interface IOpcion
        {
            string Clave { get; }
            string Texto { get; set; }
        }

        private class Opcion<T> : IOpcion
        {
            public T Valor { get; set; }
            public string Clave { get; set; }
            public string Texto { get; set; }
            public override string ToString() => Texto;
        }

        private readonly VentaBLL ventaBLL = new VentaBLL();
        private readonly List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private readonly DataGridTraducible _grilla;
        private ResultadoRanking _resultado;
        private bool _cargando;
        private Dictionary<string, string> _traducciones;

        public FormRankingAgentes()
        {
            InitializeComponent();
            PN3Comun.EstiloGrilla(dgvRanking);
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormRankingAgentes"));
            foreach (var (control, clave) in new (Control, string)[] {
                (grpFiltros, BE.NombreControl.FormRankingAgentes_grpFiltros),
                (lblPeriodo, BE.NombreControl.FormRankingAgentes_lblPeriodo),
                (lblDesde, BE.NombreControl.FormRankingAgentes_lblDesde),
                (lblHasta, BE.NombreControl.FormRankingAgentes_lblHasta),
                (lblOrden, BE.NombreControl.FormRankingAgentes_lblOrden),
                (lblVista, BE.NombreControl.FormRankingAgentes_lblVista),
                (rbMasVendieron, BE.NombreControl.FormRankingAgentes_rbMasVendieron),
                (rbMenosVendieron, BE.NombreControl.FormRankingAgentes_rbMenosVendieron),
                (btnExportarPdf, BE.NombreControl.FormRankingAgentes_btnExportarPdf) })
                _controlesTraducibles.Add(new EtiquetaTraducible(control, clave));
            _grilla = new DataGridTraducible(dgvRanking, "FormRankingAgentes.dgvRanking")
                .ConColumna("Posicion", BE.NombreControl.FormRankingAgentes_dgvRanking_posicion)
                .ConColumna("Dni", BE.NombreControl.FormRankingAgentes_dgvRanking_dni)
                .ConColumna("Agente", BE.NombreControl.FormRankingAgentes_dgvRanking_agente)
                .ConColumna("Cantidad", BE.NombreControl.FormRankingAgentes_dgvRanking_cantidad)
                .ConColumna("TotalUsd", BE.NombreControl.FormRankingAgentes_dgvRanking_totalUsd);
            _controlesTraducibles.Add(_grilla);
            GestorIdioma.Instancia.Adjuntar(this);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles) control.Traducir(traducciones);
            _traducciones = traducciones;
            TraducirOpciones(cmbPeriodo);
            TraducirOpciones(cmbOrden);
        }

        // Vuelve a cargar los ítems con el texto traducido, conservando la selección
        private void TraducirOpciones(ComboBox combo)
        {
            if (_traducciones == null || combo.Items.Count == 0) return;
            object[] items = combo.Items.Cast<object>().ToArray();
            foreach (IOpcion o in items)
                if (_traducciones.TryGetValue(o.Clave, out string texto) && !string.IsNullOrWhiteSpace(texto)) o.Texto = texto;
            bool cargando = _cargando;
            _cargando = true;
            int seleccionado = combo.SelectedIndex;
            combo.BeginUpdate();
            combo.Items.Clear();
            combo.Items.AddRange(items);
            combo.SelectedIndex = seleccionado;
            combo.EndUpdate();
            _cargando = cargando;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormRankingAgentes_Load(object sender, EventArgs e)
        {
            _cargando = true;
            cmbPeriodo.Items.AddRange(new (PeriodoRanking, string)[] {
                (PeriodoRanking.EsteMes, BE.NombreControl.FormRankingAgentes_cmbPeriodo_esteMes),
                (PeriodoRanking.EsteTrimestre, BE.NombreControl.FormRankingAgentes_cmbPeriodo_esteTrimestre),
                (PeriodoRanking.EsteAnio, BE.NombreControl.FormRankingAgentes_cmbPeriodo_esteAnio),
                (PeriodoRanking.Todo, BE.NombreControl.FormRankingAgentes_cmbPeriodo_todo),
                (PeriodoRanking.Personalizado, BE.NombreControl.FormRankingAgentes_cmbPeriodo_personalizado) }
                .Select(p => (object)new Opcion<PeriodoRanking> { Valor = p.Item1, Clave = p.Item2, Texto = VentaBLL.Texto(p.Item1) }).ToArray());
            cmbOrden.Items.AddRange(new object[] {
                new Opcion<OrdenRanking> { Valor = OrdenRanking.Cantidad, Clave = BE.NombreControl.FormRankingAgentes_cmbOrden_cantidad, Texto = "Cantidad de propiedades" },
                new Opcion<OrdenRanking> { Valor = OrdenRanking.ValorTotal, Clave = BE.NombreControl.FormRankingAgentes_cmbOrden_valorTotal, Texto = "Valor total (USD)" } });
            cmbPeriodo.SelectedIndex = 2; // Este año
            cmbOrden.SelectedIndex = 0;
            TraducirOpciones(cmbPeriodo);
            TraducirOpciones(cmbOrden);
            dtpDesde.MaxDate = dtpHasta.MaxDate = DateTime.Today;
            MostrarFechasDelPeriodo();
            PintarVista();
            _cargando = false;
            CargarRanking();
        }

        private PeriodoRanking Periodo => ((Opcion<PeriodoRanking>)cmbPeriodo.SelectedItem).Valor;
        private OrdenRanking Orden => ((Opcion<OrdenRanking>)cmbOrden.SelectedItem).Valor;

        // Un período predefinido muestra sus fechas (no editables); Personalizado habilita Desde/Hasta
        private void MostrarFechasDelPeriodo()
        {
            bool personalizado = Periodo == PeriodoRanking.Personalizado;
            dtpDesde.Enabled = dtpHasta.Enabled = personalizado;
            if (personalizado) return;
            var (desde, hasta) = VentaBLL.Rango(Periodo, DateTime.Today);
            dtpDesde.Value = desde ?? new DateTime(DateTime.Today.Year - 5, 1, 1);
            dtpHasta.Value = hasta ?? DateTime.Today;
        }

        private void Filtros_Changed(object sender, EventArgs e)
        {
            if (_cargando) return;
            if (sender == cmbPeriodo)
            {
                _cargando = true;
                MostrarFechasDelPeriodo();
                _cargando = false;
            }
            CargarRanking();
        }

        // Los dos botones de la vista funcionan como un interruptor: el elegido queda resaltado
        private void Vista_CheckedChanged(object sender, EventArgs e)
        {
            PintarVista();
            if (!_cargando && ((RadioButton)sender).Checked) CargarRanking();
        }

        private void PintarVista()
        {
            foreach (RadioButton rb in new[] { rbMasVendieron, rbMenosVendieron })
            {
                rb.BackColor = rb.Checked ? PN3Comun.BOTON : PN3Comun.CAMPO;
                rb.FlatAppearance.BorderColor = rb.Checked ? PN3Comun.VERDE : PN3Comun.CAMPO;
                rb.Font = new System.Drawing.Font(rb.Font, rb.Checked ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular);
            }
        }

        private void CargarRanking()
        {
            try
            {
                _resultado = ventaBLL.ObtenerRanking(Periodo, dtpDesde.Value.Date, dtpHasta.Value.Date, Orden, rbMasVendieron.Checked);
                dgvRanking.DataSource = _resultado.Filas.Select(f => new FilaRanking
                {
                    Posicion = f.Posicion,
                    Dni = f.Dni ?? "-",
                    Agente = f.NombreCompleto,
                    Cantidad = f.Cantidad,
                    TotalUsd = f.TotalUsd
                }).ToList();
                PN3Comun.FormatoColumnas(dgvRanking, new[] { "TotalUsd" });
                dgvRanking.Columns["Posicion"].FillWeight = 40;
                dgvRanking.Columns["Dni"].FillWeight = 80;
                dgvRanking.Columns["Agente"].FillWeight = 160;
                _grilla.Traducir(GestorIdioma.Instancia.ObtenerTraduccionesActuales());
                if (dgvRanking.Rows.Count > 0)
                    dgvRanking.Rows[0].DefaultCellStyle.ForeColor = _resultado.MasVendieron ? PN3Comun.VERDE : PN3Comun.AMARILLO;
                lblResumen.Text = $"{VentaBLL.TextoPeriodo(_resultado)} - {_resultado.VentasDelPeriodo} propiedad(es) vendida(s) por " +
                                  $"{PN3Comun.Usd(_resultado.TotalUsdDelPeriodo)} entre {_resultado.TotalAgentes} agentes comerciales.";
                btnExportarPdf.Enabled = true;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                MostrarError(ex.Message);
            }
            catch (Exception ex)
            {
                MostrarError("Error al calcular el ranking: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            _resultado = null;
            dgvRanking.DataSource = null;
            lblResumen.Text = mensaje;
            btnExportarPdf.Enabled = false;
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (_resultado == null) return;
            ResultadoRanking resultado = _resultado;
            PN3Comun.GuardarPdf(this, $"Ranking_Agentes_{DateTime.Today:yyyyMMdd}.pdf", "Guardar ranking de agentes en PDF",
                ruta => ventaBLL.GenerarPdf(resultado, ruta));
        }
    }
}
