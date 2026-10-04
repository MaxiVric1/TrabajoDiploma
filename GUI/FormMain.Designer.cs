namespace TpIngSoft
{
    partial class FormMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionUsuariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bitacoraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlCambiosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionPerfilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionIdiomasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verificarIntegridadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pn1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registrarCaptacionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionarCaptacionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.validarTasacionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cargarMaterialAudiovisualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publicarInmuebleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbIdioma = new System.Windows.Forms.ToolStripComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblSesionInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem,
            this.adminToolStripMenuItem,
            this.pn1ToolStripMenuItem,
            this.cmbIdioma});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(800, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logoutToolStripMenuItem,
            this.salirToolStripMenuItem});
            this.archivoToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(60, 23);
            this.archivoToolStripMenuItem.Text = "Archivo";
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.logoutToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.logoutToolStripMenuItem.Text = "Cerrar Sesión";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.salirToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // adminToolStripMenuItem
            // 
            this.adminToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.adminToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gestionUsuariosToolStripMenuItem,
            this.bitacoraToolStripMenuItem,
            this.controlCambiosToolStripMenuItem,
            this.gestionPerfilesToolStripMenuItem,
            this.gestionIdiomasToolStripMenuItem,
            this.verificarIntegridadToolStripMenuItem});
            this.adminToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            this.adminToolStripMenuItem.Size = new System.Drawing.Size(55, 23);
            this.adminToolStripMenuItem.Text = "Admin";
            // 
            // gestionUsuariosToolStripMenuItem
            // 
            this.gestionUsuariosToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.gestionUsuariosToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.gestionUsuariosToolStripMenuItem.Name = "gestionUsuariosToolStripMenuItem";
            this.gestionUsuariosToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.gestionUsuariosToolStripMenuItem.Text = "Gestión de Usuarios";
            this.gestionUsuariosToolStripMenuItem.Click += new System.EventHandler(this.gestionUsuariosToolStripMenuItem_Click);
            // 
            // bitacoraToolStripMenuItem
            // 
            this.bitacoraToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.bitacoraToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.bitacoraToolStripMenuItem.Name = "bitacoraToolStripMenuItem";
            this.bitacoraToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.bitacoraToolStripMenuItem.Text = "Bitácora";
            this.bitacoraToolStripMenuItem.Click += new System.EventHandler(this.bitacoraToolStripMenuItem_Click);
            // 
            // controlCambiosToolStripMenuItem
            // 
            this.controlCambiosToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.controlCambiosToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.controlCambiosToolStripMenuItem.Name = "controlCambiosToolStripMenuItem";
            this.controlCambiosToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.controlCambiosToolStripMenuItem.Text = "Control de Cambios";
            this.controlCambiosToolStripMenuItem.Click += new System.EventHandler(this.controlCambiosToolStripMenuItem_Click);
            // 
            // gestionPerfilesToolStripMenuItem
            // 
            this.gestionPerfilesToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.gestionPerfilesToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.gestionPerfilesToolStripMenuItem.Name = "gestionPerfilesToolStripMenuItem";
            this.gestionPerfilesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.gestionPerfilesToolStripMenuItem.Text = "Gestión de Roles";
            this.gestionPerfilesToolStripMenuItem.Click += new System.EventHandler(this.gestionPerfilesToolStripMenuItem_Click);
            // 
            // gestionIdiomasToolStripMenuItem
            // 
            this.gestionIdiomasToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.gestionIdiomasToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.gestionIdiomasToolStripMenuItem.Name = "gestionIdiomasToolStripMenuItem";
            this.gestionIdiomasToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.gestionIdiomasToolStripMenuItem.Text = "Gestión de Idiomas";
            this.gestionIdiomasToolStripMenuItem.Click += new System.EventHandler(this.GestionIdiomasToolStripMenuItem_Click);
            // 
            // verificarIntegridadToolStripMenuItem
            // 
            this.verificarIntegridadToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.verificarIntegridadToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.verificarIntegridadToolStripMenuItem.Name = "verificarIntegridadToolStripMenuItem";
            this.verificarIntegridadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.verificarIntegridadToolStripMenuItem.Text = "Verificar Integridad de Datos";
            this.verificarIntegridadToolStripMenuItem.Click += new System.EventHandler(this.verificarIntegridadToolStripMenuItem_Click);
            //
            // pn1ToolStripMenuItem
            //
            this.pn1ToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.pn1ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registrarCaptacionToolStripMenuItem,
            this.gestionarCaptacionesToolStripMenuItem,
            this.validarTasacionToolStripMenuItem,
            this.cargarMaterialAudiovisualToolStripMenuItem,
            this.publicarInmuebleToolStripMenuItem});
            this.pn1ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.pn1ToolStripMenuItem.Name = "pn1ToolStripMenuItem";
            this.pn1ToolStripMenuItem.Size = new System.Drawing.Size(40, 23);
            this.pn1ToolStripMenuItem.Text = "PN1";
            //
            // registrarCaptacionToolStripMenuItem
            //
            this.registrarCaptacionToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.registrarCaptacionToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.registrarCaptacionToolStripMenuItem.Name = "registrarCaptacionToolStripMenuItem";
            this.registrarCaptacionToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.registrarCaptacionToolStripMenuItem.Text = "Registrar Captación";
            this.registrarCaptacionToolStripMenuItem.Click += new System.EventHandler(this.registrarCaptacionToolStripMenuItem_Click);
            //
            // gestionarCaptacionesToolStripMenuItem
            //
            this.gestionarCaptacionesToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.gestionarCaptacionesToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.gestionarCaptacionesToolStripMenuItem.Name = "gestionarCaptacionesToolStripMenuItem";
            this.gestionarCaptacionesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.gestionarCaptacionesToolStripMenuItem.Text = "Gestionar Captaciones";
            this.gestionarCaptacionesToolStripMenuItem.Click += new System.EventHandler(this.gestionarCaptacionesToolStripMenuItem_Click);
            //
            // validarTasacionToolStripMenuItem
            //
            this.validarTasacionToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.validarTasacionToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.validarTasacionToolStripMenuItem.Name = "validarTasacionToolStripMenuItem";
            this.validarTasacionToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.validarTasacionToolStripMenuItem.Text = "Validar Tasación";
            this.validarTasacionToolStripMenuItem.Click += new System.EventHandler(this.validarTasacionToolStripMenuItem_Click);
            //
            // cargarMaterialAudiovisualToolStripMenuItem
            //
            this.cargarMaterialAudiovisualToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.cargarMaterialAudiovisualToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.cargarMaterialAudiovisualToolStripMenuItem.Name = "cargarMaterialAudiovisualToolStripMenuItem";
            this.cargarMaterialAudiovisualToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.cargarMaterialAudiovisualToolStripMenuItem.Text = "Cargar Material Audiovisual";
            this.cargarMaterialAudiovisualToolStripMenuItem.Click += new System.EventHandler(this.cargarMaterialAudiovisualToolStripMenuItem_Click);
            //
            // publicarInmuebleToolStripMenuItem
            //
            this.publicarInmuebleToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.publicarInmuebleToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.publicarInmuebleToolStripMenuItem.Name = "publicarInmuebleToolStripMenuItem";
            this.publicarInmuebleToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.publicarInmuebleToolStripMenuItem.Text = "Publicar Inmueble";
            this.publicarInmuebleToolStripMenuItem.Click += new System.EventHandler(this.publicarInmuebleToolStripMenuItem_Click);
            //
            // cmbIdioma
            // 
            this.cmbIdioma.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.cmbIdioma.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.cmbIdioma.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdioma.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbIdioma.ForeColor = System.Drawing.Color.White;
            this.cmbIdioma.Name = "cmbIdioma";
            this.cmbIdioma.Size = new System.Drawing.Size(120, 23);
            this.cmbIdioma.SelectedIndexChanged += new System.EventHandler(this.CmbIdioma_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.statusStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblSesionInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblSesionInfo
            // 
            this.lblSesionInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(93)))), ((int)(((byte)(115)))));
            this.lblSesionInfo.ForeColor = System.Drawing.Color.White;
            this.lblSesionInfo.Name = "lblSesionInfo";
            this.lblSesionInfo.Size = new System.Drawing.Size(118, 17);
            this.lblSesionInfo.Text = "toolStripStatusLabel1";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adminToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionUsuariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bitacoraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlCambiosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionPerfilesToolStripMenuItem;

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblSesionInfo;
        private System.Windows.Forms.ToolStripComboBox cmbIdioma;
        private System.Windows.Forms.ToolStripMenuItem gestionIdiomasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verificarIntegridadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pn1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registrarCaptacionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionarCaptacionesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem validarTasacionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cargarMaterialAudiovisualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem publicarInmuebleToolStripMenuItem;
    }
}
