using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using SERVICIOS;

namespace BLL
{
    // Tipos de cambio para el PN3: oficial (BCRA, minorista) y MEP / blue (DolarAPI). Las cotizaciones de DolarAPI
    // se guardan en memoria unos minutos para no consultar en cada cálculo.
    public class CotizacionesBLL
    {
        public const int ID_TIPO_CAMBIO_MINORISTA = 4; // BCRA: "Tipo de cambio minorista (promedio vendedor)", pesos por USD
        private const int MINUTOS_CACHE = 5;

        private static List<CotizacionDolar> _cache;
        private static DateTime _cacheFecha = DateTime.MinValue;
        private static readonly object _bloqueo = new object();

        private readonly ClienteDolarAPI dolarApi = new ClienteDolarAPI();
        private readonly IndicesBCRABLL indices = new IndicesBCRABLL();

        public string UltimoError { get; private set; }

        public List<CotizacionDolar> Cotizaciones()
        {
            lock (_bloqueo)
            {
                if (_cache != null && (DateTime.Now - _cacheFecha).TotalMinutes < MINUTOS_CACHE) return _cache;
                try
                {
                    _cache = dolarApi.ObtenerCotizaciones();
                    _cacheFecha = DateTime.Now;
                    UltimoError = null;
                }
                catch (Exception ex)
                {
                    UltimoError = ex.Message; // se sigue con la última obtenida, si hay
                }
                return _cache ?? new List<CotizacionDolar>();
            }
        }

        public CotizacionDolar Mep() => Cotizaciones().FirstOrDefault(c => c.Casa == ClienteDolarAPI.CASA_MEP);

        public CotizacionDolar Blue() => Cotizaciones().FirstOrDefault(c => c.Casa == ClienteDolarAPI.CASA_BLUE);

        // Tipo de cambio oficial del BCRA: el último publicado en los últimos 10 días (no hay valor los fines de
        // semana y feriados). null si no hay datos.
        public (DateTime Fecha, decimal Valor)? Oficial(DateTime? fecha = null)
        {
            DateTime hasta = (fecha ?? DateTime.Today).Date;
            DateTime desde = hasta.AddDays(-10);
            Dictionary<DateTime, decimal> serie = indices.Serie(ID_TIPO_CAMBIO_MINORISTA, desde, hasta,
                Enumerable.Range(0, 11).Select(i => hasta.AddDays(-i)).Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday).Take(1));
            if (indices.UltimoError != null) UltimoError = indices.UltimoError;
            if (serie.Count == 0) return null;
            var ultimo = serie.OrderByDescending(v => v.Key).First();
            return (ultimo.Key, ultimo.Value);
        }
    }
}
