namespace TpIngSoft
{
    partial class FormGestionarContratos
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
            this.lblFiltroEstado = new System.Windows.Forms.Label();
            this.cmbFiltroEstado = new System.Windows.Forms.ComboBox();
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
            this.grpRenovacion = new System.Windows.Forms.GroupBox();
            this.lblMesesExtension = new System.Windows.Forms.Label();
            this.nudMesesExtension = new System.Windows.Forms.NumericUpDown();
            this.lblNuevoMonto = new System.Windows.Forms.Label();
            this.nudNuevoMonto = new System.Windows.Forms.NumericUpDown();
            this.lblNuevoVencimiento = new System.Windows.Forms.Label();
            this.lblNuevoVencimientoValor = new System.Windows.Forms.Label();
            this.btnRenovar = new System.Windows.Forms.Button();
            this.grpPagos = new System.Windows.Forms.GroupBox();
            this.lblMesActual = new System.Windows.Forms.Label();
            this.lblMesActualValor = new System.Windows.Forms.Label();
            this.dgvPagos = new System.Windows.Forms.DataGridView();
            this.grpCondiciones = new System.Windows.Forms.GroupBox();
            this.lblDeposito = new System.Windows.Forms.Label();
            this.lblDepositoValor = new System.Windows.Forms.Label();
            this.lblComision = new System.Windows.Forms.Label();
            this.lblComisionValor = new System.Windows.Forms.Label();
            this.lblMetodoPago = new System.Windows.Forms.Label();
            this.lblMetodoPagoValor = new System.Windows.Forms.Label();
            this.lblIndice = new System.Windows.Forms.Label();
            this.lblIndiceValor = new System.Windows.Forms.Label();
            this.lblGarante = new System.Windows.Forms.Label();
            this.lblGaranteValor = new System.Windows.Forms.Label();
            this.grpRescision = new System.Windows.Forms.GroupBox();
            this.lblIniciadaPor = new System.Windows.Forms.Label();
            this.cmbIniciadaPor = new System.Windows.Forms.ComboBox();
            this.lblMesesRestantes = new System.Windows.Forms.Label();
            this.lblMesesRestantesValor = new System.Windows.Forms.Label();
            this.lblPenalidad = new System.Windows.Forms.Label();
            this.lblPenalidadValor = new System.Windows.Forms.Label();
            this.btnRescindir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContratos)).BeginInit();
            this.grpCondiciones.SuspendLayout();
            this.grpRescision.SuspendLayout();
            this.grpInquilino.SuspendLayout();
            this.grpRenovacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMesesExtension)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNuevoMonto)).BeginInit();
            this.grpPagos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).BeginInit();
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
            // lblActualizacion
            //
            this.lblActualizacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblActualizacion.AutoEllipsis = true;
            this.lblActualizacion.BackColor = System.Drawing.Color.Transparent;
            this.lblActualizacion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(195)))), ((int)(((byte)(220)))));
            this.lblActualizacion.Location = new System.Drawing.Point(350, 14);
            this.lblActualizacion.Name = "lblActualizacion";
            this.lblActualizacion.Size = new System.Drawing.Size(638, 21);
            this.lblActualizacion.TabIndex = 3;
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
            this.dgvContratos.Size = new System.Drawing.Size(976, 220);
            this.dgvContratos.TabIndex = 4;
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
            this.grpInquilino.Location = new System.Drawing.Point(12, 276);
            this.grpInquilino.Name = "grpInquilino";
            this.grpInquilino.Size = new System.Drawing.Size(320, 148);
            this.grpInquilino.TabIndex = 5;
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
            // grpRenovacion
            //
            this.grpRenovacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpRenovacion.BackColor = System.Drawing.Color.Transparent;
            this.grpRenovacion.Controls.Add(this.lblMesesExtension);
            this.grpRenovacion.Controls.Add(this.nudMesesExtension);
            this.grpRenovacion.Controls.Add(this.lblNuevoMonto);
            this.grpRenovacion.Controls.Add(this.nudNuevoMonto);
            this.grpRenovacion.Controls.Add(this.lblNuevoVencimiento);
            this.grpRenovacion.Controls.Add(this.lblNuevoVencimientoValor);
            this.grpRenovacion.Controls.Add(this.btnRenovar);
            this.grpRenovacion.ForeColor = System.Drawing.Color.White;
            this.grpRenovacion.Location = new System.Drawing.Point(12, 432);
            this.grpRenovacion.Name = "grpRenovacion";
            this.grpRenovacion.Size = new System.Drawing.Size(320, 164);
            this.grpRenovacion.TabIndex = 6;
            this.grpRenovacion.TabStop = false;
            this.grpRenovacion.Text = "Renovación";
            //
            // lblMesesExtension
            //
            this.lblMesesExtension.Location = new System.Drawing.Point(6, 24);
            this.lblMesesExtension.Name = "lblMesesExtension";
            this.lblMesesExtension.Size = new System.Drawing.Size(130, 20);
            this.lblMesesExtension.TabIndex = 0;
            this.lblMesesExtension.Text = "Extender (meses):";
            this.lblMesesExtension.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudMesesExtension
            //
            this.nudMesesExtension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudMesesExtension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudMesesExtension.ForeColor = System.Drawing.Color.White;
            this.nudMesesExtension.Location = new System.Drawing.Point(142, 24);
            this.nudMesesExtension.Maximum = new decimal(new int[] { 240, 0, 0, 0 });
            this.nudMesesExtension.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudMesesExtension.Name = "nudMesesExtension";
            this.nudMesesExtension.Size = new System.Drawing.Size(80, 20);
            this.nudMesesExtension.TabIndex = 1;
            this.nudMesesExtension.Value = new decimal(new int[] { 24, 0, 0, 0 });
            this.nudMesesExtension.ValueChanged += new System.EventHandler(this.nudMesesExtension_ValueChanged);
            //
            // lblNuevoMonto
            //
            this.lblNuevoMonto.Location = new System.Drawing.Point(6, 54);
            this.lblNuevoMonto.Name = "lblNuevoMonto";
            this.lblNuevoMonto.Size = new System.Drawing.Size(130, 20);
            this.lblNuevoMonto.TabIndex = 2;
            this.lblNuevoMonto.Text = "Nuevo monto mensual:";
            this.lblNuevoMonto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudNuevoMonto
            //
            this.nudNuevoMonto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudNuevoMonto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudNuevoMonto.DecimalPlaces = 2;
            this.nudNuevoMonto.ForeColor = System.Drawing.Color.White;
            this.nudNuevoMonto.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.nudNuevoMonto.Location = new System.Drawing.Point(142, 54);
            this.nudNuevoMonto.Maximum = new decimal(new int[] { 1215752191, 23, 0, 0 });
            this.nudNuevoMonto.Name = "nudNuevoMonto";
            this.nudNuevoMonto.Size = new System.Drawing.Size(160, 20);
            this.nudNuevoMonto.TabIndex = 3;
            this.nudNuevoMonto.ThousandsSeparator = true;
            //
            // lblNuevoVencimiento
            //
            this.lblNuevoVencimiento.Location = new System.Drawing.Point(6, 84);
            this.lblNuevoVencimiento.Name = "lblNuevoVencimiento";
            this.lblNuevoVencimiento.Size = new System.Drawing.Size(130, 20);
            this.lblNuevoVencimiento.TabIndex = 4;
            this.lblNuevoVencimiento.Text = "Nuevo vencimiento:";
            this.lblNuevoVencimiento.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblNuevoVencimientoValor
            //
            this.lblNuevoVencimientoValor.Location = new System.Drawing.Point(142, 84);
            this.lblNuevoVencimientoValor.Name = "lblNuevoVencimientoValor";
            this.lblNuevoVencimientoValor.Size = new System.Drawing.Size(170, 20);
            this.lblNuevoVencimientoValor.TabIndex = 5;
            this.lblNuevoVencimientoValor.Text = "-";
            this.lblNuevoVencimientoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnRenovar
            //
            this.btnRenovar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRenovar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRenovar.ForeColor = System.Drawing.Color.White;
            this.btnRenovar.Location = new System.Drawing.Point(142, 116);
            this.btnRenovar.Name = "btnRenovar";
            this.btnRenovar.Size = new System.Drawing.Size(160, 34);
            this.btnRenovar.TabIndex = 6;
            this.btnRenovar.Text = "Renovar Contrato";
            this.btnRenovar.UseVisualStyleBackColor = false;
            this.btnRenovar.Click += new System.EventHandler(this.btnRenovar_Click);
            //
            // grpPagos
            //
            this.grpPagos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPagos.BackColor = System.Drawing.Color.Transparent;
            this.grpPagos.Controls.Add(this.lblMesActual);
            this.grpPagos.Controls.Add(this.lblMesActualValor);
            this.grpPagos.Controls.Add(this.dgvPagos);
            this.grpPagos.ForeColor = System.Drawing.Color.White;
            this.grpPagos.Location = new System.Drawing.Point(342, 432);
            this.grpPagos.Name = "grpPagos";
            this.grpPagos.Size = new System.Drawing.Size(646, 276);
            this.grpPagos.TabIndex = 7;
            this.grpPagos.TabStop = false;
            this.grpPagos.Text = "Historial de pagos";
            //
            // lblMesActual
            //
            this.lblMesActual.Location = new System.Drawing.Point(10, 20);
            this.lblMesActual.Name = "lblMesActual";
            this.lblMesActual.Size = new System.Drawing.Size(80, 20);
            this.lblMesActual.TabIndex = 0;
            this.lblMesActual.Text = "Mes actual:";
            this.lblMesActual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMesActualValor
            //
            this.lblMesActualValor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMesActualValor.AutoEllipsis = true;
            this.lblMesActualValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMesActualValor.Location = new System.Drawing.Point(96, 20);
            this.lblMesActualValor.Name = "lblMesActualValor";
            this.lblMesActualValor.Size = new System.Drawing.Size(540, 20);
            this.lblMesActualValor.TabIndex = 1;
            this.lblMesActualValor.Text = "-";
            this.lblMesActualValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.dgvPagos.Location = new System.Drawing.Point(10, 46);
            this.dgvPagos.MultiSelect = false;
            this.dgvPagos.Name = "dgvPagos";
            this.dgvPagos.ReadOnly = true;
            this.dgvPagos.RowHeadersVisible = false;
            this.dgvPagos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPagos.Size = new System.Drawing.Size(626, 220);
            this.dgvPagos.TabIndex = 2;
            //
            // grpCondiciones
            //
            this.grpCondiciones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpCondiciones.BackColor = System.Drawing.Color.Transparent;
            this.grpCondiciones.Controls.Add(this.lblDeposito);
            this.grpCondiciones.Controls.Add(this.lblDepositoValor);
            this.grpCondiciones.Controls.Add(this.lblComision);
            this.grpCondiciones.Controls.Add(this.lblComisionValor);
            this.grpCondiciones.Controls.Add(this.lblMetodoPago);
            this.grpCondiciones.Controls.Add(this.lblMetodoPagoValor);
            this.grpCondiciones.Controls.Add(this.lblIndice);
            this.grpCondiciones.Controls.Add(this.lblIndiceValor);
            this.grpCondiciones.Controls.Add(this.lblGarante);
            this.grpCondiciones.Controls.Add(this.lblGaranteValor);
            this.grpCondiciones.ForeColor = System.Drawing.Color.White;
            this.grpCondiciones.Location = new System.Drawing.Point(342, 276);
            this.grpCondiciones.Name = "grpCondiciones";
            this.grpCondiciones.Size = new System.Drawing.Size(320, 148);
            this.grpCondiciones.TabStop = false;
            this.grpCondiciones.Text = "Condiciones";
            //
            // lblDeposito
            //
            this.lblDeposito.Location = new System.Drawing.Point(6, 22);
            this.lblDeposito.Name = "lblDeposito";
            this.lblDeposito.Size = new System.Drawing.Size(100, 20);
            this.lblDeposito.Text = "Depósito:";
            this.lblDeposito.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDepositoValor
            //
            this.lblDepositoValor.AutoEllipsis = true;
            this.lblDepositoValor.Location = new System.Drawing.Point(112, 22);
            this.lblDepositoValor.Name = "lblDepositoValor";
            this.lblDepositoValor.Size = new System.Drawing.Size(200, 20);
            this.lblDepositoValor.Text = "-";
            this.lblDepositoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblComision
            //
            this.lblComision.Location = new System.Drawing.Point(6, 45);
            this.lblComision.Name = "lblComision";
            this.lblComision.Size = new System.Drawing.Size(100, 20);
            this.lblComision.Text = "Comisión:";
            this.lblComision.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblComisionValor
            //
            this.lblComisionValor.AutoEllipsis = true;
            this.lblComisionValor.Location = new System.Drawing.Point(112, 45);
            this.lblComisionValor.Name = "lblComisionValor";
            this.lblComisionValor.Size = new System.Drawing.Size(200, 20);
            this.lblComisionValor.Text = "-";
            this.lblComisionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMetodoPago
            //
            this.lblMetodoPago.Location = new System.Drawing.Point(6, 68);
            this.lblMetodoPago.Name = "lblMetodoPago";
            this.lblMetodoPago.Size = new System.Drawing.Size(100, 20);
            this.lblMetodoPago.Text = "Método de pago:";
            this.lblMetodoPago.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMetodoPagoValor
            //
            this.lblMetodoPagoValor.AutoEllipsis = true;
            this.lblMetodoPagoValor.Location = new System.Drawing.Point(112, 68);
            this.lblMetodoPagoValor.Name = "lblMetodoPagoValor";
            this.lblMetodoPagoValor.Size = new System.Drawing.Size(200, 20);
            this.lblMetodoPagoValor.Text = "-";
            this.lblMetodoPagoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblIndice
            //
            this.lblIndice.Location = new System.Drawing.Point(6, 91);
            this.lblIndice.Name = "lblIndice";
            this.lblIndice.Size = new System.Drawing.Size(100, 20);
            this.lblIndice.Text = "Actualización:";
            this.lblIndice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblIndiceValor
            //
            this.lblIndiceValor.AutoEllipsis = true;
            this.lblIndiceValor.Location = new System.Drawing.Point(112, 91);
            this.lblIndiceValor.Name = "lblIndiceValor";
            this.lblIndiceValor.Size = new System.Drawing.Size(200, 20);
            this.lblIndiceValor.Text = "-";
            this.lblIndiceValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblGarante
            //
            this.lblGarante.Location = new System.Drawing.Point(6, 114);
            this.lblGarante.Name = "lblGarante";
            this.lblGarante.Size = new System.Drawing.Size(100, 20);
            this.lblGarante.Text = "Garante:";
            this.lblGarante.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblGaranteValor
            //
            this.lblGaranteValor.AutoEllipsis = true;
            this.lblGaranteValor.Location = new System.Drawing.Point(112, 114);
            this.lblGaranteValor.Name = "lblGaranteValor";
            this.lblGaranteValor.Size = new System.Drawing.Size(200, 20);
            this.lblGaranteValor.Text = "-";
            this.lblGaranteValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpRescision
            //
            this.grpRescision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpRescision.BackColor = System.Drawing.Color.Transparent;
            this.grpRescision.Controls.Add(this.lblIniciadaPor);
            this.grpRescision.Controls.Add(this.cmbIniciadaPor);
            this.grpRescision.Controls.Add(this.lblMesesRestantes);
            this.grpRescision.Controls.Add(this.lblMesesRestantesValor);
            this.grpRescision.Controls.Add(this.lblPenalidad);
            this.grpRescision.Controls.Add(this.lblPenalidadValor);
            this.grpRescision.Controls.Add(this.btnRescindir);
            this.grpRescision.ForeColor = System.Drawing.Color.White;
            this.grpRescision.Location = new System.Drawing.Point(672, 276);
            this.grpRescision.Name = "grpRescision";
            this.grpRescision.Size = new System.Drawing.Size(316, 148);
            this.grpRescision.TabStop = false;
            this.grpRescision.Text = "Rescisión";
            //
            // lblIniciadaPor
            //
            this.lblIniciadaPor.Location = new System.Drawing.Point(6, 22);
            this.lblIniciadaPor.Name = "lblIniciadaPor";
            this.lblIniciadaPor.Size = new System.Drawing.Size(110, 21);
            this.lblIniciadaPor.Text = "Iniciada por:";
            this.lblIniciadaPor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cmbIniciadaPor
            //
            this.cmbIniciadaPor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.cmbIniciadaPor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIniciadaPor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbIniciadaPor.ForeColor = System.Drawing.Color.White;
            this.cmbIniciadaPor.FormattingEnabled = true;
            this.cmbIniciadaPor.Location = new System.Drawing.Point(122, 22);
            this.cmbIniciadaPor.Name = "cmbIniciadaPor";
            this.cmbIniciadaPor.Size = new System.Drawing.Size(180, 21);
            //
            // lblMesesRestantes
            //
            this.lblMesesRestantes.Location = new System.Drawing.Point(6, 52);
            this.lblMesesRestantes.Name = "lblMesesRestantes";
            this.lblMesesRestantes.Size = new System.Drawing.Size(110, 20);
            this.lblMesesRestantes.Text = "Canon restante:";
            this.lblMesesRestantes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMesesRestantesValor
            //
            this.lblMesesRestantesValor.AutoEllipsis = true;
            this.lblMesesRestantesValor.Location = new System.Drawing.Point(122, 52);
            this.lblMesesRestantesValor.Name = "lblMesesRestantesValor";
            this.lblMesesRestantesValor.Size = new System.Drawing.Size(186, 20);
            this.lblMesesRestantesValor.Text = "-";
            this.lblMesesRestantesValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPenalidad
            //
            this.lblPenalidad.Location = new System.Drawing.Point(6, 76);
            this.lblPenalidad.Name = "lblPenalidad";
            this.lblPenalidad.Size = new System.Drawing.Size(110, 20);
            this.lblPenalidad.Text = "Penalidad:";
            this.lblPenalidad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblPenalidadValor
            //
            this.lblPenalidadValor.AutoEllipsis = true;
            this.lblPenalidadValor.Location = new System.Drawing.Point(122, 76);
            this.lblPenalidadValor.Name = "lblPenalidadValor";
            this.lblPenalidadValor.Size = new System.Drawing.Size(186, 20);
            this.lblPenalidadValor.Text = "-";
            this.lblPenalidadValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnRescindir
            //
            this.btnRescindir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnRescindir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRescindir.ForeColor = System.Drawing.Color.White;
            this.btnRescindir.Location = new System.Drawing.Point(122, 104);
            this.btnRescindir.Name = "btnRescindir";
            this.btnRescindir.Size = new System.Drawing.Size(180, 32);
            this.btnRescindir.Text = "Rescindir Contrato";
            this.btnRescindir.UseVisualStyleBackColor = false;
            this.btnRescindir.Click += new System.EventHandler(this.btnRescindir_Click);
            //
            // FormGestionarContratos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 720);
            this.Controls.Add(this.lblFiltroEstado);
            this.Controls.Add(this.cmbFiltroEstado);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.lblActualizacion);
            this.Controls.Add(this.dgvContratos);
            this.Controls.Add(this.grpInquilino);
            this.Controls.Add(this.grpCondiciones);
            this.Controls.Add(this.grpRescision);
            this.Controls.Add(this.grpRenovacion);
            this.Controls.Add(this.grpPagos);
            this.MinimumSize = new System.Drawing.Size(1016, 759);
            this.Name = "FormGestionarContratos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionar Contratos";
            this.Load += new System.EventHandler(this.FormGestionarContratos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvContratos)).EndInit();
            this.grpInquilino.ResumeLayout(false);
            this.grpCondiciones.ResumeLayout(false);
            this.grpRescision.ResumeLayout(false);
            this.grpRenovacion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudMesesExtension)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNuevoMonto)).EndInit();
            this.grpPagos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblFiltroEstado;
        private System.Windows.Forms.ComboBox cmbFiltroEstado;
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
        private System.Windows.Forms.GroupBox grpCondiciones;
        private System.Windows.Forms.Label lblDeposito;
        private System.Windows.Forms.Label lblDepositoValor;
        private System.Windows.Forms.Label lblComision;
        private System.Windows.Forms.Label lblComisionValor;
        private System.Windows.Forms.Label lblMetodoPago;
        private System.Windows.Forms.Label lblMetodoPagoValor;
        private System.Windows.Forms.Label lblIndice;
        private System.Windows.Forms.Label lblIndiceValor;
        private System.Windows.Forms.Label lblGarante;
        private System.Windows.Forms.Label lblGaranteValor;
        private System.Windows.Forms.GroupBox grpRescision;
        private System.Windows.Forms.Label lblIniciadaPor;
        private System.Windows.Forms.ComboBox cmbIniciadaPor;
        private System.Windows.Forms.Label lblMesesRestantes;
        private System.Windows.Forms.Label lblMesesRestantesValor;
        private System.Windows.Forms.Label lblPenalidad;
        private System.Windows.Forms.Label lblPenalidadValor;
        private System.Windows.Forms.Button btnRescindir;
        private System.Windows.Forms.GroupBox grpRenovacion;
        private System.Windows.Forms.Label lblMesesExtension;
        private System.Windows.Forms.NumericUpDown nudMesesExtension;
        private System.Windows.Forms.Label lblNuevoMonto;
        private System.Windows.Forms.NumericUpDown nudNuevoMonto;
        private System.Windows.Forms.Label lblNuevoVencimiento;
        private System.Windows.Forms.Label lblNuevoVencimientoValor;
        private System.Windows.Forms.Button btnRenovar;
        private System.Windows.Forms.GroupBox grpPagos;
        private System.Windows.Forms.Label lblMesActual;
        private System.Windows.Forms.Label lblMesActualValor;
        private System.Windows.Forms.DataGridView dgvPagos;
    }
}
