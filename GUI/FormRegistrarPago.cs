using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormRegistrarPago : Form, IObservador
    {
        private static readonly Color TEXTO_CON_DEUDA = Color.FromArgb(255, 130, 130);
        private static readonly Color TEXTO_AVISO = Color.FromArgb(255, 200, 120);
        private ToolTip toolTip = new ToolTip(); // detalle de la tasa de mora aplicada

        private ContratoBLL contratoBLL = new ContratoBLL();
        private PagoAlquilerBLL pagoBLL = new PagoAlquilerBLL();
        private InquilinoBLL inquilinoBLL = new InquilinoBLL();
        private InmuebleAlquilerBLL inmuebleBLL = new InmuebleAlquilerBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();

        private Contrato _contrato;        // contrato encontrado con "Buscar"
        private string _nombreInquilino;

        public FormRegistrarPago()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormRegistrarPago"));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpContrato, BE.NombreControl.FormRegistrarPago_grpContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNroContrato, BE.NombreControl.FormRegistrarPago_lblNroContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnBuscar, BE.NombreControl.FormRegistrarPago_btnBuscar));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblInquilino, BE.NombreControl.FormRegistrarPago_lblInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDireccion, BE.NombreControl.FormRegistrarPago_lblDireccion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMontoMensual, BE.NombreControl.FormRegistrarPago_lblMontoMensual));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEstadoContrato, BE.NombreControl.FormRegistrarPago_lblEstadoContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMora, BE.NombreControl.FormRegistrarPago_lblMora));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpPeriodo, BE.NombreControl.FormRegistrarPago_grpPeriodo));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPeriodo, BE.NombreControl.FormRegistrarPago_lblPeriodo));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEstadoMes, BE.NombreControl.FormRegistrarPago_lblEstadoMes));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaPago, BE.NombreControl.FormRegistrarPago_lblFechaPago));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMontoBase, BE.NombreControl.FormRegistrarPago_lblMontoBase));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblRecargo, BE.NombreControl.FormRegistrarPago_lblRecargo));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTotal, BE.NombreControl.FormRegistrarPago_lblTotal));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblComision, BE.NombreControl.FormRegistrarPago_lblComision));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNeto, BE.NombreControl.FormRegistrarPago_lblNeto));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTasa, BE.NombreControl.FormRegistrarPago_lblTasa));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpResumen, BE.NombreControl.FormRegistrarPago_grpResumen));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRegistrar, BE.NombreControl.FormRegistrarPago_btnRegistrar));
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

        private void FormRegistrarPago_Load(object sender, EventArgs e)
        {
            // El pago no puede ser futuro; la fecha define si corresponde recargo (pagos recibidos antes y cargados después)
            dtpFechaPago.Value = DateTime.Today;
            dtpFechaPago.MaxDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            nudNroContrato.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter) { ev.SuppressKeyPress = true; btnBuscar.PerformClick(); } };

            lblActualizacion.Text = EstadosPN2.EjecutarProcesosAutomaticos();
        }

        // ---------- 1. Contrato ----------

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            int nro = (int)nudNroContrato.Value;
            try
            {
                Contrato contrato = contratoBLL.BuscarPorId(nro);
                if (contrato == null)
                {
                    LimpiarContrato();
                    MessageBox.Show($"No existe el contrato N° {nro}.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _contrato = contrato;
                Inquilino inquilino = inquilinoBLL.BuscarPorId(contrato.IdInquilino);
                InmuebleAlquiler inmueble = inmuebleBLL.BuscarPorId(contrato.IdInmuebleAlquiler);
                _nombreInquilino = inquilino?.NombreCompleto ?? "-";

                lblInquilinoValor.Text = inquilino != null ? $"{inquilino.NombreCompleto} (DNI {inquilino.Dni})" : "-";
                lblDireccionValor.Text = inmueble?.ToString() ?? "-";
                lblMontoMensualValor.Text = $"${contrato.MontoMensual:N2}";
                lblEstadoContratoValor.Text = EstadosPN2.Texto(contrato.EstadoContrato);

                CargarPeriodos();
                MostrarMora();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarContrato()
        {
            _contrato = null;
            lblInquilinoValor.Text = lblDireccionValor.Text = lblMontoMensualValor.Text = lblEstadoContratoValor.Text = "-";
            lblMoraValor.Text = "-";
            lblMoraValor.ForeColor = Color.White;
            cmbPeriodo.DataSource = null;
            grpPeriodo.Enabled = false;
            btnRegistrar.Enabled = false;
            LimpiarImporte();
            lblResumenValor.Text = "Busque un contrato para registrar un pago.";
        }

        // Meses vencidos sin pagar y lo que se debe a hoy, con el recargo por mora de cada mes
        private void MostrarMora()
        {
            var (enMora, enLitigio, deuda) = pagoBLL.ResumenMora(_contrato, DateTime.Now);
            if (enMora + enLitigio == 0)
            {
                lblMoraValor.Text = "Sin mora: no hay meses vencidos impagos.";
                lblMoraValor.ForeColor = Color.White;
                return;
            }
            string detalle = enLitigio > 0 ? $"{enMora} En Mora, {enLitigio} En Litigio" : $"{enMora} En Mora";
            lblMoraValor.Text = $"{enMora + enLitigio} mes(es) impagos ({detalle}): debe ${deuda:N2} con recargos";
            lblMoraValor.ForeColor = TEXTO_CON_DEUDA;
        }

        // ---------- 2. Período ----------

        // Meses sin pago registrado hasta el mes en curso, del más antiguo al más reciente
        private void CargarPeriodos()
        {
            List<PeriodoAPagar> periodos = pagoBLL.LeerPeriodosAPagar(_contrato, DateTime.Now);
            cmbPeriodo.DataSource = null;
            cmbPeriodo.DataSource = periodos;

            bool hayPeriodos = periodos.Count > 0;
            grpPeriodo.Enabled = hayPeriodos;
            if (!hayPeriodos)
            {
                btnRegistrar.Enabled = false;
                LimpiarImporte();
                lblResumenValor.Text = $"El contrato N° {_contrato.IdContrato} no tiene meses adeudados ni pendientes de pago.";
                return;
            }
            cmbPeriodo.SelectedIndex = 0;
            DatosPago_Changed(this, EventArgs.Empty);
        }

        private PeriodoAPagar PeriodoSeleccionado() => cmbPeriodo.SelectedItem as PeriodoAPagar;

        private static string TextoEstado(PeriodoAPagar periodo)
        {
            if (periodo.Regularizado) return "Regularizado (sin pago)";
            if (periodo.Estado == null)
                return $"Pendiente (vence el {PagoAlquilerBLL.FechaVencimiento(periodo.Mes, periodo.Anio):dd/MM/yyyy})";
            return EstadosPN2.Texto(periodo.Estado.Value);
        }

        // Estado del mes, desglose del importe (según la fecha de pago) y resumen
        private void DatosPago_Changed(object sender, EventArgs e)
        {
            PeriodoAPagar periodo = PeriodoSeleccionado();
            if (_contrato == null || periodo == null)
            {
                LimpiarImporte();
                btnRegistrar.Enabled = false;
                return;
            }

            lblEstadoMesValor.Text = TextoEstado(periodo);
            lblEstadoMesValor.ForeColor = periodo.Estado.HasValue ? TEXTO_CON_DEUDA : Color.White;

            // Monto del período (con las actualizaciones por IPC que rigen para ese mes) e intereses por mora con la
            // TIM del BCRA entre el vencimiento y la fecha de pago
            ImportePeriodo importe = pagoBLL.CalcularImporte(_contrato, periodo.Mes, periodo.Anio, dtpFechaPago.Value);
            decimal montoBase = importe.Base, recargo = importe.Recargo, total = importe.Total;
            lblMontoBaseValor.Text = $"${montoBase:N2}";
            lblRecargoValor.Text = recargo > 0 ? $"${recargo:N2} ({importe.DiasMora} día(s) de mora)" : "$0,00 (dentro de término)";
            lblRecargoValor.ForeColor = recargo > 0 ? TEXTO_CON_DEUDA : Color.White;
            MostrarTasa(importe);
            lblTotalValor.Text = $"${total:N2}";

            // La inmobiliaria retiene su comisión de lo que paga el inquilino; el resto es del propietario
            decimal comision = ContratoBLL.CalcularComision(total, _contrato.ComisionInmobiliaria);
            lblComisionValor.Text = $"${comision:N2} ({_contrato.ComisionInmobiliaria:0.##}%)";
            lblNetoValor.Text = $"${total - comision:N2}";

            lblResumenValor.Text =
                $"Contrato N° {_contrato.IdContrato} — {_nombreInquilino}\n" +
                $"Período {periodo} ({TextoEstado(periodo)}), pagado el {dtpFechaPago.Value:dd/MM/yyyy}\n" +
                $"Alquiler ${montoBase:N2} + recargo ${recargo:N2}" +
                (recargo > 0 ? (importe.TasaFija ? $" ({importe.FuenteTasa})" : $" (TIM {importe.TasaPeriodo:0.####}% en {importe.DiasMora} días)") : "") +
                $" = total ${total:N2}\n" +
                $"Comisión inmobiliaria ${comision:N2} - neto al propietario ${total - comision:N2}" +
                (periodo.Estado.HasValue || periodo.Regularizado ? "\nEl pago regulariza el mes: queda Al día." : "");
            btnRegistrar.Enabled = true;
        }

        // Tasa de mora aplicada: la TIM del período (y su equivalente anual) o, sin datos del BCRA, el porcentaje fijo
        private void MostrarTasa(ImportePeriodo importe)
        {
            if (importe.DiasMora == 0)
            {
                lblTasaValor.Text = "-";
                lblTasaValor.ForeColor = Color.White;
                toolTip.SetToolTip(lblTasaValor, null);
                return;
            }
            lblTasaValor.Text = importe.TasaFija
                ? importe.FuenteTasa
                : $"TIM {importe.TasaPeriodo:0.####}% (≈ {importe.TasaAnual:0.##}% anual)";
            lblTasaValor.ForeColor = importe.TasaFija ? TEXTO_AVISO : Color.White;
            toolTip.SetToolTip(lblTasaValor, importe.TasaFija
                ? $"No se pudo obtener la TIM del BCRA ({pagoBLL.ErrorBCRA ?? "sin datos para esas fechas"}): se aplica el {importe.FuenteTasa}."
                : $"{importe.FuenteTasa}: variación del índice entre el vencimiento ({PagoAlquilerBLL.FechaVencimiento(PeriodoSeleccionado().Mes, PeriodoSeleccionado().Anio):dd/MM/yyyy}) " +
                  $"y la fecha de pago, {importe.DiasMora} día(s).");
        }

        private void LimpiarImporte()
        {
            lblEstadoMesValor.Text = lblMontoBaseValor.Text = lblRecargoValor.Text = lblTotalValor.Text = "-";
            lblComisionValor.Text = lblNetoValor.Text = lblTasaValor.Text = "-";
            lblEstadoMesValor.ForeColor = lblRecargoValor.ForeColor = lblTasaValor.ForeColor = Color.White;
        }

        // ---------- 3. Registro ----------

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            PeriodoAPagar periodo = PeriodoSeleccionado();
            if (_contrato == null || periodo == null) return;

            if (MessageBox.Show("¿Confirma el registro del pago?\n\n" + lblResumenValor.Text, "Confirmar pago",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                DateTime fechaPago = dtpFechaPago.Value.Date == DateTime.Today ? DateTime.Now : dtpFechaPago.Value.Date;
                decimal total = pagoBLL.CalcularImporte(_contrato, periodo.Mes, periodo.Anio, fechaPago).Total;

                pagoBLL.RegistrarPago(_contrato.IdContrato, periodo.Mes, periodo.Anio, total, fechaPago);

                MessageBox.Show($"Pago registrado con éxito: período {periodo} del contrato N° {_contrato.IdContrato} por ${total:N2}.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dtpFechaPago.Value = DateTime.Today;
                CargarPeriodos(); // el mes pagado sale de la lista
                MostrarMora();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el pago: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
