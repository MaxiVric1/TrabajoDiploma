using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TpIngSoft
{
    // Comportamiento de los formularios secundarios dentro del área MDI de FormMain:
    // - No pueden salir del área (ni al moverlos, ni al redimensionarlos, ni al achicar la ventana principal).
    //   Si un formulario no entra, se achica y su contenido pasa a tener scroll propio.
    // - La ruedita del mouse hace scroll del formulario que está debajo del cursor: vertical, y horizontal con
    //   Shift o con la rueda inclinable. Los controles que usan la ruedita (grillas, combos, etc.) la conservan.
    internal sealed class VentanasMdi : IMessageFilter
    {
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_MOUSEHWHEEL = 0x020E;

        // Tamaño mínimo al que se puede achicar un formulario secundario (el contenido se recorre con scroll)
        private static readonly Size TAMANIO_MINIMO = new Size(300, 200);

        private readonly Form _principal;
        private readonly MdiClient _area;
        private bool _encuadrando; // evita reentrar al mover/redimensionar desde Encuadrar

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point punto);

        private VentanasMdi(Form principal, MdiClient area)
        {
            _principal = principal;
            _area = area;
        }

        // Llamar después de IsMdiContainer = true
        public static VentanasMdi Instalar(Form principal)
        {
            MdiClient area = principal.Controls.OfType<MdiClient>().FirstOrDefault();
            if (area == null)
                throw new InvalidOperationException("El formulario principal no es un contenedor MDI.");

            var ventanas = new VentanasMdi(principal, area);
            area.ControlAdded += (s, e) => { if (e.Control is Form hijo) ventanas.Preparar(hijo); };
            area.Resize += (s, e) => ventanas.EncuadrarTodos();
            Application.AddMessageFilter(ventanas);
            principal.FormClosed += (s, e) => Application.RemoveMessageFilter(ventanas);
            return ventanas;
        }

        // ---------- Encuadre dentro del área ----------

        private void Preparar(Form hijo)
        {
            // El contenido pasa a un panel que crece con la ventana pero nunca baja de su tamaño de diseño, y el
            // formulario hace scroll sobre ese tamaño. Hacen falta las dos cosas: con AutoScrollMinSize solo, WinForms
            // igual corre los controles anclados abajo/derecha; con el panel solo (anclado), no muestra las barras.
            var contenido = new Panel
            {
                BackColor = Color.Transparent,
                Location = Point.Empty,
                Size = hijo.ClientSize,
                MinimumSize = hijo.ClientSize,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            hijo.SuspendLayout();
            contenido.Controls.AddRange(hijo.Controls.Cast<Control>().ToArray()); // mismo orden (z-order y tabulación)
            hijo.Controls.Add(contenido);
            hijo.AutoScrollMinSize = contenido.MinimumSize;
            hijo.AutoScroll = true;
            hijo.MinimumSize = new Size(Math.Min(hijo.Width, TAMANIO_MINIMO.Width), Math.Min(hijo.Height, TAMANIO_MINIMO.Height));
            hijo.ResumeLayout();

            hijo.Load += (s, e) => Encuadrar(hijo); // antes de mostrarse: ya aparece adentro del área
            hijo.Move += (s, e) => Encuadrar(hijo);
            hijo.Resize += (s, e) => Encuadrar(hijo);
        }

        private void EncuadrarTodos()
        {
            foreach (Form hijo in _principal.MdiChildren)
                Encuadrar(hijo);
        }

        // Achica el formulario si no entra en el área y lo corre para que quede completamente adentro
        private void Encuadrar(Form hijo)
        {
            if (_encuadrando || hijo.WindowState != FormWindowState.Normal) return;
            Rectangle area = _area.ClientRectangle;
            if (area.Width <= 0 || area.Height <= 0) return; // ventana principal minimizada

            _encuadrando = true;
            try
            {
                Size tamanio = new Size(Math.Min(hijo.Width, area.Width), Math.Min(hijo.Height, area.Height));
                if (tamanio != hijo.Size)
                    hijo.Size = tamanio;

                Point ubicacion = new Point(
                    Math.Max(area.Left, Math.Min(hijo.Left, area.Right - hijo.Width)),
                    Math.Max(area.Top, Math.Min(hijo.Top, area.Bottom - hijo.Height)));
                if (ubicacion != hijo.Location)
                    hijo.Location = ubicacion;
            }
            finally
            {
                _encuadrando = false;
            }
        }

        // ---------- Scroll con la ruedita ----------

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != WM_MOUSEWHEEL && m.Msg != WM_MOUSEHWHEEL) return false;
            if (Form.ActiveForm != _principal) return false; // p. ej. un diálogo modal abierto

            Control control = Control.FromChildHandle(WindowFromPoint(Cursor.Position));
            Form hijo = control?.FindForm();
            if (hijo == null || hijo.MdiParent != _principal) return false;

            // Los controles que hacen su propio scroll con la ruedita la reciben normalmente
            for (Control c = control; c != null && c != hijo; c = c.Parent)
            {
                if (UsaLaRuedita(c)) return false;
            }

            int delta = (short)((m.WParam.ToInt64() >> 16) & 0xFFFF);
            bool horizontal = m.Msg == WM_MOUSEHWHEEL || (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            // La rueda inclinable da positivo hacia la derecha; la ruedita con Shift, positivo hacia arriba (izquierda)
            if (m.Msg == WM_MOUSEHWHEEL) delta = -delta;

            return Desplazar(hijo, delta, horizontal);
        }

        private static bool UsaLaRuedita(Control c)
        {
            if (c is DataGridView || c is ListBox || c is ComboBox || c is UpDownBase || c is TreeView ||
                c is ListView || c is TrackBar || c is RichTextBox || c is WebBrowser)
                return true;
            if (c is TextBoxBase texto && texto.Multiline)
                return true;
            // Paneles internos con scroll propio
            return c is ScrollableControl s && s.AutoScroll && (s.VerticalScroll.Visible || s.HorizontalScroll.Visible);
        }

        // delta positivo = hacia arriba / izquierda (como la ruedita). Devuelve true si el formulario se desplazó.
        private static bool Desplazar(Form hijo, int delta, bool horizontal)
        {
            bool vertical = hijo.VerticalScroll.Visible;
            bool lateral = hijo.HorizontalScroll.Visible;
            // Sin barra vertical, la ruedita común desplaza en horizontal (igual que WinForms)
            if (horizontal ? !lateral : !vertical && !lateral) return false;

            // AutoScrollPosition devuelve valores negativos y se asigna con positivos
            int x = -hijo.AutoScrollPosition.X;
            int y = -hijo.AutoScrollPosition.Y;
            if (horizontal || !vertical)
                x = Math.Max(0, x - delta);
            else
                y = Math.Max(0, y - delta);

            hijo.AutoScrollPosition = new Point(x, y);
            return true;
        }
    }
}
