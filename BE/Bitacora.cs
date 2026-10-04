using System;

namespace BE
{
    public class Bitacora
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public Usuario Usuario { get; set; }
        public string Actividad { get; set; }
        public string InfoAsociada { get; set; }

        public string NombreUsuario => Usuario != null ? Usuario.Nombre : "Sistema/Anónimo";
    }
}
