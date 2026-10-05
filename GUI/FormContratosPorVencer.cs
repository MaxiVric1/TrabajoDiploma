using System.Collections.Generic;
using System.Windows.Forms;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormContratosPorVencer : Form, IObservador
    {
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();

        public FormContratosPorVencer()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormContratosPorVencer"));
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }
    }
}
