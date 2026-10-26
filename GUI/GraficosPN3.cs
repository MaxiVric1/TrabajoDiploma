using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using BE;
using BLL;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace TpIngSoft
{
    // Gráficos de los informes PDF del PN3, hechos con OxyPlot y exportados a PNG para DocumentoPdf. Se dibujan al
    // doble del tamaño en que se ven en la hoja (ANCHO x ALTO puntos -> el doble de píxeles), así no se pixelan.
    public static class GraficosPN3
    {
        public const int ANCHO = 440, ALTO = 250;           // barras: tamaño en el PDF, en puntos
        public const int ANCHO_TORTA = 360, ALTO_TORTA = 230;

        private static readonly OxyColor MORADO = OxyColor.FromRgb(89, 75, 160);
        private static readonly OxyColor GRIS = OxyColor.FromRgb(160, 160, 170);
        private static readonly OxyColor VERDE = OxyColor.FromRgb(80, 170, 110);
        private static readonly OxyColor NARANJA = OxyColor.FromRgb(235, 150, 60);
        private static readonly OxyColor[] PALETA =
        {
            MORADO, VERDE, NARANJA, OxyColor.FromRgb(70, 140, 200), OxyColor.FromRgb(200, 90, 120),
            OxyColor.FromRgb(140, 110, 200), OxyColor.FromRgb(120, 180, 170), OxyColor.FromRgb(210, 190, 80)
        };

        // ---------- Informe del Directorio ----------

        // Barras agrupadas: lo que gana cada parte (propietario, inversores, LandSmart) en el escenario A y en el B
        public static byte[] GananciaPorEscenario(AnalisisComparativo a)
        {
            PlotModel modelo = Modelo(null);
            modelo.Legends.Add(new Legend { LegendPosition = LegendPosition.BottomCenter, LegendPlacement = LegendPlacement.Outside, LegendOrientation = LegendOrientation.Horizontal, LegendFontSize = 11 });
            string[] partes = { "Propietario", "Inversores", "LandSmart" };
            EjesBarras(modelo, partes, "Miles de USD");
            modelo.Series.Add(Barras("A: " + a.Tradicional.Nombre, GRIS, new[] { a.Tradicional.ResultadoUSD, 0, a.ComisionTradicionalUSD }));
            modelo.Series.Add(Barras("B: " + a.Fideicomiso.Nombre, MORADO, new[] { a.Fideicomiso.ResultadoUSD, a.MargenInversoresUSD, a.GananciaInmobiliaria.TotalUSD }));
            return Png(modelo, ANCHO, ALTO);
        }

        // Torta: reparto de lo que genera el desarrollo entre propietario, inversores y LandSmart
        public static byte[] DistribucionGanancia(AnalisisComparativo a)
        {
            var partes = InformeInversionBLL.DistribucionGanancia(a);
            return Png(Torta(partes.Select(x => (x.Parte, x.MontoUSD)).ToList(), new[] { MORADO, VERDE, NARANJA }), ANCHO_TORTA, ALTO_TORTA);
        }

        // ---------- Propuesta para inversores ----------

        // Barras: rentabilidad anual en dólares del fideicomiso contra obligaciones negociables y plazo fijo en dólares
        public static byte[] RentabilidadComparada(ComparativaRentabilidad c)
        {
            PlotModel modelo = Modelo(null);
            var datos = new List<(string, decimal, OxyColor)>
            {
                ("Fideicomiso", c.RentabilidadAnualUSD, MORADO),
                ("ON en USD (ref.)", c.ObligacionesNegociablesAnualUSD, VERDE),
                (c.PlazoFijoUsdReferencial ? "Plazo fijo USD (ref.)" : "Plazo fijo en USD", c.PlazoFijoUsdAnual, NARANJA)
            };
            EjesBarras(modelo, datos.Select(d => d.Item1).ToArray(), "% anual en dólares", miles: false);
            BarSeries serie = new BarSeries { XAxisKey = "valor", YAxisKey = "categoria", LabelFormatString = "{0:0.#}%", LabelPlacement = LabelPlacement.Outside, FontSize = 12, BarWidth = 0.6 };
            foreach (var d in datos) serie.Items.Add(new BarItem((double)d.Item2) { Color = d.Item3 });
            modelo.Series.Add(serie);
            return Png(modelo, ANCHO, ALTO);
        }

        // Torta: participación de cada inversor sobre el capital necesario, más la parte todavía disponible
        public static byte[] ParticipacionInversores(ResumenFideicomiso r)
        {
            List<(string, decimal)> partes = r.Suscripciones.GroupBy(s => s.NombreInversor)
                .Select(g => (g.Key, g.Sum(s => s.MontoAportado))).OrderByDescending(x => x.Item2).ToList();
            List<OxyColor> colores = PALETA.Take(partes.Count).ToList();
            while (colores.Count < partes.Count) colores.Add(PALETA[colores.Count % PALETA.Length]);
            if (r.CapitalRestante > 0)
            {
                partes.Add(("Disponible", r.CapitalRestante));
                colores.Add(OxyColor.FromRgb(170, 170, 180));
            }
            return Png(Torta(partes, colores.ToArray()), ANCHO_TORTA, ALTO_TORTA);
        }

        // ---------- Comunes ----------

        private static PlotModel Modelo(string titulo)
        {
            return new PlotModel
            {
                Title = titulo,
                Background = OxyColors.White,
                PlotAreaBorderColor = OxyColors.Transparent,
                DefaultFont = "Segoe UI",
                DefaultFontSize = 11,
                TextColor = OxyColor.FromRgb(40, 40, 40)
            };
        }

        // Categorías abajo y valores a la izquierda: así BarSeries dibuja columnas verticales
        private static void EjesBarras(PlotModel modelo, string[] categorias, string tituloValores, bool miles = true)
        {
            modelo.Axes.Add(new CategoryAxis { Position = AxisPosition.Bottom, Key = "categoria", ItemsSource = categorias, GapWidth = 0.6, TickStyle = TickStyle.None, FontSize = 12 });
            modelo.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left, Key = "valor", Title = tituloValores, MinimumPadding = 0, MaximumPadding = 0.15, TickStyle = TickStyle.None,
                MajorGridlineStyle = LineStyle.Dot, MajorGridlineColor = OxyColor.FromRgb(210, 210, 210), StringFormat = miles ? "N0" : "0.#",
                ExtraGridlines = new[] { 0.0 }, ExtraGridlineColor = OxyColor.FromRgb(120, 120, 120)
            });
        }

        // Montos en miles de USD para que las etiquetas sean cortas
        private static BarSeries Barras(string titulo, OxyColor color, decimal[] valores)
        {
            BarSeries serie = new BarSeries
            {
                Title = titulo, FillColor = color, XAxisKey = "valor", YAxisKey = "categoria",
                LabelFormatString = "{0:N0}", LabelPlacement = LabelPlacement.Outside, FontSize = 10
            };
            foreach (decimal v in valores) serie.Items.Add(new BarItem((double)Math.Round(v / 1000, 0)));
            return serie;
        }

        private static PlotModel Torta(List<(string Parte, decimal Monto)> partes, OxyColor[] colores)
        {
            PlotModel modelo = Modelo(null);
            PieSeries torta = new PieSeries
            {
                StrokeThickness = 1.5, Stroke = OxyColors.White, InsideLabelPosition = 0.6, AngleSpan = 360, StartAngle = -90,
                InsideLabelFormat = "{2:0}%", InsideLabelColor = OxyColors.White, OutsideLabelFormat = "{1}", FontSize = 11,
                TickHorizontalLength = 8, TickRadialLength = 8, Diameter = 0.9
            };
            decimal total = partes.Sum(x => x.Monto);
            for (int i = 0; i < partes.Count; i++)
            {
                if (partes[i].Monto <= 0) continue;
                torta.Slices.Add(new PieSlice(partes[i].Parte, (double)partes[i].Monto) { Fill = colores[i % colores.Length] });
            }
            if (total <= 0) torta.Slices.Add(new PieSlice("Sin datos", 1) { Fill = OxyColor.FromRgb(225, 225, 230) });
            modelo.Series.Add(torta);
            return modelo;
        }

        // Dibuja el gráfico al doble de ancho x alto (en píxeles) y lo devuelve como PNG
        public static byte[] Png(PlotModel modelo, int ancho, int alto)
        {
            using (Bitmap bmp = new Bitmap(ancho * 2, alto * 2, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.ScaleTransform(2, 2);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    GraphicsRenderContext contexto = new GraphicsRenderContext(g) { RendersToScreen = false };
                    ((IPlotModel)modelo).Update(true);
                    ((IPlotModel)modelo).Render(contexto, new OxyRect(0, 0, ancho, alto));
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}
