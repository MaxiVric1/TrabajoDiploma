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

        // Sin contrato vigente: los que se pueden ofrecer en Registrar Contrato
        public List<InmuebleAlquiler> LeerDisponibles() => inmuebleDAL.LeerDisponibles();

        public InmuebleAlquiler BuscarPorId(int idInmuebleAlquiler) => inmuebleDAL.BuscarPorId(idInmuebleAlquiler);

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
