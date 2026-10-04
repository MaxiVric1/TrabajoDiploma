using System;

namespace BE
{
    // Vista de una captación para listados: datos de Captacion + Inmueble + Cliente
    public class CaptacionResumen
    {
        public int NroCaptacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Cliente { get; set; }
        public string Direccion { get; set; }
        public string TipoInmueble { get; set; }
        public decimal MetrosCuadrados { get; set; }
        public int CantidadAmbientes { get; set; }
        public decimal ValorPretendido { get; set; }
        public decimal? ValorFinal { get; set; } // valor de la última tasación, si fue aprobada
        public int CantidadArchivos { get; set; } // material audiovisual cargado para el inmueble
        public EstadoInmueble EstadoInmueble { get; set; }
        public int IdInmueble { get; set; }
        public int IdCliente { get; set; }
    }
}
