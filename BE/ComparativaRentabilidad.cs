using System;

namespace BE
{
    // Para la propuesta a inversores: rentabilidad anual del fideicomiso contra las alternativas habituales de un
    // ahorrista en dólares (obligaciones negociables y plazo fijo bancario en dólares). Todo en % anual en dólares, para que sea comparable.
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

        // Plazo fijo bancario en dólares de personas humanas (BCRA, tasa nominal anual). Sin datos de la API se usa un
        // valor referencial (los bancos pagan aproximadamente entre 0,5% y 1% anual) y FechaPlazoFijoUsd queda en null.
        public const decimal PLAZO_FIJO_USD_REFERENCIAL = 1;
        public decimal PlazoFijoUsdAnual { get; set; } = PLAZO_FIJO_USD_REFERENCIAL;
        public DateTime? FechaPlazoFijoUsd { get; set; }
        public bool PlazoFijoUsdReferencial => !FechaPlazoFijoUsd.HasValue;

        // Por cada USD 100.000 invertidos
        public const decimal MONTO_EJEMPLO = 100000;
        public decimal GananciaPorMontoEjemploUSD => Math.Round(MONTO_EJEMPLO * RentabilidadPlazoUSD / 100, 2);
        public decimal RetornoPorMontoEjemploUSD => MONTO_EJEMPLO + GananciaPorMontoEjemploUSD;
    }
}
