using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormValidarTasacion : Form, IObservador
    {
        private CaptacionBLL captacionBLL = new CaptacionBLL();
        private TasacionBLL tasacionBLL = new TasacionBLL();
        private ClienteBLL clienteBLL = new ClienteBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _dgvTraducible;
        private bool _cargando; // evita refrescar el detalle mientras se rearma la grilla

        public FormValidarTasacion()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormValidarTasacion"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPendientes, BE.NombreControl.FormValidarTasacion_lblPendientes));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnActualizar, BE.NombreControl.FormValidarTasacion_btnActualizar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpDetalle, BE.NombreControl.FormValidarTasacion_grpDetalle));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblCliente, BE.NombreControl.FormValidarTasacion_lblCliente));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDni, BE.NombreControl.FormValidarTasacion_lblDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTelefono, BE.NombreControl.FormValidarTasacion_lblTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEmail, BE.NombreControl.FormValidarTasacion_lblEmail));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblValorPretendido, BE.NombreControl.FormValidarTasacion_lblValorPretendido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDictamenAnterior, BE.NombreControl.FormValidarTasacion_lblDictamenAnterior));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpDictamen, BE.NombreControl.FormValidarTasacion_grpDictamen));
            _controlesTraducibles.Add(new EtiquetaTraducible(rbAprobar, BE.NombreControl.FormValidarTasacion_rbAprobar));
            _controlesTraducibles.Add(new EtiquetaTraducible(rbRechazar, BE.NombreControl.FormValidarTasacion_rbRechazar));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblValorFinal, BE.NombreControl.FormValidarTasacion_lblValorFinal));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMotivo, BE.NombreControl.FormValidarTasacion_lblMotivo));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblValorSugerido, BE.NombreControl.FormValidarTasacion_lblValorSugerido));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRegistrar, BE.NombreControl.FormValidarTasacion_btnRegistrar));

            _dgvTraducible = new DataGridTraducible(dgvPendientes, "FormValidarTasacion.dgvPendientes")
                .ConColumna("NroCaptacion", BE.NombreControl.FormValidarTasacion_dgvPendientes_nro)
                .ConColumna("FechaRegistro", BE.NombreControl.FormValidarTasacion_dgvPendientes_fecha)
                .ConColumna("Cliente", BE.NombreControl.FormValidarTasacion_dgvPendientes_cliente)
                .ConColumna("Direccion", BE.NombreControl.FormValidarTasacion_dgvPendientes_direccion)
                .ConColumna("TipoInmueble", BE.NombreControl.FormValidarTasacion_dgvPendientes_tipo)
                .ConColumna("MetrosCuadrados", BE.NombreControl.FormValidarTasacion_dgvPendientes_metros)
                .ConColumna("CantidadAmbientes", BE.NombreControl.FormValidarTasacion_dgvPendientes_ambientes)
                .ConColumna("ValorPretendido", BE.NombreControl.FormValidarTasacion_dgvPendientes_valor);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
            if (_dgvTraducible != null)
            {
                _dgvTraducible.Traducir(traducciones);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormValidarTasacion_Load(object sender, EventArgs e)
        {
            CargarPendientes();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarPendientes(CaptacionSeleccionada()?.NroCaptacion);
        }

        // Pendientes de tasación = captaciones con el inmueble en estado Captado
        private void CargarPendientes(int? nroASeleccionar = null)
        {
            _cargando = true;
            try
            {
                dgvPendientes.DataSource = null;
                dgvPendientes.DataSource = captacionBLL.LeerResumen(EstadoInmueble.Captado);

                foreach (string oculta in new[] { "EstadoInmueble", "IdInmueble", "IdCliente", "ValorFinal", "CantidadArchivos", "EstadoPublicacion", "FechaPublicacion", "FechaDespublicacion" })
                {
                    if (dgvPendientes.Columns[oculta] != null) dgvPendientes.Columns[oculta].Visible = false;
                }
                if (dgvPendientes.Columns["FechaRegistro"] != null) dgvPendientes.Columns["FechaRegistro"].DefaultCellStyle.Format = "dd/MM/yyyy";
                foreach (string numerica in new[] { "MetrosCuadrados", "ValorPretendido" })
                {
                    if (dgvPendientes.Columns[numerica] == null) continue;
                    dgvPendientes.Columns[numerica].DefaultCellStyle.Format = "N2";
                    dgvPendientes.Columns[numerica].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvPendientes.Columns["CantidadAmbientes"] != null)
                    dgvPendientes.Columns["CantidadAmbientes"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                var traducciones = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
                if (traducciones != null)
                {
                    _dgvTraducible.Traducir(traducciones);
                }

                if (nroASeleccionar.HasValue)
                {
                    foreach (DataGridViewRow row in dgvPendientes.Rows)
                    {
                        if (row.DataBoundItem is CaptacionResumen r && r.NroCaptacion == nroASeleccionar.Value)
                        {
                            row.Selected = true;
                            dgvPendientes.CurrentCell = row.Cells["NroCaptacion"];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las captaciones pendientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
            }

            MostrarDetalle();
        }

        private void dgvPendientes_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarDetalle();
        }

        private CaptacionResumen CaptacionSeleccionada()
        {
            return dgvPendientes.CurrentRow?.DataBoundItem as CaptacionResumen;
        }

        private void MostrarDetalle()
        {
            CaptacionResumen seleccionada = CaptacionSeleccionada();
            LimpiarDictamen(); // lo cargado para otra captación no debe aplicarse a esta

            lblClienteValor.Text = lblDniValor.Text = lblTelefonoValor.Text = lblEmailValor.Text = "-";
            lblValorPretendidoValor.Text = lblDictamenAnteriorValor.Text = "-";
            grpDictamen.Enabled = seleccionada != null;
            if (seleccionada == null) return;

            lblClienteValor.Text = seleccionada.Cliente;
            lblValorPretendidoValor.Text = seleccionada.ValorPretendido.ToString("N2");

            try
            {
                Cliente cliente = clienteBLL.BuscarPorId(seleccionada.IdCliente);
                if (cliente != null)
                {
                    lblDniValor.Text = cliente.Dni;
                    lblTelefonoValor.Text = string.IsNullOrEmpty(cliente.Telefono) ? "-" : cliente.Telefono;
                    lblEmailValor.Text = string.IsNullOrEmpty(cliente.Email) ? "-" : cliente.Email;
                }

                // Si la captación vuelve tras una renegociación, el tasador ve su dictamen previo
                Tasacion anterior = tasacionBLL.ObtenerUltimoDictamen(seleccionada.NroCaptacion);
                if (anterior == null)
                {
                    lblDictamenAnteriorValor.Text = "Primera tasación";
                }
                else
                {
                    string texto = $"{anterior.EstadoDictamen} ({anterior.FechaDictamen:dd/MM/yyyy})";
                    if (!string.IsNullOrEmpty(anterior.MotivoRechazo)) texto += $": {anterior.MotivoRechazo}";
                    if (anterior.ValorSugerido.HasValue) texto += $". Sugerido: {anterior.ValorSugerido.Value:N2}";
                    lblDictamenAnteriorValor.Text = texto;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarDictamen()
        {
            rbAprobar.Checked = false;
            rbRechazar.Checked = false;
            nudValorFinal.Value = 0;
            txtMotivo.Clear();
            nudValorSugerido.Value = 0;
            ActualizarCamposDictamen();
        }

        private void rbDictamen_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarCamposDictamen();
        }

        // Aprobar habilita el valor final; Rechazar habilita motivo y valor sugerido
        private void ActualizarCamposDictamen()
        {
            nudValorFinal.Enabled = lblValorFinal.Enabled = rbAprobar.Checked;
            txtMotivo.Enabled = lblMotivo.Enabled = rbRechazar.Checked;
            nudValorSugerido.Enabled = lblValorSugerido.Enabled = rbRechazar.Checked;
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            CaptacionResumen seleccionada = CaptacionSeleccionada();
            if (seleccionada == null) return;

            if (!rbAprobar.Checked && !rbRechazar.Checked)
            {
                MessageBox.Show("Seleccione si aprueba o rechaza la tasación.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Tasacion tasacion = new Tasacion { IdCaptacion = seleccionada.NroCaptacion };
            string confirmacion;
            if (rbAprobar.Checked)
            {
                tasacion.EstadoDictamen = EstadoDictamen.Aprobada;
                tasacion.ValorFinal = nudValorFinal.Value;
                confirmacion = $"¿Confirma la APROBACIÓN de la Captación N° {seleccionada.NroCaptacion}?\n\n" +
                               $"Valor pretendido: {seleccionada.ValorPretendido:N2}\nValor final de tasación: {nudValorFinal.Value:N2}";
            }
            else
            {
                tasacion.EstadoDictamen = EstadoDictamen.Rechazada;
                tasacion.MotivoRechazo = txtMotivo.Text;
                tasacion.ValorSugerido = nudValorSugerido.Value > 0 ? nudValorSugerido.Value : (decimal?)null;
                confirmacion = $"¿Confirma el RECHAZO de la Captación N° {seleccionada.NroCaptacion}?\n\n" +
                               $"Motivo: {txtMotivo.Text.Trim()}\n" +
                               (tasacion.ValorSugerido.HasValue ? $"Valor sugerido al agente: {tasacion.ValorSugerido.Value:N2}" : "Sin valor sugerido");
            }

            // Primero las validaciones de la BLL, después la confirmación
            if (rbAprobar.Checked && nudValorFinal.Value <= 0)
            {
                MessageBox.Show("Una tasación aprobada debe tener un valor final mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudValorFinal.Focus();
                return;
            }
            if (rbRechazar.Checked && string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show("Una tasación rechazada debe indicar el motivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMotivo.Focus();
                return;
            }

            if (MessageBox.Show(confirmacion, "Confirmar dictamen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int nro = tasacionBLL.Registrar(tasacion);
                string resultado = tasacion.EstadoDictamen == EstadoDictamen.Aprobada ? "aprobada (inmueble Tasado)" : "rechazada (inmueble Rechazado)";
                MessageBox.Show($"Tasación N° {nro} registrada: Captación N° {seleccionada.NroCaptacion} {resultado}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPendientes();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el dictamen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CargarPendientes(seleccionada.NroCaptacion); // la captación pudo haber cambiado de estado
            }
        }
    }
}
