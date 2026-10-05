using System;
using System.Collections.Generic;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class ContactoCobranzaBLL
    {
        private ContactoCobranzaDAL contactoDAL = new ContactoCobranzaDAL();
        private ContratoDAL contratoDAL = new ContratoDAL();

        public int Registrar(ContactoCobranza contacto)
        {
            // Si no se indica analista, lo registra el usuario logueado
            if (contacto != null && contacto.IdUsuarioAnalista == 0 && SessionManager.Instance.EstaLogueado())
                contacto.IdUsuarioAnalista = SessionManager.Instance.UsuarioActual.Id;

            Validar(contacto);
            Contrato contrato = contratoDAL.BuscarPorId(contacto.IdContrato);
            if (contrato == null)
                throw new ArgumentException("El contrato indicado no existe.");
            if (contacto.FechaContacto.Date < contrato.FechaInicio.Date)
                throw new ArgumentException($"La fecha del contacto no puede ser anterior al inicio del contrato ({contrato.FechaInicio:dd/MM/yyyy}).");

            int result = contactoDAL.Crear(contacto);
            if (result == -1)
                throw new Exception("No se pudo registrar el contacto.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Contacto Cobranza",
                $"Contacto con el inquilino del contrato N° {contacto.IdContrato} ({contacto.FechaContacto:dd/MM/yyyy}).");
            return result;
        }

        public void Actualizar(ContactoCobranza contacto)
        {
            Validar(contacto);

            if (!contactoDAL.Actualizar(contacto))
                throw new Exception("No se pudo actualizar el contacto.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Contacto Cobranza",
                $"Contacto {contacto.IdContacto} actualizado.");
        }

        public void Eliminar(int idContacto)
        {
            if (!contactoDAL.Eliminar(idContacto))
                throw new Exception("No se pudo eliminar el contacto.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Contacto Cobranza", $"Contacto {idContacto} eliminado.");
        }

        public List<ContactoCobranza> LeerTodos() => contactoDAL.LeerTodos();

        public ContactoCobranza BuscarPorId(int idContacto) => contactoDAL.BuscarPorId(idContacto);

        public List<ContactoCobranza> LeerPorContrato(int idContrato) => contactoDAL.LeerPorContrato(idContrato);

        private void Validar(ContactoCobranza contacto)
        {
            if (contacto == null) throw new ArgumentNullException(nameof(contacto));
            if (string.IsNullOrWhiteSpace(contacto.Descripcion))
                throw new ArgumentException("Describa lo que se habló con el inquilino.");
            if (contacto.Descripcion.Trim().Length > 1000)
                throw new ArgumentException("La descripción no puede superar los 1000 caracteres.");
            if (contacto.FechaContacto.Date > DateTime.Now.Date)
                throw new ArgumentException("La fecha del contacto no puede ser futura.");
            if (contacto.IdUsuarioAnalista <= 0)
                throw new ArgumentException("El contacto debe tener un analista asignado.");

            contacto.Descripcion = contacto.Descripcion.Trim();
        }
    }
}
