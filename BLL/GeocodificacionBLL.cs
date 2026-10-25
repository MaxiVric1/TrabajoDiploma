using System;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Coordenadas de los inmuebles (para el mapa del PN3) a partir de dirección + localidad, con Nominatim
    public class GeocodificacionBLL
    {
        private readonly ClienteNominatim nominatim = new ClienteNominatim();
        private readonly InmuebleDAL inmuebleDAL = new InmuebleDAL();

        public string UltimoError { get; private set; }

        // Busca y guarda las coordenadas del inmueble. false si no se encontró la dirección o no hubo conexión
        // (UltimoError dice cuál); el inmueble queda sin coordenadas y se puede volver a intentar.
        public bool Geocodificar(Inmueble inmueble)
        {
            if (inmueble == null) throw new ArgumentNullException(nameof(inmueble));
            if (string.IsNullOrWhiteSpace(inmueble.Direccion) || string.IsNullOrWhiteSpace(inmueble.Localidad))
            {
                UltimoError = "El inmueble no tiene dirección y localidad para ubicarlo.";
                return false;
            }
            try
            {
                ClienteNominatim.Resultado r = nominatim.Geocodificar(inmueble.Direccion, inmueble.Localidad);
                if (r == null)
                {
                    UltimoError = $"No se encontró \"{inmueble.Direccion}, {inmueble.Localidad}\" en el mapa.";
                    return false;
                }
                if (!inmuebleDAL.ActualizarCoordenadas(inmueble.IdInmueble, r.Latitud, r.Longitud))
                {
                    UltimoError = "No se pudieron guardar las coordenadas.";
                    return false;
                }
                inmueble.Latitud = r.Latitud;
                inmueble.Longitud = r.Longitud;
                UltimoError = null;
                return true;
            }
            catch (Exception ex)
            {
                UltimoError = ex.Message;
                return false;
            }
        }
    }
}
