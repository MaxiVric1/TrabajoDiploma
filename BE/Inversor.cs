namespace BE
{
    public class Inversor
    {
        public int IdInversor { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cuit { get; set; }      // con guiones: 20-12345678-3
        public string Email { get; set; }
        public string Telefono { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public override string ToString()
        {
            return $"{Apellido}, {Nombre} (CUIT {Cuit})";
        }
    }
}
