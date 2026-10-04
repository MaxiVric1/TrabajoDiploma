using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormCargarMaterialAudiovisual : Form, IObservador
    {
        private const string CARPETA_MATERIAL = "MaterialAudiovisual";
        private static readonly string[] EXTENSIONES_VIDEO = { ".mp4", ".avi", ".mov", ".mkv", ".wmv" };

        private CaptacionBLL captacionBLL = new CaptacionBLL();
        private MaterialAudiovisualBLL materialBLL = new MaterialAudiovisualBLL();
        private InmuebleBLL inmuebleBLL = new InmuebleBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private List<DataGridTraducible> _grillasTraducibles = new List<DataGridTraducible>();
        private bool _cargando; // evita refrescar el detalle mientras se rearma la grilla

        public FormCargarMaterialAudiovisual()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormCargarMaterialAudiovisual"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblInmuebles, BE.NombreControl.FormCargarMaterialAudiovisual_lblInmuebles));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnActualizar, BE.NombreControl.FormCargarMaterialAudiovisual_btnActualizar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpNuevoMaterial, BE.NombreControl.FormCargarMaterialAudiovisual_grpNuevoMaterial));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTextoComercial, BE.NombreControl.FormCargarMaterialAudiovisual_lblTextoComercial));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblArchivos, BE.NombreControl.FormCargarMaterialAudiovisual_lblArchivos));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnAgregarArchivo, BE.NombreControl.FormCargarMaterialAudiovisual_btnAgregarArchivo));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnGuardar, BE.NombreControl.FormCargarMaterialAudiovisual_btnGuardar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpMaterialCargado, BE.NombreControl.FormCargarMaterialAudiovisual_grpMaterialCargado));

            _grillasTraducibles.Clear();
            _grillasTraducibles.Add(new DataGridTraducible(dgvInmuebles, "FormCargarMaterialAudiovisual.dgvInmuebles")
                .ConColumna("NroCaptacion", BE.NombreControl.FormCargarMaterialAudiovisual_dgvInmuebles_nro)
                .ConColumna("Cliente", BE.NombreControl.FormCargarMaterialAudiovisual_dgvInmuebles_cliente)
                .ConColumna("Direccion", BE.NombreControl.FormCargarMaterialAudiovisual_dgvInmuebles_direccion)
                .ConColumna("TipoInmueble", BE.NombreControl.FormCargarMaterialAudiovisual_dgvInmuebles_tipo)
                .ConColumna("ValorFinal", BE.NombreControl.FormCargarMaterialAudiovisual_dgvInmuebles_valorFinal));
            _grillasTraducibles.Add(new DataGridTraducible(dgvArchivos, "FormCargarMaterialAudiovisual.dgvArchivos")
                .ConColumna("colArchivo", BE.NombreControl.FormCargarMaterialAudiovisual_dgvArchivos_archivo)
                .ConColumna("colTipo", BE.NombreControl.FormCargarMaterialAudiovisual_dgvArchivos_tipo)
                .ConColumna("colQuitar", BE.NombreControl.FormCargarMaterialAudiovisual_dgvArchivos_quitar));
            _grillasTraducibles.Add(new DataGridTraducible(dgvMaterialCargado, "FormCargarMaterialAudiovisual.dgvMaterialCargado")
                .ConColumna("TipoArchivo", BE.NombreControl.FormCargarMaterialAudiovisual_dgvMaterialCargado_tipo)
                .ConColumna("RutaArchivo", BE.NombreControl.FormCargarMaterialAudiovisual_dgvMaterialCargado_archivo)
                .ConColumna("FechaCarga", BE.NombreControl.FormCargarMaterialAudiovisual_dgvMaterialCargado_fecha));
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
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
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

        private void FormCargarMaterialAudiovisual_Load(object sender, EventArgs e)
        {
            colTipo.Items.Clear();
            colTipo.Items.AddRange(Enum.GetNames(typeof(TipoMaterial)));
            dgvMaterialCargado.CellFormatting += dgvMaterialCargado_CellFormatting;

            CargarInmuebles();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarInmuebles(InmuebleSeleccionado()?.NroCaptacion);
        }

        // Inmuebles con tasación aprobada = estado Tasado
        private void CargarInmuebles(int? nroASeleccionar = null)
        {
            _cargando = true;
            try
            {
                dgvInmuebles.DataSource = null;
                dgvInmuebles.DataSource = captacionBLL.LeerResumen(EstadoInmueble.Tasado);

                string[] visibles = { "NroCaptacion", "Cliente", "Direccion", "TipoInmueble", "ValorFinal" };
                foreach (DataGridViewColumn col in dgvInmuebles.Columns)
                {
                    col.Visible = visibles.Contains(col.Name);
                }
                if (dgvInmuebles.Columns["ValorFinal"] != null)
                {
                    dgvInmuebles.Columns["ValorFinal"].DefaultCellStyle.Format = "N2";
                    dgvInmuebles.Columns["ValorFinal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
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

            MostrarSeleccion();
        }

        private void dgvInmuebles_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarSeleccion();
        }

        private CaptacionResumen InmuebleSeleccionado()
        {
            return dgvInmuebles.CurrentRow?.DataBoundItem as CaptacionResumen;
        }

        private void MostrarSeleccion()
        {
            CaptacionResumen seleccionado = InmuebleSeleccionado();
            grpNuevoMaterial.Enabled = seleccionado != null;
            CargarMaterialCargado(seleccionado?.IdInmueble);
        }

        // Muestra lo que el inmueble ya tiene: su texto comercial y los archivos cargados
        private void CargarMaterialCargado(int? idInmueble)
        {
            dgvMaterialCargado.DataSource = null;
            txtTextoComercial.Clear();
            if (!idInmueble.HasValue) return;

            try
            {
                Inmueble inmueble = inmuebleBLL.BuscarPorId(idInmueble.Value);
                txtTextoComercial.Text = inmueble?.TextoComercial ?? "";

                List<MaterialAudiovisual> cargado = materialBLL.LeerPorInmueble(idInmueble.Value);
                dgvMaterialCargado.DataSource = cargado;

                string[] visibles = { "TipoArchivo", "RutaArchivo", "FechaCarga" };
                foreach (DataGridViewColumn col in dgvMaterialCargado.Columns)
                {
                    col.Visible = visibles.Contains(col.Name);
                }
                if (dgvMaterialCargado.Columns["FechaCarga"] != null)
                    dgvMaterialCargado.Columns["FechaCarga"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                TraducirGrillas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el material del inmueble: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // En la grilla se muestra solo el nombre del archivo; la ruta completa queda en el tooltip
        private void dgvMaterialCargado_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvMaterialCargado.Columns[e.ColumnIndex].Name != "RutaArchivo" || e.Value == null) return;
            string ruta = e.Value.ToString();
            dgvMaterialCargado.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = ruta;
            e.Value = Path.GetFileName(ruta);
            e.FormattingApplied = true;
        }

        private void dgvMaterialCargado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !(dgvMaterialCargado.Rows[e.RowIndex].DataBoundItem is MaterialAudiovisual material)) return;

            if (!File.Exists(material.RutaArchivo))
            {
                MessageBox.Show("El archivo ya no existe en:\n" + material.RutaArchivo, "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Process.Start(material.RutaArchivo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregarArchivo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Seleccionar fotos o videos";
                dlg.Multiselect = true;
                dlg.Filter = "Multimedia (fotos y videos)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.mp4;*.avi;*.mov;*.mkv;*.wmv|Todos los archivos (*.*)|*.*";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                foreach (string ruta in dlg.FileNames)
                {
                    bool repetido = dgvArchivos.Rows.Cast<DataGridViewRow>()
                        .Any(r => string.Equals(r.Cells["colRuta"].Value as string, ruta, StringComparison.OrdinalIgnoreCase));
                    if (repetido) continue;

                    int i = dgvArchivos.Rows.Add(Path.GetFileName(ruta), TipoSegunExtension(ruta), null, ruta);
                    dgvArchivos.Rows[i].Cells["colArchivo"].ToolTipText = ruta;
                }
            }
        }

        // Propuesta inicial de tipo; el usuario la cambia en la grilla (p. ej. un Recorrido360 suele ser una imagen)
        private string TipoSegunExtension(string ruta)
        {
            string extension = Path.GetExtension(ruta).ToLowerInvariant();
            return (EXTENSIONES_VIDEO.Contains(extension) ? TipoMaterial.Video : TipoMaterial.Foto).ToString();
        }

        private void dgvArchivos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvArchivos.Columns[e.ColumnIndex].Name == "colQuitar")
            {
                dgvArchivos.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            CaptacionResumen seleccionado = InmuebleSeleccionado();
            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un inmueble.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvArchivos.EndEdit(); // confirma un cambio de tipo que esté en edición

            // Sin archivos se guarda solo el texto comercial; si no cambió nada, la BLL lo informa.
            // La lista pudo armarse con otro inmueble seleccionado: se confirma el destino.
            int cantidad = dgvArchivos.Rows.Count;
            string accion = cantidad > 0
                ? $"Se cargarán {cantidad} archivo(s) y se guardará el texto comercial"
                : "Se guardará el texto comercial";
            DialogResult res = MessageBox.Show(
                $"{accion} del inmueble:\n\n" +
                $"{seleccionado.TipoInmueble} - {seleccionado.Direccion} (Captación N° {seleccionado.NroCaptacion})\n\n¿Desea continuar?",
                "Confirmar carga", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;

            List<string> copiados = new List<string>();
            try
            {
                List<MaterialAudiovisual> archivos = new List<MaterialAudiovisual>();
                foreach (DataGridViewRow row in dgvArchivos.Rows)
                {
                    string destino = CopiarArchivo(row.Cells["colRuta"].Value as string, seleccionado.IdInmueble, row.Index);
                    copiados.Add(destino);
                    archivos.Add(new MaterialAudiovisual
                    {
                        TipoArchivo = row.Cells["colTipo"].Value as string,
                        RutaArchivo = destino
                    });
                }

                materialBLL.RegistrarLote(seleccionado.IdInmueble, txtTextoComercial.Text, archivos);

                string exito = archivos.Count > 0
                    ? $"{archivos.Count} archivo(s) cargado(s) con éxito."
                    : "Texto comercial guardado con éxito.";
                MessageBox.Show(exito, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvArchivos.Rows.Clear();
                CargarMaterialCargado(seleccionado.IdInmueble);
            }
            catch (ArgumentException ex)
            {
                BorrarArchivos(copiados);
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                BorrarArchivos(copiados);
                MessageBox.Show("Error al guardar el material: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Copia el archivo a la carpeta de la aplicación para que la ruta guardada no dependa del original
        private string CopiarArchivo(string origen, int idInmueble, int indice)
        {
            if (string.IsNullOrEmpty(origen) || !File.Exists(origen))
                throw new ArgumentException("El archivo seleccionado ya no existe: " + origen);

            string carpeta = Path.Combine(Application.StartupPath, CARPETA_MATERIAL, idInmueble.ToString());
            Directory.CreateDirectory(carpeta);

            string destino = Path.Combine(carpeta, $"{DateTime.Now:yyyyMMddHHmmss}_{indice}_{Path.GetFileName(origen)}");
            File.Copy(origen, destino);
            return destino;
        }

        private void BorrarArchivos(List<string> rutas)
        {
            foreach (string ruta in rutas)
            {
                try
                {
                    if (File.Exists(ruta)) File.Delete(ruta);
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }
    }
}
