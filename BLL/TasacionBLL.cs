using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class TasacionBLL
    {
        private TasacionDAL tasacionDAL = new TasacionDAL();
        private CaptacionDAL captacionDAL = new CaptacionDAL();
        private InmuebleDAL inmuebleDAL = new InmuebleDAL();

        public int Registrar(Tasacion tasacion)
        {
            // Si no se indica tasador, la registra el usuario logueado
            if (tasacion != null && tasacion.IdUsuarioTasador == 0 && SessionManager.Instance.EstaLogueado())
                tasacion.IdUsuarioTasador = SessionManager.Instance.UsuarioActual.Id;

            Captacion captacion = Validar(tasacion);

            // Un inmueble rechazado solo vuelve a tasarse después de que el agente renegocia (y pasa a Captado)
            Inmueble inmueble = inmuebleDAL.BuscarPorId(captacion.IdInmueble);
            if (inmueble != null && inmueble.EstadoActual == EstadoInmueble.Rechazado)
                throw new Exception("La captación debe ser renegociada por el Agente Comercial antes de poder ser tasada nuevamente.");
            // Solo se tasa lo pendiente: evita que una tasación nueva devuelva un inmueble Tasado/Publicado a otro estado
            if (inmueble != null && inmueble.EstadoActual != EstadoInmueble.Captado)
                throw new Exception($"Solo se pueden tasar captaciones en estado Captado. Estado actual: {inmueble.EstadoActual}.");

            int result = tasacionDAL.RegistrarConDictamen(tasacion, captacion.IdInmueble, EstadoSegunDictamen(tasacion.EstadoDictamen));
            if (result == -1)
                throw new Exception("No se pudo registrar la tasación. No se guardó ningún cambio.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Tasación", $"Tasación N° {result} ({tasacion.EstadoDictamen}) para la captación N° {tasacion.IdCaptacion}.");
            return result;
        }

        public void Actualizar(Tasacion tasacion)
        {
            Captacion captacion = Validar(tasacion);

            if (!tasacionDAL.ActualizarConDictamen(tasacion, captacion.IdInmueble, EstadoSegunDictamen(tasacion.EstadoDictamen)))
                throw new Exception("No se pudo actualizar la tasación. No se guardó ningún cambio.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Tasación", $"Tasación N° {tasacion.NroTasacion} actualizada a {tasacion.EstadoDictamen}.");
        }

        public void Eliminar(int nroTasacion)
        {
            if (!tasacionDAL.Eliminar(nroTasacion))
                throw new Exception("No se pudo eliminar la tasación.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Tasación", $"Tasación N° {nroTasacion} eliminada.");
        }

        public List<Tasacion> LeerTodos() => tasacionDAL.LeerTodos();

        public List<Tasacion> LeerPorCaptacion(int nroCaptacion) => tasacionDAL.LeerPorCaptacion(nroCaptacion);

        // Dictamen vigente de una captación (el más reciente); null si todavía no fue tasada.
        // Si está rechazado, ValorSugerido es lo que el agente le propone al cliente.
        public Tasacion ObtenerUltimoDictamen(int nroCaptacion)
        {
            return tasacionDAL.LeerPorCaptacion(nroCaptacion)
                .OrderByDescending(t => t.FechaDictamen)
                .ThenByDescending(t => t.NroTasacion)
                .FirstOrDefault();
        }

        public Tasacion BuscarPorNro(int nroTasacion) => tasacionDAL.BuscarPorNro(nroTasacion);

        // El dictamen define el estado del inmueble: Aprobada -> Tasado, Rechazada -> Rechazado, Pendiente -> sin cambio
        private EstadoInmueble? EstadoSegunDictamen(EstadoDictamen dictamen)
        {
            switch (dictamen)
            {
                case EstadoDictamen.Aprobada: return EstadoInmueble.Tasado;
                case EstadoDictamen.Rechazada: return EstadoInmueble.Rechazado;
                default: return null;
            }
        }

        private Captacion Validar(Tasacion tasacion)
        {
            if (tasacion == null) throw new ArgumentNullException(nameof(tasacion));
            if (tasacion.IdUsuarioTasador <= 0)
                throw new ArgumentException("La tasación debe tener un tasador asignado.");

            Captacion captacion = captacionDAL.BuscarPorNro(tasacion.IdCaptacion);
            if (captacion == null)
                throw new ArgumentException("La captación indicada no existe.");

            switch (tasacion.EstadoDictamen)
            {
                case EstadoDictamen.Aprobada:
                    if (!tasacion.ValorFinal.HasValue || tasacion.ValorFinal.Value <= 0)
                        throw new ArgumentException("Una tasación aprobada debe tener un valor final mayor a cero.");
                    tasacion.MotivoRechazo = null;
                    tasacion.ValorSugerido = null;
                    break;
                case EstadoDictamen.Rechazada:
                    if (string.IsNullOrWhiteSpace(tasacion.MotivoRechazo))
                        throw new ArgumentException("Una tasación rechazada debe indicar el motivo.");
                    if (tasacion.ValorSugerido.HasValue && tasacion.ValorSugerido.Value <= 0)
                        throw new ArgumentException("El valor sugerido debe ser mayor a cero.");
                    tasacion.MotivoRechazo = tasacion.MotivoRechazo.Trim();
                    break;
                default:
                    tasacion.MotivoRechazo = null;
                    tasacion.ValorSugerido = null;
                    break;
            }

            if (tasacion.ValorFinal.HasValue && tasacion.ValorFinal.Value <= 0)
                tasacion.ValorFinal = null;

            return captacion;
        }
    }
}
