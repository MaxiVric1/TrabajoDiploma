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
    public partial class FormContratosPorVencer : Form, IObservador
    {
        // Fila de la grilla (aplana contrato + inquilino + inmueble). Las propiedades [Browsable(false)] no son columnas.
        private class FilaPorVencer
        {
            public int NroContrato { get; set; }
            public string Inquilino { get; set; }
            public string Direccion { get; set; }
            public string Tipo { get; set; }
            public decimal MontoMensual { get; set; }
            public DateTime FechaFin { get; set; }
            public int DiasRestantes { get; set; }
            public string Estado { get; set; }
            public string EstadoPago { get; set; }
            public string Desocupacion { get; set; }
            [Browsable(false)] public Contrato Contrato { get; set; }
            [Browsable(false)] public bool ConDeuda { get; set; }
        }

        private ContratoBLL contratoBLL = new ContratoBLL();
        private PagoAlquilerBLL pagoBLL = new PagoAlquilerBLL();
        private InquilinoBLL inquilinoBLL = new InquilinoBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private List<DataGridTraducible> _grillasTraducibles = new List<DataGridTraducible>();
        private bool _cargando; // evita refrescar el detalle mientras se rearma la grilla

        // Leídos en cada carga de la grilla y reutilizados por el detalle
        private Dictionary<int, List<PagoAlquiler>> _pagosPorContrato = new Dictionary<int, List<PagoAlquiler>>();

        public FormContratosPorVencer()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormContratosPorVencer"));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnActualizar, BE.NombreControl.FormContratosPorVencer_btnActualizar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpInquilino, BE.NombreControl.FormContratosPorVencer_grpInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNombre, BE.NombreControl.FormContratosPorVencer_lblNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblApellido, BE.NombreControl.FormContratosPorVencer_lblApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDni, BE.NombreControl.FormContratosPorVencer_lblDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTelefono, BE.NombreControl.FormContratosPorVencer_lblTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEmail, BE.NombreControl.FormContratosPorVencer_lblEmail));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpAcciones, BE.NombreControl.FormContratosPorVencer_grpAcciones));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblObservacion, BE.NombreControl.FormContratosPorVencer_lblObservacion));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRenovacion, BE.NombreControl.FormContratosPorVencer_btnRenovacion));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnDesocupacion, BE.NombreControl.FormContratosPorVencer_btnDesocupacion));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpContrato, BE.NombreControl.FormContratosPorVencer_grpContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNroContrato, BE.NombreControl.FormContratosPorVencer_lblNroContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDireccion, BE.NombreControl.FormContratosPorVencer_lblDireccion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTipo, BE.NombreControl.FormContratosPorVencer_lblTipo));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaInicio, BE.NombreControl.FormContratosPorVencer_lblFechaInicio));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDuracion, BE.NombreControl.FormContratosPorVencer_lblDuracion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaFin, BE.NombreControl.FormContratosPorVencer_lblFechaFin));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDiasRestantes, BE.NombreControl.FormContratosPorVencer_lblDiasRestantes));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMonto, BE.NombreControl.FormContratosPorVencer_lblMonto));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEstado, BE.NombreControl.FormContratosPorVencer_lblEstado));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblAlerta, BE.NombreControl.FormContratosPorVencer_lblAlerta));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPagoActual, BE.NombreControl.FormContratosPorVencer_lblPagoActual));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDeuda, BE.NombreControl.FormContratosPorVencer_lblDeuda));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDesocupacion, BE.NombreControl.FormContratosPorVencer_lblDesocupacion));

            _grillasTraducibles.Clear();
            _grillasTraducibles.Add(new DataGridTraducible(dgvContratos, "FormContratosPorVencer.dgvContratos")
                .ConColumna("NroContrato", BE.NombreControl.FormContratosPorVencer_dgvContratos_nro)
                .ConColumna("Inquilino", BE.NombreControl.FormContratosPorVencer_dgvContratos_inquilino)
                .ConColumna("Direccion", BE.NombreControl.FormContratosPorVencer_dgvContratos_direccion)
                .ConColumna("Tipo", BE.NombreControl.FormContratosPorVencer_dgvContratos_tipo)
                .ConColumna("MontoMensual", BE.NombreControl.FormContratosPorVencer_dgvContratos_monto)
                .ConColumna("FechaFin", BE.NombreControl.FormContratosPorVencer_dgvContratos_fechaFin)
                .ConColumna("DiasRestantes", BE.NombreControl.FormContratosPorVencer_dgvContratos_diasRestantes)
                .ConColumna("Estado", BE.NombreControl.FormContratosPorVencer_dgvContratos_estado)
                .ConColumna("EstadoPago", BE.NombreControl.FormContratosPorVencer_dgvContratos_estadoPago)
                .ConColumna("Desocupacion", BE.NombreControl.FormContratosPorVencer_dgvContratos_desocupacion));
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
            foreach (var grilla in _grillasTraducibles)
            {
                grilla.Traducir(traducciones);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void TraducirGrillas()
        {
            var traducciones = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
            if (traducciones == null) return;
            foreach (var grilla in _grillasTraducibles)
            {
                grilla.Traducir(traducciones);
            }
        }

        private void FormContratosPorVencer_Load(object sender, EventArgs e)
        {
            ActualizarEstados();
            CargarContratos();
        }

        // Mora/litigio y vencimientos al día antes de listar: así la lista coincide con alerta_vencimiento = true
        private void ActualizarEstados()
        {
            lblActualizacion.Text = EstadosPN2.EjecutarProcesosAutomaticos();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            int? seleccionado = ContratoSeleccionado()?.IdContrato;
            ActualizarEstados();
            CargarContratos(seleccionado);
        }

        private void CargarContratos(int? idASeleccionar = null)
        {
            _cargando = true;
            try
            {
                DateTime hoy = DateTime.Now;
                _pagosPorContrato = pagoBLL.LeerTodos().GroupBy(p => p.IdContrato).ToDictionary(g => g.Key, g => g.ToList());

                List<FilaPorVencer> filas = contratoBLL.LeerPorVencer().Select(c => new FilaPorVencer
                {
                    NroContrato = c.IdContrato,
                    Inquilino = c.Inquilino?.NombreCompleto,
                    Direccion = c.InmuebleAlquiler?.Direccion,
                    Tipo = c.InmuebleAlquiler?.TipoInmueble.ToString(),
                    MontoMensual = c.MontoMensual,
                    FechaFin = c.FechaFin,
                    DiasRestantes = ContratoBLL.DiasRestantes(c, hoy),
                    Estado = EstadosPN2.Texto(c.EstadoContrato),
                    EstadoPago = EstadosPN2.EstadoMesActual(c, PagosDe(c.IdContrato), hoy),
                    Desocupacion = c.AvisoDesocupacion ? "Avisada" : "",
                    Contrato = c,
                    ConDeuda = PagosDe(c.IdContrato).Any(p => p.EstadoPago == BE.EstadoPago.EnMora || p.EstadoPago == BE.EstadoPago.EnLitigio)
                }).OrderBy(f => f.DiasRestantes).ToList();

                dgvContratos.DataSource = null;
                dgvContratos.DataSource = filas;

                dgvContratos.Columns["MontoMensual"].DefaultCellStyle.Format = "N2";
                dgvContratos.Columns["MontoMensual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvContratos.Columns["FechaFin"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvContratos.Columns["DiasRestantes"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                TraducirGrillas();

                foreach (DataGridViewRow row in dgvContratos.Rows)
                {
                    FilaPorVencer fila = (FilaPorVencer)row.DataBoundItem;
                    EstadosPN2.Resaltar(row, fila.ConDeuda, true);
                    if (idASeleccionar.HasValue && fila.NroContrato == idASeleccionar.Value)
                    {
                        row.Selected = true;
                        dgvContratos.CurrentCell = row.Cells["NroContrato"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los contratos por vencer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
            }

            MostrarDetalle();
        }

        private List<PagoAlquiler> PagosDe(int idContrato)
        {
            return _pagosPorContrato.TryGetValue(idContrato, out var pagos) ? pagos : new List<PagoAlquiler>();
        }

        private void dgvContratos_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarDetalle();
        }

        private FilaPorVencer FilaSeleccionada()
        {
            return dgvContratos.CurrentRow?.DataBoundItem as FilaPorVencer;
        }

        private Contrato ContratoSeleccionado()
        {
            return FilaSeleccionada()?.Contrato;
        }

        private void MostrarDetalle()
        {
            Contrato contrato = ContratoSeleccionado();
            foreach (Label valor in new[] { lblNombreValor, lblApellidoValor, lblDniValor, lblTelefonoValor, lblEmailValor,
                lblNroContratoValor, lblDireccionValor, lblTipoValor, lblFechaInicioValor, lblDuracionValor, lblFechaFinValor,
                lblDiasRestantesValor, lblMontoValor, lblEstadoValor, lblAlertaValor, lblPagoActualValor, lblDeudaValor, lblDesocupacionValor })
            {
                valor.Text = "-";
            }

            btnRenovacion.Enabled = contrato != null;
            btnDesocupacion.Enabled = contrato != null && !contrato.AvisoDesocupacion;
            if (contrato == null) return;

            DateTime hoy = DateTime.Now;
            List<PagoAlquiler> pagos = PagosDe(contrato.IdContrato);
            int diasRestantes = ContratoBLL.DiasRestantes(contrato, hoy);

            lblNroContratoValor.Text = contrato.IdContrato.ToString();
            lblDireccionValor.Text = contrato.InmuebleAlquiler?.Direccion ?? "-";
            lblTipoValor.Text = contrato.InmuebleAlquiler?.TipoInmueble.ToString() ?? "-";
            lblFechaInicioValor.Text = contrato.FechaInicio.ToString("dd/MM/yyyy");
            lblDuracionValor.Text = $"{contrato.DuracionMeses} meses";
            lblFechaFinValor.Text = contrato.FechaFin.ToString("dd/MM/yyyy");
            lblDiasRestantesValor.Text = diasRestantes == 0 ? "Vence hoy" : $"{diasRestantes} días";
            lblMontoValor.Text = $"${contrato.MontoMensual:N2}";
            lblEstadoValor.Text = EstadosPN2.Texto(contrato.EstadoContrato);
            lblAlertaValor.Text = contrato.AlertaVencimiento ? "Activa" : "Inactiva";
            lblPagoActualValor.Text = EstadosPN2.DescribirMesActual(contrato, pagos, hoy);

            int enMora = pagos.Count(p => p.EstadoPago == EstadoPago.EnMora);
            int enLitigio = pagos.Count(p => p.EstadoPago == EstadoPago.EnLitigio);
            lblDeudaValor.Text = enMora + enLitigio == 0 ? "Sin deuda" : $"{enMora} mes(es) En Mora, {enLitigio} En Litigio";

            lblDesocupacionValor.Text = contrato.AvisoDesocupacion
                ? $"Avisada el {contrato.FechaAvisoDesocupacion:dd/MM/yyyy}: el inquilino desocupa al vencimiento ({contrato.FechaFin:dd/MM/yyyy})"
                : "Sin aviso";

            try
            {
                Inquilino inquilino = inquilinoBLL.BuscarPorId(contrato.IdInquilino);
                if (inquilino != null)
                {
                    lblNombreValor.Text = inquilino.Nombre;
                    lblApellidoValor.Text = inquilino.Apellido;
                    lblDniValor.Text = inquilino.Dni;
                    lblTelefonoValor.Text = string.IsNullOrEmpty(inquilino.Telefono) ? "-" : inquilino.Telefono;
                    lblEmailValor.Text = string.IsNullOrEmpty(inquilino.Email) ? "-" : inquilino.Email;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del inquilino: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRenovacion_Click(object sender, EventArgs e)
        {
            FilaPorVencer fila = FilaSeleccionada();
            if (fila == null) return;

            using (var form = new FormRenovarContrato(fila.Contrato, fila.ConDeuda))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;
            }
            // Si la renovación lo saca del período de alerta, el contrato deja de figurar en la lista
            CargarContratos(fila.Contrato.IdContrato);
        }

        private void btnDesocupacion_Click(object sender, EventArgs e)
        {
            Contrato contrato = ContratoSeleccionado();
            if (contrato == null) return;

            string confirmacion =
                $"¿Confirma que el inquilino {contrato.Inquilino?.NombreCompleto} desocupará el inmueble al vencimiento del " +
                $"Contrato N° {contrato.IdContrato}?\n\n" +
                $"Dirección: {contrato.InmuebleAlquiler?.Direccion}\n" +
                $"Vencimiento: {contrato.FechaFin:dd/MM/yyyy}\n\n" +
                "El contrato sigue vigente hasta esa fecha.";
            if (MessageBox.Show(confirmacion, "Confirmar desocupación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                contratoBLL.RegistrarDesocupacion(contrato.IdContrato, txtObservacion.Text);
                MessageBox.Show($"Desocupación registrada para el Contrato N° {contrato.IdContrato}.\n" +
                    $"El contrato sigue vigente hasta el {contrato.FechaFin:dd/MM/yyyy}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtObservacion.Clear();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la desocupación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CargarContratos(contrato.IdContrato);
        }
    }
}
