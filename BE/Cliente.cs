namespace BE
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{Apellido}, {Nombre} ({Dni})";
        }
    }
}
