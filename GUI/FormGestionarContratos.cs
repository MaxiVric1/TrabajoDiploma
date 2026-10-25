using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormGestionarContratos : Form, IObservador
    {
        // Filas de las grillas (aplanan contrato + inquilino + inmueble). Las propiedades [Browsable(false)] no son columnas.
        private class FilaContrato
        {
            public int NroContrato { get; set; }
            public string Inquilino { get; set; }
            public string Direccion { get; set; }
            public string Tipo { get; set; }
            public decimal MontoMensual { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public string Estado { get; set; }
            public bool AlertaVencimiento { get; set; }
            [Browsable(false)] public Contrato Contrato { get; set; }
            [Browsable(false)] public bool ConDeuda { get; set; }
        }

        private class FilaPago
        {
            public int Mes { get; set; }
            public int Anio { get; set; }
            public string Estado { get; set; }
            public decimal MontoPagado { get; set; }
            public decimal Recargo { get; set; }
            public DateTime? FechaPago { get; set; }
            [Browsable(false)] public EstadoPago EstadoPago { get; set; }
        }

        private ContratoBLL contratoBLL = new ContratoBLL();
        private PagoAlquilerBLL pagoBLL = new PagoAlquilerBLL();
        private InquilinoBLL inquilinoBLL = new InquilinoBLL();
        private GaranteBLL garanteBLL = new GaranteBLL();
        private ToolTip toolTip = new ToolTip(); // detalle de todos los garantes sobre lblGaranteValor
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private List<DataGridTraducible> _grillasTraducibles = new List<DataGridTraducible>();
        private bool _cargando; // evita refrescar el detalle mientras se rearma la grilla

        public FormGestionarContratos()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormGestionarContratos"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFiltroEstado, BE.NombreControl.FormGestionarContratos_lblFiltroEstado));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnActualizar, BE.NombreControl.FormGestionarContratos_btnActualizar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpInquilino, BE.NombreControl.FormGestionarContratos_grpInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNombre, BE.NombreControl.FormGestionarContratos_lblNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblApellido, BE.NombreControl.FormGestionarContratos_lblApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDni, BE.NombreControl.FormGestionarContratos_lblDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTelefono, BE.NombreControl.FormGestionarContratos_lblTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEmail, BE.NombreControl.FormGestionarContratos_lblEmail));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpCondiciones, BE.NombreControl.FormGestionarContratos_grpCondiciones));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDeposito, BE.NombreControl.FormGestionarContratos_lblDeposito));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblComision, BE.NombreControl.FormGestionarContratos_lblComision));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMetodoPago, BE.NombreControl.FormGestionarContratos_lblMetodoPago));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblIndice, BE.NombreControl.FormGestionarContratos_lblIndice));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblGarante, BE.NombreControl.FormGestionarContratos_lblGarante));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpRescision, BE.NombreControl.FormGestionarContratos_grpRescision));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblIniciadaPor, BE.NombreControl.FormGestionarContratos_lblIniciadaPor));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMesesRestantes, BE.NombreControl.FormGestionarContratos_lblMesesRestantes));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblPenalidad, BE.NombreControl.FormGestionarContratos_lblPenalidad));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRescindir, BE.NombreControl.FormGestionarContratos_btnRescindir));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpRenovacion, BE.NombreControl.FormGestionarContratos_grpRenovacion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMesesExtension, BE.NombreControl.FormGestionarContratos_lblMesesExtension));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNuevoMonto, BE.NombreControl.FormGestionarContratos_lblNuevoMonto));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNuevoVencimiento, BE.NombreControl.FormGestionarContratos_lblNuevoVencimiento));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRenovar, BE.NombreControl.FormGestionarContratos_btnRenovar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpPagos, BE.NombreControl.FormGestionarContratos_grpPagos));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMesActual, BE.NombreControl.FormGestionarContratos_lblMesActual));

            _grillasTraducibles.Clear();
            _grillasTraducibles.Add(new DataGridTraducible(dgvContratos, "FormGestionarContratos.dgvContratos")
                .ConColumna("NroContrato", BE.NombreControl.FormGestionarContratos_dgvContratos_nro)
                .ConColumna("Inquilino", BE.NombreControl.FormGestionarContratos_dgvContratos_inquilino)
                .ConColumna("Direccion", BE.NombreControl.FormGestionarContratos_dgvContratos_direccion)
                .ConColumna("Tipo", BE.NombreControl.FormGestionarContratos_dgvContratos_tipo)
                .ConColumna("MontoMensual", BE.NombreControl.FormGestionarContratos_dgvContratos_monto)
                .ConColumna("FechaInicio", BE.NombreControl.FormGestionarContratos_dgvContratos_fechaInicio)
                .ConColumna("FechaFin", BE.NombreControl.FormGestionarContratos_dgvContratos_fechaFin)
                .ConColumna("Estado", BE.NombreControl.FormGestionarContratos_dgvContratos_estado)
                .ConColumna("AlertaVencimiento", BE.NombreControl.FormGestionarContratos_dgvContratos_alerta));
            _grillasTraducibles.Add(new DataGridTraducible(dgvPagos, "FormGestionarContratos.dgvPagos")
                .ConColumna("Mes", BE.NombreControl.FormGestionarContratos_dgvPagos_mes)
                .ConColumna("Anio", BE.NombreControl.FormGestionarContratos_dgvPagos_anio)
                .ConColumna("Estado", BE.NombreControl.FormGestionarContratos_dgvPagos_estado)
                .ConColumna("MontoPagado", BE.NombreControl.FormGestionarContratos_dgvPagos_monto)
                .ConColumna("Recargo", BE.NombreControl.FormGestionarContratos_dgvPagos_recargo)
                .ConColumna("FechaPago", BE.NombreControl.FormGestionarContratos_dgvPagos_fechaPago));
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
            foreach (var grilla in _grillasTraducibles)
            {
                grilla.Traducir(traducciones);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void TraducirGrillas()
        {
            var traducciones = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
            if (traducciones == null) return;
            foreach (var grilla in _grillasTraducibles)
            {
                grilla.Traducir(traducciones);
            }
        }

        private void FormGestionarContratos_Load(object sender, EventArgs e)
        {
            cmbFiltroEstado.Items.Clear();
            cmbFiltroEstado.Items.AddRange(new object[] { "Todos", EstadosPN2.Texto(EstadoContrato.Activo),
                EstadosPN2.Texto(EstadoContrato.PorVencer), EstadosPN2.Texto(EstadoContrato.Finalizado) });
            cmbIniciadaPor.Items.Clear();
            foreach (IniciadorRescision iniciador in Enum.GetValues(typeof(IniciadorRescision)))
            {
                cmbIniciadaPor.Items.Add(iniciador);
            }
            cmbIniciadaPor.SelectedIndex = 0;

            ActualizarEstados();
            cmbFiltroEstado.SelectedIndex = 0; // dispara la primera carga
        }

        private void ActualizarEstados()
        {
            lblActualizacion.Text = EstadosPN2.EjecutarProcesosAutomaticos();
        }

        private void cmbFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarContratos();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            int? seleccionado = ContratoSeleccionado()?.IdContrato;
            ActualizarEstados();
            CargarContratos(seleccionado);
        }

        private EstadoContrato? FiltroSeleccionado()
        {
            switch (cmbFiltroEstado.SelectedIndex)
            {
                case 1: return EstadoContrato.Activo;
                case 2: return EstadoContrato.PorVencer;
                case 3: return EstadoContrato.Finalizado;
                default: return null;
            }
        }

        private void CargarContratos(int? idASeleccionar = null)
        {
            _cargando = true;
            try
            {
                // Contratos con algún mes En Mora o En Litigio (una sola lectura de pagos)
                HashSet<int> conDeuda = new HashSet<int>(pagoBLL.LeerTodos()
                    .Where(p => p.EstadoPago == EstadoPago.EnMora || p.EstadoPago == EstadoPago.EnLitigio)
                    .Select(p => p.IdContrato));

                List<FilaContrato> filas = contratoBLL.LeerDetalle(FiltroSeleccionado()).Select(c => new FilaContrato
                {
                    NroContrato = c.IdContrato,
                    Inquilino = c.Inquilino?.NombreCompleto,
                    Direccion = c.InmuebleAlquiler?.Direccion,
                    Tipo = c.InmuebleAlquiler?.TipoInmueble.ToString(),
                    MontoMensual = c.MontoMensual,
                    FechaInicio = c.FechaInicio,
                    FechaFin = c.FechaFin,
                    Estado = EstadosPN2.Texto(c.EstadoContrato),
                    AlertaVencimiento = c.AlertaVencimiento,
                    Contrato = c,
                    ConDeuda = conDeuda.Contains(c.IdContrato)
                }).ToList();

                dgvContratos.DataSource = null;
                dgvContratos.DataSource = filas;

                dgvContratos.Columns["MontoMensual"].DefaultCellStyle.Format = "N2";
                dgvContratos.Columns["MontoMensual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvContratos.Columns["FechaInicio"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvContratos.Columns["FechaFin"].DefaultCellStyle.Format = "dd/MM/yyyy";
                TraducirGrillas();

                foreach (DataGridViewRow row in dgvContratos.Rows)
                {
                    FilaContrato fila = (FilaContrato)row.DataBoundItem;
                    EstadosPN2.Resaltar(row, fila.ConDeuda, fila.Contrato.EstadoContrato == EstadoContrato.PorVencer);
                    if (idASeleccionar.HasValue && fila.NroContrato == idASeleccionar.Value)
                    {
                        row.Selected = true;
                        dgvContratos.CurrentCell = row.Cells["NroContrato"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los contratos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
            }

            MostrarDetalle();
        }

        private void dgvContratos_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarDetalle();
        }

        private Contrato ContratoSeleccionado()
        {
            return (dgvContratos.CurrentRow?.DataBoundItem as FilaContrato)?.Contrato;
        }

        private void MostrarDetalle()
        {
            Contrato contrato = ContratoSeleccionado();
            lblNombreValor.Text = lblApellidoValor.Text = lblDniValor.Text = lblTelefonoValor.Text = lblEmailValor.Text = "-";
            lblMesActualValor.Text = "-";
            lblDepositoValor.Text = lblComisionValor.Text = lblMetodoPagoValor.Text = lblIndiceValor.Text = lblGaranteValor.Text = "-";
            lblMesesRestantesValor.Text = lblPenalidadValor.Text = "-";
            toolTip.SetToolTip(lblGaranteValor, null);
            dgvPagos.DataSource = null;

            // Renovar y rescindir solo para contratos Activo o Por Vencer
            grpRenovacion.Enabled = grpRescision.Enabled = contrato != null && contrato.Vigente;
            if (contrato == null)
            {
                lblNuevoVencimientoValor.Text = "-";
                return;
            }
            nudNuevoMonto.Value = Math.Min(contrato.MontoMensual, nudNuevoMonto.Maximum);
            ActualizarNuevoVencimiento();
            MostrarCondiciones(contrato);

            try
            {
                Inquilino inquilino = inquilinoBLL.BuscarPorId(contrato.IdInquilino);
                if (inquilino != null)
                {
                    lblNombreValor.Text = inquilino.Nombre;
                    lblApellidoValor.Text = inquilino.Apellido;
                    lblDniValor.Text = inquilino.Dni;
                    lblTelefonoValor.Text = string.IsNullOrEmpty(inquilino.Telefono) ? "-" : inquilino.Telefono;
                    lblEmailValor.Text = string.IsNullOrEmpty(inquilino.Email) ? "-" : inquilino.Email;
                }

                List<PagoAlquiler> pagos = pagoBLL.LeerPorContrato(contrato.IdContrato);
                dgvPagos.DataSource = pagos.Select(p => new FilaPago
                {
                    Mes = p.MesCorrespondiente,
                    Anio = p.AnioCorrespondiente,
                    Estado = EstadosPN2.Texto(p.EstadoPago),
                    MontoPagado = p.MontoPagado,
                    Recargo = p.RecargoAplicado,
                    FechaPago = p.FechaPago,
                    EstadoPago = p.EstadoPago
                }).ToList();
                dgvPagos.Columns["MontoPagado"].DefaultCellStyle.Format = "N2";
                dgvPagos.Columns["MontoPagado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPagos.Columns["Recargo"].DefaultCellStyle.Format = "N2";
                dgvPagos.Columns["Recargo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPagos.Columns["FechaPago"].DefaultCellStyle.Format = "dd/MM/yyyy";
                TraducirGrillas();
                foreach (DataGridViewRow row in dgvPagos.Rows)
                {
                    EstadoPago estado = ((FilaPago)row.DataBoundItem).EstadoPago;
                    EstadosPN2.Resaltar(row, estado != EstadoPago.AlDia, false);
                }

                lblMesActualValor.Text = EstadosPN2.DescribirMesActual(contrato, pagos, DateTime.Now);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle del contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Condiciones económicas, garantes y rescisión (penalidad calculada a hoy, o la ya aplicada)
        private void MostrarCondiciones(Contrato contrato)
        {
            lblDepositoValor.Text = $"${contrato.DepositoGarantia:N2}" +
                (contrato.MontoDevolucionDeposito.HasValue ? $" (devuelto ${contrato.MontoDevolucionDeposito:N2})" : "");
            lblComisionValor.Text = $"{contrato.ComisionInmobiliaria:0.##}% de cada pago " +
                                    $"(${ContratoBLL.CalcularComision(contrato.MontoMensual, contrato.ComisionInmobiliaria):N2}/mes)";
            lblMetodoPagoValor.Text = contrato.MetodoPago.ToString();
            lblIndiceValor.Text = $"{contrato.IndiceActualizacion} cada {contrato.PeriodicidadActualizacionMeses} meses";

            if (contrato.Vigente)
            {
                int meses = ContratoBLL.MesesRestantes(contrato, DateTime.Now);
                lblMesesRestantesValor.Text = $"{meses} mes(es): ${ContratoBLL.CanonRestante(contrato, DateTime.Now):N2}";
                lblPenalidadValor.Text = $"${ContratoBLL.CalcularPenalidadRescision(contrato, DateTime.Now):N2} " +
                                         $"({ContratoBLL.PORCENTAJE_PENALIDAD_RESCISION:P0})";
            }
            else if (contrato.RescisionIniciadaPor.HasValue)
            {
                lblMesesRestantesValor.Text = $"Rescindido por: {contrato.RescisionIniciadaPor}";
                lblPenalidadValor.Text = contrato.PenalidadRescision.HasValue ? $"${contrato.PenalidadRescision:N2}" : "-";
            }

            try
            {
                List<Garante> garantes = garanteBLL.LeerPorContrato(contrato.IdContrato);
                lblGaranteValor.Text = garantes.Count == 0 ? "Sin garantes"
                    : garantes[0].ToString() + (garantes.Count > 1 ? $" y {garantes.Count - 1} más" : "");
                toolTip.SetToolTip(lblGaranteValor, string.Join(Environment.NewLine, garantes.Select(g =>
                    $"{g.NombreCompleto} (DNI {g.Dni}) - {g.Domicilio} - Garantía: {g.PropiedadGarantia}")));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los garantes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRescindir_Click(object sender, EventArgs e)
        {
            Contrato contrato = ContratoSeleccionado();
            if (contrato == null || !(cmbIniciadaPor.SelectedItem is IniciadorRescision iniciador)) return;

            int meses = ContratoBLL.MesesRestantes(contrato, DateTime.Now);
            decimal penalidad = ContratoBLL.CalcularPenalidadRescision(contrato, DateTime.Now);
            string confirmacion =
                $"¿Confirma la rescisión del Contrato N° {contrato.IdContrato}?\n\n" +
                $"Inquilino: {contrato.Inquilino?.NombreCompleto}\n" +
                $"Inmueble: {contrato.InmuebleAlquiler?.Direccion}\n" +
                $"Iniciada por: {iniciador}\n" +
                $"Canon restante: {meses} mes(es) x ${contrato.MontoMensual:N2} = ${ContratoBLL.CanonRestante(contrato, DateTime.Now):N2}\n" +
                $"Penalidad ({ContratoBLL.PORCENTAJE_PENALIDAD_RESCISION:P0}), a cargo del {iniciador.ToString().ToLower()}: ${penalidad:N2}\n\n" +
                "El contrato queda Finalizado y el inmueble vuelve a estar Publicado.";
            if (MessageBox.Show(confirmacion, "Confirmar rescisión", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                decimal aplicada = contratoBLL.Rescindir(contrato.IdContrato, iniciador);
                MessageBox.Show($"Contrato N° {contrato.IdContrato} rescindido. Penalidad: ${aplicada:N2}.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al rescindir el contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CargarContratos(contrato.IdContrato);
        }

        private void nudMesesExtension_ValueChanged(object sender, EventArgs e)
        {
            ActualizarNuevoVencimiento();
        }

        private void ActualizarNuevoVencimiento()
        {
            Contrato contrato = ContratoSeleccionado();
            lblNuevoVencimientoValor.Text = contrato == null
                ? "-"
                : contrato.FechaInicio.AddMonths(contrato.DuracionMeses + (int)nudMesesExtension.Value).ToString("dd/MM/yyyy");
        }

        private void btnRenovar_Click(object sender, EventArgs e)
        {
            Contrato contrato = ContratoSeleccionado();
            if (contrato == null) return;

            int meses = (int)nudMesesExtension.Value;
            decimal nuevoMonto = nudNuevoMonto.Value;
            DateTime nuevoFin = contrato.FechaInicio.AddMonths(contrato.DuracionMeses + meses);
            bool conDeuda = (dgvContratos.CurrentRow?.DataBoundItem as FilaContrato)?.ConDeuda ?? false;

            string confirmacion =
                $"¿Confirma la renovación del Contrato N° {contrato.IdContrato}?\n\n" +
                $"Inquilino: {contrato.Inquilino?.NombreCompleto}\n" +
                $"Vencimiento: {contrato.FechaFin:dd/MM/yyyy} -> {nuevoFin:dd/MM/yyyy} (+{meses} meses)\n" +
                $"Monto mensual: ${contrato.MontoMensual:N2} -> ${nuevoMonto:N2}";
            if (nuevoMonto != contrato.MontoMensual)
                confirmacion += "\n\nEl nuevo monto se aplica a todos los meses que todavía no tienen pago registrado.";
            if (conDeuda)
                confirmacion += "\n\nATENCIÓN: el contrato tiene pagos En Mora o En Litigio.";

            if (MessageBox.Show(confirmacion, "Confirmar renovación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                Contrato renovado = contratoBLL.Renovar(contrato.IdContrato, meses, nuevoMonto);
                MessageBox.Show($"Contrato N° {renovado.IdContrato} renovado con éxito.\nNuevo vencimiento: {renovado.FechaFin:dd/MM/yyyy} " +
                    $"(estado {EstadosPN2.Texto(renovado.EstadoContrato)}).", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarContratos(renovado.IdContrato);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al renovar el contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CargarContratos(contrato.IdContrato);
            }
        }
    }
}
