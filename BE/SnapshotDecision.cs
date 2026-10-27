using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BE
{
    // Foto del análisis de un proyecto en el momento en que el Directorio lo aprobó o rechazó. Se serializa a JSON
    // (DataContractJsonSerializer) y se guarda como archivo: queda como constancia aunque después cambien el dólar,
    // los índices o los datos del proyecto.
    [DataContract(Name = "snapshot")]
    public class SnapshotDecision
    {
        public const int VERSION_ACTUAL = 1;

        [DataMember(Name = "version", Order = 1)] public int Version { get; set; } = VERSION_ACTUAL;
        [DataMember(Name = "decision", Order = 2)] public string Decision { get; set; }          // Aprobado / Rechazado
        [DataMember(Name = "motivo", Order = 3)] public string Motivo { get; set; }
        [DataMember(Name = "fechaDecision", Order = 4)] public DateTime FechaDecision { get; set; }
        [DataMember(Name = "usuario", Order = 5)] public SnapshotUsuario Usuario { get; set; }
        [DataMember(Name = "proyecto", Order = 6)] public SnapshotProyecto Proyecto { get; set; }
        [DataMember(Name = "datosTecnicos", Order = 7)] public SnapshotDatosTecnicos DatosTecnicos { get; set; }
        [DataMember(Name = "mercado", Order = 8)] public SnapshotMercado Mercado { get; set; }
        [DataMember(Name = "analisis", Order = 9)] public SnapshotAnalisis Analisis { get; set; }
        [DataMember(Name = "comisiones", Order = 10)] public SnapshotComisiones Comisiones { get; set; }
        [DataMember(Name = "gananciaInmobiliaria", Order = 11)] public SnapshotGanancia GananciaInmobiliaria { get; set; }

        // De dónde se leyó (no se serializa)
        [IgnoreDataMember] public string RutaArchivo { get; set; }
    }

    [DataContract(Name = "usuario")]
    public class SnapshotUsuario
    {
        [DataMember(Name = "login", Order = 1)] public string Login { get; set; }
        [DataMember(Name = "nombre", Order = 2)] public string Nombre { get; set; }
        [DataMember(Name = "apellido", Order = 3)] public string Apellido { get; set; }

        public string NombreCompleto => string.IsNullOrWhiteSpace(Nombre) && string.IsNullOrWhiteSpace(Apellido) ? Login : $"{Nombre} {Apellido}".Trim();
    }

    [DataContract(Name = "proyecto")]
    public class SnapshotProyecto
    {
        [DataMember(Name = "id", Order = 1)] public int Id { get; set; }
        [DataMember(Name = "direccion", Order = 2)] public string Direccion { get; set; }
        [DataMember(Name = "localidad", Order = 3)] public string Localidad { get; set; }
        [DataMember(Name = "smp", Order = 4)] public string Smp { get; set; }
        [DataMember(Name = "distrito", Order = 5)] public string Distrito { get; set; }
        [DataMember(Name = "tipoInmueble", Order = 6)] public string TipoInmueble { get; set; }
        [DataMember(Name = "operacion", Order = 7)] public string Operacion { get; set; }
        [DataMember(Name = "porcentajeCanje", Order = 8)] public decimal PorcentajeCanje { get; set; }
    }

    [DataContract(Name = "datosTecnicos")]
    public class SnapshotDatosTecnicos
    {
        [DataMember(Name = "superficieLote", Order = 1)] public decimal? SuperficieLote { get; set; }
        [DataMember(Name = "superficieEdificablePlanta", Order = 2)] public decimal? SuperficieEdificablePlanta { get; set; }
        [DataMember(Name = "alturaMaxima", Order = 3)] public decimal? AlturaMaxima { get; set; }
        [DataMember(Name = "pisos", Order = 4)] public int? Pisos { get; set; }
        [DataMember(Name = "metrosConstruibles", Order = 5)] public decimal? MetrosConstruibles { get; set; }
        [DataMember(Name = "costoObraM2", Order = 6)] public decimal? CostoObraM2 { get; set; }
        [DataMember(Name = "costoTotalObra", Order = 7)] public decimal? CostoTotalObra { get; set; }
        [DataMember(Name = "costoObraProyectadoUsd", Order = 8)] public decimal CostoObraProyectadoUsd { get; set; }
        [DataMember(Name = "valorVentaM2Unidades", Order = 9)] public decimal? ValorVentaM2Unidades { get; set; }
        [DataMember(Name = "valorTotalUnidades", Order = 10)] public decimal? ValorTotalUnidades { get; set; }
    }

    // Tipos de cambio (pesos por dólar) e índices (en % mensual) usados en el análisis
    [DataContract(Name = "mercado")]
    public class SnapshotMercado
    {
        [DataMember(Name = "dolarOficial", Order = 1)] public decimal? DolarOficial { get; set; }
        [DataMember(Name = "fechaDolarOficial", Order = 2)] public DateTime? FechaDolarOficial { get; set; }
        [DataMember(Name = "dolarMep", Order = 3)] public decimal DolarMep { get; set; }
        [DataMember(Name = "dolarBlue", Order = 4)] public decimal? DolarBlue { get; set; }
        [DataMember(Name = "ipcMensualPromedio", Order = 5)] public decimal IpcMensualPromedio { get; set; }
        [DataMember(Name = "iccMensualPromedio", Order = 6)] public decimal IccMensualPromedio { get; set; }
        [DataMember(Name = "iccUltimoPeriodo", Order = 7)] public DateTime? IccUltimoPeriodo { get; set; }
        [DataMember(Name = "iccUltimoIndice", Order = 8)] public decimal? IccUltimoIndice { get; set; }
        [DataMember(Name = "iccUltimaVariacion", Order = 9)] public decimal? IccUltimaVariacion { get; set; }
        [DataMember(Name = "periodoIndices", Order = 10)] public string PeriodoIndices { get; set; }
    }

    [DataContract(Name = "escenario")]
    public class SnapshotEscenario
    {
        [DataMember(Name = "nombre", Order = 1)] public string Nombre { get; set; }
        [DataMember(Name = "resultadoUsd", Order = 2)] public decimal ResultadoUsd { get; set; }
        [DataMember(Name = "detalle", Order = 3)] public List<string> Detalle { get; set; } = new List<string>();
    }

    [DataContract(Name = "analisis")]
    public class SnapshotAnalisis
    {
        [DataMember(Name = "fechaAnalisis", Order = 1)] public DateTime FechaAnalisis { get; set; }
        [DataMember(Name = "plazoObraMeses", Order = 2)] public int PlazoObraMeses { get; set; }
        [DataMember(Name = "horizonteAlquilerMeses", Order = 3)] public int HorizonteAlquilerMeses { get; set; }
        [DataMember(Name = "comisionVentaTradicionalPorcentaje", Order = 4)] public decimal ComisionVentaTradicionalPorcentaje { get; set; }
        [DataMember(Name = "comisionAlquilerPorcentaje", Order = 5)] public decimal ComisionAlquilerPorcentaje { get; set; }
        [DataMember(Name = "valorTasadoPesos", Order = 6)] public decimal ValorTasadoPesos { get; set; }
        [DataMember(Name = "escenarioA", Order = 7)] public SnapshotEscenario EscenarioA { get; set; }
        [DataMember(Name = "escenarioB", Order = 8)] public SnapshotEscenario EscenarioB { get; set; }
        [DataMember(Name = "unidadesPropietarioUsd", Order = 9)] public decimal UnidadesPropietarioUsd { get; set; }
        [DataMember(Name = "margenInversoresUsd", Order = 10)] public decimal MargenInversoresUsd { get; set; }
        [DataMember(Name = "rentabilidadInversoresPorcentaje", Order = 11)] public decimal RentabilidadInversoresPorcentaje { get; set; }
        [DataMember(Name = "comisionTradicionalUsd", Order = 12)] public decimal ComisionTradicionalUsd { get; set; }
        [DataMember(Name = "diferenciaUsd", Order = 13)] public decimal DiferenciaUsd { get; set; }
        [DataMember(Name = "convieneFideicomiso", Order = 14)] public bool ConvieneFideicomiso { get; set; }
        [DataMember(Name = "supuestos", Order = 15)] public List<string> Supuestos { get; set; } = new List<string>();
        [DataMember(Name = "advertencias", Order = 16)] public List<string> Advertencias { get; set; } = new List<string>();
    }

    [DataContract(Name = "comisiones")]
    public class SnapshotComisiones
    {
        [DataMember(Name = "estructuracionPorcentaje", Order = 1)] public decimal EstructuracionPorcentaje { get; set; }
        [DataMember(Name = "administracionMensualPorcentaje", Order = 2)] public decimal AdministracionMensualPorcentaje { get; set; }
        [DataMember(Name = "ventaUnidadesPorcentaje", Order = 3)] public decimal VentaUnidadesPorcentaje { get; set; }
    }

    [DataContract(Name = "gananciaInmobiliaria")]
    public class SnapshotGanancia
    {
        [DataMember(Name = "estructuracionUsd", Order = 1)] public decimal EstructuracionUsd { get; set; }
        [DataMember(Name = "baseEstructuracionUsd", Order = 2)] public decimal BaseEstructuracionUsd { get; set; }
        [DataMember(Name = "administracionMensualUsd", Order = 3)] public decimal AdministracionMensualUsd { get; set; }
        [DataMember(Name = "mesesObra", Order = 4)] public int MesesObra { get; set; }
        [DataMember(Name = "administracionUsd", Order = 5)] public decimal AdministracionUsd { get; set; }
        [DataMember(Name = "baseAdministracionUsd", Order = 6)] public decimal BaseAdministracionUsd { get; set; }
        [DataMember(Name = "ventaUsd", Order = 7)] public decimal VentaUsd { get; set; }
        [DataMember(Name = "baseVentaUsd", Order = 8)] public decimal BaseVentaUsd { get; set; }
        [DataMember(Name = "totalUsd", Order = 9)] public decimal TotalUsd { get; set; }
    }

    // Dónde quedó guardado el snapshot de una decisión (o por qué no se pudo guardar)
    public class ResultadoSnapshot
    {
        public string Ruta { get; set; }
        public string Error { get; set; }
        public bool Guardado => Ruta != null;
    }
}
