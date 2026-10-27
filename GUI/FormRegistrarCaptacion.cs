using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
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

        // Validación de la dirección en el mapa mientras se escribe (Nominatim): se consulta 1 segundo después del
        // último cambio de la dirección o la localidad, en segundo plano. Es solo un aviso: se puede guardar igual.
        private enum EstadoUbicacion { Ninguno, Buscando, Encontrada, NoEncontrada, SinConexion }
        private readonly GeocodificacionBLL geocodificacionBLL = new GeocodificacionBLL();
        private readonly Timer _esperaUbicacion = new Timer { Interval = 1000 };
        private EstadoUbicacion _estadoUbicacion = EstadoUbicacion.Ninguno;
        private int _consultaUbicacion; // número de la última consulta: las respuestas de las anteriores se descartan

        public FormRegistrarCaptacion()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
            _esperaUbicacion.Tick += EsperaUbicacion_Tick;
            txtDireccion.TextChanged += DatosUbicacion_TextChanged;
            txtLocalidad.TextChanged += DatosUbicacion_TextChanged;
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
            _controlesTraducibles.Add(new EtiquetaTraducible(lblLocalidad, BE.NombreControl.FormRegistrarCaptacion_lblLocalidad));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblSuperficieLote, BE.NombreControl.FormRegistrarCaptacion_lblSuperficieLote));
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
            MostrarUbicacion(_estadoUbicacion, traducciones);
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
            _esperaUbicacion.Dispose();
            _consultaUbicacion++; // una consulta en curso ya no actualiza nada
            base.OnFormClosed(e);
        }

        // ---------- Ubicación en el mapa mientras se escribe ----------

        // Cada cambio reinicia la espera de 1 segundo; mientras tanto no se muestra el resultado anterior
        private void DatosUbicacion_TextChanged(object sender, EventArgs e)
        {
            _esperaUbicacion.Stop();
            _consultaUbicacion++;
            MostrarUbicacion(EstadoUbicacion.Ninguno);
            if (txtDireccion.Text.Trim().Length > 0 && txtLocalidad.Text.Trim().Length > 0)
                _esperaUbicacion.Start();
        }

        private void EsperaUbicacion_Tick(object sender, EventArgs e)
        {
            _esperaUbicacion.Stop();
            string direccion = txtDireccion.Text.Trim(), localidad = txtLocalidad.Text.Trim();
            if (direccion.Length == 0 || localidad.Length == 0) return;

            int consulta = ++_consultaUbicacion;
            MostrarUbicacion(EstadoUbicacion.Buscando);
            Task.Run(() => geocodificacionBLL.Buscar(direccion, localidad)).ContinueWith(t =>
            {
                if (IsDisposed || consulta != _consultaUbicacion) return; // se cerró o ya se escribió otra cosa
                MostrarUbicacion(t.IsFaulted ? EstadoUbicacion.SinConexion
                    : t.Result != null ? EstadoUbicacion.Encontrada : EstadoUbicacion.NoEncontrada);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Indicador debajo de la dirección; los textos se traducen (con el español por defecto si falta la traducción)
        private void MostrarUbicacion(EstadoUbicacion estado, Dictionary<string, string> traducciones = null)
        {
            _estadoUbicacion = estado;
            if (estado == EstadoUbicacion.Ninguno)
            {
                lblUbicacion.Visible = false;
                return;
            }
            string clave, texto;
            Color color;
            switch (estado)
            {
                case EstadoUbicacion.Buscando:
                    clave = BE.NombreControl.FormRegistrarCaptacion_lblUbicacion_buscando;
                    texto = "Buscando ubicación...";
                    color = Color.FromArgb(200, 196, 214);
                    break;
                case EstadoUbicacion.Encontrada:
                    clave = BE.NombreControl.FormRegistrarCaptacion_lblUbicacion_encontrada;
                    texto = "✓ Dirección encontrada en el mapa";
                    color = Color.FromArgb(130, 220, 150);
                    break;
                case EstadoUbicacion.NoEncontrada:
                    clave = BE.NombreControl.FormRegistrarCaptacion_lblUbicacion_noEncontrada;
                    texto = "⚠ Dirección no encontrada, verificá que sea correcta";
                    color = Color.FromArgb(255, 180, 90);
                    break;
                default:
                    clave = BE.NombreControl.FormRegistrarCaptacion_lblUbicacion_sinConexion;
                    texto = "No se pudo verificar la dirección (sin conexión)";
                    color = Color.FromArgb(200, 196, 214);
                    break;
            }
            traducciones = traducciones ?? GestorIdioma.Instancia.ObtenerTraduccionesActuales();
            if (traducciones != null && traducciones.TryGetValue(clave, out string traducido) && !string.IsNullOrWhiteSpace(traducido))
                texto = traducido;
            lblUbicacion.Text = texto;
            lblUbicacion.ForeColor = color;
            lblUbicacion.Visible = true;
        }

        private void FormRegistrarCaptacion_Load(object sender, EventArgs e)
        {
            cmbTipo.Items.Clear();
            cmbTipo.Items.AddRange(new object[] { "Casa", "Departamento", "Local", "Terreno", "Oficina" });
            cmbTipo.SelectedIndex = 0;
            cmbTipo.SelectedIndexChanged += (s, ev) => MostrarSuperficieLote();

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

        // La superficie del lote solo se pide para una casa (es la que puede evaluarse para un desarrollo)
        private void MostrarSuperficieLote()
        {
            bool casa = string.Equals(cmbTipo.SelectedItem?.ToString(), "Casa", StringComparison.OrdinalIgnoreCase);
            lblSuperficieLote.Visible = nudSuperficieLote.Visible = casa;
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

            if (string.IsNullOrWhiteSpace(txtLocalidad.Text))
            {
                MessageBox.Show("La localidad es obligatoria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocalidad.Focus();
                return;
            }
            if (nudSuperficieLote.Visible && nudSuperficieLote.Value <= 0)
            {
                MessageBox.Show("Ingrese la superficie del lote de la casa.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudSuperficieLote.Focus();
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
                    CantidadAmbientes = (int)nudAmbientes.Value,
                    Localidad = txtLocalidad.Text,
                    SuperficieLote = nudSuperficieLote.Visible ? nudSuperficieLote.Value : (decimal?)null
                };

                Captacion captacion = new Captacion
                {
                    ValorPretendido = nudValorPretendido.Value
                };

                int nro = captacionBLL.RegistrarCaptacion(cliente, inmueble, captacion, chkAutorizacionRecibida.Checked);

                string ubicacion = captacionBLL.ErrorUbicacion == null
                    ? $"\nUbicación en el mapa: {inmueble.Latitud:0.000000}, {inmueble.Longitud:0.000000}."
                    : $"\nNo se pudo ubicar en el mapa: {captacionBLL.ErrorUbicacion}";
                MessageBox.Show($"Captación N° {nro} registrada con éxito.{ubicacion}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            txtLocalidad.Clear();
            nudSuperficieLote.Value = 0;
            MostrarSuperficieLote();
            nudMetros.Value = 0;
            nudAmbientes.Value = 1;
            nudValorPretendido.Value = 0;
            chkAutorizacionRecibida.Checked = false;
            txtDni.Focus();
        }
    }
}
