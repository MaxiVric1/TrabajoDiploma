namespace TpIngSoft
{
    partial class FormVerSnapshot
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
            this.lblSnapshots = new System.Windows.Forms.Label();
            this.lstSnapshots = new System.Windows.Forms.ListBox();
            this.lblDecisionValor = new System.Windows.Forms.Label();
            this.lblQuienValor = new System.Windows.Forms.Label();
            this.lblMotivoValor = new System.Windows.Forms.Label();
            this.lblDetalle = new System.Windows.Forms.Label();
            this.txtDetalle = new System.Windows.Forms.TextBox();
            this.lblArchivoValor = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblSnapshots
            //
            this.lblSnapshots.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left));
            this.lblSnapshots.AutoEllipsis = true;
            this.lblSnapshots.BackColor = System.Drawing.Color.Transparent;
            this.lblSnapshots.Location = new System.Drawing.Point(12, 12);
            this.lblSnapshots.Name = "lblSnapshots";
            this.lblSnapshots.Size = new System.Drawing.Size(300, 20);
            this.lblSnapshots.Text = "Actas de Decisión del inmueble:";
            this.lblSnapshots.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lstSnapshots
            //
            this.lstSnapshots.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.lstSnapshots.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.lstSnapshots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSnapshots.ForeColor = System.Drawing.Color.White;
            this.lstSnapshots.FormattingEnabled = true;
            this.lstSnapshots.HorizontalScrollbar = true;
            this.lstSnapshots.IntegralHeight = false;
            this.lstSnapshots.Location = new System.Drawing.Point(12, 36);
            this.lstSnapshots.Name = "lstSnapshots";
            this.lstSnapshots.Size = new System.Drawing.Size(300, 500);
            this.lstSnapshots.SelectedIndexChanged += new System.EventHandler(this.lstSnapshots_SelectedIndexChanged);
            //
            // lblDecisionValor
            //
            this.lblDecisionValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblDecisionValor.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lblDecisionValor.AutoEllipsis = true;
            this.lblDecisionValor.BackColor = System.Drawing.Color.Transparent;
            this.lblDecisionValor.Location = new System.Drawing.Point(324, 8);
            this.lblDecisionValor.Name = "lblDecisionValor";
            this.lblDecisionValor.Size = new System.Drawing.Size(664, 32);
            this.lblDecisionValor.Text = "-";
            this.lblDecisionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblQuienValor
            //
            this.lblQuienValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblQuienValor.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lblQuienValor.AutoEllipsis = true;
            this.lblQuienValor.BackColor = System.Drawing.Color.Transparent;
            this.lblQuienValor.Location = new System.Drawing.Point(324, 42);
            this.lblQuienValor.Name = "lblQuienValor";
            this.lblQuienValor.Size = new System.Drawing.Size(664, 22);
            this.lblQuienValor.Text = "-";
            this.lblQuienValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMotivoValor
            //
            this.lblMotivoValor.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lblMotivoValor.AutoEllipsis = true;
            this.lblMotivoValor.BackColor = System.Drawing.Color.Transparent;
            this.lblMotivoValor.Location = new System.Drawing.Point(324, 66);
            this.lblMotivoValor.Name = "lblMotivoValor";
            this.lblMotivoValor.Size = new System.Drawing.Size(664, 22);
            this.lblMotivoValor.Text = "-";
            this.lblMotivoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDetalle
            //
            this.lblDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left));
            this.lblDetalle.AutoEllipsis = true;
            this.lblDetalle.BackColor = System.Drawing.Color.Transparent;
            this.lblDetalle.Location = new System.Drawing.Point(324, 92);
            this.lblDetalle.Name = "lblDetalle";
            this.lblDetalle.Size = new System.Drawing.Size(400, 20);
            this.lblDetalle.Text = "Análisis al momento de la decisión:";
            this.lblDetalle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDetalle
            //
            this.txtDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.txtDetalle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtDetalle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetalle.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtDetalle.ForeColor = System.Drawing.Color.White;
            this.txtDetalle.Location = new System.Drawing.Point(324, 116);
            this.txtDetalle.Multiline = true;
            this.txtDetalle.Name = "txtDetalle";
            this.txtDetalle.ReadOnly = true;
            this.txtDetalle.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDetalle.Size = new System.Drawing.Size(664, 420);
            this.txtDetalle.WordWrap = false;
            //
            // lblArchivoValor
            //
            this.lblArchivoValor.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lblArchivoValor.AutoEllipsis = true;
            this.lblArchivoValor.BackColor = System.Drawing.Color.Transparent;
            this.lblArchivoValor.Location = new System.Drawing.Point(12, 546);
            this.lblArchivoValor.Name = "lblArchivoValor";
            this.lblArchivoValor.Size = new System.Drawing.Size(790, 40);
            this.lblArchivoValor.Text = "";
            this.lblArchivoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCerrar
            //
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(818, 548);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(170, 36);
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // FormVerSnapshot
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 596);
            this.Controls.Add(this.lblSnapshots);
            this.Controls.Add(this.lstSnapshots);
            this.Controls.Add(this.lblDecisionValor);
            this.Controls.Add(this.lblQuienValor);
            this.Controls.Add(this.lblMotivoValor);
            this.Controls.Add(this.lblDetalle);
            this.Controls.Add(this.txtDetalle);
            this.Controls.Add(this.lblArchivoValor);
            this.Controls.Add(this.btnCerrar);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1016, 635);
            this.Name = "FormVerSnapshot";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Actas de Decisión";
            this.Load += new System.EventHandler(this.FormVerSnapshot_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblSnapshots;
        private System.Windows.Forms.ListBox lstSnapshots;
        private System.Windows.Forms.Label lblDecisionValor;
        private System.Windows.Forms.Label lblQuienValor;
        private System.Windows.Forms.Label lblMotivoValor;
        private System.Windows.Forms.Label lblDetalle;
        private System.Windows.Forms.TextBox txtDetalle;
        private System.Windows.Forms.Label lblArchivoValor;
        private System.Windows.Forms.Button btnCerrar;
    }
}
