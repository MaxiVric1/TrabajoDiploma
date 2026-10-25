namespace BE
{
    // Importe de un mes de alquiler según la fecha de pago: alquiler del período + recargo por mora
    public class ImportePeriodo
    {
        public decimal Base { get; set; }
        public decimal Recargo { get; set; }
        public decimal Total => Base + Recargo;
        public int DiasMora { get; set; }

        // Tasa del recargo: la del período de mora y su equivalente anual (TNA), en %. FuenteTasa dice de dónde
        // salió (TIM del BCRA, o el porcentaje fijo cuando no hay datos del BCRA).
        public decimal TasaPeriodo { get; set; }
        public decimal TasaAnual { get; set; }
        public string FuenteTasa { get; set; }
        public bool TasaFija { get; set; }
    }
}
