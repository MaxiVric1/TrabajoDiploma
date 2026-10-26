using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // Análisis comparativo de un proyecto (solo lectura), abierto desde Identificar Oportunidades
    public partial class FormVerAnalisis : Form, IObservador
    {
        private readonly int _idProyecto;
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();

        public FormVerAnalisis(int idProyecto)
        {
            InitializeComponent();
            _idProyecto = idProyecto;
            StartPosition = FormStartPosition.CenterParent;
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormVerAnalisis"));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCerrar, BE.NombreControl.FormVerAnalisis_btnCerrar));
            _controlesTraducibles.AddRange(panelAnalisis.Traducibles());
            GestorIdioma.Instancia.Adjuntar(this);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles) control.Traducir(traducciones);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormVerAnalisis_Load(object sender, EventArgs e)
        {
            panelAnalisis.Limpiar("Calculando el análisis con los datos de mercado...");
            BeginInvoke(new Action(Calcular)); // primero se muestra la ventana; las APIs pueden tardar
        }

        private void Calcular()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                AnalisisComparativo analisis = new ProyectoInversionBLL().ObtenerAnalisisComparativo(_idProyecto);
                ProyectoInversion p = analisis.Proyecto;
                lblProyectoValor.Text = $"{p} - {p.TipoInmueble} en {p.TipoPublicacion.ToString().ToLower()} - {PN3Comun.Texto(p.Estado)} - canje {p.PorcentajeCanje:0.##}%";
                panelAnalisis.Mostrar(analisis, new IccBLL().Ultimo());
            }
            catch (Exception ex)
            {
                panelAnalisis.Limpiar("No se pudo calcular el análisis: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
