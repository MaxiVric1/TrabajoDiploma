using System;
using System.IO;
using System.Net;
using System.Text;

namespace SERVICIOS
{
    // GET de texto para las APIs externas (DolarAPI, Nominatim, INDEC): TLS 1.2, tiempo máximo y errores con un
    // mensaje claro que nombra al servicio
    internal static class ClienteHttp
    {
        public const string USER_AGENT = "LandSmart/1.0";

        public static string Get(string url, string servicio, int timeoutMs = 15000, string accept = null)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            HttpWebRequest pedido = (HttpWebRequest)WebRequest.Create(url);
            pedido.Timeout = pedido.ReadWriteTimeout = timeoutMs;
            pedido.UserAgent = USER_AGENT;
            if (accept != null) pedido.Accept = accept;
            try
            {
                using (HttpWebResponse respuesta = (HttpWebResponse)pedido.GetResponse())
                using (StreamReader lector = new StreamReader(respuesta.GetResponseStream(), Encoding.UTF8))
                    return lector.ReadToEnd();
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse error)
            {
                throw new Exception($"{servicio} respondió con error HTTP {(int)error.StatusCode}.", ex);
            }
            catch (WebException ex)
            {
                throw new Exception($"No se pudo conectar con {servicio}: {ex.Message}", ex);
            }
        }
    }
}
