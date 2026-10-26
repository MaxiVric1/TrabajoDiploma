using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace SERVICIOS
{
    // Generador mínimo de PDF (A4, Helvetica, sin dependencias externas): encabezado con logo, títulos, párrafos
    // con ajuste de línea, pares "campo: valor", separadores, firmas, tablas, recuadros destacados, barras de
    // progreso, imágenes (gráficos) y salto de página automático. Los textos se
    // guardan en WinAnsiEncoding, que cubre los acentos, la ñ y los signos del español. Las imágenes van
    // incrustadas en el PDF (RGB comprimido con Flate), no como archivos externos.
    public class DocumentoPdf
    {
        private const float ANCHO = 595f, ALTO = 842f, MARGEN = 56f, PIE = 40f;
        private const string NORMAL = "F1", NEGRITA = "F2";

        // Anchos de Helvetica (1/1000 del tamaño de letra) para los caracteres 32 a 126
        private static readonly int[] AnchosHelvetica =
        {
            278, 278, 355, 556, 556, 889, 667, 191, 333, 333, 389, 584, 278, 333, 278, 278,
            556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 278, 278, 584, 584, 584, 556,
            1015, 667, 667, 722, 722, 667, 611, 778, 722, 278, 500, 667, 556, 833, 722, 778,
            667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 278, 278, 278, 469, 556,
            333, 556, 556, 500, 556, 556, 278, 556, 556, 222, 222, 500, 222, 833, 556, 556,
            556, 556, 333, 500, 278, 556, 500, 722, 500, 500, 500, 334, 260, 334, 584
        };

        private static readonly Encoding WinAnsi = Encoding.GetEncoding(1252);

        private class ImagenPdf
        {
            public int Ancho, Alto;
            public byte[] Datos; // RGB de 8 bits por canal, comprimido con zlib (FlateDecode)
        }

        private readonly List<StringBuilder> _paginas = new List<StringBuilder>();
        private readonly List<ImagenPdf> _imagenes = new List<ImagenPdf>();
        private StringBuilder _actual;
        private float _y;
        private readonly string _pie;

        // pie: texto que se repite al pie de cada página, junto al número de página
        public DocumentoPdf(string pie = null)
        {
            _pie = pie;
            NuevaPagina();
        }

        // Encabezado de la primera página: logo a la izquierda (recortado a su contenido, con el fondo claro pasado a
        // blanco), título centrado y dos renglones alineados a la derecha (por ejemplo número y fecha)
        public void Encabezado(byte[] logo, string titulo, string derechaArriba, string derechaAbajo)
        {
            const float altoLogo = 62f;
            float arriba = ALTO - MARGEN + 16;
            float abajo = arriba - altoLogo;
            float izquierda = MARGEN;
            if (logo != null)
            {
                ImagenPdf imagen = PrepararLogo(logo);
                _imagenes.Add(imagen);
                float anchoLogo = altoLogo * imagen.Ancho / imagen.Alto;
                _actual.AppendFormat(CultureInfo.InvariantCulture, "q {0:0.##} 0 0 {1:0.##} {2:0.##} {3:0.##} cm /Im{4} Do Q\n",
                    anchoLogo, altoLogo, MARGEN, abajo, _imagenes.Count);
                izquierda += anchoLogo + 12;
            }

            // Título centrado entre el logo y los renglones de la derecha; si es largo, se achica hasta que entre
            float medio = abajo + altoLogo / 2;
            float derecha = ANCHO - MARGEN;
            float limite = derecha - Math.Max(AnchoTexto(derechaArriba, 11, true), AnchoTexto(derechaAbajo, 8.5f, false)) - 12;
            float tamano = 17;
            while (tamano > 10 && AnchoTexto(titulo, tamano, true) > limite - izquierda) tamano -= 0.5f;
            Escribir(titulo, izquierda + (limite - izquierda - AnchoTexto(titulo, tamano, true)) / 2, medio - tamano * 0.35f, tamano, true);
            Escribir(derechaArriba, derecha - AnchoTexto(derechaArriba, 11, true), medio + 4, 11, true);
            Escribir(derechaAbajo, derecha - AnchoTexto(derechaAbajo, 8.5f, false), medio - 10, 8.5f, false);

            _y = abajo - 8;
            Linea();
            Espacio(6);
        }

        public void Titulo(string texto)
        {
            Parrafo(texto, 16, true, centrado: true);
            Espacio(6);
        }

        // reservar: lugar mínimo para el título y lo que le sigue, para que no quede solo al pie de una página
        public void Subtitulo(string texto, float reservar = 60)
        {
            AsegurarLugar(reservar);
            Espacio(8);
            Parrafo(texto, 12, true);
            Linea();
            Espacio(4);
        }

        public void Parrafo(string texto, float tamano = 10, bool negrita = false, bool centrado = false)
        {
            float anchoUtil = ANCHO - 2 * MARGEN;
            foreach (string renglon in Ajustar(texto ?? "", tamano, negrita, anchoUtil))
            {
                AsegurarLugar(tamano * 1.4f);
                float x = centrado ? (ANCHO - AnchoTexto(renglon, tamano, negrita)) / 2 : MARGEN;
                Escribir(renglon, x, _y, tamano, negrita);
                _y -= tamano * 1.4f;
            }
        }

        // "Campo: valor", con el valor ajustado a la derecha de una columna fija
        public void Campo(string campo, string valor, float tamano = 10)
        {
            const float columna = 170f;
            List<string> renglones = Ajustar(string.IsNullOrWhiteSpace(valor) ? "-" : valor, tamano, false, ANCHO - 2 * MARGEN - columna);
            AsegurarLugar(tamano * 1.4f);
            Escribir(campo + ":", MARGEN, _y, tamano, true);
            foreach (string renglon in renglones)
            {
                AsegurarLugar(tamano * 1.4f);
                Escribir(renglon, MARGEN + columna, _y, tamano, false);
                _y -= tamano * 1.4f;
            }
        }

        public void Espacio(float puntos)
        {
            _y -= puntos;
        }

        public void Linea()
        {
            AsegurarLugar(6);
            _actual.AppendFormat(CultureInfo.InvariantCulture, "0.5 w {0:0.##} {1:0.##} m {2:0.##} {1:0.##} l S\n", MARGEN, _y + 3, ANCHO - MARGEN);
            _y -= 4;
        }

        // Línea de firma con la aclaración debajo; firmaTipeada se escribe sobre la línea en letra inclinada
        public void Firma(string aclaracion, string firmaTipeada = null)
        {
            Espacio(30);
            AsegurarLugar(60);
            if (!string.IsNullOrWhiteSpace(firmaTipeada))
            {
                Escribir(firmaTipeada, MARGEN + 10, _y, 14, false, oblicua: true);
            }
            _y -= 8;
            _actual.AppendFormat(CultureInfo.InvariantCulture, "0.5 w {0:0.##} {1:0.##} m {2:0.##} {1:0.##} l S\n", MARGEN, _y, MARGEN + 220);
            _y -= 14;
            Escribir(aclaracion, MARGEN, _y, 9, false);
            _y -= 14;
        }

        // Firmas lado a lado: por cada una, el rótulo ("El LOCADOR:"), la firma tipeada en letra inclinada (o el
        // espacio en blanco para firmar a mano), la línea y la aclaración debajo
        public void Firmas(params (string Rotulo, string FirmaTipeada, string Aclaracion)[] firmas)
        {
            const float separacion = 18f;
            float ancho = (ANCHO - 2 * MARGEN - separacion * (firmas.Length - 1)) / firmas.Length;
            AsegurarLugar(95);
            float y = _y;
            int renglonesMax = 1;
            for (int i = 0; i < firmas.Length; i++)
            {
                float x = MARGEN + i * (ancho + separacion);
                Escribir(firmas[i].Rotulo, x, y, 9.5f, true);
                if (!string.IsNullOrWhiteSpace(firmas[i].FirmaTipeada))
                    Escribir(firmas[i].FirmaTipeada.Trim(), x + 6, y - 34, 13, false, oblicua: true);
                _actual.AppendFormat(CultureInfo.InvariantCulture, "0.5 w {0:0.##} {1:0.##} m {2:0.##} {1:0.##} l S\n", x, y - 42, x + ancho);
                List<string> aclaracion = Ajustar(firmas[i].Aclaracion ?? "", 8.5f, false, ancho);
                for (int r = 0; r < aclaracion.Count; r++)
                    Escribir(aclaracion[r], x, y - 54 - r * 11, 8.5f, false);
                renglonesMax = Math.Max(renglonesMax, aclaracion.Count);
            }
            _y = y - 54 - renglonesMax * 11 - 6;
        }

        public void SaltoPagina()
        {
            NuevaPagina();
        }

        // Imagen (PNG, JPG...) centrada, de 'ancho' puntos y alto proporcional; si no entra, va a la página siguiente.
        // Se incrusta tal cual (las zonas transparentes quedan blancas): conviene pasarla con el doble de píxeles que
        // de puntos para que se vea nítida al imprimir.
        public void Imagen(byte[] png, float ancho)
        {
            if (png == null) return;
            ImagenPdf imagen;
            using (MemoryStream ms = new MemoryStream(png))
            using (Bitmap original = new Bitmap(ms))
            using (Bitmap rgb = SobreBlanco(original))
                imagen = new ImagenPdf { Ancho = rgb.Width, Alto = rgb.Height, Datos = Zlib(Rgb(rgb)) };
            ancho = Math.Min(ancho, ANCHO - 2 * MARGEN);
            float alto = ancho * imagen.Alto / imagen.Ancho;
            AsegurarLugar(alto + 4);
            _imagenes.Add(imagen);
            _actual.AppendFormat(CultureInfo.InvariantCulture, "q {0:0.##} 0 0 {1:0.##} {2:0.##} {3:0.##} cm /Im{4} Do Q\n",
                ancho, alto, (ANCHO - ancho) / 2, _y - alto, _imagenes.Count);
            _y -= alto + 14;
        }

        // Tabla con encabezado sombreado: las primeras 'columnasTexto' columnas alineadas a la izquierda y las demás a
        // la derecha (montos). anchos: proporciones de cada columna sobre el ancho útil. negritaUltima: fila de totales.
        public void Tabla(float[] anchos, string[] encabezado, IEnumerable<string[]> filas, bool negritaUltima = false, float tamano = 9, int columnasTexto = 1)
        {
            float total = 0;
            foreach (float a in anchos) total += a;
            float anchoUtil = ANCHO - 2 * MARGEN;
            float[] columnas = new float[anchos.Length];
            for (int i = 0; i < anchos.Length; i++) columnas[i] = anchoUtil * anchos[i] / total;

            List<string[]> lista = new List<string[]>(filas);
            _y += 4; // _y es la línea base del renglón siguiente: la tabla empieza apenas arriba
            FilaTabla(columnas, encabezado, tamano, true, true, columnasTexto);
            for (int i = 0; i < lista.Count; i++)
                FilaTabla(columnas, lista[i], tamano, negritaUltima && i == lista.Count - 1, false, columnasTexto);
            Espacio(14);
        }

        private void FilaTabla(float[] columnas, string[] celdas, float tamano, bool negrita, bool sombreada, int columnasTexto)
        {
            const float relleno = 4f;
            List<List<string>> renglones = new List<List<string>>();
            int maximo = 1;
            for (int i = 0; i < columnas.Length; i++)
            {
                List<string> r = Ajustar(i < celdas.Length ? celdas[i] ?? "" : "", tamano, negrita, columnas[i] - 2 * relleno);
                renglones.Add(r);
                maximo = Math.Max(maximo, r.Count);
            }
            float interlinea = tamano * 1.3f;
            float alto = maximo * interlinea + 2 * relleno;
            AsegurarLugar(alto);
            float arriba = _y;
            _actual.AppendFormat(CultureInfo.InvariantCulture, "q {0} {1:0.##} {2:0.##} {3:0.##} {4:0.##} re {5} Q\n",
                sombreada ? "0.89 0.87 0.95 rg 0.6 0.6 0.6 RG 0.5 w" : "0.6 0.6 0.6 RG 0.5 w",
                MARGEN, arriba - alto, ANCHO - 2 * MARGEN, alto, sombreada ? "B" : "S");
            float x = MARGEN;
            for (int i = 0; i < columnas.Length; i++)
            {
                if (i > 0)
                    _actual.AppendFormat(CultureInfo.InvariantCulture, "q 0.6 0.6 0.6 RG 0.5 w {0:0.##} {1:0.##} m {0:0.##} {2:0.##} l S Q\n", x, arriba, arriba - alto);
                for (int r = 0; r < renglones[i].Count; r++)
                {
                    string texto = renglones[i][r];
                    float xTexto = i < columnasTexto ? x + relleno : x + columnas[i] - relleno - AnchoTexto(texto, tamano, negrita);
                    Escribir(texto, xTexto, arriba - relleno - tamano * 0.85f - r * interlinea, tamano, negrita);
                }
                x += columnas[i];
            }
            _y = arriba - alto;
        }

        // Recuadro destacado (fondo de color y borde) con un título en negrita y el texto debajo
        public void Recuadro(string titulo, string texto, float tamano = 10)
        {
            const float relleno = 8f;
            float anchoUtil = ANCHO - 2 * MARGEN - 2 * relleno;
            List<string> lineasTitulo = string.IsNullOrEmpty(titulo) ? new List<string>() : Ajustar(titulo, tamano + 1.5f, true, anchoUtil);
            List<string> lineas = string.IsNullOrEmpty(texto) ? new List<string>() : Ajustar(texto, tamano, false, anchoUtil);
            float altoTitulo = lineasTitulo.Count * (tamano + 1.5f) * 1.35f;
            float alto = 2 * relleno + altoTitulo + lineas.Count * tamano * 1.35f;
            AsegurarLugar(alto + 4);
            float arriba = _y + 4;
            _actual.AppendFormat(CultureInfo.InvariantCulture, "q 0.93 0.91 0.98 rg 0.35 0.29 0.63 RG 1.2 w {0:0.##} {1:0.##} {2:0.##} {3:0.##} re B Q\n",
                MARGEN, arriba - alto, ANCHO - 2 * MARGEN, alto);
            float y = arriba - relleno - tamano;
            foreach (string l in lineasTitulo)
            {
                Escribir(l, MARGEN + relleno, y, tamano + 1.5f, true);
                y -= (tamano + 1.5f) * 1.35f;
            }
            foreach (string l in lineas)
            {
                Escribir(l, MARGEN + relleno, y, tamano, false);
                y -= tamano * 1.35f;
            }
            _y = arriba - alto - 16;
        }

        // Barra de progreso de ancho completo: la parte cumplida en verde y el porcentaje escrito a la derecha
        public void BarraProgreso(decimal porcentaje, string rotulo = null)
        {
            const float alto = 18f, anchoTexto = 60f;
            float ancho = ANCHO - 2 * MARGEN - anchoTexto;
            if (!string.IsNullOrEmpty(rotulo)) Parrafo(rotulo, 10, true);
            AsegurarLugar(alto + 8);
            float abajo = _y - alto + 8;
            float lleno = ancho * (float)Math.Max(0, Math.Min(100, porcentaje)) / 100;
            _actual.AppendFormat(CultureInfo.InvariantCulture, "q 0.92 0.92 0.92 rg {0:0.##} {1:0.##} {2:0.##} {3:0.##} re f Q\n", MARGEN, abajo, ancho, alto);
            if (lleno > 0)
                _actual.AppendFormat(CultureInfo.InvariantCulture, "q 0.31 0.67 0.42 rg {0:0.##} {1:0.##} {2:0.##} {3:0.##} re f Q\n", MARGEN, abajo, lleno, alto);
            _actual.AppendFormat(CultureInfo.InvariantCulture, "q 0.5 0.5 0.5 RG 0.6 w {0:0.##} {1:0.##} {2:0.##} {3:0.##} re S Q\n", MARGEN, abajo, ancho, alto);
            string texto = porcentaje.ToString("0.##", CultureInfo.CurrentCulture) + "%";
            Escribir(texto, ANCHO - MARGEN - AnchoTexto(texto, 12, true), abajo + 5, 12, true);
            _y = abajo - 18;
        }

        public void Guardar(string ruta)
        {
            File.WriteAllBytes(ruta, Generar());
        }

        public byte[] Generar()
        {
            // Pie con el número de página (se conoce el total recién al final)
            for (int i = 0; i < _paginas.Count; i++)
            {
                string pie = (string.IsNullOrEmpty(_pie) ? "" : _pie + "  -  ") + $"Página {i + 1} de {_paginas.Count}";
                StringBuilder anterior = _actual;
                _actual = _paginas[i];
                Escribir(pie, MARGEN, PIE - 16, 8, false);
                _actual = anterior;
            }

            MemoryStream salida = new MemoryStream();
            List<long> posiciones = new List<long>();
            Action<string> escribir = s => { byte[] b = WinAnsi.GetBytes(s); salida.Write(b, 0, b.Length); };
            Action<int, string> objeto = (n, cuerpo) =>
            {
                posiciones.Add(salida.Position);
                escribir($"{n} 0 obj\n{cuerpo}\nendobj\n");
            };

            escribir("%PDF-1.4\n");
            int primeraPagina = 5 + _imagenes.Count;
            int total = 4 + _imagenes.Count + _paginas.Count * 2;
            StringBuilder kids = new StringBuilder();
            for (int i = 0; i < _paginas.Count; i++) kids.Append($"{primeraPagina + i * 2} 0 R ");
            StringBuilder xobjects = new StringBuilder();
            for (int i = 0; i < _imagenes.Count; i++) xobjects.Append($"/Im{i + 1} {5 + i} 0 R ");
            string recursos = $"/Font << /{NORMAL} 3 0 R /{NEGRITA} 4 0 R >>" +
                              (_imagenes.Count > 0 ? $" /XObject << {xobjects.ToString().Trim()} >>" : "");

            objeto(1, "<< /Type /Catalog /Pages 2 0 R >>");
            objeto(2, $"<< /Type /Pages /Kids [{kids.ToString().Trim()}] /Count {_paginas.Count} >>");
            objeto(3, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");
            objeto(4, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>");
            for (int i = 0; i < _imagenes.Count; i++)
            {
                ImagenPdf imagen = _imagenes[i];
                posiciones.Add(salida.Position);
                escribir($"{5 + i} 0 obj\n<< /Type /XObject /Subtype /Image /Width {imagen.Ancho} /Height {imagen.Alto} " +
                         $"/ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /FlateDecode /Length {imagen.Datos.Length} >>\nstream\n");
                salida.Write(imagen.Datos, 0, imagen.Datos.Length);
                escribir("\nendstream\nendobj\n");
            }
            for (int i = 0; i < _paginas.Count; i++)
            {
                int pagina = primeraPagina + i * 2;
                byte[] contenido = WinAnsi.GetBytes(_paginas[i].ToString());
                objeto(pagina, $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {ANCHO:0} {ALTO:0}] " +
                               $"/Resources << {recursos} >> /Contents {pagina + 1} 0 R >>");
                posiciones.Add(salida.Position);
                escribir($"{pagina + 1} 0 obj\n<< /Length {contenido.Length} >>\nstream\n");
                salida.Write(contenido, 0, contenido.Length);
                escribir("\nendstream\nendobj\n");
            }

            long xref = salida.Position;
            escribir($"xref\n0 {total + 1}\n0000000000 65535 f \n");
            foreach (long p in posiciones) escribir(p.ToString("0000000000") + " 00000 n \n");
            escribir($"trailer\n<< /Size {total + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
            return salida.ToArray();
        }

        // ---------- Internos ----------

        // Logo listo para incrustar: recortado a lo que no es fondo (con un pequeño margen), fondo casi blanco
        // pasado a blanco puro (si no, se ve un recuadro gris sobre la hoja) y achicado a 400 px de ancho como máximo
        private static ImagenPdf PrepararLogo(byte[] archivo)
        {
            const int umbralFondo = 225;
            using (MemoryStream ms = new MemoryStream(archivo))
            using (Bitmap original = new Bitmap(ms))
            using (Bitmap rgb = SobreBlanco(original))
            {
                byte[] px = Pixeles(rgb, out int paso);

                int minX = rgb.Width, minY = rgb.Height, maxX = -1, maxY = -1;
                for (int y = 0; y < rgb.Height; y++)
                    for (int x = 0; x < rgb.Width; x++)
                    {
                        int i = y * paso + x * 3;
                        if (Math.Min(px[i], Math.Min(px[i + 1], px[i + 2])) < umbralFondo)
                        {
                            if (x < minX) minX = x;
                            if (x > maxX) maxX = x;
                            if (y < minY) minY = y;
                            if (y > maxY) maxY = y;
                        }
                    }
                Rectangle contenido = maxX < 0 ? new Rectangle(0, 0, rgb.Width, rgb.Height)
                    : Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
                contenido.Inflate(contenido.Width / 40, contenido.Height / 40);
                contenido.Intersect(new Rectangle(0, 0, rgb.Width, rgb.Height));

                float escala = Math.Min(1f, 400f / contenido.Width);
                int ancho = Math.Max(1, (int)(contenido.Width * escala)), alto = Math.Max(1, (int)(contenido.Height * escala));
                using (Bitmap final = new Bitmap(ancho, alto, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(final))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(rgb, new Rectangle(0, 0, ancho, alto), contenido, GraphicsUnit.Pixel);
                    }
                    byte[] datos = Pixeles(final, out int pasoFinal);
                    byte[] filas = new byte[ancho * alto * 3];
                    for (int y = 0; y < alto; y++)
                        for (int x = 0; x < ancho; x++)
                        {
                            int o = y * pasoFinal + x * 3, d = (y * ancho + x) * 3;
                            byte b = datos[o], gr = datos[o + 1], r = datos[o + 2]; // GDI+ guarda BGR
                            bool fondo = Math.Min(r, Math.Min(gr, b)) >= umbralFondo;
                            filas[d] = fondo ? (byte)255 : r;
                            filas[d + 1] = fondo ? (byte)255 : gr;
                            filas[d + 2] = fondo ? (byte)255 : b;
                        }
                    return new ImagenPdf { Ancho = ancho, Alto = alto, Datos = Zlib(filas) };
                }
            }
        }

        // Copia RGB de 24 bits con las zonas transparentes en blanco
        private static Bitmap SobreBlanco(Bitmap original)
        {
            Bitmap rgb = new Bitmap(original.Width, original.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(rgb))
            {
                g.Clear(Color.White);
                g.DrawImage(original, 0, 0, original.Width, original.Height);
            }
            return rgb;
        }

        // Píxeles en el orden que espera el PDF: RGB por filas, sin relleno al final de cada fila
        private static byte[] Rgb(Bitmap bmp)
        {
            byte[] datos = Pixeles(bmp, out int paso);
            byte[] filas = new byte[bmp.Width * bmp.Height * 3];
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    int o = y * paso + x * 3, d = (y * bmp.Width + x) * 3;
                    filas[d] = datos[o + 2]; // GDI+ guarda BGR
                    filas[d + 1] = datos[o + 1];
                    filas[d + 2] = datos[o];
                }
            return filas;
        }

        private static byte[] Pixeles(Bitmap bmp, out int paso)
        {
            BitmapData datos = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                paso = datos.Stride;
                byte[] px = new byte[datos.Stride * bmp.Height];
                Marshal.Copy(datos.Scan0, px, 0, px.Length);
                return px;
            }
            finally { bmp.UnlockBits(datos); }
        }

        // Formato zlib (encabezado + deflate + Adler-32), que es lo que espera el filtro FlateDecode del PDF
        private static byte[] Zlib(byte[] datos)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0x78);
                ms.WriteByte(0x9C);
                using (DeflateStream deflate = new DeflateStream(ms, CompressionLevel.Optimal, true))
                    deflate.Write(datos, 0, datos.Length);
                uint a = 1, b = 0;
                foreach (byte x in datos)
                {
                    a = (a + x) % 65521;
                    b = (b + a) % 65521;
                }
                uint adler = (b << 16) | a;
                ms.WriteByte((byte)(adler >> 24));
                ms.WriteByte((byte)(adler >> 16));
                ms.WriteByte((byte)(adler >> 8));
                ms.WriteByte((byte)adler);
                return ms.ToArray();
            }
        }

        private void NuevaPagina()
        {
            _actual = new StringBuilder();
            _paginas.Add(_actual);
            _y = ALTO - MARGEN;
        }

        private void AsegurarLugar(float alto)
        {
            if (_y - alto < MARGEN + PIE / 2) NuevaPagina();
        }

        private void Escribir(string texto, float x, float y, float tamano, bool negrita, bool oblicua = false)
        {
            // Oblicua: Helvetica inclinada con una matriz de texto (para la firma tipeada)
            string matriz = oblicua
                ? string.Format(CultureInfo.InvariantCulture, "1 0 0.25 1 {0:0.##} {1:0.##} Tm", x, y)
                : string.Format(CultureInfo.InvariantCulture, "1 0 0 1 {0:0.##} {1:0.##} Tm", x, y);
            _actual.AppendFormat(CultureInfo.InvariantCulture, "BT /{0} {1:0.##} Tf {2} ({3}) Tj ET\n",
                negrita ? NEGRITA : NORMAL, tamano, matriz, Escapar(texto));
        }

        private static string Escapar(string texto)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in texto)
            {
                if (c == '(' || c == ')' || c == '\\') sb.Append('\\').Append(c);
                else if (c == '\t') sb.Append("    ");
                else if (c < 32) continue;
                else if (WinAnsi.GetBytes(c.ToString())[0] == (byte)'?' && c != '?') sb.Append('?'); // fuera de WinAnsi
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private static float AnchoTexto(string texto, float tamano, bool negrita)
        {
            float total = 0;
            foreach (char c in texto)
            {
                char baseC = SinAcento(c);
                total += baseC >= 32 && baseC <= 126 ? AnchosHelvetica[baseC - 32] : 556;
            }
            // Helvetica-Bold es algo más ancha: margen para que el ajuste de línea no se pase
            return total * tamano / 1000f * (negrita ? 1.08f : 1f);
        }

        private static char SinAcento(char c)
        {
            string d = c.ToString().Normalize(NormalizationForm.FormD);
            return d.Length > 0 ? d[0] : c;
        }

        // Ajuste de línea por palabras; respeta los saltos de línea del texto
        private static List<string> Ajustar(string texto, float tamano, bool negrita, float anchoMaximo)
        {
            List<string> renglones = new List<string>();
            foreach (string parrafo in texto.Replace("\r\n", "\n").Split('\n'))
            {
                string actual = "";
                foreach (string palabra in parrafo.Split(' '))
                {
                    string prueba = actual.Length == 0 ? palabra : actual + " " + palabra;
                    if (actual.Length > 0 && AnchoTexto(prueba, tamano, negrita) > anchoMaximo)
                    {
                        renglones.Add(actual);
                        actual = palabra;
                    }
                    else actual = prueba;
                }
                renglones.Add(actual);
            }
            return renglones;
        }
    }
}
