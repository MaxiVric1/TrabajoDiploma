using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using BE;

namespace SERVICIOS
{
    // DolarAPI (https://dolarapi.com, pública y sin autenticación): cotizaciones del dólar en Argentina.
    // Las casas que usa el PN3 son "bolsa" (MEP) y "blue".
    public class ClienteDolarAPI
    {
        public const string URL = "https://dolarapi.com/v1/dolares";
        public const string CASA_MEP = "bolsa";
        public const string CASA_BLUE = "blue";

        [DataContract]
        private class Dolar
        {
            [DataMember(Name = "casa")] public string Casa { get; set; }
            [DataMember(Name = "nombre")] public string Nombre { get; set; }
            [DataMember(Name = "compra")] public decimal? Compra { get; set; }
            [DataMember(Name = "venta")] public decimal? Venta { get; set; }
            [DataMember(Name = "fechaActualizacion")] public string Fecha { get; set; }
        }

        public List<CotizacionDolar> ObtenerCotizaciones()
        {
            string json = ClienteHttp.Get(URL, "DolarAPI", accept: "application/json");
            List<Dolar> dolares;
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    dolares = (List<Dolar>)new DataContractJsonSerializer(typeof(List<Dolar>)).ReadObject(ms);
            }
            catch (Exception ex)
            {
                throw new Exception("DolarAPI respondió algo que no se pudo interpretar.", ex);
            }
            return dolares.Select(d => new CotizacionDolar
            {
                Casa = d.Casa,
                Nombre = d.Nombre,
                Compra = d.Compra,
                Venta = d.Venta,
                FechaActualizacion = DateTime.TryParse(d.Fecha, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out DateTime fecha) ? fecha.ToLocalTime() : (DateTime?)null
            }).ToList();
        }

        // Cotización de una casa en una fecha pasada: ArgentinaDatos (https://argentinadatos.com, de los mismos autores
        // que DolarAPI, pública y sin autenticación). null si no hay cotización para ese día.
        public const string URL_HISTORICO = "https://api.argentinadatos.com/v1/cotizaciones/dolares/{0}/{1:yyyy}/{1:MM}/{1:dd}";

        [DataContract]
        private class DolarHistorico
        {
            [DataMember(Name = "casa")] public string Casa { get; set; }
            [DataMember(Name = "compra")] public decimal? Compra { get; set; }
            [DataMember(Name = "venta")] public decimal? Venta { get; set; }
            [DataMember(Name = "fecha")] public string Fecha { get; set; }
        }

        public CotizacionDolar ObtenerHistorico(string casa, DateTime fecha)
        {
            string json;
            try
            {
                json = ClienteHttp.Get(string.Format(CultureInfo.InvariantCulture, URL_HISTORICO, casa, fecha), "ArgentinaDatos", accept: "application/json");
            }
            catch (Exception ex) when (ex.Message.Contains("HTTP 404"))
            {
                return null;
            }
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DolarHistorico d = (DolarHistorico)new DataContractJsonSerializer(typeof(DolarHistorico)).ReadObject(ms);
                    return new CotizacionDolar
                    {
                        Casa = d.Casa,
                        Compra = d.Compra,
                        Venta = d.Venta,
                        FechaActualizacion = DateTime.TryParseExact(d.Fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime f) ? f : fecha.Date
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ArgentinaDatos respondió algo que no se pudo interpretar.", ex);
            }
        }
    }
}
