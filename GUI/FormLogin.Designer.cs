namespace TpIngSoft
{
    partial class FormLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxNombre = new System.Windows.Forms.TextBox();
            this.textBoxPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblIdiomaLogin = new System.Windows.Forms.Label();
            this.cmbIdiomaLogin = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            //
            // pictureBoxLogo
            //
            this.pictureBoxLogo.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLogo.Location = new System.Drawing.Point(148, 10);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(112, 112);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 9;
            this.pictureBoxLogo.TabStop = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(408, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bienvenido a LandSmart";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxNombre
            // 
            this.textBoxNombre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.textBoxNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxNombre.ForeColor = System.Drawing.Color.White;
            this.textBoxNombre.Location = new System.Drawing.Point(18, 235);
            this.textBoxNombre.Name = "textBoxNombre";
            this.textBoxNombre.Size = new System.Drawing.Size(205, 20);
            this.textBoxNombre.TabIndex = 0;
            // 
            // textBoxPass
            // 
            this.textBoxPass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.textBoxPass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxPass.ForeColor = System.Drawing.Color.White;
            this.textBoxPass.Location = new System.Drawing.Point(18, 297);
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.PasswordChar = '*';
            this.textBoxPass.Size = new System.Drawing.Size(205, 20);
            this.textBoxPass.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(15, 210);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(205, 22);
            this.label2.TabIndex = 3;
            this.label2.Text = "Nombre";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(15, 269);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(205, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Contraseña";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // buttonLogin
            // 
            this.buttonLogin.BackColor = System.Drawing.Color.White;
            this.buttonLogin.FlatAppearance.BorderSize = 0;
            this.buttonLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.buttonLogin.Location = new System.Drawing.Point(253, 284);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(128, 33);
            this.buttonLogin.TabIndex = 5;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = false;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(91)))), ((int)(((byte)(91)))));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(15, 169);
            this.label4.Margin = new System.Windows.Forms.Padding(50, 1, 50, 1);
            this.label4.MinimumSize = new System.Drawing.Size(137, 23);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(2);
            this.label4.Size = new System.Drawing.Size(137, 23);
            this.label4.TabIndex = 6;
            this.label4.Text = "Login incorrecto";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Visible = false;
            // 
            // lblIdiomaLogin
            // 
            this.lblIdiomaLogin.BackColor = System.Drawing.Color.Transparent;
            this.lblIdiomaLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdiomaLogin.ForeColor = System.Drawing.Color.White;
            this.lblIdiomaLogin.Location = new System.Drawing.Point(15, 380);
            this.lblIdiomaLogin.Name = "lblIdiomaLogin";
            this.lblIdiomaLogin.Size = new System.Drawing.Size(149, 21);
            this.lblIdiomaLogin.TabIndex = 7;
            this.lblIdiomaLogin.Text = "Idioma:";
            this.lblIdiomaLogin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cmbIdiomaLogin
            // 
            this.cmbIdiomaLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.cmbIdiomaLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbIdiomaLogin.ForeColor = System.Drawing.Color.White;
            this.cmbIdiomaLogin.Location = new System.Drawing.Point(18, 404);
            this.cmbIdiomaLogin.Name = "cmbIdiomaLogin";
            this.cmbIdiomaLogin.Size = new System.Drawing.Size(134, 21);
            this.cmbIdiomaLogin.TabIndex = 8;
            this.cmbIdiomaLogin.SelectedIndexChanged += new System.EventHandler(this.CmbIdiomaLogin_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(89, 414);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 6;
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.Visible = false;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_1024x600;
            this.ClientSize = new System.Drawing.Size(408, 437);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.lblIdiomaLogin);
            this.Controls.Add(this.cmbIdiomaLogin);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPass);
            this.Controls.Add(this.textBoxNombre);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLogin";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxPass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblIdiomaLogin;
        private System.Windows.Forms.ComboBox cmbIdiomaLogin;
        private System.Windows.Forms.TextBox textBoxNombre;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
    }
}