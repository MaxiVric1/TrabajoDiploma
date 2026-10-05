using System;

namespace BE
{
    // Intento de contacto con el inquilino por pagos adeudados (Gestionar Mora). Se vincula al contrato.
    public class ContactoCobranza
    {
        public int IdContacto { get; set; }
        public DateTime FechaContacto { get; set; }
        public string Descripcion { get; set; }
        public int IdContrato { get; set; }
        public int IdUsuarioAnalista { get; set; }

        // Nombre de usuario del analista; lo carga LeerContactosPorContrato
        public string Analista { get; set; }

        public override string ToString()
        {
            return $"{FechaContacto:dd/MM/yyyy HH:mm} - {Descripcion}";
        }
    }
}
