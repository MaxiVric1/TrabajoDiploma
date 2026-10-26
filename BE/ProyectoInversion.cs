using System;

namespace BE
{
    // En la base se guardan con el mismo nombre ('EnAnalisis', 'Aprobado', 'Rechazado')
    public enum EstadoProyecto
    {
        EnAnalisis,
        Aprobado,
        Rechazado
    }

    // Desarrollo en fideicomiso sobre un inmueble del inventario. Montos por m² y totales en dólares (USD).
    // MetrosConstruibles, CostoTotalObra y ValorTotalUnidades los calcula SQL Server (null hasta que Arquitectura
    // carga los datos técnicos).
    public class ProyectoInversion
    {
        public int IdProyecto { get; set; }
        public int IdInmueble { get; set; }
        public decimal? SuperficieLote { get; set; }
        // Código Urbanístico (en CABA los trae EPOK): superficie edificable por planta y altura máxima
        public decimal? SuperficieEdificablePlanta { get; set; }
        public decimal? AlturaMaxima { get; set; }          // metros
        public string Smp { get; set; }                     // sección-manzana-parcela (catastro de CABA)
        public string DistritoUrbanistico { get; set; }
        public decimal? MetrosConstruibles { get; set; }    // pisos (altura / 3 m) x superficie edificable en planta
        public decimal? CostoObraM2 { get; set; }
        public decimal? CostoTotalObra { get; set; }        // m² construibles x costo por m² = capital necesario
        public decimal? ValorVentaM2Unidades { get; set; }
        public decimal? ValorTotalUnidades { get; set; }    // m² construibles x valor de venta por m²
        public decimal PorcentajeCanje { get; set; } = 30;   // % de las unidades para el propietario, por aportar el lote
        // Comisiones de LandSmart (las define el Directorio): ver GananciaInmobiliaria
        public decimal PorcentajeEstructuracion { get; set; } = 2;         // sobre el costo de obra, al estructurar el fideicomiso
        public decimal PorcentajeAdministracionMensual { get; set; } = 1;  // por mes de obra, sobre el capital total del fideicomiso
        public decimal PorcentajeVentaUnidades { get; set; } = 5;          // sobre la venta de las unidades de los inversores
        public EstadoProyecto Estado { get; set; } = EstadoProyecto.EnAnalisis;
        public int? IdUsuarioArquitectura { get; set; }
        public int? IdUsuarioDirectorio { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaDecision { get; set; }
        public string Observaciones { get; set; }

        // Del inmueble y de las suscripciones (los traen las consultas)
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string TipoInmueble { get; set; }
        public TipoPublicacion TipoPublicacion { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public decimal CapitalSuscripto { get; set; }
        public int CantidadInversores { get; set; }

        public bool DatosTecnicosCompletos => SuperficieLote.HasValue && SuperficieEdificablePlanta.HasValue && AlturaMaxima.HasValue &&
                                              CostoObraM2.HasValue && ValorVentaM2Unidades.HasValue;

        // Pisos que entran en la altura máxima (3 m por piso), como lo calcula la base
        public int? Pisos => AlturaMaxima.HasValue ? (int?)Math.Floor(AlturaMaxima.Value / ParcelaCaba.ALTURA_POR_PISO) : null;

        public decimal CapitalNecesario => CostoTotalObra ?? 0;

        public override string ToString()
        {
            return $"Proyecto N° {IdProyecto} - {Direccion}" + (string.IsNullOrEmpty(Localidad) ? "" : $", {Localidad}");
        }
    }
}
