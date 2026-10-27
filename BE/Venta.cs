using System;
using System.Collections.Generic;

namespace BE
{
    // Venta de un inmueble publicado en venta: precio en dólares y el agente de la captación, a quien se le cuenta
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdInmueble { get; set; }
        public int IdUsuarioAgente { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal PrecioUsd { get; set; }
        public int? IdUsuarioRegistro { get; set; }
        public string Observaciones { get; set; }
    }

    // Una fila del ranking: lo que vendió un agente en el período
    public class RankingAgente
    {
        public int Posicion { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }        // login
        public string Dni { get; set; }
        public string NombrePersona { get; set; }
        public string Apellido { get; set; }
        public int Cantidad { get; set; }
        public decimal TotalUsd { get; set; }

        public string NombreCompleto => string.IsNullOrWhiteSpace(NombrePersona) && string.IsNullOrWhiteSpace(Apellido)
            ? Usuario : $"{NombrePersona} {Apellido}".Trim();
    }

    public enum OrdenRanking
    {
        Cantidad,
        ValorTotal
    }

    // Períodos predefinidos del ranking (por fecha de venta); Personalizado usa Desde/Hasta
    public enum PeriodoRanking
    {
        EsteMes,
        EsteTrimestre,
        EsteAnio,
        Todo,
        Personalizado
    }

    // Resultado del ranking con el período y los criterios con que se armó (lo que muestra la pantalla y el PDF)
    public class ResultadoRanking
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string Periodo { get; set; }          // "Este mes", "Personalizado"...
        public OrdenRanking Orden { get; set; }
        public bool MasVendieron { get; set; }        // true: los 5 que más vendieron; false: los 5 que menos
        public int TotalAgentes { get; set; }
        public int VentasDelPeriodo { get; set; }
        public decimal TotalUsdDelPeriodo { get; set; }
        public List<RankingAgente> Filas { get; set; } = new List<RankingAgente>();
    }
}
