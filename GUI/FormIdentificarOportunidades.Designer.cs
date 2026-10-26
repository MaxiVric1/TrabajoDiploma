namespace TpIngSoft
{
    partial class FormIdentificarOportunidades
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
            this.dgvCasas = new System.Windows.Forms.DataGridView();
            this.lblEstadoSeleccion = new System.Windows.Forms.Label();
            this.btnCrearProyecto = new System.Windows.Forms.Button();
            this.btnVerAnalisis = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // dgvCasas
            //
            this.dgvCasas.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.dgvCasas.Location = new System.Drawing.Point(12, 12);
            this.dgvCasas.Name = "dgvCasas";
            this.dgvCasas.Size = new System.Drawing.Size(936, 420);
            this.dgvCasas.SelectionChanged += new System.EventHandler(this.dgvCasas_SelectionChanged);
            //
            // lblEstadoSeleccion
            //
            this.lblEstadoSeleccion.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lblEstadoSeleccion.AutoEllipsis = true;
            this.lblEstadoSeleccion.BackColor = System.Drawing.Color.Transparent;
            this.lblEstadoSeleccion.Location = new System.Drawing.Point(12, 442);
            this.lblEstadoSeleccion.Name = "lblEstadoSeleccion";
            this.lblEstadoSeleccion.Size = new System.Drawing.Size(600, 34);
            this.lblEstadoSeleccion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCrearProyecto
            //
            this.btnCrearProyecto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCrearProyecto.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCrearProyecto.ForeColor = System.Drawing.Color.White;
            this.btnCrearProyecto.Location = new System.Drawing.Point(628, 442);
            this.btnCrearProyecto.Name = "btnCrearProyecto";
            this.btnCrearProyecto.Size = new System.Drawing.Size(150, 34);
            this.btnCrearProyecto.Text = "Crear Proyecto";
            this.btnCrearProyecto.UseVisualStyleBackColor = false;
            this.btnCrearProyecto.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.btnCrearProyecto.Click += new System.EventHandler(this.btnCrearProyecto_Click);
            //
            // btnVerAnalisis
            //
            this.btnVerAnalisis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnVerAnalisis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVerAnalisis.ForeColor = System.Drawing.Color.White;
            this.btnVerAnalisis.Location = new System.Drawing.Point(788, 442);
            this.btnVerAnalisis.Name = "btnVerAnalisis";
            this.btnVerAnalisis.Size = new System.Drawing.Size(160, 34);
            this.btnVerAnalisis.Text = "Ver Análisis";
            this.btnVerAnalisis.UseVisualStyleBackColor = false;
            this.btnVerAnalisis.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.btnVerAnalisis.Click += new System.EventHandler(this.btnVerAnalisis_Click);
            //
            // FormIdentificarOportunidades
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(960, 488);
            this.Controls.Add(this.dgvCasas);
            this.Controls.Add(this.lblEstadoSeleccion);
            this.Controls.Add(this.btnCrearProyecto);
            this.Controls.Add(this.btnVerAnalisis);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(976, 527);
            this.Name = "FormIdentificarOportunidades";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Identificar Oportunidades";
            this.Load += new System.EventHandler(this.FormIdentificarOportunidades_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dgvCasas;
        private System.Windows.Forms.Label lblEstadoSeleccion;
        private System.Windows.Forms.Button btnCrearProyecto;
        private System.Windows.Forms.Button btnVerAnalisis;
    }
}
