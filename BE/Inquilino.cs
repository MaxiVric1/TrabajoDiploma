namespace BE
{
    public class Inquilino
    {
        public int IdInquilino { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        // Las consultas con detalle de contratos y pagos traen el nombre ya concatenado en Nombre (Apellido vacío)
        public string NombreCompleto => string.IsNullOrEmpty(Apellido) ? Nombre : $"{Nombre} {Apellido}";

        public override string ToString()
        {
            return $"{NombreCompleto} ({Dni})";
        }
    }
}
