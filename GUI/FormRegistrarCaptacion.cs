using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormRegistrarCaptacion : Form, IObservador
    {
        private ClienteBLL clienteBLL = new ClienteBLL();
        private CaptacionBLL captacionBLL = new CaptacionBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();

        private Cliente _clienteEncontrado; // cliente existente cargado con "Buscar"

        public FormRegistrarCaptacion()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormRegistrarCaptacion"));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpCliente, BE.NombreControl.FormRegistrarCaptacion_grpCliente));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDni, BE.NombreControl.FormRegistrarCaptacion_lblDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnBuscarCliente, BE.NombreControl.FormRegistrarCaptacion_btnBuscarCliente));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNombre, BE.NombreControl.FormRegistrarCaptacion_lblNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblApellido, BE.NombreControl.FormRegistrarCaptacion_lblApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTelefono, BE.NombreControl.FormRegistrarCaptacion_lblTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEmail, BE.NombreControl.FormRegistrarCaptacion_lblEmail));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpInmueble, BE.NombreControl.FormRegistrarCaptacion_grpInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTipo, BE.NombreControl.FormRegistrarCaptacion_lblTipo));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDireccion, BE.NombreControl.FormRegistrarCaptacion_lblDireccion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMetros, BE.NombreControl.FormRegistrarCaptacion_lblMetros));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblAmbientes, BE.NombreControl.FormRegistrarCaptacion_lblAmbientes));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpCaptacion, BE.NombreControl.FormRegistrarCaptacion_grpCaptacion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblValorPretendido, BE.NombreControl.FormRegistrarCaptacion_lblValorPretendido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTipoPublicacion, BE.NombreControl.FormRegistrarCaptacion_lblTipoPublicacion));
            // chkAutorizacionRecibida: su texto depende de la operación (venta / alquiler), ver ActualizarTextoAutorizacion
            _controlesTraducibles.Add(new EtiquetaTraducible(btnGuardar, BE.NombreControl.FormRegistrarCaptacion_btnGuardar));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCancelar, BE.NombreControl.FormRegistrarCaptacion_btnCancelar));
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
            ActualizarTextoAutorizacion(traducciones);
        }

        private TipoPublicacion TipoPublicacionSeleccionado()
        {
            return cmbTipoPublicacion.SelectedItem is TipoPublicacion tipo ? tipo : TipoPublicacion.Venta;
        }

        // "Autorización de Venta/Alquiler firmada recibida": una traducción por operación, igual que EtiquetaTraducible
        private void ActualizarTextoAutorizacion(Dictionary<string, string> traducciones = null)
        {
            traducciones = traducciones ?? GestorIdioma.Instancia.ObtenerTraduccionesActuales();
            string clave = TipoPublicacionSeleccionado() == TipoPublicacion.Alquiler
                ? BE.NombreControl.FormRegistrarCaptacion_chkAutorizacionAlquiler
                : BE.NombreControl.FormRegistrarCaptacion_chkAutorizacionRecibida;
            chkAutorizacionRecibida.Text = traducciones != null && traducciones.TryGetValue(clave, out string texto) && !string.IsNullOrWhiteSpace(texto)
                ? texto
                : $"<{clave}>";
        }

        private void cmbTipoPublicacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarTextoAutorizacion();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormRegistrarCaptacion_Load(object sender, EventArgs e)
        {
            cmbTipo.Items.Clear();
            cmbTipo.Items.AddRange(new object[] { "Casa", "Departamento", "Local", "Terreno", "Oficina" });
            cmbTipo.SelectedIndex = 0;

            cmbTipoPublicacion.Items.Clear();
            cmbTipoPublicacion.Items.AddRange(new object[] { TipoPublicacion.Venta, TipoPublicacion.Alquiler });
            cmbTipoPublicacion.SelectedIndex = 0;

            Limpiar();
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI para buscar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            try
            {
                Cliente cliente = clienteBLL.BuscarPorDni(dni);
                if (cliente != null)
                {
                    MostrarClienteExistente(cliente);
                    MessageBox.Show($"Cliente encontrado: {cliente.Apellido}, {cliente.Nombre}.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmbTipo.Focus();
                }
                else
                {
                    LiberarDatosCliente(limpiar: true);
                    MessageBox.Show("No existe un cliente con ese DNI. Complete sus datos para registrarlo.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Si el agente cambia el DNI de un cliente ya encontrado, deja de ser ese cliente
        private void txtDni_TextChanged(object sender, EventArgs e)
        {
            if (_clienteEncontrado != null && txtDni.Text.Trim() != _clienteEncontrado.Dni)
                LiberarDatosCliente(limpiar: true);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("El DNI del cliente es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            if (!chkAutorizacionRecibida.Checked)
            {
                MessageBox.Show(CaptacionBLL.MensajeAutorizacionRequerida(TipoPublicacionSeleccionado()), "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkAutorizacionRecibida.Focus();
                return;
            }

            try
            {
                // Sin "Buscar" previo: si el DNI ya existe se usan los datos registrados, no los tipeados
                if (_clienteEncontrado == null)
                {
                    Cliente existente = clienteBLL.BuscarPorDni(dni);
                    if (existente != null)
                    {
                        DialogResult res = MessageBox.Show(
                            $"Ya existe un cliente con DNI {dni}: {existente.Apellido}, {existente.Nombre}.\n\n" +
                            "La captación se registrará con los datos ya guardados de ese cliente. ¿Desea continuar?",
                            "Cliente existente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        MostrarClienteExistente(existente);
                        if (res != DialogResult.Yes) return;
                    }
                }

                Cliente cliente = new Cliente
                {
                    Dni = dni,
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Telefono = txtTelefono.Text,
                    Email = txtEmail.Text
                };

                Inmueble inmueble = new Inmueble
                {
                    TipoInmueble = cmbTipo.SelectedItem?.ToString(),
                    TipoPublicacion = TipoPublicacionSeleccionado(),
                    Direccion = txtDireccion.Text,
                    MetrosCuadrados = nudMetros.Value,
                    CantidadAmbientes = (int)nudAmbientes.Value
                };

                Captacion captacion = new Captacion
                {
                    ValorPretendido = nudValorPretendido.Value
                };

                int nro = captacionBLL.RegistrarCaptacion(cliente, inmueble, captacion, chkAutorizacionRecibida.Checked);

                MessageBox.Show($"Captación N° {nro} registrada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la captación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MostrarClienteExistente(Cliente cliente)
        {
            _clienteEncontrado = cliente;
            txtDni.Text = cliente.Dni;
            txtNombre.Text = cliente.Nombre;
            txtApellido.Text = cliente.Apellido;
            txtTelefono.Text = cliente.Telefono;
            txtEmail.Text = cliente.Email;
            // Los datos de un cliente existente no se modifican desde este formulario
            txtNombre.ReadOnly = txtApellido.ReadOnly = txtTelefono.ReadOnly = txtEmail.ReadOnly = true;
        }

        private void LiberarDatosCliente(bool limpiar)
        {
            _clienteEncontrado = null;
            txtNombre.ReadOnly = txtApellido.ReadOnly = txtTelefono.ReadOnly = txtEmail.ReadOnly = false;
            if (limpiar)
            {
                txtNombre.Clear();
                txtApellido.Clear();
                txtTelefono.Clear();
                txtEmail.Clear();
            }
        }

        private void Limpiar()
        {
            txtDni.Clear();
            LiberarDatosCliente(limpiar: true);
            if (cmbTipo.Items.Count > 0) cmbTipo.SelectedIndex = 0;
            if (cmbTipoPublicacion.Items.Count > 0) cmbTipoPublicacion.SelectedIndex = 0;
            txtDireccion.Clear();
            nudMetros.Value = 0;
            nudAmbientes.Value = 1;
            nudValorPretendido.Value = 0;
            chkAutorizacionRecibida.Checked = false;
            txtDni.Focus();
        }
    }
}
