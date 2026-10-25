using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Índice del Costo de la Construcción (INDEC). Se trabaja con lo guardado en HistorialICC; el CSV se vuelve a
    // descargar solo cuando falta el último mes que ya debería estar publicado (el INDEC publica cada mes a mediados
    // del siguiente) y, como mucho, una vez cada tanto. Sin conexión se usa lo guardado.
    public class IccBLL
    {
        private const int MINUTOS_ENTRE_DESCARGAS = 30;
        private static DateTime _ultimaDescarga = DateTime.MinValue;
        private static readonly object _bloqueo = new object();

        private readonly HistorialICCDAL historialDAL = new HistorialICCDAL();
        private readonly ClienteICC cliente = new ClienteICC();

        // Último error al descargar (null si la última descarga funcionó o no hizo falta)
        public string UltimoError { get; private set; }

        // Serie completa (actualizada si hace falta), del mes más antiguo al más reciente
        public List<ValorICC> Serie()
        {
            List<ValorICC> guardados = historialDAL.Leer();
            if (FaltanDatos(guardados, DateTime.Now) && PuedeDescargar())
            {
                try
                {
                    Dictionary<DateTime, decimal> descargados = cliente.ObtenerNivelGeneral();
                    List<ValorICC> nuevos = ConVariaciones(descargados);
                    Dictionary<DateTime, ValorICC> anteriores = guardados.ToDictionary(v => v.Periodo);
                    // Solo los meses nuevos o con valor revisado por el INDEC
                    List<ValorICC> cambios = nuevos.Where(v => !anteriores.TryGetValue(v.Periodo, out ValorICC a) ||
                                                               Math.Round(a.ValorIndice, 4) != Math.Round(v.ValorIndice, 4)).ToList();
                    if (cambios.Count > 0 && !historialDAL.Guardar(cambios))
                        throw new Exception("No se pudo guardar el ICC descargado.");
                    UltimoError = null;
                    guardados = historialDAL.Leer();
                }
                catch (Exception ex)
                {
                    UltimoError = ex.Message;
                }
            }
            return guardados;
        }

        public ValorICC Ultimo() => Serie().LastOrDefault();

        // Valor de un mes (null si no está publicado o no se pudo descargar)
        public ValorICC DelMes(int anio, int mes) => Serie().FirstOrDefault(v => v.Periodo.Year == anio && v.Periodo.Month == mes);

        // Variación % entre dos meses (null si falta alguno)
        public decimal? Variacion(DateTime desde, DateTime hasta)
        {
            List<ValorICC> serie = Serie();
            ValorICC a = serie.FirstOrDefault(v => v.Periodo == new DateTime(desde.Year, desde.Month, 1));
            ValorICC b = serie.FirstOrDefault(v => v.Periodo == new DateTime(hasta.Year, hasta.Month, 1));
            if (a == null || b == null) return null;
            return Math.Round((b.ValorIndice / a.ValorIndice - 1) * 100, 4);
        }

        // Falta el mes anterior al actual (publicado a mediados de este mes) o no hay nada guardado
        public static bool FaltanDatos(List<ValorICC> guardados, DateTime hoy)
        {
            if (guardados.Count == 0) return true;
            DateTime esperado = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-1);
            return guardados.Max(v => v.Periodo) < esperado;
        }

        private static bool PuedeDescargar()
        {
            lock (_bloqueo)
            {
                if ((DateTime.Now - _ultimaDescarga).TotalMinutes < MINUTOS_ENTRE_DESCARGAS) return false;
                _ultimaDescarga = DateTime.Now;
                return true;
            }
        }

        // Variación mensual de cada mes respecto del anterior (el primero queda sin variación)
        private static List<ValorICC> ConVariaciones(Dictionary<DateTime, decimal> valores)
        {
            List<ValorICC> lista = new List<ValorICC>();
            ValorICC anterior = null;
            foreach (var v in valores.OrderBy(x => x.Key))
            {
                ValorICC actual = new ValorICC
                {
                    Periodo = v.Key,
                    ValorIndice = Math.Round(v.Value, 4),
                    VariacionMensual = anterior != null && anterior.Periodo == v.Key.AddMonths(-1)
                        ? Math.Round((v.Value / anterior.ValorIndice - 1) * 100, 4) : (decimal?)null
                };
                lista.Add(actual);
                anterior = actual;
            }
            return lista;
        }
    }
}
