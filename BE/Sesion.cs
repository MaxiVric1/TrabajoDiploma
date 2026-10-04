using System;

namespace BE
{
    public class Sesion
    {
        public int Id { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime FechaLogin { get; set; }
        public DateTime? FechaLogout { get; set; }
        public DateTime? FechaExpiracion { get; set; }
    }
}
