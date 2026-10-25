using System;

namespace BE
{
    // Los nombres deben coincidir con el CHECK de Inmueble.estado_actual en la base.
    // Ciclo: Captado -> Tasado -> Publicado; un inmueble de alquiler pasa a Alquilado con el contrato y vuelve a
    // Publicado cuando el contrato finaliza.
    public enum EstadoInmueble
    {
        Captado,
        Tasado,
        Rechazado,
        Publicado,
        Alquilado
    }

    // Los nombres deben coincidir con el CHECK de Inmueble.tipo_publicacion en la base
    public enum TipoPublicacion
    {
        Venta,
        Alquiler
    }

    public class Inmueble
    {
        public int IdInmueble { get; set; }
        public string TipoInmueble { get; set; }
        public string Direccion { get; set; }
        public decimal MetrosCuadrados { get; set; }
        public int CantidadAmbientes { get; set; }
        public EstadoInmueble EstadoActual { get; set; } = EstadoInmueble.Captado;
        public TipoPublicacion TipoPublicacion { get; set; } = TipoPublicacion.Venta;
        public string TextoComercial { get; set; } // texto de la publicación, lo carga Marketing
        public DateTime? FechaPublicacion { get; set; }    // última publicación
        public DateTime? FechaDespublicacion { get; set; } // última despublicación (null si está publicado)
        public int IdCliente { get; set; }

        // Ubicación y lote (PN3): la localidad es obligatoria en captaciones nuevas y la superficie del lote en casas;
        // las coordenadas las completa la geocodificación (Nominatim). null en los inmuebles anteriores.
        public string Localidad { get; set; }
        public decimal? SuperficieLote { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public bool Geocodificado => Latitud.HasValue && Longitud.HasValue;

        // Ya estuvo publicado y se retiró: vuelve a Tasado conservando la fecha de publicación
        public bool Despublicado => EstadoActual == EstadoInmueble.Tasado && FechaPublicacion.HasValue;

        public override string ToString()
        {
            return $"{TipoInmueble} - {Direccion}";
        }
    }
}
