using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace SERVICIOS
{
    // Mapa estático a partir de las teselas de OpenStreetMap (256x256 px por tesela): arma una imagen del tamaño
    // pedido centrada en el punto, con un marcador y la atribución que exige OSM. Las teselas se guardan en disco
    // (%LOCALAPPDATA%\LandSmart\Teselas) para no volver a pedirlas: la política de uso de OSM pide no abusar del
    // servidor y un User-Agent propio (LandSmart/1.0).
    public static class MapaOSM
    {
        public const string URL_TESELAS = "https://tile.openstreetmap.org/{0}/{1}/{2}.png";
        public const string ATRIBUCION = "© OpenStreetMap contributors";
        private const int TESELA = 256;
        private static readonly string _carpeta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LandSmart", "Teselas");
        private static readonly object _bloqueo = new object();

        // atribucion: false solo si quien usa el mapa escribe "© OpenStreetMap contributors" junto a él (p. ej. el PDF,
        // donde la imagen se imprime achicada y el texto dentro del mapa no se leería)
        public static Bitmap Generar(double latitud, double longitud, int ancho, int alto, int zoom = 16, bool atribucion = true)
        {
            if (ancho <= 0 || alto <= 0) throw new ArgumentException("Tamaño de mapa inválido.");
            int n = 1 << zoom;
            double latRad = latitud * Math.PI / 180;
            // Posición del punto en píxeles del mundo (proyección Web Mercator)
            double px = (longitud + 180) / 360 * n * TESELA;
            double py = (1 - Math.Log(Math.Tan(latRad) + 1 / Math.Cos(latRad)) / Math.PI) / 2 * n * TESELA;
            double izquierda = px - ancho / 2.0, arriba = py - alto / 2.0;

            Bitmap mapa = new Bitmap(ancho, alto);
            using (Graphics g = Graphics.FromImage(mapa))
            {
                g.Clear(Color.FromArgb(230, 228, 224));
                int x0 = (int)Math.Floor(izquierda / TESELA), x1 = (int)Math.Floor((izquierda + ancho) / TESELA);
                int y0 = (int)Math.Floor(arriba / TESELA), y1 = (int)Math.Floor((arriba + alto) / TESELA);
                for (int tx = x0; tx <= x1; tx++)
                    for (int ty = y0; ty <= y1; ty++)
                    {
                        if (ty < 0 || ty >= n) continue;
                        int txNorm = ((tx % n) + n) % n;
                        using (MemoryStream ms = new MemoryStream(Tesela(zoom, txNorm, ty)))
                        using (Image tesela = Image.FromStream(ms))
                            g.DrawImage(tesela, (float)(tx * TESELA - izquierda), (float)(ty * TESELA - arriba), TESELA, TESELA);
                    }

                // Marcador en el centro
                g.SmoothingMode = SmoothingMode.AntiAlias;
                float cx = ancho / 2f, cy = alto / 2f;
                using (GraphicsPath pin = new GraphicsPath())
                {
                    pin.AddEllipse(cx - 9, cy - 30, 18, 18);
                    pin.AddPolygon(new[] { new PointF(cx - 7, cy - 17), new PointF(cx + 7, cy - 17), new PointF(cx, cy) });
                    using (SolidBrush rojo = new SolidBrush(Color.FromArgb(220, 40, 50)))
                        g.FillPath(rojo, pin);
                    using (Pen borde = new Pen(Color.FromArgb(120, 20, 30), 1.5f))
                        g.DrawPath(borde, pin);
                }
                g.FillEllipse(Brushes.White, cx - 4, cy - 25, 8, 8);

                // Atribución obligatoria de OpenStreetMap
                if (atribucion)
                    using (Font fuente = new Font("Segoe UI", 7.5f))
                    {
                        SizeF tam = g.MeasureString(ATRIBUCION, fuente);
                        using (SolidBrush fondo = new SolidBrush(Color.FromArgb(190, Color.White)))
                            g.FillRectangle(fondo, ancho - tam.Width - 4, alto - tam.Height - 2, tam.Width + 4, tam.Height + 2);
                        g.DrawString(ATRIBUCION, fuente, Brushes.Black, ancho - tam.Width - 2, alto - tam.Height - 1);
                    }
            }
            return mapa;
        }

        // Tesela desde el disco o, si no está, desde OSM (y se guarda)
        private static byte[] Tesela(int z, int x, int y)
        {
            string archivo = Path.Combine(_carpeta, z.ToString(), x.ToString(), y + ".png");
            lock (_bloqueo)
            {
                if (File.Exists(archivo)) return File.ReadAllBytes(archivo);
            }
            byte[] datos = ClienteHttp.GetBytes(string.Format(URL_TESELAS, z, x, y), "OpenStreetMap", 10000);
            lock (_bloqueo)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(archivo));
                File.WriteAllBytes(archivo, datos);
            }
            return datos;
        }
    }
}
