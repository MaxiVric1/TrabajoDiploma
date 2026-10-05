using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // Mini formulario de FormContratosPorVencer: extiende un contrato vigente con ContratoBLL.Renovar.
    // Devuelve DialogResult.OK si la renovación se guardó.
    public partial class FormRenovarContrato : Form, IObservador
    {
        private ContratoBLL contratoBLL = new ContratoBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private readonly Contrato _contrato;
        private readonly bool _conDeuda;

        public FormRenovarContrato(Contrato contrato, bool conDeuda)
        {
            InitializeComponent();
            _contrato = contrato ?? throw new ArgumentNullException(nameof(contrato));
            _conDeuda = conDeuda;

            lblContratoValor.Text = $"N° {contrato.IdContrato} — {contrato.Inquilino?.NombreCompleto}";
            lblVencimientoActualValor.Text = contrato.FechaFin.ToString("dd/MM/yyyy");
            lblMontoActualValor.Text = $"${contrato.MontoMensual:N2}";
            nudNuevoMonto.Value = Math.Min(contrato.MontoMensual, nudNuevoMonto.Maximum);
            ActualizarNuevoVencimiento();

            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormRenovarContrato"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblContrato, BE.NombreControl.FormRenovarContrato_lblContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblVencimientoActual, BE.NombreControl.FormRenovarContrato_lblVencimientoActual));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMontoActual, BE.NombreControl.FormRenovarContrato_lblMontoActual));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMeses, BE.NombreControl.FormRenovarContrato_lblMeses));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNuevoMonto, BE.NombreControl.FormRenovarContrato_lblNuevoMonto));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNuevoVencimiento, BE.NombreControl.FormRenovarContrato_lblNuevoVencimiento));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnConfirmar, BE.NombreControl.FormRenovarContrato_btnConfirmar));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCancelar, BE.NombreControl.FormRenovarContrato_btnCancelar));
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

        private DateTime NuevoVencimiento()
        {
            return _contrato.FechaInicio.AddMonths(_contrato.DuracionMeses + (int)nudMeses.Value);
        }

        private void nudMeses_ValueChanged(object sender, EventArgs e)
        {
            ActualizarNuevoVencimiento();
        }

        private void ActualizarNuevoVencimiento()
        {
            lblNuevoVencimientoValor.Text = NuevoVencimiento().ToString("dd/MM/yyyy");
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            int meses = (int)nudMeses.Value;
            decimal nuevoMonto = nudNuevoMonto.Value;

            string confirmacion =
                $"¿Confirma la renovación del Contrato N° {_contrato.IdContrato}?\n\n" +
                $"Inquilino: {_contrato.Inquilino?.NombreCompleto}\n" +
                $"Vencimiento: {_contrato.FechaFin:dd/MM/yyyy} -> {NuevoVencimiento():dd/MM/yyyy} (+{meses} meses)\n" +
                $"Monto mensual: ${_contrato.MontoMensual:N2} -> ${nuevoMonto:N2}";
            if (nuevoMonto != _contrato.MontoMensual)
                confirmacion += "\n\nEl nuevo monto se aplica a todos los meses que todavía no tienen pago registrado.";
            if (_contrato.AvisoDesocupacion)
                confirmacion += "\n\nEl contrato tiene un aviso de desocupación: la renovación lo deja sin efecto.";
            if (_conDeuda)
                confirmacion += "\n\nATENCIÓN: el contrato tiene pagos En Mora o En Litigio.";

            if (MessageBox.Show(confirmacion, "Confirmar renovación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                Contrato renovado = contratoBLL.Renovar(_contrato.IdContrato, meses, nuevoMonto);
                string mensaje = $"Contrato N° {renovado.IdContrato} renovado con éxito.\nNuevo vencimiento: {renovado.FechaFin:dd/MM/yyyy} " +
                    $"(estado {EstadosPN2.Texto(renovado.EstadoContrato)}).";
                if (renovado.EstadoContrato != EstadoContrato.PorVencer)
                    mensaje += "\n\nEl contrato ya no figura entre los contratos por vencer.";
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al renovar el contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
