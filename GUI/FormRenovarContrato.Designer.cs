namespace TpIngSoft
{
    partial class FormRenovarContrato
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
            this.lblContrato = new System.Windows.Forms.Label();
            this.lblContratoValor = new System.Windows.Forms.Label();
            this.lblVencimientoActual = new System.Windows.Forms.Label();
            this.lblVencimientoActualValor = new System.Windows.Forms.Label();
            this.lblMontoActual = new System.Windows.Forms.Label();
            this.lblMontoActualValor = new System.Windows.Forms.Label();
            this.lblMeses = new System.Windows.Forms.Label();
            this.nudMeses = new System.Windows.Forms.NumericUpDown();
            this.lblNuevoMonto = new System.Windows.Forms.Label();
            this.nudNuevoMonto = new System.Windows.Forms.NumericUpDown();
            this.lblNuevoVencimiento = new System.Windows.Forms.Label();
            this.lblNuevoVencimientoValor = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudMeses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNuevoMonto)).BeginInit();
            this.SuspendLayout();
            //
            // lblContrato
            //
            this.lblContrato.BackColor = System.Drawing.Color.Transparent;
            this.lblContrato.ForeColor = System.Drawing.Color.White;
            this.lblContrato.Location = new System.Drawing.Point(12, 16);
            this.lblContrato.Name = "lblContrato";
            this.lblContrato.Size = new System.Drawing.Size(130, 20);
            this.lblContrato.TabIndex = 0;
            this.lblContrato.Text = "Contrato:";
            this.lblContrato.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblContratoValor
            //
            this.lblContratoValor.AutoEllipsis = true;
            this.lblContratoValor.BackColor = System.Drawing.Color.Transparent;
            this.lblContratoValor.ForeColor = System.Drawing.Color.White;
            this.lblContratoValor.Location = new System.Drawing.Point(148, 16);
            this.lblContratoValor.Name = "lblContratoValor";
            this.lblContratoValor.Size = new System.Drawing.Size(220, 20);
            this.lblContratoValor.TabIndex = 1;
            this.lblContratoValor.Text = "-";
            this.lblContratoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblVencimientoActual
            //
            this.lblVencimientoActual.BackColor = System.Drawing.Color.Transparent;
            this.lblVencimientoActual.ForeColor = System.Drawing.Color.White;
            this.lblVencimientoActual.Location = new System.Drawing.Point(12, 40);
            this.lblVencimientoActual.Name = "lblVencimientoActual";
            this.lblVencimientoActual.Size = new System.Drawing.Size(130, 20);
            this.lblVencimientoActual.TabIndex = 2;
            this.lblVencimientoActual.Text = "Vencimiento actual:";
            this.lblVencimientoActual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblVencimientoActualValor
            //
            this.lblVencimientoActualValor.BackColor = System.Drawing.Color.Transparent;
            this.lblVencimientoActualValor.ForeColor = System.Drawing.Color.White;
            this.lblVencimientoActualValor.Location = new System.Drawing.Point(148, 40);
            this.lblVencimientoActualValor.Name = "lblVencimientoActualValor";
            this.lblVencimientoActualValor.Size = new System.Drawing.Size(220, 20);
            this.lblVencimientoActualValor.TabIndex = 3;
            this.lblVencimientoActualValor.Text = "-";
            this.lblVencimientoActualValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMontoActual
            //
            this.lblMontoActual.BackColor = System.Drawing.Color.Transparent;
            this.lblMontoActual.ForeColor = System.Drawing.Color.White;
            this.lblMontoActual.Location = new System.Drawing.Point(12, 64);
            this.lblMontoActual.Name = "lblMontoActual";
            this.lblMontoActual.Size = new System.Drawing.Size(130, 20);
            this.lblMontoActual.TabIndex = 4;
            this.lblMontoActual.Text = "Monto actual:";
            this.lblMontoActual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMontoActualValor
            //
            this.lblMontoActualValor.BackColor = System.Drawing.Color.Transparent;
            this.lblMontoActualValor.ForeColor = System.Drawing.Color.White;
            this.lblMontoActualValor.Location = new System.Drawing.Point(148, 64);
            this.lblMontoActualValor.Name = "lblMontoActualValor";
            this.lblMontoActualValor.Size = new System.Drawing.Size(220, 20);
            this.lblMontoActualValor.TabIndex = 5;
            this.lblMontoActualValor.Text = "-";
            this.lblMontoActualValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMeses
            //
            this.lblMeses.BackColor = System.Drawing.Color.Transparent;
            this.lblMeses.ForeColor = System.Drawing.Color.White;
            this.lblMeses.Location = new System.Drawing.Point(12, 98);
            this.lblMeses.Name = "lblMeses";
            this.lblMeses.Size = new System.Drawing.Size(130, 20);
            this.lblMeses.TabIndex = 6;
            this.lblMeses.Text = "Nueva duración (meses):";
            this.lblMeses.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudMeses
            //
            this.nudMeses.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudMeses.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudMeses.ForeColor = System.Drawing.Color.White;
            this.nudMeses.Location = new System.Drawing.Point(148, 98);
            this.nudMeses.Maximum = new decimal(new int[] { 240, 0, 0, 0 });
            this.nudMeses.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudMeses.Name = "nudMeses";
            this.nudMeses.Size = new System.Drawing.Size(80, 20);
            this.nudMeses.TabIndex = 7;
            this.nudMeses.Value = new decimal(new int[] { 24, 0, 0, 0 });
            this.nudMeses.ValueChanged += new System.EventHandler(this.nudMeses_ValueChanged);
            //
            // lblNuevoMonto
            //
            this.lblNuevoMonto.BackColor = System.Drawing.Color.Transparent;
            this.lblNuevoMonto.ForeColor = System.Drawing.Color.White;
            this.lblNuevoMonto.Location = new System.Drawing.Point(12, 128);
            this.lblNuevoMonto.Name = "lblNuevoMonto";
            this.lblNuevoMonto.Size = new System.Drawing.Size(130, 20);
            this.lblNuevoMonto.TabIndex = 8;
            this.lblNuevoMonto.Text = "Nuevo monto mensual:";
            this.lblNuevoMonto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudNuevoMonto
            //
            this.nudNuevoMonto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudNuevoMonto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudNuevoMonto.DecimalPlaces = 2;
            this.nudNuevoMonto.ForeColor = System.Drawing.Color.White;
            this.nudNuevoMonto.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudNuevoMonto.Location = new System.Drawing.Point(148, 128);
            this.nudNuevoMonto.Maximum = new decimal(new int[] { 1215752191, 23, 0, 0 });
            this.nudNuevoMonto.Name = "nudNuevoMonto";
            this.nudNuevoMonto.Size = new System.Drawing.Size(160, 20);
            this.nudNuevoMonto.TabIndex = 9;
            this.nudNuevoMonto.ThousandsSeparator = true;
            //
            // lblNuevoVencimiento
            //
            this.lblNuevoVencimiento.BackColor = System.Drawing.Color.Transparent;
            this.lblNuevoVencimiento.ForeColor = System.Drawing.Color.White;
            this.lblNuevoVencimiento.Location = new System.Drawing.Point(12, 158);
            this.lblNuevoVencimiento.Name = "lblNuevoVencimiento";
            this.lblNuevoVencimiento.Size = new System.Drawing.Size(130, 20);
            this.lblNuevoVencimiento.TabIndex = 10;
            this.lblNuevoVencimiento.Text = "Nuevo vencimiento:";
            this.lblNuevoVencimiento.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblNuevoVencimientoValor
            //
            this.lblNuevoVencimientoValor.BackColor = System.Drawing.Color.Transparent;
            this.lblNuevoVencimientoValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNuevoVencimientoValor.ForeColor = System.Drawing.Color.White;
            this.lblNuevoVencimientoValor.Location = new System.Drawing.Point(148, 158);
            this.lblNuevoVencimientoValor.Name = "lblNuevoVencimientoValor";
            this.lblNuevoVencimientoValor.Size = new System.Drawing.Size(220, 20);
            this.lblNuevoVencimientoValor.TabIndex = 11;
            this.lblNuevoVencimientoValor.Text = "-";
            this.lblNuevoVencimientoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnConfirmar
            //
            this.btnConfirmar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConfirmar.ForeColor = System.Drawing.Color.White;
            this.btnConfirmar.Location = new System.Drawing.Point(118, 198);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(120, 34);
            this.btnConfirmar.TabIndex = 12;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.UseVisualStyleBackColor = false;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(248, 198);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(120, 34);
            this.btnCancelar.TabIndex = 13;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            //
            // FormRenovarContrato
            //
            this.AcceptButton = this.btnConfirmar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(384, 246);
            this.Controls.Add(this.lblContrato);
            this.Controls.Add(this.lblContratoValor);
            this.Controls.Add(this.lblVencimientoActual);
            this.Controls.Add(this.lblVencimientoActualValor);
            this.Controls.Add(this.lblMontoActual);
            this.Controls.Add(this.lblMontoActualValor);
            this.Controls.Add(this.lblMeses);
            this.Controls.Add(this.nudMeses);
            this.Controls.Add(this.lblNuevoMonto);
            this.Controls.Add(this.nudNuevoMonto);
            this.Controls.Add(this.lblNuevoVencimiento);
            this.Controls.Add(this.lblNuevoVencimientoValor);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRenovarContrato";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Registrar Renovación";
            ((System.ComponentModel.ISupportInitialize)(this.nudMeses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNuevoMonto)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblContrato;
        private System.Windows.Forms.Label lblContratoValor;
        private System.Windows.Forms.Label lblVencimientoActual;
        private System.Windows.Forms.Label lblVencimientoActualValor;
        private System.Windows.Forms.Label lblMontoActual;
        private System.Windows.Forms.Label lblMontoActualValor;
        private System.Windows.Forms.Label lblMeses;
        private System.Windows.Forms.NumericUpDown nudMeses;
        private System.Windows.Forms.Label lblNuevoMonto;
        private System.Windows.Forms.NumericUpDown nudNuevoMonto;
        private System.Windows.Forms.Label lblNuevoVencimiento;
        private System.Windows.Forms.Label lblNuevoVencimientoValor;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
