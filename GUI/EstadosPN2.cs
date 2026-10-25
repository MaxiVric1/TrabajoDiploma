using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;

namespace TpIngSoft
{
    // Presentación de los estados del PN2, compartida por sus formularios
    internal static class EstadosPN2
    {
        public static readonly Color FONDO_POR_VENCER = Color.FromArgb(214, 180, 60);   // amarillo
        public static readonly Color FONDO_CON_DEUDA = Color.FromArgb(170, 50, 60);     // rojo

        public static string Texto(EstadoContrato estado)
        {
            return estado == EstadoContrato.PorVencer ? "Por Vencer" : estado.ToString();
        }

        public static string Texto(EstadoPago estado)
        {
            switch (estado)
            {
                case EstadoPago.AlDia: return "Al día";
                case EstadoPago.EnMora: return "En Mora";
                default: return "En Litigio";
            }
        }

        // Procesos automáticos que ejecutan los formularios del PN2 al abrirse: primero las actualizaciones por IPC
        // (definen el monto de los meses que entran en mora), después la mora (un contrato recién vencido todavía
        // genera sus últimos meses impagos) y por último los vencimientos de contratos.
        // Devuelve el resumen para mostrar; si falla, avisa con un MessageBox y devuelve el aviso.
        public static string EjecutarProcesosAutomaticos()
        {
            string ipc = ActualizarPorIpc();
            try
            {
                var (enMora, enLitigio) = new BLL.PagoAlquilerBLL().ActualizarEstadosAutomaticos();
                int contratos = new BLL.ContratoBLL().ActualizarVencimientos();

                return (enMora + enLitigio + contratos == 0
                    ? $"Estados actualizados al {System.DateTime.Now:dd/MM/yyyy HH:mm}: sin cambios."
                    : $"Estados actualizados al {System.DateTime.Now:dd/MM/yyyy HH:mm}: {enMora} mes(es) pasaron a En Mora, " +
                      $"{enLitigio} a En Litigio y {contratos} contrato(s) cambiaron por vencimiento.") + ipc;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("No se pudieron actualizar los estados de pagos y contratos: " + ex.Message,
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "No se pudieron actualizar los estados automáticamente.";
            }
        }

        // Actualizaciones por IPC vencidas (datos del BCRA). Sin conexión o sin IPC publicado quedan pendientes para la
        // próxima vez: no frenan el resto de los procesos. Devuelve el texto que se agrega al resumen.
        private static string ActualizarPorIpc()
        {
            try
            {
                BLL.ResultadoActualizacionIpc resultado = new BLL.ActualizacionIpcBLL().AplicarPendientes();
                string texto = resultado.Aplicadas.Count > 0 ? $" {resultado.Aplicadas.Count} monto(s) actualizado(s) por IPC." : "";
                if (resultado.Pendientes.Count > 0)
                    texto += $" {resultado.Pendientes.Count} actualización(es) por IPC pendiente(s): {resultado.Pendientes[0]}";
                return texto;
            }
            catch (System.Exception ex)
            {
                return " No se pudieron aplicar las actualizaciones por IPC: " + ex.Message;
            }
        }

        // Estado del pago del mes en curso, con las mismas reglas que PagoAlquilerBLL. Versión corta para grillas.
        public static string EstadoMesActual(Contrato contrato, List<PagoAlquiler> pagos, System.DateTime hoy)
        {
            PagoAlquiler pago = pagos.FirstOrDefault(p => p.MesCorrespondiente == hoy.Month && p.AnioCorrespondiente == hoy.Year);
            if (pago != null)
                return pago.Pagado ? "Pagado" : Texto(pago.EstadoPago);
            if (!PagoAlquilerBLL.PeriodosDelContrato(contrato).Contains((hoy.Month, hoy.Year)))
                return "No corresponde";

            EstadoPago? estado = PagoAlquilerBLL.EstadoSegunFecha(hoy.Month, hoy.Year, hoy);
            return estado == null ? "Pendiente" : Texto(estado.Value);
        }

        // Versión completa para el detalle
        public static string DescribirMesActual(Contrato contrato, List<PagoAlquiler> pagos, System.DateTime hoy)
        {
            string periodo = $"{hoy.Month:00}/{hoy.Year}";
            PagoAlquiler pago = pagos.FirstOrDefault(p => p.MesCorrespondiente == hoy.Month && p.AnioCorrespondiente == hoy.Year);
            if (pago != null)
            {
                if (pago.Pagado)
                    return $"{periodo}: Al día — pagado el {pago.FechaPago:dd/MM/yyyy} (${pago.MontoPagado:N2})";
                return $"{periodo}: {Texto(pago.EstadoPago)} — sin pago (recargo ${pago.RecargoAplicado:N2})";
            }

            if (!PagoAlquilerBLL.PeriodosDelContrato(contrato).Contains((hoy.Month, hoy.Year)))
                return $"{periodo}: no corresponde al período del contrato";

            EstadoPago? estado = PagoAlquilerBLL.EstadoSegunFecha(hoy.Month, hoy.Year, hoy);
            return estado == null
                ? $"{periodo}: pendiente — vence el {PagoAlquilerBLL.FechaVencimiento(hoy.Month, hoy.Year):dd/MM/yyyy}"
                : $"{periodo}: {Texto(estado.Value)} — sin pago";
        }

        // Colorea una fila de grilla: rojo (deuda) tiene prioridad sobre amarillo (por vencer)
        public static void Resaltar(DataGridViewRow fila, bool conDeuda, bool porVencer)
        {
            if (conDeuda)
            {
                fila.DefaultCellStyle.BackColor = FONDO_CON_DEUDA;
                fila.DefaultCellStyle.ForeColor = Color.White;
                fila.DefaultCellStyle.SelectionBackColor = ControlPaint.Light(FONDO_CON_DEUDA);
                fila.DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else if (porVencer)
            {
                fila.DefaultCellStyle.BackColor = FONDO_POR_VENCER;
                fila.DefaultCellStyle.ForeColor = Color.Black;
                fila.DefaultCellStyle.SelectionBackColor = ControlPaint.Light(FONDO_POR_VENCER);
                fila.DefaultCellStyle.SelectionForeColor = Color.Black;
            }
        }
    }
}
