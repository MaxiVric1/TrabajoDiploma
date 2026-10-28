using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Ventas de inmuebles publicados en venta y ranking de agentes comerciales por propiedades vendidas y valor (USD)
    public class VentaBLL
    {
        public const int TOP = 5;
        public const decimal PRECIO_MAXIMO_USD = 100000000;

        private readonly VentaDAL ventaDAL = new VentaDAL();
        private readonly InmuebleDAL inmuebleDAL = new InmuebleDAL();

        // ---------- Venta ----------

        // Un inmueble Publicado en venta pasa a Vendido; la venta se le cuenta al agente de la captación. La registran
        // Marketing (desde Inmuebles Publicados) o el Agente Comercial.
        public int RegistrarVenta(int idInmueble, DateTime fechaVenta, decimal precioUsd, string observaciones = null)
        {
            if (!SessionManager.Instance.EstaLogueado() ||
                !(SessionManager.Instance.TienePermiso(PermisosDefinidos.AccesoPN1Marketing) || SessionManager.Instance.TienePermiso(PermisosDefinidos.AccesoPN1AgenteComercial)))
                throw new UnauthorizedAccessException("Solo Marketing o un Agente Comercial pueden registrar una venta.");
            if (precioUsd <= 0 || precioUsd > PRECIO_MAXIMO_USD)
                throw new ArgumentException($"El precio de venta debe ser mayor a cero y como máximo USD {PRECIO_MAXIMO_USD:N0}.");
            if (fechaVenta.Date > DateTime.Today)
                throw new ArgumentException("La fecha de venta no puede ser posterior a hoy.");

            Inmueble inmueble = inmuebleDAL.BuscarPorId(idInmueble);
            if (inmueble == null)
                throw new Exception("El inmueble no existe.");
            if (inmueble.TipoPublicacion != TipoPublicacion.Venta)
                throw new ArgumentException("Solo se vende un inmueble publicado en venta (este es de alquiler).");
            if (inmueble.EstadoActual != EstadoInmueble.Publicado)
                throw new ArgumentException($"Solo se vende un inmueble Publicado (este está {inmueble.EstadoActual}).");
            if (inmueble.FechaPublicacion.HasValue && fechaVenta.Date < inmueble.FechaPublicacion.Value.Date)
                throw new ArgumentException($"La fecha de venta no puede ser anterior a la publicación ({inmueble.FechaPublicacion:dd/MM/yyyy}).");

            Venta venta = new Venta
            {
                IdInmueble = idInmueble,
                FechaVenta = fechaVenta.Date,
                PrecioUsd = Math.Round(precioUsd, 2),
                IdUsuarioRegistro = SessionManager.Instance.UsuarioActual.Id,
                Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim()
            };
            int id = ventaDAL.Registrar(venta);
            if (id == -1)
                throw new Exception("No se pudo registrar la venta (el inmueble ya no está publicado en venta o ya tiene una venta).");
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Venta Inmueble",
                $"Venta N° {id}: inmueble {idInmueble} ({inmueble.Direccion}) vendido el {venta.FechaVenta:dd/MM/yyyy} en USD {venta.PrecioUsd:N2}.");
            return id;
        }

        // ---------- Ranking ----------

        private static void ExigirPermisoRanking()
        {
            if (!SessionManager.Instance.EstaLogueado() ||
                !(SessionManager.Instance.TienePermiso(PermisosDefinidos.AccesoAdmin) || SessionManager.Instance.TienePermiso(PermisosDefinidos.AccesoPN3Directorio)))
                throw new UnauthorizedAccessException("Solo el Administrador o el Directorio pueden ver el ranking de agentes.");
        }

        public static string Texto(PeriodoRanking periodo)
        {
            switch (periodo)
            {
                case PeriodoRanking.EsteMes: return "Este mes";
                case PeriodoRanking.EsteTrimestre: return "Este trimestre";
                case PeriodoRanking.EsteAnio: return "Este año";
                case PeriodoRanking.Todo: return "Todo";
                default: return "Personalizado";
            }
        }

        // Fechas de un período predefinido (hasta hoy); Todo = sin límites. Personalizado no tiene fechas propias.
        public static (DateTime? Desde, DateTime? Hasta) Rango(PeriodoRanking periodo, DateTime hoy)
        {
            hoy = hoy.Date;
            switch (periodo)
            {
                case PeriodoRanking.EsteMes: return (new DateTime(hoy.Year, hoy.Month, 1), hoy);
                case PeriodoRanking.EsteTrimestre: return (new DateTime(hoy.Year, (hoy.Month - 1) / 3 * 3 + 1, 1), hoy);
                case PeriodoRanking.EsteAnio: return (new DateTime(hoy.Year, 1, 1), hoy);
                case PeriodoRanking.Todo: return (null, null);
                default: throw new ArgumentException("El período personalizado se arma con las fechas Desde y Hasta.");
            }
        }

        // Ranking del período por cantidad de propiedades o por valor total: todos los agentes de mayor a menor, o
        // filtrado a los TOP que más (o menos) vendieron. Entran todos los agentes comerciales activos, aunque no hayan
        // vendido (así aparecen en "menos vendieron").
        public ResultadoRanking ObtenerRanking(PeriodoRanking periodo, DateTime? desde, DateTime? hasta, OrdenRanking orden, VistaRanking vista)
        {
            bool masVendieron = vista != VistaRanking.MenosVendieron;
            ExigirPermisoRanking();
            if (periodo != PeriodoRanking.Personalizado)
                (desde, hasta) = Rango(periodo, DateTime.Today);
            else if (!desde.HasValue || !hasta.HasValue)
                throw new ArgumentException("Indique las fechas Desde y Hasta del período personalizado.");
            if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
                throw new ArgumentException("La fecha Desde no puede ser posterior a la fecha Hasta.");

            List<RankingAgente> todos = ventaDAL.LeerRanking(desde, hasta);
            Func<RankingAgente, decimal> principal = orden == OrdenRanking.Cantidad ? (Func<RankingAgente, decimal>)(r => r.Cantidad) : r => r.TotalUsd;
            Func<RankingAgente, decimal> secundario = orden == OrdenRanking.Cantidad ? (Func<RankingAgente, decimal>)(r => r.TotalUsd) : r => r.Cantidad;
            IOrderedEnumerable<RankingAgente> ordenados = masVendieron
                ? todos.OrderByDescending(principal).ThenByDescending(secundario)
                : todos.OrderBy(principal).ThenBy(secundario);
            IEnumerable<RankingAgente> enOrden = ordenados.ThenBy(r => r.Apellido).ThenBy(r => r.NombrePersona);
            List<RankingAgente> filas = (vista == VistaRanking.Todos ? enOrden : enOrden.Take(TOP)).ToList();
            for (int i = 0; i < filas.Count; i++) filas[i].Posicion = i + 1;

            return new ResultadoRanking
            {
                Desde = desde?.Date,
                Hasta = hasta?.Date,
                Periodo = Texto(periodo),
                Orden = orden,
                Vista = vista,
                TotalAgentes = todos.Count,
                VentasDelPeriodo = todos.Sum(r => r.Cantidad),
                TotalUsdDelPeriodo = todos.Sum(r => r.TotalUsd),
                Filas = filas
            };
        }

        public static string TextoPeriodo(ResultadoRanking r) =>
            r.Desde.HasValue || r.Hasta.HasValue
                ? $"{r.Periodo}: del {r.Desde:dd/MM/yyyy} al {r.Hasta:dd/MM/yyyy}"
                : $"{r.Periodo} (todas las ventas)";

        // Informe PDF con el mismo estilo que los demás informes del sistema
        public void GenerarPdf(ResultadoRanking r, string ruta)
        {
            ExigirPermisoRanking();
            if (r == null) throw new ArgumentNullException(nameof(r));
            if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException("Indique dónde guardar el PDF.");

            string vista = TextoVista(r.Vista);
            string criterio = r.Orden == OrdenRanking.Cantidad ? "cantidad de propiedades vendidas" : "valor total vendido (USD)";
            DocumentoPdf pdf = new DocumentoPdf("LandSmart - Ranking de agentes comerciales");
            pdf.Encabezado(ContratoBLL.Logo(), "RANKING DE AGENTES",
                r.Vista == VistaRanking.Todos ? "Todos los agentes" : r.MasVendieron ? "Más vendieron" : "Menos vendieron", $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");

            pdf.Subtitulo("Criterios");
            pdf.Campo("Período", TextoPeriodo(r));
            pdf.Campo("Ordenado por", criterio);
            pdf.Campo("Vista", vista);
            pdf.Campo("Agentes evaluados", $"{r.TotalAgentes} agente(s) comercial(es)");

            pdf.Subtitulo(vista, reservar: 160);
            List<string[]> filas = r.Filas.Select(f => new[]
            {
                f.Posicion.ToString(), f.Dni ?? "-", f.NombreCompleto, f.Cantidad.ToString(), $"USD {f.TotalUsd:N2}"
            }).ToList();
            if (filas.Count == 0) filas.Add(new[] { "-", "-", "No hay agentes comerciales", "0", "USD 0,00" });
            pdf.Tabla(new[] { 0.6f, 1.3f, 2.6f, 1.4f, 1.7f },
                new[] { "Pos.", "DNI", "Agente", "Propiedades vendidas", "Valor total (USD)" }, filas, columnasTexto: 3);

            pdf.Recuadro("TOTAL DEL PERÍODO",
                $"{r.VentasDelPeriodo} propiedad(es) vendida(s) por USD {r.TotalUsdDelPeriodo:N2} entre los {r.TotalAgentes} agentes comerciales" +
                (r.VentasDelPeriodo > 0 ? $" (promedio USD {r.TotalUsdDelPeriodo / r.VentasDelPeriodo:N2} por propiedad)." : "."));
            pdf.Parrafo("Las ventas se cuentan por fecha de venta y se asignan al agente que captó el inmueble. Los valores son los precios de " +
                        "venta registrados en dólares.", 8.5f);

            pdf.Guardar(ruta);
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Informe Ranking Agentes",
                $"Ranking de agentes ({TextoPeriodo(r)}, {criterio}, {vista.ToLower()}) exportado a PDF.");
        }

        public static string TextoVista(VistaRanking vista)
        {
            switch (vista)
            {
                case VistaRanking.MasVendieron: return $"Top {TOP}: los que más vendieron";
                case VistaRanking.MenosVendieron: return $"Top {TOP}: los que menos vendieron";
                default: return "Todos los agentes, de mayor a menor";
            }
        }
    }
}
