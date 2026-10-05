namespace TpIngSoft
{
    partial class FormContratosPorVencer
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
            this.btnActualizar = new System.Windows.Forms.Button();
            this.lblActualizacion = new System.Windows.Forms.Label();
            this.dgvContratos = new System.Windows.Forms.DataGridView();
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
            this.grpAcciones = new System.Windows.Forms.GroupBox();
            this.lblObservacion = new System.Windows.Forms.Label();
            this.txtObservacion = new System.Windows.Forms.TextBox();
            this.btnRenovacion = new System.Windows.Forms.Button();
            this.btnDesocupacion = new System.Windows.Forms.Button();
            this.grpContrato = new System.Windows.Forms.GroupBox();
            this.lblNroContrato = new System.Windows.Forms.Label();
            this.lblNroContratoValor = new System.Windows.Forms.Label();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.lblDireccionValor = new System.Windows.Forms.Label();
            this.lblTipo = new System.Windows.Forms.Label();
            this.lblTipoValor = new System.Windows.Forms.Label();
            this.lblFechaInicio = new System.Windows.Forms.Label();
            this.lblFechaInicioValor = new System.Windows.Forms.Label();
            this.lblDuracion = new System.Windows.Forms.Label();
            this.lblDuracionValor = new System.Windows.Forms.Label();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.lblFechaFinValor = new System.Windows.Forms.Label();
            this.lblDiasRestantes = new System.Windows.Forms.Label();
            this.lblDiasRestantesValor = new System.Windows.Forms.Label();
            this.lblMonto = new System.Windows.Forms.Label();
            this.lblMontoValor = new System.Windows.Forms.Label();
            this.lblEstado = new System.Windows.Forms.Label();
            this.lblEstadoValor = new System.Windows.Forms.Label();
            this.lblAlerta = new System.Windows.Forms.Label();
            this.lblAlertaValor = new System.Windows.Forms.Label();
            this.lblPagoActual = new System.Windows.Forms.Label();
            this.lblPagoActualValor = new System.Windows.Forms.Label();
            this.lblDeuda = new System.Windows.Forms.Label();
            this.lblDeudaValor = new System.Windows.Forms.Label();
            this.lblDesocupacion = new System.Windows.Forms.Label();
            this.lblDesocupacionValor = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContratos)).BeginInit();
            this.grpInquilino.SuspendLayout();
            this.grpAcciones.SuspendLayout();
            this.grpContrato.SuspendLayout();
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
            // btnActualizar
            //
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(12, 10);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 28);
            this.btnActualizar.TabIndex = 0;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // lblActualizacion
            //
            this.lblActualizacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblActualizacion.AutoEllipsis = true;
            this.lblActualizacion.BackColor = System.Drawing.Color.Transparent;
            this.lblActualizacion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(195)))), ((int)(((byte)(220)))));
            this.lblActualizacion.Location = new System.Drawing.Point(124, 14);
            this.lblActualizacion.Name = "lblActualizacion";
            this.lblActualizacion.Size = new System.Drawing.Size(864, 21);
            this.lblActualizacion.TabIndex = 1;
            this.lblActualizacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dgvContratos
            //
            this.dgvContratos.AllowUserToAddRows = false;
            this.dgvContratos.AllowUserToDeleteRows = false;
            this.dgvContratos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvContratos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvContratos.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(31)))), ((int)(((byte)(50)))));
            this.dgvContratos.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            this.dgvContratos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvContratos.DefaultCellStyle = estiloCelda;
            this.dgvContratos.EnableHeadersVisualStyles = false;
            this.dgvContratos.GridColor = System.Drawing.Color.Black;
            this.dgvContratos.Location = new System.Drawing.Point(12, 46);
            this.dgvContratos.MultiSelect = false;
            this.dgvContratos.Name = "dgvContratos";
            this.dgvContratos.ReadOnly = true;
            this.dgvContratos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvContratos.Size = new System.Drawing.Size(976, 222);
            this.dgvContratos.TabIndex = 2;
            this.dgvContratos.SelectionChanged += new System.EventHandler(this.dgvContratos_SelectionChanged);
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
            this.grpInquilino.Size = new System.Drawing.Size(320, 148);
            this.grpInquilino.TabIndex = 3;
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
            // grpAcciones
            //
            this.grpAcciones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpAcciones.BackColor = System.Drawing.Color.Transparent;
            this.grpAcciones.Controls.Add(this.lblObservacion);
            this.grpAcciones.Controls.Add(this.txtObservacion);
            this.grpAcciones.Controls.Add(this.btnRenovacion);
            this.grpAcciones.Controls.Add(this.btnDesocupacion);
            this.grpAcciones.ForeColor = System.Drawing.Color.White;
            this.grpAcciones.Location = new System.Drawing.Point(12, 434);
            this.grpAcciones.Name = "grpAcciones";
            this.grpAcciones.Size = new System.Drawing.Size(320, 136);
            this.grpAcciones.TabIndex = 4;
            this.grpAcciones.TabStop = false;
            this.grpAcciones.Text = "Acciones";
            //
            // lblObservacion
            //
            this.lblObservacion.Location = new System.Drawing.Point(10, 18);
            this.lblObservacion.Name = "lblObservacion";
            this.lblObservacion.Size = new System.Drawing.Size(300, 18);
            this.lblObservacion.TabIndex = 0;
            this.lblObservacion.Text = "Observación de la desocupación (opcional):";
            this.lblObservacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtObservacion
            //
            this.txtObservacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.txtObservacion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtObservacion.ForeColor = System.Drawing.Color.White;
            this.txtObservacion.Location = new System.Drawing.Point(10, 38);
            this.txtObservacion.MaxLength = 300;
            this.txtObservacion.Multiline = true;
            this.txtObservacion.Name = "txtObservacion";
            this.txtObservacion.Size = new System.Drawing.Size(300, 40);
            this.txtObservacion.TabIndex = 1;
            //
            // btnRenovacion
            //
            this.btnRenovacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRenovacion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRenovacion.ForeColor = System.Drawing.Color.White;
            this.btnRenovacion.Location = new System.Drawing.Point(10, 88);
            this.btnRenovacion.Name = "btnRenovacion";
            this.btnRenovacion.Size = new System.Drawing.Size(145, 36);
            this.btnRenovacion.TabIndex = 2;
            this.btnRenovacion.Text = "Registrar Renovación";
            this.btnRenovacion.UseVisualStyleBackColor = false;
            this.btnRenovacion.Click += new System.EventHandler(this.btnRenovacion_Click);
            //
            // btnDesocupacion
            //
            this.btnDesocupacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnDesocupacion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDesocupacion.ForeColor = System.Drawing.Color.White;
            this.btnDesocupacion.Location = new System.Drawing.Point(165, 88);
            this.btnDesocupacion.Name = "btnDesocupacion";
            this.btnDesocupacion.Size = new System.Drawing.Size(145, 36);
            this.btnDesocupacion.TabIndex = 3;
            this.btnDesocupacion.Text = "Registrar Desocupación";
            this.btnDesocupacion.UseVisualStyleBackColor = false;
            this.btnDesocupacion.Click += new System.EventHandler(this.btnDesocupacion_Click);
            //
            // grpContrato
            //
            this.grpContrato.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpContrato.BackColor = System.Drawing.Color.Transparent;
            this.grpContrato.Controls.Add(this.lblNroContrato);
            this.grpContrato.Controls.Add(this.lblNroContratoValor);
            this.grpContrato.Controls.Add(this.lblDireccion);
            this.grpContrato.Controls.Add(this.lblDireccionValor);
            this.grpContrato.Controls.Add(this.lblTipo);
            this.grpContrato.Controls.Add(this.lblTipoValor);
            this.grpContrato.Controls.Add(this.lblFechaInicio);
            this.grpContrato.Controls.Add(this.lblFechaInicioValor);
            this.grpContrato.Controls.Add(this.lblDuracion);
            this.grpContrato.Controls.Add(this.lblDuracionValor);
            this.grpContrato.Controls.Add(this.lblFechaFin);
            this.grpContrato.Controls.Add(this.lblFechaFinValor);
            this.grpContrato.Controls.Add(this.lblDiasRestantes);
            this.grpContrato.Controls.Add(this.lblDiasRestantesValor);
            this.grpContrato.Controls.Add(this.lblMonto);
            this.grpContrato.Controls.Add(this.lblMontoValor);
            this.grpContrato.Controls.Add(this.lblEstado);
            this.grpContrato.Controls.Add(this.lblEstadoValor);
            this.grpContrato.Controls.Add(this.lblAlerta);
            this.grpContrato.Controls.Add(this.lblAlertaValor);
            this.grpContrato.Controls.Add(this.lblPagoActual);
            this.grpContrato.Controls.Add(this.lblPagoActualValor);
            this.grpContrato.Controls.Add(this.lblDeuda);
            this.grpContrato.Controls.Add(this.lblDeudaValor);
            this.grpContrato.Controls.Add(this.lblDesocupacion);
            this.grpContrato.Controls.Add(this.lblDesocupacionValor);
            this.grpContrato.ForeColor = System.Drawing.Color.White;
            this.grpContrato.Location = new System.Drawing.Point(342, 278);
            this.grpContrato.Name = "grpContrato";
            this.grpContrato.Size = new System.Drawing.Size(646, 292);
            this.grpContrato.TabIndex = 5;
            this.grpContrato.TabStop = false;
            this.grpContrato.Text = "Contrato";
            //
            // lblNroContrato
            //
            this.lblNroContrato.Location = new System.Drawing.Point(6, 22);
            this.lblNroContrato.Name = "lblNroContrato";
            this.lblNroContrato.Size = new System.Drawing.Size(110, 20);
            this.lblNroContrato.TabIndex = 0;
            this.lblNroContrato.Text = "Nro. contrato:";
            this.lblNroContrato.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblNroContratoValor
            //
            this.lblNroContratoValor.Location = new System.Drawing.Point(122, 22);
            this.lblNroContratoValor.Name = "lblNroContratoValor";
            this.lblNroContratoValor.Size = new System.Drawing.Size(190, 20);
            this.lblNroContratoValor.TabIndex = 1;
            this.lblNroContratoValor.Text = "-";
            this.lblNroContratoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDireccion
            //
            this.lblDireccion.Location = new System.Drawing.Point(6, 45);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(110, 20);
            this.lblDireccion.TabIndex = 2;
            this.lblDireccion.Text = "Dirección:";
            this.lblDireccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDireccionValor
            //
            this.lblDireccionValor.AutoEllipsis = true;
            this.lblDireccionValor.Location = new System.Drawing.Point(122, 45);
            this.lblDireccionValor.Name = "lblDireccionValor";
            this.lblDireccionValor.Size = new System.Drawing.Size(190, 20);
            this.lblDireccionValor.TabIndex = 3;
            this.lblDireccionValor.Text = "-";
            this.lblDireccionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTipo
            //
            this.lblTipo.Location = new System.Drawing.Point(6, 68);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(110, 20);
            this.lblTipo.TabIndex = 4;
            this.lblTipo.Text = "Tipo:";
            this.lblTipo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTipoValor
            //
            this.lblTipoValor.Location = new System.Drawing.Point(122, 68);
            this.lblTipoValor.Name = "lblTipoValor";
            this.lblTipoValor.Size = new System.Drawing.Size(190, 20);
            this.lblTipoValor.TabIndex = 5;
            this.lblTipoValor.Text = "-";
            this.lblTipoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblFechaInicio
            //
            this.lblFechaInicio.Location = new System.Drawing.Point(6, 91);
            this.lblFechaInicio.Name = "lblFechaInicio";
            this.lblFechaInicio.Size = new System.Drawing.Size(110, 20);
            this.lblFechaInicio.TabIndex = 6;
            this.lblFechaInicio.Text = "Fecha de inicio:";
            this.lblFechaInicio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblFechaInicioValor
            //
            this.lblFechaInicioValor.Location = new System.Drawing.Point(122, 91);
            this.lblFechaInicioValor.Name = "lblFechaInicioValor";
            this.lblFechaInicioValor.Size = new System.Drawing.Size(190, 20);
            this.lblFechaInicioValor.TabIndex = 7;
            this.lblFechaInicioValor.Text = "-";
            this.lblFechaInicioValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDuracion
            //
            this.lblDuracion.Location = new System.Drawing.Point(6, 114);
            this.lblDuracion.Name = "lblDuracion";
            this.lblDuracion.Size = new System.Drawing.Size(110, 20);
            this.lblDuracion.TabIndex = 8;
            this.lblDuracion.Text = "Duración:";
            this.lblDuracion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDuracionValor
            //
            this.lblDuracionValor.Location = new System.Drawing.Point(122, 114);
            this.lblDuracionValor.Name = "lblDuracionValor";
            this.lblDuracionValor.Size = new System.Drawing.Size(190, 20);
            this.lblDuracionValor.TabIndex = 9;
            this.lblDuracionValor.Text = "-";
            this.lblDuracionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblFechaFin
            //
            this.lblFechaFin.Location = new System.Drawing.Point(320, 22);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(110, 20);
            this.lblFechaFin.TabIndex = 10;
            this.lblFechaFin.Text = "Fecha de fin:";
            this.lblFechaFin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblFechaFinValor
            //
            this.lblFechaFinValor.Location = new System.Drawing.Point(436, 22);
            this.lblFechaFinValor.Name = "lblFechaFinValor";
            this.lblFechaFinValor.Size = new System.Drawing.Size(200, 20);
            this.lblFechaFinValor.TabIndex = 11;
            this.lblFechaFinValor.Text = "-";
            this.lblFechaFinValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDiasRestantes
            //
            this.lblDiasRestantes.Location = new System.Drawing.Point(320, 45);
            this.lblDiasRestantes.Name = "lblDiasRestantes";
            this.lblDiasRestantes.Size = new System.Drawing.Size(110, 20);
            this.lblDiasRestantes.TabIndex = 12;
            this.lblDiasRestantes.Text = "Días restantes:";
            this.lblDiasRestantes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDiasRestantesValor
            //
            this.lblDiasRestantesValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiasRestantesValor.Location = new System.Drawing.Point(436, 45);
            this.lblDiasRestantesValor.Name = "lblDiasRestantesValor";
            this.lblDiasRestantesValor.Size = new System.Drawing.Size(200, 20);
            this.lblDiasRestantesValor.TabIndex = 13;
            this.lblDiasRestantesValor.Text = "-";
            this.lblDiasRestantesValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMonto
            //
            this.lblMonto.Location = new System.Drawing.Point(320, 68);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(110, 20);
            this.lblMonto.TabIndex = 14;
            this.lblMonto.Text = "Monto mensual:";
            this.lblMonto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMontoValor
            //
            this.lblMontoValor.Location = new System.Drawing.Point(436, 68);
            this.lblMontoValor.Name = "lblMontoValor";
            this.lblMontoValor.Size = new System.Drawing.Size(200, 20);
            this.lblMontoValor.TabIndex = 15;
            this.lblMontoValor.Text = "-";
            this.lblMontoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblEstado
            //
            this.lblEstado.Location = new System.Drawing.Point(320, 91);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(110, 20);
            this.lblEstado.TabIndex = 16;
            this.lblEstado.Text = "Estado:";
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblEstadoValor
            //
            this.lblEstadoValor.Location = new System.Drawing.Point(436, 91);
            this.lblEstadoValor.Name = "lblEstadoValor";
            this.lblEstadoValor.Size = new System.Drawing.Size(200, 20);
            this.lblEstadoValor.TabIndex = 17;
            this.lblEstadoValor.Text = "-";
            this.lblEstadoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblAlerta
            //
            this.lblAlerta.Location = new System.Drawing.Point(320, 114);
            this.lblAlerta.Name = "lblAlerta";
            this.lblAlerta.Size = new System.Drawing.Size(110, 20);
            this.lblAlerta.TabIndex = 18;
            this.lblAlerta.Text = "Alerta de vencim.:";
            this.lblAlerta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblAlertaValor
            //
            this.lblAlertaValor.Location = new System.Drawing.Point(436, 114);
            this.lblAlertaValor.Name = "lblAlertaValor";
            this.lblAlertaValor.Size = new System.Drawing.Size(200, 20);
            this.lblAlertaValor.TabIndex = 19;
            this.lblAlertaValor.Text = "-";
            this.lblAlertaValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPagoActual
            //
            this.lblPagoActual.Location = new System.Drawing.Point(6, 150);
            this.lblPagoActual.Name = "lblPagoActual";
            this.lblPagoActual.Size = new System.Drawing.Size(110, 20);
            this.lblPagoActual.TabIndex = 20;
            this.lblPagoActual.Text = "Pago mes actual:";
            this.lblPagoActual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblPagoActualValor
            //
            this.lblPagoActualValor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPagoActualValor.AutoEllipsis = true;
            this.lblPagoActualValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPagoActualValor.Location = new System.Drawing.Point(122, 150);
            this.lblPagoActualValor.Name = "lblPagoActualValor";
            this.lblPagoActualValor.Size = new System.Drawing.Size(514, 20);
            this.lblPagoActualValor.TabIndex = 21;
            this.lblPagoActualValor.Text = "-";
            this.lblPagoActualValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDeuda
            //
            this.lblDeuda.Location = new System.Drawing.Point(6, 173);
            this.lblDeuda.Name = "lblDeuda";
            this.lblDeuda.Size = new System.Drawing.Size(110, 20);
            this.lblDeuda.TabIndex = 22;
            this.lblDeuda.Text = "Deuda:";
            this.lblDeuda.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDeudaValor
            //
            this.lblDeudaValor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDeudaValor.AutoEllipsis = true;
            this.lblDeudaValor.Location = new System.Drawing.Point(122, 173);
            this.lblDeudaValor.Name = "lblDeudaValor";
            this.lblDeudaValor.Size = new System.Drawing.Size(514, 20);
            this.lblDeudaValor.TabIndex = 23;
            this.lblDeudaValor.Text = "-";
            this.lblDeudaValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDesocupacion
            //
            this.lblDesocupacion.Location = new System.Drawing.Point(6, 196);
            this.lblDesocupacion.Name = "lblDesocupacion";
            this.lblDesocupacion.Size = new System.Drawing.Size(110, 20);
            this.lblDesocupacion.TabIndex = 24;
            this.lblDesocupacion.Text = "Desocupación:";
            this.lblDesocupacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDesocupacionValor
            //
            this.lblDesocupacionValor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDesocupacionValor.AutoEllipsis = true;
            this.lblDesocupacionValor.Location = new System.Drawing.Point(122, 196);
            this.lblDesocupacionValor.Name = "lblDesocupacionValor";
            this.lblDesocupacionValor.Size = new System.Drawing.Size(514, 20);
            this.lblDesocupacionValor.TabIndex = 25;
            this.lblDesocupacionValor.Text = "-";
            this.lblDesocupacionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FormContratosPorVencer
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.ClientSize = new System.Drawing.Size(1000, 582);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.lblActualizacion);
            this.Controls.Add(this.dgvContratos);
            this.Controls.Add(this.grpInquilino);
            this.Controls.Add(this.grpAcciones);
            this.Controls.Add(this.grpContrato);
            this.MinimumSize = new System.Drawing.Size(1016, 621);
            this.Name = "FormContratosPorVencer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contratos Por Vencer";
            this.Load += new System.EventHandler(this.FormContratosPorVencer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvContratos)).EndInit();
            this.grpInquilino.ResumeLayout(false);
            this.grpAcciones.ResumeLayout(false);
            this.grpAcciones.PerformLayout();
            this.grpContrato.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Label lblActualizacion;
        private System.Windows.Forms.DataGridView dgvContratos;
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
        private System.Windows.Forms.GroupBox grpAcciones;
        private System.Windows.Forms.Label lblObservacion;
        private System.Windows.Forms.TextBox txtObservacion;
        private System.Windows.Forms.Button btnRenovacion;
        private System.Windows.Forms.Button btnDesocupacion;
        private System.Windows.Forms.GroupBox grpContrato;
        private System.Windows.Forms.Label lblNroContrato;
        private System.Windows.Forms.Label lblNroContratoValor;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.Label lblDireccionValor;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.Label lblTipoValor;
        private System.Windows.Forms.Label lblFechaInicio;
        private System.Windows.Forms.Label lblFechaInicioValor;
        private System.Windows.Forms.Label lblDuracion;
        private System.Windows.Forms.Label lblDuracionValor;
        private System.Windows.Forms.Label lblFechaFin;
        private System.Windows.Forms.Label lblFechaFinValor;
        private System.Windows.Forms.Label lblDiasRestantes;
        private System.Windows.Forms.Label lblDiasRestantesValor;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.Label lblMontoValor;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Label lblEstadoValor;
        private System.Windows.Forms.Label lblAlerta;
        private System.Windows.Forms.Label lblAlertaValor;
        private System.Windows.Forms.Label lblPagoActual;
        private System.Windows.Forms.Label lblPagoActualValor;
        private System.Windows.Forms.Label lblDeuda;
        private System.Windows.Forms.Label lblDeudaValor;
        private System.Windows.Forms.Label lblDesocupacion;
        private System.Windows.Forms.Label lblDesocupacionValor;
    }
}
