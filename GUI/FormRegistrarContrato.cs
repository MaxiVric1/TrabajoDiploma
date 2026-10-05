using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormRegistrarContrato : Form, IObservador
    {
        private InquilinoBLL inquilinoBLL = new InquilinoBLL();
        private InmuebleAlquilerBLL inmuebleBLL = new InmuebleAlquilerBLL();
        private ContratoBLL contratoBLL = new ContratoBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();

        private Inquilino _inquilinoEncontrado; // inquilino existente cargado con "Buscar"

        public FormRegistrarContrato()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormRegistrarContrato"));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpInquilino, BE.NombreControl.FormRegistrarContrato_grpInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDni, BE.NombreControl.FormRegistrarContrato_lblDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnBuscarInquilino, BE.NombreControl.FormRegistrarContrato_btnBuscarInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNombre, BE.NombreControl.FormRegistrarContrato_lblNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblApellido, BE.NombreControl.FormRegistrarContrato_lblApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTelefono, BE.NombreControl.FormRegistrarContrato_lblTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEmail, BE.NombreControl.FormRegistrarContrato_lblEmail));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpInmueble, BE.NombreControl.FormRegistrarContrato_grpInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblInmueble, BE.NombreControl.FormRegistrarContrato_lblInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnNuevoInmueble, BE.NombreControl.FormRegistrarContrato_btnNuevoInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNuevaDireccion, BE.NombreControl.FormRegistrarContrato_lblNuevaDireccion));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnAgregarInmueble, BE.NombreControl.FormRegistrarContrato_btnAgregarInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCancelarInmueble, BE.NombreControl.FormRegistrarContrato_btnCancelarInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpContrato, BE.NombreControl.FormRegistrarContrato_grpContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaInicio, BE.NombreControl.FormRegistrarContrato_lblFechaInicio));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDuracion, BE.NombreControl.FormRegistrarContrato_lblDuracion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMonto, BE.NombreControl.FormRegistrarContrato_lblMonto));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaFin, BE.NombreControl.FormRegistrarContrato_lblFechaFin));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnGuardar, BE.NombreControl.FormRegistrarContrato_btnGuardar));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCancelar, BE.NombreControl.FormRegistrarContrato_btnCancelar));
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormRegistrarContrato_Load(object sender, EventArgs e)
        {
            cmbNuevoTipo.Items.Clear();
            foreach (TipoInmueble tipo in Enum.GetValues(typeof(TipoInmueble)))
            {
                cmbNuevoTipo.Items.Add(tipo);
            }
            cmbNuevoTipo.SelectedIndex = 0;

            CargarInmuebles();
            Limpiar();
        }

        // ---------- Inquilino ----------

        private void btnBuscarInquilino_Click(object sender, EventArgs e)
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
                Inquilino inquilino = inquilinoBLL.BuscarPorDni(dni);
                if (inquilino != null)
                {
                    MostrarInquilinoExistente(inquilino);
                    MessageBox.Show($"Inquilino encontrado: {inquilino.Apellido}, {inquilino.Nombre}.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmbInmueble.Focus();
                }
                else
                {
                    LiberarDatosInquilino(limpiar: true);
                    MessageBox.Show("No existe un inquilino con ese DNI. Complete sus datos para registrarlo junto con el contrato.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el inquilino: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Si el agente cambia el DNI de un inquilino ya encontrado, deja de ser ese inquilino
        private void txtDni_TextChanged(object sender, EventArgs e)
        {
            if (_inquilinoEncontrado != null && txtDni.Text.Trim() != _inquilinoEncontrado.Dni)
                LiberarDatosInquilino(limpiar: true);
        }

        private void MostrarInquilinoExistente(Inquilino inquilino)
        {
            _inquilinoEncontrado = inquilino;
            txtDni.Text = inquilino.Dni;
            txtNombre.Text = inquilino.Nombre;
            txtApellido.Text = inquilino.Apellido;
            txtTelefono.Text = inquilino.Telefono;
            txtEmail.Text = inquilino.Email;
            // Los datos de un inquilino existente no se modifican desde este formulario
            txtNombre.ReadOnly = txtApellido.ReadOnly = txtTelefono.ReadOnly = txtEmail.ReadOnly = true;
        }

        private void LiberarDatosInquilino(bool limpiar)
        {
            _inquilinoEncontrado = null;
            txtNombre.ReadOnly = txtApellido.ReadOnly = txtTelefono.ReadOnly = txtEmail.ReadOnly = false;
            if (limpiar)
            {
                txtNombre.Clear();
                txtApellido.Clear();
                txtTelefono.Clear();
                txtEmail.Clear();
            }
        }

        // ---------- Inmueble ----------

        // Solo inmuebles sin contrato vigente
        private void CargarInmuebles(int? idASeleccionar = null)
        {
            try
            {
                List<InmuebleAlquiler> disponibles = inmuebleBLL.LeerDisponibles();
                cmbInmueble.DataSource = null;
                cmbInmueble.DataSource = disponibles; // se muestra "Tipo - Dirección" (InmuebleAlquiler.ToString)
                cmbInmueble.SelectedIndex = -1;
                if (idASeleccionar.HasValue)
                    cmbInmueble.SelectedItem = disponibles.FirstOrDefault(i => i.IdInmuebleAlquiler == idASeleccionar.Value);
                else if (disponibles.Count > 0)
                    cmbInmueble.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los inmuebles disponibles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevoInmueble_Click(object sender, EventArgs e)
        {
            MostrarNuevoInmueble(true);
            txtNuevaDireccion.Focus();
        }

        private void btnCancelarInmueble_Click(object sender, EventArgs e)
        {
            MostrarNuevoInmueble(false);
        }

        private void MostrarNuevoInmueble(bool visible)
        {
            lblNuevaDireccion.Visible = txtNuevaDireccion.Visible = cmbNuevoTipo.Visible = visible;
            btnAgregarInmueble.Visible = btnCancelarInmueble.Visible = visible;
            btnNuevoInmueble.Enabled = !visible;
            if (!visible)
            {
                txtNuevaDireccion.Clear();
                if (cmbNuevoTipo.Items.Count > 0) cmbNuevoTipo.SelectedIndex = 0;
            }
        }

        // El inmueble se registra en el momento: queda disponible aunque después no se guarde el contrato
        private void btnAgregarInmueble_Click(object sender, EventArgs e)
        {
            try
            {
                InmuebleAlquiler nuevo = new InmuebleAlquiler
                {
                    Direccion = txtNuevaDireccion.Text,
                    TipoInmueble = (TipoInmueble)cmbNuevoTipo.SelectedItem
                };
                int id = inmuebleBLL.Registrar(nuevo);

                MostrarNuevoInmueble(false);
                CargarInmuebles(id);
                dtpFechaInicio.Focus();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNuevaDireccion.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el inmueble: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------- Contrato ----------

        // Fecha de fin, estado con el que nacerá el contrato y primer mes a pagar (mismas reglas que la BLL)
        private void DatosContrato_Changed(object sender, EventArgs e)
        {
            Contrato borrador = new Contrato { FechaInicio = dtpFechaInicio.Value.Date, DuracionMeses = (int)nudDuracion.Value };
            DateTime fin = borrador.FechaFin;
            var primerMes = PagoAlquilerBLL.PeriodosDelContrato(borrador).First();
            string texto = $"{fin:dd/MM/yyyy} — primer mes a pagar: {primerMes.Mes:00}/{primerMes.Anio}";

            switch (ContratoBLL.EstadoSegunVencimiento(fin, DateTime.Now))
            {
                case EstadoContrato.PorVencer:
                    texto += $" — nacerá Por Vencer (vence en {ContratoBLL.DiasRestantes(borrador, DateTime.Now)} días)";
                    break;
                case EstadoContrato.Finalizado:
                    texto += " — con estas fechas ya estaría vencido";
                    break;
            }
            lblFechaFinValor.Text = texto;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("El DNI del inquilino es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }
            if (!(cmbInmueble.SelectedItem is InmuebleAlquiler inmueble))
            {
                MessageBox.Show(cmbInmueble.Items.Count == 0
                        ? "No hay inmuebles disponibles: cargue uno con \"Nuevo Inmueble\"."
                        : "Seleccione el inmueble del contrato.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Sin "Buscar" previo: si el DNI ya existe se usan los datos registrados, no los tipeados
                if (_inquilinoEncontrado == null)
                {
                    Inquilino existente = inquilinoBLL.BuscarPorDni(dni);
                    if (existente != null)
                    {
                        DialogResult res = MessageBox.Show(
                            $"Ya existe un inquilino con DNI {dni}: {existente.Apellido}, {existente.Nombre}.\n\n" +
                            "El contrato se registrará con los datos ya guardados de ese inquilino. ¿Desea continuar?",
                            "Inquilino existente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        MostrarInquilinoExistente(existente);
                        if (res != DialogResult.Yes) return;
                    }
                }

                Contrato contrato = new Contrato
                {
                    FechaInicio = dtpFechaInicio.Value.Date,
                    DuracionMeses = (int)nudDuracion.Value,
                    MontoMensual = nudMonto.Value,
                    IdInmuebleAlquiler = inmueble.IdInmuebleAlquiler
                };

                // Inquilino nuevo: se da de alta junto con el contrato en una sola transacción
                Inquilino inquilinoNuevo = null;
                if (_inquilinoEncontrado != null)
                {
                    contrato.IdInquilino = _inquilinoEncontrado.IdInquilino;
                }
                else
                {
                    inquilinoNuevo = new Inquilino
                    {
                        Dni = dni,
                        Nombre = txtNombre.Text,
                        Apellido = txtApellido.Text,
                        Telefono = txtTelefono.Text,
                        Email = txtEmail.Text
                    };
                }

                int nro = contratoBLL.Registrar(contrato, inquilinoNuevo);

                string estado = contrato.EstadoContrato == EstadoContrato.PorVencer ? " Queda Por Vencer (alerta de vencimiento activa)." : "";
                MessageBox.Show($"Contrato N° {nro} registrado con éxito.\nVence el {contrato.FechaFin:dd/MM/yyyy}.{estado}",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                CargarInmuebles(); // el inmueble alquilado deja de estar disponible
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Limpiar()
        {
            txtDni.Clear();
            LiberarDatosInquilino(limpiar: true);
            MostrarNuevoInmueble(false);
            dtpFechaInicio.Value = DateTime.Today;
            nudDuracion.Value = 24;
            nudMonto.Value = 0;
            DatosContrato_Changed(this, EventArgs.Empty);
            txtDni.Focus();
        }
    }
}
