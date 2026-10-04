namespace TpIngSoft
{
    partial class FormInmueblesPublicados
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
            this.lblPublicados = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.dgvInmuebles = new System.Windows.Forms.DataGridView();
            this.grpDetalle = new System.Windows.Forms.GroupBox();
            this.lblTextoComercial = new System.Windows.Forms.Label();
            this.txtTextoComercial = new System.Windows.Forms.TextBox();
            this.lblMaterial = new System.Windows.Forms.Label();
            this.dgvMaterial = new System.Windows.Forms.DataGridView();
            this.lblVistaPrevia = new System.Windows.Forms.Label();
            this.lblSinVistaPrevia = new System.Windows.Forms.Label();
            this.picVistaPrevia = new System.Windows.Forms.PictureBox();
            this.btnDespublicar = new System.Windows.Forms.Button();
            this.btnRepublicar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInmuebles)).BeginInit();
            this.grpDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVistaPrevia)).BeginInit();
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
            // lblPublicados
            //
            this.lblPublicados.BackColor = System.Drawing.Color.Transparent;
            this.lblPublicados.ForeColor = System.Drawing.Color.White;
            this.lblPublicados.Location = new System.Drawing.Point(12, 12);
            this.lblPublicados.Name = "lblPublicados";
            this.lblPublicados.Size = new System.Drawing.Size(500, 24);
            this.lblPublicados.TabIndex = 0;
            this.lblPublicados.Text = "Inmuebles publicados";
            this.lblPublicados.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.dgvInmuebles.Size = new System.Drawing.Size(936, 190);
            this.dgvInmuebles.TabIndex = 2;
            this.dgvInmuebles.SelectionChanged += new System.EventHandler(this.dgvInmuebles_SelectionChanged);
            //
            // grpDetalle
            //
            this.grpDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetalle.BackColor = System.Drawing.Color.Transparent;
            this.grpDetalle.Controls.Add(this.lblTextoComercial);
            this.grpDetalle.Controls.Add(this.txtTextoComercial);
            this.grpDetalle.Controls.Add(this.lblMaterial);
            this.grpDetalle.Controls.Add(this.dgvMaterial);
            this.grpDetalle.Controls.Add(this.lblVistaPrevia);
            this.grpDetalle.Controls.Add(this.lblSinVistaPrevia);
            this.grpDetalle.Controls.Add(this.picVistaPrevia);
            this.grpDetalle.ForeColor = System.Drawing.Color.White;
            this.grpDetalle.Location = new System.Drawing.Point(12, 242);
            this.grpDetalle.Name = "grpDetalle";
            this.grpDetalle.Size = new System.Drawing.Size(936, 320);
            this.grpDetalle.TabIndex = 3;
            this.grpDetalle.TabStop = false;
            this.grpDetalle.Text = "Detalle del inmueble";
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
            this.txtTextoComercial.Multiline = true;
            this.txtTextoComercial.Name = "txtTextoComercial";
            this.txtTextoComercial.ReadOnly = true;
            this.txtTextoComercial.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTextoComercial.Size = new System.Drawing.Size(430, 110);
            this.txtTextoComercial.TabIndex = 1;
            //
            // lblMaterial
            //
            this.lblMaterial.Location = new System.Drawing.Point(10, 160);
            this.lblMaterial.Name = "lblMaterial";
            this.lblMaterial.Size = new System.Drawing.Size(300, 20);
            this.lblMaterial.TabIndex = 2;
            this.lblMaterial.Text = "Material cargado:";
            this.lblMaterial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dgvMaterial
            //
            this.dgvMaterial.AllowUserToAddRows = false;
            this.dgvMaterial.AllowUserToDeleteRows = false;
            this.dgvMaterial.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaterial.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.dgvMaterial.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.dgvMaterial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMaterial.DefaultCellStyle = estiloCelda;
            this.dgvMaterial.EnableHeadersVisualStyles = false;
            this.dgvMaterial.GridColor = System.Drawing.Color.Black;
            this.dgvMaterial.Location = new System.Drawing.Point(10, 182);
            this.dgvMaterial.MultiSelect = false;
            this.dgvMaterial.Name = "dgvMaterial";
            this.dgvMaterial.ReadOnly = true;
            this.dgvMaterial.RowHeadersVisible = false;
            this.dgvMaterial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaterial.Size = new System.Drawing.Size(430, 128);
            this.dgvMaterial.TabIndex = 3;
            this.dgvMaterial.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMaterial_CellDoubleClick);
            //
            // lblVistaPrevia
            //
            this.lblVistaPrevia.Location = new System.Drawing.Point(452, 20);
            this.lblVistaPrevia.Name = "lblVistaPrevia";
            this.lblVistaPrevia.Size = new System.Drawing.Size(300, 20);
            this.lblVistaPrevia.TabIndex = 4;
            this.lblVistaPrevia.Text = "Vista previa:";
            this.lblVistaPrevia.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSinVistaPrevia
            //
            this.lblSinVistaPrevia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSinVistaPrevia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.lblSinVistaPrevia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(129)))), ((int)(((byte)(158)))));
            this.lblSinVistaPrevia.Location = new System.Drawing.Point(453, 43);
            this.lblSinVistaPrevia.Name = "lblSinVistaPrevia";
            this.lblSinVistaPrevia.Size = new System.Drawing.Size(472, 266);
            this.lblSinVistaPrevia.TabIndex = 5;
            this.lblSinVistaPrevia.Text = "Vista previa no disponible";
            this.lblSinVistaPrevia.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // picVistaPrevia
            //
            this.picVistaPrevia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picVistaPrevia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.picVistaPrevia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picVistaPrevia.Location = new System.Drawing.Point(452, 42);
            this.picVistaPrevia.Name = "picVistaPrevia";
            this.picVistaPrevia.Size = new System.Drawing.Size(474, 268);
            this.picVistaPrevia.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picVistaPrevia.TabIndex = 6;
            this.picVistaPrevia.TabStop = false;
            //
            // btnDespublicar
            //
            this.btnDespublicar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDespublicar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnDespublicar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDespublicar.ForeColor = System.Drawing.Color.White;
            this.btnDespublicar.Location = new System.Drawing.Point(808, 572);
            this.btnDespublicar.Name = "btnDespublicar";
            this.btnDespublicar.Size = new System.Drawing.Size(140, 36);
            this.btnDespublicar.TabIndex = 4;
            this.btnDespublicar.Text = "Despublicar";
            this.btnDespublicar.UseVisualStyleBackColor = false;
            this.btnDespublicar.Click += new System.EventHandler(this.btnDespublicar_Click);
            //
            // btnRepublicar (mismo lugar que btnDespublicar: se muestra uno u otro según el inmueble)
            //
            this.btnRepublicar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRepublicar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRepublicar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRepublicar.ForeColor = System.Drawing.Color.White;
            this.btnRepublicar.Location = new System.Drawing.Point(808, 572);
            this.btnRepublicar.Name = "btnRepublicar";
            this.btnRepublicar.Size = new System.Drawing.Size(140, 36);
            this.btnRepublicar.TabIndex = 5;
            this.btnRepublicar.Text = "Republicar";
            this.btnRepublicar.UseVisualStyleBackColor = false;
            this.btnRepublicar.Visible = false;
            this.btnRepublicar.Click += new System.EventHandler(this.btnRepublicar_Click);
            //
            // FormInmueblesPublicados
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(960, 620);
            this.Controls.Add(this.lblPublicados);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.dgvInmuebles);
            this.Controls.Add(this.grpDetalle);
            this.Controls.Add(this.btnDespublicar);
            this.Controls.Add(this.btnRepublicar);
            this.MinimumSize = new System.Drawing.Size(976, 659);
            this.Name = "FormInmueblesPublicados";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inmuebles Publicados";
            this.Load += new System.EventHandler(this.FormInmueblesPublicados_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInmuebles)).EndInit();
            this.grpDetalle.ResumeLayout(false);
            this.grpDetalle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picVistaPrevia)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblPublicados;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.DataGridView dgvInmuebles;
        private System.Windows.Forms.GroupBox grpDetalle;
        private System.Windows.Forms.Label lblTextoComercial;
        private System.Windows.Forms.TextBox txtTextoComercial;
        private System.Windows.Forms.Label lblMaterial;
        private System.Windows.Forms.DataGridView dgvMaterial;
        private System.Windows.Forms.Label lblVistaPrevia;
        private System.Windows.Forms.Label lblSinVistaPrevia;
        private System.Windows.Forms.PictureBox picVistaPrevia;
        private System.Windows.Forms.Button btnDespublicar;
        private System.Windows.Forms.Button btnRepublicar;
    }
}
