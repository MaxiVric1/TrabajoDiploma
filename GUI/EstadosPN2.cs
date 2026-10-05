using System.Drawing;
using System.Windows.Forms;
using BE;

namespace TpIngSoft
{
    // Presentación de los estados del PN2, compartida por sus formularios
    internal static class EstadosPN2
    {
        public static readonly Color FONDO_POR_VENCER = Color.FromArgb(214, 180, 60);   // amarillo
        public static readonly Color FONDO_CON_DEUDA = Color.FromArgb(170, 50, 60);     // rojo

        public static string Texto(EstadoContrato estado)
        {
            return estado == EstadoContrato.PorVencer ? "Por Vencer" : estado.ToString();
        }

        public static string Texto(EstadoPago estado)
        {
            switch (estado)
            {
                case EstadoPago.AlDia: return "Al día";
                case EstadoPago.EnMora: return "En Mora";
                default: return "En Litigio";
            }
        }

        // Colorea una fila de grilla: rojo (deuda) tiene prioridad sobre amarillo (por vencer)
        public static void Resaltar(DataGridViewRow fila, bool conDeuda, bool porVencer)
        {
            if (conDeuda)
            {
                fila.DefaultCellStyle.BackColor = FONDO_CON_DEUDA;
                fila.DefaultCellStyle.ForeColor = Color.White;
                fila.DefaultCellStyle.SelectionBackColor = ControlPaint.Light(FONDO_CON_DEUDA);
                fila.DefaultCellStyle.SelectionForeColor = Color.White;
            }
            else if (porVencer)
            {
                fila.DefaultCellStyle.BackColor = FONDO_POR_VENCER;
                fila.DefaultCellStyle.ForeColor = Color.Black;
                fila.DefaultCellStyle.SelectionBackColor = ControlPaint.Light(FONDO_POR_VENCER);
                fila.DefaultCellStyle.SelectionForeColor = Color.Black;
            }
        }
    }
}
