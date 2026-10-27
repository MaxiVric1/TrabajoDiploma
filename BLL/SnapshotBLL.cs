using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using SERVICIOS;

namespace BLL
{
    // Snapshot JSON del análisis de inversión en el momento de la decisión del Directorio (y su lectura para
    // consultarlo después, por ejemplo desde Gestión del Fideicomiso)
    public class SnapshotBLL
    {
        // Arma la foto con el análisis que vio el Directorio y el proyecto ya decidido (estado, fecha y motivo de la base)
        public static SnapshotDecision Construir(AnalisisComparativo a, ProyectoInversion decidido, Usuario usuario, ValorICC icc)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (decidido == null) throw new ArgumentNullException(nameof(decidido));
            ProyectoInversion p = decidido;
            GananciaInmobiliaria g = a.GananciaInmobiliaria;
            return new SnapshotDecision
            {
                Decision = p.Estado.ToString(),
                Motivo = p.Observaciones,
                FechaDecision = p.FechaDecision ?? DateTime.Now,
                Usuario = new SnapshotUsuario { Login = usuario?.Nombre, Nombre = usuario?.NombrePersona, Apellido = usuario?.Apellido },
                Proyecto = new SnapshotProyecto
                {
                    Id = p.IdProyecto, Direccion = p.Direccion, Localidad = p.Localidad, Smp = p.Smp, Distrito = p.DistritoUrbanistico,
                    TipoInmueble = p.TipoInmueble, Operacion = p.TipoPublicacion.ToString(), PorcentajeCanje = p.PorcentajeCanje
                },
                DatosTecnicos = new SnapshotDatosTecnicos
                {
                    SuperficieLote = p.SuperficieLote, SuperficieEdificablePlanta = p.SuperficieEdificablePlanta, AlturaMaxima = p.AlturaMaxima,
                    Pisos = p.Pisos, MetrosConstruibles = p.MetrosConstruibles, CostoObraM2 = p.CostoObraM2, CostoTotalObra = p.CostoTotalObra,
                    CostoObraProyectadoUsd = a.CostoObraProyectadoUSD, ValorVentaM2Unidades = p.ValorVentaM2Unidades, ValorTotalUnidades = p.ValorTotalUnidades
                },
                Mercado = new SnapshotMercado
                {
                    DolarOficial = a.DolarOficial.HasValue ? Math.Round(a.DolarOficial.Value, 2) : (decimal?)null, FechaDolarOficial = a.FechaDolarOficial, DolarMep = a.DolarMep, DolarBlue = a.DolarBlue,
                    IpcMensualPromedio = a.IpcMensualPromedio, IccMensualPromedio = a.IccMensualPromedio,
                    IccUltimoPeriodo = icc?.Periodo, IccUltimoIndice = icc?.ValorIndice, IccUltimaVariacion = icc?.VariacionMensual,
                    PeriodoIndices = a.PeriodoIndices
                },
                Analisis = new SnapshotAnalisis
                {
                    FechaAnalisis = a.Fecha,
                    PlazoObraMeses = a.Parametros.PlazoObraMeses,
                    HorizonteAlquilerMeses = a.Parametros.HorizonteAlquilerMeses,
                    ComisionVentaTradicionalPorcentaje = a.Parametros.ComisionVentaPorcentaje,
                    ComisionAlquilerPorcentaje = a.Parametros.ComisionAlquilerPorcentaje,
                    ValorTasadoPesos = a.ValorTasadoPesos,
                    EscenarioA = Escenario(a.Tradicional),
                    EscenarioB = Escenario(a.Fideicomiso),
                    UnidadesPropietarioUsd = a.UnidadesPropietarioUSD,
                    MargenInversoresUsd = a.MargenInversoresUSD,
                    RentabilidadInversoresPorcentaje = a.RentabilidadInversoresPorcentaje,
                    ComisionTradicionalUsd = a.ComisionTradicionalUSD,
                    DiferenciaUsd = a.DiferenciaUSD,
                    ConvieneFideicomiso = a.ConvieneFideicomiso,
                    Supuestos = a.Supuestos.ToList(),
                    Advertencias = a.Advertencias.ToList()
                },
                Comisiones = new SnapshotComisiones
                {
                    EstructuracionPorcentaje = p.PorcentajeEstructuracion,
                    AdministracionMensualPorcentaje = p.PorcentajeAdministracionMensual,
                    VentaUnidadesPorcentaje = p.PorcentajeVentaUnidades
                },
                GananciaInmobiliaria = g == null ? null : new SnapshotGanancia
                {
                    EstructuracionUsd = g.EstructuracionUSD, BaseEstructuracionUsd = g.BaseEstructuracionUSD,
                    AdministracionMensualUsd = g.AdministracionMensualUSD, MesesObra = g.MesesObra, AdministracionUsd = g.AdministracionUSD,
                    BaseAdministracionUsd = g.BaseAdministracionUSD, VentaUsd = g.VentaUSD, BaseVentaUsd = g.BaseVentaUSD, TotalUsd = g.TotalUSD
                }
            };
        }

        private static SnapshotEscenario Escenario(EscenarioAnalisis e) =>
            e == null ? null : new SnapshotEscenario { Nombre = e.Nombre, ResultadoUsd = e.ResultadoUSD, Detalle = e.Detalle.ToList() };

        // Serializa y guarda el snapshot. No lanza excepción: la decisión ya quedó registrada en la base, así que un
        // problema con el archivo se informa en el resultado.
        internal ResultadoSnapshot GuardarDecision(AnalisisComparativo a, ProyectoInversion decidido)
        {
            try
            {
                SnapshotDecision snapshot = Construir(a, decidido, SessionManager.Instance.UsuarioActual, new IccBLL().Ultimo());
                string ruta = GestorSnapshots.Guardar(snapshot, out bool alternativa);
                GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Snapshot Decisión",
                    $"Proyecto N° {decidido.IdProyecto}: análisis guardado en {ruta}.");
                return new ResultadoSnapshot { Ruta = ruta, EnCarpetaAlternativa = alternativa };
            }
            catch (Exception ex)
            {
                return new ResultadoSnapshot { Error = ex.Message };
            }
        }

        // ---------- Lectura ----------

        private static void ExigirPermisoLectura()
        {
            if (!SessionManager.Instance.EstaLogueado() ||
                !(SessionManager.Instance.TienePermiso(PermisosDefinidos.AccesoPN3Inversiones) || SessionManager.Instance.TienePermiso(PermisosDefinidos.AccesoPN3Directorio)))
                throw new UnauthorizedAccessException("Solo Inversiones o el Directorio pueden consultar los snapshots.");
        }

        public List<string> ListarArchivos(int idProyecto)
        {
            ExigirPermisoLectura();
            return GestorSnapshots.Listar(idProyecto);
        }

        public SnapshotDecision Leer(string ruta)
        {
            ExigirPermisoLectura();
            return GestorSnapshots.Leer(ruta);
        }
    }
}
