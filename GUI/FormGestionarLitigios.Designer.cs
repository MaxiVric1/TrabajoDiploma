namespace TpIngSoft
{
    partial class FormGestionarLitigios
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
            this.lblActualizacion = new System.Windows.Forms.Label();
            this.dgvPagos = new System.Windows.Forms.DataGridView();
            this.grpInquilino = new System.Windows.Forms.GroupBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblNombreValor = new System.Windows.Forms.Label();
            this.lblApellido = new System.Windows.Forms.Label();
            this.lblApellidoValor = new System.Windows.Forms.Label();
            this.lblDni = new System.Windows.Forms.Label();
            this.lblDniValor = new System.Windows.Forms.Label();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.lblTelefonoValor = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblEmailValor = new System.Windows.Forms.Label();
            this.grpContrato = new System.Windows.Forms.GroupBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.lblDireccionValor = new System.Windows.Forms.Label();
            this.lblMontoMensual = new System.Windows.Forms.Label();
            this.lblMontoMensualValor = new System.Windows.Forms.Label();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.lblFechaFinValor = new System.Windows.Forms.Label();
            this.lblDiasLitigio = new System.Windows.Forms.Label();
            this.lblDiasLitigioValor = new System.Windows.Forms.Label();
            this.lblMontoAdeudado = new System.Windows.Forms.Label();
            this.lblMontoAdeudadoValor = new System.Windows.Forms.Label();
            this.lblDeudaTotal = new System.Windows.Forms.Label();
            this.lblDeudaTotalValor = new System.Windows.Forms.Label();
            this.btnResuelto = new System.Windows.Forms.Button();
            this.btnDesalojo = new System.Windows.Forms.Button();
            this.grpAcciones = new System.Windows.Forms.GroupBox();
            this.dgvAcciones = new System.Windows.Forms.DataGridView();
            this.lblFechaAccion = new System.Windows.Forms.Label();
            this.dtpFechaAccion = new System.Windows.Forms.DateTimePicker();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.btnIntimacion = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).BeginInit();
            this.grpInquilino.SuspendLayout();
            this.grpContrato.SuspendLayout();
            this.grpAcciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAcciones)).BeginInit();
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
            // lblActualizacion
            //
            this.lblActualizacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblActualizacion.AutoEllipsis = true;
            this.lblActualizacion.BackColor = System.Drawing.Color.Transparent;
            this.lblActualizacion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(195)))), ((int)(((byte)(220)))));
            this.lblActualizacion.Location = new System.Drawing.Point(12, 10);
            this.lblActualizacion.Name = "lblActualizacion";
            this.lblActualizacion.Size = new System.Drawing.Size(976, 20);
            this.lblActualizacion.TabIndex = 0;
            this.lblActualizacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dgvPagos
            //
            this.dgvPagos.AllowUserToAddRows = false;
            this.dgvPagos.AllowUserToDeleteRows = false;
            this.dgvPagos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPagos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPagos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.dgvPagos.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.dgvPagos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPagos.DefaultCellStyle = estiloCelda;
            this.dgvPagos.EnableHeadersVisualStyles = false;
            this.dgvPagos.GridColor = System.Drawing.Color.Black;
            this.dgvPagos.Location = new System.Drawing.Point(12, 36);
            this.dgvPagos.MultiSelect = false;
            this.dgvPagos.Name = "dgvPagos";
            this.dgvPagos.ReadOnly = true;
            this.dgvPagos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPagos.Size = new System.Drawing.Size(976, 232);
            this.dgvPagos.TabIndex = 1;
            this.dgvPagos.SelectionChanged += new System.EventHandler(this.dgvPagos_SelectionChanged);
            //
            // grpInquilino
            //
            this.grpInquilino.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpInquilino.BackColor = System.Drawing.Color.Transparent;
            this.grpInquilino.Controls.Add(this.lblNombre);
            this.grpInquilino.Controls.Add(this.lblNombreValor);
            this.grpInquilino.Controls.Add(this.lblApellido);
            this.grpInquilino.Controls.Add(this.lblApellidoValor);
            this.grpInquilino.Controls.Add(this.lblDni);
            this.grpInquilino.Controls.Add(this.lblDniValor);
            this.grpInquilino.Controls.Add(this.lblTelefono);
            this.grpInquilino.Controls.Add(this.lblTelefonoValor);
            this.grpInquilino.Controls.Add(this.lblEmail);
            this.grpInquilino.Controls.Add(this.lblEmailValor);
            this.grpInquilino.ForeColor = System.Drawing.Color.White;
            this.grpInquilino.Location = new System.Drawing.Point(12, 278);
            this.grpInquilino.Name = "grpInquilino";
            this.grpInquilino.Size = new System.Drawing.Size(320, 146);
            this.grpInquilino.TabIndex = 2;
            this.grpInquilino.TabStop = false;
            this.grpInquilino.Text = "Inquilino";
            //
            // lblNombre
            //
            this.lblNombre.Location = new System.Drawing.Point(6, 22);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(80, 20);
            this.lblNombre.TabIndex = 0;
            this.lblNombre.Text = "Nombre:";
            this.lblNombre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblNombreValor
            //
            this.lblNombreValor.AutoEllipsis = true;
            this.lblNombreValor.Location = new System.Drawing.Point(92, 22);
            this.lblNombreValor.Name = "lblNombreValor";
            this.lblNombreValor.Size = new System.Drawing.Size(220, 20);
            this.lblNombreValor.TabIndex = 1;
            this.lblNombreValor.Text = "-";
            this.lblNombreValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblApellido
            //
            this.lblApellido.Location = new System.Drawing.Point(6, 45);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(80, 20);
            this.lblApellido.TabIndex = 2;
            this.lblApellido.Text = "Apellido:";
            this.lblApellido.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblApellidoValor
            //
            this.lblApellidoValor.AutoEllipsis = true;
            this.lblApellidoValor.Location = new System.Drawing.Point(92, 45);
            this.lblApellidoValor.Name = "lblApellidoValor";
            this.lblApellidoValor.Size = new System.Drawing.Size(220, 20);
            this.lblApellidoValor.TabIndex = 3;
            this.lblApellidoValor.Text = "-";
            this.lblApellidoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDni
            //
            this.lblDni.Location = new System.Drawing.Point(6, 68);
            this.lblDni.Name = "lblDni";
            this.lblDni.Size = new System.Drawing.Size(80, 20);
            this.lblDni.TabIndex = 4;
            this.lblDni.Text = "DNI:";
            this.lblDni.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDniValor
            //
            this.lblDniValor.Location = new System.Drawing.Point(92, 68);
            this.lblDniValor.Name = "lblDniValor";
            this.lblDniValor.Size = new System.Drawing.Size(220, 20);
            this.lblDniValor.TabIndex = 5;
            this.lblDniValor.Text = "-";
            this.lblDniValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTelefono
            //
            this.lblTelefono.Location = new System.Drawing.Point(6, 91);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(80, 20);
            this.lblTelefono.TabIndex = 6;
            this.lblTelefono.Text = "Teléfono:";
            this.lblTelefono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTelefonoValor
            //
            this.lblTelefonoValor.Location = new System.Drawing.Point(92, 91);
            this.lblTelefonoValor.Name = "lblTelefonoValor";
            this.lblTelefonoValor.Size = new System.Drawing.Size(220, 20);
            this.lblTelefonoValor.TabIndex = 7;
            this.lblTelefonoValor.Text = "-";
            this.lblTelefonoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblEmail
            //
            this.lblEmail.Location = new System.Drawing.Point(6, 114);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(80, 20);
            this.lblEmail.TabIndex = 8;
            this.lblEmail.Text = "Email:";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblEmailValor
            //
            this.lblEmailValor.AutoEllipsis = true;
            this.lblEmailValor.Location = new System.Drawing.Point(92, 114);
            this.lblEmailValor.Name = "lblEmailValor";
            this.lblEmailValor.Size = new System.Drawing.Size(220, 20);
            this.lblEmailValor.TabIndex = 9;
            this.lblEmailValor.Text = "-";
            this.lblEmailValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpContrato
            //
            this.grpContrato.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpContrato.BackColor = System.Drawing.Color.Transparent;
            this.grpContrato.Controls.Add(this.lblDireccion);
            this.grpContrato.Controls.Add(this.lblDireccionValor);
            this.grpContrato.Controls.Add(this.lblMontoMensual);
            this.grpContrato.Controls.Add(this.lblMontoMensualValor);
            this.grpContrato.Controls.Add(this.lblFechaFin);
            this.grpContrato.Controls.Add(this.lblFechaFinValor);
            this.grpContrato.Controls.Add(this.lblDiasLitigio);
            this.grpContrato.Controls.Add(this.lblDiasLitigioValor);
            this.grpContrato.Controls.Add(this.lblMontoAdeudado);
            this.grpContrato.Controls.Add(this.lblMontoAdeudadoValor);
            this.grpContrato.Controls.Add(this.lblDeudaTotal);
            this.grpContrato.Controls.Add(this.lblDeudaTotalValor);
            this.grpContrato.ForeColor = System.Drawing.Color.White;
            this.grpContrato.Location = new System.Drawing.Point(12, 430);
            this.grpContrato.Name = "grpContrato";
            this.grpContrato.Size = new System.Drawing.Size(320, 182);
            this.grpContrato.TabIndex = 3;
            this.grpContrato.TabStop = false;
            this.grpContrato.Text = "Contrato y deuda";
            //
            // lblDireccion
            //
            this.lblDireccion.Location = new System.Drawing.Point(6, 22);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(100, 20);
            this.lblDireccion.TabIndex = 0;
            this.lblDireccion.Text = "Dirección:";
            this.lblDireccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDireccionValor
            //
            this.lblDireccionValor.AutoEllipsis = true;
            this.lblDireccionValor.Location = new System.Drawing.Point(112, 22);
            this.lblDireccionValor.Name = "lblDireccionValor";
            this.lblDireccionValor.Size = new System.Drawing.Size(200, 20);
            this.lblDireccionValor.TabIndex = 1;
            this.lblDireccionValor.Text = "-";
            this.lblDireccionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMontoMensual
            //
            this.lblMontoMensual.Location = new System.Drawing.Point(6, 45);
            this.lblMontoMensual.Name = "lblMontoMensual";
            this.lblMontoMensual.Size = new System.Drawing.Size(100, 20);
            this.lblMontoMensual.TabIndex = 2;
            this.lblMontoMensual.Text = "Monto mensual:";
            this.lblMontoMensual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMontoMensualValor
            //
            this.lblMontoMensualValor.Location = new System.Drawing.Point(112, 45);
            this.lblMontoMensualValor.Name = "lblMontoMensualValor";
            this.lblMontoMensualValor.Size = new System.Drawing.Size(200, 20);
            this.lblMontoMensualValor.TabIndex = 3;
            this.lblMontoMensualValor.Text = "-";
            this.lblMontoMensualValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblFechaFin
            //
            this.lblFechaFin.Location = new System.Drawing.Point(6, 68);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(100, 20);
            this.lblFechaFin.TabIndex = 4;
            this.lblFechaFin.Text = "Fin del contrato:";
            this.lblFechaFin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblFechaFinValor
            //
            this.lblFechaFinValor.Location = new System.Drawing.Point(112, 68);
            this.lblFechaFinValor.Name = "lblFechaFinValor";
            this.lblFechaFinValor.Size = new System.Drawing.Size(200, 20);
            this.lblFechaFinValor.TabIndex = 5;
            this.lblFechaFinValor.Text = "-";
            this.lblFechaFinValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDiasLitigio
            //
            this.lblDiasLitigio.Location = new System.Drawing.Point(6, 91);
            this.lblDiasLitigio.Name = "lblDiasLitigio";
            this.lblDiasLitigio.Size = new System.Drawing.Size(100, 20);
            this.lblDiasLitigio.TabIndex = 6;
            this.lblDiasLitigio.Text = "Días en litigio:";
            this.lblDiasLitigio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDiasLitigioValor
            //
            this.lblDiasLitigioValor.Location = new System.Drawing.Point(112, 91);
            this.lblDiasLitigioValor.Name = "lblDiasLitigioValor";
            this.lblDiasLitigioValor.Size = new System.Drawing.Size(200, 20);
            this.lblDiasLitigioValor.TabIndex = 7;
            this.lblDiasLitigioValor.Text = "-";
            this.lblDiasLitigioValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMontoAdeudado
            //
            this.lblMontoAdeudado.Location = new System.Drawing.Point(6, 114);
            this.lblMontoAdeudado.Name = "lblMontoAdeudado";
            this.lblMontoAdeudado.Size = new System.Drawing.Size(100, 20);
            this.lblMontoAdeudado.TabIndex = 8;
            this.lblMontoAdeudado.Text = "Con recargo:";
            this.lblMontoAdeudado.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMontoAdeudadoValor
            //
            this.lblMontoAdeudadoValor.AutoEllipsis = true;
            this.lblMontoAdeudadoValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMontoAdeudadoValor.Location = new System.Drawing.Point(112, 114);
            this.lblMontoAdeudadoValor.Name = "lblMontoAdeudadoValor";
            this.lblMontoAdeudadoValor.Size = new System.Drawing.Size(200, 20);
            this.lblMontoAdeudadoValor.TabIndex = 9;
            this.lblMontoAdeudadoValor.Text = "-";
            this.lblMontoAdeudadoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnDesalojo
            //
            this.btnDesalojo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(50)))), ((int)(((byte)(60)))));
            this.btnDesalojo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDesalojo.ForeColor = System.Drawing.Color.White;
            this.btnDesalojo.Location = new System.Drawing.Point(466, 294);
            this.btnDesalojo.Name = "btnDesalojo";
            this.btnDesalojo.Size = new System.Drawing.Size(170, 32);
            this.btnDesalojo.TabIndex = 7;
            this.btnDesalojo.Text = "Registrar Desalojo";
            this.btnDesalojo.UseVisualStyleBackColor = false;
            this.btnDesalojo.Click += new System.EventHandler(this.btnDesalojo_Click);
            //
            // lblDeudaTotal
            //
            this.lblDeudaTotal.Location = new System.Drawing.Point(6, 140);
            this.lblDeudaTotal.Name = "lblDeudaTotal";
            this.lblDeudaTotal.Size = new System.Drawing.Size(100, 32);
            this.lblDeudaTotal.TabIndex = 10;
            this.lblDeudaTotal.Text = "Deuda en litigio del contrato:";
            this.lblDeudaTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDeudaTotalValor
            //
            this.lblDeudaTotalValor.AutoEllipsis = true;
            this.lblDeudaTotalValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeudaTotalValor.Location = new System.Drawing.Point(112, 140);
            this.lblDeudaTotalValor.Name = "lblDeudaTotalValor";
            this.lblDeudaTotalValor.Size = new System.Drawing.Size(200, 32);
            this.lblDeudaTotalValor.TabIndex = 11;
            this.lblDeudaTotalValor.Text = "-";
            this.lblDeudaTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnResuelto
            //
            this.btnResuelto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnResuelto.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnResuelto.ForeColor = System.Drawing.Color.White;
            this.btnResuelto.Location = new System.Drawing.Point(286, 294);
            this.btnResuelto.Name = "btnResuelto";
            this.btnResuelto.Size = new System.Drawing.Size(170, 32);
            this.btnResuelto.TabIndex = 6;
            this.btnResuelto.Text = "Marcar Resuelto";
            this.btnResuelto.UseVisualStyleBackColor = false;
            this.btnResuelto.Click += new System.EventHandler(this.btnResuelto_Click);
            //
            // grpAcciones
            //
            this.grpAcciones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAcciones.BackColor = System.Drawing.Color.Transparent;
            this.grpAcciones.Controls.Add(this.dgvAcciones);
            this.grpAcciones.Controls.Add(this.lblFechaAccion);
            this.grpAcciones.Controls.Add(this.dtpFechaAccion);
            this.grpAcciones.Controls.Add(this.lblDescripcion);
            this.grpAcciones.Controls.Add(this.txtDescripcion);
            this.grpAcciones.Controls.Add(this.btnIntimacion);
            this.grpAcciones.Controls.Add(this.btnResuelto);
            this.grpAcciones.Controls.Add(this.btnDesalojo);
            this.grpAcciones.ForeColor = System.Drawing.Color.White;
            this.grpAcciones.Location = new System.Drawing.Point(342, 278);
            this.grpAcciones.Name = "grpAcciones";
            this.grpAcciones.Size = new System.Drawing.Size(646, 334);
            this.grpAcciones.TabIndex = 4;
            this.grpAcciones.TabStop = false;
            this.grpAcciones.Text = "Acciones legales";
            //
            // dgvAcciones
            //
            this.dgvAcciones.AllowUserToAddRows = false;
            this.dgvAcciones.AllowUserToDeleteRows = false;
            this.dgvAcciones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAcciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAcciones.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.dgvAcciones.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.dgvAcciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAcciones.DefaultCellStyle = estiloCelda;
            this.dgvAcciones.EnableHeadersVisualStyles = false;
            this.dgvAcciones.GridColor = System.Drawing.Color.Black;
            this.dgvAcciones.Location = new System.Drawing.Point(10, 20);
            this.dgvAcciones.MultiSelect = false;
            this.dgvAcciones.Name = "dgvAcciones";
            this.dgvAcciones.ReadOnly = true;
            this.dgvAcciones.RowHeadersVisible = false;
            this.dgvAcciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAcciones.Size = new System.Drawing.Size(626, 168);
            this.dgvAcciones.TabIndex = 0;
            //
            // lblFechaAccion
            //
            this.lblFechaAccion.Location = new System.Drawing.Point(10, 198);
            this.lblFechaAccion.Name = "lblFechaAccion";
            this.lblFechaAccion.Size = new System.Drawing.Size(84, 20);
            this.lblFechaAccion.TabIndex = 1;
            this.lblFechaAccion.Text = "Fecha:";
            this.lblFechaAccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dtpFechaAccion
            //
            this.dtpFechaAccion.CalendarForeColor = System.Drawing.Color.White;
            this.dtpFechaAccion.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.dtpFechaAccion.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.dtpFechaAccion.CalendarTitleForeColor = System.Drawing.Color.White;
            this.dtpFechaAccion.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaAccion.Location = new System.Drawing.Point(100, 198);
            this.dtpFechaAccion.Name = "dtpFechaAccion";
            this.dtpFechaAccion.Size = new System.Drawing.Size(150, 20);
            this.dtpFechaAccion.TabIndex = 2;
            //
            // lblDescripcion
            //
            this.lblDescripcion.Location = new System.Drawing.Point(10, 226);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(84, 20);
            this.lblDescripcion.TabIndex = 3;
            this.lblDescripcion.Text = "Descripción:";
            this.lblDescripcion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtDescripcion
            //
            this.txtDescripcion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescripcion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtDescripcion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescripcion.ForeColor = System.Drawing.Color.White;
            this.txtDescripcion.Location = new System.Drawing.Point(100, 226);
            this.txtDescripcion.MaxLength = 1000;
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescripcion.Size = new System.Drawing.Size(536, 62);
            this.txtDescripcion.TabIndex = 4;
            //
            // btnIntimacion
            //
            this.btnIntimacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnIntimacion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnIntimacion.ForeColor = System.Drawing.Color.White;
            this.btnIntimacion.Location = new System.Drawing.Point(106, 294);
            this.btnIntimacion.Name = "btnIntimacion";
            this.btnIntimacion.Size = new System.Drawing.Size(170, 32);
            this.btnIntimacion.TabIndex = 5;
            this.btnIntimacion.Text = "Registrar Intimación";
            this.btnIntimacion.UseVisualStyleBackColor = false;
            this.btnIntimacion.Click += new System.EventHandler(this.btnIntimacion_Click);
            //
            // FormGestionarLitigios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(1000, 624);
            this.Controls.Add(this.lblActualizacion);
            this.Controls.Add(this.dgvPagos);
            this.Controls.Add(this.grpInquilino);
            this.Controls.Add(this.grpContrato);
            this.Controls.Add(this.grpAcciones);
            this.MinimumSize = new System.Drawing.Size(1016, 663);
            this.Name = "FormGestionarLitigios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionar Litigios";
            this.Load += new System.EventHandler(this.FormGestionarLitigios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).EndInit();
            this.grpInquilino.ResumeLayout(false);
            this.grpContrato.ResumeLayout(false);
            this.grpAcciones.ResumeLayout(false);
            this.grpAcciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAcciones)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblActualizacion;
        private System.Windows.Forms.DataGridView dgvPagos;
        private System.Windows.Forms.GroupBox grpInquilino;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblNombreValor;
        private System.Windows.Forms.Label lblApellido;
        private System.Windows.Forms.Label lblApellidoValor;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.Label lblDniValor;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.Label lblTelefonoValor;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblEmailValor;
        private System.Windows.Forms.GroupBox grpContrato;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.Label lblDireccionValor;
        private System.Windows.Forms.Label lblMontoMensual;
        private System.Windows.Forms.Label lblMontoMensualValor;
        private System.Windows.Forms.Label lblFechaFin;
        private System.Windows.Forms.Label lblFechaFinValor;
        private System.Windows.Forms.Label lblDiasLitigio;
        private System.Windows.Forms.Label lblDiasLitigioValor;
        private System.Windows.Forms.Label lblMontoAdeudado;
        private System.Windows.Forms.Label lblMontoAdeudadoValor;
        private System.Windows.Forms.Label lblDeudaTotal;
        private System.Windows.Forms.Label lblDeudaTotalValor;
        private System.Windows.Forms.Button btnResuelto;
        private System.Windows.Forms.Button btnDesalojo;
        private System.Windows.Forms.GroupBox grpAcciones;
        private System.Windows.Forms.DataGridView dgvAcciones;
        private System.Windows.Forms.Label lblFechaAccion;
        private System.Windows.Forms.DateTimePicker dtpFechaAccion;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Button btnIntimacion;
    }
}
