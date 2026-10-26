namespace TpIngSoft
{
    partial class FormVerAnalisis
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
            this.lblProyectoValor = new System.Windows.Forms.Label();
            this.panelAnalisis = new TpIngSoft.PanelAnalisis();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblProyectoValor
            //
            this.lblProyectoValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblProyectoValor.AutoEllipsis = true;
            this.lblProyectoValor.BackColor = System.Drawing.Color.Transparent;
            this.lblProyectoValor.Location = new System.Drawing.Point(12, 10);
            this.lblProyectoValor.Name = "lblProyectoValor";
            this.lblProyectoValor.Size = new System.Drawing.Size(976, 22);
            this.lblProyectoValor.Text = "-";
            this.lblProyectoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelAnalisis
            //
            this.panelAnalisis.Location = new System.Drawing.Point(12, 38);
            this.panelAnalisis.Name = "panelAnalisis";
            this.panelAnalisis.Size = new System.Drawing.Size(976, 380);
            //
            // btnCerrar
            //
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(888, 428);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // FormVerAnalisis
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 472);
            this.Controls.Add(this.lblProyectoValor);
            this.Controls.Add(this.panelAnalisis);
            this.Controls.Add(this.btnCerrar);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1016, 511);
            this.Name = "FormVerAnalisis";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Análisis Comparativo";
            this.Load += new System.EventHandler(this.FormVerAnalisis_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblProyectoValor;
        private TpIngSoft.PanelAnalisis panelAnalisis;
        private System.Windows.Forms.Button btnCerrar;
    }
}
