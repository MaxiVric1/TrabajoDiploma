using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class AccionLegalBLL
    {
        private AccionLegalDAL accionDAL = new AccionLegalDAL();
        private ContratoDAL contratoDAL = new ContratoDAL();
        private PagoAlquilerDAL pagoDAL = new PagoAlquilerDAL();

        // Registra la acción y aplica sus efectos en la misma transacción:
        // - Intimacion: sin efectos sobre estados.
        // - Resuelto: los pagos En Litigio del contrato pasan a Al día (conservan el recargo) y el contrato vuelve
        //   a estar vigente según su vencimiento (Activo, o Por Vencer si faltan 60 días o menos). Un contrato ya
        //   Finalizado (vencido o desalojado) no se reabre: solo se regularizan los pagos.
        // - Desalojo: el contrato pasa a Finalizado (sin alerta) y el inmueble queda libre.
        public int Registrar(AccionLegal accion)
        {
            // Si no se indica usuario del área legal, la registra el usuario logueado
            if (accion != null && accion.IdUsuarioLegal == 0 && SessionManager.Instance.EstaLogueado())
                accion.IdUsuarioLegal = SessionManager.Instance.UsuarioActual.Id;

            Validar(accion);
            Contrato contrato = contratoDAL.BuscarPorId(accion.IdContrato);
            if (contrato == null)
                throw new ArgumentException("El contrato indicado no existe.");

            List<PagoAlquiler> pagosAfectados = new List<PagoAlquiler>();
            EstadoContrato? nuevoEstado = null;
            string efecto = "";

            switch (accion.TipoAccion)
            {
                case TipoAccion.Resuelto:
                    pagosAfectados = pagoDAL.LeerPorContrato(contrato.IdContrato)
                        .Where(p => p.EstadoPago == EstadoPago.EnLitigio).ToList();
                    if (pagosAfectados.Count == 0)
                        throw new ArgumentException($"El contrato N° {contrato.IdContrato} no tiene pagos En Litigio para resolver.");
                    pagosAfectados.ForEach(p => p.EstadoPago = EstadoPago.AlDia);
                    efecto = $"{pagosAfectados.Count} pago(s) En Litigio pasan a Al día";

                    if (contrato.Vigente)
                    {
                        nuevoEstado = ContratoBLL.EstadoSegunVencimiento(contrato.FechaFin, DateTime.Now);
                        if (nuevoEstado == EstadoContrato.Finalizado) nuevoEstado = null; // lo finaliza el proceso de vencimientos
                        if (nuevoEstado.HasValue) efecto += $"; contrato {nuevoEstado}";
                    }
                    break;

                case TipoAccion.Desalojo:
                    if (!contrato.Vigente)
                        throw new ArgumentException($"El contrato N° {contrato.IdContrato} ya está finalizado.");
                    nuevoEstado = EstadoContrato.Finalizado;
                    efecto = "contrato Finalizado";
                    break;
            }

            bool ok = accion.TipoAccion == TipoAccion.Intimacion
                ? accionDAL.Crear(accion) != -1
                : accionDAL.CrearConEfectos(accion, pagosAfectados, nuevoEstado, ContratoBLL.AlertaSegunEstado(nuevoEstado ?? contrato.EstadoContrato));
            if (!ok)
                throw new Exception("No se pudo registrar la acción legal. No se guardó ningún cambio.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Acción Legal",
                $"{accion.TipoAccion} sobre el contrato N° {accion.IdContrato}" + (efecto != "" ? $": {efecto}." : "."));
            return accion.IdAccion;
        }

        // Solo se corrige la descripción: cambiar el tipo no aplicaría (ni revertiría) los efectos sobre los estados
        public void Actualizar(AccionLegal accion)
        {
            Validar(accion);
            AccionLegal actual = accionDAL.BuscarPorId(accion.IdAccion);
            if (actual == null)
                throw new Exception("La acción legal no existe.");
            if (actual.TipoAccion != accion.TipoAccion)
                throw new ArgumentException("El tipo de una acción registrada no se puede cambiar: registre una nueva acción.");

            if (!accionDAL.Actualizar(accion))
                throw new Exception("No se pudo actualizar la acción legal.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Acción Legal",
                $"Acción legal {accion.IdAccion} actualizada ({accion.TipoAccion}).");
        }

        public void Eliminar(int idAccion)
        {
            AccionLegal accion = accionDAL.BuscarPorId(idAccion);
            if (accion != null && accion.TipoAccion != TipoAccion.Intimacion)
                throw new Exception($"No se puede eliminar una acción {accion.TipoAccion}: sus efectos sobre el contrato y los pagos ya se aplicaron.");

            if (!accionDAL.Eliminar(idAccion))
                throw new Exception("No se pudo eliminar la acción legal.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Acción Legal", $"Acción legal {idAccion} eliminada.");
        }

        public List<AccionLegal> LeerTodos() => accionDAL.LeerTodos();

        public AccionLegal BuscarPorId(int idAccion) => accionDAL.BuscarPorId(idAccion);

        // Historial legal de un contrato, de la más reciente a la más antigua
        public List<AccionLegal> LeerPorContrato(int idContrato) => accionDAL.LeerPorContrato(idContrato);

        private void Validar(AccionLegal accion)
        {
            if (accion == null) throw new ArgumentNullException(nameof(accion));
            if (string.IsNullOrWhiteSpace(accion.Descripcion))
                throw new ArgumentException("La descripción de la acción legal es obligatoria.");
            if (accion.Descripcion.Trim().Length > 1000)
                throw new ArgumentException("La descripción no puede superar los 1000 caracteres.");
            if (!Enum.IsDefined(typeof(TipoAccion), accion.TipoAccion))
                throw new ArgumentException("El tipo de acción no es válido.");
            if (accion.IdUsuarioLegal <= 0)
                throw new ArgumentException("La acción debe tener un usuario del área legal asignado.");

            accion.Descripcion = accion.Descripcion.Trim();
        }
    }
}
