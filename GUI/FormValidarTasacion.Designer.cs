namespace TpIngSoft
{
    partial class FormValidarTasacion
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblPendientes = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.dgvPendientes = new System.Windows.Forms.DataGridView();
            this.grpDetalle = new System.Windows.Forms.GroupBox();
            this.lblCliente = new System.Windows.Forms.Label();
            this.lblClienteValor = new System.Windows.Forms.Label();
            this.lblDni = new System.Windows.Forms.Label();
            this.lblDniValor = new System.Windows.Forms.Label();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.lblTelefonoValor = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblEmailValor = new System.Windows.Forms.Label();
            this.lblValorPretendido = new System.Windows.Forms.Label();
            this.lblValorPretendidoValor = new System.Windows.Forms.Label();
            this.lblDictamenAnterior = new System.Windows.Forms.Label();
            this.lblDictamenAnteriorValor = new System.Windows.Forms.Label();
            this.grpDictamen = new System.Windows.Forms.GroupBox();
            this.rbAprobar = new System.Windows.Forms.RadioButton();
            this.rbRechazar = new System.Windows.Forms.RadioButton();
            this.lblValorFinal = new System.Windows.Forms.Label();
            this.nudValorFinal = new System.Windows.Forms.NumericUpDown();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.lblValorSugerido = new System.Windows.Forms.Label();
            this.nudValorSugerido = new System.Windows.Forms.NumericUpDown();
            this.btnRegistrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendientes)).BeginInit();
            this.grpDetalle.SuspendLayout();
            this.grpDictamen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValorFinal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValorSugerido)).BeginInit();
            this.SuspendLayout();
            //
            // lblPendientes
            //
            this.lblPendientes.BackColor = System.Drawing.Color.Transparent;
            this.lblPendientes.ForeColor = System.Drawing.Color.White;
            this.lblPendientes.Location = new System.Drawing.Point(12, 12);
            this.lblPendientes.Name = "lblPendientes";
            this.lblPendientes.Size = new System.Drawing.Size(400, 24);
            this.lblPendientes.TabIndex = 0;
            this.lblPendientes.Text = "Captaciones pendientes de tasación";
            this.lblPendientes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnActualizar
            //
            this.btnActualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(788, 8);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 28);
            this.btnActualizar.TabIndex = 1;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // dgvPendientes
            //
            this.dgvPendientes.AllowUserToAddRows = false;
            this.dgvPendientes.AllowUserToDeleteRows = false;
            this.dgvPendientes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPendientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPendientes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPendientes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPendientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(129)))), ((int)(((byte)(158)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPendientes.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPendientes.EnableHeadersVisualStyles = false;
            this.dgvPendientes.GridColor = System.Drawing.Color.Black;
            this.dgvPendientes.Location = new System.Drawing.Point(12, 42);
            this.dgvPendientes.MultiSelect = false;
            this.dgvPendientes.Name = "dgvPendientes";
            this.dgvPendientes.ReadOnly = true;
            this.dgvPendientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPendientes.Size = new System.Drawing.Size(876, 230);
            this.dgvPendientes.TabIndex = 2;
            this.dgvPendientes.SelectionChanged += new System.EventHandler(this.dgvPendientes_SelectionChanged);
            //
            // grpDetalle
            //
            this.grpDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpDetalle.BackColor = System.Drawing.Color.Transparent;
            this.grpDetalle.Controls.Add(this.lblCliente);
            this.grpDetalle.Controls.Add(this.lblClienteValor);
            this.grpDetalle.Controls.Add(this.lblDni);
            this.grpDetalle.Controls.Add(this.lblDniValor);
            this.grpDetalle.Controls.Add(this.lblTelefono);
            this.grpDetalle.Controls.Add(this.lblTelefonoValor);
            this.grpDetalle.Controls.Add(this.lblEmail);
            this.grpDetalle.Controls.Add(this.lblEmailValor);
            this.grpDetalle.Controls.Add(this.lblValorPretendido);
            this.grpDetalle.Controls.Add(this.lblValorPretendidoValor);
            this.grpDetalle.Controls.Add(this.lblDictamenAnterior);
            this.grpDetalle.Controls.Add(this.lblDictamenAnteriorValor);
            this.grpDetalle.ForeColor = System.Drawing.Color.White;
            this.grpDetalle.Location = new System.Drawing.Point(12, 282);
            this.grpDetalle.Name = "grpDetalle";
            this.grpDetalle.Size = new System.Drawing.Size(430, 206);
            this.grpDetalle.TabIndex = 3;
            this.grpDetalle.TabStop = false;
            this.grpDetalle.Text = "Detalle de la captación";
            //
            // lblCliente
            //
            this.lblCliente.Location = new System.Drawing.Point(6, 22);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(130, 20);
            this.lblCliente.TabIndex = 0;
            this.lblCliente.Text = "Cliente:";
            this.lblCliente.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblClienteValor
            //
            this.lblClienteValor.AutoEllipsis = true;
            this.lblClienteValor.Location = new System.Drawing.Point(142, 22);
            this.lblClienteValor.Name = "lblClienteValor";
            this.lblClienteValor.Size = new System.Drawing.Size(280, 20);
            this.lblClienteValor.TabIndex = 1;
            this.lblClienteValor.Text = "-";
            this.lblClienteValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDni
            //
            this.lblDni.Location = new System.Drawing.Point(6, 47);
            this.lblDni.Name = "lblDni";
            this.lblDni.Size = new System.Drawing.Size(130, 20);
            this.lblDni.TabIndex = 2;
            this.lblDni.Text = "DNI:";
            this.lblDni.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDniValor
            //
            this.lblDniValor.Location = new System.Drawing.Point(142, 47);
            this.lblDniValor.Name = "lblDniValor";
            this.lblDniValor.Size = new System.Drawing.Size(280, 20);
            this.lblDniValor.TabIndex = 3;
            this.lblDniValor.Text = "-";
            this.lblDniValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTelefono
            //
            this.lblTelefono.Location = new System.Drawing.Point(6, 72);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(130, 20);
            this.lblTelefono.TabIndex = 4;
            this.lblTelefono.Text = "Teléfono:";
            this.lblTelefono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTelefonoValor
            //
            this.lblTelefonoValor.Location = new System.Drawing.Point(142, 72);
            this.lblTelefonoValor.Name = "lblTelefonoValor";
            this.lblTelefonoValor.Size = new System.Drawing.Size(280, 20);
            this.lblTelefonoValor.TabIndex = 5;
            this.lblTelefonoValor.Text = "-";
            this.lblTelefonoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblEmail
            //
            this.lblEmail.Location = new System.Drawing.Point(6, 97);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(130, 20);
            this.lblEmail.TabIndex = 6;
            this.lblEmail.Text = "Email:";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblEmailValor
            //
            this.lblEmailValor.AutoEllipsis = true;
            this.lblEmailValor.Location = new System.Drawing.Point(142, 97);
            this.lblEmailValor.Name = "lblEmailValor";
            this.lblEmailValor.Size = new System.Drawing.Size(280, 20);
            this.lblEmailValor.TabIndex = 7;
            this.lblEmailValor.Text = "-";
            this.lblEmailValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblValorPretendido
            //
            this.lblValorPretendido.Location = new System.Drawing.Point(6, 122);
            this.lblValorPretendido.Name = "lblValorPretendido";
            this.lblValorPretendido.Size = new System.Drawing.Size(130, 20);
            this.lblValorPretendido.TabIndex = 8;
            this.lblValorPretendido.Text = "Valor pretendido:";
            this.lblValorPretendido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblValorPretendidoValor
            //
            this.lblValorPretendidoValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValorPretendidoValor.Location = new System.Drawing.Point(142, 122);
            this.lblValorPretendidoValor.Name = "lblValorPretendidoValor";
            this.lblValorPretendidoValor.Size = new System.Drawing.Size(280, 20);
            this.lblValorPretendidoValor.TabIndex = 9;
            this.lblValorPretendidoValor.Text = "-";
            this.lblValorPretendidoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDictamenAnterior
            //
            this.lblDictamenAnterior.Location = new System.Drawing.Point(6, 147);
            this.lblDictamenAnterior.Name = "lblDictamenAnterior";
            this.lblDictamenAnterior.Size = new System.Drawing.Size(130, 20);
            this.lblDictamenAnterior.TabIndex = 10;
            this.lblDictamenAnterior.Text = "Dictamen anterior:";
            this.lblDictamenAnterior.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDictamenAnteriorValor
            //
            this.lblDictamenAnteriorValor.AutoEllipsis = true;
            this.lblDictamenAnteriorValor.Location = new System.Drawing.Point(142, 147);
            this.lblDictamenAnteriorValor.Name = "lblDictamenAnteriorValor";
            this.lblDictamenAnteriorValor.Size = new System.Drawing.Size(280, 50);
            this.lblDictamenAnteriorValor.TabIndex = 11;
            this.lblDictamenAnteriorValor.Text = "-";
            //
            // grpDictamen
            //
            this.grpDictamen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDictamen.BackColor = System.Drawing.Color.Transparent;
            this.grpDictamen.Controls.Add(this.rbAprobar);
            this.grpDictamen.Controls.Add(this.rbRechazar);
            this.grpDictamen.Controls.Add(this.lblValorFinal);
            this.grpDictamen.Controls.Add(this.nudValorFinal);
            this.grpDictamen.Controls.Add(this.lblMotivo);
            this.grpDictamen.Controls.Add(this.txtMotivo);
            this.grpDictamen.Controls.Add(this.lblValorSugerido);
            this.grpDictamen.Controls.Add(this.nudValorSugerido);
            this.grpDictamen.Controls.Add(this.btnRegistrar);
            this.grpDictamen.ForeColor = System.Drawing.Color.White;
            this.grpDictamen.Location = new System.Drawing.Point(452, 282);
            this.grpDictamen.Name = "grpDictamen";
            this.grpDictamen.Size = new System.Drawing.Size(436, 206);
            this.grpDictamen.TabIndex = 4;
            this.grpDictamen.TabStop = false;
            this.grpDictamen.Text = "Dictamen";
            //
            // rbAprobar
            //
            this.rbAprobar.AutoSize = true;
            this.rbAprobar.Location = new System.Drawing.Point(152, 22);
            this.rbAprobar.Name = "rbAprobar";
            this.rbAprobar.Size = new System.Drawing.Size(62, 17);
            this.rbAprobar.TabIndex = 0;
            this.rbAprobar.Text = "Aprobar";
            this.rbAprobar.UseVisualStyleBackColor = true;
            this.rbAprobar.CheckedChanged += new System.EventHandler(this.rbDictamen_CheckedChanged);
            //
            // rbRechazar
            //
            this.rbRechazar.AutoSize = true;
            this.rbRechazar.Location = new System.Drawing.Point(252, 22);
            this.rbRechazar.Name = "rbRechazar";
            this.rbRechazar.Size = new System.Drawing.Size(70, 17);
            this.rbRechazar.TabIndex = 1;
            this.rbRechazar.Text = "Rechazar";
            this.rbRechazar.UseVisualStyleBackColor = true;
            this.rbRechazar.CheckedChanged += new System.EventHandler(this.rbDictamen_CheckedChanged);
            //
            // lblValorFinal
            //
            this.lblValorFinal.Location = new System.Drawing.Point(6, 50);
            this.lblValorFinal.Name = "lblValorFinal";
            this.lblValorFinal.Size = new System.Drawing.Size(140, 20);
            this.lblValorFinal.TabIndex = 2;
            this.lblValorFinal.Text = "Valor final:";
            this.lblValorFinal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudValorFinal
            //
            this.nudValorFinal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudValorFinal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudValorFinal.DecimalPlaces = 2;
            this.nudValorFinal.ForeColor = System.Drawing.Color.White;
            this.nudValorFinal.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudValorFinal.Location = new System.Drawing.Point(152, 50);
            this.nudValorFinal.Maximum = new decimal(new int[] { 1215752191, 23, 0, 0 });
            this.nudValorFinal.Name = "nudValorFinal";
            this.nudValorFinal.Size = new System.Drawing.Size(180, 20);
            this.nudValorFinal.TabIndex = 3;
            this.nudValorFinal.ThousandsSeparator = true;
            //
            // lblMotivo
            //
            this.lblMotivo.Location = new System.Drawing.Point(6, 78);
            this.lblMotivo.Name = "lblMotivo";
            this.lblMotivo.Size = new System.Drawing.Size(140, 20);
            this.lblMotivo.TabIndex = 4;
            this.lblMotivo.Text = "Motivo de rechazo:";
            this.lblMotivo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtMotivo
            //
            this.txtMotivo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtMotivo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMotivo.ForeColor = System.Drawing.Color.White;
            this.txtMotivo.Location = new System.Drawing.Point(152, 78);
            this.txtMotivo.MaxLength = 500;
            this.txtMotivo.Multiline = true;
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMotivo.Size = new System.Drawing.Size(270, 46);
            this.txtMotivo.TabIndex = 5;
            //
            // lblValorSugerido
            //
            this.lblValorSugerido.Location = new System.Drawing.Point(6, 132);
            this.lblValorSugerido.Name = "lblValorSugerido";
            this.lblValorSugerido.Size = new System.Drawing.Size(140, 20);
            this.lblValorSugerido.TabIndex = 6;
            this.lblValorSugerido.Text = "Valor sugerido (opcional):";
            this.lblValorSugerido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudValorSugerido
            //
            this.nudValorSugerido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudValorSugerido.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudValorSugerido.DecimalPlaces = 2;
            this.nudValorSugerido.ForeColor = System.Drawing.Color.White;
            this.nudValorSugerido.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudValorSugerido.Location = new System.Drawing.Point(152, 132);
            this.nudValorSugerido.Maximum = new decimal(new int[] { 1215752191, 23, 0, 0 });
            this.nudValorSugerido.Name = "nudValorSugerido";
            this.nudValorSugerido.Size = new System.Drawing.Size(180, 20);
            this.nudValorSugerido.TabIndex = 7;
            this.nudValorSugerido.ThousandsSeparator = true;
            //
            // btnRegistrar
            //
            this.btnRegistrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRegistrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRegistrar.ForeColor = System.Drawing.Color.White;
            this.btnRegistrar.Location = new System.Drawing.Point(152, 164);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(180, 32);
            this.btnRegistrar.TabIndex = 8;
            this.btnRegistrar.Text = "Registrar dictamen";
            this.btnRegistrar.UseVisualStyleBackColor = false;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // FormValidarTasacion
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.lblPendientes);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.dgvPendientes);
            this.Controls.Add(this.grpDetalle);
            this.Controls.Add(this.grpDictamen);
            this.MinimumSize = new System.Drawing.Size(916, 539);
            this.Name = "FormValidarTasacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Validar Tasación";
            this.Load += new System.EventHandler(this.FormValidarTasacion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendientes)).EndInit();
            this.grpDetalle.ResumeLayout(false);
            this.grpDictamen.ResumeLayout(false);
            this.grpDictamen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValorFinal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValorSugerido)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblPendientes;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.DataGridView dgvPendientes;
        private System.Windows.Forms.GroupBox grpDetalle;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.Label lblClienteValor;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.Label lblDniValor;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.Label lblTelefonoValor;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblEmailValor;
        private System.Windows.Forms.Label lblValorPretendido;
        private System.Windows.Forms.Label lblValorPretendidoValor;
        private System.Windows.Forms.Label lblDictamenAnterior;
        private System.Windows.Forms.Label lblDictamenAnteriorValor;
        private System.Windows.Forms.GroupBox grpDictamen;
        private System.Windows.Forms.RadioButton rbAprobar;
        private System.Windows.Forms.RadioButton rbRechazar;
        private System.Windows.Forms.Label lblValorFinal;
        private System.Windows.Forms.NumericUpDown nudValorFinal;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Label lblValorSugerido;
        private System.Windows.Forms.NumericUpDown nudValorSugerido;
        private System.Windows.Forms.Button btnRegistrar;
    }
}
