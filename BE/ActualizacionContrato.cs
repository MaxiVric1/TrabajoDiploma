using System;

namespace BE
{
    // Una actualización del monto mensual de un contrato por índice (hoy IPC). Rige desde el período que empieza
    // en FechaActualizacion; los meses anteriores conservan MontoAnterior.
    public class ActualizacionContrato
    {
        public int IdActualizacion { get; set; }
        public int IdContrato { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Indice { get; set; } = "IPC";
        public decimal Porcentaje { get; set; }   // variación acumulada aplicada, en % (ej. 5,8 = 5,8%)
        public string Detalle { get; set; }       // valores mensuales usados
        public decimal MontoAnterior { get; set; }
        public decimal MontoNuevo { get; set; }
        public int? IdUsuario { get; set; }

        public override string ToString()
        {
            return $"{FechaActualizacion:dd/MM/yyyy}: {Indice} {Porcentaje:0.##}% (${MontoAnterior:N2} -> ${MontoNuevo:N2})";
        }
    }
}
