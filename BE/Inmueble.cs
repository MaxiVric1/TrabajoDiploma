using System;

namespace BE
{
    // Los nombres deben coincidir con el CHECK de Inmueble.estado_actual en la base
    public enum EstadoInmueble
    {
        Captado,
        Tasado,
        Rechazado,
        Publicado
    }

    public class Inmueble
    {
        public int IdInmueble { get; set; }
        public string TipoInmueble { get; set; }
        public string Direccion { get; set; }
        public decimal MetrosCuadrados { get; set; }
        public int CantidadAmbientes { get; set; }
        public EstadoInmueble EstadoActual { get; set; } = EstadoInmueble.Captado;
        public string TextoComercial { get; set; } // texto de la publicación, lo carga Marketing
        public DateTime? FechaPublicacion { get; set; }    // última publicación
        public DateTime? FechaDespublicacion { get; set; } // última despublicación (null si está publicado)
        public int IdCliente { get; set; }

        // Ya estuvo publicado y se retiró: vuelve a Tasado conservando la fecha de publicación
        public bool Despublicado => EstadoActual == EstadoInmueble.Tasado && FechaPublicacion.HasValue;

        public override string ToString()
        {
            return $"{TipoInmueble} - {Direccion}";
        }
    }
}
