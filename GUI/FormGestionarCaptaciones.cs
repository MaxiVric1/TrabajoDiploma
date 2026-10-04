using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormGestionarCaptaciones : Form, IObservador
    {
        private const string FILTRO_TODOS = "Todos";

        private CaptacionBLL captacionBLL = new CaptacionBLL();
        private TasacionBLL tasacionBLL = new TasacionBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _dgvTraducible;
        private bool _cargando; // evita refrescar el detalle mientras se rearma la grilla

        public FormGestionarCaptaciones()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormGestionarCaptaciones"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFiltroEstado, BE.NombreControl.FormGestionarCaptaciones_lblFiltroEstado));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnActualizar, BE.NombreControl.FormGestionarCaptaciones_btnActualizar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpRenegociacion, BE.NombreControl.FormGestionarCaptaciones_grpRenegociacion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDictamen, BE.NombreControl.FormGestionarCaptaciones_lblDictamen));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblValorActual, BE.NombreControl.FormGestionarCaptaciones_lblValorActual));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMotivo, BE.NombreControl.FormGestionarCaptaciones_lblMotivo));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblValorSugerido, BE.NombreControl.FormGestionarCaptaciones_lblValorSugerido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNuevoValor, BE.NombreControl.FormGestionarCaptaciones_lblNuevoValor));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRenegociar, BE.NombreControl.FormGestionarCaptaciones_btnRenegociar));

            _dgvTraducible = new DataGridTraducible(dgvCaptaciones, "FormGestionarCaptaciones.dgvCaptaciones")
                .ConColumna("NroCaptacion", BE.NombreControl.FormGestionarCaptaciones_dgvCaptaciones_nro)
                .ConColumna("FechaRegistro", BE.NombreControl.FormGestionarCaptaciones_dgvCaptaciones_fecha)
                .ConColumna("Cliente", BE.NombreControl.FormGestionarCaptaciones_dgvCaptaciones_cliente)
                .ConColumna("Direccion", BE.NombreControl.FormGestionarCaptaciones_dgvCaptaciones_direccion)
                .ConColumna("TipoInmueble", BE.NombreControl.FormGestionarCaptaciones_dgvCaptaciones_tipo)
                .ConColumna("ValorPretendido", BE.NombreControl.FormGestionarCaptaciones_dgvCaptaciones_valor)
                .ConColumna("EstadoInmueble", BE.NombreControl.FormGestionarCaptaciones_dgvCaptaciones_estado);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
            if (_dgvTraducible != null)
            {
                _dgvTraducible.Traducir(traducciones);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormGestionarCaptaciones_Load(object sender, EventArgs e)
        {
            cmbFiltroEstado.Items.Clear();
            cmbFiltroEstado.Items.Add(FILTRO_TODOS);
            foreach (EstadoInmueble estado in Enum.GetValues(typeof(EstadoInmueble)))
            {
                cmbFiltroEstado.Items.Add(estado);
            }
            cmbFiltroEstado.SelectedIndex = 0; // dispara la primera carga
        }

        private void cmbFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCaptaciones();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarCaptaciones(CaptacionSeleccionada()?.NroCaptacion);
        }

        private void CargarCaptaciones(int? nroASeleccionar = null)
        {
            _cargando = true;
            try
            {
                EstadoInmueble? filtro = cmbFiltroEstado.SelectedItem is EstadoInmueble estado ? estado : (EstadoInmueble?)null;

                dgvCaptaciones.DataSource = null;
                dgvCaptaciones.DataSource = captacionBLL.LeerResumen(filtro);

                if (dgvCaptaciones.Columns["IdInmueble"] != null) dgvCaptaciones.Columns["IdInmueble"].Visible = false;
                if (dgvCaptaciones.Columns["IdCliente"] != null) dgvCaptaciones.Columns["IdCliente"].Visible = false;
                if (dgvCaptaciones.Columns["MetrosCuadrados"] != null) dgvCaptaciones.Columns["MetrosCuadrados"].Visible = false;
                if (dgvCaptaciones.Columns["CantidadAmbientes"] != null) dgvCaptaciones.Columns["CantidadAmbientes"].Visible = false;
                if (dgvCaptaciones.Columns["FechaRegistro"] != null) dgvCaptaciones.Columns["FechaRegistro"].DefaultCellStyle.Format = "dd/MM/yyyy";
                if (dgvCaptaciones.Columns["ValorPretendido"] != null)
                {
                    dgvCaptaciones.Columns["ValorPretendido"].DefaultCellStyle.Format = "N2";
                    dgvCaptaciones.Columns["ValorPretendido"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                var traducciones = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
                if (traducciones != null)
                {
                    _dgvTraducible.Traducir(traducciones);
                }

                if (nroASeleccionar.HasValue)
                {
                    foreach (DataGridViewRow row in dgvCaptaciones.Rows)
                    {
                        if (row.DataBoundItem is CaptacionResumen r && r.NroCaptacion == nroASeleccionar.Value)
                        {
                            row.Selected = true;
                            dgvCaptaciones.CurrentCell = row.Cells["NroCaptacion"];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar captaciones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
            }

            MostrarDetalle();
        }

        private void dgvCaptaciones_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarDetalle();
        }

        private CaptacionResumen CaptacionSeleccionada()
        {
            return dgvCaptaciones.CurrentRow?.DataBoundItem as CaptacionResumen;
        }

        // Muestra el último dictamen de la captación seleccionada y habilita la renegociación si corresponde
        private void MostrarDetalle()
        {
            CaptacionResumen seleccionada = CaptacionSeleccionada();
            lblDictamenValor.Text = "-";
            lblMotivoValor.Text = "-";
            lblValorSugeridoValor.Text = "-";
            lblValorActualValor.Text = "-";
            nudNuevoValor.Value = 0;
            nudNuevoValor.Enabled = false;
            btnRenegociar.Enabled = false;

            if (seleccionada == null) return;

            lblValorActualValor.Text = seleccionada.ValorPretendido.ToString("N2");

            try
            {
                Tasacion dictamen = tasacionBLL.ObtenerUltimoDictamen(seleccionada.NroCaptacion);
                if (dictamen == null)
                {
                    lblDictamenValor.Text = "Sin tasar";
                    return;
                }

                lblDictamenValor.Text = $"{dictamen.EstadoDictamen} ({dictamen.FechaDictamen:dd/MM/yyyy})";
                if (dictamen.EstadoDictamen != EstadoDictamen.Rechazada) return;

                lblMotivoValor.Text = string.IsNullOrEmpty(dictamen.MotivoRechazo) ? "-" : dictamen.MotivoRechazo;
                lblValorSugeridoValor.Text = dictamen.ValorSugerido.HasValue
                    ? dictamen.ValorSugerido.Value.ToString("N2")
                    : "El tasador no sugirió un valor";

                // Mismas condiciones que exige CaptacionBLL.RenegociarCaptacion
                bool puedeRenegociar = seleccionada.EstadoInmueble == EstadoInmueble.Rechazado;
                nudNuevoValor.Enabled = puedeRenegociar;
                btnRenegociar.Enabled = puedeRenegociar;
                if (puedeRenegociar && dictamen.ValorSugerido.HasValue)
                {
                    nudNuevoValor.Value = Math.Min(dictamen.ValorSugerido.Value, nudNuevoValor.Maximum);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer el dictamen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRenegociar_Click(object sender, EventArgs e)
        {
            CaptacionResumen seleccionada = CaptacionSeleccionada();
            if (seleccionada == null) return;

            decimal nuevoValor = nudNuevoValor.Value;
            if (nuevoValor <= 0)
            {
                MessageBox.Show("Ingrese el nuevo valor pretendido aceptado por el cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudNuevoValor.Focus();
                return;
            }

            DialogResult res = MessageBox.Show(
                $"¿Confirma la renegociación de la Captación N° {seleccionada.NroCaptacion}?\n\n" +
                $"Valor pretendido: {seleccionada.ValorPretendido:N2} -> {nuevoValor:N2}\n\n" +
                "El inmueble volverá al estado Captado para que el Tasador lo revise nuevamente.",
                "Confirmar renegociación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res != DialogResult.Yes) return;

            try
            {
                captacionBLL.RenegociarCaptacion(seleccionada.NroCaptacion, nuevoValor);
                MessageBox.Show($"Captación N° {seleccionada.NroCaptacion} renegociada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarCaptaciones(seleccionada.NroCaptacion);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al renegociar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
