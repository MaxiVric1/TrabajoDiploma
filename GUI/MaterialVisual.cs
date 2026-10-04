using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BE;

namespace TpIngSoft
{
    // Funciones de presentación del material audiovisual compartidas por los formularios del PN1
    internal static class MaterialVisual
    {
        // Primera foto cargada que se pueda abrir; si una no se puede (archivo inexistente o formato que
        // GDI+ no reconoce), prueba con la siguiente. Devuelve null y el motivo si no hay ninguna.
        public static Image PrimeraFoto(IEnumerable<MaterialAudiovisual> material, out string motivo)
        {
            List<MaterialAudiovisual> fotos = material
                .Where(m => m.TipoArchivo == TipoMaterial.Foto.ToString())
                .OrderBy(m => m.FechaCarga).ThenBy(m => m.IdMaterial)
                .ToList();

            motivo = null;
            if (fotos.Count == 0)
            {
                motivo = "No hay fotos cargadas";
                return null;
            }

            foreach (MaterialAudiovisual foto in fotos)
            {
                Image imagen = CargarImagen(foto.RutaArchivo);
                if (imagen != null) return imagen;
            }

            motivo = "Vista previa no disponible: las fotos no se encuentran o tienen un formato no soportado";
            return null;
        }

        // Carga la imagen en memoria para no dejar el archivo bloqueado; null si no se puede leer
        public static Image CargarImagen(string ruta)
        {
            if (string.IsNullOrEmpty(ruta) || !File.Exists(ruta)) return null;
            try
            {
                using (FileStream fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (Image original = Image.FromStream(fs))
                {
                    return new Bitmap(original);
                }
            }
            catch (ArgumentException) { return null; }      // formato que GDI+ no reconoce
            catch (OutOfMemoryException) { return null; }   // GDI+ también lo informa así para imágenes inválidas
            catch (IOException) { return null; }
            catch (UnauthorizedAccessException) { return null; }
        }

        // Para el CellFormatting de una grilla de material: muestra solo el nombre del archivo y deja la ruta en el tooltip
        public static void MostrarNombreArchivo(DataGridView grilla, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || grilla.Columns[e.ColumnIndex].Name != "RutaArchivo" || e.Value == null) return;
            string ruta = e.Value.ToString();
            grilla.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = ruta;
            e.Value = Path.GetFileName(ruta);
            e.FormattingApplied = true;
        }

        // Abre el archivo con el programa asociado de Windows
        public static void AbrirArchivo(string ruta)
        {
            if (!File.Exists(ruta))
            {
                MessageBox.Show("El archivo ya no existe en:\n" + ruta, "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Process.Start(ruta);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
