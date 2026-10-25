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
    public partial class FormPublicarInmueble : Form, IObservador
    {
        private CaptacionBLL captacionBLL = new CaptacionBLL();
        private InmuebleBLL inmuebleBLL = new InmuebleBLL();
        private MaterialAudiovisualBLL materialBLL = new MaterialAudiovisualBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private List<DataGridTraducible> _grillasTraducibles = new List<DataGridTraducible>();
        private bool _cargando; // evita refrescar el detalle mientras se rearma la grilla
        private string _textoSinVistaPrevia; // texto (traducido) del aviso cuando no hay imagen

        public FormPublicarInmueble()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormPublicarInmueble"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblListos, BE.NombreControl.FormPublicarInmueble_lblListos));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnActualizar, BE.NombreControl.FormPublicarInmueble_btnActualizar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpDetalle, BE.NombreControl.FormPublicarInmueble_grpDetalle));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTextoComercial, BE.NombreControl.FormPublicarInmueble_lblTextoComercial));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMaterial, BE.NombreControl.FormPublicarInmueble_lblMaterial));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblVistaPrevia, BE.NombreControl.FormPublicarInmueble_lblVistaPrevia));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblSinVistaPrevia, BE.NombreControl.FormPublicarInmueble_lblSinVistaPrevia));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnPublicar, BE.NombreControl.FormPublicarInmueble_btnPublicar));

            _grillasTraducibles.Clear();
            _grillasTraducibles.Add(new DataGridTraducible(dgvInmuebles, "FormPublicarInmueble.dgvInmuebles")
                .ConColumna("NroCaptacion", BE.NombreControl.FormPublicarInmueble_dgvInmuebles_nro)
                .ConColumna("Cliente", BE.NombreControl.FormPublicarInmueble_dgvInmuebles_cliente)
                .ConColumna("Direccion", BE.NombreControl.FormPublicarInmueble_dgvInmuebles_direccion)
                .ConColumna("TipoInmueble", BE.NombreControl.FormPublicarInmueble_dgvInmuebles_tipo)
                .ConColumna("TipoPublicacion", BE.NombreControl.FormPublicarInmueble_dgvInmuebles_tipoPublicacion)
                .ConColumna("ValorFinal", BE.NombreControl.FormPublicarInmueble_dgvInmuebles_valorFinal)
                .ConColumna("CantidadArchivos", BE.NombreControl.FormPublicarInmueble_dgvInmuebles_archivos));
            _grillasTraducibles.Add(new DataGridTraducible(dgvMaterial, "FormPublicarInmueble.dgvMaterial")
                .ConColumna("TipoArchivo", BE.NombreControl.FormPublicarInmueble_dgvMaterial_tipo)
                .ConColumna("RutaArchivo", BE.NombreControl.FormPublicarInmueble_dgvMaterial_archivo)
                .ConColumna("FechaCarga", BE.NombreControl.FormPublicarInmueble_dgvMaterial_fecha));
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

        private void FormPublicarInmueble_Load(object sender, EventArgs e)
        {
            _textoSinVistaPrevia = lblSinVistaPrevia.Text;
            dgvMaterial.CellFormatting += dgvMaterial_CellFormatting;
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
                dgvInmuebles.DataSource = captacionBLL.LeerListosParaPublicar();

                string[] visibles = { "NroCaptacion", "Cliente", "Direccion", "TipoInmueble", "TipoPublicacion", "ValorFinal", "CantidadArchivos" };
                foreach (DataGridViewColumn col in dgvInmuebles.Columns)
                {
                    col.Visible = visibles.Contains(col.Name);
                }
                if (dgvInmuebles.Columns["ValorFinal"] != null)
                {
                    dgvInmuebles.Columns["ValorFinal"].DefaultCellStyle.Format = "N2";
                    dgvInmuebles.Columns["ValorFinal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvInmuebles.Columns["CantidadArchivos"] != null)
                    dgvInmuebles.Columns["CantidadArchivos"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                TraducirGrillas();

                if (nroASeleccionar.HasValue)
                {
                    foreach (DataGridViewRow row in dgvInmuebles.Rows)
                    {
                        if (row.DataBoundItem is CaptacionResumen r && r.NroCaptacion == nroASeleccionar.Value)
                        {
                            row.Selected = true;
                            dgvInmuebles.CurrentCell = row.Cells["NroCaptacion"];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los inmuebles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            btnPublicar.Enabled = seleccionado != null;
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

                MostrarVistaPrevia(material);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle del inmueble: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarVistaPrevia(List<MaterialAudiovisual> material)
        {
            Image imagen = MaterialVisual.PrimeraFoto(material, out string motivo);
            if (imagen == null)
            {
                MostrarSinVistaPrevia(motivo);
                return;
            }
            LiberarVistaPrevia();
            picVistaPrevia.Image = imagen;
            lblSinVistaPrevia.Visible = false;
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

        private void dgvMaterial_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            MaterialVisual.MostrarNombreArchivo(dgvMaterial, e);
        }

        private void dgvMaterial_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvMaterial.Rows[e.RowIndex].DataBoundItem is MaterialAudiovisual material)
                MaterialVisual.AbrirArchivo(material.RutaArchivo);
        }

        private void btnPublicar_Click(object sender, EventArgs e)
        {
            CaptacionResumen seleccionado = InmuebleSeleccionado();
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un inmueble.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string confirmacion =
                $"¿Confirma la publicación oficial del inmueble?\n\n" +
                $"{seleccionado.TipoInmueble} - {seleccionado.Direccion} (Captación N° {seleccionado.NroCaptacion})\n" +
                $"Cliente: {seleccionado.Cliente}\n" +
                $"Valor final de tasación: {(seleccionado.ValorFinal.HasValue ? seleccionado.ValorFinal.Value.ToString("N2") : "-")}\n" +
                $"Archivos cargados: {seleccionado.CantidadArchivos}";
            // No bloquea la publicación, pero se avisa antes de confirmar
            if (string.IsNullOrWhiteSpace(txtTextoComercial.Text))
                confirmacion += "\n\nATENCIÓN: el inmueble no tiene texto comercial.";

            if (MessageBox.Show(confirmacion, "Confirmar publicación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                inmuebleBLL.Publicar(seleccionado.IdInmueble);
                MessageBox.Show($"Inmueble publicado con éxito: {seleccionado.Direccion} (Captación N° {seleccionado.NroCaptacion}).",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarInmuebles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo publicar el inmueble: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CargarInmuebles(seleccionado.NroCaptacion); // el inmueble pudo haber cambiado de estado
            }
        }
    }
}
