using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormGestionUsuarios : Form, IObservador
    {
        private UsuarioBLL usuarioBLL = new UsuarioBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _dgvTraducible;

        public FormGestionUsuarios()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormGestionUsuarios"));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpAcciones, BE.NombreControl.FormGestionUsuarios_grpAcciones));
            _controlesTraducibles.Add(new EtiquetaTraducible(rbModoCrear, BE.NombreControl.FormGestionUsuarios_rbModoCrear));
            _controlesTraducibles.Add(new EtiquetaTraducible(rbModoEditar, BE.NombreControl.FormGestionUsuarios_rbModoEditar));
            _controlesTraducibles.Add(new EtiquetaTraducible(label1, BE.NombreControl.FormGestionUsuarios_label1));
            _controlesTraducibles.Add(new EtiquetaTraducible(labelNombrePersona, BE.NombreControl.FormGestionUsuarios_labelNombrePersona));
            _controlesTraducibles.Add(new EtiquetaTraducible(labelApellido, BE.NombreControl.FormGestionUsuarios_labelApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(label2, BE.NombreControl.FormGestionUsuarios_label2));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblRol, BE.NombreControl.FormGestionUsuarios_lblRol));
            _controlesTraducibles.Add(new EtiquetaTraducible(chkActivo, BE.NombreControl.FormGestionUsuarios_chkActivo));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTiempoBloqueo, BE.NombreControl.FormGestionUsuarios_lblTiempoBloqueo));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRegistrar, BE.NombreControl.FormGestionUsuarios_btnRegistrar));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnBloquear, BE.NombreControl.FormGestionUsuarios_btnBloquear));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnDesbloquear, BE.NombreControl.FormGestionUsuarios_btnDesbloquear));

            _dgvTraducible = new DataGridTraducible(dgvUsuarios, "FormGestionUsuarios.dgvUsuarios")
                .ConColumna("Nombre", BE.NombreControl.FormGestionUsuarios_dgvUsuarios_nombre)
                .ConColumna("NombrePersona", BE.NombreControl.FormGestionUsuarios_dgvUsuarios_nombrePersona)
                .ConColumna("Apellido", BE.NombreControl.FormGestionUsuarios_dgvUsuarios_apellido)
                .ConColumna("Activo", BE.NombreControl.FormGestionUsuarios_dgvUsuarios_activo)
                .ConColumna("IntentosFallidos", BE.NombreControl.FormGestionUsuarios_dgvUsuarios_intentos)
                .ConColumna("BloqueadoHasta", BE.NombreControl.FormGestionUsuarios_dgvUsuarios_bloqueadoHasta);
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

        private void FormGestionUsuarios_Load(object sender, EventArgs e)
        {
            // Comprobación de integridad preventiva
            try
            {
                var reporte = new BLL.IntegridadBLL().VerificarIntegridad();
                if (!reporte.EsValido)
                {
                    string msg = reporte.DvvInvalido
                        ? "ERROR DE INTEGRIDAD: La estructura de la tabla de usuarios ha sido alterada externamente."
                        : $"ERROR DE INTEGRIDAD: Se ha detectado corrupción en los usuarios: {string.Join(", ", reporte.UsuariosCorruptos)}";

                    MessageBox.Show(msg + "\n\nSe sugiere realizar una reparación o restaurar un backup antes de operar.",
                        "Advertencia de Integridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar integridad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Cargar tiempo bloqueo opciones
            cmbTiempoBloqueo.Items.Clear();
            cmbTiempoBloqueo.Items.Add("5 min");
            cmbTiempoBloqueo.Items.Add("1 hora");
            cmbTiempoBloqueo.Items.Add("1 día");
            cmbTiempoBloqueo.SelectedIndex = 0;

            // Cargar roles
            cmbRoles.DataSource = usuarioBLL.ObtenerRoles();
            cmbRoles.DisplayMember = "Nombre";
            cmbRoles.ValueMember = "Id";

            CargarUsuarios();
            ActualizarControlesPorModo();
        }

        private void CargarUsuarios()
        {
            try
            {
                dgvUsuarios.DataSource = null;
                var list = usuarioBLL.LeerTodos();
                dgvUsuarios.DataSource = list;

                if (dgvUsuarios.Columns["Password"] != null) dgvUsuarios.Columns["Password"].Visible = false;
                if (dgvUsuarios.Columns["DVH"] != null) dgvUsuarios.Columns["DVH"].Visible = false;
                if (dgvUsuarios.Columns["IdIdioma"] != null) dgvUsuarios.Columns["IdIdioma"].Visible = false;
                
                // Refresh translation
                var traducciones = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
                if (traducciones != null)
                {
                    _dgvTraducible.Traducir(traducciones);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarControlesPorModo()
        {
            if (rbModoCrear.Checked)
            {
                txtNombre.Text = "";
                txtNombre.ReadOnly = false;
                txtNombrePersona.Text = "";
                txtApellido.Text = "";
                txtPassword.Text = "";
                chkActivo.Checked = true;
                chkActivo.Enabled = false; // default for new user is true / active

                lblTiempoBloqueo.Visible = false;
                cmbTiempoBloqueo.Visible = false;
                btnBloquear.Visible = false;
                btnDesbloquear.Visible = false;
            }
            else // rbModoEditar.Checked
            {
                txtNombre.ReadOnly = true; // name cannot be edited to avoid corruption or let it be read-only
                chkActivo.Enabled = true;

                lblTiempoBloqueo.Visible = true;
                cmbTiempoBloqueo.Visible = true;
                btnBloquear.Visible = true;
                btnDesbloquear.Visible = true;

                CargarUsuarioSeleccionado();
            }
        }

        private void CargarUsuarioSeleccionado()
        {
            if (dgvUsuarios.CurrentRow != null)
            {
                Usuario user = dgvUsuarios.CurrentRow.DataBoundItem as Usuario;
                if (user != null)
                {
                    txtNombre.Text = user.Nombre;
                    txtNombrePersona.Text = user.NombrePersona;
                    txtApellido.Text = user.Apellido;
                    txtPassword.Text = ""; // leave blank to keep existing
                    chkActivo.Checked = user.Activo;

                    // Load roles
                    usuarioBLL.CargarRoles(user);
                    if (user.Roles != null && user.Roles.Count > 0)
                    {
                        cmbRoles.SelectedValue = user.Roles[0].Id;
                    }
                    else
                    {
                        cmbRoles.SelectedIndex = -1;
                    }

                    // Enable/disable blocking buttons
                    bool isBlocked = user.BloqueadoHasta.HasValue && user.BloqueadoHasta.Value > DateTime.Now;
                    btnBloquear.Enabled = !isBlocked;
                    btnDesbloquear.Enabled = isBlocked;
                }
            }
            else
            {
                txtNombre.Text = "";
                txtNombrePersona.Text = "";
                txtApellido.Text = "";
                txtPassword.Text = "";
                chkActivo.Checked = false;
                cmbRoles.SelectedIndex = -1;
                btnBloquear.Enabled = false;
                btnDesbloquear.Enabled = false;
            }
        }

        private void rbModo_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarControlesPorModo();
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (rbModoEditar.Checked)
            {
                CargarUsuarioSeleccionado();
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string nombrePersona = txtNombrePersona.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("El nombre de usuario es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(nombrePersona) || string.IsNullOrEmpty(apellido))
                {
                    MessageBox.Show("El nombre y apellido son requeridos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idRol = 2; // Default User
                if (cmbRoles.SelectedValue != null)
                {
                    idRol = (int)cmbRoles.SelectedValue;
                }

                if (rbModoCrear.Checked)
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        MessageBox.Show("La contraseña es requerida para un nuevo usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Usuario nuevo = new Usuario
                    {
                        Nombre = nombre,
                        NombrePersona = nombrePersona,
                        Apellido = apellido,
                        Password = password,
                        Activo = true
                    };

                    int result = usuarioBLL.Registrar(nuevo, idRol);
                    if (result == -2)
                    {
                        MessageBox.Show("El usuario ya existe.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (result != -1)
                    {
                        MessageBox.Show("Usuario registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarUsuarios();
                        rbModoEditar.Checked = true; // Switch to edit mode to see the newly created user
                    }
                    else
                    {
                        MessageBox.Show("Error al registrar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else // rbModoEditar.Checked
                {
                    if (dgvUsuarios.CurrentRow == null) return;
                    Usuario selected = dgvUsuarios.CurrentRow.DataBoundItem as Usuario;
                    if (selected == null) return;

                    selected.Nombre = nombre;
                    selected.NombrePersona = nombrePersona;
                    selected.Apellido = apellido;
                    selected.Activo = chkActivo.Checked;
                    if (!string.IsNullOrEmpty(password))
                    {
                        selected.Password = password; // Will be hashed in BLL
                    }
                    else
                    {
                        selected.Password = null; // Will trigger BLL to keep existing in ActualizarUsuarioYRol
                    }

                    usuarioBLL.ActualizarUsuarioYRol(selected, idRol);
                    MessageBox.Show("Usuario actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBloquear_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsuarios.CurrentRow == null) return;
                Usuario selected = dgvUsuarios.CurrentRow.DataBoundItem as Usuario;
                if (selected == null) return;

                int minutos = 5;
                switch (cmbTiempoBloqueo.SelectedIndex)
                {
                    case 0: minutos = 5; break;
                    case 1: minutos = 60; break;
                    case 2: minutos = 1440; break;
                }

                usuarioBLL.BloquearUsuario(selected.Nombre, minutos);
                MessageBox.Show("Usuario bloqueado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarUsuarios();
                CargarUsuarioSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al bloquear: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsuarios.CurrentRow == null) return;
                Usuario selected = dgvUsuarios.CurrentRow.DataBoundItem as Usuario;
                if (selected == null) return;

                usuarioBLL.DesbloquearUsuario(selected.Nombre);
                MessageBox.Show("Usuario desbloqueado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarUsuarios();
                CargarUsuarioSeleccionado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desbloquear: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
