using System;

namespace BE
{
    // Los nombres deben coincidir con el CHECK de Tasacion.estado_dictamen en la base
    public enum EstadoDictamen
    {
        Pendiente,
        Aprobada,
        Rechazada
    }

    public class Tasacion
    {
        public int NroTasacion { get; set; }
        public DateTime FechaDictamen { get; set; }
        public EstadoDictamen EstadoDictamen { get; set; } = EstadoDictamen.Pendiente;
        public string MotivoRechazo { get; set; }
        public decimal? ValorFinal { get; set; }
        public decimal? ValorSugerido { get; set; } // solo en dictámenes rechazados: valor alternativo para renegociar
        public int IdCaptacion { get; set; }
        public int IdUsuarioTasador { get; set; }

        public override string ToString()
        {
            return $"Tasación N° {NroTasacion}";
        }
    }
}
