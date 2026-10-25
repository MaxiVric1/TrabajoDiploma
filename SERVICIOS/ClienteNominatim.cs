using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

namespace SERVICIOS
{
    // Geocodificación con Nominatim (OpenStreetMap). Su política de uso exige un User-Agent que identifique a la
    // aplicación (LandSmart/1.0) y como máximo una consulta por segundo: las consultas se espacian solas.
    public class ClienteNominatim
    {
        public const string URL = "https://nominatim.openstreetmap.org/search";
        private static readonly object _bloqueo = new object();
        private static DateTime _ultimaConsulta = DateTime.MinValue;

        public class Resultado
        {
            public decimal Latitud { get; set; }
            public decimal Longitud { get; set; }
            public string Descripcion { get; set; } // dirección completa que encontró
        }

        [DataContract]
        private class Lugar
        {
            [DataMember(Name = "lat")] public string Lat { get; set; }
            [DataMember(Name = "lon")] public string Lon { get; set; }
            [DataMember(Name = "display_name")] public string Descripcion { get; set; }
        }

        // Dirección + localidad en Argentina; null si no la encuentra
        public Resultado Geocodificar(string direccion, string localidad)
        {
            string consulta = $"{direccion}, {localidad}, Argentina";
            string url = $"{URL}?q={Uri.EscapeDataString(consulta)}&format=json&limit=1&countrycodes=ar";

            string json;
            lock (_bloqueo)
            {
                TimeSpan espera = TimeSpan.FromMilliseconds(1100) - (DateTime.Now - _ultimaConsulta);
                if (espera > TimeSpan.Zero) Thread.Sleep(espera);
                try
                {
                    json = ClienteHttp.Get(url, "Nominatim", timeoutMs: 8000, accept: "application/json");
                }
                finally
                {
                    _ultimaConsulta = DateTime.Now;
                }
            }

            List<Lugar> lugares;
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    lugares = (List<Lugar>)new DataContractJsonSerializer(typeof(List<Lugar>)).ReadObject(ms);
            }
            catch (Exception ex)
            {
                throw new Exception("Nominatim respondió algo que no se pudo interpretar.", ex);
            }
            if (lugares == null || lugares.Count == 0) return null;
            return new Resultado
            {
                Latitud = Math.Round(decimal.Parse(lugares[0].Lat, CultureInfo.InvariantCulture), 6),
                Longitud = Math.Round(decimal.Parse(lugares[0].Lon, CultureInfo.InvariantCulture), 6),
                Descripcion = lugares[0].Descripcion
            };
        }
    }
}
