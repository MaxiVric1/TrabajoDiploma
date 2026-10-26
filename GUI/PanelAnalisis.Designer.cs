namespace TpIngSoft
{
    partial class PanelAnalisis
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
            this.grpEscenarioA = new System.Windows.Forms.GroupBox();
            this.txtEscenarioA = new System.Windows.Forms.TextBox();
            this.lblResultadoA = new System.Windows.Forms.Label();
            this.grpEscenarioB = new System.Windows.Forms.GroupBox();
            this.txtEscenarioB = new System.Windows.Forms.TextBox();
            this.lblResultadoB = new System.Windows.Forms.Label();
            this.picMapa = new System.Windows.Forms.PictureBox();
            this.grpMercado = new System.Windows.Forms.GroupBox();
            this.lblMep = new System.Windows.Forms.Label();
            this.lblMepValor = new System.Windows.Forms.Label();
            this.lblBlue = new System.Windows.Forms.Label();
            this.lblBlueValor = new System.Windows.Forms.Label();
            this.lblOficial = new System.Windows.Forms.Label();
            this.lblOficialValor = new System.Windows.Forms.Label();
            this.lblIcc = new System.Windows.Forms.Label();
            this.lblIccValor = new System.Windows.Forms.Label();
            this.lblIpc = new System.Windows.Forms.Label();
            this.lblIpcValor = new System.Windows.Forms.Label();
            this.grpConclusion = new System.Windows.Forms.GroupBox();
            this.lblDiferenciaValor = new System.Windows.Forms.Label();
            this.txtSupuestos = new System.Windows.Forms.TextBox();
            this.grpEscenarioA.SuspendLayout();
            this.grpEscenarioB.SuspendLayout();
            this.grpMercado.SuspendLayout();
            this.grpConclusion.SuspendLayout();
            this.SuspendLayout();
            //
            // grpEscenarioA
            //
            this.grpEscenarioA.BackColor = System.Drawing.Color.Transparent;
            this.grpEscenarioA.Controls.Add(this.txtEscenarioA);
            this.grpEscenarioA.Controls.Add(this.lblResultadoA);
            this.grpEscenarioA.ForeColor = System.Drawing.Color.White;
            this.grpEscenarioA.Location = new System.Drawing.Point(0, 0);
            this.grpEscenarioA.Name = "grpEscenarioA";
            this.grpEscenarioA.Size = new System.Drawing.Size(320, 230);
            this.grpEscenarioA.TabStop = false;
            this.grpEscenarioA.Text = "Escenario A";
            //
            // txtEscenarioA
            //
            this.txtEscenarioA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtEscenarioA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEscenarioA.ForeColor = System.Drawing.Color.White;
            this.txtEscenarioA.Location = new System.Drawing.Point(8, 20);
            this.txtEscenarioA.Multiline = true;
            this.txtEscenarioA.Name = "txtEscenarioA";
            this.txtEscenarioA.ReadOnly = true;
            this.txtEscenarioA.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEscenarioA.Size = new System.Drawing.Size(304, 170);
            this.txtEscenarioA.TabStop = false;
            //
            // lblResultadoA
            //
            this.lblResultadoA.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblResultadoA.AutoEllipsis = true;
            this.lblResultadoA.Location = new System.Drawing.Point(8, 196);
            this.lblResultadoA.Name = "lblResultadoA";
            this.lblResultadoA.Size = new System.Drawing.Size(304, 26);
            this.lblResultadoA.Text = "-";
            this.lblResultadoA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpEscenarioB
            //
            this.grpEscenarioB.BackColor = System.Drawing.Color.Transparent;
            this.grpEscenarioB.Controls.Add(this.txtEscenarioB);
            this.grpEscenarioB.Controls.Add(this.lblResultadoB);
            this.grpEscenarioB.ForeColor = System.Drawing.Color.White;
            this.grpEscenarioB.Location = new System.Drawing.Point(328, 0);
            this.grpEscenarioB.Name = "grpEscenarioB";
            this.grpEscenarioB.Size = new System.Drawing.Size(320, 230);
            this.grpEscenarioB.TabStop = false;
            this.grpEscenarioB.Text = "Escenario B: fideicomiso";
            //
            // txtEscenarioB
            //
            this.txtEscenarioB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtEscenarioB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEscenarioB.ForeColor = System.Drawing.Color.White;
            this.txtEscenarioB.Location = new System.Drawing.Point(8, 20);
            this.txtEscenarioB.Multiline = true;
            this.txtEscenarioB.Name = "txtEscenarioB";
            this.txtEscenarioB.ReadOnly = true;
            this.txtEscenarioB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEscenarioB.Size = new System.Drawing.Size(304, 170);
            this.txtEscenarioB.TabStop = false;
            //
            // lblResultadoB
            //
            this.lblResultadoB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblResultadoB.AutoEllipsis = true;
            this.lblResultadoB.Location = new System.Drawing.Point(8, 196);
            this.lblResultadoB.Name = "lblResultadoB";
            this.lblResultadoB.Size = new System.Drawing.Size(304, 26);
            this.lblResultadoB.Text = "-";
            this.lblResultadoB.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // picMapa
            //
            this.picMapa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.picMapa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picMapa.Location = new System.Drawing.Point(656, 6);
            this.picMapa.Name = "picMapa";
            this.picMapa.Size = new System.Drawing.Size(320, 224);
            this.picMapa.TabStop = false;
            //
            // grpMercado
            //
            this.grpMercado.BackColor = System.Drawing.Color.Transparent;
            this.grpMercado.Controls.Add(this.lblMep);
            this.grpMercado.Controls.Add(this.lblMepValor);
            this.grpMercado.Controls.Add(this.lblBlue);
            this.grpMercado.Controls.Add(this.lblBlueValor);
            this.grpMercado.Controls.Add(this.lblOficial);
            this.grpMercado.Controls.Add(this.lblOficialValor);
            this.grpMercado.Controls.Add(this.lblIcc);
            this.grpMercado.Controls.Add(this.lblIccValor);
            this.grpMercado.Controls.Add(this.lblIpc);
            this.grpMercado.Controls.Add(this.lblIpcValor);
            this.grpMercado.ForeColor = System.Drawing.Color.White;
            this.grpMercado.Location = new System.Drawing.Point(0, 236);
            this.grpMercado.Name = "grpMercado";
            this.grpMercado.Size = new System.Drawing.Size(486, 140);
            this.grpMercado.TabStop = false;
            this.grpMercado.Text = "Datos de mercado";
            //
            // lblMep
            //
            this.lblMep.Location = new System.Drawing.Point(6, 20);
            this.lblMep.Name = "lblMep";
            this.lblMep.Size = new System.Drawing.Size(130, 20);
            this.lblMep.Text = "Dólar MEP:";
            this.lblMep.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMepValor
            //
            this.lblMepValor.AutoEllipsis = true;
            this.lblMepValor.Location = new System.Drawing.Point(142, 20);
            this.lblMepValor.Name = "lblMepValor";
            this.lblMepValor.Size = new System.Drawing.Size(334, 20);
            this.lblMepValor.Text = "-";
            this.lblMepValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblBlue
            //
            this.lblBlue.Location = new System.Drawing.Point(6, 43);
            this.lblBlue.Name = "lblBlue";
            this.lblBlue.Size = new System.Drawing.Size(130, 20);
            this.lblBlue.Text = "Dólar blue:";
            this.lblBlue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblBlueValor
            //
            this.lblBlueValor.AutoEllipsis = true;
            this.lblBlueValor.Location = new System.Drawing.Point(142, 43);
            this.lblBlueValor.Name = "lblBlueValor";
            this.lblBlueValor.Size = new System.Drawing.Size(334, 20);
            this.lblBlueValor.Text = "-";
            this.lblBlueValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblOficial
            //
            this.lblOficial.Location = new System.Drawing.Point(6, 66);
            this.lblOficial.Name = "lblOficial";
            this.lblOficial.Size = new System.Drawing.Size(130, 20);
            this.lblOficial.Text = "Dólar oficial (BCRA):";
            this.lblOficial.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblOficialValor
            //
            this.lblOficialValor.AutoEllipsis = true;
            this.lblOficialValor.Location = new System.Drawing.Point(142, 66);
            this.lblOficialValor.Name = "lblOficialValor";
            this.lblOficialValor.Size = new System.Drawing.Size(334, 20);
            this.lblOficialValor.Text = "-";
            this.lblOficialValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblIcc
            //
            this.lblIcc.Location = new System.Drawing.Point(6, 89);
            this.lblIcc.Name = "lblIcc";
            this.lblIcc.Size = new System.Drawing.Size(130, 20);
            this.lblIcc.Text = "ICC (INDEC):";
            this.lblIcc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblIccValor
            //
            this.lblIccValor.AutoEllipsis = true;
            this.lblIccValor.Location = new System.Drawing.Point(142, 89);
            this.lblIccValor.Name = "lblIccValor";
            this.lblIccValor.Size = new System.Drawing.Size(334, 20);
            this.lblIccValor.Text = "-";
            this.lblIccValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblIpc
            //
            this.lblIpc.Location = new System.Drawing.Point(6, 112);
            this.lblIpc.Name = "lblIpc";
            this.lblIpc.Size = new System.Drawing.Size(130, 20);
            this.lblIpc.Text = "IPC (BCRA):";
            this.lblIpc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblIpcValor
            //
            this.lblIpcValor.AutoEllipsis = true;
            this.lblIpcValor.Location = new System.Drawing.Point(142, 112);
            this.lblIpcValor.Name = "lblIpcValor";
            this.lblIpcValor.Size = new System.Drawing.Size(334, 20);
            this.lblIpcValor.Text = "-";
            this.lblIpcValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpConclusion
            //
            this.grpConclusion.BackColor = System.Drawing.Color.Transparent;
            this.grpConclusion.Controls.Add(this.lblDiferenciaValor);
            this.grpConclusion.Controls.Add(this.txtSupuestos);
            this.grpConclusion.ForeColor = System.Drawing.Color.White;
            this.grpConclusion.Location = new System.Drawing.Point(494, 236);
            this.grpConclusion.Name = "grpConclusion";
            this.grpConclusion.Size = new System.Drawing.Size(482, 140);
            this.grpConclusion.TabStop = false;
            this.grpConclusion.Text = "Diferencia y recomendación";
            //
            // lblDiferenciaValor
            //
            this.lblDiferenciaValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblDiferenciaValor.AutoEllipsis = true;
            this.lblDiferenciaValor.Location = new System.Drawing.Point(10, 20);
            this.lblDiferenciaValor.Name = "lblDiferenciaValor";
            this.lblDiferenciaValor.Size = new System.Drawing.Size(462, 34);
            this.lblDiferenciaValor.Text = "-";
            this.lblDiferenciaValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtSupuestos
            //
            this.txtSupuestos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtSupuestos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSupuestos.ForeColor = System.Drawing.Color.White;
            this.txtSupuestos.Location = new System.Drawing.Point(10, 58);
            this.txtSupuestos.Multiline = true;
            this.txtSupuestos.Name = "txtSupuestos";
            this.txtSupuestos.ReadOnly = true;
            this.txtSupuestos.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSupuestos.Size = new System.Drawing.Size(462, 74);
            this.txtSupuestos.TabStop = false;
            //
            // PanelAnalisis
            //
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.grpEscenarioA);
            this.Controls.Add(this.grpEscenarioB);
            this.Controls.Add(this.picMapa);
            this.Controls.Add(this.grpMercado);
            this.Controls.Add(this.grpConclusion);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "PanelAnalisis";
            this.Size = new System.Drawing.Size(976, 380);
            this.grpEscenarioA.ResumeLayout(false);
            this.grpEscenarioA.PerformLayout();
            this.grpEscenarioB.ResumeLayout(false);
            this.grpEscenarioB.PerformLayout();
            this.grpMercado.ResumeLayout(false);
            this.grpMercado.PerformLayout();
            this.grpConclusion.ResumeLayout(false);
            this.grpConclusion.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox grpEscenarioA;
        private System.Windows.Forms.TextBox txtEscenarioA;
        private System.Windows.Forms.Label lblResultadoA;
        private System.Windows.Forms.GroupBox grpEscenarioB;
        private System.Windows.Forms.TextBox txtEscenarioB;
        private System.Windows.Forms.Label lblResultadoB;
        private System.Windows.Forms.PictureBox picMapa;
        private System.Windows.Forms.GroupBox grpMercado;
        private System.Windows.Forms.Label lblMep;
        private System.Windows.Forms.Label lblMepValor;
        private System.Windows.Forms.Label lblBlue;
        private System.Windows.Forms.Label lblBlueValor;
        private System.Windows.Forms.Label lblOficial;
        private System.Windows.Forms.Label lblOficialValor;
        private System.Windows.Forms.Label lblIcc;
        private System.Windows.Forms.Label lblIccValor;
        private System.Windows.Forms.Label lblIpc;
        private System.Windows.Forms.Label lblIpcValor;
        private System.Windows.Forms.GroupBox grpConclusion;
        private System.Windows.Forms.Label lblDiferenciaValor;
        private System.Windows.Forms.TextBox txtSupuestos;
    }
}
