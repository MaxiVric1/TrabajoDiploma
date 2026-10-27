using System;
using System.Collections.Generic;

namespace BE
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } // Nombre de usuario (Login)
        public string NombrePersona { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }     // formato 12.345.678 (opcional, único)
        public string Password { get; set; }
        public bool Activo { get; set; }
        public int IntentosFallidos { get; set; }
        public DateTime? BloqueadoHasta { get; set; }
        public long DVH { get; set; }
        public int? IdIdioma { get; set; }
        public List<Rol> Roles { get; set; } = new List<Rol>();

        public override string ToString()
        {
            return Nombre;
        }
    }
}