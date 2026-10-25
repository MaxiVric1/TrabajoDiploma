using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class ResultadoActualizacionIpc
    {
        public List<ActualizacionContrato> Aplicadas { get; } = new List<ActualizacionContrato>();
        public List<string> Pendientes { get; } = new List<string>(); // actualizaciones que corresponden y no se pudieron hacer
    }

    // Actualización del monto mensual por IPC (datos del BCRA): cada PeriodicidadActualizacionMeses meses del contrato
    // el monto se multiplica por la inflación acumulada de los 3 últimos meses publicados.
    // - La actualización n rige desde el período n x periodicidad del contrato (los anteriores conservan su monto).
    // - Usa los meses M-4, M-3 y M-2 respecto del mes M desde el que rige: el INDEC publica cada mes a mediados
    //   del siguiente, así que esos tres ya están publicados al empezar el mes M.
    public class ActualizacionIpcBLL
    {
        public const int MESES_IPC = 3;

        private ContratoDAL contratoDAL = new ContratoDAL();
        private ActualizacionContratoDAL actualizacionDAL = new ActualizacionContratoDAL();
        private IndicesBCRABLL indices = new IndicesBCRABLL();

        public string ErrorBCRA => indices.UltimoError;

        // Primer día de cada período desde el que rige una actualización (sin la del inicio del contrato)
        public static List<DateTime> FechasDeActualizacion(Contrato contrato)
        {
            List<(int Mes, int Anio)> periodos = PagoAlquilerBLL.PeriodosDelContrato(contrato);
            int cada = Math.Max(1, contrato.PeriodicidadActualizacionMeses);
            List<DateTime> fechas = new List<DateTime>();
            for (int i = cada; i < periodos.Count; i += cada)
                fechas.Add(new DateTime(periodos[i].Anio, periodos[i].Mes, 1));
            return fechas;
        }

        // Meses de IPC que usa la actualización que rige desde 'fechaActualizacion'
        public static List<DateTime> MesesIpc(DateTime fechaActualizacion)
        {
            DateTime mes = new DateTime(fechaActualizacion.Year, fechaActualizacion.Month, 1);
            return Enumerable.Range(0, MESES_IPC).Select(i => mes.AddMonths(-(MESES_IPC + 1) + i)).ToList();
        }

        public List<ActualizacionContrato> LeerHistorial(int idContrato) => actualizacionDAL.LeerPorContrato(idContrato);

        // Próxima fecha de actualización todavía no aplicada (null si al contrato no le quedan)
        public DateTime? ProximaActualizacion(Contrato contrato, List<ActualizacionContrato> historial)
        {
            HashSet<DateTime> aplicadas = new HashSet<DateTime>(historial.Select(a => a.FechaActualizacion.Date));
            return FechasDeActualizacion(contrato).Where(f => !aplicadas.Contains(f)).Cast<DateTime?>().FirstOrDefault();
        }

        // Cálculo de una actualización (sin guardarla). null si el IPC de algún mes no está disponible (motivo dice cuál).
        public ActualizacionContrato Calcular(Contrato contrato, DateTime fechaActualizacion, decimal montoAnterior, out string motivo)
        {
            List<DateTime> meses = MesesIpc(fechaActualizacion);
            Dictionary<DateTime, decimal?> ipc = indices.InflacionMensual(meses);
            List<DateTime> faltan = ipc.Where(v => !v.Value.HasValue).Select(v => v.Key).ToList();
            if (faltan.Count > 0)
            {
                motivo = $"Contrato N° {contrato.IdContrato}: falta el IPC de {string.Join(", ", faltan.Select(f => f.ToString("MM/yyyy")))}" +
                         (ErrorBCRA != null ? $" (BCRA: {ErrorBCRA})" : " (todavía no publicado)") +
                         $" para la actualización del {fechaActualizacion:dd/MM/yyyy}.";
                return null;
            }

            decimal factor = ipc.OrderBy(v => v.Key).Aggregate(1m, (acum, v) => acum * (1 + v.Value.Value / 100));
            motivo = null;
            return new ActualizacionContrato
            {
                IdContrato = contrato.IdContrato,
                FechaActualizacion = fechaActualizacion,
                Indice = "IPC",
                Porcentaje = Math.Round((factor - 1) * 100, 4),
                Detalle = "IPC " + string.Join(" - ", ipc.OrderBy(v => v.Key)
                    .Select(v => $"{v.Key:MM/yyyy} {v.Value.Value.ToString("0.##", CultureInfo.GetCultureInfo("es-AR"))}%")),
                MontoAnterior = montoAnterior,
                MontoNuevo = Math.Round(montoAnterior * factor, 2),
                IdUsuario = SessionManager.Instance.EstaLogueado() ? SessionManager.Instance.UsuarioActual.Id : (int?)null
            };
        }

        // Proceso automático (lo ejecutan los formularios del PN2 al abrirse): aplica, en orden, las actualizaciones
        // ya vencidas de los contratos vigentes. Cada una se guarda junto con el nuevo monto en una transacción.
        public ResultadoActualizacionIpc AplicarPendientes(DateTime? fechaReferencia = null)
        {
            DateTime hoy = (fechaReferencia ?? DateTime.Now).Date;
            ResultadoActualizacionIpc resultado = new ResultadoActualizacionIpc();

            foreach (Contrato contrato in contratoDAL.LeerTodos().Where(c => c.Vigente && c.IndiceActualizacion == "IPC"))
            {
                HashSet<DateTime> aplicadas = new HashSet<DateTime>(LeerHistorial(contrato.IdContrato).Select(a => a.FechaActualizacion.Date));
                foreach (DateTime fecha in FechasDeActualizacion(contrato).Where(f => f <= hoy && !aplicadas.Contains(f)))
                {
                    ActualizacionContrato actualizacion = Calcular(contrato, fecha, contrato.MontoMensual, out string motivo);
                    if (actualizacion == null)
                    {
                        resultado.Pendientes.Add(motivo);
                        break; // las siguientes dependen de esta
                    }
                    if (!actualizacionDAL.Aplicar(actualizacion))
                    {
                        resultado.Pendientes.Add($"Contrato N° {contrato.IdContrato}: no se pudo aplicar la actualización del {fecha:dd/MM/yyyy}.");
                        break;
                    }

                    contrato.MontoMensual = actualizacion.MontoNuevo;
                    resultado.Aplicadas.Add(actualizacion);
                    GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Actualización IPC",
                        $"Contrato N° {contrato.IdContrato}: monto ${actualizacion.MontoAnterior:N2} -> ${actualizacion.MontoNuevo:N2} " +
                        $"({actualizacion.Porcentaje:+0.####;-0.####}%, {actualizacion.Detalle}), rige desde {fecha:dd/MM/yyyy}.");
                }
            }
            return resultado;
        }
    }
}
