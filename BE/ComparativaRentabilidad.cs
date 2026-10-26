using System;

namespace BE
{
    // Para la propuesta a inversores: rentabilidad anual del fideicomiso contra las alternativas habituales de un
    // ahorrista (plazo fijo en pesos y comprar dólar MEP). Todo en % anual en pesos, para que sea comparable.
    public class ComparativaRentabilidad
    {
        public AnalisisComparativo Analisis { get; set; }
        public int PlazoObraMeses { get; set; }

        // Fideicomiso: ganancia de los inversores sobre la obra, en todo el plazo y anualizada (en dólares y en pesos)
        public decimal RentabilidadPlazoUSD { get; set; }
        public decimal RentabilidadAnualUSD { get; set; }
        public decimal? RentabilidadAnualPesos { get; set; }

        // Plazo fijo en pesos de personas humanas (BCRA): tasa nominal anual y efectiva anual (renovando cada 30 días)
        public decimal? PlazoFijoTna { get; set; }
        public decimal? PlazoFijoTea { get; set; }
        public DateTime? FechaPlazoFijo { get; set; }

        // Dólar MEP: variación de los últimos 12 meses (ArgentinaDatos)
        public decimal? VariacionMepAnual { get; set; }
        public decimal? MepHaceUnAnio { get; set; }

        // Por cada USD 100.000 invertidos
        public const decimal MONTO_EJEMPLO = 100000;
        public decimal GananciaPorMontoEjemploUSD => Math.Round(MONTO_EJEMPLO * RentabilidadPlazoUSD / 100, 2);
        public decimal RetornoPorMontoEjemploUSD => MONTO_EJEMPLO + GananciaPorMontoEjemploUSD;
    }
}
