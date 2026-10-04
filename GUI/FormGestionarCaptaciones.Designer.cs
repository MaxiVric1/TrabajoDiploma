namespace TpIngSoft
{
    partial class FormGestionarCaptaciones
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
            this.lblFiltroEstado = new System.Windows.Forms.Label();
            this.cmbFiltroEstado = new System.Windows.Forms.ComboBox();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.dgvCaptaciones = new System.Windows.Forms.DataGridView();
            this.grpRenegociacion = new System.Windows.Forms.GroupBox();
            this.lblDictamen = new System.Windows.Forms.Label();
            this.lblDictamenValor = new System.Windows.Forms.Label();
            this.lblValorActual = new System.Windows.Forms.Label();
            this.lblValorActualValor = new System.Windows.Forms.Label();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.lblMotivoValor = new System.Windows.Forms.Label();
            this.lblValorSugerido = new System.Windows.Forms.Label();
            this.lblValorSugeridoValor = new System.Windows.Forms.Label();
            this.lblNuevoValor = new System.Windows.Forms.Label();
            this.nudNuevoValor = new System.Windows.Forms.NumericUpDown();
            this.btnRenegociar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCaptaciones)).BeginInit();
            this.grpRenegociacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNuevoValor)).BeginInit();
            this.SuspendLayout();
            //
            // lblFiltroEstado
            //
            this.lblFiltroEstado.BackColor = System.Drawing.Color.Transparent;
            this.lblFiltroEstado.ForeColor = System.Drawing.Color.White;
            this.lblFiltroEstado.Location = new System.Drawing.Point(12, 14);
            this.lblFiltroEstado.Name = "lblFiltroEstado";
            this.lblFiltroEstado.Size = new System.Drawing.Size(60, 21);
            this.lblFiltroEstado.TabIndex = 0;
            this.lblFiltroEstado.Text = "Estado:";
            this.lblFiltroEstado.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cmbFiltroEstado
            //
            this.cmbFiltroEstado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbFiltroEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltroEstado.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbFiltroEstado.ForeColor = System.Drawing.Color.White;
            this.cmbFiltroEstado.FormattingEnabled = true;
            this.cmbFiltroEstado.Location = new System.Drawing.Point(78, 14);
            this.cmbFiltroEstado.Name = "cmbFiltroEstado";
            this.cmbFiltroEstado.Size = new System.Drawing.Size(150, 21);
            this.cmbFiltroEstado.TabIndex = 1;
            this.cmbFiltroEstado.SelectedIndexChanged += new System.EventHandler(this.cmbFiltroEstado_SelectedIndexChanged);
            //
            // btnActualizar
            //
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(238, 10);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 28);
            this.btnActualizar.TabIndex = 2;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // dgvCaptaciones
            //
            this.dgvCaptaciones.AllowUserToAddRows = false;
            this.dgvCaptaciones.AllowUserToDeleteRows = false;
            this.dgvCaptaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCaptaciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCaptaciones.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCaptaciones.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCaptaciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(129)))), ((int)(((byte)(158)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCaptaciones.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCaptaciones.EnableHeadersVisualStyles = false;
            this.dgvCaptaciones.GridColor = System.Drawing.Color.Black;
            this.dgvCaptaciones.Location = new System.Drawing.Point(12, 48);
            this.dgvCaptaciones.MultiSelect = false;
            this.dgvCaptaciones.Name = "dgvCaptaciones";
            this.dgvCaptaciones.ReadOnly = true;
            this.dgvCaptaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCaptaciones.Size = new System.Drawing.Size(876, 300);
            this.dgvCaptaciones.TabIndex = 3;
            this.dgvCaptaciones.SelectionChanged += new System.EventHandler(this.dgvCaptaciones_SelectionChanged);
            //
            // grpRenegociacion
            //
            this.grpRenegociacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRenegociacion.BackColor = System.Drawing.Color.Transparent;
            this.grpRenegociacion.Controls.Add(this.lblDictamen);
            this.grpRenegociacion.Controls.Add(this.lblDictamenValor);
            this.grpRenegociacion.Controls.Add(this.lblValorActual);
            this.grpRenegociacion.Controls.Add(this.lblValorActualValor);
            this.grpRenegociacion.Controls.Add(this.lblMotivo);
            this.grpRenegociacion.Controls.Add(this.lblMotivoValor);
            this.grpRenegociacion.Controls.Add(this.lblValorSugerido);
            this.grpRenegociacion.Controls.Add(this.lblValorSugeridoValor);
            this.grpRenegociacion.Controls.Add(this.lblNuevoValor);
            this.grpRenegociacion.Controls.Add(this.nudNuevoValor);
            this.grpRenegociacion.Controls.Add(this.btnRenegociar);
            this.grpRenegociacion.ForeColor = System.Drawing.Color.White;
            this.grpRenegociacion.Location = new System.Drawing.Point(12, 358);
            this.grpRenegociacion.Name = "grpRenegociacion";
            this.grpRenegociacion.Size = new System.Drawing.Size(876, 150);
            this.grpRenegociacion.TabIndex = 4;
            this.grpRenegociacion.TabStop = false;
            this.grpRenegociacion.Text = "Renegociación";
            //
            // lblDictamen
            //
            this.lblDictamen.Location = new System.Drawing.Point(6, 22);
            this.lblDictamen.Name = "lblDictamen";
            this.lblDictamen.Size = new System.Drawing.Size(150, 20);
            this.lblDictamen.TabIndex = 0;
            this.lblDictamen.Text = "Último dictamen:";
            this.lblDictamen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDictamenValor
            //
            this.lblDictamenValor.AutoEllipsis = true;
            this.lblDictamenValor.Location = new System.Drawing.Point(162, 22);
            this.lblDictamenValor.Name = "lblDictamenValor";
            this.lblDictamenValor.Size = new System.Drawing.Size(250, 20);
            this.lblDictamenValor.TabIndex = 1;
            this.lblDictamenValor.Text = "-";
            this.lblDictamenValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblValorActual
            //
            this.lblValorActual.Location = new System.Drawing.Point(430, 22);
            this.lblValorActual.Name = "lblValorActual";
            this.lblValorActual.Size = new System.Drawing.Size(150, 20);
            this.lblValorActual.TabIndex = 2;
            this.lblValorActual.Text = "Valor pretendido actual:";
            this.lblValorActual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblValorActualValor
            //
            this.lblValorActualValor.Location = new System.Drawing.Point(586, 22);
            this.lblValorActualValor.Name = "lblValorActualValor";
            this.lblValorActualValor.Size = new System.Drawing.Size(200, 20);
            this.lblValorActualValor.TabIndex = 3;
            this.lblValorActualValor.Text = "-";
            this.lblValorActualValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMotivo
            //
            this.lblMotivo.Location = new System.Drawing.Point(6, 47);
            this.lblMotivo.Name = "lblMotivo";
            this.lblMotivo.Size = new System.Drawing.Size(150, 20);
            this.lblMotivo.TabIndex = 4;
            this.lblMotivo.Text = "Motivo de rechazo:";
            this.lblMotivo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMotivoValor
            //
            this.lblMotivoValor.AutoEllipsis = true;
            this.lblMotivoValor.Location = new System.Drawing.Point(162, 47);
            this.lblMotivoValor.Name = "lblMotivoValor";
            this.lblMotivoValor.Size = new System.Drawing.Size(700, 20);
            this.lblMotivoValor.TabIndex = 5;
            this.lblMotivoValor.Text = "-";
            this.lblMotivoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblValorSugerido
            //
            this.lblValorSugerido.Location = new System.Drawing.Point(6, 72);
            this.lblValorSugerido.Name = "lblValorSugerido";
            this.lblValorSugerido.Size = new System.Drawing.Size(150, 20);
            this.lblValorSugerido.TabIndex = 6;
            this.lblValorSugerido.Text = "Valor sugerido:";
            this.lblValorSugerido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblValorSugeridoValor
            //
            this.lblValorSugeridoValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValorSugeridoValor.Location = new System.Drawing.Point(162, 72);
            this.lblValorSugeridoValor.Name = "lblValorSugeridoValor";
            this.lblValorSugeridoValor.Size = new System.Drawing.Size(250, 20);
            this.lblValorSugeridoValor.TabIndex = 7;
            this.lblValorSugeridoValor.Text = "-";
            this.lblValorSugeridoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblNuevoValor
            //
            this.lblNuevoValor.Location = new System.Drawing.Point(6, 107);
            this.lblNuevoValor.Name = "lblNuevoValor";
            this.lblNuevoValor.Size = new System.Drawing.Size(150, 20);
            this.lblNuevoValor.TabIndex = 8;
            this.lblNuevoValor.Text = "Nuevo valor pretendido:";
            this.lblNuevoValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudNuevoValor
            //
            this.nudNuevoValor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudNuevoValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudNuevoValor.DecimalPlaces = 2;
            this.nudNuevoValor.ForeColor = System.Drawing.Color.White;
            this.nudNuevoValor.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudNuevoValor.Location = new System.Drawing.Point(162, 107);
            this.nudNuevoValor.Maximum = new decimal(new int[] { 1215752191, 23, 0, 0 });
            this.nudNuevoValor.Name = "nudNuevoValor";
            this.nudNuevoValor.Size = new System.Drawing.Size(180, 20);
            this.nudNuevoValor.TabIndex = 9;
            this.nudNuevoValor.ThousandsSeparator = true;
            //
            // btnRenegociar
            //
            this.btnRenegociar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRenegociar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRenegociar.ForeColor = System.Drawing.Color.White;
            this.btnRenegociar.Location = new System.Drawing.Point(352, 102);
            this.btnRenegociar.Name = "btnRenegociar";
            this.btnRenegociar.Size = new System.Drawing.Size(180, 30);
            this.btnRenegociar.TabIndex = 10;
            this.btnRenegociar.Text = "Renegociar";
            this.btnRenegociar.UseVisualStyleBackColor = false;
            this.btnRenegociar.Click += new System.EventHandler(this.btnRenegociar_Click);
            //
            // FormGestionarCaptaciones
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(900, 520);
            this.Controls.Add(this.lblFiltroEstado);
            this.Controls.Add(this.cmbFiltroEstado);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.dgvCaptaciones);
            this.Controls.Add(this.grpRenegociacion);
            this.MinimumSize = new System.Drawing.Size(916, 559);
            this.Name = "FormGestionarCaptaciones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionar Captaciones";
            this.Load += new System.EventHandler(this.FormGestionarCaptaciones_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCaptaciones)).EndInit();
            this.grpRenegociacion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudNuevoValor)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblFiltroEstado;
        private System.Windows.Forms.ComboBox cmbFiltroEstado;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.DataGridView dgvCaptaciones;
        private System.Windows.Forms.GroupBox grpRenegociacion;
        private System.Windows.Forms.Label lblDictamen;
        private System.Windows.Forms.Label lblDictamenValor;
        private System.Windows.Forms.Label lblValorActual;
        private System.Windows.Forms.Label lblValorActualValor;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.Label lblMotivoValor;
        private System.Windows.Forms.Label lblValorSugerido;
        private System.Windows.Forms.Label lblValorSugeridoValor;
        private System.Windows.Forms.Label lblNuevoValor;
        private System.Windows.Forms.NumericUpDown nudNuevoValor;
        private System.Windows.Forms.Button btnRenegociar;
    }
}
