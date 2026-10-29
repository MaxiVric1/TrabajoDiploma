using System;

namespace BE
{
    // Parámetros urbanísticos reales de una parcela de CABA (USIG + EPOK, Código Urbanístico):
    // superficie de la parcela, superficie edificable en planta, altura máxima y distrito
    public class ParcelaCaba
    {
        public const decimal ALTURA_POR_PISO = 3.0m; // metros por piso para estimar la cantidad de pisos
        // Si EPOK no informa la superficie edificable en planta, se estima como este porcentaje del lote
        public const decimal FACTOR_EDIFICABLE_ESTIMADO = 0.60m;

        public string DireccionNormalizada { get; set; }   // como la entiende USIG ("GURRUCHAGA 1801, CABA")
        public string DireccionCatastral { get; set; }     // la de la parcela en catastro (puede ser la de la esquina)
        public int AlturaConsultada { get; set; }          // altura que encontró la parcela (puede ser una vecina)
        public string Smp { get; set; }                    // sección-manzana-parcela
        public decimal? SuperficieParcela { get; set; }
        public decimal? SuperficieEdificablePlanta { get; set; }
        public bool SuperficieEdificableEstimada { get; set; } // true: lote x FACTOR_EDIFICABLE_ESTIMADO (EPOK no la informó)
        public decimal? AlturaMaxima { get; set; }
        public string Distrito { get; set; }
        public string Busqueda { get; set; }               // cómo se encontró: "dirección exacta", "altura vecina 1801", "coordenadas"

        public int? Pisos => AlturaMaxima.HasValue ? (int?)Math.Floor(AlturaMaxima.Value / ALTURA_POR_PISO) : null;
        public decimal? MetrosConstruibles => Pisos.HasValue && SuperficieEdificablePlanta.HasValue ? Pisos * SuperficieEdificablePlanta : null;
    }
}
