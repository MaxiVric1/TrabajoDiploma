using System;

namespace BE
{
    // Índice del Costo de la Construcción (INDEC, nivel general) de un mes
    public class ValorICC
    {
        public DateTime Periodo { get; set; }           // primer día del mes
        public decimal ValorIndice { get; set; }
        public decimal? VariacionMensual { get; set; }  // en %, respecto del mes anterior
        public DateTime FechaConsulta { get; set; }

        public override string ToString()
        {
            return $"{Periodo:MM/yyyy}: {ValorIndice:N2}" + (VariacionMensual.HasValue ? $" ({VariacionMensual:+0.##;-0.##}%)" : "");
        }
    }
}
