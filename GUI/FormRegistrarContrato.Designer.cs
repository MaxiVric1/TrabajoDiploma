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
            this.grpInmueble = new System.Windows.Forms.GroupBox();
            this.lblInmueble = new System.Windows.Forms.Label();
            this.cmbInmueble = new System.Windows.Forms.ComboBox();
            this.grpContrato = new System.Windows.Forms.GroupBox();
            this.lblFechaInicio = new System.Windows.Forms.Label();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.lblDuracion = new System.Windows.Forms.Label();
            this.nudDuracion = new System.Windows.Forms.NumericUpDown();
            this.lblMonto = new System.Windows.Forms.Label();
            this.nudMonto = new System.Windows.Forms.NumericUpDown();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.lblFechaFinValor = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.grpInquilino.SuspendLayout();
            this.grpInmueble.SuspendLayout();
            this.grpContrato.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDuracion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonto)).BeginInit();
            this.SuspendLayout();
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
            this.grpInquilino.Location = new System.Drawing.Point(12, 12);
            this.grpInquilino.Name = "grpInquilino";
            this.grpInquilino.Size = new System.Drawing.Size(616, 130);
            this.grpInquilino.TabIndex = 0;
            this.grpInquilino.TabStop = false;
            this.grpInquilino.Text = "Inquilino";
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
            // btnBuscarInquilino
            //
            this.btnBuscarInquilino.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnBuscarInquilino.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuscarInquilino.ForeColor = System.Drawing.Color.White;
            this.btnBuscarInquilino.Location = new System.Drawing.Point(300, 21);
            this.btnBuscarInquilino.Name = "btnBuscarInquilino";
            this.btnBuscarInquilino.Size = new System.Drawing.Size(96, 27);
            this.btnBuscarInquilino.TabIndex = 2;
            this.btnBuscarInquilino.Text = "Buscar";
            this.btnBuscarInquilino.UseVisualStyleBackColor = false;
            this.btnBuscarInquilino.Click += new System.EventHandler(this.btnBuscarInquilino_Click);
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
            this.grpInmueble.Controls.Add(this.lblInmueble);
            this.grpInmueble.Controls.Add(this.cmbInmueble);
            this.grpInmueble.ForeColor = System.Drawing.Color.White;
            this.grpInmueble.Location = new System.Drawing.Point(12, 152);
            this.grpInmueble.Name = "grpInmueble";
            this.grpInmueble.Size = new System.Drawing.Size(616, 60);
            this.grpInmueble.TabIndex = 1;
            this.grpInmueble.TabStop = false;
            this.grpInmueble.Text = "Inmueble";
            //
            // lblInmueble
            //
            this.lblInmueble.Location = new System.Drawing.Point(6, 25);
            this.lblInmueble.Name = "lblInmueble";
            this.lblInmueble.Size = new System.Drawing.Size(100, 21);
            this.lblInmueble.TabIndex = 0;
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
            this.cmbInmueble.Location = new System.Drawing.Point(112, 25);
            this.cmbInmueble.Name = "cmbInmueble";
            this.cmbInmueble.Size = new System.Drawing.Size(490, 21);
            this.cmbInmueble.TabIndex = 1;
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
            this.grpContrato.Controls.Add(this.lblFechaFin);
            this.grpContrato.Controls.Add(this.lblFechaFinValor);
            this.grpContrato.ForeColor = System.Drawing.Color.White;
            this.grpContrato.Location = new System.Drawing.Point(12, 222);
            this.grpContrato.Name = "grpContrato";
            this.grpContrato.Size = new System.Drawing.Size(616, 130);
            this.grpContrato.TabIndex = 2;
            this.grpContrato.TabStop = false;
            this.grpContrato.Text = "Contrato";
            //
            // lblFechaInicio
            //
            this.lblFechaInicio.Location = new System.Drawing.Point(6, 25);
            this.lblFechaInicio.Name = "lblFechaInicio";
            this.lblFechaInicio.Size = new System.Drawing.Size(100, 20);
            this.lblFechaInicio.TabIndex = 0;
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
            this.dtpFechaInicio.Location = new System.Drawing.Point(112, 25);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(180, 20);
            this.dtpFechaInicio.TabIndex = 1;
            this.dtpFechaInicio.ValueChanged += new System.EventHandler(this.DatosContrato_Changed);
            //
            // lblDuracion
            //
            this.lblDuracion.Location = new System.Drawing.Point(296, 25);
            this.lblDuracion.Name = "lblDuracion";
            this.lblDuracion.Size = new System.Drawing.Size(100, 20);
            this.lblDuracion.TabIndex = 2;
            this.lblDuracion.Text = "Duración (meses):";
            this.lblDuracion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudDuracion
            //
            this.nudDuracion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudDuracion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudDuracion.ForeColor = System.Drawing.Color.White;
            this.nudDuracion.Location = new System.Drawing.Point(402, 25);
            this.nudDuracion.Maximum = new decimal(new int[] { 240, 0, 0, 0 });
            this.nudDuracion.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudDuracion.Name = "nudDuracion";
            this.nudDuracion.Size = new System.Drawing.Size(80, 20);
            this.nudDuracion.TabIndex = 3;
            this.nudDuracion.Value = new decimal(new int[] { 24, 0, 0, 0 });
            this.nudDuracion.ValueChanged += new System.EventHandler(this.DatosContrato_Changed);
            //
            // lblMonto
            //
            this.lblMonto.Location = new System.Drawing.Point(6, 60);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(100, 20);
            this.lblMonto.TabIndex = 4;
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
            this.nudMonto.Location = new System.Drawing.Point(112, 60);
            this.nudMonto.Maximum = new decimal(new int[] { 1215752191, 23, 0, 0 });
            this.nudMonto.Name = "nudMonto";
            this.nudMonto.Size = new System.Drawing.Size(180, 20);
            this.nudMonto.TabIndex = 5;
            this.nudMonto.ThousandsSeparator = true;
            //
            // lblFechaFin
            //
            this.lblFechaFin.Location = new System.Drawing.Point(6, 95);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(100, 20);
            this.lblFechaFin.TabIndex = 6;
            this.lblFechaFin.Text = "Finaliza:";
            this.lblFechaFin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblFechaFinValor
            //
            this.lblFechaFinValor.Location = new System.Drawing.Point(112, 95);
            this.lblFechaFinValor.Name = "lblFechaFinValor";
            this.lblFechaFinValor.Size = new System.Drawing.Size(490, 20);
            this.lblFechaFinValor.TabIndex = 7;
            this.lblFechaFinValor.Text = "-";
            this.lblFechaFinValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(412, 364);
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
            this.btnCancelar.Location = new System.Drawing.Point(523, 364);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(105, 32);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // FormRegistrarContrato
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(640, 408);
            this.Controls.Add(this.grpInquilino);
            this.Controls.Add(this.grpInmueble);
            this.Controls.Add(this.grpContrato);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormRegistrarContrato";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registrar Contrato";
            this.Load += new System.EventHandler(this.FormRegistrarContrato_Load);
            this.grpInquilino.ResumeLayout(false);
            this.grpInquilino.PerformLayout();
            this.grpInmueble.ResumeLayout(false);
            this.grpInmueble.PerformLayout();
            this.grpContrato.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudDuracion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonto)).EndInit();
            this.ResumeLayout(false);

        }

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
        private System.Windows.Forms.GroupBox grpInmueble;
        private System.Windows.Forms.Label lblInmueble;
        private System.Windows.Forms.ComboBox cmbInmueble;
        private System.Windows.Forms.GroupBox grpContrato;
        private System.Windows.Forms.Label lblFechaInicio;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private System.Windows.Forms.Label lblDuracion;
        private System.Windows.Forms.NumericUpDown nudDuracion;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.NumericUpDown nudMonto;
        private System.Windows.Forms.Label lblFechaFin;
        private System.Windows.Forms.Label lblFechaFinValor;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
