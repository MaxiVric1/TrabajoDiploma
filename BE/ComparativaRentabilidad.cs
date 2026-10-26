using System;

namespace BE
{
    // Para la propuesta a inversores: rentabilidad anual del fideicomiso contra las alternativas habituales de un
    // ahorrista en dólares (obligaciones negociables y dólar MEP). Todo en % anual en dólares, para que sea comparable.
    public class ComparativaRentabilidad
    {
        public AnalisisComparativo Analisis { get; set; }
        public int PlazoObraMeses { get; set; }

        // Fideicomiso: ganancia de los inversores sobre la obra, en todo el plazo y anualizada, en dólares
        public decimal RentabilidadPlazoUSD { get; set; }
        public decimal RentabilidadAnualUSD { get; set; }

        // Obligaciones negociables corporativas argentinas en dólares: no hay una API pública con su rendimiento, se
        // usa un valor referencial (rinden aproximadamente entre 6% y 8% anual en USD)
        public const decimal ON_ANUAL_USD_REFERENCIAL = 7;
        public decimal ObligacionesNegociablesAnualUSD { get; set; } = ON_ANUAL_USD_REFERENCIAL;

        // Dólar MEP: variación de los últimos 12 meses (ArgentinaDatos)
        public decimal? VariacionMepAnual { get; set; }
        public decimal? MepHaceUnAnio { get; set; }

        // Por cada USD 100.000 invertidos
        public const decimal MONTO_EJEMPLO = 100000;
        public decimal GananciaPorMontoEjemploUSD => Math.Round(MONTO_EJEMPLO * RentabilidadPlazoUSD / 100, 2);
        public decimal RetornoPorMontoEjemploUSD => MONTO_EJEMPLO + GananciaPorMontoEjemploUSD;
    }
}
