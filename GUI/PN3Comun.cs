using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE;

namespace TpIngSoft
{
    // Utilidades compartidas por los formularios del PN3
    internal static class PN3Comun
    {
        public static readonly Color FONDO = Color.FromArgb(38, 31, 50);
        public static readonly Color CAMPO = Color.FromArgb(65, 57, 79);
        public static readonly Color BOTON = Color.FromArgb(89, 75, 160);
        public static readonly Color VERDE = Color.FromArgb(130, 200, 150);
        public static readonly Color ROJO = Color.FromArgb(255, 130, 130);
        public static readonly Color AMARILLO = Color.FromArgb(255, 200, 120);

        // Mismo estilo de grilla que el resto de la aplicación (FormGestionarContratos)
        public static void EstiloGrilla(DataGridView grilla)
        {
            grilla.AllowUserToAddRows = grilla.AllowUserToDeleteRows = false;
            grilla.ReadOnly = true;
            grilla.MultiSelect = false;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grilla.RowHeadersVisible = false;
            grilla.BackgroundColor = FONDO;
            grilla.GridColor = Color.Black;
            grilla.EnableHeadersVisualStyles = false;
            grilla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grilla.ColumnHeadersDefaultCellStyle.BackColor = BOTON;
            grilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grilla.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(98, 93, 115);
            grilla.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grilla.DefaultCellStyle.BackColor = CAMPO;
            grilla.DefaultCellStyle.ForeColor = Color.White;
            grilla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(134, 129, 158);
            grilla.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // Formato de columnas: montos (N2, a la derecha), porcentajes y fechas
        public static void FormatoColumnas(DataGridView grilla, string[] montos = null, string[] fechas = null)
        {
            foreach (string c in montos ?? new string[0])
                if (grilla.Columns.Contains(c))
                {
                    grilla.Columns[c].DefaultCellStyle.Format = "N2";
                    grilla.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            foreach (string c in fechas ?? new string[0])
                if (grilla.Columns.Contains(c)) grilla.Columns[c].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        public static string Usd(decimal? valor) => valor.HasValue ? $"USD {valor.Value:N2}" : "-";

        public static string Pesos(decimal? valor) => valor.HasValue ? $"${valor.Value:N2}" : "-";

        public static string Texto(EstadoProyecto? estado) =>
            estado.HasValue ? BLL.ProyectoInversionBLL.Texto(estado.Value) : "Sin proyecto";

        // Mapa de OpenStreetMap en segundo plano (no traba la pantalla mientras se descargan las teselas)
        public static void CargarMapa(PictureBox destino, decimal? latitud, decimal? longitud)
        {
            Image anterior = destino.Image;
            destino.Image = null;
            anterior?.Dispose();
            if (!latitud.HasValue || !longitud.HasValue)
            {
                destino.Image = Aviso(destino, "Inmueble sin ubicación en el mapa");
                return;
            }
            int ancho = destino.ClientSize.Width, alto = destino.ClientSize.Height;
            destino.Image = Aviso(destino, "Cargando mapa...");
            object pedido = new object();
            destino.Tag = pedido;
            Task.Run(() =>
            {
                try { return (Image)SERVICIOS.MapaOSM.Generar((double)latitud.Value, (double)longitud.Value, ancho, alto); }
                catch (Exception ex) { return Aviso(destino, "No se pudo cargar el mapa: " + ex.Message, ancho, alto); }
            }).ContinueWith(t =>
            {
                if (destino.IsDisposed || destino.Tag != pedido) { t.Result.Dispose(); return; } // ya se pidió otro
                Image vieja = destino.Image;
                destino.Image = t.Result;
                vieja?.Dispose();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static Image Aviso(PictureBox destino, string texto, int ancho = 0, int alto = 0)
        {
            Bitmap b = new Bitmap(Math.Max(1, ancho > 0 ? ancho : destino.ClientSize.Width), Math.Max(1, alto > 0 ? alto : destino.ClientSize.Height));
            using (Graphics g = Graphics.FromImage(b))
            using (Font f = new Font("Microsoft Sans Serif", 8.25f))
            {
                g.Clear(CAMPO);
                TextRenderer.DrawText(g, texto, f, new Rectangle(0, 0, b.Width, b.Height), Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
            }
            return b;
        }

        // ---------- Diálogos simples con el estilo de la aplicación ----------

        private static Form Dialogo(string titulo, int ancho, int alto)
        {
            return new Form
            {
                Text = titulo,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(ancho, alto),
                BackColor = FONDO,
                ForeColor = Color.White
            };
        }

        private static void Botones(Form dialogo, int y, string aceptar = "Aceptar")
        {
            Button ok = new Button { Text = aceptar, DialogResult = DialogResult.OK, Location = new Point(dialogo.ClientSize.Width - 204, y), Size = new Size(90, 28) };
            Button cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(dialogo.ClientSize.Width - 102, y), Size = new Size(90, 28) };
            foreach (Button b in new[] { ok, cancelar })
            {
                b.FlatStyle = FlatStyle.Popup;
                b.BackColor = BOTON;
                b.ForeColor = Color.White;
                dialogo.Controls.Add(b);
            }
            dialogo.AcceptButton = ok;
            dialogo.CancelButton = cancelar;
        }

        // Texto (null si se cancela). multilinea: para motivos u observaciones.
        public static string PedirTexto(IWin32Window owner, string titulo, string etiqueta, int largoMaximo, bool multilinea = false, bool obligatorio = false)
        {
            int altoTexto = multilinea ? 80 : 20;
            using (Form d = Dialogo(titulo, 440, 86 + altoTexto))
            {
                d.Controls.Add(new Label { Text = etiqueta, Location = new Point(12, 12), Size = new Size(416, 20) });
                TextBox txt = new TextBox
                {
                    Location = new Point(12, 36), Size = new Size(416, altoTexto), MaxLength = largoMaximo, Multiline = multilinea,
                    BackColor = CAMPO, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle,
                    ScrollBars = multilinea ? ScrollBars.Vertical : ScrollBars.None
                };
                d.Controls.Add(txt);
                Botones(d, 46 + altoTexto);
                while (true)
                {
                    if (d.ShowDialog(owner) != DialogResult.OK) return null;
                    if (!obligatorio || !string.IsNullOrWhiteSpace(txt.Text)) return txt.Text.Trim();
                    MessageBox.Show(d, "El dato es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // Número (null si se cancela)
        public static decimal? PedirNumero(IWin32Window owner, string titulo, string etiqueta, decimal valor, decimal minimo, decimal maximo, int decimales)
        {
            using (Form d = Dialogo(titulo, 360, 106))
            {
                d.Controls.Add(new Label { Text = etiqueta, Location = new Point(12, 12), Size = new Size(336, 34) });
                NumericUpDown nud = new NumericUpDown
                {
                    Location = new Point(12, 50), Size = new Size(120, 20), Minimum = minimo, Maximum = maximo, DecimalPlaces = decimales,
                    Value = valor, BackColor = CAMPO, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle
                };
                d.Controls.Add(nud);
                Botones(d, 70);
                return d.ShowDialog(owner) == DialogResult.OK ? nud.Value : (decimal?)null;
            }
        }

        // Varios números en un mismo diálogo, uno por renglón; null si se cancela
        public static decimal[] PedirNumeros(IWin32Window owner, string titulo, string etiqueta, (string Rotulo, decimal Valor)[] campos,
                                             decimal minimo, decimal maximo, int decimales)
        {
            using (Form d = Dialogo(titulo, 400, 96 + campos.Length * 30))
            {
                d.Controls.Add(new Label { Text = etiqueta, Location = new Point(12, 12), Size = new Size(376, 34) });
                NumericUpDown[] numeros = new NumericUpDown[campos.Length];
                for (int i = 0; i < campos.Length; i++)
                {
                    d.Controls.Add(new Label { Text = campos[i].Rotulo, Location = new Point(12, 52 + i * 30), Size = new Size(240, 20), TextAlign = ContentAlignment.MiddleLeft });
                    numeros[i] = new NumericUpDown
                    {
                        Location = new Point(258, 52 + i * 30), Size = new Size(90, 20), Minimum = minimo, Maximum = maximo, DecimalPlaces = decimales,
                        Value = Math.Max(minimo, Math.Min(maximo, campos[i].Valor)), BackColor = CAMPO, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle
                    };
                    d.Controls.Add(numeros[i]);
                    d.Controls.Add(new Label { Text = "%", Location = new Point(352, 52 + i * 30), Size = new Size(20, 20), TextAlign = ContentAlignment.MiddleLeft });
                }
                Botones(d, 60 + campos.Length * 30);
                return d.ShowDialog(owner) == DialogResult.OK ? numeros.Select(n => n.Value).ToArray() : null;
            }
        }

        // Pide dónde guardar un PDF, lo genera y ofrece abrirlo
        public static void GuardarPdf(IWin32Window owner, string nombreArchivo, string titulo, Action<string> generar)
        {
            using (SaveFileDialog dialogo = new SaveFileDialog { Filter = "Documento PDF (*.pdf)|*.pdf", FileName = nombreArchivo, Title = titulo })
            {
                if (dialogo.ShowDialog(owner) != DialogResult.OK) return;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    generar(dialogo.FileName);
                    Cursor.Current = Cursors.Default;
                    if (MessageBox.Show(owner, "PDF generado con éxito. ¿Desea abrirlo?", "Éxito", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(dialogo.FileName);
                }
                catch (Exception ex) when (ex is ArgumentException || ex is UnauthorizedAccessException)
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show(owner, ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show(owner, "Error al generar el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
