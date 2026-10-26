using System;
using System.Collections.Generic;

namespace BE
{
    // Supuestos del análisis comparativo (el canje es de cada proyecto: ProyectoInversion.PorcentajeCanje)
    public class ParametrosAnalisis
    {
        public int PlazoObraMeses { get; set; } = 24;                 // el propietario recibe sus unidades al terminar
        public int HorizonteAlquilerMeses { get; set; } = 48;         // meses de alquiler del escenario tradicional
        public decimal ComisionVentaPorcentaje { get; set; } = 5;     // comisión inmobiliaria de una venta tradicional
        public decimal ComisionAlquilerPorcentaje { get; set; } = 4;  // comisión mensual del alquiler (PN2)
        public decimal RentabilidadAlquilerAnual { get; set; } = 5;   // % para estimar el valor de un inmueble de alquiler
        public int MesesPromedioIndices { get; set; } = 12;           // meses para promediar IPC e ICC
    }

    // Un escenario visto desde el propietario: lo que obtiene al final del horizonte, en USD
    public class EscenarioAnalisis
    {
        public string Nombre { get; set; }
        public decimal ResultadoUSD { get; set; }
        public List<string> Detalle { get; set; } = new List<string>();
    }

    // Lo que gana LandSmart con el fideicomiso (en USD), según las tres comisiones del proyecto
    public class GananciaInmobiliaria
    {
        public decimal PorcentajeEstructuracion { get; set; }
        public decimal BaseEstructuracionUSD { get; set; }          // costo de obra
        public decimal EstructuracionUSD { get; set; }

        public decimal PorcentajeAdministracionMensual { get; set; }
        public decimal CuotaMensualObraUSD { get; set; }            // costo de obra / meses de obra
        public int MesesObra { get; set; }
        public decimal AdministracionMensualUSD { get; set; }
        public decimal AdministracionUSD { get; set; }              // la mensual por los meses de obra

        public decimal PorcentajeVentaUnidades { get; set; }
        public decimal BaseVentaUSD { get; set; }                   // unidades de los inversores (las del propietario no se venden)
        public decimal VentaUSD { get; set; }

        public decimal TotalUSD => EstructuracionUSD + AdministracionUSD + VentaUSD;
    }

    // Comparación de una propiedad: escenario tradicional (venta o alquiler) contra desarrollo en fideicomiso
    public class AnalisisComparativo
    {
        public ProyectoInversion Proyecto { get; set; }
        public ParametrosAnalisis Parametros { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Datos de mercado usados (pesos por dólar; índices mensuales en %) y de dónde salieron
        public decimal DolarMep { get; set; }
        public decimal? DolarBlue { get; set; }
        public decimal? DolarOficial { get; set; }
        public DateTime? FechaDolarOficial { get; set; }
        public decimal IpcMensualPromedio { get; set; }
        public decimal IccMensualPromedio { get; set; }
        public string PeriodoIndices { get; set; }

        // Valor del inmueble según la tasación del PN1 (en pesos): de venta, o alquiler mensual si es de alquiler
        public decimal ValorTasadoPesos { get; set; }

        // Fideicomiso: costo de obra proyectado con el ICC y lo que queda para inversores
        public decimal CostoObraProyectadoUSD { get; set; }
        public decimal ValorUnidadesUSD { get; set; }
        public decimal UnidadesPropietarioUSD { get; set; }
        public GananciaInmobiliaria GananciaInmobiliaria { get; set; }
        public decimal MargenInversoresUSD { get; set; }                // descontadas las comisiones de LandSmart
        public decimal RentabilidadInversoresPorcentaje { get; set; }   // sobre la obra, en todo el plazo

        // Comisión de LandSmart en el escenario tradicional (venta, o alquiler durante el horizonte)
        public decimal ComisionTradicionalUSD { get; set; }

        public EscenarioAnalisis Tradicional { get; set; }
        public EscenarioAnalisis Fideicomiso { get; set; }
        public decimal DiferenciaUSD => Fideicomiso.ResultadoUSD - Tradicional.ResultadoUSD;
        public bool ConvieneFideicomiso => DiferenciaUSD > 0;

        public List<string> Supuestos { get; set; } = new List<string>();
        public List<string> Advertencias { get; set; } = new List<string>(); // datos faltantes, APIs sin conexión...
    }
}
