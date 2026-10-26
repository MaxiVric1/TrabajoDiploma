using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Series del BCRA que usa el PN2: primero se buscan en la base (valores ya consultados) y lo que falta se pide
    // a la API y se guarda. Sin conexión se trabaja con lo guardado.
    public class IndicesBCRABLL
    {
        public const int ID_INFLACION_MENSUAL = 27;  // "Inflación mensual" (IPC, variación % del mes; fecha = último día del mes)
        public const int ID_TASA_PLAZO_FIJO = 1190;  // "Tasa de interés de depósitos a plazo fijo en pesos de personas humanas" (% n.a.)
        public const int ID_TIM = 1197;              // "Tasa de Intereses Moratorios (TIM) CCC, art. 768(c)": índice diario

        // Una consulta a la API por serie y rango cada tantos minutos como máximo (los datos que no están
        // publicados todavía no se vuelven a pedir en cada cálculo)
        private const int MINUTOS_ENTRE_CONSULTAS = 10;
        private static readonly Dictionary<string, DateTime> _ultimaConsulta = new Dictionary<string, DateTime>();

        private readonly IndiceBCRADAL indiceDAL = new IndiceBCRADAL();
        private readonly ClienteBCRA cliente = new ClienteBCRA();
        private readonly Dictionary<int, Dictionary<DateTime, decimal>> _memoria = new Dictionary<int, Dictionary<DateTime, decimal>>();

        // Último error de la API (null si la última consulta funcionó): para avisar que se usan datos guardados
        public string UltimoError { get; private set; }

        // Valores de la serie entre las fechas. Si falta alguna de las fechas pedidas en 'necesarias', consulta la API.
        public Dictionary<DateTime, decimal> Serie(int idVariable, DateTime desde, DateTime hasta, IEnumerable<DateTime> necesarias)
        {
            desde = desde.Date;
            hasta = hasta.Date;
            if (!_memoria.TryGetValue(idVariable, out Dictionary<DateTime, decimal> conocidos))
                _memoria[idVariable] = conocidos = new Dictionary<DateTime, decimal>();

            List<DateTime> faltantes = necesarias.Select(f => f.Date).Where(f => !conocidos.ContainsKey(f)).Distinct().ToList();
            if (faltantes.Count > 0)
            {
                foreach (var v in indiceDAL.Leer(idVariable, desde, hasta)) conocidos[v.Key] = v.Value;
                faltantes = faltantes.Where(f => !conocidos.ContainsKey(f)).ToList();
            }
            if (faltantes.Count > 0 && PuedeConsultar(idVariable, faltantes.Min(), faltantes.Max()))
            {
                try
                {
                    Dictionary<DateTime, decimal> nuevos = cliente.ObtenerSerie(idVariable, faltantes.Min(), faltantes.Max());
                    if (nuevos.Count > 0) indiceDAL.Guardar(idVariable, nuevos);
                    foreach (var v in nuevos) conocidos[v.Key] = v.Value;
                    UltimoError = null;
                }
                catch (Exception ex)
                {
                    UltimoError = ex.Message;
                }
            }
            return conocidos.Where(v => v.Key >= desde && v.Key <= hasta).ToDictionary(v => v.Key, v => v.Value);
        }

        private static bool PuedeConsultar(int idVariable, DateTime desde, DateTime hasta)
        {
            string clave = $"{idVariable}|{desde:yyyyMMdd}|{hasta:yyyyMMdd}";
            lock (_ultimaConsulta)
            {
                if (_ultimaConsulta.TryGetValue(clave, out DateTime ultima) && (DateTime.Now - ultima).TotalMinutes < MINUTOS_ENTRE_CONSULTAS)
                    return false;
                _ultimaConsulta[clave] = DateTime.Now;
                return true;
            }
        }

        // ---------- IPC ----------

        public static DateTime FechaMes(int anio, int mes) => new DateTime(anio, mes, 1).AddMonths(1).AddDays(-1);

        // Inflación mensual (en %) de cada mes pedido; null en los meses que el BCRA todavía no publicó
        public Dictionary<DateTime, decimal?> InflacionMensual(IEnumerable<DateTime> meses)
        {
            List<DateTime> fechas = meses.Select(m => FechaMes(m.Year, m.Month)).ToList();
            Dictionary<DateTime, decimal> serie = Serie(ID_INFLACION_MENSUAL, fechas.Min().AddDays(-1), fechas.Max(), fechas);
            return fechas.ToDictionary(f => f, f => serie.TryGetValue(f, out decimal v) ? v : (decimal?)null);
        }

        // ---------- Plazo fijo ----------

        // Tasa nominal anual (en %) de plazo fijo en pesos de personas humanas: la última publicada en los últimos
        // 10 días (no hay valor los fines de semana y feriados, y se publica con unos días de demora). null sin datos.
        public (DateTime Fecha, decimal Tna)? TasaPlazoFijo()
        {
            DateTime hasta = DateTime.Today;
            Dictionary<DateTime, decimal> serie = Serie(ID_TASA_PLAZO_FIJO, hasta.AddDays(-10), hasta,
                Enumerable.Range(1, 10).Select(i => hasta.AddDays(-i)).Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday));
            if (serie.Count == 0) return null;
            var ultimo = serie.OrderByDescending(v => v.Key).First();
            return (ultimo.Key, ultimo.Value);
        }

        // ---------- TIM ----------

        // Variación de la TIM entre dos fechas (tasa del período, en fracción: 0,0123 = 1,23%); null sin datos
        public decimal? VariacionTim(DateTime desde, DateTime hasta)
        {
            Dictionary<DateTime, decimal> serie = Serie(ID_TIM, desde, hasta, new[] { desde, hasta });
            if (!serie.TryGetValue(desde.Date, out decimal inicial) || !serie.TryGetValue(hasta.Date, out decimal final) || inicial <= 0)
                return null;
            return final / inicial - 1;
        }
    }
}
