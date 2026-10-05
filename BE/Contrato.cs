using System;

namespace BE
{
    // En la base se guardan como 'Activo', 'Por Vencer' y 'Finalizado' (la conversión está en ContratoDAL)
    public enum EstadoContrato
    {
        Activo,
        PorVencer,
        Finalizado
    }

    public class Contrato
    {
        public int IdContrato { get; set; }
        public DateTime FechaInicio { get; set; }
        public int DuracionMeses { get; set; }
        public decimal MontoMensual { get; set; }
        public EstadoContrato EstadoContrato { get; set; } = EstadoContrato.Activo;
        public bool AlertaVencimiento { get; set; }
        public int IdInquilino { get; set; }
        public int IdInmuebleAlquiler { get; set; }
        public int IdUsuarioAgente { get; set; }

        // Cargados por las consultas con detalle (LeerContratosDetalle / LeerContratosPorVencer); null en las demás
        public Inquilino Inquilino { get; set; }
        public InmuebleAlquiler InmuebleAlquiler { get; set; }

        // No se guarda en la base: mismo cálculo que DATEADD(MONTH, duracion_meses, fecha_inicio) en los SPs
        public DateTime FechaFin => FechaInicio.AddMonths(DuracionMeses);

        public bool Vigente => EstadoContrato != EstadoContrato.Finalizado;

        public override string ToString()
        {
            return $"Contrato N° {IdContrato}";
        }
    }
}
