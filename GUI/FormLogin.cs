using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
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
            // Se arma con el tamaño ya escalado según los DPI de Windows
            pictureBoxLogo.Image = CrearLogo(Math.Min(pictureBoxLogo.ClientSize.Width, pictureBoxLogo.ClientSize.Height));
        }

        // Logo de LandSmart (GUI\Resources\logo-landsmart.png, incrustado en el exe) en una tarjeta de esquinas
        // redondeadas: el fondo claro del propio logo hace de tarjeta sobre el fondo oscuro del formulario
        private static Image CrearLogo(int lado)
        {
            using (Stream recurso = typeof(FormLogin).Assembly.GetManifestResourceStream("TpIngSoft.logo-landsmart.png"))
            {
                if (recurso == null || lado <= 0) return null;
                using (Image original = Image.FromStream(recurso))
                using (Bitmap escalado = new Bitmap(lado, lado))
                {
                    using (Graphics g = Graphics.FromImage(escalado))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.DrawImage(original, 0, 0, lado, lado);
                    }

                    Bitmap tarjeta = new Bitmap(lado, lado);
                    using (Graphics g = Graphics.FromImage(tarjeta))
                    using (GraphicsPath borde = Redondeado(lado, lado / 7f))
                    using (TextureBrush pincel = new TextureBrush(escalado))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.FillPath(pincel, borde);
                    }
                    return tarjeta;
                }
            }
        }

        private static GraphicsPath Redondeado(int lado, float radio)
        {
            float d = radio * 2, fin = lado - 1;
            GraphicsPath p = new GraphicsPath();
            p.AddArc(0, 0, d, d, 180, 90);
            p.AddArc(fin - d, 0, d, d, 270, 90);
            p.AddArc(fin - d, fin - d, d, d, 0, 90);
            p.AddArc(0, fin - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
