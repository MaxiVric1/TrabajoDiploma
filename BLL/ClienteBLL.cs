using System;
using System.Collections.Generic;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class ClienteBLL
    {
        private ClienteDAL clienteDAL = new ClienteDAL();
        private InmuebleDAL inmuebleDAL = new InmuebleDAL();

        public int Registrar(Cliente cliente)
        {
            Validar(cliente);

            if (clienteDAL.BuscarPorDni(cliente.Dni) != null)
                throw new Exception($"Ya existe un cliente con DNI {cliente.Dni}.");

            int result = clienteDAL.Crear(cliente);
            if (result == -1)
                throw new Exception("No se pudo registrar el cliente.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Cliente", $"Cliente DNI {cliente.Dni} registrado.");
            return result;
        }

        public void Actualizar(Cliente cliente)
        {
            Validar(cliente);

            Cliente otro = clienteDAL.BuscarPorDni(cliente.Dni);
            if (otro != null && otro.IdCliente != cliente.IdCliente)
                throw new Exception($"Ya existe otro cliente con DNI {cliente.Dni}.");

            if (!clienteDAL.Actualizar(cliente))
                throw new Exception("No se pudo actualizar el cliente.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Cliente", $"Cliente DNI {cliente.Dni} actualizado.");
        }

        public void Eliminar(int idCliente)
        {
            if (inmuebleDAL.LeerPorCliente(idCliente).Count > 0)
                throw new Exception("No se puede eliminar el cliente porque tiene inmuebles asociados.");

            if (!clienteDAL.Eliminar(idCliente))
                throw new Exception("No se pudo eliminar el cliente.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Cliente", $"Cliente {idCliente} eliminado.");
        }

        public List<Cliente> LeerTodos() => clienteDAL.LeerTodos();

        public Cliente BuscarPorId(int idCliente) => clienteDAL.BuscarPorId(idCliente);

        public Cliente BuscarPorDni(string dni) => clienteDAL.BuscarPorDni(dni?.Trim());

        internal void Validar(Cliente cliente)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (string.IsNullOrWhiteSpace(cliente.Dni))
                throw new ArgumentException("El DNI es obligatorio.");
            if (string.IsNullOrWhiteSpace(cliente.Nombre) || string.IsNullOrWhiteSpace(cliente.Apellido))
                throw new ArgumentException("El nombre y el apellido son obligatorios.");
            if (!string.IsNullOrWhiteSpace(cliente.Email) && !cliente.Email.Contains("@"))
                throw new ArgumentException("El email no tiene un formato válido.");

            cliente.Dni = cliente.Dni.Trim();
            cliente.Nombre = cliente.Nombre.Trim();
            cliente.Apellido = cliente.Apellido.Trim();
            cliente.Telefono = string.IsNullOrWhiteSpace(cliente.Telefono) ? null : cliente.Telefono.Trim();
            cliente.Email = string.IsNullOrWhiteSpace(cliente.Email) ? null : cliente.Email.Trim();
        }
    }
}
