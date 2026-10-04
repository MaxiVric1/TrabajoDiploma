using System;

namespace BE
{
    public class UsuarioHistorial
    {
        public int IdHistorial { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string NombrePersona { get; set; }
        public string Apellido { get; set; }
        public string Password { get; set; }
        public bool Activo { get; set; }
        public long DVH { get; set; }
        public DateTime FechaCambio { get; set; }
        public int IdUsuarioAutor { get; set; }
        public string TipoOperacion { get; set; }
        public string EditorNombre { get; set; }
    }
}
