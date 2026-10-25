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
    }
}
