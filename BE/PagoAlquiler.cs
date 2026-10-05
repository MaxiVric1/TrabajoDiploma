using System;

namespace BE
{
    // En la base se guardan como 'Al día', 'En Mora' y 'En Litigio' (la conversión está en PagoAlquilerDAL).
    // El orden importa: PagoAlquilerBLL escala los meses impagos comparando valores (AlDia < EnMora < EnLitigio).
    public enum EstadoPago
    {
        AlDia,
        EnMora,
        EnLitigio
    }

    // Un registro por contrato y mes. Mientras el mes no se paga: FechaPago null y MontoPagado 0.
    public class PagoAlquiler
    {
        public int IdPago { get; set; }
        public int MesCorrespondiente { get; set; }
        public int AnioCorrespondiente { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal MontoPagado { get; set; }
        public EstadoPago EstadoPago { get; set; } = EstadoPago.AlDia;
        public decimal RecargoAplicado { get; set; } // monto en pesos
        public int IdContrato { get; set; }
        public int IdUsuarioAnalista { get; set; }

        // Cargado por LeerPagosDetalle (con su Inquilino e InmuebleAlquiler); null en las demás consultas
        public Contrato Contrato { get; set; }

        public bool Pagado => FechaPago.HasValue;

        public override string ToString()
        {
            return $"{MesCorrespondiente:00}/{AnioCorrespondiente}";
        }
    }
}
