namespace BE
{
    // Garante de un contrato de alquiler: respalda la garantía con un inmueble propio (PropiedadGarantia)
    public class Garante
    {
        public int IdGarante { get; set; }
        public int IdContrato { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Domicilio { get; set; }
        public string PropiedadGarantia { get; set; } // dirección / datos registrales del inmueble en garantía

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public override string ToString()
        {
            return $"{NombreCompleto} ({Dni})";
        }
    }
}
