namespace TpIngSoft
{
    partial class FormGestionFideicomiso
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
            this.dgvProyectos = new System.Windows.Forms.DataGridView();
            this.grpTablero = new System.Windows.Forms.GroupBox();
            this.lblProyectoValor = new System.Windows.Forms.Label();
            this.barFondeo = new System.Windows.Forms.ProgressBar();
            this.lblPorcentajeValor = new System.Windows.Forms.Label();
            this.lblNecesario = new System.Windows.Forms.Label();
            this.lblNecesarioValor = new System.Windows.Forms.Label();
            this.lblSuscripto = new System.Windows.Forms.Label();
            this.lblSuscriptoValor = new System.Windows.Forms.Label();
            this.lblRestante = new System.Windows.Forms.Label();
            this.lblRestanteValor = new System.Windows.Forms.Label();
            this.lblDolarMep = new System.Windows.Forms.Label();
            this.lblDolarMepValor = new System.Windows.Forms.Label();
            this.lblDolarBlue = new System.Windows.Forms.Label();
            this.lblDolarBlueValor = new System.Windows.Forms.Label();
            this.lblRestantePesos = new System.Windows.Forms.Label();
            this.lblRestantePesosValor = new System.Windows.Forms.Label();
            this.grpInversores = new System.Windows.Forms.GroupBox();
            this.dgvInversores = new System.Windows.Forms.DataGridView();
            this.lblActualizacion = new System.Windows.Forms.Label();
            this.btnAgregarInversor = new System.Windows.Forms.Button();
            this.btnInforme = new System.Windows.Forms.Button();
            this.btnVerSnapshot = new System.Windows.Forms.Button();
            this.grpTablero.SuspendLayout();
            this.grpInversores.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvProyectos
            //
            this.dgvProyectos.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.dgvProyectos.Location = new System.Drawing.Point(12, 12);
            this.dgvProyectos.Name = "dgvProyectos";
            this.dgvProyectos.Size = new System.Drawing.Size(976, 166);
            this.dgvProyectos.SelectionChanged += new System.EventHandler(this.dgvProyectos_SelectionChanged);
            //
            // grpTablero
            //
            this.grpTablero.BackColor = System.Drawing.Color.Transparent;
            this.grpTablero.Controls.Add(this.lblProyectoValor);
            this.grpTablero.Controls.Add(this.barFondeo);
            this.grpTablero.Controls.Add(this.lblPorcentajeValor);
            this.grpTablero.Controls.Add(this.lblNecesario);
            this.grpTablero.Controls.Add(this.lblNecesarioValor);
            this.grpTablero.Controls.Add(this.lblSuscripto);
            this.grpTablero.Controls.Add(this.lblSuscriptoValor);
            this.grpTablero.Controls.Add(this.lblRestante);
            this.grpTablero.Controls.Add(this.lblRestanteValor);
            this.grpTablero.Controls.Add(this.lblDolarMep);
            this.grpTablero.Controls.Add(this.lblDolarMepValor);
            this.grpTablero.Controls.Add(this.lblDolarBlue);
            this.grpTablero.Controls.Add(this.lblDolarBlueValor);
            this.grpTablero.Controls.Add(this.lblRestantePesos);
            this.grpTablero.Controls.Add(this.lblRestantePesosValor);
            this.grpTablero.ForeColor = System.Drawing.Color.White;
            this.grpTablero.Location = new System.Drawing.Point(12, 186);
            this.grpTablero.Name = "grpTablero";
            this.grpTablero.Size = new System.Drawing.Size(976, 172);
            this.grpTablero.TabStop = false;
            this.grpTablero.Text = "Fondeo del fideicomiso";
            //
            // lblProyectoValor
            //
            this.lblProyectoValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblProyectoValor.AutoEllipsis = true;
            this.lblProyectoValor.Location = new System.Drawing.Point(10, 22);
            this.lblProyectoValor.Name = "lblProyectoValor";
            this.lblProyectoValor.Size = new System.Drawing.Size(956, 22);
            this.lblProyectoValor.Text = "-";
            this.lblProyectoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // barFondeo
            //
            this.barFondeo.Location = new System.Drawing.Point(10, 50);
            this.barFondeo.Name = "barFondeo";
            this.barFondeo.Size = new System.Drawing.Size(780, 28);
            this.barFondeo.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            //
            // lblPorcentajeValor
            //
            this.lblPorcentajeValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblPorcentajeValor.AutoEllipsis = true;
            this.lblPorcentajeValor.Location = new System.Drawing.Point(800, 48);
            this.lblPorcentajeValor.Name = "lblPorcentajeValor";
            this.lblPorcentajeValor.Size = new System.Drawing.Size(166, 32);
            this.lblPorcentajeValor.Text = "-";
            this.lblPorcentajeValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblNecesario
            //
            this.lblNecesario.Location = new System.Drawing.Point(6, 90);
            this.lblNecesario.Name = "lblNecesario";
            this.lblNecesario.Size = new System.Drawing.Size(130, 20);
            this.lblNecesario.Text = "Capital necesario:";
            this.lblNecesario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblNecesarioValor
            //
            this.lblNecesarioValor.AutoEllipsis = true;
            this.lblNecesarioValor.Location = new System.Drawing.Point(142, 90);
            this.lblNecesarioValor.Name = "lblNecesarioValor";
            this.lblNecesarioValor.Size = new System.Drawing.Size(300, 20);
            this.lblNecesarioValor.Text = "-";
            this.lblNecesarioValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSuscripto
            //
            this.lblSuscripto.Location = new System.Drawing.Point(6, 114);
            this.lblSuscripto.Name = "lblSuscripto";
            this.lblSuscripto.Size = new System.Drawing.Size(130, 20);
            this.lblSuscripto.Text = "Capital suscripto:";
            this.lblSuscripto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblSuscriptoValor
            //
            this.lblSuscriptoValor.AutoEllipsis = true;
            this.lblSuscriptoValor.Location = new System.Drawing.Point(142, 114);
            this.lblSuscriptoValor.Name = "lblSuscriptoValor";
            this.lblSuscriptoValor.Size = new System.Drawing.Size(300, 20);
            this.lblSuscriptoValor.Text = "-";
            this.lblSuscriptoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblRestante
            //
            this.lblRestante.Location = new System.Drawing.Point(6, 138);
            this.lblRestante.Name = "lblRestante";
            this.lblRestante.Size = new System.Drawing.Size(130, 20);
            this.lblRestante.Text = "Capital restante:";
            this.lblRestante.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblRestanteValor
            //
            this.lblRestanteValor.AutoEllipsis = true;
            this.lblRestanteValor.Location = new System.Drawing.Point(142, 138);
            this.lblRestanteValor.Name = "lblRestanteValor";
            this.lblRestanteValor.Size = new System.Drawing.Size(300, 20);
            this.lblRestanteValor.Text = "-";
            this.lblRestanteValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDolarMep
            //
            this.lblDolarMep.Location = new System.Drawing.Point(480, 90);
            this.lblDolarMep.Name = "lblDolarMep";
            this.lblDolarMep.Size = new System.Drawing.Size(140, 20);
            this.lblDolarMep.Text = "Dólar MEP:";
            this.lblDolarMep.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDolarMepValor
            //
            this.lblDolarMepValor.AutoEllipsis = true;
            this.lblDolarMepValor.Location = new System.Drawing.Point(626, 90);
            this.lblDolarMepValor.Name = "lblDolarMepValor";
            this.lblDolarMepValor.Size = new System.Drawing.Size(340, 20);
            this.lblDolarMepValor.Text = "-";
            this.lblDolarMepValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDolarBlue
            //
            this.lblDolarBlue.Location = new System.Drawing.Point(480, 114);
            this.lblDolarBlue.Name = "lblDolarBlue";
            this.lblDolarBlue.Size = new System.Drawing.Size(140, 20);
            this.lblDolarBlue.Text = "Dólar blue:";
            this.lblDolarBlue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDolarBlueValor
            //
            this.lblDolarBlueValor.AutoEllipsis = true;
            this.lblDolarBlueValor.Location = new System.Drawing.Point(626, 114);
            this.lblDolarBlueValor.Name = "lblDolarBlueValor";
            this.lblDolarBlueValor.Size = new System.Drawing.Size(340, 20);
            this.lblDolarBlueValor.Text = "-";
            this.lblDolarBlueValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblRestantePesos
            //
            this.lblRestantePesos.Location = new System.Drawing.Point(480, 138);
            this.lblRestantePesos.Name = "lblRestantePesos";
            this.lblRestantePesos.Size = new System.Drawing.Size(140, 20);
            this.lblRestantePesos.Text = "Restante en pesos:";
            this.lblRestantePesos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblRestantePesosValor
            //
            this.lblRestantePesosValor.AutoEllipsis = true;
            this.lblRestantePesosValor.Location = new System.Drawing.Point(626, 138);
            this.lblRestantePesosValor.Name = "lblRestantePesosValor";
            this.lblRestantePesosValor.Size = new System.Drawing.Size(340, 20);
            this.lblRestantePesosValor.Text = "-";
            this.lblRestantePesosValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpInversores
            //
            this.grpInversores.BackColor = System.Drawing.Color.Transparent;
            this.grpInversores.Controls.Add(this.dgvInversores);
            this.grpInversores.ForeColor = System.Drawing.Color.White;
            this.grpInversores.Location = new System.Drawing.Point(12, 366);
            this.grpInversores.Name = "grpInversores";
            this.grpInversores.Size = new System.Drawing.Size(976, 222);
            this.grpInversores.TabStop = false;
            this.grpInversores.Text = "Inversores";
            //
            // dgvInversores
            //
            this.dgvInversores.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.dgvInversores.Location = new System.Drawing.Point(10, 20);
            this.dgvInversores.Name = "dgvInversores";
            this.dgvInversores.Size = new System.Drawing.Size(956, 192);
            //
            // lblActualizacion
            //
            this.lblActualizacion.AutoEllipsis = true;
            this.lblActualizacion.BackColor = System.Drawing.Color.Transparent;
            this.lblActualizacion.Location = new System.Drawing.Point(12, 598);
            this.lblActualizacion.Name = "lblActualizacion";
            this.lblActualizacion.Size = new System.Drawing.Size(436, 34);
            this.lblActualizacion.Text = "";
            this.lblActualizacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnAgregarInversor
            //
            this.btnAgregarInversor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnAgregarInversor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAgregarInversor.ForeColor = System.Drawing.Color.White;
            this.btnAgregarInversor.Location = new System.Drawing.Point(818, 596);
            this.btnAgregarInversor.Name = "btnAgregarInversor";
            this.btnAgregarInversor.Size = new System.Drawing.Size(170, 36);
            this.btnAgregarInversor.Text = "Agregar Inversor";
            this.btnAgregarInversor.UseVisualStyleBackColor = false;
            this.btnAgregarInversor.Click += new System.EventHandler(this.btnAgregarInversor_Click);
            //
            // btnInforme
            //
            this.btnInforme.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnInforme.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnInforme.ForeColor = System.Drawing.Color.White;
            this.btnInforme.Location = new System.Drawing.Point(638, 596);
            this.btnInforme.Name = "btnInforme";
            this.btnInforme.Size = new System.Drawing.Size(170, 36);
            this.btnInforme.Text = "Informe Inversores";
            this.btnInforme.UseVisualStyleBackColor = false;
            this.btnInforme.Click += new System.EventHandler(this.btnInforme_Click);
            //
            // btnVerSnapshot
            //
            this.btnVerSnapshot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnVerSnapshot.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVerSnapshot.ForeColor = System.Drawing.Color.White;
            this.btnVerSnapshot.Location = new System.Drawing.Point(458, 596);
            this.btnVerSnapshot.Name = "btnVerSnapshot";
            this.btnVerSnapshot.Size = new System.Drawing.Size(170, 36);
            this.btnVerSnapshot.Text = "Ver snapshot";
            this.btnVerSnapshot.UseVisualStyleBackColor = false;
            this.btnVerSnapshot.Click += new System.EventHandler(this.btnVerSnapshot_Click);
            //
            // FormGestionFideicomiso
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 644);
            this.Controls.Add(this.dgvProyectos);
            this.Controls.Add(this.grpTablero);
            this.Controls.Add(this.grpInversores);
            this.Controls.Add(this.lblActualizacion);
            this.Controls.Add(this.btnAgregarInversor);
            this.Controls.Add(this.btnInforme);
            this.Controls.Add(this.btnVerSnapshot);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1016, 683);
            this.Name = "FormGestionFideicomiso";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión del Fideicomiso";
            this.Load += new System.EventHandler(this.FormGestionFideicomiso_Load);
            this.grpTablero.ResumeLayout(false);
            this.grpTablero.PerformLayout();
            this.grpInversores.ResumeLayout(false);
            this.grpInversores.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dgvProyectos;
        private System.Windows.Forms.GroupBox grpTablero;
        private System.Windows.Forms.Label lblProyectoValor;
        private System.Windows.Forms.ProgressBar barFondeo;
        private System.Windows.Forms.Label lblPorcentajeValor;
        private System.Windows.Forms.Label lblNecesario;
        private System.Windows.Forms.Label lblNecesarioValor;
        private System.Windows.Forms.Label lblSuscripto;
        private System.Windows.Forms.Label lblSuscriptoValor;
        private System.Windows.Forms.Label lblRestante;
        private System.Windows.Forms.Label lblRestanteValor;
        private System.Windows.Forms.Label lblDolarMep;
        private System.Windows.Forms.Label lblDolarMepValor;
        private System.Windows.Forms.Label lblDolarBlue;
        private System.Windows.Forms.Label lblDolarBlueValor;
        private System.Windows.Forms.Label lblRestantePesos;
        private System.Windows.Forms.Label lblRestantePesosValor;
        private System.Windows.Forms.GroupBox grpInversores;
        private System.Windows.Forms.DataGridView dgvInversores;
        private System.Windows.Forms.Label lblActualizacion;
        private System.Windows.Forms.Button btnAgregarInversor;
        private System.Windows.Forms.Button btnInforme;
        private System.Windows.Forms.Button btnVerSnapshot;
    }
}
