namespace TpIngSoft
{
    partial class FormEvaluacionArquitectonica
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
            this.dgvProyectos = new System.Windows.Forms.DataGridView();
            this.grpInmueble = new System.Windows.Forms.GroupBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.lblDireccionValor = new System.Windows.Forms.Label();
            this.lblLocalidad = new System.Windows.Forms.Label();
            this.lblLocalidadValor = new System.Windows.Forms.Label();
            this.lblTipo = new System.Windows.Forms.Label();
            this.lblTipoValor = new System.Windows.Forms.Label();
            this.lblSuperficie = new System.Windows.Forms.Label();
            this.lblSuperficieValor = new System.Windows.Forms.Label();
            this.lblCanje = new System.Windows.Forms.Label();
            this.lblCanjeValor = new System.Windows.Forms.Label();
            this.grpPropietario = new System.Windows.Forms.GroupBox();
            this.lblPropietario = new System.Windows.Forms.Label();
            this.lblPropietarioValor = new System.Windows.Forms.Label();
            this.lblDni = new System.Windows.Forms.Label();
            this.lblDniValor = new System.Windows.Forms.Label();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.lblTelefonoValor = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblEmailValor = new System.Windows.Forms.Label();
            this.grpDatosTecnicos = new System.Windows.Forms.GroupBox();
            this.lblLote = new System.Windows.Forms.Label();
            this.nudLote = new System.Windows.Forms.NumericUpDown();
            this.lblSupEdificable = new System.Windows.Forms.Label();
            this.lblConstruiblesTotales = new System.Windows.Forms.Label();
            this.nudSupEdificable = new System.Windows.Forms.NumericUpDown();
            this.lblAltura = new System.Windows.Forms.Label();
            this.nudAltura = new System.Windows.Forms.NumericUpDown();
            this.lblCostoM2 = new System.Windows.Forms.Label();
            this.nudCostoM2 = new System.Windows.Forms.NumericUpDown();
            this.lblValorM2 = new System.Windows.Forms.Label();
            this.nudValorM2 = new System.Windows.Forms.NumericUpDown();
            this.lblMetros = new System.Windows.Forms.Label();
            this.lblMetrosValor = new System.Windows.Forms.Label();
            this.lblCostoTotal = new System.Windows.Forms.Label();
            this.lblCostoTotalValor = new System.Windows.Forms.Label();
            this.lblValorTotal = new System.Windows.Forms.Label();
            this.lblValorTotalValor = new System.Windows.Forms.Label();
            this.lblMargen = new System.Windows.Forms.Label();
            this.lblMargenValor = new System.Windows.Forms.Label();
            this.lblIcc = new System.Windows.Forms.Label();
            this.lblIccValor = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnConsultarCaba = new System.Windows.Forms.Button();
            this.lblParcela = new System.Windows.Forms.Label();
            this.grpInmueble.SuspendLayout();
            this.grpPropietario.SuspendLayout();
            this.grpDatosTecnicos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSupEdificable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAltura)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCostoM2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValorM2)).BeginInit();
            this.SuspendLayout();
            //
            // dgvProyectos
            //
            this.dgvProyectos.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
            this.dgvProyectos.Location = new System.Drawing.Point(12, 12);
            this.dgvProyectos.Name = "dgvProyectos";
            this.dgvProyectos.Size = new System.Drawing.Size(976, 196);
            this.dgvProyectos.SelectionChanged += new System.EventHandler(this.dgvProyectos_SelectionChanged);
            //
            // grpInmueble
            //
            this.grpInmueble.BackColor = System.Drawing.Color.Transparent;
            this.grpInmueble.Controls.Add(this.lblDireccion);
            this.grpInmueble.Controls.Add(this.lblDireccionValor);
            this.grpInmueble.Controls.Add(this.lblLocalidad);
            this.grpInmueble.Controls.Add(this.lblLocalidadValor);
            this.grpInmueble.Controls.Add(this.lblTipo);
            this.grpInmueble.Controls.Add(this.lblTipoValor);
            this.grpInmueble.Controls.Add(this.lblSuperficie);
            this.grpInmueble.Controls.Add(this.lblSuperficieValor);
            this.grpInmueble.Controls.Add(this.lblCanje);
            this.grpInmueble.Controls.Add(this.lblCanjeValor);
            this.grpInmueble.ForeColor = System.Drawing.Color.White;
            this.grpInmueble.Location = new System.Drawing.Point(12, 216);
            this.grpInmueble.Name = "grpInmueble";
            this.grpInmueble.Size = new System.Drawing.Size(486, 150);
            this.grpInmueble.TabStop = false;
            this.grpInmueble.Text = "Inmueble";
            //
            // lblDireccion
            //
            this.lblDireccion.Location = new System.Drawing.Point(6, 22);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(130, 20);
            this.lblDireccion.Text = "Dirección:";
            this.lblDireccion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDireccionValor
            //
            this.lblDireccionValor.AutoEllipsis = true;
            this.lblDireccionValor.Location = new System.Drawing.Point(142, 22);
            this.lblDireccionValor.Name = "lblDireccionValor";
            this.lblDireccionValor.Size = new System.Drawing.Size(334, 20);
            this.lblDireccionValor.Text = "-";
            this.lblDireccionValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblLocalidad
            //
            this.lblLocalidad.Location = new System.Drawing.Point(6, 45);
            this.lblLocalidad.Name = "lblLocalidad";
            this.lblLocalidad.Size = new System.Drawing.Size(130, 20);
            this.lblLocalidad.Text = "Localidad:";
            this.lblLocalidad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblLocalidadValor
            //
            this.lblLocalidadValor.AutoEllipsis = true;
            this.lblLocalidadValor.Location = new System.Drawing.Point(142, 45);
            this.lblLocalidadValor.Name = "lblLocalidadValor";
            this.lblLocalidadValor.Size = new System.Drawing.Size(334, 20);
            this.lblLocalidadValor.Text = "-";
            this.lblLocalidadValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTipo
            //
            this.lblTipo.Location = new System.Drawing.Point(6, 68);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(130, 20);
            this.lblTipo.Text = "Tipo / operación:";
            this.lblTipo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTipoValor
            //
            this.lblTipoValor.AutoEllipsis = true;
            this.lblTipoValor.Location = new System.Drawing.Point(142, 68);
            this.lblTipoValor.Name = "lblTipoValor";
            this.lblTipoValor.Size = new System.Drawing.Size(334, 20);
            this.lblTipoValor.Text = "-";
            this.lblTipoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSuperficie
            //
            this.lblSuperficie.Location = new System.Drawing.Point(6, 91);
            this.lblSuperficie.Name = "lblSuperficie";
            this.lblSuperficie.Size = new System.Drawing.Size(130, 20);
            this.lblSuperficie.Text = "Superficie:";
            this.lblSuperficie.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblSuperficieValor
            //
            this.lblSuperficieValor.AutoEllipsis = true;
            this.lblSuperficieValor.Location = new System.Drawing.Point(142, 91);
            this.lblSuperficieValor.Name = "lblSuperficieValor";
            this.lblSuperficieValor.Size = new System.Drawing.Size(334, 20);
            this.lblSuperficieValor.Text = "-";
            this.lblSuperficieValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCanje
            //
            this.lblCanje.Location = new System.Drawing.Point(6, 114);
            this.lblCanje.Name = "lblCanje";
            this.lblCanje.Size = new System.Drawing.Size(130, 20);
            this.lblCanje.Text = "Canje propietario:";
            this.lblCanje.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCanjeValor
            //
            this.lblCanjeValor.AutoEllipsis = true;
            this.lblCanjeValor.Location = new System.Drawing.Point(142, 114);
            this.lblCanjeValor.Name = "lblCanjeValor";
            this.lblCanjeValor.Size = new System.Drawing.Size(334, 20);
            this.lblCanjeValor.Text = "-";
            this.lblCanjeValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpPropietario
            //
            this.grpPropietario.BackColor = System.Drawing.Color.Transparent;
            this.grpPropietario.Controls.Add(this.lblPropietario);
            this.grpPropietario.Controls.Add(this.lblPropietarioValor);
            this.grpPropietario.Controls.Add(this.lblDni);
            this.grpPropietario.Controls.Add(this.lblDniValor);
            this.grpPropietario.Controls.Add(this.lblTelefono);
            this.grpPropietario.Controls.Add(this.lblTelefonoValor);
            this.grpPropietario.Controls.Add(this.lblEmail);
            this.grpPropietario.Controls.Add(this.lblEmailValor);
            this.grpPropietario.ForeColor = System.Drawing.Color.White;
            this.grpPropietario.Location = new System.Drawing.Point(508, 216);
            this.grpPropietario.Name = "grpPropietario";
            this.grpPropietario.Size = new System.Drawing.Size(480, 150);
            this.grpPropietario.TabStop = false;
            this.grpPropietario.Text = "Propietario";
            //
            // lblPropietario
            //
            this.lblPropietario.Location = new System.Drawing.Point(6, 22);
            this.lblPropietario.Name = "lblPropietario";
            this.lblPropietario.Size = new System.Drawing.Size(130, 20);
            this.lblPropietario.Text = "Nombre:";
            this.lblPropietario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblPropietarioValor
            //
            this.lblPropietarioValor.AutoEllipsis = true;
            this.lblPropietarioValor.Location = new System.Drawing.Point(142, 22);
            this.lblPropietarioValor.Name = "lblPropietarioValor";
            this.lblPropietarioValor.Size = new System.Drawing.Size(328, 20);
            this.lblPropietarioValor.Text = "-";
            this.lblPropietarioValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblDni
            //
            this.lblDni.Location = new System.Drawing.Point(6, 45);
            this.lblDni.Name = "lblDni";
            this.lblDni.Size = new System.Drawing.Size(130, 20);
            this.lblDni.Text = "DNI:";
            this.lblDni.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblDniValor
            //
            this.lblDniValor.AutoEllipsis = true;
            this.lblDniValor.Location = new System.Drawing.Point(142, 45);
            this.lblDniValor.Name = "lblDniValor";
            this.lblDniValor.Size = new System.Drawing.Size(328, 20);
            this.lblDniValor.Text = "-";
            this.lblDniValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTelefono
            //
            this.lblTelefono.Location = new System.Drawing.Point(6, 68);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(130, 20);
            this.lblTelefono.Text = "Teléfono:";
            this.lblTelefono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTelefonoValor
            //
            this.lblTelefonoValor.AutoEllipsis = true;
            this.lblTelefonoValor.Location = new System.Drawing.Point(142, 68);
            this.lblTelefonoValor.Name = "lblTelefonoValor";
            this.lblTelefonoValor.Size = new System.Drawing.Size(328, 20);
            this.lblTelefonoValor.Text = "-";
            this.lblTelefonoValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblEmail
            //
            this.lblEmail.Location = new System.Drawing.Point(6, 91);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(130, 20);
            this.lblEmail.Text = "Email:";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblEmailValor
            //
            this.lblEmailValor.AutoEllipsis = true;
            this.lblEmailValor.Location = new System.Drawing.Point(142, 91);
            this.lblEmailValor.Name = "lblEmailValor";
            this.lblEmailValor.Size = new System.Drawing.Size(328, 20);
            this.lblEmailValor.Text = "-";
            this.lblEmailValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpDatosTecnicos
            //
            this.grpDatosTecnicos.BackColor = System.Drawing.Color.Transparent;
            this.grpDatosTecnicos.Controls.Add(this.lblLote);
            this.grpDatosTecnicos.Controls.Add(this.nudLote);
            this.grpDatosTecnicos.Controls.Add(this.lblSupEdificable);
            this.grpDatosTecnicos.Controls.Add(this.nudSupEdificable);
            this.grpDatosTecnicos.Controls.Add(this.lblConstruiblesTotales);
            this.grpDatosTecnicos.Controls.Add(this.lblAltura);
            this.grpDatosTecnicos.Controls.Add(this.nudAltura);
            this.grpDatosTecnicos.Controls.Add(this.lblCostoM2);
            this.grpDatosTecnicos.Controls.Add(this.nudCostoM2);
            this.grpDatosTecnicos.Controls.Add(this.lblValorM2);
            this.grpDatosTecnicos.Controls.Add(this.nudValorM2);
            this.grpDatosTecnicos.Controls.Add(this.lblMetros);
            this.grpDatosTecnicos.Controls.Add(this.lblMetrosValor);
            this.grpDatosTecnicos.Controls.Add(this.lblCostoTotal);
            this.grpDatosTecnicos.Controls.Add(this.lblCostoTotalValor);
            this.grpDatosTecnicos.Controls.Add(this.lblValorTotal);
            this.grpDatosTecnicos.Controls.Add(this.lblValorTotalValor);
            this.grpDatosTecnicos.Controls.Add(this.lblMargen);
            this.grpDatosTecnicos.Controls.Add(this.lblMargenValor);
            this.grpDatosTecnicos.Controls.Add(this.lblIcc);
            this.grpDatosTecnicos.Controls.Add(this.lblIccValor);
            this.grpDatosTecnicos.ForeColor = System.Drawing.Color.White;
            this.grpDatosTecnicos.Location = new System.Drawing.Point(12, 374);
            this.grpDatosTecnicos.Name = "grpDatosTecnicos";
            this.grpDatosTecnicos.Size = new System.Drawing.Size(976, 196);
            this.grpDatosTecnicos.TabStop = false;
            this.grpDatosTecnicos.Text = "Datos técnicos";
            //
            // lblLote
            //
            this.lblLote.Location = new System.Drawing.Point(6, 24);
            this.lblLote.Name = "lblLote";
            this.lblLote.Size = new System.Drawing.Size(170, 20);
            this.lblLote.Text = "Superficie lote (m²):";
            this.lblLote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudLote
            //
            this.nudLote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudLote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudLote.DecimalPlaces = 2;
            this.nudLote.ForeColor = System.Drawing.Color.White;
            this.nudLote.Location = new System.Drawing.Point(180, 24);
            this.nudLote.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.nudLote.Name = "nudLote";
            this.nudLote.Size = new System.Drawing.Size(120, 20);
            this.nudLote.ThousandsSeparator = true;
            this.nudLote.ValueChanged += new System.EventHandler(this.DatosTecnicos_Changed);
            //
            // lblSupEdificable
            //
            this.lblSupEdificable.Location = new System.Drawing.Point(6, 52);
            this.lblSupEdificable.Name = "lblSupEdificable";
            this.lblSupEdificable.Size = new System.Drawing.Size(170, 20);
            this.lblSupEdificable.Text = "Sup. edificable por planta (m²):";
            this.lblSupEdificable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudSupEdificable
            //
            this.nudSupEdificable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudSupEdificable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudSupEdificable.DecimalPlaces = 2;
            this.nudSupEdificable.ForeColor = System.Drawing.Color.White;
            this.nudSupEdificable.Location = new System.Drawing.Point(180, 52);
            this.nudSupEdificable.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudSupEdificable.Name = "nudSupEdificable";
            this.nudSupEdificable.Size = new System.Drawing.Size(120, 20);
            this.nudSupEdificable.ThousandsSeparator = true;
            this.nudSupEdificable.ValueChanged += new System.EventHandler(this.DatosTecnicos_Changed);
            //
            // lblConstruiblesTotales
            //
            this.lblConstruiblesTotales.AutoEllipsis = true;
            this.lblConstruiblesTotales.BackColor = System.Drawing.Color.Transparent;
            this.lblConstruiblesTotales.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.lblConstruiblesTotales.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(196)))), ((int)(((byte)(214)))));
            this.lblConstruiblesTotales.Location = new System.Drawing.Point(6, 75);
            this.lblConstruiblesTotales.Name = "lblConstruiblesTotales";
            this.lblConstruiblesTotales.Size = new System.Drawing.Size(296, 20);
            this.lblConstruiblesTotales.Text = "";
            this.lblConstruiblesTotales.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblAltura
            //
            this.lblAltura.Location = new System.Drawing.Point(6, 100);
            this.lblAltura.Name = "lblAltura";
            this.lblAltura.Size = new System.Drawing.Size(170, 20);
            this.lblAltura.Text = "Altura máxima (m):";
            this.lblAltura.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudAltura
            //
            this.nudAltura.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudAltura.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudAltura.DecimalPlaces = 2;
            this.nudAltura.ForeColor = System.Drawing.Color.White;
            this.nudAltura.Location = new System.Drawing.Point(180, 100);
            this.nudAltura.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            this.nudAltura.Name = "nudAltura";
            this.nudAltura.Size = new System.Drawing.Size(120, 20);
            this.nudAltura.ThousandsSeparator = true;
            this.nudAltura.ValueChanged += new System.EventHandler(this.DatosTecnicos_Changed);
            //
            // lblCostoM2
            //
            this.lblCostoM2.Location = new System.Drawing.Point(6, 128);
            this.lblCostoM2.Name = "lblCostoM2";
            this.lblCostoM2.Size = new System.Drawing.Size(170, 20);
            this.lblCostoM2.Text = "Costo obra / m² (USD):";
            this.lblCostoM2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudCostoM2
            //
            this.nudCostoM2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudCostoM2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudCostoM2.DecimalPlaces = 2;
            this.nudCostoM2.ForeColor = System.Drawing.Color.White;
            this.nudCostoM2.Location = new System.Drawing.Point(180, 128);
            this.nudCostoM2.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudCostoM2.Name = "nudCostoM2";
            this.nudCostoM2.Size = new System.Drawing.Size(120, 20);
            this.nudCostoM2.ThousandsSeparator = true;
            this.nudCostoM2.ValueChanged += new System.EventHandler(this.DatosTecnicos_Changed);
            //
            // lblValorM2
            //
            this.lblValorM2.Location = new System.Drawing.Point(6, 156);
            this.lblValorM2.Name = "lblValorM2";
            this.lblValorM2.Size = new System.Drawing.Size(170, 20);
            this.lblValorM2.Text = "Venta unid. / m² (USD):";
            this.lblValorM2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nudValorM2
            //
            this.nudValorM2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(57)))), ((int)(((byte)(79)))));
            this.nudValorM2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudValorM2.DecimalPlaces = 2;
            this.nudValorM2.ForeColor = System.Drawing.Color.White;
            this.nudValorM2.Location = new System.Drawing.Point(180, 156);
            this.nudValorM2.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudValorM2.Name = "nudValorM2";
            this.nudValorM2.Size = new System.Drawing.Size(120, 20);
            this.nudValorM2.ThousandsSeparator = true;
            this.nudValorM2.ValueChanged += new System.EventHandler(this.DatosTecnicos_Changed);
            //
            // lblMetros
            //
            this.lblMetros.Location = new System.Drawing.Point(310, 24);
            this.lblMetros.Name = "lblMetros";
            this.lblMetros.Size = new System.Drawing.Size(170, 20);
            this.lblMetros.Text = "M² construibles:";
            this.lblMetros.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMetrosValor
            //
            this.lblMetrosValor.AutoEllipsis = true;
            this.lblMetrosValor.Location = new System.Drawing.Point(486, 24);
            this.lblMetrosValor.Name = "lblMetrosValor";
            this.lblMetrosValor.Size = new System.Drawing.Size(480, 20);
            this.lblMetrosValor.Text = "-";
            this.lblMetrosValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCostoTotal
            //
            this.lblCostoTotal.Location = new System.Drawing.Point(310, 52);
            this.lblCostoTotal.Name = "lblCostoTotal";
            this.lblCostoTotal.Size = new System.Drawing.Size(170, 20);
            this.lblCostoTotal.Text = "Costo total de obra:";
            this.lblCostoTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCostoTotalValor
            //
            this.lblCostoTotalValor.AutoEllipsis = true;
            this.lblCostoTotalValor.Location = new System.Drawing.Point(486, 52);
            this.lblCostoTotalValor.Name = "lblCostoTotalValor";
            this.lblCostoTotalValor.Size = new System.Drawing.Size(480, 20);
            this.lblCostoTotalValor.Text = "-";
            this.lblCostoTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblValorTotal
            //
            this.lblValorTotal.Location = new System.Drawing.Point(310, 80);
            this.lblValorTotal.Name = "lblValorTotal";
            this.lblValorTotal.Size = new System.Drawing.Size(170, 20);
            this.lblValorTotal.Text = "Valor total unidades:";
            this.lblValorTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblValorTotalValor
            //
            this.lblValorTotalValor.AutoEllipsis = true;
            this.lblValorTotalValor.Location = new System.Drawing.Point(486, 80);
            this.lblValorTotalValor.Name = "lblValorTotalValor";
            this.lblValorTotalValor.Size = new System.Drawing.Size(480, 20);
            this.lblValorTotalValor.Text = "-";
            this.lblValorTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMargen
            //
            this.lblMargen.Location = new System.Drawing.Point(310, 108);
            this.lblMargen.Name = "lblMargen";
            this.lblMargen.Size = new System.Drawing.Size(170, 20);
            this.lblMargen.Text = "Diferencia bruta:";
            this.lblMargen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblMargenValor
            //
            this.lblMargenValor.AutoEllipsis = true;
            this.lblMargenValor.Location = new System.Drawing.Point(486, 108);
            this.lblMargenValor.Name = "lblMargenValor";
            this.lblMargenValor.Size = new System.Drawing.Size(480, 20);
            this.lblMargenValor.Text = "-";
            this.lblMargenValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblIcc
            //
            this.lblIcc.Location = new System.Drawing.Point(310, 136);
            this.lblIcc.Name = "lblIcc";
            this.lblIcc.Size = new System.Drawing.Size(170, 20);
            this.lblIcc.Text = "ICC actual (INDEC):";
            this.lblIcc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblIccValor
            //
            this.lblIccValor.AutoEllipsis = true;
            this.lblIccValor.Location = new System.Drawing.Point(486, 136);
            this.lblIccValor.Name = "lblIccValor";
            this.lblIccValor.Size = new System.Drawing.Size(480, 20);
            this.lblIccValor.Text = "-";
            this.lblIccValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(818, 578);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(170, 34);
            this.btnGuardar.Text = "Guardar datos técnicos";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnConsultarCaba
            //
            this.btnConsultarCaba.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(75)))), ((int)(((byte)(160)))));
            this.btnConsultarCaba.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConsultarCaba.ForeColor = System.Drawing.Color.White;
            this.btnConsultarCaba.Location = new System.Drawing.Point(638, 578);
            this.btnConsultarCaba.Name = "btnConsultarCaba";
            this.btnConsultarCaba.Size = new System.Drawing.Size(170, 34);
            this.btnConsultarCaba.Text = "Consultar CABA";
            this.btnConsultarCaba.UseVisualStyleBackColor = false;
            this.btnConsultarCaba.Click += new System.EventHandler(this.btnConsultarCaba_Click);
            //
            // lblParcela
            //
            this.lblParcela.AutoEllipsis = true;
            this.lblParcela.BackColor = System.Drawing.Color.Transparent;
            this.lblParcela.Location = new System.Drawing.Point(12, 578);
            this.lblParcela.Name = "lblParcela";
            this.lblParcela.Size = new System.Drawing.Size(616, 34);
            this.lblParcela.Text = "";
            this.lblParcela.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FormEvaluacionArquitectonica
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TpIngSoft.Properties.Resources.mesh_bg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 620);
            this.Controls.Add(this.dgvProyectos);
            this.Controls.Add(this.grpInmueble);
            this.Controls.Add(this.grpPropietario);
            this.Controls.Add(this.grpDatosTecnicos);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnConsultarCaba);
            this.Controls.Add(this.lblParcela);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1016, 659);
            this.Name = "FormEvaluacionArquitectonica";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Evaluación Arquitectónica";
            this.Load += new System.EventHandler(this.FormEvaluacionArquitectonica_Load);
            this.grpInmueble.ResumeLayout(false);
            this.grpInmueble.PerformLayout();
            this.grpPropietario.ResumeLayout(false);
            this.grpPropietario.PerformLayout();
            this.grpDatosTecnicos.ResumeLayout(false);
            this.grpDatosTecnicos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSupEdificable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAltura)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCostoM2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValorM2)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dgvProyectos;
        private System.Windows.Forms.GroupBox grpInmueble;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.Label lblDireccionValor;
        private System.Windows.Forms.Label lblLocalidad;
        private System.Windows.Forms.Label lblLocalidadValor;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.Label lblTipoValor;
        private System.Windows.Forms.Label lblSuperficie;
        private System.Windows.Forms.Label lblSuperficieValor;
        private System.Windows.Forms.Label lblCanje;
        private System.Windows.Forms.Label lblCanjeValor;
        private System.Windows.Forms.GroupBox grpPropietario;
        private System.Windows.Forms.Label lblPropietario;
        private System.Windows.Forms.Label lblPropietarioValor;
        private System.Windows.Forms.Label lblDni;
        private System.Windows.Forms.Label lblDniValor;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.Label lblTelefonoValor;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblEmailValor;
        private System.Windows.Forms.GroupBox grpDatosTecnicos;
        private System.Windows.Forms.Label lblLote;
        private System.Windows.Forms.NumericUpDown nudLote;
        private System.Windows.Forms.Label lblSupEdificable;
        private System.Windows.Forms.NumericUpDown nudSupEdificable;
        private System.Windows.Forms.Label lblConstruiblesTotales;
        private System.Windows.Forms.Label lblAltura;
        private System.Windows.Forms.NumericUpDown nudAltura;
        private System.Windows.Forms.Label lblCostoM2;
        private System.Windows.Forms.NumericUpDown nudCostoM2;
        private System.Windows.Forms.Label lblValorM2;
        private System.Windows.Forms.NumericUpDown nudValorM2;
        private System.Windows.Forms.Label lblMetros;
        private System.Windows.Forms.Label lblMetrosValor;
        private System.Windows.Forms.Label lblCostoTotal;
        private System.Windows.Forms.Label lblCostoTotalValor;
        private System.Windows.Forms.Label lblValorTotal;
        private System.Windows.Forms.Label lblValorTotalValor;
        private System.Windows.Forms.Label lblMargen;
        private System.Windows.Forms.Label lblMargenValor;
        private System.Windows.Forms.Label lblIcc;
        private System.Windows.Forms.Label lblIccValor;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnConsultarCaba;
        private System.Windows.Forms.Label lblParcela;
    }
}
