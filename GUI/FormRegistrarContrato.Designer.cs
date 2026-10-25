namespace TpIngSoft
{
    partial class FormRegistrarContrato
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabContrato = new System.Windows.Forms.TabControl();
            this.tabInmueble = new System.Windows.Forms.TabPage();
            this.lblInmueble = new System.Windows.Forms.Label();
            this.cmbInmueble = new System.Windows.Forms.ComboBox();
            this.grpDatosInmueble = new System.Windows.Forms.GroupBox();
            this.lblTipoInmueble = new System.Windows.Forms.Label();
            this.lblTipoInmuebleValor = new System.Windows.Forms.Label();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.lblDireccionValor = new System.Windows.Forms.Label();
            this.lblSuperficie = new System.Windows.Forms.Label();
            this.lblSuperficieValor = new System.Windows.Forms.Label();
            this.lblAmbientes = new System.Windows.Forms.Label();
            this.lblAmbientesValor = new System.Windows.Forms.Label();
            this.lblValorTasado = new System.Windows.Forms.Label();
            this.lblValorTasadoValor = new System.Windows.Forms.Label();
            this.grpPropietario = new System.Windows.Forms.GroupBox();
            this.lblPropietarioNombre = new System.Windows.Forms.Label();
            this.lblPropietarioNombreValor = new System.Windows.Forms.Label();
            this.lblPropietarioDni = new System.Windows.Forms.Label();
            this.lblPropietarioDniValor = new System.Windows.Forms.Label();
            this.lblPropietarioTelefono = new System.Windows.Forms.Label();
            this.lblPropietarioTelefonoValor = new System.Windows.Forms.Label();
            this.lblPropietarioEmail = new System.Windows.Forms.Label();
            this.lblPropietarioEmailValor = new System.Windows.Forms.Label();
            this.tabInquilino = new System.Windows.Forms.TabPage();
            this.grpInquilino = new System.Windows.Forms.GroupBox();
            this.lblDni = new System.Windows.Forms.Label();
            this.txtDni = new System.Windows.Forms.TextBox();
            this.btnBuscarInquilino = new System.Windows.Forms.Button();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblApellido = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.tabGarante = new System.Windows.Forms.TabPage();
            this.grpGarante = new System.Windows.Forms.GroupBox();
            this.lblGaranteNombre = new System.Windows.Forms.Label();
            this.txtGaranteNombre = new System.Windows.Forms.TextBox();
            this.lblGaranteApellido = new System.Windows.Forms.Label();
            this.txtGaranteApellido = new System.Windows.Forms.TextBox();
            this.lblGaranteDni = new System.Windows.Forms.Label();
            this.txtGaranteDni = new System.Windows.Forms.TextBox();
            this.lblGaranteDomicilio = new System.Windows.Forms.Label();
            this.txtGaranteDomicilio = new System.Windows.Forms.TextBox();
            this.lblPropiedadGarantia = new System.Windows.Forms.Label();
            this.txtPropiedadGarantia = new System.Windows.Forms.TextBox();
            this.tabCondiciones = new System.Windows.Forms.TabPage();
            this.grpContrato = new System.Windows.Forms.GroupBox();
            this.lblFechaInicio = new System.Windows.Forms.Label();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.lblDuracion = new System.Windows.Forms.Label();
            this.nudDuracion = new System.Windows.Forms.NumericUpDown();
            this.lblMonto = new System.Windows.Forms.Label();
            this.nudMonto = new System.Windows.Forms.NumericUpDown();
            this.lblMetodoPago = new System.Windows.Forms.Label();
            this.cmbMetodoPago = new System.Windows.Forms.ComboBox();
            this.lblDeposito = new System.Windows.Forms.Label();
            this.lblDepositoValor = new System.Windows.Forms.Label();
            this.lblComision = new System.Windows.Forms.Label();
            this.lblComisionValor = new System.Windows.Forms.Label();
            this.lblActualizacion = new System.Windows.Forms.Label();
            this.lblActualizacionValor = new System.Windows.Forms.Label();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.lblFechaFinValor = new System.Windows.Forms.Label();
            this.tabResumen = new System.Windows.Forms.TabPage();
            this.txtResumen = new System.Windows.Forms.TextBox();
            this.lblFirma = new System.Windows.Forms.Label();
            this.txtFirma = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.btnGenerarPdf = new System.Windows.Forms.Button();
            this.btnNuevoContrato = new System.Windows.Forms.Button();
            this.lblEstadoRegistro = new System.Windows.Forms.Label();
            this.btnAnterior = new System.Windows.Forms.Button();
            this.btnSiguiente = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.tabContrato.SuspendLayout();
            this.tabInmueble.SuspendLayout();
            this.grpDatosInmueble.SuspendLayout();
            this.grpPropietario.SuspendLayout();
            this.tabInquilino.SuspendLayout();
            this.grpInquilino.SuspendLayout();
            this.tabGarante.SuspendLayout();
            this.grpGarante.SuspendLayout();
            this.tabCondiciones.SuspendLayout();
            this.grpContrato.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuracion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonto)).BeginInit();
            this.tabResumen.SuspendLayout();
            this.SuspendLayout();
            //
            // tabContrato
            //
            this.tabContrato.Controls.Add(this.tabInmueble);
            this.tabContrato.Controls.Add(this.tabInquilino);
            this.tabContrato.Controls.Add(this.tabGarante);
            this.tabContrato.Controls.Add(this.tabCondiciones);
            this.tabContrato.Controls.Add(this.tabResumen);
            this.tabContrato.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabContrato.ItemSize = new System.Drawing.Size(133, 24);
            this.tabContrato.Location = new System.Drawing.Point(12, 12);
            this.tabContrato.Name = "tabContrato";
            this.tabContrato.SelectedIndex = 0;
            this.tabContrato.Size = new System.Drawing.Size(676, 446);
            this.tabContrato.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabContrato.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabContrato_DrawItem);
            this.tabContrato.SelectedIndexChanged += new System.EventHandler(this.tabContrato_SelectedIndexChanged);
            //
            // tabInmueble
            //
            this.tabInmueble.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.tabInmueble.Controls.Add(this.lblInmueble);
            this.tabInmueble.Controls.Add(this.cmbInmueble);
            this.tabInmueble.Controls.Add(this.grpDatosInmueble);
            this.tabInmueble.Controls.Add(this.grpPropietario);
            this.tabInmueble.ForeColor = System.Drawing.Color.White;
            this.tabInmueble.Location = new System.Drawing.Point(4, 22);
            this.tabInmueble.Name = "tabInmueble";
            this.tabInmueble.Padding = new System.Windows.Forms.Padding(3);
            this.tabInmueble.Size = new System.Drawing.Size(668, 420);
            this.tabInmueble.Text = "Inmueble y Propietario";
            //
            // lblInmueble
            //
            this.lblInmueble.Location = new System.Drawing.Point(10, 18);
            this.lblInmueble.Name = "lblInmueble";
            this.lblInmueble.Size = new System.Drawing.Size(130, 21);
            this.lblInmueble.Text = "Disponibles:";
            this.lblInmueble.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cmbInmueble
            //
            this.cmbInmueble.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbInmueble.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInmueble.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbInmueble.ForeColor = System.Drawing.Color.White;
            this.cmbInmueble.FormattingEnabled = true;
            this.cmbInmueble.Location = new System.Drawing.Point(146, 18);
            this.cmbInmueble.Name = "cmbInmueble";
            this.cmbInmueble.Size = new System.Drawing.Size(510, 21);
            this.cmbInmueble.SelectedIndexChanged += new System.EventHandler(this.cmbInmueble_SelectedIndexChanged);
            //
            // grpDatosInmueble
            //
            this.grpDatosInmueble.BackColor = System.Drawing.Color.Transparent;
            this.grpDatosInmueble.Controls.Add(this.lblTipoInmueble);
            this.grpDatosInmueble.Controls.Add(this.lblTipoInmuebleValor);
            this.grpDatosInmueble.Controls.Add(this.lblDireccion);
            this.grpDatosInmueble.Controls.Add(this.lblDireccionValor);
            this.grpDatosInmueble.Controls.Add(this.lblSuperficie);
            this.grpDatosInmueble.Controls.Add(this.lblSuperficieValor);
            this.grpDatosInmueble.Controls.Add(this.lblAmbientes);
            this.grpDatosInmueble.Controls.Add(this.lblAmbientesValor);
            this.grpDatosInmueble.Controls.Add(this.lblValorTasado);
            this.grpDatosInmueble.Controls.Add(this.lblValorTasadoValor);
            this.grpDatosInmueble.ForeColor = System.Drawing.Color.White;
            this.grpDatosInmueble.Location = new System.Drawing.Point(10, 52);
            this.grpDatosInmueble.Name = "grpDatosInmueble";
            this.grpDatosInmueble.Size = new System.Drawing.Size(646, 150);
            this.grpDatosInmueble.TabStop = false;
            this.grpDatosInmueble.Text = "Datos del inmueble";
            //
            // lblTipoInmueble
            //
            this.lblTipoInmueble.Location = new System.Drawing.Point(6, 25);
            this.lblTipoInmueble.Name = "lblTipoInmueble";
            this.lblTipoInmueble.Size = new System.Drawing.Size(130, 20);
            this.lblTipoInmueble.Text = "Tipo:";
            this.lblTipoInmueble.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTipoInmuebleValor
            //
            this.lblTipoInmuebleValor.AutoEllipsis = true;
            this.lblTipoInmuebleValor.Location = new System.Drawing.Point(142, 25);
            this.lblTipoInmuebleValor.Name = "lblTipoInmuebleValor";
            this.lblTipoInmuebleValor.Size = new System.Drawing.Size(180, 20);
            this.lblTipoInmuebleValor.Text = "-";
            this.lblTipoInmuebleValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDireccion
            //
            this.lblDireccion.Location = new System.Drawing.Point(6, 50);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(130, 20);
            this.lblDireccion.Text = "Dirección:";
            this.lblDireccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDireccionValor
            //
            this.lblDireccionValor.AutoEllipsis = true;
            this.lblDireccionValor.Location = new System.Drawing.Point(142, 50);
            this.lblDireccionValor.Name = "lblDireccionValor";
            this.lblDireccionValor.Size = new System.Drawing.Size(490, 20);
            this.lblDireccionValor.Text = "-";
            this.lblDireccionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSuperficie
            //
            this.lblSuperficie.Location = new System.Drawing.Point(6, 75);
            this.lblSuperficie.Name = "lblSuperficie";
            this.lblSuperficie.Size = new System.Drawing.Size(130, 20);
            this.lblSuperficie.Text = "Superficie:";
            this.lblSuperficie.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblSuperficieValor
            //
            this.lblSuperficieValor.AutoEllipsis = true;
            this.lblSuperficieValor.Location = new System.Drawing.Point(142, 75);
            this.lblSuperficieValor.Name = "lblSuperficieValor";
            this.lblSuperficieValor.Size = new System.Drawing.Size(180, 20);
            this.lblSuperficieValor.Text = "-";
            this.lblSuperficieValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblAmbientes
            //
            this.lblAmbientes.Location = new System.Drawing.Point(326, 75);
            this.lblAmbientes.Name = "lblAmbientes";
            this.lblAmbientes.Size = new System.Drawing.Size(100, 20);
            this.lblAmbientes.Text = "Ambientes:";
            this.lblAmbientes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblAmbientesValor
            //
            this.lblAmbientesValor.AutoEllipsis = true;
            this.lblAmbientesValor.Location = new System.Drawing.Point(432, 75);
            this.lblAmbientesValor.Name = "lblAmbientesValor";
            this.lblAmbientesValor.Size = new System.Drawing.Size(200, 20);
            this.lblAmbientesValor.Text = "-";
            this.lblAmbientesValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblValorTasado
            //
            this.lblValorTasado.Location = new System.Drawing.Point(6, 100);
            this.lblValorTasado.Name = "lblValorTasado";
            this.lblValorTasado.Size = new System.Drawing.Size(130, 20);
            this.lblValorTasado.Text = "Valor tasado:";
            this.lblValorTasado.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblValorTasadoValor
            //
            this.lblValorTasadoValor.AutoEllipsis = true;
            this.lblValorTasadoValor.Location = new System.Drawing.Point(142, 100);
            this.lblValorTasadoValor.Name = "lblValorTasadoValor";
            this.lblValorTasadoValor.Size = new System.Drawing.Size(490, 20);
            this.lblValorTasadoValor.Text = "-";
            this.lblValorTasadoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpPropietario
            //
            this.grpPropietario.BackColor = System.Drawing.Color.Transparent;
            this.grpPropietario.Controls.Add(this.lblPropietarioNombre);
            this.grpPropietario.Controls.Add(this.lblPropietarioNombreValor);
            this.grpPropietario.Controls.Add(this.lblPropietarioDni);
            this.grpPropietario.Controls.Add(this.lblPropietarioDniValor);
            this.grpPropietario.Controls.Add(this.lblPropietarioTelefono);
            this.grpPropietario.Controls.Add(this.lblPropietarioTelefonoValor);
            this.grpPropietario.Controls.Add(this.lblPropietarioEmail);
            this.grpPropietario.Controls.Add(this.lblPropietarioEmailValor);
            this.grpPropietario.ForeColor = System.Drawing.Color.White;
            this.grpPropietario.Location = new System.Drawing.Point(10, 212);
            this.grpPropietario.Name = "grpPropietario";
            this.grpPropietario.Size = new System.Drawing.Size(646, 125);
            this.grpPropietario.TabStop = false;
            this.grpPropietario.Text = "Propietario";
            //
            // lblPropietarioNombre
            //
            this.lblPropietarioNombre.Location = new System.Drawing.Point(6, 25);
            this.lblPropietarioNombre.Name = "lblPropietarioNombre";
            this.lblPropietarioNombre.Size = new System.Drawing.Size(130, 20);
            this.lblPropietarioNombre.Text = "Nombre:";
            this.lblPropietarioNombre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblPropietarioNombreValor
            //
            this.lblPropietarioNombreValor.AutoEllipsis = true;
            this.lblPropietarioNombreValor.Location = new System.Drawing.Point(142, 25);
            this.lblPropietarioNombreValor.Name = "lblPropietarioNombreValor";
            this.lblPropietarioNombreValor.Size = new System.Drawing.Size(490, 20);
            this.lblPropietarioNombreValor.Text = "-";
            this.lblPropietarioNombreValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPropietarioDni
            //
            this.lblPropietarioDni.Location = new System.Drawing.Point(6, 50);
            this.lblPropietarioDni.Name = "lblPropietarioDni";
            this.lblPropietarioDni.Size = new System.Drawing.Size(130, 20);
            this.lblPropietarioDni.Text = "DNI:";
            this.lblPropietarioDni.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblPropietarioDniValor
            //
            this.lblPropietarioDniValor.AutoEllipsis = true;
            this.lblPropietarioDniValor.Location = new System.Drawing.Point(142, 50);
            this.lblPropietarioDniValor.Name = "lblPropietarioDniValor";
            this.lblPropietarioDniValor.Size = new System.Drawing.Size(180, 20);
            this.lblPropietarioDniValor.Text = "-";
            this.lblPropietarioDniValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPropietarioTelefono
            //
            this.lblPropietarioTelefono.Location = new System.Drawing.Point(6, 75);
            this.lblPropietarioTelefono.Name = "lblPropietarioTelefono";
            this.lblPropietarioTelefono.Size = new System.Drawing.Size(130, 20);
            this.lblPropietarioTelefono.Text = "Teléfono:";
            this.lblPropietarioTelefono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblPropietarioTelefonoValor
            //
            this.lblPropietarioTelefonoValor.AutoEllipsis = true;
            this.lblPropietarioTelefonoValor.Location = new System.Drawing.Point(142, 75);
            this.lblPropietarioTelefonoValor.Name = "lblPropietarioTelefonoValor";
            this.lblPropietarioTelefonoValor.Size = new System.Drawing.Size(180, 20);
            this.lblPropietarioTelefonoValor.Text = "-";
            this.lblPropietarioTelefonoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPropietarioEmail
            //
            this.lblPropietarioEmail.Location = new System.Drawing.Point(326, 75);
            this.lblPropietarioEmail.Name = "lblPropietarioEmail";
            this.lblPropietarioEmail.Size = new System.Drawing.Size(100, 20);
            this.lblPropietarioEmail.Text = "Email:";
            this.lblPropietarioEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblPropietarioEmailValor
            //
            this.lblPropietarioEmailValor.AutoEllipsis = true;
            this.lblPropietarioEmailValor.Location = new System.Drawing.Point(432, 75);
            this.lblPropietarioEmailValor.Name = "lblPropietarioEmailValor";
            this.lblPropietarioEmailValor.Size = new System.Drawing.Size(200, 20);
            this.lblPropietarioEmailValor.Text = "-";
            this.lblPropietarioEmailValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tabInquilino
            //
            this.tabInquilino.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.tabInquilino.Controls.Add(this.grpInquilino);
            this.tabInquilino.ForeColor = System.Drawing.Color.White;
            this.tabInquilino.Location = new System.Drawing.Point(4, 22);
            this.tabInquilino.Name = "tabInquilino";
            this.tabInquilino.Padding = new System.Windows.Forms.Padding(3);
            this.tabInquilino.Size = new System.Drawing.Size(668, 420);
            this.tabInquilino.Text = "Inquilino";
            //
            // grpInquilino
            //
            this.grpInquilino.BackColor = System.Drawing.Color.Transparent;
            this.grpInquilino.Controls.Add(this.lblDni);
            this.grpInquilino.Controls.Add(this.txtDni);
            this.grpInquilino.Controls.Add(this.btnBuscarInquilino);
            this.grpInquilino.Controls.Add(this.lblNombre);
            this.grpInquilino.Controls.Add(this.txtNombre);
            this.grpInquilino.Controls.Add(this.lblApellido);
            this.grpInquilino.Controls.Add(this.txtApellido);
            this.grpInquilino.Controls.Add(this.lblTelefono);
            this.grpInquilino.Controls.Add(this.txtTelefono);
            this.grpInquilino.Controls.Add(this.lblEmail);
            this.grpInquilino.Controls.Add(this.txtEmail);
            this.grpInquilino.ForeColor = System.Drawing.Color.White;
            this.grpInquilino.Location = new System.Drawing.Point(10, 15);
            this.grpInquilino.Name = "grpInquilino";
            this.grpInquilino.Size = new System.Drawing.Size(646, 130);
            this.grpInquilino.TabStop = false;
            this.grpInquilino.Text = "Inquilino";
            //
            // lblDni
            //
            this.lblDni.Location = new System.Drawing.Point(6, 25);
            this.lblDni.Name = "lblDni";
            this.lblDni.Size = new System.Drawing.Size(130, 20);
            this.lblDni.Text = "DNI:";
            this.lblDni.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtDni
            //
            this.txtDni.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtDni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDni.ForeColor = System.Drawing.Color.White;
            this.txtDni.Location = new System.Drawing.Point(142, 25);
            this.txtDni.MaxLength = 15;
            this.txtDni.Name = "txtDni";
            this.txtDni.Size = new System.Drawing.Size(180, 20);
            this.txtDni.TextChanged += new System.EventHandler(this.txtDni_TextChanged);
            //
            // btnBuscarInquilino
            //
            this.btnBuscarInquilino.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnBuscarInquilino.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuscarInquilino.ForeColor = System.Drawing.Color.White;
            this.btnBuscarInquilino.Location = new System.Drawing.Point(330, 21);
            this.btnBuscarInquilino.Name = "btnBuscarInquilino";
            this.btnBuscarInquilino.Size = new System.Drawing.Size(96, 27);
            this.btnBuscarInquilino.Text = "Buscar";
            this.btnBuscarInquilino.UseVisualStyleBackColor = false;
            this.btnBuscarInquilino.Click += new System.EventHandler(this.btnBuscarInquilino_Click);
            //
            // lblNombre
            //
            this.lblNombre.Location = new System.Drawing.Point(6, 60);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(130, 20);
            this.lblNombre.Text = "Nombre:";
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtNombre
            //
            this.txtNombre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.ForeColor = System.Drawing.Color.White;
            this.txtNombre.Location = new System.Drawing.Point(142, 60);
            this.txtNombre.MaxLength = 100;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(180, 20);
            //
            // lblApellido
            //
            this.lblApellido.Location = new System.Drawing.Point(326, 60);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(100, 20);
            this.lblApellido.Text = "Apellido:";
            this.lblApellido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtApellido
            //
            this.txtApellido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtApellido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtApellido.ForeColor = System.Drawing.Color.White;
            this.txtApellido.Location = new System.Drawing.Point(432, 60);
            this.txtApellido.MaxLength = 100;
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(200, 20);
            //
            // lblTelefono
            //
            this.lblTelefono.Location = new System.Drawing.Point(6, 95);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(130, 20);
            this.lblTelefono.Text = "Teléfono:";
            this.lblTelefono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtTelefono
            //
            this.txtTelefono.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtTelefono.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTelefono.ForeColor = System.Drawing.Color.White;
            this.txtTelefono.Location = new System.Drawing.Point(142, 95);
            this.txtTelefono.MaxLength = 30;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(180, 20);
            //
            // lblEmail
            //
            this.lblEmail.Location = new System.Drawing.Point(326, 95);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(100, 20);
            this.lblEmail.Text = "Email:";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtEmail
            //
            this.txtEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.ForeColor = System.Drawing.Color.White;
            this.txtEmail.Location = new System.Drawing.Point(432, 95);
            this.txtEmail.MaxLength = 100;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(200, 20);
            //
            // tabGarante
            //
            this.tabGarante.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.tabGarante.Controls.Add(this.grpGarante);
            this.tabGarante.ForeColor = System.Drawing.Color.White;
            this.tabGarante.Location = new System.Drawing.Point(4, 22);
            this.tabGarante.Name = "tabGarante";
            this.tabGarante.Padding = new System.Windows.Forms.Padding(3);
            this.tabGarante.Size = new System.Drawing.Size(668, 420);
            this.tabGarante.Text = "Garante";
            //
            // grpGarante
            //
            this.grpGarante.BackColor = System.Drawing.Color.Transparent;
            this.grpGarante.Controls.Add(this.lblGaranteNombre);
            this.grpGarante.Controls.Add(this.txtGaranteNombre);
            this.grpGarante.Controls.Add(this.lblGaranteApellido);
            this.grpGarante.Controls.Add(this.txtGaranteApellido);
            this.grpGarante.Controls.Add(this.lblGaranteDni);
            this.grpGarante.Controls.Add(this.txtGaranteDni);
            this.grpGarante.Controls.Add(this.lblGaranteDomicilio);
            this.grpGarante.Controls.Add(this.txtGaranteDomicilio);
            this.grpGarante.Controls.Add(this.lblPropiedadGarantia);
            this.grpGarante.Controls.Add(this.txtPropiedadGarantia);
            this.grpGarante.ForeColor = System.Drawing.Color.White;
            this.grpGarante.Location = new System.Drawing.Point(10, 15);
            this.grpGarante.Name = "grpGarante";
            this.grpGarante.Size = new System.Drawing.Size(646, 165);
            this.grpGarante.TabStop = false;
            this.grpGarante.Text = "Garante (obligatorio)";
            //
            // lblGaranteNombre
            //
            this.lblGaranteNombre.Location = new System.Drawing.Point(6, 25);
            this.lblGaranteNombre.Name = "lblGaranteNombre";
            this.lblGaranteNombre.Size = new System.Drawing.Size(130, 20);
            this.lblGaranteNombre.Text = "Nombre:";
            this.lblGaranteNombre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtGaranteNombre
            //
            this.txtGaranteNombre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtGaranteNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGaranteNombre.ForeColor = System.Drawing.Color.White;
            this.txtGaranteNombre.Location = new System.Drawing.Point(142, 25);
            this.txtGaranteNombre.MaxLength = 100;
            this.txtGaranteNombre.Name = "txtGaranteNombre";
            this.txtGaranteNombre.Size = new System.Drawing.Size(180, 20);
            //
            // lblGaranteApellido
            //
            this.lblGaranteApellido.Location = new System.Drawing.Point(326, 25);
            this.lblGaranteApellido.Name = "lblGaranteApellido";
            this.lblGaranteApellido.Size = new System.Drawing.Size(100, 20);
            this.lblGaranteApellido.Text = "Apellido:";
            this.lblGaranteApellido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtGaranteApellido
            //
            this.txtGaranteApellido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtGaranteApellido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGaranteApellido.ForeColor = System.Drawing.Color.White;
            this.txtGaranteApellido.Location = new System.Drawing.Point(432, 25);
            this.txtGaranteApellido.MaxLength = 100;
            this.txtGaranteApellido.Name = "txtGaranteApellido";
            this.txtGaranteApellido.Size = new System.Drawing.Size(200, 20);
            //
            // lblGaranteDni
            //
            this.lblGaranteDni.Location = new System.Drawing.Point(6, 60);
            this.lblGaranteDni.Name = "lblGaranteDni";
            this.lblGaranteDni.Size = new System.Drawing.Size(130, 20);
            this.lblGaranteDni.Text = "DNI:";
            this.lblGaranteDni.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtGaranteDni
            //
            this.txtGaranteDni.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtGaranteDni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGaranteDni.ForeColor = System.Drawing.Color.White;
            this.txtGaranteDni.Location = new System.Drawing.Point(142, 60);
            this.txtGaranteDni.MaxLength = 20;
            this.txtGaranteDni.Name = "txtGaranteDni";
            this.txtGaranteDni.Size = new System.Drawing.Size(180, 20);
            //
            // lblGaranteDomicilio
            //
            this.lblGaranteDomicilio.Location = new System.Drawing.Point(6, 95);
            this.lblGaranteDomicilio.Name = "lblGaranteDomicilio";
            this.lblGaranteDomicilio.Size = new System.Drawing.Size(130, 20);
            this.lblGaranteDomicilio.Text = "Domicilio:";
            this.lblGaranteDomicilio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtGaranteDomicilio
            //
            this.txtGaranteDomicilio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtGaranteDomicilio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGaranteDomicilio.ForeColor = System.Drawing.Color.White;
            this.txtGaranteDomicilio.Location = new System.Drawing.Point(142, 95);
            this.txtGaranteDomicilio.MaxLength = 200;
            this.txtGaranteDomicilio.Name = "txtGaranteDomicilio";
            this.txtGaranteDomicilio.Size = new System.Drawing.Size(490, 20);
            //
            // lblPropiedadGarantia
            //
            this.lblPropiedadGarantia.Location = new System.Drawing.Point(6, 130);
            this.lblPropiedadGarantia.Name = "lblPropiedadGarantia";
            this.lblPropiedadGarantia.Size = new System.Drawing.Size(130, 20);
            this.lblPropiedadGarantia.Text = "Propiedad en garantía:";
            this.lblPropiedadGarantia.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtPropiedadGarantia
            //
            this.txtPropiedadGarantia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtPropiedadGarantia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPropiedadGarantia.ForeColor = System.Drawing.Color.White;
            this.txtPropiedadGarantia.Location = new System.Drawing.Point(142, 130);
            this.txtPropiedadGarantia.MaxLength = 200;
            this.txtPropiedadGarantia.Name = "txtPropiedadGarantia";
            this.txtPropiedadGarantia.Size = new System.Drawing.Size(490, 20);
            //
            // tabCondiciones
            //
            this.tabCondiciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.tabCondiciones.Controls.Add(this.grpContrato);
            this.tabCondiciones.ForeColor = System.Drawing.Color.White;
            this.tabCondiciones.Location = new System.Drawing.Point(4, 22);
            this.tabCondiciones.Name = "tabCondiciones";
            this.tabCondiciones.Padding = new System.Windows.Forms.Padding(3);
            this.tabCondiciones.Size = new System.Drawing.Size(668, 420);
            this.tabCondiciones.Text = "Condiciones";
            //
            // grpContrato
            //
            this.grpContrato.BackColor = System.Drawing.Color.Transparent;
            this.grpContrato.Controls.Add(this.lblFechaInicio);
            this.grpContrato.Controls.Add(this.dtpFechaInicio);
            this.grpContrato.Controls.Add(this.lblDuracion);
            this.grpContrato.Controls.Add(this.nudDuracion);
            this.grpContrato.Controls.Add(this.lblMonto);
            this.grpContrato.Controls.Add(this.nudMonto);
            this.grpContrato.Controls.Add(this.lblMetodoPago);
            this.grpContrato.Controls.Add(this.cmbMetodoPago);
            this.grpContrato.Controls.Add(this.lblDeposito);
            this.grpContrato.Controls.Add(this.lblDepositoValor);
            this.grpContrato.Controls.Add(this.lblComision);
            this.grpContrato.Controls.Add(this.lblComisionValor);
            this.grpContrato.Controls.Add(this.lblActualizacion);
            this.grpContrato.Controls.Add(this.lblActualizacionValor);
            this.grpContrato.Controls.Add(this.lblFechaFin);
            this.grpContrato.Controls.Add(this.lblFechaFinValor);
            this.grpContrato.ForeColor = System.Drawing.Color.White;
            this.grpContrato.Location = new System.Drawing.Point(10, 15);
            this.grpContrato.Name = "grpContrato";
            this.grpContrato.Size = new System.Drawing.Size(646, 275);
            this.grpContrato.TabStop = false;
            this.grpContrato.Text = "Contrato";
            //
            // lblFechaInicio
            //
            this.lblFechaInicio.Location = new System.Drawing.Point(6, 25);
            this.lblFechaInicio.Name = "lblFechaInicio";
            this.lblFechaInicio.Size = new System.Drawing.Size(130, 20);
            this.lblFechaInicio.Text = "Fecha de inicio:";
            this.lblFechaInicio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dtpFechaInicio
            //
            this.dtpFechaInicio.CalendarForeColor = System.Drawing.Color.White;
            this.dtpFechaInicio.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.dtpFechaInicio.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.dtpFechaInicio.CalendarTitleForeColor = System.Drawing.Color.White;
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaInicio.Location = new System.Drawing.Point(142, 25);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(180, 20);
            this.dtpFechaInicio.ValueChanged += new System.EventHandler(this.DatosContrato_Changed);
            //
            // lblDuracion
            //
            this.lblDuracion.Location = new System.Drawing.Point(326, 25);
            this.lblDuracion.Name = "lblDuracion";
            this.lblDuracion.Size = new System.Drawing.Size(100, 20);
            this.lblDuracion.Text = "Duración (meses):";
            this.lblDuracion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudDuracion
            //
            this.nudDuracion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudDuracion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudDuracion.ForeColor = System.Drawing.Color.White;
            this.nudDuracion.Location = new System.Drawing.Point(432, 25);
            this.nudDuracion.Maximum = new decimal(new int[] { 240, 0, 0, 0 });
            this.nudDuracion.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudDuracion.Name = "nudDuracion";
            this.nudDuracion.Size = new System.Drawing.Size(80, 20);
            this.nudDuracion.Value = new decimal(new int[] { 24, 0, 0, 0 });
            this.nudDuracion.ValueChanged += new System.EventHandler(this.DatosContrato_Changed);
            //
            // lblMonto
            //
            this.lblMonto.Location = new System.Drawing.Point(6, 60);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(130, 20);
            this.lblMonto.Text = "Monto mensual:";
            this.lblMonto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudMonto
            //
            this.nudMonto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudMonto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudMonto.DecimalPlaces = 2;
            this.nudMonto.ForeColor = System.Drawing.Color.White;
            this.nudMonto.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudMonto.Location = new System.Drawing.Point(142, 60);
            this.nudMonto.Maximum = new decimal(new int[] { 1215752191, 23, 0, 0 });
            this.nudMonto.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.nudMonto.Name = "nudMonto";
            this.nudMonto.Size = new System.Drawing.Size(180, 20);
            this.nudMonto.ThousandsSeparator = true;
            this.nudMonto.Value = new decimal(new int[] { 0, 0, 0, 0 });
            this.nudMonto.ValueChanged += new System.EventHandler(this.DatosContrato_Changed);
            //
            // lblMetodoPago
            //
            this.lblMetodoPago.Location = new System.Drawing.Point(326, 60);
            this.lblMetodoPago.Name = "lblMetodoPago";
            this.lblMetodoPago.Size = new System.Drawing.Size(100, 20);
            this.lblMetodoPago.Text = "Método de pago:";
            this.lblMetodoPago.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cmbMetodoPago
            //
            this.cmbMetodoPago.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetodoPago.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbMetodoPago.ForeColor = System.Drawing.Color.White;
            this.cmbMetodoPago.FormattingEnabled = true;
            this.cmbMetodoPago.Location = new System.Drawing.Point(432, 60);
            this.cmbMetodoPago.Name = "cmbMetodoPago";
            this.cmbMetodoPago.Size = new System.Drawing.Size(200, 21);
            this.cmbMetodoPago.SelectedIndexChanged += new System.EventHandler(this.DatosContrato_Changed);
            //
            // lblDeposito
            //
            this.lblDeposito.Location = new System.Drawing.Point(6, 100);
            this.lblDeposito.Name = "lblDeposito";
            this.lblDeposito.Size = new System.Drawing.Size(130, 20);
            this.lblDeposito.Text = "Depósito de garantía:";
            this.lblDeposito.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDepositoValor
            //
            this.lblDepositoValor.AutoEllipsis = true;
            this.lblDepositoValor.Location = new System.Drawing.Point(142, 100);
            this.lblDepositoValor.Name = "lblDepositoValor";
            this.lblDepositoValor.Size = new System.Drawing.Size(490, 20);
            this.lblDepositoValor.Text = "-";
            this.lblDepositoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblComision
            //
            this.lblComision.Location = new System.Drawing.Point(6, 130);
            this.lblComision.Name = "lblComision";
            this.lblComision.Size = new System.Drawing.Size(130, 20);
            this.lblComision.Text = "Comisión inmobiliaria:";
            this.lblComision.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblComisionValor
            //
            this.lblComisionValor.AutoEllipsis = true;
            this.lblComisionValor.Location = new System.Drawing.Point(142, 130);
            this.lblComisionValor.Name = "lblComisionValor";
            this.lblComisionValor.Size = new System.Drawing.Size(490, 20);
            this.lblComisionValor.Text = "-";
            this.lblComisionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblActualizacion
            //
            this.lblActualizacion.Location = new System.Drawing.Point(6, 160);
            this.lblActualizacion.Name = "lblActualizacion";
            this.lblActualizacion.Size = new System.Drawing.Size(130, 20);
            this.lblActualizacion.Text = "Actualización:";
            this.lblActualizacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblActualizacionValor
            //
            this.lblActualizacionValor.AutoEllipsis = true;
            this.lblActualizacionValor.Location = new System.Drawing.Point(142, 160);
            this.lblActualizacionValor.Name = "lblActualizacionValor";
            this.lblActualizacionValor.Size = new System.Drawing.Size(490, 20);
            this.lblActualizacionValor.Text = "-";
            this.lblActualizacionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblFechaFin
            //
            this.lblFechaFin.Location = new System.Drawing.Point(6, 190);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(130, 20);
            this.lblFechaFin.Text = "Finaliza:";
            this.lblFechaFin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblFechaFinValor
            //
            this.lblFechaFinValor.Location = new System.Drawing.Point(142, 190);
            this.lblFechaFinValor.Name = "lblFechaFinValor";
            this.lblFechaFinValor.Size = new System.Drawing.Size(490, 40);
            this.lblFechaFinValor.Text = "-";
            this.lblFechaFinValor.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // tabResumen
            //
            this.tabResumen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.tabResumen.Controls.Add(this.txtResumen);
            this.tabResumen.Controls.Add(this.lblFirma);
            this.tabResumen.Controls.Add(this.txtFirma);
            this.tabResumen.Controls.Add(this.btnRegistrar);
            this.tabResumen.Controls.Add(this.btnGenerarPdf);
            this.tabResumen.Controls.Add(this.btnNuevoContrato);
            this.tabResumen.Controls.Add(this.lblEstadoRegistro);
            this.tabResumen.ForeColor = System.Drawing.Color.White;
            this.tabResumen.Location = new System.Drawing.Point(4, 22);
            this.tabResumen.Name = "tabResumen";
            this.tabResumen.Padding = new System.Windows.Forms.Padding(3);
            this.tabResumen.Size = new System.Drawing.Size(668, 420);
            this.tabResumen.Text = "Resumen y PDF";
            //
            // txtResumen
            //
            this.txtResumen.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtResumen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtResumen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResumen.ForeColor = System.Drawing.Color.White;
            this.txtResumen.Location = new System.Drawing.Point(10, 15);
            this.txtResumen.MaxLength = 32767;
            this.txtResumen.Name = "txtResumen";
            this.txtResumen.Size = new System.Drawing.Size(646, 255);
            this.txtResumen.Multiline = true;
            this.txtResumen.ReadOnly = true;
            this.txtResumen.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResumen.TabStop = false;
            //
            // lblFirma
            //
            this.lblFirma.Location = new System.Drawing.Point(10, 280);
            this.lblFirma.Name = "lblFirma";
            this.lblFirma.Size = new System.Drawing.Size(250, 20);
            this.lblFirma.Text = "Firma del inquilino (nombre y apellido):";
            this.lblFirma.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtFirma
            //
            this.txtFirma.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtFirma.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFirma.ForeColor = System.Drawing.Color.White;
            this.txtFirma.Location = new System.Drawing.Point(266, 280);
            this.txtFirma.MaxLength = 200;
            this.txtFirma.Name = "txtFirma";
            this.txtFirma.Size = new System.Drawing.Size(270, 20);
            //
            // btnRegistrar
            //
            this.btnRegistrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRegistrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRegistrar.ForeColor = System.Drawing.Color.White;
            this.btnRegistrar.Location = new System.Drawing.Point(10, 318);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(160, 34);
            this.btnRegistrar.Text = "Registrar Contrato";
            this.btnRegistrar.UseVisualStyleBackColor = false;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // btnGenerarPdf
            //
            this.btnGenerarPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnGenerarPdf.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGenerarPdf.ForeColor = System.Drawing.Color.White;
            this.btnGenerarPdf.Location = new System.Drawing.Point(180, 318);
            this.btnGenerarPdf.Name = "btnGenerarPdf";
            this.btnGenerarPdf.Size = new System.Drawing.Size(140, 34);
            this.btnGenerarPdf.Text = "Generar PDF";
            this.btnGenerarPdf.UseVisualStyleBackColor = false;
            this.btnGenerarPdf.Enabled = false;
            this.btnGenerarPdf.Click += new System.EventHandler(this.btnGenerarPdf_Click);
            //
            // btnNuevoContrato
            //
            this.btnNuevoContrato.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnNuevoContrato.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNuevoContrato.ForeColor = System.Drawing.Color.White;
            this.btnNuevoContrato.Location = new System.Drawing.Point(330, 318);
            this.btnNuevoContrato.Name = "btnNuevoContrato";
            this.btnNuevoContrato.Size = new System.Drawing.Size(140, 34);
            this.btnNuevoContrato.Text = "Nuevo Contrato";
            this.btnNuevoContrato.UseVisualStyleBackColor = false;
            this.btnNuevoContrato.Enabled = false;
            this.btnNuevoContrato.Click += new System.EventHandler(this.btnNuevoContrato_Click);
            //
            // lblEstadoRegistro
            //
            this.lblEstadoRegistro.AutoEllipsis = true;
            this.lblEstadoRegistro.Location = new System.Drawing.Point(10, 362);
            this.lblEstadoRegistro.Name = "lblEstadoRegistro";
            this.lblEstadoRegistro.Size = new System.Drawing.Size(646, 44);
            this.lblEstadoRegistro.Text = "";
            this.lblEstadoRegistro.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnAnterior
            //
            this.btnAnterior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAnterior.ForeColor = System.Drawing.Color.White;
            this.btnAnterior.Location = new System.Drawing.Point(12, 468);
            this.btnAnterior.Name = "btnAnterior";
            this.btnAnterior.Size = new System.Drawing.Size(105, 32);
            this.btnAnterior.Text = "< Anterior";
            this.btnAnterior.UseVisualStyleBackColor = false;
            this.btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            //
            // btnSiguiente
            //
            this.btnSiguiente.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSiguiente.ForeColor = System.Drawing.Color.White;
            this.btnSiguiente.Location = new System.Drawing.Point(123, 468);
            this.btnSiguiente.Name = "btnSiguiente";
            this.btnSiguiente.Size = new System.Drawing.Size(105, 32);
            this.btnSiguiente.Text = "Siguiente >";
            this.btnSiguiente.UseVisualStyleBackColor = false;
            this.btnSiguiente.Click += new System.EventHandler(this.btnSiguiente_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(583, 468);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(105, 32);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // FormRegistrarContrato
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(700, 512);
            this.Controls.Add(this.tabContrato);
            this.Controls.Add(this.btnAnterior);
            this.Controls.Add(this.btnSiguiente);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormRegistrarContrato";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registrar Contrato";
            this.Load += new System.EventHandler(this.FormRegistrarContrato_Load);
            this.tabContrato.ResumeLayout(false);
            this.tabInmueble.ResumeLayout(false);
            this.tabInmueble.PerformLayout();
            this.grpDatosInmueble.ResumeLayout(false);
            this.grpDatosInmueble.PerformLayout();
            this.grpPropietario.ResumeLayout(false);
            this.grpPropietario.PerformLayout();
            this.tabInquilino.ResumeLayout(false);
            this.tabInquilino.PerformLayout();
            this.grpInquilino.ResumeLayout(false);
            this.grpInquilino.PerformLayout();
            this.tabGarante.ResumeLayout(false);
            this.tabGarante.PerformLayout();
            this.grpGarante.ResumeLayout(false);
            this.grpGarante.PerformLayout();
            this.tabCondiciones.ResumeLayout(false);
            this.tabCondiciones.PerformLayout();
            this.grpContrato.ResumeLayout(false);
            this.grpContrato.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuracion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonto)).EndInit();
            this.tabResumen.ResumeLayout(false);
            this.tabResumen.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabContrato;
        private System.Windows.Forms.TabPage tabInmueble;
        private System.Windows.Forms.Label lblInmueble;
        private System.Windows.Forms.ComboBox cmbInmueble;
        private System.Windows.Forms.GroupBox grpDatosInmueble;
        private System.Windows.Forms.Label lblTipoInmueble;
        private System.Windows.Forms.Label lblTipoInmuebleValor;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.Label lblDireccionValor;
        private System.Windows.Forms.Label lblSuperficie;
        private System.Windows.Forms.Label lblSuperficieValor;
        private System.Windows.Forms.Label lblAmbientes;
        private System.Windows.Forms.Label lblAmbientesValor;
        private System.Windows.Forms.Label lblValorTasado;
        private System.Windows.Forms.Label lblValorTasadoValor;
        private System.Windows.Forms.GroupBox grpPropietario;
        private System.Windows.Forms.Label lblPropietarioNombre;
        private System.Windows.Forms.Label lblPropietarioNombreValor;
        private System.Windows.Forms.Label lblPropietarioDni;
        private System.Windows.Forms.Label lblPropietarioDniValor;
        private System.Windows.Forms.Label lblPropietarioTelefono;
        private System.Windows.Forms.Label lblPropietarioTelefonoValor;
        private System.Windows.Forms.Label lblPropietarioEmail;
        private System.Windows.Forms.Label lblPropietarioEmailValor;
        private System.Windows.Forms.TabPage tabInquilino;
        private System.Windows.Forms.GroupBox grpInquilino;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.Button btnBuscarInquilino;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblApellido;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TabPage tabGarante;
        private System.Windows.Forms.GroupBox grpGarante;
        private System.Windows.Forms.Label lblGaranteNombre;
        private System.Windows.Forms.TextBox txtGaranteNombre;
        private System.Windows.Forms.Label lblGaranteApellido;
        private System.Windows.Forms.TextBox txtGaranteApellido;
        private System.Windows.Forms.Label lblGaranteDni;
        private System.Windows.Forms.TextBox txtGaranteDni;
        private System.Windows.Forms.Label lblGaranteDomicilio;
        private System.Windows.Forms.TextBox txtGaranteDomicilio;
        private System.Windows.Forms.Label lblPropiedadGarantia;
        private System.Windows.Forms.TextBox txtPropiedadGarantia;
        private System.Windows.Forms.TabPage tabCondiciones;
        private System.Windows.Forms.GroupBox grpContrato;
        private System.Windows.Forms.Label lblFechaInicio;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private System.Windows.Forms.Label lblDuracion;
        private System.Windows.Forms.NumericUpDown nudDuracion;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.NumericUpDown nudMonto;
        private System.Windows.Forms.Label lblMetodoPago;
        private System.Windows.Forms.ComboBox cmbMetodoPago;
        private System.Windows.Forms.Label lblDeposito;
        private System.Windows.Forms.Label lblDepositoValor;
        private System.Windows.Forms.Label lblComision;
        private System.Windows.Forms.Label lblComisionValor;
        private System.Windows.Forms.Label lblActualizacion;
        private System.Windows.Forms.Label lblActualizacionValor;
        private System.Windows.Forms.Label lblFechaFin;
        private System.Windows.Forms.Label lblFechaFinValor;
        private System.Windows.Forms.TabPage tabResumen;
        private System.Windows.Forms.TextBox txtResumen;
        private System.Windows.Forms.Label lblFirma;
        private System.Windows.Forms.TextBox txtFirma;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.Button btnGenerarPdf;
        private System.Windows.Forms.Button btnNuevoContrato;
        private System.Windows.Forms.Label lblEstadoRegistro;
        private System.Windows.Forms.Button btnAnterior;
        private System.Windows.Forms.Button btnSiguiente;
        private System.Windows.Forms.Button btnCancelar;
    }
}
