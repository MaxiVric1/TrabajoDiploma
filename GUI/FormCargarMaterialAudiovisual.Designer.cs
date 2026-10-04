namespace TpIngSoft
{
    partial class FormCargarMaterialAudiovisual
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
            System.Windows.Forms.DataGridViewCellStyle estiloEncabezado = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle estiloCelda = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblInmuebles = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.dgvInmuebles = new System.Windows.Forms.DataGridView();
            this.grpNuevoMaterial = new System.Windows.Forms.GroupBox();
            this.lblTextoComercial = new System.Windows.Forms.Label();
            this.txtTextoComercial = new System.Windows.Forms.TextBox();
            this.lblArchivos = new System.Windows.Forms.Label();
            this.btnAgregarArchivo = new System.Windows.Forms.Button();
            this.dgvArchivos = new System.Windows.Forms.DataGridView();
            this.colArchivo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTipo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colQuitar = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colRuta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.grpMaterialCargado = new System.Windows.Forms.GroupBox();
            this.dgvMaterialCargado = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInmuebles)).BeginInit();
            this.grpNuevoMaterial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArchivos)).BeginInit();
            this.grpMaterialCargado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterialCargado)).BeginInit();
            this.SuspendLayout();
            //
            // estiloEncabezado / estiloCelda (mismo estilo de grilla que FormGestionUsuarios)
            //
            estiloEncabezado.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            estiloEncabezado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            estiloEncabezado.ForeColor = System.Drawing.Color.White;
            estiloEncabezado.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            estiloEncabezado.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            estiloEncabezado.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            estiloCelda.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            estiloCelda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            estiloCelda.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            estiloCelda.ForeColor = System.Drawing.Color.White;
            estiloCelda.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(129)))), ((int)(((byte)(158)))));
            estiloCelda.SelectionForeColor = System.Drawing.Color.White;
            estiloCelda.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            //
            // lblInmuebles
            //
            this.lblInmuebles.BackColor = System.Drawing.Color.Transparent;
            this.lblInmuebles.ForeColor = System.Drawing.Color.White;
            this.lblInmuebles.Location = new System.Drawing.Point(12, 12);
            this.lblInmuebles.Name = "lblInmuebles";
            this.lblInmuebles.Size = new System.Drawing.Size(500, 24);
            this.lblInmuebles.TabIndex = 0;
            this.lblInmuebles.Text = "Inmuebles con tasación aprobada";
            this.lblInmuebles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnActualizar
            //
            this.btnActualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(848, 8);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 28);
            this.btnActualizar.TabIndex = 1;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // dgvInmuebles
            //
            this.dgvInmuebles.AllowUserToAddRows = false;
            this.dgvInmuebles.AllowUserToDeleteRows = false;
            this.dgvInmuebles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInmuebles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInmuebles.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.dgvInmuebles.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.dgvInmuebles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInmuebles.DefaultCellStyle = estiloCelda;
            this.dgvInmuebles.EnableHeadersVisualStyles = false;
            this.dgvInmuebles.GridColor = System.Drawing.Color.Black;
            this.dgvInmuebles.Location = new System.Drawing.Point(12, 42);
            this.dgvInmuebles.MultiSelect = false;
            this.dgvInmuebles.Name = "dgvInmuebles";
            this.dgvInmuebles.ReadOnly = true;
            this.dgvInmuebles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInmuebles.Size = new System.Drawing.Size(936, 170);
            this.dgvInmuebles.TabIndex = 2;
            this.dgvInmuebles.SelectionChanged += new System.EventHandler(this.dgvInmuebles_SelectionChanged);
            //
            // grpNuevoMaterial
            //
            this.grpNuevoMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpNuevoMaterial.BackColor = System.Drawing.Color.Transparent;
            this.grpNuevoMaterial.Controls.Add(this.lblTextoComercial);
            this.grpNuevoMaterial.Controls.Add(this.txtTextoComercial);
            this.grpNuevoMaterial.Controls.Add(this.lblArchivos);
            this.grpNuevoMaterial.Controls.Add(this.btnAgregarArchivo);
            this.grpNuevoMaterial.Controls.Add(this.dgvArchivos);
            this.grpNuevoMaterial.Controls.Add(this.btnGuardar);
            this.grpNuevoMaterial.ForeColor = System.Drawing.Color.White;
            this.grpNuevoMaterial.Location = new System.Drawing.Point(12, 222);
            this.grpNuevoMaterial.Name = "grpNuevoMaterial";
            this.grpNuevoMaterial.Size = new System.Drawing.Size(560, 386);
            this.grpNuevoMaterial.TabIndex = 3;
            this.grpNuevoMaterial.TabStop = false;
            this.grpNuevoMaterial.Text = "Nuevo material";
            //
            // lblTextoComercial
            //
            this.lblTextoComercial.Location = new System.Drawing.Point(10, 20);
            this.lblTextoComercial.Name = "lblTextoComercial";
            this.lblTextoComercial.Size = new System.Drawing.Size(300, 20);
            this.lblTextoComercial.TabIndex = 0;
            this.lblTextoComercial.Text = "Texto comercial:";
            this.lblTextoComercial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtTextoComercial
            //
            this.txtTextoComercial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtTextoComercial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTextoComercial.ForeColor = System.Drawing.Color.White;
            this.txtTextoComercial.Location = new System.Drawing.Point(10, 42);
            this.txtTextoComercial.MaxLength = 2000;
            this.txtTextoComercial.Multiline = true;
            this.txtTextoComercial.Name = "txtTextoComercial";
            this.txtTextoComercial.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTextoComercial.Size = new System.Drawing.Size(540, 90);
            this.txtTextoComercial.TabIndex = 1;
            //
            // lblArchivos
            //
            this.lblArchivos.Location = new System.Drawing.Point(10, 142);
            this.lblArchivos.Name = "lblArchivos";
            this.lblArchivos.Size = new System.Drawing.Size(300, 20);
            this.lblArchivos.TabIndex = 2;
            this.lblArchivos.Text = "Archivos:";
            this.lblArchivos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnAgregarArchivo
            //
            this.btnAgregarArchivo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnAgregarArchivo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAgregarArchivo.ForeColor = System.Drawing.Color.White;
            this.btnAgregarArchivo.Location = new System.Drawing.Point(410, 137);
            this.btnAgregarArchivo.Name = "btnAgregarArchivo";
            this.btnAgregarArchivo.Size = new System.Drawing.Size(140, 27);
            this.btnAgregarArchivo.TabIndex = 3;
            this.btnAgregarArchivo.Text = "Agregar archivo...";
            this.btnAgregarArchivo.UseVisualStyleBackColor = false;
            this.btnAgregarArchivo.Click += new System.EventHandler(this.btnAgregarArchivo_Click);
            //
            // dgvArchivos
            //
            this.dgvArchivos.AllowUserToAddRows = false;
            this.dgvArchivos.AllowUserToDeleteRows = false;
            this.dgvArchivos.AllowUserToResizeRows = false;
            this.dgvArchivos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.dgvArchivos.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.dgvArchivos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvArchivos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colArchivo,
            this.colTipo,
            this.colQuitar,
            this.colRuta});
            this.dgvArchivos.DefaultCellStyle = estiloCelda;
            this.dgvArchivos.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvArchivos.EnableHeadersVisualStyles = false;
            this.dgvArchivos.GridColor = System.Drawing.Color.Black;
            this.dgvArchivos.Location = new System.Drawing.Point(10, 170);
            this.dgvArchivos.MultiSelect = false;
            this.dgvArchivos.Name = "dgvArchivos";
            this.dgvArchivos.RowHeadersVisible = false;
            this.dgvArchivos.Size = new System.Drawing.Size(540, 162);
            this.dgvArchivos.TabIndex = 4;
            this.dgvArchivos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvArchivos_CellContentClick);
            //
            // colArchivo
            //
            this.colArchivo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colArchivo.HeaderText = "Archivo";
            this.colArchivo.Name = "colArchivo";
            this.colArchivo.ReadOnly = true;
            //
            // colTipo
            //
            this.colTipo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colTipo.HeaderText = "Tipo";
            this.colTipo.Name = "colTipo";
            this.colTipo.Width = 110;
            //
            // colQuitar
            //
            this.colQuitar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.colQuitar.HeaderText = "Quitar";
            this.colQuitar.Name = "colQuitar";
            this.colQuitar.Text = "X";
            this.colQuitar.UseColumnTextForButtonValue = true;
            this.colQuitar.Width = 55;
            //
            // colRuta
            //
            this.colRuta.HeaderText = "Ruta";
            this.colRuta.Name = "colRuta";
            this.colRuta.Visible = false;
            //
            // btnGuardar
            //
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(410, 342);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(140, 32);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // grpMaterialCargado
            //
            this.grpMaterialCargado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMaterialCargado.BackColor = System.Drawing.Color.Transparent;
            this.grpMaterialCargado.Controls.Add(this.dgvMaterialCargado);
            this.grpMaterialCargado.ForeColor = System.Drawing.Color.White;
            this.grpMaterialCargado.Location = new System.Drawing.Point(582, 222);
            this.grpMaterialCargado.Name = "grpMaterialCargado";
            this.grpMaterialCargado.Size = new System.Drawing.Size(366, 386);
            this.grpMaterialCargado.TabIndex = 4;
            this.grpMaterialCargado.TabStop = false;
            this.grpMaterialCargado.Text = "Material ya cargado";
            //
            // dgvMaterialCargado
            //
            this.dgvMaterialCargado.AllowUserToAddRows = false;
            this.dgvMaterialCargado.AllowUserToDeleteRows = false;
            this.dgvMaterialCargado.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaterialCargado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMaterialCargado.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.dgvMaterialCargado.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.dgvMaterialCargado.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMaterialCargado.DefaultCellStyle = estiloCelda;
            this.dgvMaterialCargado.EnableHeadersVisualStyles = false;
            this.dgvMaterialCargado.GridColor = System.Drawing.Color.Black;
            this.dgvMaterialCargado.Location = new System.Drawing.Point(10, 20);
            this.dgvMaterialCargado.MultiSelect = false;
            this.dgvMaterialCargado.Name = "dgvMaterialCargado";
            this.dgvMaterialCargado.ReadOnly = true;
            this.dgvMaterialCargado.RowHeadersVisible = false;
            this.dgvMaterialCargado.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaterialCargado.Size = new System.Drawing.Size(346, 356);
            this.dgvMaterialCargado.TabIndex = 0;
            this.dgvMaterialCargado.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMaterialCargado_CellDoubleClick);
            //
            // FormCargarMaterialAudiovisual
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(960, 620);
            this.Controls.Add(this.lblInmuebles);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.dgvInmuebles);
            this.Controls.Add(this.grpNuevoMaterial);
            this.Controls.Add(this.grpMaterialCargado);
            this.MinimumSize = new System.Drawing.Size(976, 659);
            this.Name = "FormCargarMaterialAudiovisual";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cargar Material Audiovisual";
            this.Load += new System.EventHandler(this.FormCargarMaterialAudiovisual_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInmuebles)).EndInit();
            this.grpNuevoMaterial.ResumeLayout(false);
            this.grpNuevoMaterial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvArchivos)).EndInit();
            this.grpMaterialCargado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterialCargado)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblInmuebles;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.DataGridView dgvInmuebles;
        private System.Windows.Forms.GroupBox grpNuevoMaterial;
        private System.Windows.Forms.Label lblTextoComercial;
        private System.Windows.Forms.TextBox txtTextoComercial;
        private System.Windows.Forms.Label lblArchivos;
        private System.Windows.Forms.Button btnAgregarArchivo;
        private System.Windows.Forms.DataGridView dgvArchivos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colArchivo;
        private System.Windows.Forms.DataGridViewComboBoxColumn colTipo;
        private System.Windows.Forms.DataGridViewButtonColumn colQuitar;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRuta;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.GroupBox grpMaterialCargado;
        private System.Windows.Forms.DataGridView dgvMaterialCargado;
    }
}
