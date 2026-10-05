using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class PagoAlquilerBLL
    {
        // Reglas de cobro: se paga hasta el día 10; desde el 11 el mes impago está En Mora (con recargo) y
        // desde el día 1 del mes siguiente, En Litigio.
        public const int DIA_VENCIMIENTO = 10;
        public const decimal PORCENTAJE_RECARGO = 0.10m;

        private PagoAlquilerDAL pagoDAL = new PagoAlquilerDAL();
        private ContratoDAL contratoDAL = new ContratoDAL();

        // ---------- Reglas (sin acceso a la base) ----------

        public static DateTime FechaVencimiento(int mes, int anio) => new DateTime(anio, mes, DIA_VENCIMIENTO);

        // Estado de un mes que todavía no se pagó; null si aún no venció (hasta el día 10 inclusive)
        public static EstadoPago? EstadoSegunFecha(int mes, int anio, DateTime hoy)
        {
            if (hoy.Date <= FechaVencimiento(mes, anio)) return null;
            if (hoy.Date < new DateTime(anio, mes, 1).AddMonths(1)) return EstadoPago.EnMora;
            return EstadoPago.EnLitigio;
        }

        public static decimal CalcularRecargo(decimal montoMensual) => Math.Round(montoMensual * PORCENTAJE_RECARGO, 2);

        // Meses que se pagan durante el contrato: arranca en el mes de inicio si el contrato empieza hasta el
        // día 10, si no en el mes siguiente (ese mes ya estaría vencido al firmar). Son DuracionMeses meses.
        public static List<(int Mes, int Anio)> PeriodosDelContrato(Contrato contrato)
        {
            DateTime primero = new DateTime(contrato.FechaInicio.Year, contrato.FechaInicio.Month, 1);
            if (contrato.FechaInicio.Day > DIA_VENCIMIENTO) primero = primero.AddMonths(1);

            List<(int Mes, int Anio)> periodos = new List<(int Mes, int Anio)>();
            for (int i = 0; i < contrato.DuracionMeses; i++)
            {
                DateTime p = primero.AddMonths(i);
                periodos.Add((p.Month, p.Year));
            }
            return periodos;
        }

        // Importe de un mes según la fecha en que se paga: hasta el día 10 el monto mensual; después, monto + 10%.
        // Es el mismo cálculo que valida RegistrarPago.
        public static (decimal Base, decimal Recargo, decimal Total) CalcularImporte(Contrato contrato, int mes, int anio, DateTime fechaPago)
        {
            decimal recargo = fechaPago.Date > FechaVencimiento(mes, anio) ? CalcularRecargo(contrato.MontoMensual) : 0;
            return (contrato.MontoMensual, recargo, contrato.MontoMensual + recargo);
        }

        // ---------- Operaciones ----------

        // Meses del contrato sin pago registrado, desde el primero hasta el mes en curso (los futuros no se adelantan):
        // los vencidos (En Mora / En Litigio / regularizados) y el del mes actual si todavía está pendiente
        public List<PeriodoAPagar> LeerPeriodosAPagar(Contrato contrato, DateTime hoy)
        {
            Dictionary<(int, int), PagoAlquiler> registrados = pagoDAL.LeerPorContrato(contrato.IdContrato)
                .ToDictionary(p => (p.MesCorrespondiente, p.AnioCorrespondiente));
            DateTime mesEnCurso = new DateTime(hoy.Year, hoy.Month, 1);

            List<PeriodoAPagar> periodos = new List<PeriodoAPagar>();
            foreach (var (mes, anio) in PeriodosDelContrato(contrato))
            {
                if (new DateTime(anio, mes, 1) > mesEnCurso) break;
                registrados.TryGetValue((mes, anio), out PagoAlquiler registro);
                if (registro != null && registro.Pagado) continue;

                periodos.Add(registro != null
                    ? new PeriodoAPagar { Mes = mes, Anio = anio, Estado = registro.EstadoPago == EstadoPago.AlDia ? (EstadoPago?)null : registro.EstadoPago, Regularizado = registro.EstadoPago == EstadoPago.AlDia }
                    : new PeriodoAPagar { Mes = mes, Anio = anio, Estado = EstadoSegunFecha(mes, anio, hoy) });
            }
            return periodos;
        }

        // Registra el pago de un mes. Hasta el día 10 se paga el monto mensual; después, monto + recargo del 10%.
        // Si el mes ya estaba En Mora o En Litigio, el pago lo regulariza (queda Al día con el recargo registrado).
        public int RegistrarPago(int idContrato, int mes, int anio, decimal montoPagado, DateTime fechaPago)
        {
            if (!SessionManager.Instance.EstaLogueado())
                throw new ArgumentException("El pago debe registrarlo un analista con sesión iniciada.");

            Contrato contrato = contratoDAL.BuscarPorId(idContrato);
            if (contrato == null)
                throw new ArgumentException("El contrato no existe.");
            if (!PeriodosDelContrato(contrato).Contains((mes, anio)))
                throw new ArgumentException($"El período {mes:00}/{anio} no corresponde al contrato N° {idContrato}.");
            if (fechaPago.Date > DateTime.Now.Date)
                throw new ArgumentException("La fecha de pago no puede ser futura.");

            PagoAlquiler existente = pagoDAL.BuscarPorPeriodo(idContrato, mes, anio);
            if (existente != null && existente.Pagado)
                throw new ArgumentException($"El período {mes:00}/{anio} ya tiene un pago registrado el {existente.FechaPago:dd/MM/yyyy}.");

            var (_, recargo, totalAPagar) = CalcularImporte(contrato, mes, anio, fechaPago);
            bool fueraDeTermino = recargo > 0;
            if (montoPagado < totalAPagar)
                throw new ArgumentException(fueraDeTermino
                    ? $"Pago fuera de término: el monto debe ser al menos ${totalAPagar:N2} (alquiler ${contrato.MontoMensual:N2} + recargo por mora ${recargo:N2})."
                    : $"El monto debe ser al menos ${totalAPagar:N2}.");

            PagoAlquiler pago = existente ?? new PagoAlquiler { IdContrato = idContrato, MesCorrespondiente = mes, AnioCorrespondiente = anio };
            EstadoPago? estadoAnterior = existente?.EstadoPago;
            pago.FechaPago = fechaPago;
            pago.MontoPagado = montoPagado;
            pago.EstadoPago = EstadoPago.AlDia;
            pago.RecargoAplicado = recargo;
            pago.IdUsuarioAnalista = SessionManager.Instance.UsuarioActual.Id;

            bool ok = existente != null ? pagoDAL.Actualizar(pago) : pagoDAL.Crear(pago) != -1;
            if (!ok)
                throw new Exception("No se pudo registrar el pago.");

            string detalle = $"Contrato N° {idContrato}, período {mes:00}/{anio}: ${montoPagado:N2}";
            if (recargo > 0) detalle += $" (incluye recargo ${recargo:N2})";
            if (estadoAnterior.HasValue && estadoAnterior != EstadoPago.AlDia) detalle += $"; regulariza un período {estadoAnterior}";
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Registro Pago", detalle + ".");
            return pago.IdPago;
        }

        // Proceso automático de mora y litigio (lo ejecutan los formularios al abrirse).
        // - Meses vencidos sin registro de contratos vigentes: se crean En Mora (con recargo) o En Litigio.
        // - Meses impagos registrados En Mora: pasan a En Litigio según la fecha (nunca retroceden; los regularizados
        //   por una acción legal "Resuelto" quedan Al día).
        // Todo se guarda en una sola transacción. Devuelve cuántos meses quedaron en mora y en litigio por este proceso.
        public (int EnMora, int EnLitigio) ActualizarEstadosAutomaticos(DateTime? fechaReferencia = null)
        {
            if (!SessionManager.Instance.EstaLogueado())
                throw new ArgumentException("El proceso de mora debe ejecutarlo un usuario con sesión iniciada.");

            DateTime hoy = (fechaReferencia ?? DateTime.Now).Date;
            int idAnalista = SessionManager.Instance.UsuarioActual.Id;

            List<Contrato> contratos = contratoDAL.LeerTodos();
            List<PagoAlquiler> pagos = pagoDAL.LeerTodos();
            HashSet<(int, int, int)> registrados = new HashSet<(int, int, int)>(
                pagos.Select(p => (p.IdContrato, p.MesCorrespondiente, p.AnioCorrespondiente)));

            List<PagoAlquiler> nuevos = new List<PagoAlquiler>();
            List<PagoAlquiler> cambios = new List<PagoAlquiler>();

            // Meses vencidos sin registro (solo contratos vigentes: uno finalizado no genera deuda nueva)
            foreach (Contrato contrato in contratos.Where(c => c.Vigente))
            {
                foreach (var (mes, anio) in PeriodosDelContrato(contrato))
                {
                    EstadoPago? estado = EstadoSegunFecha(mes, anio, hoy);
                    if (estado == null) break; // los meses siguientes tampoco vencieron
                    if (registrados.Contains((contrato.IdContrato, mes, anio))) continue;

                    nuevos.Add(new PagoAlquiler
                    {
                        IdContrato = contrato.IdContrato,
                        MesCorrespondiente = mes,
                        AnioCorrespondiente = anio,
                        FechaPago = null,
                        MontoPagado = 0,
                        EstadoPago = estado.Value,
                        RecargoAplicado = CalcularRecargo(contrato.MontoMensual),
                        IdUsuarioAnalista = idAnalista
                    });
                }
            }

            // Meses impagos ya registrados que deben escalar (En Mora -> En Litigio). Un mes impago que figura
            // Al día fue regularizado por una acción legal "Resuelto": no vuelve a escalar.
            Dictionary<int, decimal> montoPorContrato = contratos.ToDictionary(c => c.IdContrato, c => c.MontoMensual);
            foreach (PagoAlquiler pago in pagos.Where(p => !p.Pagado && p.EstadoPago != EstadoPago.AlDia))
            {
                EstadoPago? estado = EstadoSegunFecha(pago.MesCorrespondiente, pago.AnioCorrespondiente, hoy);
                if (estado == null || estado.Value <= pago.EstadoPago) continue;

                pago.EstadoPago = estado.Value;
                if (pago.RecargoAplicado == 0 && montoPorContrato.TryGetValue(pago.IdContrato, out decimal monto))
                    pago.RecargoAplicado = CalcularRecargo(monto);
                cambios.Add(pago);
            }

            if (nuevos.Count == 0 && cambios.Count == 0) return (0, 0);

            if (!pagoDAL.GuardarLote(nuevos, cambios))
                throw new Exception("No se pudo actualizar la mora de los alquileres. No se guardó ningún cambio.");

            List<PagoAlquiler> afectados = nuevos.Concat(cambios).ToList();
            int enMora = afectados.Count(p => p.EstadoPago == EstadoPago.EnMora);
            int enLitigio = afectados.Count(p => p.EstadoPago == EstadoPago.EnLitigio);
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Actualización Mora",
                $"{enMora} período(s) pasaron a En Mora y {enLitigio} a En Litigio (al {hoy:dd/MM/yyyy}).");
            return (enMora, enLitigio);
        }

        // Días transcurridos desde el vencimiento del mes (día 10); 0 si todavía no venció
        public static int DiasEnMora(int mes, int anio, DateTime hoy) => Math.Max(0, (hoy.Date - FechaVencimiento(mes, anio)).Days);

        // Escalamiento manual a legal (además del automático del día 1 del mes siguiente): En Mora -> En Litigio.
        // Conserva el recargo. Solo para meses impagos En Mora.
        public void EscalarALitigio(int idPago)
        {
            PagoAlquiler pago = pagoDAL.BuscarPorId(idPago);
            if (pago == null)
                throw new Exception("El pago no existe.");
            if (pago.Pagado)
                throw new ArgumentException($"El período {pago} ya está pagado.");
            if (pago.EstadoPago != EstadoPago.EnMora)
                throw new ArgumentException($"Solo se puede escalar a legal un período En Mora (el {pago} está {pago.EstadoPago}).");

            if (!pagoDAL.ActualizarEstado(idPago, EstadoPago.EnLitigio, pago.RecargoAplicado))
                throw new Exception("No se pudo escalar el pago a legal.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Escalamiento Legal",
                $"Contrato N° {pago.IdContrato}, período {pago}: En Mora -> En Litigio (escalado manualmente, " +
                $"{DiasEnMora(pago.MesCorrespondiente, pago.AnioCorrespondiente, DateTime.Now)} días en mora).");
        }

        public void Eliminar(int idPago)
        {
            if (!pagoDAL.Eliminar(idPago))
                throw new Exception("No se pudo eliminar el pago.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Pago", $"Registro de pago {idPago} eliminado.");
        }

        public List<PagoAlquiler> LeerTodos() => pagoDAL.LeerTodos();

        public PagoAlquiler BuscarPorId(int idPago) => pagoDAL.BuscarPorId(idPago);

        public List<PagoAlquiler> LeerPorContrato(int idContrato) => pagoDAL.LeerPorContrato(idContrato);

        public PagoAlquiler BuscarPorPeriodo(int idContrato, int mes, int anio) => pagoDAL.BuscarPorPeriodo(idContrato, mes, anio);

        // Con contrato, inquilino e inmueble; estado null = todos (Gestionar Mora: EnMora, Gestionar Litigios: EnLitigio)
        public List<PagoAlquiler> LeerDetalle(EstadoPago? estado = null) => pagoDAL.LeerDetalle(estado);
    }
}
