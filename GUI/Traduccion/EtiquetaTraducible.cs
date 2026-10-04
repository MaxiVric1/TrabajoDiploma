using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TpIngSoft.Traduccion
{
    public class EtiquetaTraducible : IControlTraducible
    {
        public string NombreControl { get; }
        private readonly Control _control;

        public EtiquetaTraducible(Control control, string nombreControl)
        {
            _control = control ?? throw new ArgumentNullException(nameof(control));
            NombreControl = nombreControl;
        }

        public void Traducir(Dictionary<string, string> traducciones)
        {
            if (traducciones.TryGetValue(NombreControl, out string texto) && !string.IsNullOrWhiteSpace(texto))
            {
                _control.Text = texto;
            }
            else
            {
                _control.Text = $"<{NombreControl}>";
            }
        }
    }
}
