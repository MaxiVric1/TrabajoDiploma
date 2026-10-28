using System;

namespace BE
{
    // Vista de una captación para listados: datos de Captacion + Inmueble + Cliente
    public class CaptacionResumen
    {
        public int NroCaptacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Cliente { get; set; }
        public string Agente { get; set; } // nombre y apellido del agente comercial que registró la captación
        public string Direccion { get; set; }
        public string TipoInmueble { get; set; }
        public TipoPublicacion TipoPublicacion { get; set; } // Venta / Alquiler
        public decimal MetrosCuadrados { get; set; }
        public int CantidadAmbientes { get; set; }
        public decimal ValorPretendido { get; set; }
        public decimal? ValorFinal { get; set; } // valor de la última tasación, si fue aprobada
        public int CantidadArchivos { get; set; } // material audiovisual cargado para el inmueble
        public string EstadoPublicacion { get; set; } // "Publicado" / "Despublicado" / "Alquilado" (vacío si nunca se publicó)
        public DateTime? FechaPublicacion { get; set; }
        public DateTime? FechaDespublicacion { get; set; }
        public EstadoInmueble EstadoInmueble { get; set; }
        public int IdInmueble { get; set; }
        public int IdCliente { get; set; }
    }
}
