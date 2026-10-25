using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormInmueblesPublicados : Form, IObservador
    {
        private const string PUBLICADO = "Publicado";
        private const string DESPUBLICADO = "Despublicado";
        private const string ALQUILADO = "Alquilado"; // no se puede despublicar mientras dure el contrato
        private static readonly Color COLOR_DESPUBLICADO = Color.FromArgb(170, 165, 190);
        private static readonly Color COLOR_ALQUILADO = Color.FromArgb(130, 200, 150);

        private CaptacionBLL captacionBLL = new CaptacionBLL();
        private InmuebleBLL inmuebleBLL = new InmuebleBLL();
        private MaterialAudiovisualBLL materialBLL = new MaterialAudiovisualBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private List<DataGridTraducible> _grillasTraducibles = new List<DataGridTraducible>();
        private bool _cargando; // evita refrescar el detalle mientras se rearma la grilla
        private string _textoSinVistaPrevia; // texto (traducido) del aviso cuando no hay imagen

        public FormInmueblesPublicados()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormInmueblesPublicados"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPublicados, BE.NombreControl.FormInmueblesPublicados_lblPublicados));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnActualizar, BE.NombreControl.FormInmueblesPublicados_btnActualizar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpDetalle, BE.NombreControl.FormInmueblesPublicados_grpDetalle));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTextoComercial, BE.NombreControl.FormInmueblesPublicados_lblTextoComercial));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMaterial, BE.NombreControl.FormInmueblesPublicados_lblMaterial));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblVistaPrevia, BE.NombreControl.FormInmueblesPublicados_lblVistaPrevia));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblSinVistaPrevia, BE.NombreControl.FormInmueblesPublicados_lblSinVistaPrevia));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnDespublicar, BE.NombreControl.FormInmueblesPublicados_btnDespublicar));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRepublicar, BE.NombreControl.FormInmueblesPublicados_btnRepublicar));

            _grillasTraducibles.Clear();
            _grillasTraducibles.Add(new DataGridTraducible(dgvInmuebles, "FormInmueblesPublicados.dgvInmuebles")
                .ConColumna("NroCaptacion", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_nro)
                .ConColumna("Cliente", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_cliente)
                .ConColumna("Direccion", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_direccion)
                .ConColumna("TipoInmueble", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_tipo)
                .ConColumna("TipoPublicacion", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_tipoPublicacion)
                .ConColumna("ValorFinal", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_valorFinal)
                .ConColumna("EstadoPublicacion", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_estado)
                .ConColumna("FechaPublicacion", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_fechaPublicacion)
                .ConColumna("FechaDespublicacion", BE.NombreControl.FormInmueblesPublicados_dgvInmuebles_fechaDespublicacion));
            _grillasTraducibles.Add(new DataGridTraducible(dgvMaterial, "FormInmueblesPublicados.dgvMaterial")
                .ConColumna("TipoArchivo", BE.NombreControl.FormInmueblesPublicados_dgvMaterial_tipo)
                .ConColumna("RutaArchivo", BE.NombreControl.FormInmueblesPublicados_dgvMaterial_archivo)
                .ConColumna("FechaCarga", BE.NombreControl.FormInmueblesPublicados_dgvMaterial_fecha));
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
            _textoSinVistaPrevia = lblSinVistaPrevia.Text;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            LiberarVistaPrevia();
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

        private void FormInmueblesPublicados_Load(object sender, EventArgs e)
        {
            _textoSinVistaPrevia = lblSinVistaPrevia.Text;
            dgvMaterial.CellFormatting += (s, ev) => MaterialVisual.MostrarNombreArchivo(dgvMaterial, ev);
            CargarInmuebles();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarInmuebles(InmuebleSeleccionado()?.NroCaptacion);
        }

        private void CargarInmuebles(int? nroASeleccionar = null)
        {
            _cargando = true;
            try
            {
                dgvInmuebles.DataSource = null;
                dgvInmuebles.DataSource = captacionBLL.LeerPublicados();

                string[] visibles = { "NroCaptacion", "Cliente", "Direccion", "TipoInmueble", "TipoPublicacion", "ValorFinal",
                                      "EstadoPublicacion", "FechaPublicacion", "FechaDespublicacion" };
                foreach (DataGridViewColumn col in dgvInmuebles.Columns)
                {
                    col.Visible = visibles.Contains(col.Name);
                }
                // Proporción del ancho (la grilla reparte todo el ancho): más para la dirección, menos para los datos cortos
                var proporciones = new Dictionary<string, float>
                {
                    { "NroCaptacion", 70 }, { "Cliente", 100 }, { "Direccion", 175 }, { "TipoInmueble", 85 },
                    { "TipoPublicacion", 70 }, { "ValorFinal", 105 }, { "EstadoPublicacion", 85 },
                    { "FechaPublicacion", 100 }, { "FechaDespublicacion", 100 }
                };
                foreach (var p in proporciones)
                {
                    if (dgvInmuebles.Columns[p.Key] != null) dgvInmuebles.Columns[p.Key].FillWeight = p.Value;
                }
                if (dgvInmuebles.Columns["ValorFinal"] != null)
                {
                    dgvInmuebles.Columns["ValorFinal"].DefaultCellStyle.Format = "N2";
                    dgvInmuebles.Columns["ValorFinal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                foreach (string fecha in new[] { "FechaPublicacion", "FechaDespublicacion" })
                {
                    if (dgvInmuebles.Columns[fecha] != null) dgvInmuebles.Columns[fecha].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                TraducirGrillas();

                foreach (DataGridViewRow row in dgvInmuebles.Rows)
                {
                    if (!(row.DataBoundItem is CaptacionResumen r)) continue;
                    if (r.EstadoPublicacion == DESPUBLICADO) row.DefaultCellStyle.ForeColor = COLOR_DESPUBLICADO;
                    else if (r.EstadoPublicacion == ALQUILADO) row.DefaultCellStyle.ForeColor = COLOR_ALQUILADO;
                    if (nroASeleccionar.HasValue && r.NroCaptacion == nroASeleccionar.Value)
                    {
                        row.Selected = true;
                        dgvInmuebles.CurrentCell = row.Cells["NroCaptacion"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los inmuebles publicados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
            }

            MostrarDetalle();
        }

        private void dgvInmuebles_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarDetalle();
        }

        private CaptacionResumen InmuebleSeleccionado()
        {
            return dgvInmuebles.CurrentRow?.DataBoundItem as CaptacionResumen;
        }

        private void MostrarDetalle()
        {
            CaptacionResumen seleccionado = InmuebleSeleccionado();
            txtTextoComercial.Clear();
            dgvMaterial.DataSource = null;
            MostrarSinVistaPrevia(_textoSinVistaPrevia);
            ActualizarBotones(seleccionado);
            if (seleccionado == null) return;

            try
            {
                Inmueble inmueble = inmuebleBLL.BuscarPorId(seleccionado.IdInmueble);
                txtTextoComercial.Text = inmueble?.TextoComercial ?? "";

                List<MaterialAudiovisual> material = materialBLL.LeerPorInmueble(seleccionado.IdInmueble);
                dgvMaterial.DataSource = material;
                string[] visibles = { "TipoArchivo", "RutaArchivo", "FechaCarga" };
                foreach (DataGridViewColumn col in dgvMaterial.Columns)
                {
                    col.Visible = visibles.Contains(col.Name);
                }
                if (dgvMaterial.Columns["FechaCarga"] != null)
                    dgvMaterial.Columns["FechaCarga"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                TraducirGrillas();

                Image imagen = MaterialVisual.PrimeraFoto(material, out string motivo);
                if (imagen == null)
                {
                    MostrarSinVistaPrevia(motivo);
                }
                else
                {
                    LiberarVistaPrevia();
                    picVistaPrevia.Image = imagen;
                    lblSinVistaPrevia.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle del inmueble: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Publicado -> se puede despublicar; despublicado -> se puede republicar (ocupan el mismo lugar)
        private void ActualizarBotones(CaptacionResumen seleccionado)
        {
            bool despublicado = seleccionado?.EstadoPublicacion == DESPUBLICADO;
            btnRepublicar.Visible = despublicado;
            btnDespublicar.Visible = !despublicado;
            btnDespublicar.Enabled = seleccionado?.EstadoPublicacion == PUBLICADO;
        }

        private void MostrarSinVistaPrevia(string mensaje)
        {
            LiberarVistaPrevia();
            lblSinVistaPrevia.Text = mensaje;
            lblSinVistaPrevia.Visible = true;
        }

        private void LiberarVistaPrevia()
        {
            Image anterior = picVistaPrevia.Image;
            picVistaPrevia.Image = null;
            anterior?.Dispose();
        }

        private void dgvMaterial_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvMaterial.Rows[e.RowIndex].DataBoundItem is MaterialAudiovisual material)
                MaterialVisual.AbrirArchivo(material.RutaArchivo);
        }

        private void btnDespublicar_Click(object sender, EventArgs e)
        {
            CaptacionResumen seleccionado = InmuebleSeleccionado();
            if (seleccionado == null) return;

            DialogResult res = MessageBox.Show(
                $"¿Confirma que desea despublicar el inmueble?\n\n" +
                $"{seleccionado.TipoInmueble} - {seleccionado.Direccion} (Captación N° {seleccionado.NroCaptacion})\n\n" +
                "Dejará de estar publicado y volverá al estado Tasado. Podrá republicarlo desde esta pantalla.",
                "Confirmar despublicación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes) return;

            try
            {
                inmuebleBLL.Despublicar(seleccionado.IdInmueble);
                MessageBox.Show($"Inmueble despublicado: {seleccionado.Direccion} (Captación N° {seleccionado.NroCaptacion}).",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo despublicar el inmueble: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CargarInmuebles(seleccionado.NroCaptacion); // refleja el estado actual, haya funcionado o no
        }

        private void btnRepublicar_Click(object sender, EventArgs e)
        {
            CaptacionResumen seleccionado = InmuebleSeleccionado();
            if (seleccionado == null) return;

            string confirmacion =
                $"¿Confirma que desea volver a publicar el inmueble?\n\n" +
                $"{seleccionado.TipoInmueble} - {seleccionado.Direccion} (Captación N° {seleccionado.NroCaptacion})\n" +
                $"Archivos cargados: {seleccionado.CantidadArchivos}";
            if (string.IsNullOrWhiteSpace(txtTextoComercial.Text))
                confirmacion += "\n\nATENCIÓN: el inmueble no tiene texto comercial.";

            if (MessageBox.Show(confirmacion, "Confirmar republicación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                inmuebleBLL.Republicar(seleccionado.IdInmueble);
                MessageBox.Show($"Inmueble publicado nuevamente: {seleccionado.Direccion} (Captación N° {seleccionado.NroCaptacion}).",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo republicar el inmueble: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CargarInmuebles(seleccionado.NroCaptacion);
        }
    }
}
