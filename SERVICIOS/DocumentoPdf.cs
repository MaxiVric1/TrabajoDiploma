using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SERVICIOS
{
    // Generador mínimo de PDF de texto (A4, Helvetica, sin dependencias externas): títulos, párrafos con
    // ajuste de línea, pares "campo: valor", separadores y salto de página automático. Los textos se guardan
    // en WinAnsiEncoding, que cubre los acentos, la ñ y los signos del español.
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

        private readonly List<StringBuilder> _paginas = new List<StringBuilder>();
        private StringBuilder _actual;
        private float _y;
        private readonly string _pie;

        // pie: texto que se repite al pie de cada página, junto al número de página
        public DocumentoPdf(string pie = null)
        {
            _pie = pie;
            NuevaPagina();
        }

        public void Titulo(string texto)
        {
            Parrafo(texto, 16, true, centrado: true);
            Espacio(6);
        }

        public void Subtitulo(string texto)
        {
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
            int total = 4 + _paginas.Count * 2;
            StringBuilder kids = new StringBuilder();
            for (int i = 0; i < _paginas.Count; i++) kids.Append($"{5 + i * 2} 0 R ");

            objeto(1, "<< /Type /Catalog /Pages 2 0 R >>");
            objeto(2, $"<< /Type /Pages /Kids [{kids.ToString().Trim()}] /Count {_paginas.Count} >>");
            objeto(3, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");
            objeto(4, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>");
            for (int i = 0; i < _paginas.Count; i++)
            {
                int pagina = 5 + i * 2;
                byte[] contenido = WinAnsi.GetBytes(_paginas[i].ToString());
                objeto(pagina, $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {ANCHO:0} {ALTO:0}] " +
                               $"/Resources << /Font << /{NORMAL} 3 0 R /{NEGRITA} 4 0 R >> >> /Contents {pagina + 1} 0 R >>");
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
