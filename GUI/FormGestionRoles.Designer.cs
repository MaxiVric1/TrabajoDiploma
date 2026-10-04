namespace TpIngSoft
{
    partial class FormGestionRoles
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Nodo0");
            this.tvPerfiles = new System.Windows.Forms.TreeView();
            this.grpAcciones = new System.Windows.Forms.GroupBox();
            this.btnQuitarItem = new System.Windows.Forms.Button();
            this.grpAsignarPermiso = new System.Windows.Forms.GroupBox();
            this.btnAsignarPermiso = new System.Windows.Forms.Button();
            this.cmbPermisos = new System.Windows.Forms.ComboBox();
            this.btnEliminarRol = new System.Windows.Forms.Button();
            this.btnEditarNombre = new System.Windows.Forms.Button();
            this.btnCrearSubRol = new System.Windows.Forms.Button();
            this.btnCrearRolRaiz = new System.Windows.Forms.Button();
            this.txtNombreRol = new System.Windows.Forms.TextBox();
            this.lblNombreRol = new System.Windows.Forms.Label();
            this.lblDetalle = new System.Windows.Forms.Label();
            this.lblModo = new System.Windows.Forms.Label();
            this.rbModoCrear = new System.Windows.Forms.RadioButton();
            this.rbModoEditar = new System.Windows.Forms.RadioButton();
            this.grpAcciones.SuspendLayout();
            this.grpAsignarPermiso.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvPerfiles
            // 
            this.tvPerfiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvPerfiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.tvPerfiles.ForeColor = System.Drawing.Color.White;
            this.tvPerfiles.Location = new System.Drawing.Point(12, 12);
            this.tvPerfiles.Name = "tvPerfiles";
            treeNode1.Name = "Nodo0";
            treeNode1.Text = "Nodo0";
            this.tvPerfiles.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvPerfiles.Size = new System.Drawing.Size(350, 436);
            this.tvPerfiles.TabIndex = 0;
            this.tvPerfiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvPerfiles_AfterSelect);
            // 
            // grpAcciones
            // 
            this.grpAcciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAcciones.BackColor = System.Drawing.Color.Transparent;
            this.grpAcciones.Controls.Add(this.btnQuitarItem);
            this.grpAcciones.Controls.Add(this.grpAsignarPermiso);
            this.grpAcciones.Controls.Add(this.btnEliminarRol);
            this.grpAcciones.Controls.Add(this.btnEditarNombre);
            this.grpAcciones.Controls.Add(this.btnCrearSubRol);
            this.grpAcciones.Controls.Add(this.btnCrearRolRaiz);
            this.grpAcciones.Controls.Add(this.txtNombreRol);
            this.grpAcciones.Controls.Add(this.lblNombreRol);
            this.grpAcciones.Controls.Add(this.lblDetalle);
            this.grpAcciones.Controls.Add(this.lblModo);
            this.grpAcciones.Controls.Add(this.rbModoCrear);
            this.grpAcciones.Controls.Add(this.rbModoEditar);
            this.grpAcciones.ForeColor = System.Drawing.Color.White;
            this.grpAcciones.Location = new System.Drawing.Point(378, 12);
            this.grpAcciones.Name = "grpAcciones";
            this.grpAcciones.Size = new System.Drawing.Size(394, 436);
            this.grpAcciones.TabIndex = 1;
            this.grpAcciones.TabStop = false;
            this.grpAcciones.Text = "Acciones";
            // 
            // btnQuitarItem
            // 
            this.btnQuitarItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnQuitarItem.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnQuitarItem.ForeColor = System.Drawing.Color.White;
            this.btnQuitarItem.Location = new System.Drawing.Point(15, 295);
            this.btnQuitarItem.Name = "btnQuitarItem";
            this.btnQuitarItem.Size = new System.Drawing.Size(364, 30);
            this.btnQuitarItem.TabIndex = 11;
            this.btnQuitarItem.Text = "Quitar Ítem Seleccionado del Padre";
            this.btnQuitarItem.UseVisualStyleBackColor = false;
            this.btnQuitarItem.Click += new System.EventHandler(this.btnQuitarItem_Click);
            // 
            // grpAsignarPermiso
            // 
            this.grpAsignarPermiso.BackColor = System.Drawing.Color.Transparent;
            this.grpAsignarPermiso.Controls.Add(this.btnAsignarPermiso);
            this.grpAsignarPermiso.Controls.Add(this.cmbPermisos);
            this.grpAsignarPermiso.ForeColor = System.Drawing.Color.White;
            this.grpAsignarPermiso.Location = new System.Drawing.Point(15, 175);
            this.grpAsignarPermiso.Name = "grpAsignarPermiso";
            this.grpAsignarPermiso.Size = new System.Drawing.Size(364, 100);
            this.grpAsignarPermiso.TabIndex = 10;
            this.grpAsignarPermiso.TabStop = false;
            this.grpAsignarPermiso.Text = "Asignar Permiso";
            // 
            // btnAsignarPermiso
            // 
            this.btnAsignarPermiso.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnAsignarPermiso.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAsignarPermiso.ForeColor = System.Drawing.Color.White;
            this.btnAsignarPermiso.Location = new System.Drawing.Point(15, 60);
            this.btnAsignarPermiso.Name = "btnAsignarPermiso";
            this.btnAsignarPermiso.Size = new System.Drawing.Size(150, 23);
            this.btnAsignarPermiso.TabIndex = 1;
            this.btnAsignarPermiso.Text = "Asignar Permiso";
            this.btnAsignarPermiso.UseVisualStyleBackColor = false;
            this.btnAsignarPermiso.Click += new System.EventHandler(this.btnAsignarPermiso_Click);
            // 
            // cmbPermisos
            // 
            this.cmbPermisos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbPermisos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPermisos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbPermisos.ForeColor = System.Drawing.Color.White;
            this.cmbPermisos.FormattingEnabled = true;
            this.cmbPermisos.Location = new System.Drawing.Point(15, 30);
            this.cmbPermisos.Name = "cmbPermisos";
            this.cmbPermisos.Size = new System.Drawing.Size(334, 21);
            this.cmbPermisos.TabIndex = 0;
            // 
            // btnEliminarRol
            // 
            this.btnEliminarRol.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnEliminarRol.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEliminarRol.ForeColor = System.Drawing.Color.White;
            this.btnEliminarRol.Location = new System.Drawing.Point(204, 135);
            this.btnEliminarRol.Name = "btnEliminarRol";
            this.btnEliminarRol.Size = new System.Drawing.Size(175, 23);
            this.btnEliminarRol.TabIndex = 9;
            this.btnEliminarRol.Text = "Eliminar Rol";
            this.btnEliminarRol.UseVisualStyleBackColor = false;
            this.btnEliminarRol.Click += new System.EventHandler(this.btnEliminarRol_Click);
            // 
            // btnEditarNombre
            // 
            this.btnEditarNombre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnEditarNombre.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditarNombre.ForeColor = System.Drawing.Color.White;
            this.btnEditarNombre.Location = new System.Drawing.Point(15, 135);
            this.btnEditarNombre.Name = "btnEditarNombre";
            this.btnEditarNombre.Size = new System.Drawing.Size(175, 23);
            this.btnEditarNombre.TabIndex = 8;
            this.btnEditarNombre.Text = "Guardar Nombre";
            this.btnEditarNombre.UseVisualStyleBackColor = false;
            this.btnEditarNombre.Click += new System.EventHandler(this.btnEditarNombre_Click);
            // 
            // btnCrearSubRol
            // 
            this.btnCrearSubRol.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCrearSubRol.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCrearSubRol.ForeColor = System.Drawing.Color.White;
            this.btnCrearSubRol.Location = new System.Drawing.Point(204, 135);
            this.btnCrearSubRol.Name = "btnCrearSubRol";
            this.btnCrearSubRol.Size = new System.Drawing.Size(175, 23);
            this.btnCrearSubRol.TabIndex = 7;
            this.btnCrearSubRol.Text = "Crear Sub-Rol";
            this.btnCrearSubRol.UseVisualStyleBackColor = false;
            this.btnCrearSubRol.Click += new System.EventHandler(this.btnCrearSubRol_Click);
            // 
            // btnCrearRolRaiz
            // 
            this.btnCrearRolRaiz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnCrearRolRaiz.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCrearRolRaiz.ForeColor = System.Drawing.Color.White;
            this.btnCrearRolRaiz.Location = new System.Drawing.Point(15, 135);
            this.btnCrearRolRaiz.Name = "btnCrearRolRaiz";
            this.btnCrearRolRaiz.Size = new System.Drawing.Size(175, 23);
            this.btnCrearRolRaiz.TabIndex = 6;
            this.btnCrearRolRaiz.Text = "Crear Rol Raíz";
            this.btnCrearRolRaiz.UseVisualStyleBackColor = false;
            this.btnCrearRolRaiz.Click += new System.EventHandler(this.btnCrearRolRaiz_Click);
            // 
            // txtNombreRol
            // 
            this.txtNombreRol.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtNombreRol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombreRol.ForeColor = System.Drawing.Color.White;
            this.txtNombreRol.Location = new System.Drawing.Point(15, 105);
            this.txtNombreRol.Name = "txtNombreRol";
            this.txtNombreRol.Size = new System.Drawing.Size(364, 20);
            this.txtNombreRol.TabIndex = 5;
            // 
            // lblNombreRol
            // 
            this.lblNombreRol.BackColor = System.Drawing.Color.Transparent;
            this.lblNombreRol.ForeColor = System.Drawing.Color.White;
            this.lblNombreRol.Location = new System.Drawing.Point(15, 85);
            this.lblNombreRol.Name = "lblNombreRol";
            this.lblNombreRol.Size = new System.Drawing.Size(364, 17);
            this.lblNombreRol.TabIndex = 4;
            this.lblNombreRol.Text = "Nombre Rol:";
            this.lblNombreRol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDetalle
            // 
            this.lblDetalle.BackColor = System.Drawing.Color.Transparent;
            this.lblDetalle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetalle.ForeColor = System.Drawing.Color.White;
            this.lblDetalle.Location = new System.Drawing.Point(15, 20);
            this.lblDetalle.Name = "lblDetalle";
            this.lblDetalle.Size = new System.Drawing.Size(364, 34);
            this.lblDetalle.TabIndex = 0;
            this.lblDetalle.Text = "Detalle: Selección vacía";
            // 
            // lblModo
            // 
            this.lblModo.BackColor = System.Drawing.Color.Transparent;
            this.lblModo.ForeColor = System.Drawing.Color.White;
            this.lblModo.Location = new System.Drawing.Point(15, 64);
            this.lblModo.Name = "lblModo";
            this.lblModo.Size = new System.Drawing.Size(90, 15);
            this.lblModo.TabIndex = 1;
            this.lblModo.Text = "Modo:";
            // 
            // rbModoCrear
            // 
            this.rbModoCrear.AutoSize = true;
            this.rbModoCrear.BackColor = System.Drawing.Color.Transparent;
            this.rbModoCrear.Checked = true;
            this.rbModoCrear.ForeColor = System.Drawing.Color.White;
            this.rbModoCrear.Location = new System.Drawing.Point(111, 62);
            this.rbModoCrear.Name = "rbModoCrear";
            this.rbModoCrear.Size = new System.Drawing.Size(69, 17);
            this.rbModoCrear.TabIndex = 2;
            this.rbModoCrear.TabStop = true;
            this.rbModoCrear.Text = "Crear Rol";
            this.rbModoCrear.UseVisualStyleBackColor = true;
            this.rbModoCrear.CheckedChanged += new System.EventHandler(this.rbModo_CheckedChanged);
            // 
            // rbModoEditar
            // 
            this.rbModoEditar.AutoSize = true;
            this.rbModoEditar.BackColor = System.Drawing.Color.Transparent;
            this.rbModoEditar.ForeColor = System.Drawing.Color.White;
            this.rbModoEditar.Location = new System.Drawing.Point(237, 61);
            this.rbModoEditar.Name = "rbModoEditar";
            this.rbModoEditar.Size = new System.Drawing.Size(99, 17);
            this.rbModoEditar.TabIndex = 3;
            this.rbModoEditar.Text = "Editar / Eliminar";
            this.rbModoEditar.UseVisualStyleBackColor = true;
            this.rbModoEditar.CheckedChanged += new System.EventHandler(this.rbModo_CheckedChanged);
            // 
            // FormGestionRoles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.grpAcciones);
            this.Controls.Add(this.tvPerfiles);
            this.Name = "FormGestionRoles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gestión de Roles";
            this.Load += new System.EventHandler(this.FormGestionRoles_Load);
            this.grpAcciones.ResumeLayout(false);
            this.grpAcciones.PerformLayout();
            this.grpAsignarPermiso.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvPerfiles;
        private System.Windows.Forms.GroupBox grpAcciones;
        private System.Windows.Forms.Label lblDetalle;
        private System.Windows.Forms.Label lblNombreRol;
        private System.Windows.Forms.TextBox txtNombreRol;
        private System.Windows.Forms.Button btnCrearRolRaiz;
        private System.Windows.Forms.Button btnCrearSubRol;
        private System.Windows.Forms.Button btnEditarNombre;
        private System.Windows.Forms.Button btnEliminarRol;
        private System.Windows.Forms.GroupBox grpAsignarPermiso;
        private System.Windows.Forms.ComboBox cmbPermisos;
        private System.Windows.Forms.Button btnAsignarPermiso;
        private System.Windows.Forms.Button btnQuitarItem;
        private System.Windows.Forms.Label lblModo;
        private System.Windows.Forms.RadioButton rbModoCrear;
        private System.Windows.Forms.RadioButton rbModoEditar;
    }
}
