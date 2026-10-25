using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class InquilinoBLL
    {
        private InquilinoDAL inquilinoDAL = new InquilinoDAL();
        private ContratoDAL contratoDAL = new ContratoDAL();

        public int Registrar(Inquilino inquilino)
        {
            Validar(inquilino);

            if (inquilinoDAL.BuscarPorDni(inquilino.Dni) != null)
                throw new Exception($"Ya existe un inquilino con DNI {inquilino.Dni}.");

            int result = inquilinoDAL.Crear(inquilino);
            if (result == -1)
                throw new Exception("No se pudo registrar el inquilino.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Inquilino", $"Inquilino DNI {inquilino.Dni} registrado.");
            return result;
        }

        public void Actualizar(Inquilino inquilino)
        {
            Validar(inquilino);

            Inquilino otro = inquilinoDAL.BuscarPorDni(inquilino.Dni);
            if (otro != null && otro.IdInquilino != inquilino.IdInquilino)
                throw new Exception($"Ya existe otro inquilino con DNI {inquilino.Dni}.");

            if (!inquilinoDAL.Actualizar(inquilino))
                throw new Exception("No se pudo actualizar el inquilino.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Inquilino", $"Inquilino DNI {inquilino.Dni} actualizado.");
        }

        public void Eliminar(int idInquilino)
        {
            if (contratoDAL.LeerTodos().Any(c => c.IdInquilino == idInquilino))
                throw new Exception("No se puede eliminar el inquilino porque tiene contratos registrados.");

            if (!inquilinoDAL.Eliminar(idInquilino))
                throw new Exception("No se pudo eliminar el inquilino.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Inquilino", $"Inquilino {idInquilino} eliminado.");
        }

        public List<Inquilino> LeerTodos() => inquilinoDAL.LeerTodos();

        public Inquilino BuscarPorId(int idInquilino) => inquilinoDAL.BuscarPorId(idInquilino);

        public Inquilino BuscarPorDni(string dni) => inquilinoDAL.BuscarPorDni(dni?.Trim());

        internal void Validar(Inquilino inquilino)
        {
            if (inquilino == null) throw new ArgumentNullException(nameof(inquilino));
            if (string.IsNullOrWhiteSpace(inquilino.Dni))
                throw new ArgumentException("El DNI es obligatorio.");
            if (string.IsNullOrWhiteSpace(inquilino.Nombre) || string.IsNullOrWhiteSpace(inquilino.Apellido))
                throw new ArgumentException("El nombre y el apellido son obligatorios.");
            if (!string.IsNullOrWhiteSpace(inquilino.Email) && !inquilino.Email.Contains("@"))
                throw new ArgumentException("El email no tiene un formato válido.");

            inquilino.Dni = inquilino.Dni.Trim();
            inquilino.Nombre = inquilino.Nombre.Trim();
            inquilino.Apellido = inquilino.Apellido.Trim();
            inquilino.Telefono = string.IsNullOrWhiteSpace(inquilino.Telefono) ? null : inquilino.Telefono.Trim();
            inquilino.Email = string.IsNullOrWhiteSpace(inquilino.Email) ? null : inquilino.Email.Trim();
            inquilino.Domicilio = string.IsNullOrWhiteSpace(inquilino.Domicilio) ? null : inquilino.Domicilio.Trim();
            if (inquilino.Domicilio != null && inquilino.Domicilio.Length > 200)
                throw new ArgumentException("El domicilio no puede superar los 200 caracteres.");
        }
    }
}
