namespace TpIngSoft
{
    partial class FormRegistrarPago
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
            this.lblActualizacion = new System.Windows.Forms.Label();
            this.grpContrato = new System.Windows.Forms.GroupBox();
            this.lblNroContrato = new System.Windows.Forms.Label();
            this.nudNroContrato = new System.Windows.Forms.NumericUpDown();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblInquilino = new System.Windows.Forms.Label();
            this.lblInquilinoValor = new System.Windows.Forms.Label();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.lblDireccionValor = new System.Windows.Forms.Label();
            this.lblMontoMensual = new System.Windows.Forms.Label();
            this.lblMontoMensualValor = new System.Windows.Forms.Label();
            this.lblEstadoContrato = new System.Windows.Forms.Label();
            this.lblEstadoContratoValor = new System.Windows.Forms.Label();
            this.grpPeriodo = new System.Windows.Forms.GroupBox();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.cmbPeriodo = new System.Windows.Forms.ComboBox();
            this.lblEstadoMes = new System.Windows.Forms.Label();
            this.lblEstadoMesValor = new System.Windows.Forms.Label();
            this.lblFechaPago = new System.Windows.Forms.Label();
            this.dtpFechaPago = new System.Windows.Forms.DateTimePicker();
            this.lblMontoBase = new System.Windows.Forms.Label();
            this.lblMontoBaseValor = new System.Windows.Forms.Label();
            this.lblRecargo = new System.Windows.Forms.Label();
            this.lblRecargoValor = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTotalValor = new System.Windows.Forms.Label();
            this.grpResumen = new System.Windows.Forms.GroupBox();
            this.lblResumenValor = new System.Windows.Forms.Label();
            this.btnRegistrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudNroContrato)).BeginInit();
            this.lblMora = new System.Windows.Forms.Label();
            this.lblMoraValor = new System.Windows.Forms.Label();
            this.grpContrato.SuspendLayout();
            this.grpPeriodo.SuspendLayout();
            this.grpResumen.SuspendLayout();
            this.SuspendLayout();
            //
            // lblActualizacion
            //
            this.lblActualizacion.AutoEllipsis = true;
            this.lblActualizacion.BackColor = System.Drawing.Color.Transparent;
            this.lblActualizacion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(195)))), ((int)(((byte)(220)))));
            this.lblActualizacion.Location = new System.Drawing.Point(12, 8);
            this.lblActualizacion.Name = "lblActualizacion";
            this.lblActualizacion.Size = new System.Drawing.Size(616, 20);
            this.lblActualizacion.TabIndex = 0;
            this.lblActualizacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpContrato
            //
            this.grpContrato.BackColor = System.Drawing.Color.Transparent;
            this.grpContrato.Controls.Add(this.lblNroContrato);
            this.grpContrato.Controls.Add(this.nudNroContrato);
            this.grpContrato.Controls.Add(this.btnBuscar);
            this.grpContrato.Controls.Add(this.lblInquilino);
            this.grpContrato.Controls.Add(this.lblInquilinoValor);
            this.grpContrato.Controls.Add(this.lblDireccion);
            this.grpContrato.Controls.Add(this.lblDireccionValor);
            this.grpContrato.Controls.Add(this.lblMontoMensual);
            this.grpContrato.Controls.Add(this.lblMontoMensualValor);
            this.grpContrato.Controls.Add(this.lblEstadoContrato);
            this.grpContrato.Controls.Add(this.lblEstadoContratoValor);
            this.grpContrato.Controls.Add(this.lblMora);
            this.grpContrato.Controls.Add(this.lblMoraValor);
            this.grpContrato.ForeColor = System.Drawing.Color.White;
            this.grpContrato.Location = new System.Drawing.Point(12, 32);
            this.grpContrato.Name = "grpContrato";
            this.grpContrato.Size = new System.Drawing.Size(616, 154);
            this.grpContrato.TabIndex = 1;
            this.grpContrato.TabStop = false;
            this.grpContrato.Text = "Contrato";
            //
            // lblNroContrato
            //
            this.lblNroContrato.Location = new System.Drawing.Point(6, 25);
            this.lblNroContrato.Name = "lblNroContrato";
            this.lblNroContrato.Size = new System.Drawing.Size(100, 20);
            this.lblNroContrato.TabIndex = 0;
            this.lblNroContrato.Text = "Nro Contrato:";
            this.lblNroContrato.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudNroContrato
            //
            this.nudNroContrato.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudNroContrato.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudNroContrato.ForeColor = System.Drawing.Color.White;
            this.nudNroContrato.Location = new System.Drawing.Point(112, 25);
            this.nudNroContrato.Maximum = new decimal(new int[] { 2147483647, 0, 0, 0 });
            this.nudNroContrato.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudNroContrato.Name = "nudNroContrato";
            this.nudNroContrato.Size = new System.Drawing.Size(120, 20);
            this.nudNroContrato.TabIndex = 1;
            this.nudNroContrato.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // btnBuscar
            //
            this.btnBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuscar.ForeColor = System.Drawing.Color.White;
            this.btnBuscar.Location = new System.Drawing.Point(240, 21);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(96, 27);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = false;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // lblInquilino
            //
            this.lblInquilino.Location = new System.Drawing.Point(6, 58);
            this.lblInquilino.Name = "lblInquilino";
            this.lblInquilino.Size = new System.Drawing.Size(100, 20);
            this.lblInquilino.TabIndex = 3;
            this.lblInquilino.Text = "Inquilino:";
            this.lblInquilino.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblInquilinoValor
            //
            this.lblInquilinoValor.AutoEllipsis = true;
            this.lblInquilinoValor.Location = new System.Drawing.Point(112, 58);
            this.lblInquilinoValor.Name = "lblInquilinoValor";
            this.lblInquilinoValor.Size = new System.Drawing.Size(490, 20);
            this.lblInquilinoValor.TabIndex = 4;
            this.lblInquilinoValor.Text = "-";
            this.lblInquilinoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDireccion
            //
            this.lblDireccion.Location = new System.Drawing.Point(6, 80);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(100, 20);
            this.lblDireccion.TabIndex = 5;
            this.lblDireccion.Text = "Dirección:";
            this.lblDireccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDireccionValor
            //
            this.lblDireccionValor.AutoEllipsis = true;
            this.lblDireccionValor.Location = new System.Drawing.Point(112, 80);
            this.lblDireccionValor.Name = "lblDireccionValor";
            this.lblDireccionValor.Size = new System.Drawing.Size(490, 20);
            this.lblDireccionValor.TabIndex = 6;
            this.lblDireccionValor.Text = "-";
            this.lblDireccionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMontoMensual
            //
            this.lblMontoMensual.Location = new System.Drawing.Point(6, 102);
            this.lblMontoMensual.Name = "lblMontoMensual";
            this.lblMontoMensual.Size = new System.Drawing.Size(100, 20);
            this.lblMontoMensual.TabIndex = 7;
            this.lblMontoMensual.Text = "Monto mensual:";
            this.lblMontoMensual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMontoMensualValor
            //
            this.lblMontoMensualValor.Location = new System.Drawing.Point(112, 102);
            this.lblMontoMensualValor.Name = "lblMontoMensualValor";
            this.lblMontoMensualValor.Size = new System.Drawing.Size(180, 20);
            this.lblMontoMensualValor.TabIndex = 8;
            this.lblMontoMensualValor.Text = "-";
            this.lblMontoMensualValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblEstadoContrato
            //
            this.lblEstadoContrato.Location = new System.Drawing.Point(296, 102);
            this.lblEstadoContrato.Name = "lblEstadoContrato";
            this.lblEstadoContrato.Size = new System.Drawing.Size(100, 20);
            this.lblEstadoContrato.TabIndex = 9;
            this.lblEstadoContrato.Text = "Estado:";
            this.lblEstadoContrato.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblEstadoContratoValor
            //
            this.lblEstadoContratoValor.Location = new System.Drawing.Point(402, 102);
            this.lblEstadoContratoValor.Name = "lblEstadoContratoValor";
            this.lblEstadoContratoValor.Size = new System.Drawing.Size(200, 20);
            this.lblEstadoContratoValor.TabIndex = 10;
            this.lblEstadoContratoValor.Text = "-";
            this.lblEstadoContratoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMora
            //
            this.lblMora.Location = new System.Drawing.Point(6, 124);
            this.lblMora.Name = "lblMora";
            this.lblMora.Size = new System.Drawing.Size(100, 20);
            this.lblMora.Text = "Mora:";
            this.lblMora.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMoraValor
            //
            this.lblMoraValor.AutoEllipsis = true;
            this.lblMoraValor.Location = new System.Drawing.Point(112, 124);
            this.lblMoraValor.Name = "lblMoraValor";
            this.lblMoraValor.Size = new System.Drawing.Size(490, 20);
            this.lblMoraValor.Text = "-";
            this.lblMoraValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpPeriodo
            //
            this.grpPeriodo.BackColor = System.Drawing.Color.Transparent;
            this.grpPeriodo.Controls.Add(this.lblPeriodo);
            this.grpPeriodo.Controls.Add(this.cmbPeriodo);
            this.grpPeriodo.Controls.Add(this.lblEstadoMes);
            this.grpPeriodo.Controls.Add(this.lblEstadoMesValor);
            this.grpPeriodo.Controls.Add(this.lblFechaPago);
            this.grpPeriodo.Controls.Add(this.dtpFechaPago);
            this.grpPeriodo.Controls.Add(this.lblMontoBase);
            this.grpPeriodo.Controls.Add(this.lblMontoBaseValor);
            this.grpPeriodo.Controls.Add(this.lblRecargo);
            this.grpPeriodo.Controls.Add(this.lblRecargoValor);
            this.grpPeriodo.Controls.Add(this.lblTotal);
            this.grpPeriodo.Controls.Add(this.lblTotalValor);
            this.grpPeriodo.Enabled = false;
            this.grpPeriodo.ForeColor = System.Drawing.Color.White;
            this.grpPeriodo.Location = new System.Drawing.Point(12, 194);
            this.grpPeriodo.Name = "grpPeriodo";
            this.grpPeriodo.Size = new System.Drawing.Size(616, 150);
            this.grpPeriodo.TabIndex = 2;
            this.grpPeriodo.TabStop = false;
            this.grpPeriodo.Text = "Período a pagar";
            //
            // lblPeriodo
            //
            this.lblPeriodo.Location = new System.Drawing.Point(6, 25);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(100, 21);
            this.lblPeriodo.TabIndex = 0;
            this.lblPeriodo.Text = "Período:";
            this.lblPeriodo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cmbPeriodo
            //
            this.cmbPeriodo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbPeriodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPeriodo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbPeriodo.ForeColor = System.Drawing.Color.White;
            this.cmbPeriodo.FormattingEnabled = true;
            this.cmbPeriodo.Location = new System.Drawing.Point(112, 25);
            this.cmbPeriodo.Name = "cmbPeriodo";
            this.cmbPeriodo.Size = new System.Drawing.Size(180, 21);
            this.cmbPeriodo.TabIndex = 1;
            this.cmbPeriodo.SelectedIndexChanged += new System.EventHandler(this.DatosPago_Changed);
            //
            // lblEstadoMes
            //
            this.lblEstadoMes.Location = new System.Drawing.Point(296, 25);
            this.lblEstadoMes.Name = "lblEstadoMes";
            this.lblEstadoMes.Size = new System.Drawing.Size(100, 21);
            this.lblEstadoMes.TabIndex = 2;
            this.lblEstadoMes.Text = "Estado del mes:";
            this.lblEstadoMes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblEstadoMesValor
            //
            this.lblEstadoMesValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoMesValor.Location = new System.Drawing.Point(402, 25);
            this.lblEstadoMesValor.Name = "lblEstadoMesValor";
            this.lblEstadoMesValor.Size = new System.Drawing.Size(200, 21);
            this.lblEstadoMesValor.TabIndex = 3;
            this.lblEstadoMesValor.Text = "-";
            this.lblEstadoMesValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblFechaPago
            //
            this.lblFechaPago.Location = new System.Drawing.Point(6, 58);
            this.lblFechaPago.Name = "lblFechaPago";
            this.lblFechaPago.Size = new System.Drawing.Size(100, 20);
            this.lblFechaPago.TabIndex = 4;
            this.lblFechaPago.Text = "Fecha de pago:";
            this.lblFechaPago.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dtpFechaPago
            //
            this.dtpFechaPago.CalendarForeColor = System.Drawing.Color.White;
            this.dtpFechaPago.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.dtpFechaPago.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.dtpFechaPago.CalendarTitleForeColor = System.Drawing.Color.White;
            this.dtpFechaPago.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaPago.Location = new System.Drawing.Point(112, 58);
            this.dtpFechaPago.Name = "dtpFechaPago";
            this.dtpFechaPago.Size = new System.Drawing.Size(180, 20);
            this.dtpFechaPago.TabIndex = 5;
            this.dtpFechaPago.ValueChanged += new System.EventHandler(this.DatosPago_Changed);
            //
            // lblMontoBase
            //
            this.lblMontoBase.Location = new System.Drawing.Point(6, 90);
            this.lblMontoBase.Name = "lblMontoBase";
            this.lblMontoBase.Size = new System.Drawing.Size(100, 20);
            this.lblMontoBase.TabIndex = 6;
            this.lblMontoBase.Text = "Alquiler:";
            this.lblMontoBase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMontoBaseValor
            //
            this.lblMontoBaseValor.Location = new System.Drawing.Point(112, 90);
            this.lblMontoBaseValor.Name = "lblMontoBaseValor";
            this.lblMontoBaseValor.Size = new System.Drawing.Size(180, 20);
            this.lblMontoBaseValor.TabIndex = 7;
            this.lblMontoBaseValor.Text = "-";
            this.lblMontoBaseValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblRecargo
            //
            this.lblRecargo.Location = new System.Drawing.Point(296, 90);
            this.lblRecargo.Name = "lblRecargo";
            this.lblRecargo.Size = new System.Drawing.Size(100, 20);
            this.lblRecargo.TabIndex = 8;
            this.lblRecargo.Text = "Recargo (10%):";
            this.lblRecargo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblRecargoValor
            //
            this.lblRecargoValor.Location = new System.Drawing.Point(402, 90);
            this.lblRecargoValor.Name = "lblRecargoValor";
            this.lblRecargoValor.Size = new System.Drawing.Size(200, 20);
            this.lblRecargoValor.TabIndex = 9;
            this.lblRecargoValor.Text = "-";
            this.lblRecargoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTotal
            //
            this.lblTotal.Location = new System.Drawing.Point(6, 118);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(100, 20);
            this.lblTotal.TabIndex = 10;
            this.lblTotal.Text = "Total a pagar:";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTotalValor
            //
            this.lblTotalValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalValor.Location = new System.Drawing.Point(112, 118);
            this.lblTotalValor.Name = "lblTotalValor";
            this.lblTotalValor.Size = new System.Drawing.Size(180, 20);
            this.lblTotalValor.TabIndex = 11;
            this.lblTotalValor.Text = "-";
            this.lblTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpResumen
            //
            this.grpResumen.BackColor = System.Drawing.Color.Transparent;
            this.grpResumen.Controls.Add(this.lblResumenValor);
            this.grpResumen.ForeColor = System.Drawing.Color.White;
            this.grpResumen.Location = new System.Drawing.Point(12, 352);
            this.grpResumen.Name = "grpResumen";
            this.grpResumen.Size = new System.Drawing.Size(616, 100);
            this.grpResumen.TabIndex = 3;
            this.grpResumen.TabStop = false;
            this.grpResumen.Text = "Resumen del pago";
            //
            // lblResumenValor
            //
            this.lblResumenValor.Location = new System.Drawing.Point(10, 20);
            this.lblResumenValor.Name = "lblResumenValor";
            this.lblResumenValor.Size = new System.Drawing.Size(596, 72);
            this.lblResumenValor.TabIndex = 0;
            this.lblResumenValor.Text = "Busque un contrato para registrar un pago.";
            //
            // btnRegistrar
            //
            this.btnRegistrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRegistrar.Enabled = false;
            this.btnRegistrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRegistrar.ForeColor = System.Drawing.Color.White;
            this.btnRegistrar.Location = new System.Drawing.Point(468, 462);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(160, 36);
            this.btnRegistrar.TabIndex = 4;
            this.btnRegistrar.Text = "Registrar Pago";
            this.btnRegistrar.UseVisualStyleBackColor = false;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // FormRegistrarPago
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(640, 510);
            this.Controls.Add(this.lblActualizacion);
            this.Controls.Add(this.grpContrato);
            this.Controls.Add(this.grpPeriodo);
            this.Controls.Add(this.grpResumen);
            this.Controls.Add(this.btnRegistrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormRegistrarPago";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registrar Pago";
            this.Load += new System.EventHandler(this.FormRegistrarPago_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudNroContrato)).EndInit();
            this.grpContrato.ResumeLayout(false);
            this.grpPeriodo.ResumeLayout(false);
            this.grpResumen.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblActualizacion;
        private System.Windows.Forms.GroupBox grpContrato;
        private System.Windows.Forms.Label lblNroContrato;
        private System.Windows.Forms.NumericUpDown nudNroContrato;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblInquilino;
        private System.Windows.Forms.Label lblInquilinoValor;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.Label lblDireccionValor;
        private System.Windows.Forms.Label lblMontoMensual;
        private System.Windows.Forms.Label lblMontoMensualValor;
        private System.Windows.Forms.Label lblEstadoContrato;
        private System.Windows.Forms.Label lblEstadoContratoValor;
        private System.Windows.Forms.Label lblMora;
        private System.Windows.Forms.Label lblMoraValor;
        private System.Windows.Forms.GroupBox grpPeriodo;
        private System.Windows.Forms.Label lblPeriodo;
        private System.Windows.Forms.ComboBox cmbPeriodo;
        private System.Windows.Forms.Label lblEstadoMes;
        private System.Windows.Forms.Label lblEstadoMesValor;
        private System.Windows.Forms.Label lblFechaPago;
        private System.Windows.Forms.DateTimePicker dtpFechaPago;
        private System.Windows.Forms.Label lblMontoBase;
        private System.Windows.Forms.Label lblMontoBaseValor;
        private System.Windows.Forms.Label lblRecargo;
        private System.Windows.Forms.Label lblRecargoValor;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTotalValor;
        private System.Windows.Forms.GroupBox grpResumen;
        private System.Windows.Forms.Label lblResumenValor;
        private System.Windows.Forms.Button btnRegistrar;
    }
}
