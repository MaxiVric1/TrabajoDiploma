using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using SERVICIOS;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormLogin : Form, IObservador
    {
        private UsuarioBLL usuarioBLL = new UsuarioBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private bool _isChangingLanguage = false;

        public FormLogin()
        {
            InitializeComponent();

            RegistrarControlesTraducibles();

            // Set default language if none is active
            if (GestorIdioma.Instancia.IdiomaActual == null)
            {
                IdiomaBLL bll = new IdiomaBLL();
                var activos = bll.ObtenerIdiomasActivos();
                if (activos.Count > 0)
                {
                    GestorIdioma.Instancia.CambiarIdioma(activos[0]);
                }
            }

            CargarIdiomas();
            
            // Subscribe to language changes
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormLogin"));
            _controlesTraducibles.Add(new EtiquetaTraducible(label1, BE.NombreControl.FormLogin_label1));
            _controlesTraducibles.Add(new EtiquetaTraducible(label2, BE.NombreControl.FormLogin_label2));
            _controlesTraducibles.Add(new EtiquetaTraducible(label3, BE.NombreControl.FormLogin_label3));
            _controlesTraducibles.Add(new EtiquetaTraducible(buttonLogin, BE.NombreControl.FormLogin_buttonLogin));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblIdiomaLogin, BE.NombreControl.FormLogin_labelIdioma));
        }

        private void CargarIdiomas()
        {
            _isChangingLanguage = true;
            try
            {
                cmbIdiomaLogin.Items.Clear();
                IdiomaBLL bll = new IdiomaBLL();
                List<Idioma> activos = bll.ObtenerIdiomasActivos();
                foreach (var id in activos)
                {
                    cmbIdiomaLogin.Items.Add(id);
                }

                if (GestorIdioma.Instancia.IdiomaActual != null)
                {
                    for (int i = 0; i < cmbIdiomaLogin.Items.Count; i++)
                    {
                        if (((Idioma)cmbIdiomaLogin.Items[i]).Id == GestorIdioma.Instancia.IdiomaActual.Id)
                        {
                            cmbIdiomaLogin.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else if (cmbIdiomaLogin.Items.Count > 0)
                {
                    cmbIdiomaLogin.SelectedIndex = 0;
                }
            }
            finally
            {
                _isChangingLanguage = false;
            }
        }

        private void CmbIdiomaLogin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isChangingLanguage) return;

            if (cmbIdiomaLogin.SelectedItem is Idioma seleccionado)
            {
                GestorIdioma.Instancia.CambiarIdioma(seleccionado);
            }
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

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = textBoxNombre.Text.Trim();
                string pass = textBoxPass.Text.Trim();

                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(pass))
                {
                    MostrarError("Por favor, complete todos los campos.");
                    return;
                }

                if (usuarioBLL.Login(nombre, pass))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MostrarError("Usuario o contraseña incorrectos.");
                }
            }
            catch (Exception ex)
            {
                GestorBitacora.Instance.RegistrarEvento(null, "Error Crítico", "Error en login: " + ex.Message);
                MostrarError("Error: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            label4.Text = mensaje;
            label4.Visible = true;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
