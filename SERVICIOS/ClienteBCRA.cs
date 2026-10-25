using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SERVICIOS
{
    // Cliente de la API de Estadísticas Monetarias del BCRA (v4.0, pública y sin autenticación):
    // https://api.bcra.gob.ar/estadisticas/v4.0/monetarias/{idVariable}?desde=yyyy-MM-dd&hasta=yyyy-MM-dd
    // Devuelve hasta 3000 valores por consulta; las series más largas se piden por páginas (offset).
    public class ClienteBCRA
    {
        public const string URL_BASE = "https://api.bcra.gob.ar/estadisticas/v4.0/monetarias";
        private const int LIMITE = 3000;
        private const int TIMEOUT_MS = 15000;

        // ---------- Respuesta JSON ----------
        [DataContract]
        private class Respuesta
        {
            [DataMember(Name = "status")] public int Status { get; set; }
            [DataMember(Name = "errorMessages")] public List<string> Errores { get; set; }
            [DataMember(Name = "metadata")] public Metadata Metadata { get; set; }
            [DataMember(Name = "results")] public List<Serie> Resultados { get; set; }
        }

        [DataContract]
        private class Metadata
        {
            [DataMember(Name = "resultset")] public Resultset Resultset { get; set; }
        }

        [DataContract]
        private class Resultset
        {
            [DataMember(Name = "count")] public int Cantidad { get; set; }
        }

        [DataContract]
        private class Serie
        {
            [DataMember(Name = "idVariable")] public int IdVariable { get; set; }
            [DataMember(Name = "detalle")] public List<Punto> Detalle { get; set; }
        }

        [DataContract]
        private class Punto
        {
            [DataMember(Name = "fecha")] public string Fecha { get; set; }
            [DataMember(Name = "valor")] public decimal Valor { get; set; }
        }

        // Valores de la variable entre las dos fechas (inclusive), por fecha. Lanza una excepción con un mensaje
        // claro si no hay conexión o la API responde con error.
        public Dictionary<DateTime, decimal> ObtenerSerie(int idVariable, DateTime desde, DateTime hasta)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            Dictionary<DateTime, decimal> valores = new Dictionary<DateTime, decimal>();
            int offset = 0;
            while (true)
            {
                string url = string.Format(CultureInfo.InvariantCulture, "{0}/{1}?desde={2:yyyy-MM-dd}&hasta={3:yyyy-MM-dd}&limit={4}&offset={5}",
                    URL_BASE, idVariable, desde, hasta, LIMITE, offset);
                Respuesta respuesta = Consultar(url);
                int recibidos = 0;
                foreach (Serie serie in respuesta.Resultados ?? new List<Serie>())
                {
                    foreach (Punto punto in serie.Detalle ?? new List<Punto>())
                    {
                        valores[DateTime.ParseExact(punto.Fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture)] = punto.Valor;
                        recibidos++;
                    }
                }
                int total = respuesta.Metadata?.Resultset?.Cantidad ?? 0;
                offset += recibidos;
                if (recibidos == 0 || offset >= total) break;
            }
            return valores;
        }

        private static Respuesta Consultar(string url)
        {
            HttpWebRequest pedido = (HttpWebRequest)WebRequest.Create(url);
            pedido.Timeout = pedido.ReadWriteTimeout = TIMEOUT_MS;
            pedido.Accept = "application/json";
            pedido.UserAgent = "LandSmart";

            string cuerpo;
            try
            {
                using (HttpWebResponse r = (HttpWebResponse)pedido.GetResponse())
                using (StreamReader lector = new StreamReader(r.GetResponseStream(), Encoding.UTF8))
                    cuerpo = lector.ReadToEnd();
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse error)
            {
                // La API informa los errores (fechas inválidas, versión deprecada...) con un JSON y un código HTTP
                using (StreamReader lector = new StreamReader(error.GetResponseStream(), Encoding.UTF8))
                    cuerpo = lector.ReadToEnd();
            }
            catch (WebException ex)
            {
                throw new Exception("No se pudo conectar con la API del BCRA: " + ex.Message, ex);
            }

            Respuesta respuesta;
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(cuerpo)))
                    respuesta = (Respuesta)new DataContractJsonSerializer(typeof(Respuesta)).ReadObject(ms);
            }
            catch (Exception ex)
            {
                throw new Exception("La API del BCRA respondió algo que no se pudo interpretar.", ex);
            }
            if (respuesta.Status != 200)
                throw new Exception($"La API del BCRA respondió con error {respuesta.Status}: " +
                    string.Join(" ", respuesta.Errores ?? new List<string>()));
            return respuesta;
        }
    }
}
