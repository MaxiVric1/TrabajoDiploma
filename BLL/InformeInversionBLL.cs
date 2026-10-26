using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // PN3: informes PDF. El del Directorio (interno) resume la evaluación de un proyecto: datos, comparación de
    // escenarios y ganancia de LandSmart. El de Inversores es la propuesta de inversión de un fideicomiso aprobado.
    // Los gráficos llegan ya dibujados (PNG): los arma la interfaz con OxyPlot a partir de los mismos datos.
    public class InformeInversionBLL
    {
        // ---------- Datos ----------

        // Rentabilidad del fideicomiso para los inversores contra obligaciones negociables en dólares (valor
        // referencial) y plazo fijo bancario en dólares (BCRA), todo en % anual en dólares
        public ComparativaRentabilidad ObtenerComparativaRentabilidad(int idProyecto)
        {
            AnalisisComparativo a = new ProyectoInversionBLL().ObtenerAnalisisComparativo(idProyecto);
            int plazo = a.Parametros.PlazoObraMeses;
            ComparativaRentabilidad c = new ComparativaRentabilidad
            {
                Analisis = a,
                PlazoObraMeses = plazo,
                RentabilidadPlazoUSD = a.RentabilidadInversoresPorcentaje,
                RentabilidadAnualUSD = Anualizar(a.RentabilidadInversoresPorcentaje, plazo)
            };

            var plazoFijoUsd = new IndicesBCRABLL().TasaPlazoFijoUsd();
            if (plazoFijoUsd.HasValue)
            {
                c.PlazoFijoUsdAnual = plazoFijoUsd.Value.Tasa;
                c.FechaPlazoFijoUsd = plazoFijoUsd.Value.Fecha;
            }
            else
                a.Advertencias.Add($"Sin la tasa de plazo fijo en dólares del BCRA: se usa el valor referencial de {ComparativaRentabilidad.PLAZO_FIJO_USD_REFERENCIAL:0.##}% anual.");
            return c;
        }

        // Tasa del plazo completo (en %) llevada a tasa anual equivalente
        public static decimal Anualizar(decimal porcentajePlazo, int meses)
        {
            if (meses <= 0) return 0;
            double factor = 1 + (double)porcentajePlazo / 100;
            if (factor <= 0) return -100;
            return Math.Round((decimal)(Math.Pow(factor, 12.0 / meses) - 1) * 100, 2);
        }

        // Reparto de lo que genera el desarrollo (valor de las unidades menos el costo de obra) entre propietario,
        // inversores y LandSmart. Un margen negativo de los inversores se muestra como cero.
        public static List<(string Parte, decimal MontoUSD)> DistribucionGanancia(AnalisisComparativo a)
        {
            return new List<(string, decimal)>
            {
                ("Propietario", a.UnidadesPropietarioUSD),
                ("Inversores", Math.Max(0, a.MargenInversoresUSD)),
                ("LandSmart", a.GananciaInmobiliaria.TotalUSD)
            };
        }

        // ---------- Informe del Directorio ----------

        // graficoComparativo: barras propietario / inversores / inmobiliaria en cada escenario.
        // graficoDistribucion: torta del reparto de la ganancia del desarrollo.
        public void GenerarInformeDirectorio(AnalisisComparativo a, byte[] graficoComparativo, byte[] graficoDistribucion, string ruta)
        {
            ProyectoInversionBLL.ExigirPermiso(PermisosDefinidos.AccesoPN3Directorio, "el Directorio");
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException("Indique dónde guardar el PDF.");

            ProyectoInversion p = a.Proyecto;
            Inmueble inmueble = new InmuebleDAL().BuscarPorId(p.IdInmueble);
            Cliente propietario = inmueble != null ? new ClienteDAL().BuscarPorId(inmueble.IdCliente) : null;
            ValorICC icc = new IccBLL().Ultimo();
            GananciaInmobiliaria g = a.GananciaInmobiliaria;
            bool venta = p.TipoPublicacion == TipoPublicacion.Venta;

            DocumentoPdf pdf = new DocumentoPdf($"LandSmart - Informe de evaluación - Proyecto N° {p.IdProyecto} - Uso interno");

            // ----- Página 1: datos -----
            pdf.Encabezado(ContratoBLL.Logo(), "INFORME DE EVALUACIÓN DE INVERSIÓN", $"Proyecto N° {p.IdProyecto}", $"Generado el {a.Fecha:dd/MM/yyyy HH:mm}");
            pdf.Parrafo($"Estado: {ProyectoInversionBLL.Texto(p.Estado)} - creado el {p.FechaCreacion:dd/MM/yyyy}. Informe para el Directorio.", 9);

            pdf.Subtitulo("Inmueble");
            pdf.Campo("Dirección", $"{p.Direccion}" + (string.IsNullOrEmpty(p.Localidad) ? "" : $", {p.Localidad}"));
            pdf.Campo("Tipo y operación", $"{p.TipoInmueble} en {(venta ? "venta" : "alquiler")}");
            if (inmueble != null)
                pdf.Campo("Superficie", $"{inmueble.MetrosCuadrados:N2} m² cubiertos, {inmueble.CantidadAmbientes} ambientes");
            pdf.Campo(venta ? "Valor tasado" : "Alquiler tasado", $"${a.ValorTasadoPesos:N2}" + (venta ? "" : " por mes") +
                      $" (USD {a.ValorTasadoPesos / a.DolarMep:N2} al MEP)");
            if (p.Latitud.HasValue && p.Longitud.HasValue)
                pdf.Campo("Coordenadas", $"{p.Latitud:0.000000}, {p.Longitud:0.000000}");

            pdf.Subtitulo("Propietario");
            pdf.Campo("Nombre", propietario != null ? $"{propietario.Nombre} {propietario.Apellido}" : "-");
            pdf.Campo("DNI", propietario?.Dni);
            pdf.Campo("Contacto", propietario == null ? "-" : string.Join(" - ", new[] { propietario.Telefono, propietario.Email }.Where(x => !string.IsNullOrWhiteSpace(x))));

            pdf.Subtitulo("Datos técnicos (Arquitectura)");
            pdf.Campo("Superficie del lote", $"{p.SuperficieLote:N2} m²");
            pdf.Campo("Superficie edificable", $"{p.SuperficieEdificablePlanta:N2} m² por planta");
            pdf.Campo("Altura máxima", $"{p.AlturaMaxima:0.##} m: {p.Pisos} pisos de {ParcelaCaba.ALTURA_POR_PISO:0} m");
            pdf.Campo("M² construibles", $"{p.MetrosConstruibles:N2} m² ({p.Pisos} pisos x {p.SuperficieEdificablePlanta:N2} m²)");
            pdf.Campo("SMP / distrito", p.Smp == null && p.DistritoUrbanistico == null ? "Carga manual (sin consulta a catastro)"
                : $"{p.Smp ?? "-"} / {p.DistritoUrbanistico ?? "-"} (Código Urbanístico de CABA, EPOK)");
            pdf.Campo("Costo de obra", $"USD {p.CostoObraM2:N2} por m² - total USD {p.CostoTotalObra:N2}");
            pdf.Campo("Proyectado con el ICC", $"USD {a.CostoObraProyectadoUSD:N2} (obra de {a.Parametros.PlazoObraMeses} meses)");
            pdf.Campo("Valor de las unidades", $"USD {p.ValorVentaM2Unidades:N2} por m² - total USD {a.ValorUnidadesUSD:N2}");
            pdf.Campo("Canje del propietario", $"{p.PorcentajeCanje:0.##}% de las unidades (USD {a.UnidadesPropietarioUSD:N2})");

            pdf.Subtitulo("Tipos de cambio e índices del día");
            pdf.Campo("Dólar MEP (DolarAPI)", $"${a.DolarMep:N2} - el usado para convertir");
            pdf.Campo("Dólar blue (DolarAPI)", a.DolarBlue.HasValue ? $"${a.DolarBlue:N2}" : "Sin datos");
            pdf.Campo("Dólar oficial (BCRA)", a.DolarOficial.HasValue ? $"${a.DolarOficial:N2} al {a.FechaDolarOficial:dd/MM/yyyy}" : "Sin datos");
            pdf.Campo("IPC (BCRA)", $"{a.IpcMensualPromedio:0.##}% mensual promedio (últimos {a.Parametros.MesesPromedioIndices} meses)");
            pdf.Campo("ICC (INDEC)", (icc != null ? $"{icc.Periodo:MM/yyyy}: índice {icc.ValorIndice:N2}, {icc.VariacionMensual:+0.##;-0.##}% mensual. " : "") +
                                     $"Promedio {a.IccMensualPromedio:0.##}% mensual");

            // ----- Página 2: análisis comparativo -----
            pdf.SaltoPagina();
            pdf.Titulo("Análisis comparativo");
            pdf.Parrafo($"Escenario A: {a.Tradicional.Nombre.ToLower()} de la propiedad. Escenario B: el propietario aporta el lote a un " +
                        $"fideicomiso que construye {p.MetrosConstruibles:N2} m² y recibe el {p.PorcentajeCanje:0.##}% de las unidades. Montos en USD.", 9.5f);
            pdf.Espacio(6);
            pdf.Tabla(new[] { 2.2f, 1.6f, 1.6f },
                new[] { "Concepto", "A: " + a.Tradicional.Nombre, "B: " + a.Fideicomiso.Nombre },
                new[]
                {
                    new[] { "Propietario obtiene", Usd(a.Tradicional.ResultadoUSD), Usd(a.Fideicomiso.ResultadoUSD) },
                    new[] { "Cuándo", venta ? "Hoy" : $"En {a.Parametros.HorizonteAlquilerMeses} meses", $"Al terminar la obra ({a.Parametros.PlazoObraMeses} meses)" },
                    new[] { "Inversores ganan", "-", Usd(a.MargenInversoresUSD) + $" ({a.RentabilidadInversoresPorcentaje:0.##}%)" },
                    new[] { "LandSmart gana", Usd(a.ComisionTradicionalUSD), Usd(g.TotalUSD) },
                    new[] { "Diferencia para el propietario", "", (a.DiferenciaUSD >= 0 ? "+" : "") + Usd(a.DiferenciaUSD) }
                }, negritaUltima: true);
            pdf.Parrafo("Detalle A: " + string.Join("; ", a.Tradicional.Detalle) + ".", 8.5f);
            pdf.Espacio(3);
            pdf.Parrafo("Detalle B: " + string.Join("; ", a.Fideicomiso.Detalle) + ".", 8.5f);
            pdf.Espacio(8);
            pdf.Parrafo("Ganancia por parte en cada escenario (USD)", 10, true, centrado: true);
            pdf.Imagen(graficoComparativo, 440);
            pdf.Recuadro("RECOMENDACIÓN: " + (a.ConvieneFideicomiso ? "DESARROLLO EN FIDEICOMISO" : a.Tradicional.Nombre.ToUpper()),
                (a.ConvieneFideicomiso
                    ? $"El propietario obtiene USD {a.DiferenciaUSD:N2} más con el fideicomiso que con {a.Tradicional.Nombre.ToLower()}"
                    : $"El propietario obtiene USD {-a.DiferenciaUSD:N2} más con {a.Tradicional.Nombre.ToLower()} que con el fideicomiso") +
                $"; los inversores ganan USD {a.MargenInversoresUSD:N2} ({a.RentabilidadInversoresPorcentaje:0.##}% sobre la obra) y LandSmart USD {g.TotalUSD:N2}." +
                (a.MargenInversoresUSD <= 0 ? " ATENCIÓN: el margen de los inversores no es positivo: el fideicomiso no conseguiría fondeo." : ""));
            foreach (string advertencia in a.Advertencias)
                pdf.Parrafo("Atención: " + advertencia, 8.5f);

            // ----- Página 3: ganancia de la inmobiliaria -----
            pdf.SaltoPagina();
            pdf.Titulo("Ganancia de la inmobiliaria");
            pdf.Parrafo("Comisiones de LandSmart sobre el fideicomiso, con los porcentajes definidos para este proyecto. Montos en USD: " +
                        "la estructuración sobre el costo de obra proyectado con el ICC y la administración sobre el capital total del " +
                        "fideicomiso, todos los meses de obra.", 9.5f);
            pdf.Espacio(6);
            pdf.Tabla(new[] { 1.6f, 2.4f, 1.1f, 1.3f },
                new[] { "Comisión", "Base", "Porcentaje", "Monto" },
                new[]
                {
                    new[] { "Estructuración", $"Costo de obra {Usd(g.BaseEstructuracionUSD)}", $"{g.PorcentajeEstructuracion:0.##}%", Usd(g.EstructuracionUSD) },
                    new[] { "Administración mensual", $"Capital total del fideicomiso {Usd(g.BaseAdministracionUSD)} durante {g.MesesObra} meses",
                            $"{g.PorcentajeAdministracionMensual:0.##}% mensual", $"{Usd(g.AdministracionMensualUSD)} x {g.MesesObra} = {Usd(g.AdministracionUSD)}" },
                    new[] { "Venta de unidades", $"Unidades de los inversores {Usd(g.BaseVentaUSD)}", $"{g.PorcentajeVentaUnidades:0.##}%", Usd(g.VentaUSD) },
                    new[] { "Total", "", "", Usd(g.TotalUSD) }
                }, negritaUltima: true, columnasTexto: 2);
            pdf.Espacio(4);
            decimal totalReparto = DistribucionGanancia(a).Sum(x => x.MontoUSD);
            pdf.Parrafo($"Distribución de la ganancia del desarrollo (USD {totalReparto:N2})", 10, true, centrado: true);
            pdf.Imagen(graficoDistribucion, 360);
            pdf.Parrafo(string.Join("   ", DistribucionGanancia(a).Select(x =>
                $"{x.Parte}: {Usd(x.MontoUSD)} ({(totalReparto > 0 ? x.MontoUSD * 100 / totalReparto : 0):0.#}%)")), 9, centrado: true);
            pdf.Recuadro($"GANANCIA TOTAL DE LANDSMART: {Usd(g.TotalUSD)}",
                $"Equivale al {(a.ValorUnidadesUSD > 0 ? g.TotalUSD * 100 / a.ValorUnidadesUSD : 0):0.##}% del valor de las unidades. " +
                $"Escenario A ({a.Tradicional.Nombre.ToLower()}): {Usd(a.ComisionTradicionalUSD)}, " +
                $"{(g.TotalUSD >= a.ComisionTradicionalUSD ? "USD " + (g.TotalUSD - a.ComisionTradicionalUSD).ToString("N2") + " más con el fideicomiso" : "USD " + (a.ComisionTradicionalUSD - g.TotalUSD).ToString("N2") + " menos con el fideicomiso")}.", 11);

            pdf.Guardar(ruta);
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Informe Directorio",
                $"Proyecto N° {p.IdProyecto}: informe de evaluación generado.");
        }

        // ---------- Propuesta para inversores ----------

        // graficoRentabilidad: barras fideicomiso / ON / plazo fijo en dólares (% anual en USD). graficoParticipacion: torta de los aportes.
        public void GenerarInformeInversores(ResumenFideicomiso r, ComparativaRentabilidad c, byte[] graficoRentabilidad, byte[] graficoParticipacion, string ruta)
        {
            ProyectoInversionBLL.ExigirPermiso(PermisosDefinidos.AccesoPN3Inversiones, "Inversiones");
            if (r == null) throw new ArgumentNullException(nameof(r));
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException("Indique dónde guardar el PDF.");
            ProyectoInversion p = r.Proyecto;
            if (p.Estado != EstadoProyecto.Aprobado)
                throw new ArgumentException("Solo se presenta a inversores un proyecto aprobado por el Directorio.");
            AnalisisComparativo a = c.Analisis;

            DocumentoPdf pdf = new DocumentoPdf($"LandSmart - Propuesta de inversión - Proyecto N° {p.IdProyecto}");

            // ----- Página 1: el proyecto -----
            pdf.Encabezado(ContratoBLL.Logo(), "PROPUESTA DE INVERSIÓN", $"Proyecto N° {p.IdProyecto}", $"{DateTime.Now:dd/MM/yyyy}");
            pdf.Subtitulo("El proyecto");
            pdf.Campo("Ubicación", $"{p.Direccion}" + (string.IsNullOrEmpty(p.Localidad) ? "" : $", {p.Localidad}"));
            pdf.Campo("Lote", $"{p.SuperficieLote:N2} m²");
            pdf.Campo("Edificabilidad", $"{p.Pisos} pisos (altura máxima {p.AlturaMaxima:0.##} m) de {p.SuperficieEdificablePlanta:N2} m² por planta");
            pdf.Campo("Superficie a construir", $"{p.MetrosConstruibles:N2} m²");
            pdf.Campo("Costo de obra", $"USD {p.CostoObraM2:N2} por m² - USD {p.CostoTotalObra:N2} en total");
            pdf.Campo("Valor de las unidades", $"USD {p.ValorVentaM2Unidades:N2} por m² - USD {p.ValorTotalUnidades:N2} en total");
            pdf.Campo("Plazo estimado de obra", $"{c.PlazoObraMeses} meses");
            pdf.Campo("Aprobado por el Directorio", p.FechaDecision.HasValue ? p.FechaDecision.Value.ToString("dd/MM/yyyy") : "-");

            pdf.Subtitulo("Resumen ejecutivo del fideicomiso");
            pdf.Parrafo($"El propietario aporta el lote de {p.Direccion} a un fideicomiso inmobiliario administrado por LandSmart, a cambio del " +
                        $"{p.PorcentajeCanje:0.##}% de las unidades terminadas. Los inversores aportan el capital de la obra " +
                        $"(USD {r.CapitalNecesario:N2}) y reciben, en proporción a su aporte, las unidades restantes o el producido de su venta.");
            pdf.Espacio(4);
            pdf.Parrafo($"Con un costo de obra proyectado de USD {a.CostoObraProyectadoUSD:N2} (ajustado por el índice del costo de la construcción del INDEC) " +
                        $"y descontadas las comisiones de LandSmart (estructuración, administración y venta), la ganancia estimada para los inversores es de USD {a.MargenInversoresUSD:N2}: " +
                        $"un {c.RentabilidadPlazoUSD:0.##}% en dólares en {c.PlazoObraMeses} meses ({c.RentabilidadAnualUSD:0.##}% anual).");
            pdf.Espacio(6);
            pdf.Campo("Capital necesario", Usd(r.CapitalNecesario));
            pdf.Campo("Capital suscripto", $"{Usd(r.CapitalSuscripto)} ({r.PorcentajeFondeado:0.##}%, {r.Suscripciones.Select(s => s.IdInversor).Distinct().Count()} inversor(es))");
            pdf.Campo("Capital disponible", Usd(r.CapitalRestante));
            pdf.Campo("Rentabilidad estimada", $"{c.RentabilidadPlazoUSD:0.##}% en dólares al terminar la obra ({c.RentabilidadAnualUSD:0.##}% anual)");

            // ----- Página 2: la oportunidad -----
            pdf.SaltoPagina();
            pdf.Titulo("La oportunidad");
            pdf.Parrafo("Rentabilidad anual estimada del fideicomiso frente a las alternativas habituales del ahorrista en dólares, en % anual en dólares.", 9.5f, centrado: true);
            pdf.Espacio(4);
            pdf.Imagen(graficoRentabilidad, 440);
            pdf.Campo("Fideicomiso", $"{c.RentabilidadAnualUSD:0.##}% anual en dólares ({c.RentabilidadPlazoUSD:0.##}% en los {c.PlazoObraMeses} meses de obra)");
            pdf.Campo("Obligaciones negociables", $"{c.ObligacionesNegociablesAnualUSD:0.##}% anual en dólares: valor referencial de las ON corporativas " +
                                                  "argentinas en dólares (rinden aproximadamente entre 6% y 8% anual)");
            pdf.Campo("Plazo fijo en dólares", c.PlazoFijoUsdReferencial
                ? $"{c.PlazoFijoUsdAnual:0.##}% anual en dólares: valor referencial del plazo fijo bancario en dólares (los bancos pagan " +
                  "aproximadamente entre 0,5% y 1% anual)"
                : $"{c.PlazoFijoUsdAnual:0.##}% anual en dólares: tasa de plazo fijo en dólares de personas humanas (BCRA al {c.FechaPlazoFijoUsd:dd/MM/yyyy})");
            pdf.Recuadro($"POR CADA USD {ComparativaRentabilidad.MONTO_EJEMPLO:N0} INVERTIDOS",
                $"Recibe un estimado de {Usd(c.RetornoPorMontoEjemploUSD)} al terminar la obra, en {c.PlazoObraMeses} meses: " +
                $"una ganancia de {Usd(c.GananciaPorMontoEjemploUSD)} ({c.RentabilidadPlazoUSD:0.##}% en dólares). " +
                $"Participación: {ComparativaRentabilidad.MONTO_EJEMPLO * 100 / Math.Max(1, r.CapitalNecesario):0.##}% del fideicomiso.", 11);
            pdf.Parrafo($"Supuestos: valores en dólares MEP (${a.DolarMep:N2} por USD). Costo de obra ajustado por el ICC del INDEC (promedio " +
                        $"{a.IccMensualPromedio:0.##}% mensual) frente a un dólar que acompaña a la inflación (IPC promedio {a.IpcMensualPromedio:0.##}% mensual). " +
                        $"Unidades vendidas a USD {p.ValorVentaM2Unidades:N2} por m² al terminar la obra. Comparación en dólares: el rendimiento de las " +
                        $"obligaciones negociables ({c.ObligacionesNegociablesAnualUSD:0.##}% anual) es un valor referencial, no una cotización del día (no hay " +
                        "una fuente pública que lo publique); " + (c.PlazoFijoUsdReferencial
                            ? $"el del plazo fijo en dólares ({c.PlazoFijoUsdAnual:0.##}% anual) también es referencial, porque no se pudo consultar al BCRA. "
                            : $"el del plazo fijo en dólares es la tasa nominal anual que publica el BCRA para personas humanas. ") + "Los rendimientos " +
                        "pasados no garantizan los futuros; la ganancia del fideicomiso es una estimación sujeta a los costos y precios de venta reales.", 8);

            // ----- Página 3: estado del fideicomiso -----
            pdf.SaltoPagina();
            pdf.Titulo("Estado del fideicomiso");
            pdf.BarraProgreso(r.PorcentajeFondeado, $"Fondeo: {Usd(r.CapitalSuscripto)} de {Usd(r.CapitalNecesario)}");
            pdf.Parrafo("Participación de los inversores", 10, true, centrado: true);
            pdf.Imagen(graficoParticipacion, 330);
            List<string[]> filas = r.Suscripciones.Select(s => new[]
            {
                s.NombreInversor, Usd(s.MontoAportado), $"{s.PorcentajeParticipacion:0.##}%", s.UnidadesAsignadas.ToString(), s.FechaSuscripcion.ToString("dd/MM/yyyy")
            }).ToList();
            filas.Add(new[] { "Total suscripto", Usd(r.CapitalSuscripto), $"{r.PorcentajeFondeado:0.##}%", r.Suscripciones.Sum(s => s.UnidadesAsignadas).ToString(), "" });
            pdf.Tabla(new[] { 2.4f, 1.5f, 1.1f, 0.9f, 1.1f }, new[] { "Inversor", "Aporte", "Participación", "Unidades", "Fecha" }, filas, negritaUltima: true);
            pdf.Recuadro($"CAPITAL DISPONIBLE: {Usd(r.CapitalRestante)}",
                r.Fondeado ? "El fideicomiso ya tiene suscripto todo el capital necesario."
                    : $"Falta suscribir el {100 - r.PorcentajeFondeado:0.##}% del capital (${r.CapitalRestante * a.DolarMep:N2} al dólar MEP de hoy). " +
                      "Para sumarse, contacte al área de Inversiones de LandSmart.", 11);

            pdf.Guardar(ruta);
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Informe Inversores",
                $"Proyecto N° {p.IdProyecto}: propuesta de inversión generada ({r.PorcentajeFondeado:0.##}% fondeado).");
        }

        private static string Usd(decimal valor) => $"USD {valor:N2}";
    }
}
