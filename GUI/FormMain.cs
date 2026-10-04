using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormMain : Form, IObservador
    {
        private UsuarioBLL usuarioBLL = new UsuarioBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private bool _isChangingLanguage = false;

        public FormMain()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;
            this.Text = "TpIngSoft - Sistema de Gestión";

            RegistrarControlesTraducibles();

            CargarIdiomas();

            // Subscribe to language changes
            GestorIdioma.Instancia.Adjuntar(this);

            ConfigurarMenu();
            this.Shown += FormMain_Shown;
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormMain"));

            var menuTraducible = new MenuStripTraducible("FormMain.menuStrip1")
                .ConItem(archivoToolStripMenuItem, BE.NombreControl.FormMain_archivoToolStripMenuItem)
                .ConItem(logoutToolStripMenuItem, BE.NombreControl.FormMain_logoutToolStripMenuItem)
                .ConItem(salirToolStripMenuItem, BE.NombreControl.FormMain_salirToolStripMenuItem)
                .ConItem(adminToolStripMenuItem, BE.NombreControl.FormMain_adminToolStripMenuItem)
                .ConItem(gestionUsuariosToolStripMenuItem, BE.NombreControl.FormMain_gestionUsuariosToolStripMenuItem)
                .ConItem(bitacoraToolStripMenuItem, BE.NombreControl.FormMain_bitacoraToolStripMenuItem)
                .ConItem(controlCambiosToolStripMenuItem, BE.NombreControl.FormMain_controlCambiosToolStripMenuItem)
                .ConItem(gestionPerfilesToolStripMenuItem, BE.NombreControl.FormMain_gestionPerfilesToolStripMenuItem)
                .ConItem(gestionIdiomasToolStripMenuItem, BE.NombreControl.FormMain_gestionIdiomasToolStripMenuItem)
                .ConItem(verificarIntegridadToolStripMenuItem, BE.NombreControl.FormMain_verificarIntegridadToolStripMenuItem)
                .ConItem(pn1ToolStripMenuItem, BE.NombreControl.FormMain_pn1ToolStripMenuItem)
                .ConItem(registrarCaptacionToolStripMenuItem, BE.NombreControl.FormMain_registrarCaptacionToolStripMenuItem)
                .ConItem(gestionarCaptacionesToolStripMenuItem, BE.NombreControl.FormMain_gestionarCaptacionesToolStripMenuItem)
                .ConItem(validarTasacionToolStripMenuItem, BE.NombreControl.FormMain_validarTasacionToolStripMenuItem)
                .ConItem(cargarMaterialAudiovisualToolStripMenuItem, BE.NombreControl.FormMain_cargarMaterialAudiovisualToolStripMenuItem)
                .ConItem(publicarInmuebleToolStripMenuItem, BE.NombreControl.FormMain_publicarInmuebleToolStripMenuItem);

            _controlesTraducibles.Add(menuTraducible);
        }

        private void CargarIdiomas()
        {
            _isChangingLanguage = true;
            try
            {
                cmbIdioma.Items.Clear();
                IdiomaBLL bll = new IdiomaBLL();
                List<Idioma> activos = bll.ObtenerIdiomasActivos();
                foreach (var id in activos)
                {
                    cmbIdioma.Items.Add(id);
                }

                if (GestorIdioma.Instancia.IdiomaActual != null)
                {
                    for (int i = 0; i < cmbIdioma.Items.Count; i++)
                    {
                        if (((Idioma)cmbIdioma.Items[i]).Id == GestorIdioma.Instancia.IdiomaActual.Id)
                        {
                            cmbIdioma.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else if (cmbIdioma.Items.Count > 0)
                {
                    cmbIdioma.SelectedIndex = 0;
                }
            }
            finally
            {
                _isChangingLanguage = false;
            }
        }

        private void CmbIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isChangingLanguage) return;

            if (cmbIdioma.SelectedItem is Idioma seleccionado)
            {
                if (SessionManager.Instance.EstaLogueado())
                {
                    var user = SessionManager.Instance.UsuarioActual;
                    user.IdIdioma = seleccionado.Id;
                    new IdiomaBLL().GuardarIdiomaUsuario(user.Id, seleccionado.Id);
                }

                GestorIdioma.Instancia.CambiarIdioma(seleccionado);
            }
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }

            if (SessionManager.Instance.EstaLogueado())
            {
                string usuarioLabel = "Usuario: ";
                if (traducciones.TryGetValue(BE.NombreControl.FormMain_lblSesionInfo, out string trans))
                {
                    usuarioLabel = trans;
                }
                lblSesionInfo.Text = $"{usuarioLabel}{SessionManager.Instance.UsuarioActual.Nombre}";
            }
            else
            {
                lblSesionInfo.Text = "Usuario no autenticado";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void ConfigurarMenu()
        {
            if (SERVICIOS.SessionManager.Instance.EstaLogueado())
            {
                string usuarioLabel = "Usuario: ";
                var traducciones = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
                if (traducciones.TryGetValue(BE.NombreControl.FormMain_lblSesionInfo, out string trans))
                {
                    usuarioLabel = trans;
                }
                lblSesionInfo.Text = $"{usuarioLabel}{SERVICIOS.SessionManager.Instance.UsuarioActual.Nombre}";

                adminToolStripMenuItem.Visible = SERVICIOS.SessionManager.Instance.TienePermiso("AccesoAdmin");
                gestionUsuariosToolStripMenuItem.Visible = SERVICIOS.SessionManager.Instance.TienePermiso("GestionUsuarios");
                controlCambiosToolStripMenuItem.Visible = SERVICIOS.SessionManager.Instance.TienePermiso("AccesoAdmin");
                gestionPerfilesToolStripMenuItem.Visible = SERVICIOS.SessionManager.Instance.TienePermiso("AccesoAdmin");
                gestionIdiomasToolStripMenuItem.Visible = SERVICIOS.SessionManager.Instance.TienePermiso("AccesoAdmin");
                verificarIntegridadToolStripMenuItem.Visible = SERVICIOS.SessionManager.Instance.TienePermiso("AccesoAdmin");
            }
            else
            {
                lblSesionInfo.Text = "Usuario no autenticado";
                adminToolStripMenuItem.Visible = false;
                gestionUsuariosToolStripMenuItem.Visible = false;
                controlCambiosToolStripMenuItem.Visible = false;
                gestionPerfilesToolStripMenuItem.Visible = false;
                gestionIdiomasToolStripMenuItem.Visible = false;
                verificarIntegridadToolStripMenuItem.Visible = false;
            }

            // Re-sync selected language in combobox when session config updates
            CargarIdiomas();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            ChequearErroresIntegridad();
        }

        private void ChequearErroresIntegridad()
        {
            if (!SERVICIOS.SessionManager.Instance.EstaLogueado() || 
                !SERVICIOS.SessionManager.Instance.TienePermiso("AccesoAdmin"))
            {
                return;
            }

            var rep = SERVICIOS.SessionManager.Instance.ReporteIntegridadTemporal;
            if (rep == null) return;

            string usuariosCorruptosStr = string.Join(", ", rep.UsuariosCorruptos);

            DialogResult res = MessageBox.Show(
                $"ATENCIÓN: Se detectaron fallos de integridad de datos en la tabla Usuario.\n\n" +
                $"Usuarios afectados: {usuariosCorruptosStr}\n\n" +
                $"¿Desea recalcular los dígitos verificadores ahora?\n" +
                $"Si elige 'No', la base de datos permanecerá en este estado y se sugiere cerrar la aplicación para restaurar un backup desde la CLI.",
                "Fallo de Integridad de Datos",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (res == DialogResult.Yes)
            {
                try
                {
                    new BLL.IntegridadBLL().RepararIntegridad();
                    SERVICIOS.SessionManager.Instance.ReporteIntegridadTemporal = null;
                    MessageBox.Show(
                        "Dígitos verificadores recalculados y restaurados con éxito.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al recalcular dígitos verificadores: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            usuarioBLL.Logout();
            this.Hide();
            if (new FormLogin().ShowDialog() == DialogResult.OK)
            {
                ConfigurarMenu();
                this.Show();
                ChequearErroresIntegridad();
            }
            else
            {
                this.Close();
            }
        }

        private void gestionUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormGestionUsuarios frm = new FormGestionUsuarios();
            frm.MdiParent = this;
            frm.Show();
        }

        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBitacora frm = new FormBitacora();
            frm.MdiParent = this;
            frm.Show();
        }

        private void controlCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHistorialUsuario frm = new FormHistorialUsuario();
            frm.MdiParent = this;
            frm.Show();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void gestionPerfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormGestionRoles frm = new FormGestionRoles();
            frm.MdiParent = this;
            frm.Show();
        }

        private void GestionIdiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormGestionIdiomas frm = new FormGestionIdiomas();
            frm.MdiParent = this;
            frm.FormClosed += (s, args) => CargarIdiomas();
            frm.Show();
        }

        private void verificarIntegridadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var reporte = new BLL.IntegridadBLL().VerificarIntegridad();
                if (reporte.EsValido)
                {
                    MessageBox.Show("La integridad de la base de datos es correcta (Todos los dígitos verificadores coinciden).", 
                        "Verificación de Integridad", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string usuariosCorruptosStr = string.Join(", ", reporte.UsuariosCorruptos);
                    string msg = reporte.DvvInvalido
                        ? "ATENCIÓN: Se detectaron fallos de integridad vertical (DVV) en la tabla Usuario."
                        : $"ATENCIÓN: Se detectaron fallos de integridad horizontal (DVH) en la tabla Usuario.\n\nUsuarios afectados: {usuariosCorruptosStr}";

                    DialogResult res = MessageBox.Show(
                        msg + "\n\n¿Desea recalcular los dígitos verificadores ahora para reparar la base de datos?\n" +
                        "Nota: Esto requiere privilegios de Administrador y se registrará en la bitácora.",
                        "Fallo de Integridad Detectado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (res == DialogResult.Yes)
                    {
                        new BLL.IntegridadBLL().RepararIntegridad();
                        MessageBox.Show("Dígitos verificadores recalculados y restaurados con éxito.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar integridad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void registrarCaptacionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRegistrarCaptacion frm = new FormRegistrarCaptacion();
            frm.MdiParent = this;
            frm.Show();
        }

        private void gestionarCaptacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormGestionarCaptaciones frm = new FormGestionarCaptaciones();
            frm.MdiParent = this;
            frm.Show();
        }

        private void validarTasacionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormValidarTasacion frm = new FormValidarTasacion();
            frm.MdiParent = this;
            frm.Show();
        }

        private void cargarMaterialAudiovisualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCargarMaterialAudiovisual frm = new FormCargarMaterialAudiovisual();
            frm.MdiParent = this;
            frm.Show();
        }

        private void publicarInmuebleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPublicarInmueble frm = new FormPublicarInmueble();
            frm.MdiParent = this;
            frm.Show();
        }
    }
}
