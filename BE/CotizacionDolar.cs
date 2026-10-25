using System;

namespace BE
{
    // Cotización de un tipo de dólar (DolarAPI: "oficial", "blue", "bolsa" = MEP, ...), en pesos por dólar
    public class CotizacionDolar
    {
        public string Casa { get; set; }
        public string Nombre { get; set; }
        public decimal? Compra { get; set; }
        public decimal? Venta { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public override string ToString()
        {
            return $"{Nombre}: compra {Compra:N2} / venta {Venta:N2}";
        }
    }
}
