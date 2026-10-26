namespace TpIngSoft
{
    partial class FormDecisionDirectorio
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
            this.panelAnalisis = new TpIngSoft.PanelAnalisis();
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnAprobar = new System.Windows.Forms.Button();
            this.btnRechazar = new System.Windows.Forms.Button();
            this.btnComisiones = new System.Windows.Forms.Button();
            this.btnInforme = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // dgvProyectos
            //
            this.dgvProyectos.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.dgvProyectos.Location = new System.Drawing.Point(12, 12);
            this.dgvProyectos.Name = "dgvProyectos";
            this.dgvProyectos.Size = new System.Drawing.Size(976, 186);
            this.dgvProyectos.SelectionChanged += new System.EventHandler(this.dgvProyectos_SelectionChanged);
            //
            // panelAnalisis
            //
            this.panelAnalisis.Location = new System.Drawing.Point(12, 206);
            this.panelAnalisis.Name = "panelAnalisis";
            this.panelAnalisis.Size = new System.Drawing.Size(976, 380);
            //
            // lblEstado
            //
            this.lblEstado.AutoEllipsis = true;
            this.lblEstado.BackColor = System.Drawing.Color.Transparent;
            this.lblEstado.Location = new System.Drawing.Point(12, 600);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(320, 36);
            this.lblEstado.Text = "";
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnAprobar
            //
            this.btnAprobar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnAprobar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAprobar.ForeColor = System.Drawing.Color.White;
            this.btnAprobar.Location = new System.Drawing.Point(668, 600);
            this.btnAprobar.Name = "btnAprobar";
            this.btnAprobar.Size = new System.Drawing.Size(150, 36);
            this.btnAprobar.Text = "Aprobar";
            this.btnAprobar.UseVisualStyleBackColor = false;
            this.btnAprobar.Click += new System.EventHandler(this.btnAprobar_Click);
            //
            // btnRechazar
            //
            this.btnRechazar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRechazar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRechazar.ForeColor = System.Drawing.Color.White;
            this.btnRechazar.Location = new System.Drawing.Point(828, 600);
            this.btnRechazar.Name = "btnRechazar";
            this.btnRechazar.Size = new System.Drawing.Size(160, 36);
            this.btnRechazar.Text = "Rechazar";
            this.btnRechazar.UseVisualStyleBackColor = false;
            this.btnRechazar.Click += new System.EventHandler(this.btnRechazar_Click);
            //
            // btnComisiones
            //
            this.btnComisiones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnComisiones.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnComisiones.ForeColor = System.Drawing.Color.White;
            this.btnComisiones.Location = new System.Drawing.Point(348, 600);
            this.btnComisiones.Name = "btnComisiones";
            this.btnComisiones.Size = new System.Drawing.Size(150, 36);
            this.btnComisiones.Text = "Comisiones";
            this.btnComisiones.UseVisualStyleBackColor = false;
            this.btnComisiones.Click += new System.EventHandler(this.btnComisiones_Click);
            //
            // btnInforme
            //
            this.btnInforme.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnInforme.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnInforme.ForeColor = System.Drawing.Color.White;
            this.btnInforme.Location = new System.Drawing.Point(508, 600);
            this.btnInforme.Name = "btnInforme";
            this.btnInforme.Size = new System.Drawing.Size(150, 36);
            this.btnInforme.Text = "Informe PDF";
            this.btnInforme.UseVisualStyleBackColor = false;
            this.btnInforme.Click += new System.EventHandler(this.btnInforme_Click);
            //
            // FormDecisionDirectorio
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 648);
            this.Controls.Add(this.dgvProyectos);
            this.Controls.Add(this.panelAnalisis);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.btnAprobar);
            this.Controls.Add(this.btnRechazar);
            this.Controls.Add(this.btnComisiones);
            this.Controls.Add(this.btnInforme);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1016, 687);
            this.Name = "FormDecisionDirectorio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Decisión del Directorio";
            this.Load += new System.EventHandler(this.FormDecisionDirectorio_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dgvProyectos;
        private TpIngSoft.PanelAnalisis panelAnalisis;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Button btnAprobar;
        private System.Windows.Forms.Button btnRechazar;
        private System.Windows.Forms.Button btnComisiones;
        private System.Windows.Forms.Button btnInforme;
    }
}
