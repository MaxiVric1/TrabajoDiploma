using System;

namespace BE
{
    public class Captacion
    {
        public int NroCaptacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public decimal ValorPretendido { get; set; }
        public int IdInmueble { get; set; }
        public int IdUsuarioAgente { get; set; }

        public override string ToString()
        {
            return $"Captación N° {NroCaptacion}";
        }
    }
}
