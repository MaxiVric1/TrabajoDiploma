using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // Aporte de un inversor a un fideicomiso: se busca el inversor por CUIT (o se carga uno nuevo), se ingresa el
    // monto en USD y se ve al instante la participación que le corresponde
    public partial class FormAgregarSuscripcion : Form, IObservador
    {
        private readonly ResumenFideicomiso _resumen;
        private InversorBLL inversorBLL = new InversorBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private Inversor _encontrado;
        private readonly ToolTip toolTip = new ToolTip();
        private bool _unidadesManuales; // el usuario cambió las unidades sugeridas
        private bool _calculando;       // las unidades las está poniendo el cálculo, no el usuario

        public FormAgregarSuscripcion(ResumenFideicomiso resumen)
        {
            InitializeComponent();
            _resumen = resumen;
            StartPosition = FormStartPosition.CenterParent;
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormAgregarSuscripcion"));
            foreach (var (control, clave) in new (Control, string)[] {
                (grpInversor, BE.NombreControl.FormAgregarSuscripcion_grpInversor),
                (lblCuit, BE.NombreControl.FormAgregarSuscripcion_lblCuit),
                (btnBuscar, BE.NombreControl.FormAgregarSuscripcion_btnBuscar),
                (lblNombre, BE.NombreControl.FormAgregarSuscripcion_lblNombre),
                (lblApellido, BE.NombreControl.FormAgregarSuscripcion_lblApellido),
                (lblEmail, BE.NombreControl.FormAgregarSuscripcion_lblEmail),
                (lblTelefono, BE.NombreControl.FormAgregarSuscripcion_lblTelefono),
                (grpAporte, BE.NombreControl.FormAgregarSuscripcion_grpAporte),
                (lblMonto, BE.NombreControl.FormAgregarSuscripcion_lblMonto),
                (lblUnidades, BE.NombreControl.FormAgregarSuscripcion_lblUnidades),
                (btnGuardar, BE.NombreControl.FormAgregarSuscripcion_btnGuardar),
                (btnCancelar, BE.NombreControl.FormAgregarSuscripcion_btnCancelar) })
                _controlesTraducibles.Add(new EtiquetaTraducible(control, clave));
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

        private void FormAgregarSuscripcion_Load(object sender, EventArgs e)
        {
            Text += $" - Proyecto N° {_resumen.Proyecto.IdProyecto}";
            nudMonto.Maximum = Math.Max(1, _resumen.CapitalRestante);
            nudMonto.Increment = 1000;
            txtCuit.TextChanged += (s, ev) => { if (_encontrado != null && InversorBLL.FormatearCuit(txtCuit.Text) != _encontrado.Cuit) Liberar(); };
            Liberar();
            nudMonto_ValueChanged(this, EventArgs.Empty);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (!InversorBLL.CuitValido(txtCuit.Text))
            {
                MessageBox.Show("Ingrese un CUIT válido (11 dígitos, con o sin guiones).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCuit.Focus();
                return;
            }
            try
            {
                Inversor inversor = inversorBLL.BuscarPorCuit(txtCuit.Text);
                if (inversor == null)
                {
                    Liberar();
                    txtCuit.Text = InversorBLL.FormatearCuit(txtCuit.Text);
                    lblEstadoInversor.Text = "Inversor nuevo: complete sus datos.";
                    txtNombre.Focus();
                    return;
                }
                _encontrado = inversor;
                txtCuit.Text = inversor.Cuit;
                txtNombre.Text = inversor.Nombre;
                txtApellido.Text = inversor.Apellido;
                txtEmail.Text = inversor.Email;
                txtTelefono.Text = inversor.Telefono;
                txtNombre.ReadOnly = txtApellido.ReadOnly = txtEmail.ReadOnly = txtTelefono.ReadOnly = true;
                lblEstadoInversor.Text = "Inversor registrado.";
                nudMonto.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el inversor: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Liberar()
        {
            _encontrado = null;
            txtNombre.ReadOnly = txtApellido.ReadOnly = txtEmail.ReadOnly = txtTelefono.ReadOnly = false;
            txtNombre.Clear();
            txtApellido.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            lblEstadoInversor.Text = "Busque el inversor por CUIT o cargue uno nuevo.";
        }

        // Participación sobre el capital necesario (costo total de la obra), lo que quedaría por suscribir y las
        // unidades que le corresponden (sugeridas mientras el usuario no las cambie a mano)
        private void nudMonto_ValueChanged(object sender, EventArgs e)
        {
            decimal monto = nudMonto.Value;
            decimal participacion = _resumen.CapitalNecesario > 0 ? Math.Round(monto * 100 / _resumen.CapitalNecesario, 4) : 0;
            lblParticipacionValor.Text = $"Participación: {participacion:0.####}% del capital" + Environment.NewLine +
                                         $"Quedarían por suscribir {PN3Comun.Usd(_resumen.CapitalRestante - monto)}";
            if (!_unidadesManuales)
            {
                _calculando = true;
                nudUnidades.Value = Math.Min(Asignacion().Unidades, nudUnidades.Maximum);
                _calculando = false;
            }
            MostrarAsignacion();
        }

        private (decimal Metros, int Unidades) Asignacion() =>
            SuscripcionFideicomisoBLL.CalcularAsignacion(_resumen.Proyecto, _resumen.CapitalNecesario, nudMonto.Value);

        // Un cambio a mano de las unidades se respeta: deja de seguir al monto
        private void nudUnidades_ValueChanged(object sender, EventArgs e)
        {
            if (_calculando || _resumen == null) return;
            _unidadesManuales = nudUnidades.Value != Asignacion().Unidades;
            MostrarAsignacion();
        }

        private void MostrarAsignacion()
        {
            var (metros, unidades) = Asignacion();
            lblAsignacion.Text = $"≈ {metros:N2} m² ({SuscripcionFideicomisoBLL.M2_POR_UNIDAD:0} m² por unidad)" +
                                 (_unidadesManuales ? $" - sugeridas: {unidades}" : "");
            toolTip.SetToolTip(lblAsignacion, $"m² construibles {_resumen.Proyecto.MetrosConstruibles:N2} menos el canje del propietario " +
                $"({_resumen.Proyecto.PorcentajeCanje:0.##}%), en proporción al aporte sobre el capital necesario.");
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int idInversor;
                if (_encontrado != null)
                    idInversor = _encontrado.IdInversor;
                else
                {
                    // Sin "Buscar": si el CUIT ya existe se usa ese inversor
                    Inversor existente = InversorBLL.CuitValido(txtCuit.Text) ? inversorBLL.BuscarPorCuit(txtCuit.Text) : null;
                    idInversor = existente?.IdInversor ?? inversorBLL.Registrar(new Inversor
                    {
                        Cuit = txtCuit.Text,
                        Nombre = txtNombre.Text,
                        Apellido = txtApellido.Text,
                        Email = txtEmail.Text,
                        Telefono = txtTelefono.Text
                    });
                }

                new SuscripcionFideicomisoBLL().AgregarSuscripcion(new SuscripcionFideicomiso
                {
                    IdProyecto = _resumen.Proyecto.IdProyecto,
                    IdInversor = idInversor,
                    MontoAportado = nudMonto.Value,
                    UnidadesAsignadas = (int)nudUnidades.Value
                });
                MessageBox.Show($"Aporte de {PN3Comun.Usd(nudMonto.Value)} registrado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el aporte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
