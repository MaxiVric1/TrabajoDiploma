namespace TpIngSoft
{
    partial class FormRegistrarCaptacion
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
            this.grpCliente = new System.Windows.Forms.GroupBox();
            this.lblDni = new System.Windows.Forms.Label();
            this.txtDni = new System.Windows.Forms.TextBox();
            this.btnBuscarCliente = new System.Windows.Forms.Button();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblApellido = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.grpInmueble = new System.Windows.Forms.GroupBox();
            this.lblTipo = new System.Windows.Forms.Label();
            this.cmbTipo = new System.Windows.Forms.ComboBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.lblMetros = new System.Windows.Forms.Label();
            this.nudMetros = new System.Windows.Forms.NumericUpDown();
            this.lblAmbientes = new System.Windows.Forms.Label();
            this.nudAmbientes = new System.Windows.Forms.NumericUpDown();
            this.grpCaptacion = new System.Windows.Forms.GroupBox();
            this.lblValorPretendido = new System.Windows.Forms.Label();
            this.nudValorPretendido = new System.Windows.Forms.NumericUpDown();
            this.chkAutorizacionRecibida = new System.Windows.Forms.CheckBox();
            this.lblTipoPublicacion = new System.Windows.Forms.Label();
            this.cmbTipoPublicacion = new System.Windows.Forms.ComboBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblLocalidad = new System.Windows.Forms.Label();
            this.txtLocalidad = new System.Windows.Forms.TextBox();
            this.lblSuperficieLote = new System.Windows.Forms.Label();
            this.nudSuperficieLote = new System.Windows.Forms.NumericUpDown();
            this.grpCliente.SuspendLayout();
            this.grpInmueble.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMetros)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmbientes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSuperficieLote)).BeginInit();
            this.grpCaptacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValorPretendido)).BeginInit();
            this.SuspendLayout();
            //
            // grpCliente
            //
            this.grpCliente.BackColor = System.Drawing.Color.Transparent;
            this.grpCliente.Controls.Add(this.lblDni);
            this.grpCliente.Controls.Add(this.txtDni);
            this.grpCliente.Controls.Add(this.btnBuscarCliente);
            this.grpCliente.Controls.Add(this.lblNombre);
            this.grpCliente.Controls.Add(this.txtNombre);
            this.grpCliente.Controls.Add(this.lblApellido);
            this.grpCliente.Controls.Add(this.txtApellido);
            this.grpCliente.Controls.Add(this.lblTelefono);
            this.grpCliente.Controls.Add(this.txtTelefono);
            this.grpCliente.Controls.Add(this.lblEmail);
            this.grpCliente.Controls.Add(this.txtEmail);
            this.grpCliente.ForeColor = System.Drawing.Color.White;
            this.grpCliente.Location = new System.Drawing.Point(12, 12);
            this.grpCliente.Name = "grpCliente";
            this.grpCliente.Size = new System.Drawing.Size(616, 130);
            this.grpCliente.TabIndex = 0;
            this.grpCliente.TabStop = false;
            this.grpCliente.Text = "Cliente";
            //
            // lblDni
            //
            this.lblDni.Location = new System.Drawing.Point(6, 25);
            this.lblDni.Name = "lblDni";
            this.lblDni.Size = new System.Drawing.Size(100, 20);
            this.lblDni.TabIndex = 0;
            this.lblDni.Text = "DNI:";
            this.lblDni.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtDni
            //
            this.txtDni.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtDni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDni.ForeColor = System.Drawing.Color.White;
            this.txtDni.Location = new System.Drawing.Point(112, 25);
            this.txtDni.MaxLength = 15;
            this.txtDni.Name = "txtDni";
            this.txtDni.Size = new System.Drawing.Size(180, 20);
            this.txtDni.TabIndex = 1;
            this.txtDni.TextChanged += new System.EventHandler(this.txtDni_TextChanged);
            //
            // btnBuscarCliente
            //
            this.btnBuscarCliente.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnBuscarCliente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuscarCliente.ForeColor = System.Drawing.Color.White;
            this.btnBuscarCliente.Location = new System.Drawing.Point(300, 21);
            this.btnBuscarCliente.Name = "btnBuscarCliente";
            this.btnBuscarCliente.Size = new System.Drawing.Size(96, 27);
            this.btnBuscarCliente.TabIndex = 2;
            this.btnBuscarCliente.Text = "Buscar";
            this.btnBuscarCliente.UseVisualStyleBackColor = false;
            this.btnBuscarCliente.Click += new System.EventHandler(this.btnBuscarCliente_Click);
            //
            // lblNombre
            //
            this.lblNombre.Location = new System.Drawing.Point(6, 60);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(100, 20);
            this.lblNombre.TabIndex = 3;
            this.lblNombre.Text = "Nombre:";
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtNombre
            //
            this.txtNombre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.ForeColor = System.Drawing.Color.White;
            this.txtNombre.Location = new System.Drawing.Point(112, 60);
            this.txtNombre.MaxLength = 100;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(180, 20);
            this.txtNombre.TabIndex = 4;
            //
            // lblApellido
            //
            this.lblApellido.Location = new System.Drawing.Point(296, 60);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(100, 20);
            this.lblApellido.TabIndex = 5;
            this.lblApellido.Text = "Apellido:";
            this.lblApellido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtApellido
            //
            this.txtApellido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtApellido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtApellido.ForeColor = System.Drawing.Color.White;
            this.txtApellido.Location = new System.Drawing.Point(402, 60);
            this.txtApellido.MaxLength = 100;
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(200, 20);
            this.txtApellido.TabIndex = 6;
            //
            // lblTelefono
            //
            this.lblTelefono.Location = new System.Drawing.Point(6, 95);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(100, 20);
            this.lblTelefono.TabIndex = 7;
            this.lblTelefono.Text = "Teléfono:";
            this.lblTelefono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtTelefono
            //
            this.txtTelefono.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtTelefono.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTelefono.ForeColor = System.Drawing.Color.White;
            this.txtTelefono.Location = new System.Drawing.Point(112, 95);
            this.txtTelefono.MaxLength = 30;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(180, 20);
            this.txtTelefono.TabIndex = 8;
            //
            // lblEmail
            //
            this.lblEmail.Location = new System.Drawing.Point(296, 95);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(100, 20);
            this.lblEmail.TabIndex = 9;
            this.lblEmail.Text = "Email:";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtEmail
            //
            this.txtEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.ForeColor = System.Drawing.Color.White;
            this.txtEmail.Location = new System.Drawing.Point(402, 95);
            this.txtEmail.MaxLength = 100;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(200, 20);
            this.txtEmail.TabIndex = 10;
            //
            // grpInmueble
            //
            this.grpInmueble.BackColor = System.Drawing.Color.Transparent;
            this.grpInmueble.Controls.Add(this.lblTipo);
            this.grpInmueble.Controls.Add(this.cmbTipo);
            this.grpInmueble.Controls.Add(this.lblDireccion);
            this.grpInmueble.Controls.Add(this.txtDireccion);
            this.grpInmueble.Controls.Add(this.lblMetros);
            this.grpInmueble.Controls.Add(this.nudMetros);
            this.grpInmueble.Controls.Add(this.lblAmbientes);
            this.grpInmueble.Controls.Add(this.nudAmbientes);
            this.grpInmueble.Controls.Add(this.lblLocalidad);
            this.grpInmueble.Controls.Add(this.txtLocalidad);
            this.grpInmueble.Controls.Add(this.lblSuperficieLote);
            this.grpInmueble.Controls.Add(this.nudSuperficieLote);
            this.grpInmueble.ForeColor = System.Drawing.Color.White;
            this.grpInmueble.Location = new System.Drawing.Point(12, 152);
            this.grpInmueble.Name = "grpInmueble";
            this.grpInmueble.Size = new System.Drawing.Size(616, 130);
            this.grpInmueble.TabIndex = 1;
            this.grpInmueble.TabStop = false;
            this.grpInmueble.Text = "Inmueble";
            //
            // lblTipo
            //
            this.lblTipo.Location = new System.Drawing.Point(6, 25);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(100, 21);
            this.lblTipo.TabIndex = 0;
            this.lblTipo.Text = "Tipo:";
            this.lblTipo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cmbTipo
            //
            this.cmbTipo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbTipo.ForeColor = System.Drawing.Color.White;
            this.cmbTipo.FormattingEnabled = true;
            this.cmbTipo.Location = new System.Drawing.Point(112, 25);
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Size = new System.Drawing.Size(180, 21);
            this.cmbTipo.TabIndex = 1;
            //
            // lblDireccion
            //
            this.lblDireccion.Location = new System.Drawing.Point(296, 25);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(100, 21);
            this.lblDireccion.TabIndex = 2;
            this.lblDireccion.Text = "Dirección:";
            this.lblDireccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtDireccion
            //
            this.txtDireccion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtDireccion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDireccion.ForeColor = System.Drawing.Color.White;
            this.txtDireccion.Location = new System.Drawing.Point(402, 25);
            this.txtDireccion.MaxLength = 200;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(200, 20);
            this.txtDireccion.TabIndex = 3;
            //
            // lblMetros
            //
            this.lblMetros.Location = new System.Drawing.Point(6, 60);
            this.lblMetros.Name = "lblMetros";
            this.lblMetros.Size = new System.Drawing.Size(100, 20);
            this.lblMetros.TabIndex = 4;
            this.lblMetros.Text = "Metros cuadrados:";
            this.lblMetros.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudMetros
            //
            this.nudMetros.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudMetros.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudMetros.DecimalPlaces = 2;
            this.nudMetros.ForeColor = System.Drawing.Color.White;
            this.nudMetros.Location = new System.Drawing.Point(112, 60);
            this.nudMetros.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudMetros.Name = "nudMetros";
            this.nudMetros.Size = new System.Drawing.Size(180, 20);
            this.nudMetros.TabIndex = 5;
            this.nudMetros.ThousandsSeparator = true;
            //
            // lblAmbientes
            //
            this.lblAmbientes.Location = new System.Drawing.Point(296, 60);
            this.lblAmbientes.Name = "lblAmbientes";
            this.lblAmbientes.Size = new System.Drawing.Size(100, 20);
            this.lblAmbientes.TabIndex = 6;
            this.lblAmbientes.Text = "Ambientes:";
            this.lblAmbientes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudAmbientes
            //
            this.nudAmbientes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudAmbientes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudAmbientes.ForeColor = System.Drawing.Color.White;
            this.nudAmbientes.Location = new System.Drawing.Point(402, 60);
            this.nudAmbientes.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudAmbientes.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudAmbientes.Name = "nudAmbientes";
            this.nudAmbientes.Size = new System.Drawing.Size(80, 20);
            this.nudAmbientes.TabIndex = 7;
            this.nudAmbientes.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblLocalidad
            //
            this.lblLocalidad.Location = new System.Drawing.Point(6, 95);
            this.lblLocalidad.Name = "lblLocalidad";
            this.lblLocalidad.Size = new System.Drawing.Size(100, 20);
            this.lblLocalidad.Text = "Localidad:";
            this.lblLocalidad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtLocalidad
            //
            this.txtLocalidad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtLocalidad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLocalidad.ForeColor = System.Drawing.Color.White;
            this.txtLocalidad.Location = new System.Drawing.Point(112, 95);
            this.txtLocalidad.MaxLength = 100;
            this.txtLocalidad.Name = "txtLocalidad";
            this.txtLocalidad.Size = new System.Drawing.Size(180, 20);
            //
            // lblSuperficieLote
            //
            this.lblSuperficieLote.Location = new System.Drawing.Point(296, 95);
            this.lblSuperficieLote.Name = "lblSuperficieLote";
            this.lblSuperficieLote.Size = new System.Drawing.Size(100, 20);
            this.lblSuperficieLote.Text = "Sup. lote (m²):";
            this.lblSuperficieLote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudSuperficieLote
            //
            this.nudSuperficieLote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudSuperficieLote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudSuperficieLote.DecimalPlaces = 2;
            this.nudSuperficieLote.ForeColor = System.Drawing.Color.White;
            this.nudSuperficieLote.Location = new System.Drawing.Point(402, 95);
            this.nudSuperficieLote.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.nudSuperficieLote.Name = "nudSuperficieLote";
            this.nudSuperficieLote.Size = new System.Drawing.Size(120, 20);
            this.nudSuperficieLote.ThousandsSeparator = true;
            //
            // grpCaptacion
            //
            this.grpCaptacion.BackColor = System.Drawing.Color.Transparent;
            this.grpCaptacion.Controls.Add(this.lblValorPretendido);
            this.grpCaptacion.Controls.Add(this.nudValorPretendido);
            this.grpCaptacion.Controls.Add(this.chkAutorizacionRecibida);
            this.grpCaptacion.Controls.Add(this.lblTipoPublicacion);
            this.grpCaptacion.Controls.Add(this.cmbTipoPublicacion);
            this.grpCaptacion.ForeColor = System.Drawing.Color.White;
            this.grpCaptacion.Location = new System.Drawing.Point(12, 292);
            this.grpCaptacion.Name = "grpCaptacion";
            this.grpCaptacion.Size = new System.Drawing.Size(616, 95);
            this.grpCaptacion.TabIndex = 2;
            this.grpCaptacion.TabStop = false;
            this.grpCaptacion.Text = "Captación";
            //
            // lblValorPretendido
            //
            this.lblValorPretendido.Location = new System.Drawing.Point(6, 25);
            this.lblValorPretendido.Name = "lblValorPretendido";
            this.lblValorPretendido.Size = new System.Drawing.Size(100, 20);
            this.lblValorPretendido.TabIndex = 0;
            this.lblValorPretendido.Text = "Valor pretendido:";
            this.lblValorPretendido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudValorPretendido
            //
            this.nudValorPretendido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudValorPretendido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudValorPretendido.DecimalPlaces = 2;
            this.nudValorPretendido.ForeColor = System.Drawing.Color.White;
            this.nudValorPretendido.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudValorPretendido.Location = new System.Drawing.Point(112, 25);
            this.nudValorPretendido.Maximum = new decimal(new int[] { 1215752191, 23, 0, 0 });
            this.nudValorPretendido.Name = "nudValorPretendido";
            this.nudValorPretendido.Size = new System.Drawing.Size(180, 20);
            this.nudValorPretendido.TabIndex = 1;
            this.nudValorPretendido.ThousandsSeparator = true;
            //
            // chkAutorizacionRecibida
            //
            this.chkAutorizacionRecibida.AutoSize = true;
            this.chkAutorizacionRecibida.Location = new System.Drawing.Point(112, 60);
            this.chkAutorizacionRecibida.Name = "chkAutorizacionRecibida";
            this.chkAutorizacionRecibida.Size = new System.Drawing.Size(219, 17);
            this.chkAutorizacionRecibida.TabIndex = 2;
            this.chkAutorizacionRecibida.Text = "Autorización de Venta firmada recibida";
            this.chkAutorizacionRecibida.UseVisualStyleBackColor = true;
            //
            // lblTipoPublicacion
            //
            this.lblTipoPublicacion.Location = new System.Drawing.Point(296, 25);
            this.lblTipoPublicacion.Name = "lblTipoPublicacion";
            this.lblTipoPublicacion.Size = new System.Drawing.Size(100, 21);
            this.lblTipoPublicacion.TabIndex = 3;
            this.lblTipoPublicacion.Text = "Operación:";
            this.lblTipoPublicacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cmbTipoPublicacion
            //
            this.cmbTipoPublicacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbTipoPublicacion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoPublicacion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbTipoPublicacion.ForeColor = System.Drawing.Color.White;
            this.cmbTipoPublicacion.FormattingEnabled = true;
            this.cmbTipoPublicacion.Location = new System.Drawing.Point(402, 25);
            this.cmbTipoPublicacion.Name = "cmbTipoPublicacion";
            this.cmbTipoPublicacion.Size = new System.Drawing.Size(200, 21);
            this.cmbTipoPublicacion.TabIndex = 4;
            this.cmbTipoPublicacion.SelectedIndexChanged += new System.EventHandler(this.cmbTipoPublicacion_SelectedIndexChanged);
            //
            // btnGuardar
            //
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(412, 399);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(105, 32);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(523, 399);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(105, 32);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // FormRegistrarCaptacion
            //
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(640, 443);
            this.Controls.Add(this.grpCliente);
            this.Controls.Add(this.grpInmueble);
            this.Controls.Add(this.grpCaptacion);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormRegistrarCaptacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registrar Captación";
            this.Load += new System.EventHandler(this.FormRegistrarCaptacion_Load);
            this.grpCliente.ResumeLayout(false);
            this.grpCliente.PerformLayout();
            this.grpInmueble.ResumeLayout(false);
            this.grpInmueble.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMetros)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAmbientes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSuperficieLote)).EndInit();
            this.grpCaptacion.ResumeLayout(false);
            this.grpCaptacion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValorPretendido)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox grpCliente;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.Button btnBuscarCliente;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblApellido;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.GroupBox grpInmueble;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cmbTipo;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label lblMetros;
        private System.Windows.Forms.NumericUpDown nudMetros;
        private System.Windows.Forms.Label lblAmbientes;
        private System.Windows.Forms.NumericUpDown nudAmbientes;
        private System.Windows.Forms.Label lblLocalidad;
        private System.Windows.Forms.TextBox txtLocalidad;
        private System.Windows.Forms.Label lblSuperficieLote;
        private System.Windows.Forms.NumericUpDown nudSuperficieLote;
        private System.Windows.Forms.GroupBox grpCaptacion;
        private System.Windows.Forms.Label lblValorPretendido;
        private System.Windows.Forms.NumericUpDown nudValorPretendido;
        private System.Windows.Forms.CheckBox chkAutorizacionRecibida;
        private System.Windows.Forms.Label lblTipoPublicacion;
        private System.Windows.Forms.ComboBox cmbTipoPublicacion;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
