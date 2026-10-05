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
    public partial class FormGestionarLitigios : Form, IObservador
    {
        // Filas de las grillas. Las propiedades [Browsable(false)] no son columnas.
        private class FilaLitigio
        {
            public int NroContrato { get; set; }
            public string Inquilino { get; set; }
            public string Direccion { get; set; }
            public string Periodo { get; set; }
            public decimal MontoAdeudado { get; set; }
            public int DiasEnLitigio { get; set; }
            [Browsable(false)] public PagoAlquiler Pago { get; set; }
        }

        private class FilaAccion
        {
            public DateTime Fecha { get; set; }
            public string Tipo { get; set; }
            public string Descripcion { get; set; }
        }

        private PagoAlquilerBLL pagoBLL = new PagoAlquilerBLL();
        private ContratoBLL contratoBLL = new ContratoBLL();
        private InquilinoBLL inquilinoBLL = new InquilinoBLL();
        private AccionLegalBLL accionBLL = new AccionLegalBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private List<DataGridTraducible> _grillasTraducibles = new List<DataGridTraducible>();
        private List<FilaLitigio> _filas = new List<FilaLitigio>();
        private Contrato _contrato; // contrato del pago seleccionado
        private bool _cargando;     // evita refrescar el detalle mientras se rearma la grilla

        public FormGestionarLitigios()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormGestionarLitigios"));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpInquilino, BE.NombreControl.FormGestionarLitigios_grpInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNombre, BE.NombreControl.FormGestionarLitigios_lblNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblApellido, BE.NombreControl.FormGestionarLitigios_lblApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDni, BE.NombreControl.FormGestionarLitigios_lblDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTelefono, BE.NombreControl.FormGestionarLitigios_lblTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEmail, BE.NombreControl.FormGestionarLitigios_lblEmail));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpContrato, BE.NombreControl.FormGestionarLitigios_grpContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDireccion, BE.NombreControl.FormGestionarLitigios_lblDireccion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMontoMensual, BE.NombreControl.FormGestionarLitigios_lblMontoMensual));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaFin, BE.NombreControl.FormGestionarLitigios_lblFechaFin));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDiasLitigio, BE.NombreControl.FormGestionarLitigios_lblDiasLitigio));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMontoAdeudado, BE.NombreControl.FormGestionarLitigios_lblMontoAdeudado));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDeudaTotal, BE.NombreControl.FormGestionarLitigios_lblDeudaTotal));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpAcciones, BE.NombreControl.FormGestionarLitigios_grpAcciones));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaAccion, BE.NombreControl.FormGestionarLitigios_lblFechaAccion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDescripcion, BE.NombreControl.FormGestionarLitigios_lblDescripcion));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnIntimacion, BE.NombreControl.FormGestionarLitigios_btnIntimacion));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnResuelto, BE.NombreControl.FormGestionarLitigios_btnResuelto));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnDesalojo, BE.NombreControl.FormGestionarLitigios_btnDesalojo));

            _grillasTraducibles.Clear();
            _grillasTraducibles.Add(new DataGridTraducible(dgvPagos, "FormGestionarLitigios.dgvPagos")
                .ConColumna("NroContrato", BE.NombreControl.FormGestionarLitigios_dgvPagos_nro)
                .ConColumna("Inquilino", BE.NombreControl.FormGestionarLitigios_dgvPagos_inquilino)
                .ConColumna("Direccion", BE.NombreControl.FormGestionarLitigios_dgvPagos_direccion)
                .ConColumna("Periodo", BE.NombreControl.FormGestionarLitigios_dgvPagos_periodo)
                .ConColumna("MontoAdeudado", BE.NombreControl.FormGestionarLitigios_dgvPagos_montoAdeudado)
                .ConColumna("DiasEnLitigio", BE.NombreControl.FormGestionarLitigios_dgvPagos_diasLitigio));
            _grillasTraducibles.Add(new DataGridTraducible(dgvAcciones, "FormGestionarLitigios.dgvAcciones")
                .ConColumna("Fecha", BE.NombreControl.FormGestionarLitigios_dgvAcciones_fecha)
                .ConColumna("Tipo", BE.NombreControl.FormGestionarLitigios_dgvAcciones_tipo)
                .ConColumna("Descripcion", BE.NombreControl.FormGestionarLitigios_dgvAcciones_descripcion));
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

        private void FormGestionarLitigios_Load(object sender, EventArgs e)
        {
            dtpFechaAccion.Value = DateTime.Today;
            dtpFechaAccion.MaxDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            lblActualizacion.Text = EstadosPN2.EjecutarProcesosAutomaticos();
            CargarPagos();
        }

        // Pagos En Litigio, de los más antiguos a los más recientes
        private void CargarPagos(int? idPagoASeleccionar = null)
        {
            _cargando = true;
            try
            {
                DateTime hoy = DateTime.Now;
                _filas = pagoBLL.LeerDetalle(EstadoPago.EnLitigio).Select(p => new FilaLitigio
                {
                    NroContrato = p.IdContrato,
                    Inquilino = p.Contrato?.Inquilino?.NombreCompleto,
                    Direccion = p.Contrato?.InmuebleAlquiler?.Direccion,
                    Periodo = p.ToString(),
                    MontoAdeudado = (p.Contrato?.MontoMensual ?? 0) + p.RecargoAplicado,
                    DiasEnLitigio = PagoAlquilerBLL.DiasEnLitigio(p.MesCorrespondiente, p.AnioCorrespondiente, hoy),
                    Pago = p
                }).OrderByDescending(f => f.DiasEnLitigio).ThenBy(f => f.NroContrato).ToList();

                dgvPagos.DataSource = null;
                dgvPagos.DataSource = _filas;
                dgvPagos.Columns["MontoAdeudado"].DefaultCellStyle.Format = "N2";
                dgvPagos.Columns["MontoAdeudado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPagos.Columns["DiasEnLitigio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                TraducirGrillas();

                if (idPagoASeleccionar.HasValue)
                {
                    foreach (DataGridViewRow row in dgvPagos.Rows)
                    {
                        if (((FilaLitigio)row.DataBoundItem).Pago.IdPago == idPagoASeleccionar.Value)
                        {
                            row.Selected = true;
                            dgvPagos.CurrentCell = row.Cells["NroContrato"];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los pagos en litigio: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
            }

            MostrarDetalle();
        }

        private void dgvPagos_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarDetalle();
        }

        private FilaLitigio FilaSeleccionada() => dgvPagos.CurrentRow?.DataBoundItem as FilaLitigio;

        // Meses En Litigio del mismo contrato (lo que regulariza "Marcar Resuelto")
        private List<FilaLitigio> FilasDelContrato(int idContrato) => _filas.Where(f => f.NroContrato == idContrato).ToList();

        private void MostrarDetalle()
        {
            FilaLitigio fila = FilaSeleccionada();
            _contrato = null;
            lblNombreValor.Text = lblApellidoValor.Text = lblDniValor.Text = lblTelefonoValor.Text = lblEmailValor.Text = "-";
            lblDireccionValor.Text = lblMontoMensualValor.Text = lblFechaFinValor.Text = "-";
            lblDiasLitigioValor.Text = lblMontoAdeudadoValor.Text = lblDeudaTotalValor.Text = "-";
            dgvAcciones.DataSource = null;
            grpContrato.Enabled = grpAcciones.Enabled = fila != null;
            if (fila == null) return;

            PagoAlquiler pago = fila.Pago;
            try
            {
                _contrato = contratoBLL.BuscarPorId(pago.IdContrato);
                Inquilino inquilino = _contrato != null ? inquilinoBLL.BuscarPorId(_contrato.IdInquilino) : null;
                if (inquilino != null)
                {
                    lblNombreValor.Text = inquilino.Nombre;
                    lblApellidoValor.Text = inquilino.Apellido;
                    lblDniValor.Text = inquilino.Dni;
                    lblTelefonoValor.Text = string.IsNullOrEmpty(inquilino.Telefono) ? "-" : inquilino.Telefono;
                    lblEmailValor.Text = string.IsNullOrEmpty(inquilino.Email) ? "-" : inquilino.Email;
                }

                lblDireccionValor.Text = fila.Direccion ?? "-";
                if (_contrato != null)
                {
                    lblMontoMensualValor.Text = $"${_contrato.MontoMensual:N2}";
                    lblFechaFinValor.Text = $"{_contrato.FechaFin:dd/MM/yyyy} ({EstadosPN2.Texto(_contrato.EstadoContrato)})";
                }
                lblDiasLitigioValor.Text = fila.DiasEnLitigio.ToString();
                lblMontoAdeudadoValor.Text = $"${fila.MontoAdeudado:N2} (recargo ${pago.RecargoAplicado:N2})";

                List<FilaLitigio> delContrato = FilasDelContrato(pago.IdContrato);
                lblDeudaTotalValor.Text = $"${delContrato.Sum(f => f.MontoAdeudado):N2} ({delContrato.Count} mes(es))";

                // Desalojo solo sobre un contrato vigente
                btnDesalojo.Enabled = _contrato != null && _contrato.Vigente;

                CargarAcciones(pago.IdContrato);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarAcciones(int idContrato)
        {
            dgvAcciones.DataSource = accionBLL.LeerPorContrato(idContrato).Select(a => new FilaAccion
            {
                Fecha = a.FechaAccion,
                Tipo = a.TipoAccion == TipoAccion.Intimacion ? "Intimación" : a.TipoAccion.ToString(),
                Descripcion = a.Descripcion
            }).ToList();
            dgvAcciones.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvAcciones.Columns["Fecha"].FillWeight = 18;
            dgvAcciones.Columns["Tipo"].FillWeight = 18;
            TraducirGrillas();
        }

        // ---------- Acciones legales ----------

        private AccionLegal NuevaAccion(TipoAccion tipo, int idContrato)
        {
            return new AccionLegal
            {
                TipoAccion = tipo,
                IdContrato = idContrato,
                Descripcion = txtDescripcion.Text,
                FechaAccion = dtpFechaAccion.Value.Date == DateTime.Today ? DateTime.Now : dtpFechaAccion.Value.Date
            };
        }

        // Registra la acción (la BLL aplica sus efectos en la misma transacción) y refresca la pantalla
        private bool Registrar(AccionLegal accion, string mensajeExito)
        {
            try
            {
                accionBLL.Registrar(accion);
                MessageBox.Show(mensajeExito, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDescripcion.Clear();
                dtpFechaAccion.Value = DateTime.Today;
                return true;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la acción legal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void btnIntimacion_Click(object sender, EventArgs e)
        {
            FilaLitigio fila = FilaSeleccionada();
            if (fila == null) return;

            if (Registrar(NuevaAccion(TipoAccion.Intimacion, fila.NroContrato),
                    $"Intimación registrada para el contrato N° {fila.NroContrato}."))
                CargarAcciones(fila.NroContrato);
        }

        private void btnResuelto_Click(object sender, EventArgs e)
        {
            FilaLitigio fila = FilaSeleccionada();
            if (fila == null) return;

            List<FilaLitigio> delContrato = FilasDelContrato(fila.NroContrato);
            string periodos = string.Join(", ", delContrato.Select(f => f.Periodo));
            string contrato = _contrato != null && _contrato.Vigente
                ? "El contrato sigue vigente."
                : "El contrato está finalizado: solo se regularizan los pagos.";

            DialogResult res = MessageBox.Show(
                $"¿Confirma marcar como RESUELTO el litigio del contrato N° {fila.NroContrato}?\n\n" +
                $"Inquilino: {fila.Inquilino}\n" +
                $"Se regularizan {delContrato.Count} mes(es) En Litigio ({periodos}) por ${delContrato.Sum(f => f.MontoAdeudado):N2}: " +
                $"pasan a Al día.\n{contrato}",
                "Confirmar resolución", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;

            if (Registrar(NuevaAccion(TipoAccion.Resuelto, fila.NroContrato),
                    $"Litigio del contrato N° {fila.NroContrato} resuelto: {delContrato.Count} mes(es) regularizado(s)."))
                CargarPagos(); // los meses regularizados dejan de estar En Litigio
        }

        private void btnDesalojo_Click(object sender, EventArgs e)
        {
            FilaLitigio fila = FilaSeleccionada();
            if (fila == null) return;

            DialogResult res = MessageBox.Show(
                $"ATENCIÓN: va a registrar el DESALOJO del contrato N° {fila.NroContrato}.\n\n" +
                $"Inquilino: {fila.Inquilino}\nInmueble: {fila.Direccion}\n\n" +
                "• El contrato pasa a Finalizado de forma definitiva y no se puede reabrir.\n" +
                "• El inmueble queda libre para un nuevo contrato.\n" +
                "• La deuda En Litigio NO se cancela: sigue pendiente hasta que se marque como resuelta.\n\n" +
                "¿Desea continuar?",
                "Confirmar desalojo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (res != DialogResult.Yes) return;

            if (Registrar(NuevaAccion(TipoAccion.Desalojo, fila.NroContrato),
                    $"Desalojo registrado: el contrato N° {fila.NroContrato} quedó Finalizado."))
                CargarPagos(fila.Pago.IdPago); // los pagos siguen En Litigio; se refresca el estado del contrato
        }
    }
}
