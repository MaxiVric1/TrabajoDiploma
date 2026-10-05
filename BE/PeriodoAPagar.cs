namespace BE
{
    // Mes de un contrato que todavía no tiene pago registrado (para Registrar Pago)
    public class PeriodoAPagar
    {
        public int Mes { get; set; }
        public int Anio { get; set; }

        // null = Pendiente (todavía no venció: hasta el día 10); si no, En Mora o En Litigio
        public EstadoPago? Estado { get; set; }

        // Mes impago que figura Al día porque lo regularizó una acción legal "Resuelto"
        public bool Regularizado { get; set; }

        public override string ToString()
        {
            return $"{Mes:00}/{Anio}";
        }
    }
}
