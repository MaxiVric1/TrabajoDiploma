namespace BE
{
    // Los nombres deben coincidir con el CHECK de InmuebleAlquiler.tipo_inmueble en la base
    public enum TipoInmueble
    {
        Casa,
        Departamento,
        Local,
        Terreno,
        Oficina
    }

    public class InmuebleAlquiler
    {
        public int IdInmuebleAlquiler { get; set; }
        public string Direccion { get; set; }
        public TipoInmueble TipoInmueble { get; set; }

        public override string ToString()
        {
            return $"{TipoInmueble} - {Direccion}";
        }
    }
}
