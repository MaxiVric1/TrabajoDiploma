using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // PN3: desarrollo en fideicomiso de una casa del inventario. El Analista de Inversiones crea el proyecto,
    // Arquitectura carga los datos técnicos, el Directorio aprueba o rechaza.
    public class ProyectoInversionBLL
    {
        private readonly ProyectoInversionDAL proyectoDAL = new ProyectoInversionDAL();
        private readonly InmuebleDAL inmuebleDAL = new InmuebleDAL();

        // ---------- Permisos ----------

        internal static void ExigirPermiso(string permiso, string quien)
        {
            if (!SessionManager.Instance.EstaLogueado() || !SessionManager.Instance.TienePermiso(permiso))
                throw new UnauthorizedAccessException($"Solo {quien} puede realizar esta operación.");
        }

        private static int UsuarioActual() => SessionManager.Instance.UsuarioActual.Id;

        public static string Texto(EstadoProyecto estado) => estado == EstadoProyecto.EnAnalisis ? "En Análisis" : estado.ToString();

        // ---------- Proyecto ----------

        // Casas Publicadas (en venta o en alquiler) que todavía no tienen un proyecto en análisis o aprobado
        public List<Inmueble> LeerInmueblesEvaluables()
        {
            HashSet<int> conProyecto = new HashSet<int>(proyectoDAL.LeerTodos()
                .Where(p => p.Estado != EstadoProyecto.Rechazado).Select(p => p.IdInmueble));
            return inmuebleDAL.LeerTodos()
                .Where(i => InmuebleBLL.EsCasa(i) && i.EstadoActual == EstadoInmueble.Publicado && !conProyecto.Contains(i.IdInmueble))
                .ToList();
        }

        public const decimal PORCENTAJE_CANJE_DEFECTO = 30;

        // Solo para una casa Publicada sin proyecto vigente (uno rechazado no impide evaluarla de nuevo).
        // porcentajeCanje: % de las unidades que recibe el propietario por aportar el lote.
        public int CrearProyecto(int idInmueble, decimal porcentajeCanje = PORCENTAJE_CANJE_DEFECTO, string observaciones = null)
        {
            ExigirPermiso(PermisosDefinidos.AccesoPN3Analisis, "el Analista de Inversiones");
            if (porcentajeCanje <= 0 || porcentajeCanje >= 100)
                throw new ArgumentException("El canje debe ser un porcentaje mayor a 0 y menor a 100.");
            Inmueble inmueble = inmuebleDAL.BuscarPorId(idInmueble);
            if (inmueble == null)
                throw new ArgumentException("El inmueble no existe.");
            if (!InmuebleBLL.EsCasa(inmueble))
                throw new ArgumentException("Solo se evalúan desarrollos sobre casas (el lote es lo que se aprovecha).");
            if (inmueble.EstadoActual != EstadoInmueble.Publicado)
                throw new ArgumentException($"Solo se evalúan inmuebles Publicados (este está {inmueble.EstadoActual}).");
            ProyectoInversion vigente = proyectoDAL.LeerTodos().FirstOrDefault(p => p.IdInmueble == idInmueble && p.Estado != EstadoProyecto.Rechazado);
            if (vigente != null)
                throw new ArgumentException($"El inmueble ya tiene el Proyecto N° {vigente.IdProyecto} ({Texto(vigente.Estado)}).");

            ProyectoInversion proyecto = new ProyectoInversion
            {
                IdInmueble = idInmueble,
                SuperficieLote = inmueble.SuperficieLote,
                PorcentajeCanje = Math.Round(porcentajeCanje, 2),
                Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
            };
            int id = proyectoDAL.Crear(proyecto);
            if (id == -1)
                throw new Exception("No se pudo crear el proyecto.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Proyecto Inversión",
                $"Proyecto N° {id} sobre el inmueble {idInmueble} ({inmueble.Direccion}, {inmueble.Localidad}), canje {proyecto.PorcentajeCanje:0.##}%.");
            return id;
        }

        // Datos técnicos de Arquitectura (lote, FOS, FOT, costo de obra y valor de venta por m², en USD): solo En Análisis
        public void ActualizarDatosTecnicos(ProyectoInversion datos)
        {
            ExigirPermiso(PermisosDefinidos.AccesoPN3Arquitectura, "Arquitectura");
            if (datos == null) throw new ArgumentNullException(nameof(datos));
            if (!(datos.SuperficieLote > 0))
                throw new ArgumentException("La superficie del lote debe ser mayor a cero.");
            if (!(datos.Fos > 0 && datos.Fos <= 1))
                throw new ArgumentException("El FOS debe estar entre 0 y 1 (por ejemplo 0,6 = 60% del lote).");
            if (!(datos.Fot > 0 && datos.Fot <= 20))
                throw new ArgumentException("El FOT debe ser mayor a cero y como máximo 20.");
            if (!(datos.CostoObraM2 > 0))
                throw new ArgumentException("El costo de obra por m² debe ser mayor a cero.");
            if (!(datos.ValorVentaM2Unidades > 0))
                throw new ArgumentException("El valor de venta por m² de las unidades debe ser mayor a cero.");

            ProyectoInversion actual = proyectoDAL.BuscarPorId(datos.IdProyecto);
            if (actual == null)
                throw new Exception("El proyecto no existe.");
            if (actual.Estado != EstadoProyecto.EnAnalisis)
                throw new ArgumentException($"El proyecto ya fue {actual.Estado.ToString().ToLower()}: sus datos técnicos no se modifican.");

            datos.IdUsuarioArquitectura = UsuarioActual();
            datos.Observaciones = string.IsNullOrWhiteSpace(datos.Observaciones) ? null : datos.Observaciones.Trim();
            if (!proyectoDAL.ActualizarDatosTecnicos(datos))
                throw new Exception("No se pudieron guardar los datos técnicos.");

            ProyectoInversion guardado = proyectoDAL.BuscarPorId(datos.IdProyecto);
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Datos Técnicos Proyecto",
                $"Proyecto N° {datos.IdProyecto}: lote {datos.SuperficieLote:N2} m², FOS {datos.Fos}, FOT {datos.Fot}, " +
                $"{guardado.MetrosConstruibles:N2} m² construibles, obra USD {guardado.CostoTotalObra:N2}, unidades USD {guardado.ValorTotalUnidades:N2}.");
        }

        // Comisiones de LandSmart sobre el fideicomiso: las fija el Directorio antes de decidir (solo En Análisis)
        public void ActualizarComisiones(int idProyecto, decimal estructuracion, decimal administracionMensual, decimal ventaUnidades)
        {
            ExigirPermiso(PermisosDefinidos.AccesoPN3Directorio, "el Directorio");
            foreach (var (valor, nombre) in new[] { (estructuracion, "estructuración"), (administracionMensual, "administración mensual"), (ventaUnidades, "venta de unidades") })
                if (valor < 0 || valor >= 100)
                    throw new ArgumentException($"La comisión de {nombre} debe ser un porcentaje entre 0 y 100.");
            ProyectoInversion proyecto = proyectoDAL.BuscarPorId(idProyecto);
            if (proyecto == null)
                throw new Exception("El proyecto no existe.");
            if (proyecto.Estado != EstadoProyecto.EnAnalisis)
                throw new ArgumentException($"El proyecto ya fue {proyecto.Estado.ToString().ToLower()}: sus comisiones no se modifican.");
            if (!proyectoDAL.ActualizarComisiones(idProyecto, Math.Round(estructuracion, 2), Math.Round(administracionMensual, 2), Math.Round(ventaUnidades, 2)))
                throw new Exception("No se pudieron guardar las comisiones.");
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Comisiones Proyecto",
                $"Proyecto N° {idProyecto}: estructuración {estructuracion:0.##}%, administración {administracionMensual:0.##}% mensual, venta de unidades {ventaUnidades:0.##}%.");
        }

        // Ganancia de LandSmart con el fideicomiso, en USD:
        //  - estructuración: % sobre el costo de obra, al armar el fideicomiso
        //  - administración: % mensual sobre la cuota mensual de obra (costo / meses), durante los meses de obra
        //  - venta: % sobre las unidades de los inversores (las del propietario son suyas y no se venden)
        public static GananciaInmobiliaria CalcularGananciaInmobiliaria(ProyectoInversion proyecto, decimal costoObraUSD, decimal unidadesInversoresUSD, int mesesObra)
        {
            if (proyecto == null) throw new ArgumentNullException(nameof(proyecto));
            if (mesesObra <= 0) throw new ArgumentException("Los meses de obra deben ser mayores a cero.");
            GananciaInmobiliaria g = new GananciaInmobiliaria
            {
                PorcentajeEstructuracion = proyecto.PorcentajeEstructuracion,
                BaseEstructuracionUSD = costoObraUSD,
                EstructuracionUSD = Math.Round(costoObraUSD * proyecto.PorcentajeEstructuracion / 100, 2),
                PorcentajeAdministracionMensual = proyecto.PorcentajeAdministracionMensual,
                CuotaMensualObraUSD = Math.Round(costoObraUSD / mesesObra, 2),
                MesesObra = mesesObra,
                PorcentajeVentaUnidades = proyecto.PorcentajeVentaUnidades,
                BaseVentaUSD = Math.Max(0, unidadesInversoresUSD),
                VentaUSD = Math.Round(Math.Max(0, unidadesInversoresUSD) * proyecto.PorcentajeVentaUnidades / 100, 2)
            };
            g.AdministracionMensualUSD = Math.Round(g.CuotaMensualObraUSD * proyecto.PorcentajeAdministracionMensual / 100, 2);
            g.AdministracionUSD = g.AdministracionMensualUSD * mesesObra;
            return g;
        }

        public void AprobarProyecto(int idProyecto, string observaciones = null)
        {
            Decidir(idProyecto, EstadoProyecto.Aprobado, observaciones);
        }

        // El rechazo debe tener motivo
        public void RechazarProyecto(int idProyecto, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Indique el motivo del rechazo.");
            Decidir(idProyecto, EstadoProyecto.Rechazado, motivo);
        }

        private void Decidir(int idProyecto, EstadoProyecto estado, string observaciones)
        {
            ExigirPermiso(PermisosDefinidos.AccesoPN3Directorio, "el Directorio");
            ProyectoInversion proyecto = proyectoDAL.BuscarPorId(idProyecto);
            if (proyecto == null)
                throw new Exception("El proyecto no existe.");
            if (proyecto.Estado != EstadoProyecto.EnAnalisis)
                throw new ArgumentException($"El proyecto ya fue {proyecto.Estado.ToString().ToLower()}.");
            if (!proyecto.DatosTecnicosCompletos)
                throw new ArgumentException("Arquitectura todavía no cargó todos los datos técnicos del proyecto.");

            if (!proyectoDAL.RegistrarDecision(idProyecto, estado, UsuarioActual(), string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()))
                throw new Exception("No se pudo registrar la decisión.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual,
                estado == EstadoProyecto.Aprobado ? "Aprobación Proyecto" : "Rechazo Proyecto",
                $"Proyecto N° {idProyecto} {estado.ToString().ToLower()} por el Directorio" + (string.IsNullOrWhiteSpace(observaciones) ? "." : $": {observaciones.Trim()}"));
        }

        // Solo En Análisis y sin suscripciones
        public void EliminarProyecto(int idProyecto)
        {
            ExigirPermiso(PermisosDefinidos.AccesoPN3Analisis, "el Analista de Inversiones");
            ProyectoInversion proyecto = proyectoDAL.BuscarPorId(idProyecto);
            if (proyecto == null)
                throw new Exception("El proyecto no existe.");
            if (proyecto.Estado != EstadoProyecto.EnAnalisis)
                throw new ArgumentException("Solo se elimina un proyecto En Análisis.");
            if (!proyectoDAL.Eliminar(idProyecto))
                throw new Exception("No se pudo eliminar el proyecto.");
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Proyecto Inversión", $"Proyecto N° {idProyecto} eliminado.");
        }

        public List<ProyectoInversion> LeerTodos(EstadoProyecto? estado = null) => proyectoDAL.LeerTodos(estado);

        public ProyectoInversion BuscarPorId(int idProyecto) => proyectoDAL.BuscarPorId(idProyecto);

        // ---------- Análisis comparativo ----------

        // Compara, para el propietario y en dólares, el escenario tradicional de la propiedad (venta si está en venta,
        // alquiler continuo si está en alquiler) contra aportar el lote al fideicomiso a cambio de unidades.
        // Datos de mercado: dólar MEP y blue (DolarAPI), oficial (BCRA), IPC (BCRA) e ICC (INDEC).
        public AnalisisComparativo ObtenerAnalisisComparativo(int idProyecto, ParametrosAnalisis parametros = null)
        {
            ParametrosAnalisis p = parametros ?? new ParametrosAnalisis();
            if (p.PlazoObraMeses <= 0 || p.HorizonteAlquilerMeses <= 0 || p.RentabilidadAlquilerAnual <= 0)
                throw new ArgumentException("Parámetros del análisis inválidos.");

            ProyectoInversion proyecto = proyectoDAL.BuscarPorId(idProyecto);
            if (proyecto == null)
                throw new Exception("El proyecto no existe.");
            if (!proyecto.DatosTecnicosCompletos)
                throw new ArgumentException("Arquitectura todavía no cargó los datos técnicos: no se puede comparar.");

            AnalisisComparativo a = new AnalisisComparativo { Proyecto = proyecto, Parametros = p };

            // Dólar: MEP para convertir (es el que usa el mercado inmobiliario); oficial y blue como referencia
            CotizacionesBLL cotizaciones = new CotizacionesBLL();
            CotizacionDolar mep = cotizaciones.Mep(), blue = cotizaciones.Blue();
            var oficial = cotizaciones.Oficial();
            a.DolarBlue = blue?.Venta;
            a.DolarOficial = oficial?.Valor;
            a.FechaDolarOficial = oficial?.Fecha;
            if (mep?.Venta > 0)
                a.DolarMep = mep.Venta.Value;
            else if (oficial.HasValue)
            {
                a.DolarMep = oficial.Value.Valor;
                a.Advertencias.Add("Sin cotización del dólar MEP (DolarAPI): se usa el oficial del BCRA.");
            }
            else
                throw new Exception("No hay cotización del dólar disponible (DolarAPI y BCRA sin datos): no se puede comparar en USD.");
            if (cotizaciones.UltimoError != null) a.Advertencias.Add("Dólar: " + cotizaciones.UltimoError);

            // Índices: promedio mensual de los últimos meses publicados
            DateTime mesActual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            IndicesBCRABLL indices = new IndicesBCRABLL();
            List<decimal> ipc = indices.InflacionMensual(Enumerable.Range(1, p.MesesPromedioIndices).Select(i => mesActual.AddMonths(-i)))
                .Where(v => v.Value.HasValue).Select(v => v.Value.Value).ToList();
            IccBLL iccBLL = new IccBLL();
            List<ValorICC> icc = iccBLL.Serie().Where(v => v.VariacionMensual.HasValue).OrderByDescending(v => v.Periodo)
                .Take(p.MesesPromedioIndices).ToList();
            if (ipc.Count == 0) a.Advertencias.Add("Sin IPC del BCRA: se proyecta sin inflación." + (indices.UltimoError != null ? " " + indices.UltimoError : ""));
            if (icc.Count == 0) a.Advertencias.Add("Sin ICC del INDEC: el costo de obra no se ajusta." + (iccBLL.UltimoError != null ? " " + iccBLL.UltimoError : ""));
            a.IpcMensualPromedio = ipc.Count > 0 ? Math.Round(ipc.Average(), 4) : 0;
            a.IccMensualPromedio = icc.Count > 0 ? Math.Round(icc.Average(v => v.VariacionMensual.Value), 4) : 0;
            a.PeriodoIndices = icc.Count > 0 ? $"ICC {icc.Last().Periodo:MM/yyyy}-{icc.First().Periodo:MM/yyyy} ({icc.Count} meses), IPC {ipc.Count} meses" : $"IPC {ipc.Count} meses";

            // Valor de la tasación del PN1 (última aprobada; si no hay, el pretendido)
            CaptacionResumen captacion = new CaptacionBLL().LeerResumen().Where(r => r.IdInmueble == proyecto.IdInmueble)
                .OrderByDescending(r => r.NroCaptacion).FirstOrDefault();
            a.ValorTasadoPesos = captacion?.ValorFinal ?? captacion?.ValorPretendido ?? 0;
            if (captacion?.ValorFinal == null) a.Advertencias.Add("El inmueble no tiene tasación aprobada: se usa el valor pretendido.");

            // ---------- Fideicomiso ----------
            // Costo de obra: se paga a lo largo de la obra (en promedio, a mitad de plazo). En pesos sube con el ICC;
            // el dólar se supone acompañando al IPC, así que en USD varía según la diferencia entre ICC e IPC.
            double mitad = p.PlazoObraMeses / 2.0;
            decimal factorIcc = (decimal)Math.Pow(1 + (double)a.IccMensualPromedio / 100, mitad);
            decimal factorDolar = (decimal)Math.Pow(1 + (double)a.IpcMensualPromedio / 100, mitad);
            a.CostoObraProyectadoUSD = Math.Round(proyecto.CostoTotalObra.Value * factorIcc / factorDolar, 2);
            a.ValorUnidadesUSD = proyecto.ValorTotalUnidades.Value;
            a.UnidadesPropietarioUSD = Math.Round(a.ValorUnidadesUSD * proyecto.PorcentajeCanje / 100, 2);
            a.GananciaInmobiliaria = CalcularGananciaInmobiliaria(proyecto, a.CostoObraProyectadoUSD, a.ValorUnidadesUSD - a.UnidadesPropietarioUSD, p.PlazoObraMeses);
            a.MargenInversoresUSD = Math.Round(a.ValorUnidadesUSD - a.UnidadesPropietarioUSD - a.CostoObraProyectadoUSD - a.GananciaInmobiliaria.TotalUSD, 2);
            a.RentabilidadInversoresPorcentaje = a.CostoObraProyectadoUSD > 0 ? Math.Round(a.MargenInversoresUSD * 100 / a.CostoObraProyectadoUSD, 2) : 0;
            a.Fideicomiso = new EscenarioAnalisis
            {
                Nombre = "Desarrollo en fideicomiso",
                ResultadoUSD = a.UnidadesPropietarioUSD,
                Detalle =
                {
                    $"{proyecto.MetrosConstruibles:N2} m² construibles (lote {proyecto.SuperficieLote:N2} m² x FOT {proyecto.Fot})",
                    $"Unidades: {proyecto.MetrosConstruibles:N2} m² x USD {proyecto.ValorVentaM2Unidades:N2} = USD {a.ValorUnidadesUSD:N2}",
                    $"El propietario aporta el lote y recibe el {proyecto.PorcentajeCanje:0.##}% de las unidades: USD {a.UnidadesPropietarioUSD:N2} al terminar la obra ({p.PlazoObraMeses} meses)",
                    $"Costo de obra USD {proyecto.CostoTotalObra:N2}, proyectado con el ICC: USD {a.CostoObraProyectadoUSD:N2}",
                    $"Comisiones de LandSmart: USD {a.GananciaInmobiliaria.TotalUSD:N2} (estructuración {proyecto.PorcentajeEstructuracion:0.##}%, " +
                    $"administración {proyecto.PorcentajeAdministracionMensual:0.##}% mensual, venta {proyecto.PorcentajeVentaUnidades:0.##}%)",
                    $"Margen para los inversores: USD {a.MargenInversoresUSD:N2} ({a.RentabilidadInversoresPorcentaje:0.##}% sobre la obra)"
                }
            };

            // ---------- Escenario tradicional ----------
            decimal tasadoUSD = Math.Round(a.ValorTasadoPesos / a.DolarMep, 2);
            if (proyecto.TipoPublicacion == TipoPublicacion.Venta)
            {
                decimal neto = Math.Round(tasadoUSD * (1 - p.ComisionVentaPorcentaje / 100), 2);
                a.ComisionTradicionalUSD = tasadoUSD - neto;
                a.Tradicional = new EscenarioAnalisis
                {
                    Nombre = "Venta tradicional",
                    ResultadoUSD = neto,
                    Detalle =
                    {
                        $"Valor tasado ${a.ValorTasadoPesos:N2} / MEP {a.DolarMep:N2} = USD {tasadoUSD:N2}",
                        $"Menos comisión de venta {p.ComisionVentaPorcentaje:0.##}%: USD {neto:N2}, cobrado hoy"
                    }
                };
            }
            else
            {
                // Alquiler en pesos ajustado por IPC: con el dólar acompañando al IPC, en USD se mantiene constante
                decimal alquilerUSD = tasadoUSD;
                decimal ingresos = Math.Round(alquilerUSD * p.HorizonteAlquilerMeses * (1 - p.ComisionAlquilerPorcentaje / 100), 2);
                decimal valorInmueble = Math.Round(alquilerUSD * 12 * 100 / p.RentabilidadAlquilerAnual, 2);
                a.ComisionTradicionalUSD = Math.Round(alquilerUSD * p.HorizonteAlquilerMeses, 2) - ingresos;
                a.Tradicional = new EscenarioAnalisis
                {
                    Nombre = "Alquiler continuo",
                    ResultadoUSD = ingresos + valorInmueble,
                    Detalle =
                    {
                        $"Alquiler ${a.ValorTasadoPesos:N2} / MEP {a.DolarMep:N2} = USD {alquilerUSD:N2} por mes",
                        $"{p.HorizonteAlquilerMeses} meses menos comisión {p.ComisionAlquilerPorcentaje:0.##}%: USD {ingresos:N2}",
                        $"Conserva el inmueble: valor estimado USD {valorInmueble:N2} (renta anual al {p.RentabilidadAlquilerAnual:0.##}%)"
                    }
                };
            }

            a.Supuestos.Add($"Valores en dólares MEP ({a.DolarMep:N2} $/USD); oficial BCRA {a.DolarOficial:N2} y blue {a.DolarBlue:N2} como referencia.");
            a.Supuestos.Add($"El dólar acompaña a la inflación (IPC promedio {a.IpcMensualPromedio:0.##}% mensual); el costo de obra sube con el ICC (promedio {a.IccMensualPromedio:0.##}% mensual).");
            a.Supuestos.Add(proyecto.TipoPublicacion == TipoPublicacion.Venta
                ? $"Venta cobrada hoy contra unidades al terminar la obra ({p.PlazoObraMeses} meses), sin descontar el valor tiempo del dinero."
                : $"Alquiler durante {p.HorizonteAlquilerMeses} meses contra unidades al terminar la obra ({p.PlazoObraMeses} meses), sin descontar el valor tiempo del dinero.");
            a.Supuestos.Add($"Valor de venta por m² de las unidades (USD {proyecto.ValorVentaM2Unidades:N2}) constante en dólares.");
            return a;
        }
    }
}
