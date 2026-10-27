namespace TpIngSoft
{
    partial class FormRankingAgentes
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
            this.grpFiltros = new System.Windows.Forms.GroupBox();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.cmbPeriodo = new System.Windows.Forms.ComboBox();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.lblOrden = new System.Windows.Forms.Label();
            this.cmbOrden = new System.Windows.Forms.ComboBox();
            this.lblVista = new System.Windows.Forms.Label();
            this.rbMasVendieron = new System.Windows.Forms.RadioButton();
            this.rbMenosVendieron = new System.Windows.Forms.RadioButton();
            this.dgvRanking = new System.Windows.Forms.DataGridView();
            this.lblResumen = new System.Windows.Forms.Label();
            this.btnExportarPdf = new System.Windows.Forms.Button();
            this.grpFiltros.SuspendLayout();
            this.SuspendLayout();
            //
            // grpFiltros
            //
            this.grpFiltros.BackColor = System.Drawing.Color.Transparent;
            this.grpFiltros.Controls.Add(this.lblPeriodo);
            this.grpFiltros.Controls.Add(this.cmbPeriodo);
            this.grpFiltros.Controls.Add(this.lblDesde);
            this.grpFiltros.Controls.Add(this.dtpDesde);
            this.grpFiltros.Controls.Add(this.lblHasta);
            this.grpFiltros.Controls.Add(this.dtpHasta);
            this.grpFiltros.Controls.Add(this.lblOrden);
            this.grpFiltros.Controls.Add(this.cmbOrden);
            this.grpFiltros.Controls.Add(this.lblVista);
            this.grpFiltros.Controls.Add(this.rbMasVendieron);
            this.grpFiltros.Controls.Add(this.rbMenosVendieron);
            this.grpFiltros.ForeColor = System.Drawing.Color.White;
            this.grpFiltros.Location = new System.Drawing.Point(12, 12);
            this.grpFiltros.Name = "grpFiltros";
            this.grpFiltros.Size = new System.Drawing.Size(976, 98);
            this.grpFiltros.TabStop = false;
            this.grpFiltros.Text = "Filtros";
            //
            // lblPeriodo
            //
            this.lblPeriodo.Location = new System.Drawing.Point(6, 26);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(80, 20);
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
            this.cmbPeriodo.Location = new System.Drawing.Point(92, 26);
            this.cmbPeriodo.Name = "cmbPeriodo";
            this.cmbPeriodo.Size = new System.Drawing.Size(150, 21);
            this.cmbPeriodo.SelectedIndexChanged += new System.EventHandler(this.Filtros_Changed);
            //
            // lblDesde
            //
            this.lblDesde.Location = new System.Drawing.Point(252, 26);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(56, 20);
            this.lblDesde.Text = "Desde:";
            this.lblDesde.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dtpDesde
            //
            this.dtpDesde.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(314, 26);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(110, 20);
            this.dtpDesde.ValueChanged += new System.EventHandler(this.Filtros_Changed);
            //
            // lblHasta
            //
            this.lblHasta.Location = new System.Drawing.Point(434, 26);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(56, 20);
            this.lblHasta.Text = "Hasta:";
            this.lblHasta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dtpHasta
            //
            this.dtpHasta.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(496, 26);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(110, 20);
            this.dtpHasta.ValueChanged += new System.EventHandler(this.Filtros_Changed);
            //
            // lblOrden
            //
            this.lblOrden.Location = new System.Drawing.Point(616, 26);
            this.lblOrden.Name = "lblOrden";
            this.lblOrden.Size = new System.Drawing.Size(100, 20);
            this.lblOrden.Text = "Ordenar por:";
            this.lblOrden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cmbOrden
            //
            this.cmbOrden.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbOrden.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrden.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbOrden.ForeColor = System.Drawing.Color.White;
            this.cmbOrden.FormattingEnabled = true;
            this.cmbOrden.Location = new System.Drawing.Point(722, 26);
            this.cmbOrden.Name = "cmbOrden";
            this.cmbOrden.Size = new System.Drawing.Size(170, 21);
            this.cmbOrden.SelectedIndexChanged += new System.EventHandler(this.Filtros_Changed);
            //
            // lblVista
            //
            this.lblVista.Location = new System.Drawing.Point(6, 60);
            this.lblVista.Name = "lblVista";
            this.lblVista.Size = new System.Drawing.Size(80, 20);
            this.lblVista.Text = "Vista:";
            this.lblVista.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // rbMasVendieron
            //
            this.rbMasVendieron.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMasVendieron.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.rbMasVendieron.Checked = true;
            this.rbMasVendieron.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMasVendieron.ForeColor = System.Drawing.Color.White;
            this.rbMasVendieron.Location = new System.Drawing.Point(92, 58);
            this.rbMasVendieron.Name = "rbMasVendieron";
            this.rbMasVendieron.Size = new System.Drawing.Size(240, 28);
            this.rbMasVendieron.TabStop = true;
            this.rbMasVendieron.Text = "Top 5 más vendieron";
            this.rbMasVendieron.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMasVendieron.UseVisualStyleBackColor = false;
            this.rbMasVendieron.CheckedChanged += new System.EventHandler(this.Vista_CheckedChanged);
            //
            // rbMenosVendieron
            //
            this.rbMenosVendieron.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMenosVendieron.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.rbMenosVendieron.Checked = false;
            this.rbMenosVendieron.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbMenosVendieron.ForeColor = System.Drawing.Color.White;
            this.rbMenosVendieron.Location = new System.Drawing.Point(340, 58);
            this.rbMenosVendieron.Name = "rbMenosVendieron";
            this.rbMenosVendieron.Size = new System.Drawing.Size(240, 28);
            this.rbMenosVendieron.TabStop = true;
            this.rbMenosVendieron.Text = "Top 5 menos vendieron";
            this.rbMenosVendieron.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbMenosVendieron.UseVisualStyleBackColor = false;
            this.rbMenosVendieron.CheckedChanged += new System.EventHandler(this.Vista_CheckedChanged);
            //
            // dgvRanking
            //
            this.dgvRanking.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.dgvRanking.Location = new System.Drawing.Point(12, 118);
            this.dgvRanking.Name = "dgvRanking";
            this.dgvRanking.Size = new System.Drawing.Size(976, 330);
            //
            // lblResumen
            //
            this.lblResumen.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.lblResumen.AutoEllipsis = true;
            this.lblResumen.BackColor = System.Drawing.Color.Transparent;
            this.lblResumen.Location = new System.Drawing.Point(12, 456);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(790, 40);
            this.lblResumen.Text = "";
            this.lblResumen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnExportarPdf
            //
            this.btnExportarPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnExportarPdf.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExportarPdf.ForeColor = System.Drawing.Color.White;
            this.btnExportarPdf.Location = new System.Drawing.Point(818, 458);
            this.btnExportarPdf.Name = "btnExportarPdf";
            this.btnExportarPdf.Size = new System.Drawing.Size(170, 36);
            this.btnExportarPdf.Text = "Exportar PDF";
            this.btnExportarPdf.UseVisualStyleBackColor = false;
            this.btnExportarPdf.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.btnExportarPdf.Click += new System.EventHandler(this.btnExportarPdf_Click);
            //
            // FormRankingAgentes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 506);
            this.Controls.Add(this.grpFiltros);
            this.Controls.Add(this.dgvRanking);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.btnExportarPdf);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1016, 545);
            this.Name = "FormRankingAgentes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ranking de Agentes";
            this.Load += new System.EventHandler(this.FormRankingAgentes_Load);
            this.grpFiltros.ResumeLayout(false);
            this.grpFiltros.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox grpFiltros;
        private System.Windows.Forms.Label lblPeriodo;
        private System.Windows.Forms.ComboBox cmbPeriodo;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label lblOrden;
        private System.Windows.Forms.ComboBox cmbOrden;
        private System.Windows.Forms.Label lblVista;
        private System.Windows.Forms.RadioButton rbMasVendieron;
        private System.Windows.Forms.RadioButton rbMenosVendieron;
        private System.Windows.Forms.DataGridView dgvRanking;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.Button btnExportarPdf;
    }
}
