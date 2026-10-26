using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BE;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // Análisis comparativo de un proyecto: escenario tradicional (venta o alquiler), fideicomiso, datos de mercado,
    // diferencia con la recomendación y el mapa del inmueble. Lo usan Decisión del Directorio y Ver Análisis.
    public partial class PanelAnalisis : UserControl
    {
        public PanelAnalisis()
        {
            InitializeComponent();
        }

        // Controles con texto traducible; los registra el formulario que contiene el panel
        public IEnumerable<IControlTraducible> Traducibles()
        {
            yield return new EtiquetaTraducible(grpEscenarioA, BE.NombreControl.PanelAnalisis_grpEscenarioA);
            yield return new EtiquetaTraducible(grpEscenarioB, BE.NombreControl.PanelAnalisis_grpEscenarioB);
            yield return new EtiquetaTraducible(grpMercado, BE.NombreControl.PanelAnalisis_grpMercado);
            yield return new EtiquetaTraducible(lblMep, BE.NombreControl.PanelAnalisis_lblMep);
            yield return new EtiquetaTraducible(lblBlue, BE.NombreControl.PanelAnalisis_lblBlue);
            yield return new EtiquetaTraducible(lblOficial, BE.NombreControl.PanelAnalisis_lblOficial);
            yield return new EtiquetaTraducible(lblIcc, BE.NombreControl.PanelAnalisis_lblIcc);
            yield return new EtiquetaTraducible(lblIpc, BE.NombreControl.PanelAnalisis_lblIpc);
            yield return new EtiquetaTraducible(grpConclusion, BE.NombreControl.PanelAnalisis_grpConclusion);
        }

        public void Limpiar(string mensaje = "-")
        {
            txtEscenarioA.Clear();
            txtEscenarioB.Clear();
            txtSupuestos.Clear();
            lblResultadoA.Text = lblResultadoB.Text = lblMepValor.Text = lblBlueValor.Text = lblOficialValor.Text = lblIccValor.Text = lblIpcValor.Text = "-";
            lblDiferenciaValor.Text = mensaje;
            lblDiferenciaValor.ForeColor = Color.White;
            PN3Comun.CargarMapa(picMapa, null, null);
        }

        public void Mostrar(AnalisisComparativo a, ValorICC iccActual)
        {
            txtEscenarioA.Text = string.Join(Environment.NewLine, a.Tradicional.Detalle);
            lblResultadoA.Text = $"{a.Tradicional.Nombre}: {PN3Comun.Usd(a.Tradicional.ResultadoUSD)}";
            txtEscenarioB.Text = string.Join(Environment.NewLine, a.Fideicomiso.Detalle);
            lblResultadoB.Text = $"{a.Fideicomiso.Nombre}: {PN3Comun.Usd(a.Fideicomiso.ResultadoUSD)}";
            lblResultadoA.ForeColor = a.ConvieneFideicomiso ? Color.White : PN3Comun.VERDE;
            lblResultadoB.ForeColor = a.ConvieneFideicomiso ? PN3Comun.VERDE : Color.White;

            lblMepValor.Text = $"$ {a.DolarMep:N2} (DolarAPI)";
            lblBlueValor.Text = a.DolarBlue.HasValue ? $"$ {a.DolarBlue:N2} (DolarAPI)" : "-";
            lblOficialValor.Text = a.DolarOficial.HasValue ? $"$ {a.DolarOficial:N2} al {a.FechaDolarOficial:dd/MM/yyyy}" : "-";
            lblIccValor.Text = iccActual != null
                ? $"{iccActual.Periodo:MM/yyyy}: {iccActual.ValorIndice:N2} ({iccActual.VariacionMensual:+0.##;-0.##}% mensual; promedio {a.IccMensualPromedio:0.##}%)"
                : $"promedio {a.IccMensualPromedio:0.##}% mensual";
            lblIpcValor.Text = $"promedio {a.IpcMensualPromedio:0.##}% mensual (últimos {a.Parametros.MesesPromedioIndices} meses)";

            lblDiferenciaValor.Text = a.ConvieneFideicomiso
                ? $"Conviene el fideicomiso: USD {a.DiferenciaUSD:N2} más para el propietario"
                : $"Conviene {a.Tradicional.Nombre.ToLower()}: USD {-a.DiferenciaUSD:N2} más para el propietario";
            lblDiferenciaValor.ForeColor = PN3Comun.VERDE;
            txtSupuestos.Text = string.Join(Environment.NewLine,
                a.Advertencias.Select(x => "ATENCIÓN: " + x).Concat(a.Supuestos).Concat(new[] {
                    $"Inversores: margen {PN3Comun.Usd(a.MargenInversoresUSD)} ({a.RentabilidadInversoresPorcentaje:0.##}% sobre la obra proyectada)." }));

            PN3Comun.CargarMapa(picMapa, a.Proyecto.Latitud, a.Proyecto.Longitud);
        }
    }
}
