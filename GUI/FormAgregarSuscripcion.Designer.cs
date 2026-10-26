namespace TpIngSoft
{
    partial class FormAgregarSuscripcion
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
            this.grpInversor = new System.Windows.Forms.GroupBox();
            this.lblCuit = new System.Windows.Forms.Label();
            this.txtCuit = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblApellido = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblEstadoInversor = new System.Windows.Forms.Label();
            this.grpAporte = new System.Windows.Forms.GroupBox();
            this.lblMonto = new System.Windows.Forms.Label();
            this.nudMonto = new System.Windows.Forms.NumericUpDown();
            this.lblUnidades = new System.Windows.Forms.Label();
            this.nudUnidades = new System.Windows.Forms.NumericUpDown();
            this.lblParticipacionValor = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.grpInversor.SuspendLayout();
            this.grpAporte.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUnidades)).BeginInit();
            this.SuspendLayout();
            //
            // grpInversor
            //
            this.grpInversor.BackColor = System.Drawing.Color.Transparent;
            this.grpInversor.Controls.Add(this.lblCuit);
            this.grpInversor.Controls.Add(this.txtCuit);
            this.grpInversor.Controls.Add(this.btnBuscar);
            this.grpInversor.Controls.Add(this.lblNombre);
            this.grpInversor.Controls.Add(this.txtNombre);
            this.grpInversor.Controls.Add(this.lblApellido);
            this.grpInversor.Controls.Add(this.txtApellido);
            this.grpInversor.Controls.Add(this.lblEmail);
            this.grpInversor.Controls.Add(this.txtEmail);
            this.grpInversor.Controls.Add(this.lblTelefono);
            this.grpInversor.Controls.Add(this.txtTelefono);
            this.grpInversor.Controls.Add(this.lblEstadoInversor);
            this.grpInversor.ForeColor = System.Drawing.Color.White;
            this.grpInversor.Location = new System.Drawing.Point(12, 12);
            this.grpInversor.Name = "grpInversor";
            this.grpInversor.Size = new System.Drawing.Size(496, 186);
            this.grpInversor.TabStop = false;
            this.grpInversor.Text = "Inversor";
            //
            // lblCuit
            //
            this.lblCuit.Location = new System.Drawing.Point(6, 25);
            this.lblCuit.Name = "lblCuit";
            this.lblCuit.Size = new System.Drawing.Size(130, 20);
            this.lblCuit.Text = "CUIT:";
            this.lblCuit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtCuit
            //
            this.txtCuit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtCuit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCuit.ForeColor = System.Drawing.Color.White;
            this.txtCuit.Location = new System.Drawing.Point(142, 25);
            this.txtCuit.MaxLength = 13;
            this.txtCuit.Name = "txtCuit";
            this.txtCuit.Size = new System.Drawing.Size(160, 20);
            //
            // btnBuscar
            //
            this.btnBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuscar.ForeColor = System.Drawing.Color.White;
            this.btnBuscar.Location = new System.Drawing.Point(310, 21);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(90, 27);
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = false;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // lblNombre
            //
            this.lblNombre.Location = new System.Drawing.Point(6, 58);
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
            this.txtNombre.Location = new System.Drawing.Point(142, 58);
            this.txtNombre.MaxLength = 100;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(340, 20);
            //
            // lblApellido
            //
            this.lblApellido.Location = new System.Drawing.Point(6, 84);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(130, 20);
            this.lblApellido.Text = "Apellido:";
            this.lblApellido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtApellido
            //
            this.txtApellido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtApellido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtApellido.ForeColor = System.Drawing.Color.White;
            this.txtApellido.Location = new System.Drawing.Point(142, 84);
            this.txtApellido.MaxLength = 100;
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(340, 20);
            //
            // lblEmail
            //
            this.lblEmail.Location = new System.Drawing.Point(6, 110);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(130, 20);
            this.lblEmail.Text = "Email:";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtEmail
            //
            this.txtEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.ForeColor = System.Drawing.Color.White;
            this.txtEmail.Location = new System.Drawing.Point(142, 110);
            this.txtEmail.MaxLength = 100;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(340, 20);
            //
            // lblTelefono
            //
            this.lblTelefono.Location = new System.Drawing.Point(6, 136);
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
            this.txtTelefono.Location = new System.Drawing.Point(142, 136);
            this.txtTelefono.MaxLength = 30;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(340, 20);
            //
            // lblEstadoInversor
            //
            this.lblEstadoInversor.AutoEllipsis = true;
            this.lblEstadoInversor.Location = new System.Drawing.Point(142, 160);
            this.lblEstadoInversor.Name = "lblEstadoInversor";
            this.lblEstadoInversor.Size = new System.Drawing.Size(340, 20);
            this.lblEstadoInversor.Text = "";
            this.lblEstadoInversor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpAporte
            //
            this.grpAporte.BackColor = System.Drawing.Color.Transparent;
            this.grpAporte.Controls.Add(this.lblMonto);
            this.grpAporte.Controls.Add(this.nudMonto);
            this.grpAporte.Controls.Add(this.lblUnidades);
            this.grpAporte.Controls.Add(this.nudUnidades);
            this.grpAporte.Controls.Add(this.lblParticipacionValor);
            this.grpAporte.ForeColor = System.Drawing.Color.White;
            this.grpAporte.Location = new System.Drawing.Point(12, 206);
            this.grpAporte.Name = "grpAporte";
            this.grpAporte.Size = new System.Drawing.Size(496, 118);
            this.grpAporte.TabStop = false;
            this.grpAporte.Text = "Aporte";
            //
            // lblMonto
            //
            this.lblMonto.Location = new System.Drawing.Point(6, 25);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(130, 20);
            this.lblMonto.Text = "Monto (USD):";
            this.lblMonto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudMonto
            //
            this.nudMonto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudMonto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudMonto.DecimalPlaces = 2;
            this.nudMonto.ForeColor = System.Drawing.Color.White;
            this.nudMonto.Location = new System.Drawing.Point(142, 25);
            this.nudMonto.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            this.nudMonto.Name = "nudMonto";
            this.nudMonto.Size = new System.Drawing.Size(160, 20);
            this.nudMonto.ThousandsSeparator = true;
            this.nudMonto.ValueChanged += new System.EventHandler(this.nudMonto_ValueChanged);
            //
            // lblUnidades
            //
            this.lblUnidades.Location = new System.Drawing.Point(6, 55);
            this.lblUnidades.Name = "lblUnidades";
            this.lblUnidades.Size = new System.Drawing.Size(130, 20);
            this.lblUnidades.Text = "Unidades asignadas:";
            this.lblUnidades.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudUnidades
            //
            this.nudUnidades.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudUnidades.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudUnidades.DecimalPlaces = 0;
            this.nudUnidades.ForeColor = System.Drawing.Color.White;
            this.nudUnidades.Location = new System.Drawing.Point(142, 55);
            this.nudUnidades.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.nudUnidades.Name = "nudUnidades";
            this.nudUnidades.Size = new System.Drawing.Size(80, 20);
            this.nudUnidades.ThousandsSeparator = true;
            //
            // lblParticipacionValor
            //
            this.lblParticipacionValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblParticipacionValor.AutoEllipsis = true;
            this.lblParticipacionValor.Location = new System.Drawing.Point(10, 84);
            this.lblParticipacionValor.Name = "lblParticipacionValor";
            this.lblParticipacionValor.Size = new System.Drawing.Size(476, 26);
            this.lblParticipacionValor.Text = "-";
            this.lblParticipacionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(288, 334);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(105, 36);
            this.btnGuardar.Text = "Guardar Aporte";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(403, 334);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(105, 36);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // FormAgregarSuscripcion
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(520, 382);
            this.Controls.Add(this.grpInversor);
            this.Controls.Add(this.grpAporte);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(536, 421);
            this.Name = "FormAgregarSuscripcion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Agregar Inversor";
            this.Load += new System.EventHandler(this.FormAgregarSuscripcion_Load);
            this.grpInversor.ResumeLayout(false);
            this.grpInversor.PerformLayout();
            this.grpAporte.ResumeLayout(false);
            this.grpAporte.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUnidades)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox grpInversor;
        private System.Windows.Forms.Label lblCuit;
        private System.Windows.Forms.TextBox txtCuit;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblApellido;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblEstadoInversor;
        private System.Windows.Forms.GroupBox grpAporte;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.NumericUpDown nudMonto;
        private System.Windows.Forms.Label lblUnidades;
        private System.Windows.Forms.NumericUpDown nudUnidades;
        private System.Windows.Forms.Label lblParticipacionValor;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
