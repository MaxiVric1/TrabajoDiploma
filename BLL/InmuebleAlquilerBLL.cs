using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class InmuebleAlquilerBLL
    {
        private InmuebleAlquilerDAL inmuebleDAL = new InmuebleAlquilerDAL();
        private ContratoDAL contratoDAL = new ContratoDAL();

        public int Registrar(InmuebleAlquiler inmueble)
        {
            Validar(inmueble);

            int result = inmuebleDAL.Crear(inmueble);
            if (result == -1)
                throw new Exception("No se pudo registrar el inmueble.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Inmueble Alquiler",
                $"Inmueble {result} registrado: {inmueble.TipoInmueble} - {inmueble.Direccion}.");
            return result;
        }

        public void Actualizar(InmuebleAlquiler inmueble)
        {
            Validar(inmueble);

            if (!inmuebleDAL.Actualizar(inmueble))
                throw new Exception("No se pudo actualizar el inmueble.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Inmueble Alquiler",
                $"Inmueble {inmueble.IdInmuebleAlquiler} actualizado.");
        }

        public void Eliminar(int idInmuebleAlquiler)
        {
            if (contratoDAL.LeerTodos().Any(c => c.IdInmuebleAlquiler == idInmuebleAlquiler))
                throw new Exception("No se puede eliminar el inmueble porque tiene contratos registrados.");

            if (!inmuebleDAL.Eliminar(idInmuebleAlquiler))
                throw new Exception("No se pudo eliminar el inmueble.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Inmueble Alquiler",
                $"Inmueble {idInmuebleAlquiler} eliminado.");
        }

        public List<InmuebleAlquiler> LeerTodos() => inmuebleDAL.LeerTodos();

        // Registros de alquiler sin contrato vigente
        public List<InmuebleAlquiler> LeerDisponibles() => inmuebleDAL.LeerDisponibles();

        // Inmuebles del PN1 de alquiler, Publicados y sin contrato vigente: los que se ofrecen en Registrar Contrato
        public List<Inmueble> LeerInmueblesParaAlquilar() => new InmuebleDAL().LeerDisponiblesParaAlquiler();

        public InmuebleAlquiler BuscarPorId(int idInmuebleAlquiler) => inmuebleDAL.BuscarPorId(idInmuebleAlquiler);

        // Registro de alquiler de un inmueble del PN1 (null si todavía no tuvo contratos)
        public InmuebleAlquiler BuscarPorInmueble(int idInmueble) => inmuebleDAL.BuscarPorInmueble(idInmueble);

        private void Validar(InmuebleAlquiler inmueble)
        {
            if (inmueble == null) throw new ArgumentNullException(nameof(inmueble));
            if (string.IsNullOrWhiteSpace(inmueble.Direccion))
                throw new ArgumentException("La dirección es obligatoria.");
            if (!Enum.IsDefined(typeof(TipoInmueble), inmueble.TipoInmueble))
                throw new ArgumentException("El tipo de inmueble no es válido.");

            inmueble.Direccion = inmueble.Direccion.Trim();
        }
    }
}
