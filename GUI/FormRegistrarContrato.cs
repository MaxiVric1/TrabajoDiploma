using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    // Firma de un contrato en 5 pasos: inmueble del PN1 (y su propietario), inquilino, garante, condiciones y
    // resumen con registro y PDF firmado
    public partial class FormRegistrarContrato : Form, IObservador
    {
        private static readonly Color PESTANA = Color.FromArgb(38, 31, 50);
        private static readonly Color PESTANA_ACTIVA = Color.FromArgb(89, 75, 160);

        private InquilinoBLL inquilinoBLL = new InquilinoBLL();
        private InmuebleAlquilerBLL inmuebleBLL = new InmuebleAlquilerBLL();
        private ContratoBLL contratoBLL = new ContratoBLL();
        private ClienteBLL clienteBLL = new ClienteBLL();
        private CaptacionBLL captacionBLL = new CaptacionBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();

        private Inquilino _inquilinoEncontrado;   // inquilino existente cargado con "Buscar"
        private Cliente _propietario;             // propietario del inmueble seleccionado
        private Dictionary<int, decimal?> _valoresTasados = new Dictionary<int, decimal?>(); // por IdInmueble
        private int? _contratoRegistrado;          // N° del contrato ya registrado (habilita el PDF)

        public FormRegistrarContrato()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormRegistrarContrato"));
            // Pestañas
            _controlesTraducibles.Add(new EtiquetaTraducible(tabInmueble, BE.NombreControl.FormRegistrarContrato_tabInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(tabInquilino, BE.NombreControl.FormRegistrarContrato_tabInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(tabGarante, BE.NombreControl.FormRegistrarContrato_tabGarante));
            _controlesTraducibles.Add(new EtiquetaTraducible(tabCondiciones, BE.NombreControl.FormRegistrarContrato_tabCondiciones));
            _controlesTraducibles.Add(new EtiquetaTraducible(tabResumen, BE.NombreControl.FormRegistrarContrato_tabResumen));
            // 1. Inmueble y propietario
            _controlesTraducibles.Add(new EtiquetaTraducible(lblInmueble, BE.NombreControl.FormRegistrarContrato_lblInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpDatosInmueble, BE.NombreControl.FormRegistrarContrato_grpDatosInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTipoInmueble, BE.NombreControl.FormRegistrarContrato_lblTipoInmueble));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDireccion, BE.NombreControl.FormRegistrarContrato_lblDireccion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblSuperficie, BE.NombreControl.FormRegistrarContrato_lblSuperficie));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblAmbientes, BE.NombreControl.FormRegistrarContrato_lblAmbientes));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblValorTasado, BE.NombreControl.FormRegistrarContrato_lblValorTasado));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpPropietario, BE.NombreControl.FormRegistrarContrato_grpPropietario));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPropietarioNombre, BE.NombreControl.FormRegistrarContrato_lblPropietarioNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPropietarioDni, BE.NombreControl.FormRegistrarContrato_lblPropietarioDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPropietarioTelefono, BE.NombreControl.FormRegistrarContrato_lblPropietarioTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPropietarioEmail, BE.NombreControl.FormRegistrarContrato_lblPropietarioEmail));
            // 2. Inquilino
            _controlesTraducibles.Add(new EtiquetaTraducible(grpInquilino, BE.NombreControl.FormRegistrarContrato_grpInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDni, BE.NombreControl.FormRegistrarContrato_lblDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnBuscarInquilino, BE.NombreControl.FormRegistrarContrato_btnBuscarInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNombre, BE.NombreControl.FormRegistrarContrato_lblNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblApellido, BE.NombreControl.FormRegistrarContrato_lblApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTelefono, BE.NombreControl.FormRegistrarContrato_lblTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEmail, BE.NombreControl.FormRegistrarContrato_lblEmail));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDomicilio, BE.NombreControl.FormRegistrarContrato_lblDomicilio));
            // 3. Garante
            _controlesTraducibles.Add(new EtiquetaTraducible(grpGarante, BE.NombreControl.FormRegistrarContrato_grpGarante));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblGaranteNombre, BE.NombreControl.FormRegistrarContrato_lblGaranteNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblGaranteApellido, BE.NombreControl.FormRegistrarContrato_lblGaranteApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblGaranteDni, BE.NombreControl.FormRegistrarContrato_lblGaranteDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblGaranteDomicilio, BE.NombreControl.FormRegistrarContrato_lblGaranteDomicilio));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPropiedadGarantia, BE.NombreControl.FormRegistrarContrato_lblPropiedadGarantia));
            // 4. Condiciones
            _controlesTraducibles.Add(new EtiquetaTraducible(grpContrato, BE.NombreControl.FormRegistrarContrato_grpContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaInicio, BE.NombreControl.FormRegistrarContrato_lblFechaInicio));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDuracion, BE.NombreControl.FormRegistrarContrato_lblDuracion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMonto, BE.NombreControl.FormRegistrarContrato_lblMonto));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMetodoPago, BE.NombreControl.FormRegistrarContrato_lblMetodoPago));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDeposito, BE.NombreControl.FormRegistrarContrato_lblDeposito));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblComision, BE.NombreControl.FormRegistrarContrato_lblComision));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblActualizacion, BE.NombreControl.FormRegistrarContrato_lblActualizacion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaFin, BE.NombreControl.FormRegistrarContrato_lblFechaFin));
            // 5. Resumen y PDF
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFirma, BE.NombreControl.FormRegistrarContrato_lblFirma));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRegistrar, BE.NombreControl.FormRegistrarContrato_btnRegistrar));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnGenerarPdf, BE.NombreControl.FormRegistrarContrato_btnGenerarPdf));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnNuevoContrato, BE.NombreControl.FormRegistrarContrato_btnNuevoContrato));
            // Navegación
            _controlesTraducibles.Add(new EtiquetaTraducible(btnAnterior, BE.NombreControl.FormRegistrarContrato_btnAnterior));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnSiguiente, BE.NombreControl.FormRegistrarContrato_btnSiguiente));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCancelar, BE.NombreControl.FormRegistrarContrato_btnCancelar));
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
            tabContrato.Invalidate(); // los títulos de las pestañas se dibujan a mano
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormRegistrarContrato_Load(object sender, EventArgs e)
        {
            cmbMetodoPago.Items.Clear();
            foreach (MetodoPago metodo in Enum.GetValues(typeof(MetodoPago)))
            {
                cmbMetodoPago.Items.Add(metodo);
            }
            lblActualizacionValor.Text = $"{ContratoBLL.PERIODICIDAD_IPC_MESES} meses por IPC (fijo)";

            NuevoContrato();
        }

        // ---------- Navegación entre pestañas ----------

        // Pestañas oscuras como el resto del formulario
        private void tabContrato_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage pagina = tabContrato.TabPages[e.Index];
            bool activa = e.Index == tabContrato.SelectedIndex;
            using (SolidBrush fondo = new SolidBrush(activa ? PESTANA_ACTIVA : PESTANA))
            {
                e.Graphics.FillRectangle(fondo, e.Bounds);
            }
            TextRenderer.DrawText(e.Graphics, $"{e.Index + 1}. {pagina.Text}", tabContrato.Font, e.Bounds, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (tabContrato.SelectedIndex > 0) tabContrato.SelectedIndex--;
        }

        // Antes de avanzar se revisa que la pestaña actual esté completa (el registro valida todo de nuevo)
        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            string faltante = _contratoRegistrado.HasValue ? null : FaltanteEnPestana(tabContrato.SelectedIndex);
            if (faltante != null)
            {
                MessageBox.Show(faltante, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tabContrato.SelectedIndex < tabContrato.TabCount - 1) tabContrato.SelectedIndex++;
        }

        private void tabContrato_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAnterior.Enabled = tabContrato.SelectedIndex > 0;
            btnSiguiente.Enabled = tabContrato.SelectedIndex < tabContrato.TabCount - 1;
            if (tabContrato.SelectedTab == tabResumen) ActualizarResumen();
        }

        private string FaltanteEnPestana(int indice)
        {
            switch (indice)
            {
                case 0:
                    if (!(cmbInmueble.SelectedItem is Inmueble))
                        return cmbInmueble.Items.Count == 0
                            ? "No hay inmuebles de alquiler publicados y disponibles. Los inmuebles se captan, tasan y publican en el circuito de captación."
                            : "Seleccione el inmueble del contrato.";
                    return null;
                case 1:
                    if (string.IsNullOrWhiteSpace(txtDni.Text)) return "Ingrese el DNI del inquilino.";
                    if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text))
                        return "Busque al inquilino por DNI o complete su nombre y apellido.";
                    if (_inquilinoEncontrado == null && string.IsNullOrWhiteSpace(txtDomicilio.Text))
                        return "Ingrese el domicilio del inquilino: figura en el contrato.";
                    return null;
                case 2:
                    if (new[] { txtGaranteNombre, txtGaranteApellido, txtGaranteDni, txtGaranteDomicilio, txtPropiedadGarantia }
                            .Any(t => string.IsNullOrWhiteSpace(t.Text)))
                        return "Complete todos los datos del garante: es obligatorio.";
                    if (txtGaranteDni.Text.Trim() == txtDni.Text.Trim())
                        return "El inquilino no puede ser garante de su propio contrato.";
                    return null;
                case 3:
                    if (nudMonto.Value <= 0) return "Ingrese el monto mensual del alquiler.";
                    return null;
                default:
                    return null;
            }
        }

        // ---------- 1. Inmueble y propietario ----------

        // Inmuebles del PN1 de alquiler, Publicados y sin contrato vigente (los inmuebles se cargan en el PN1)
        private void CargarInmuebles()
        {
            try
            {
                _valoresTasados = captacionBLL.LeerPublicados()
                    .GroupBy(r => r.IdInmueble)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.NroCaptacion).First().ValorFinal);

                List<Inmueble> disponibles = inmuebleBLL.LeerInmueblesParaAlquilar();
                cmbInmueble.DataSource = null;
                cmbInmueble.DataSource = disponibles; // se muestra "Tipo - Dirección" (Inmueble.ToString)
                cmbInmueble.SelectedIndex = disponibles.Count > 0 ? 0 : -1;
                MostrarInmueble();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los inmuebles disponibles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbInmueble_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostrarInmueble();
        }

        private void MostrarInmueble()
        {
            Inmueble inmueble = cmbInmueble.SelectedItem as Inmueble;
            _propietario = null;
            lblTipoInmuebleValor.Text = lblDireccionValor.Text = lblSuperficieValor.Text = lblAmbientesValor.Text = lblValorTasadoValor.Text = "-";
            lblPropietarioNombreValor.Text = lblPropietarioDniValor.Text = lblPropietarioTelefonoValor.Text = lblPropietarioEmailValor.Text = "-";
            if (inmueble == null) return;

            lblTipoInmuebleValor.Text = inmueble.TipoInmueble;
            lblDireccionValor.Text = inmueble.Direccion;
            lblSuperficieValor.Text = $"{inmueble.MetrosCuadrados:N0} m²";
            lblAmbientesValor.Text = inmueble.CantidadAmbientes.ToString();

            // El valor tasado es el alquiler mensual de referencia: se propone como monto del contrato
            if (_valoresTasados.TryGetValue(inmueble.IdInmueble, out decimal? tasado) && tasado.HasValue)
            {
                lblValorTasadoValor.Text = $"${tasado.Value:N2} mensuales";
                if (!_contratoRegistrado.HasValue) nudMonto.Value = Math.Min(tasado.Value, nudMonto.Maximum);
            }

            try
            {
                _propietario = clienteBLL.BuscarPorId(inmueble.IdCliente);
                if (_propietario != null)
                {
                    lblPropietarioNombreValor.Text = $"{_propietario.Nombre} {_propietario.Apellido}";
                    lblPropietarioDniValor.Text = _propietario.Dni;
                    lblPropietarioTelefonoValor.Text = string.IsNullOrEmpty(_propietario.Telefono) ? "-" : _propietario.Telefono;
                    lblPropietarioEmailValor.Text = string.IsNullOrEmpty(_propietario.Email) ? "-" : _propietario.Email;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el propietario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------- 2. Inquilino ----------

        private void btnBuscarInquilino_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI para buscar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return;
            }

            try
            {
                Inquilino inquilino = inquilinoBLL.BuscarPorDni(dni);
                if (inquilino != null)
                {
                    MostrarInquilinoExistente(inquilino);
                    MessageBox.Show($"Inquilino encontrado: {inquilino.Apellido}, {inquilino.Nombre}.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    LiberarDatosInquilino(limpiar: true);
                    MessageBox.Show("No existe un inquilino con ese DNI. Complete sus datos para registrarlo junto con el contrato.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el inquilino: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Si el agente cambia el DNI de un inquilino ya encontrado, deja de ser ese inquilino
        private void txtDni_TextChanged(object sender, EventArgs e)
        {
            if (_inquilinoEncontrado != null && txtDni.Text.Trim() != _inquilinoEncontrado.Dni)
                LiberarDatosInquilino(limpiar: true);
        }

        private void MostrarInquilinoExistente(Inquilino inquilino)
        {
            _inquilinoEncontrado = inquilino;
            txtDni.Text = inquilino.Dni;
            txtNombre.Text = inquilino.Nombre;
            txtApellido.Text = inquilino.Apellido;
            txtTelefono.Text = inquilino.Telefono;
            txtEmail.Text = inquilino.Email;
            txtDomicilio.Text = inquilino.Domicilio;
            // Los datos de un inquilino existente no se modifican desde este formulario
            txtNombre.ReadOnly = txtApellido.ReadOnly = txtTelefono.ReadOnly = txtEmail.ReadOnly = txtDomicilio.ReadOnly = true;
        }

        private void LiberarDatosInquilino(bool limpiar)
        {
            _inquilinoEncontrado = null;
            txtNombre.ReadOnly = txtApellido.ReadOnly = txtTelefono.ReadOnly = txtEmail.ReadOnly = txtDomicilio.ReadOnly = false;
            if (limpiar)
            {
                txtNombre.Clear();
                txtApellido.Clear();
                txtTelefono.Clear();
                txtEmail.Clear();
                txtDomicilio.Clear();
            }
        }

        // ---------- 4. Condiciones ----------

        private Contrato Borrador()
        {
            return new Contrato
            {
                FechaInicio = dtpFechaInicio.Value.Date,
                DuracionMeses = (int)nudDuracion.Value,
                MontoMensual = nudMonto.Value,
                DepositoGarantia = ContratoBLL.CalcularDeposito(nudMonto.Value),
                ComisionInmobiliaria = ContratoBLL.PORCENTAJE_COMISION,
                MetodoPago = cmbMetodoPago.SelectedItem is MetodoPago metodo ? metodo : MetodoPago.Transferencia,
                PeriodicidadActualizacionMeses = ContratoBLL.PERIODICIDAD_IPC_MESES
            };
        }

        // Fecha de fin, estado con el que nacerá el contrato, primer mes a pagar, depósito y comisión
        // (mismas reglas que la BLL)
        private void DatosContrato_Changed(object sender, EventArgs e)
        {
            Contrato borrador = Borrador();
            DateTime fin = borrador.FechaFin;
            var primerMes = PagoAlquilerBLL.PeriodosDelContrato(borrador).First();
            string texto = $"{fin:dd/MM/yyyy} — primer mes a pagar: {primerMes.Mes:00}/{primerMes.Anio}";

            switch (ContratoBLL.EstadoSegunVencimiento(fin, DateTime.Now))
            {
                case EstadoContrato.PorVencer:
                    texto += $"\nNacerá Por Vencer (vence en {ContratoBLL.DiasRestantes(borrador, DateTime.Now)} días)";
                    break;
                case EstadoContrato.Finalizado:
                    texto += "\nCon estas fechas ya estaría vencido";
                    break;
            }
            lblFechaFinValor.Text = texto;
            lblDepositoValor.Text = $"${borrador.DepositoGarantia:N2} ({ContratoBLL.MESES_DEPOSITO} mes de alquiler)";
            lblComisionValor.Text = $"{borrador.ComisionInmobiliaria:0.##}% de cada pago: " +
                                    $"${ContratoBLL.CalcularComision(borrador.MontoMensual, borrador.ComisionInmobiliaria):N2} por mes " +
                                    $"(neto al propietario ${ContratoBLL.NetoPropietario(borrador.MontoMensual, borrador.ComisionInmobiliaria):N2})";
        }

        // ---------- 5. Resumen, registro y PDF ----------

        private void ActualizarResumen()
        {
            Inmueble inmueble = cmbInmueble.SelectedItem as Inmueble;
            Contrato c = Borrador();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INMUEBLE");
            sb.AppendLine(inmueble != null
                ? $"  {inmueble.TipoInmueble} - {inmueble.Direccion} ({inmueble.MetrosCuadrados:N0} m², {inmueble.CantidadAmbientes} amb.)"
                : "  (sin seleccionar)");
            sb.AppendLine(_propietario != null
                ? $"  Propietario: {_propietario.Nombre} {_propietario.Apellido}, DNI {_propietario.Dni}"
                : "  Propietario: -");
            sb.AppendLine();
            sb.AppendLine("INQUILINO" + (_inquilinoEncontrado != null ? " (ya registrado)" : string.IsNullOrWhiteSpace(txtDni.Text) ? "" : " (nuevo)"));
            sb.AppendLine($"  {Texto(txtNombre)} {Texto(txtApellido)}, DNI {Texto(txtDni)}");
            sb.AppendLine($"  Teléfono: {Texto(txtTelefono)}   Email: {Texto(txtEmail)}");
            sb.AppendLine($"  Domicilio: {Texto(txtDomicilio)}");
            sb.AppendLine();
            sb.AppendLine("GARANTE");
            sb.AppendLine($"  {Texto(txtGaranteNombre)} {Texto(txtGaranteApellido)}, DNI {Texto(txtGaranteDni)}");
            sb.AppendLine($"  Domicilio: {Texto(txtGaranteDomicilio)}");
            sb.AppendLine($"  Propiedad en garantía: {Texto(txtPropiedadGarantia)}");
            sb.AppendLine();
            sb.AppendLine("CONDICIONES");
            sb.AppendLine($"  Plazo: {c.DuracionMeses} meses, del {c.FechaInicio:dd/MM/yyyy} al {c.FechaFin:dd/MM/yyyy}");
            sb.AppendLine($"  Alquiler mensual: ${c.MontoMensual:N2} ({c.MetodoPago})");
            sb.AppendLine($"  Depósito de garantía: ${c.DepositoGarantia:N2}");
            sb.AppendLine($"  Comisión inmobiliaria: {c.ComisionInmobiliaria:0.##}% de cada pago " +
                          $"(${ContratoBLL.CalcularComision(c.MontoMensual, c.ComisionInmobiliaria):N2} por mes; neto al propietario " +
                          $"${ContratoBLL.NetoPropietario(c.MontoMensual, c.ComisionInmobiliaria):N2})");
            sb.AppendLine($"  Actualización: IPC cada {c.PeriodicidadActualizacionMeses} meses");
            txtResumen.Text = sb.ToString();
        }

        private static string Texto(TextBox caja) => string.IsNullOrWhiteSpace(caja.Text) ? "-" : caja.Text.Trim();

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            // Cada pestaña completa; si falta algo se muestra la pestaña correspondiente
            for (int i = 0; i < 4; i++)
            {
                string faltante = FaltanteEnPestana(i);
                if (faltante != null)
                {
                    tabContrato.SelectedIndex = i;
                    MessageBox.Show(faltante, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            Inmueble inmueble = (Inmueble)cmbInmueble.SelectedItem;
            string dni = txtDni.Text.Trim();

            try
            {
                // Sin "Buscar" previo: si el DNI ya existe se usan los datos registrados, no los tipeados
                if (_inquilinoEncontrado == null)
                {
                    Inquilino existente = inquilinoBLL.BuscarPorDni(dni);
                    if (existente != null)
                    {
                        DialogResult res = MessageBox.Show(
                            $"Ya existe un inquilino con DNI {dni}: {existente.Apellido}, {existente.Nombre}.\n\n" +
                            "El contrato se registrará con los datos ya guardados de ese inquilino. ¿Desea continuar?",
                            "Inquilino existente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        MostrarInquilinoExistente(existente);
                        ActualizarResumen();
                        if (res != DialogResult.Yes) return;
                    }
                }

                Contrato contrato = Borrador();
                contrato.Garantes.Add(new Garante
                {
                    Nombre = txtGaranteNombre.Text,
                    Apellido = txtGaranteApellido.Text,
                    Dni = txtGaranteDni.Text,
                    Domicilio = txtGaranteDomicilio.Text,
                    PropiedadGarantia = txtPropiedadGarantia.Text
                });

                // Inquilino nuevo: se da de alta junto con el contrato en una sola transacción
                Inquilino inquilinoNuevo = null;
                if (_inquilinoEncontrado != null)
                {
                    contrato.IdInquilino = _inquilinoEncontrado.IdInquilino;
                }
                else
                {
                    inquilinoNuevo = new Inquilino
                    {
                        Dni = dni,
                        Nombre = txtNombre.Text,
                        Apellido = txtApellido.Text,
                        Telefono = txtTelefono.Text,
                        Email = txtEmail.Text,
                        Domicilio = txtDomicilio.Text
                    };
                }

                int nro = contratoBLL.Registrar(contrato, inmueble.IdInmueble, inquilinoNuevo);
                _contratoRegistrado = nro;
                if (inquilinoNuevo != null) MostrarInquilinoExistente(inquilinoNuevo);
                BloquearDatos(true);

                string estado = contrato.EstadoContrato == EstadoContrato.PorVencer ? " Queda Por Vencer (alerta de vencimiento activa)." : "";
                lblEstadoRegistro.Text = $"Contrato N° {nro} registrado. Vence el {contrato.FechaFin:dd/MM/yyyy}.{estado} " +
                                         "El inmueble pasó a Alquilado. Ingrese la firma del inquilino y genere el PDF.";
                MessageBox.Show($"Contrato N° {nro} registrado con éxito.\nVence el {contrato.FechaFin:dd/MM/yyyy}.{estado}\nEl inmueble pasa a Alquilado.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtFirma.Focus();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerarPdf_Click(object sender, EventArgs e)
        {
            if (!_contratoRegistrado.HasValue) return;
            Inquilino firmante = new Inquilino { Nombre = txtNombre.Text, Apellido = txtApellido.Text };
            if (!ContratoBLL.FirmaCoincide(txtFirma.Text, firmante))
            {
                MessageBox.Show($"La firma debe ser el nombre y apellido del inquilino: {txtNombre.Text.Trim()} {txtApellido.Text.Trim()}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirma.Focus();
                return;
            }

            using (SaveFileDialog dialogo = new SaveFileDialog
            {
                Filter = "Documento PDF (*.pdf)|*.pdf",
                FileName = $"Contrato_{_contratoRegistrado.Value}.pdf",
                Title = "Guardar contrato en PDF"
            })
            {
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    contratoBLL.GenerarPdf(_contratoRegistrado.Value, txtFirma.Text, dialogo.FileName);
                    lblEstadoRegistro.Text = $"Contrato N° {_contratoRegistrado.Value} registrado. PDF guardado en {dialogo.FileName}.";
                    if (MessageBox.Show("PDF generado con éxito. ¿Desea abrirlo?", "Éxito", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Process.Start(dialogo.FileName);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNuevoContrato_Click(object sender, EventArgs e)
        {
            NuevoContrato();
        }

        // Una vez registrado, los datos quedan fijos (solo se puede generar el PDF o empezar otro contrato)
        private void BloquearDatos(bool bloquear)
        {
            cmbInmueble.Enabled = grpInquilino.Enabled = grpGarante.Enabled = grpContrato.Enabled = !bloquear;
            btnRegistrar.Enabled = !bloquear;
            btnGenerarPdf.Enabled = btnNuevoContrato.Enabled = txtFirma.Enabled = bloquear;
        }

        private void NuevoContrato()
        {
            _contratoRegistrado = null;
            BloquearDatos(false);
            txtFirma.Enabled = true;
            txtDni.Clear();
            LiberarDatosInquilino(limpiar: true);
            foreach (TextBox t in new[] { txtGaranteNombre, txtGaranteApellido, txtGaranteDni, txtGaranteDomicilio, txtPropiedadGarantia, txtFirma })
            {
                t.Clear();
            }
            dtpFechaInicio.Value = DateTime.Today;
            nudDuracion.Value = 24;
            nudMonto.Value = 0;
            cmbMetodoPago.SelectedItem = MetodoPago.Transferencia;
            lblEstadoRegistro.Text = "";
            CargarInmuebles(); // propone el valor tasado como monto
            DatosContrato_Changed(this, EventArgs.Empty);
            tabContrato.SelectedIndex = 0;
            tabContrato_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
