using System;
using System.Collections.Generic;

namespace BE
{
    // Aporte de un inversor a un fideicomiso (en USD). La participación es el aporte sobre el capital necesario
    // (costo total de la obra); la calcula la consulta, no se guarda.
    public class SuscripcionFideicomiso
    {
        public int IdSuscripcion { get; set; }
        public int IdProyecto { get; set; }
        public int IdInversor { get; set; }
        public decimal MontoAportado { get; set; }
        public decimal? PorcentajeParticipacion { get; set; }
        public int UnidadesAsignadas { get; set; }
        public DateTime FechaSuscripcion { get; set; }

        // Del inversor (los trae la consulta por proyecto)
        public string NombreInversor { get; set; }
        public string Cuit { get; set; }
    }

    // Estado del fondeo de un fideicomiso: lo que muestra el tablero de Inversiones
    public class ResumenFideicomiso
    {
        public ProyectoInversion Proyecto { get; set; }
        public decimal CapitalNecesario { get; set; }
        public decimal CapitalSuscripto { get; set; }
        public decimal CapitalRestante => Math.Max(0, CapitalNecesario - CapitalSuscripto);
        public decimal PorcentajeFondeado => CapitalNecesario > 0 ? Math.Round(CapitalSuscripto * 100 / CapitalNecesario, 2) : 0;
        public bool Fondeado => CapitalNecesario > 0 && CapitalSuscripto >= CapitalNecesario;
        public List<SuscripcionFideicomiso> Suscripciones { get; set; } = new List<SuscripcionFideicomiso>();
    }
}
