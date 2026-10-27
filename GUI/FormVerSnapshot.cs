using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // Actas de Decisión (snapshots JSON de las decisiones del Directorio) del inmueble del proyecto: las de todos sus
    // proyectos, aprobados y rechazados. Deserializa la elegida y muestra el análisis tal como estaba al momento de
    // decidir (con la fecha y quién decidió bien a la vista)
    public partial class FormVerSnapshot : Form, IObservador
    {
        private class ItemSnapshot
        {
            public SnapshotDecision Snapshot { get; set; }
            public string Ruta { get; set; }
            public string Error { get; set; }

            public override string ToString() => Snapshot == null
                ? $"(no se pudo leer) {Path.GetFileName(Ruta)}"
                : $"{Snapshot.FechaDecision:dd/MM/yyyy HH:mm:ss} - Proyecto N° {Snapshot.Proyecto?.Id} - {Snapshot.Decision} - {Snapshot.Usuario?.NombreCompleto}";
        }

        private readonly SnapshotBLL snapshotBLL = new SnapshotBLL();
        private readonly List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private readonly int _idProyecto;
        private readonly string _proyecto;

        public FormVerSnapshot(int idProyecto, string proyecto)
        {
            InitializeComponent();
            _idProyecto = idProyecto;
            _proyecto = proyecto;
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormVerSnapshot"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblSnapshots, BE.NombreControl.FormVerSnapshot_lblSnapshots));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDetalle, BE.NombreControl.FormVerSnapshot_lblDetalle));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCerrar, BE.NombreControl.FormVerSnapshot_btnCerrar));
            GestorIdioma.Instancia.Adjuntar(this);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles) control.Traducir(traducciones);
            Text += $" - {_proyecto}";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormVerSnapshot_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (string ruta in snapshotBLL.ListarArchivosDelInmueble(_idProyecto))
                {
                    ItemSnapshot item = new ItemSnapshot { Ruta = ruta };
                    try { item.Snapshot = snapshotBLL.Leer(ruta); }
                    catch (Exception ex) { item.Error = ex.Message; }
                    lstSnapshots.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Limpiar("No se pudieron leer las Actas de Decisión: " + ex.Message);
                return;
            }
            if (lstSnapshots.Items.Count == 0)
                Limpiar($"No hay Actas de Decisión del inmueble del Proyecto N° {_idProyecto} (se guardan cuando el Directorio aprueba o rechaza).");
            else
                lstSnapshots.SelectedIndex = 0;
        }

        private void Limpiar(string mensaje)
        {
            lblDecisionValor.Text = mensaje;
            lblDecisionValor.ForeColor = PN3Comun.AMARILLO;
            lblQuienValor.Text = lblMotivoValor.Text = lblArchivoValor.Text = "";
            txtDetalle.Clear();
        }

        private void lstSnapshots_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(lstSnapshots.SelectedItem is ItemSnapshot item)) return;
            lblArchivoValor.Text = item.Ruta;
            if (item.Snapshot == null)
            {
                Limpiar(item.Error);
                lblArchivoValor.Text = item.Ruta;
                return;
            }
            SnapshotDecision s = item.Snapshot;
            bool aprobado = s.Decision == EstadoProyecto.Aprobado.ToString();
            lblDecisionValor.Text = $"{s.Decision.ToUpper()} - Proyecto N° {s.Proyecto?.Id}";
            lblDecisionValor.ForeColor = aprobado ? PN3Comun.VERDE : PN3Comun.ROJO;
            lblQuienValor.Text = $"Decidió {s.Usuario?.NombreCompleto} (usuario {s.Usuario?.Login}) el {s.FechaDecision:dd/MM/yyyy} a las {s.FechaDecision:HH:mm:ss}";
            lblMotivoValor.Text = string.IsNullOrWhiteSpace(s.Motivo) ? "Sin observaciones." : (aprobado ? "Observaciones: " : "Motivo: ") + s.Motivo;
            txtDetalle.Text = Detalle(s);
        }

        // El análisis guardado, por secciones
        private static string Detalle(SnapshotDecision s)
        {
            StringBuilder sb = new StringBuilder();
            Action<string> titulo = t => sb.AppendLine().AppendLine(t.ToUpper()).AppendLine(new string('-', t.Length));
            Action<string, object> campo = (rotulo, v) => sb.AppendLine($"  {rotulo,-34} {(v == null || (v is string x && x.Length == 0) ? "-" : v)}");
            Func<decimal?, string> usd = v => v.HasValue ? $"USD {v:N2}" : null;

            titulo("Proyecto");
            campo("Número", s.Proyecto?.Id);
            campo("Dirección", $"{s.Proyecto?.Direccion}, {s.Proyecto?.Localidad}");
            campo("Inmueble / operación", $"{s.Proyecto?.TipoInmueble} en {s.Proyecto?.Operacion?.ToLower()}");
            campo("SMP / distrito", $"{s.Proyecto?.Smp ?? "-"} / {s.Proyecto?.Distrito ?? "-"}");
            campo("Canje del propietario", $"{s.Proyecto?.PorcentajeCanje:0.##}%");

            SnapshotDatosTecnicos d = s.DatosTecnicos ?? new SnapshotDatosTecnicos();
            titulo("Datos técnicos");
            campo("Superficie del lote", d.SuperficieLote.HasValue ? $"{d.SuperficieLote:N2} m²" : null);
            campo("Superficie edificable en planta", d.SuperficieEdificablePlanta.HasValue ? $"{d.SuperficieEdificablePlanta:N2} m²" : null);
            campo("Altura máxima", d.AlturaMaxima.HasValue ? $"{d.AlturaMaxima:0.##} m ({d.Pisos} pisos)" : null);
            campo("M² construibles", d.MetrosConstruibles.HasValue ? $"{d.MetrosConstruibles:N2} m²" : null);
            campo("Costo de obra", $"{usd(d.CostoObraM2)} por m² - {usd(d.CostoTotalObra)}");
            campo("Costo de obra proyectado (ICC)", usd(d.CostoObraProyectadoUsd));
            campo("Valor de las unidades", $"{usd(d.ValorVentaM2Unidades)} por m² - {usd(d.ValorTotalUnidades)}");

            SnapshotMercado m = s.Mercado ?? new SnapshotMercado();
            titulo("Tipos de cambio e índices del momento");
            campo("Dólar oficial (BCRA)", m.DolarOficial.HasValue ? $"$ {m.DolarOficial:N2} al {m.FechaDolarOficial:dd/MM/yyyy}" : null);
            campo("Dólar MEP (DolarAPI)", $"$ {m.DolarMep:N2}");
            campo("Dólar blue (DolarAPI)", m.DolarBlue.HasValue ? $"$ {m.DolarBlue:N2}" : null);
            campo("IPC (BCRA)", $"{m.IpcMensualPromedio:0.##}% mensual promedio");
            campo("ICC (INDEC)", $"{m.IccMensualPromedio:0.##}% mensual promedio" +
                (m.IccUltimoPeriodo.HasValue ? $"; {m.IccUltimoPeriodo:MM/yyyy}: {m.IccUltimoIndice:N2} ({m.IccUltimaVariacion:+0.##;-0.##}%)" : ""));
            campo("Período de los índices", m.PeriodoIndices);

            SnapshotAnalisis a = s.Analisis ?? new SnapshotAnalisis();
            titulo($"Análisis comparativo ({a.FechaAnalisis:dd/MM/yyyy HH:mm})");
            campo("Valor tasado", $"$ {a.ValorTasadoPesos:N2}");
            campo("Plazo de obra", $"{a.PlazoObraMeses} meses");
            foreach (var (rotulo, e) in new[] { ("Escenario A", a.EscenarioA), ("Escenario B", a.EscenarioB) })
            {
                if (e == null) continue;
                campo(rotulo, $"{e.Nombre}: {usd(e.ResultadoUsd)}");
                foreach (string linea in e.Detalle) sb.AppendLine($"      · {linea}");
            }
            campo("Diferencia para el propietario", $"{(a.DiferenciaUsd >= 0 ? "+" : "")}{usd(a.DiferenciaUsd)}");
            campo("Recomendación", a.ConvieneFideicomiso ? "Desarrollo en fideicomiso" : a.EscenarioA?.Nombre);
            campo("Unidades del propietario", usd(a.UnidadesPropietarioUsd));
            campo("Margen de los inversores", $"{usd(a.MargenInversoresUsd)} ({a.RentabilidadInversoresPorcentaje:0.##}% sobre la obra)");
            campo("Comisión en el escenario A", usd(a.ComisionTradicionalUsd));

            SnapshotComisiones c = s.Comisiones ?? new SnapshotComisiones();
            titulo("Comisiones configuradas");
            campo("Estructuración", $"{c.EstructuracionPorcentaje:0.##}%");
            campo("Administración", $"{c.AdministracionMensualPorcentaje:0.##}% mensual");
            campo("Venta de unidades", $"{c.VentaUnidadesPorcentaje:0.##}%");

            if (s.GananciaInmobiliaria != null)
            {
                SnapshotGanancia g = s.GananciaInmobiliaria;
                titulo("Ganancia de la inmobiliaria");
                campo("Estructuración", $"{usd(g.EstructuracionUsd)} (sobre {usd(g.BaseEstructuracionUsd)})");
                campo("Administración", $"{usd(g.AdministracionMensualUsd)} x {g.MesesObra} meses = {usd(g.AdministracionUsd)}");
                campo("Venta de unidades", $"{usd(g.VentaUsd)} (sobre {usd(g.BaseVentaUsd)})");
                campo("Total", usd(g.TotalUsd));
            }

            if (a.Advertencias.Count > 0)
            {
                titulo("Advertencias");
                foreach (string x in a.Advertencias) sb.AppendLine("  · " + x);
            }
            titulo("Supuestos");
            foreach (string x in a.Supuestos) sb.AppendLine("  · " + x);
            return sb.ToString().TrimStart();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
