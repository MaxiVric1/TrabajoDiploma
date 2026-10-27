using System;
using System.Collections.Generic;
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

        // Últimas búsquedas (también las no encontradas): la validación en tiempo real de Registrar Captación y el
        // guardado de la captación consultan la misma dirección, y Nominatim pide no repetir consultas
        private const int MAXIMO_RECORDADAS = 200;
        private static readonly object _bloqueo = new object();
        private static readonly Dictionary<string, ClienteNominatim.Resultado> _recordadas =
            new Dictionary<string, ClienteNominatim.Resultado>(StringComparer.OrdinalIgnoreCase);

        // Ubicación de una dirección + localidad; null si Nominatim no la encuentra. Si no hay conexión lanza la excepción.
        public ClienteNominatim.Resultado Buscar(string direccion, string localidad)
        {
            string clave = (direccion ?? "").Trim() + "|" + (localidad ?? "").Trim();
            lock (_bloqueo)
                if (_recordadas.TryGetValue(clave, out ClienteNominatim.Resultado recordada)) return recordada;

            ClienteNominatim.Resultado r = nominatim.Geocodificar(direccion.Trim(), localidad.Trim());
            lock (_bloqueo)
            {
                if (_recordadas.Count >= MAXIMO_RECORDADAS) _recordadas.Clear();
                _recordadas[clave] = r;
            }
            return r;
        }

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
                ClienteNominatim.Resultado r = Buscar(inmueble.Direccion, inmueble.Localidad);
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
