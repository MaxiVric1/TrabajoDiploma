using System;
using System.Collections.Generic;

namespace BE
{
    // En la base se guardan como 'Activo', 'Por Vencer' y 'Finalizado' (la conversión está en ContratoDAL)
    public enum EstadoContrato
    {
        Activo,
        PorVencer,
        Finalizado
    }

    // Los nombres deben coincidir con el CHECK de Contrato.metodo_pago en la base
    public enum MetodoPago
    {
        Transferencia,
        Efectivo,
        Cheque
    }

    // Quién inició una rescisión anticipada (CHECK de Contrato.rescision_iniciada_por)
    public enum IniciadorRescision
    {
        Inquilino,
        Propietario
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

        // El inquilino avisó que desocupa al vencimiento; el contrato sigue vigente hasta entonces
        public bool AvisoDesocupacion { get; set; }
        public DateTime? FechaAvisoDesocupacion { get; set; }

        // Condiciones económicas
        public decimal DepositoGarantia { get; set; }
        public decimal ComisionInmobiliaria { get; set; } // PORCENTAJE (ej. 4 = 4%) que la inmobiliaria retiene de cada pago
        public MetodoPago MetodoPago { get; set; } = MetodoPago.Transferencia;

        // Actualización del monto mensual por índice (hoy siempre IPC cada 3 meses)
        public string IndiceActualizacion { get; set; } = "IPC";
        public int PeriodicidadActualizacionMeses { get; set; } = 3;

        // Rescisión anticipada (solo en contratos finalizados antes de su vencimiento)
        public IniciadorRescision? RescisionIniciadaPor { get; set; }
        public decimal? PenalidadRescision { get; set; }

        // Entrega del inmueble al finalizar: estado en que se recibió y destino del depósito
        public string EstadoEntrega { get; set; }
        public decimal? MontoDescuentoDeposito { get; set; }
        public decimal? MontoDevolucionDeposito { get; set; }

        // Garantes: se guardan junto con el contrato al registrarlo (GaranteBLL para leerlos o modificarlos después)
        public List<Garante> Garantes { get; set; } = new List<Garante>();

        // Cargados por las consultas con detalle (LeerContratosDetalle / LeerContratosPorVencer); null en las demás
        public Inquilino Inquilino { get; set; }
        public InmuebleAlquiler InmuebleAlquiler { get; set; }

        // La base la guarda calculada (columna fecha_fin = DATEADD(MONTH, duracion_meses, fecha_inicio)); acá se
        // calcula igual para que siga siendo correcta mientras se edita el contrato
        public DateTime FechaFin => FechaInicio.AddMonths(DuracionMeses);

        public bool Vigente => EstadoContrato != EstadoContrato.Finalizado;

        public override string ToString()
        {
            return $"Contrato N° {IdContrato}";
        }
    }
}
