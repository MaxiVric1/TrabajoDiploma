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
            dgvPagos.DataSource = null;

            // Renovar solo para contratos Activo o Por Vencer
            grpRenovacion.Enabled = contrato != null && contrato.Vigente;
            if (contrato == null)
            {
                lblNuevoVencimientoValor.Text = "-";
                return;
            }
            nudNuevoMonto.Value = Math.Min(contrato.MontoMensual, nudNuevoMonto.Maximum);
            ActualizarNuevoVencimiento();

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

                lblMesActualValor.Text = DescribirMesActual(contrato, pagos, DateTime.Now);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle del contrato: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Estado del pago del mes en curso, con las mismas reglas que PagoAlquilerBLL
        private string DescribirMesActual(Contrato contrato, List<PagoAlquiler> pagos, DateTime hoy)
        {
            string periodo = $"{hoy.Month:00}/{hoy.Year}";
            PagoAlquiler pago = pagos.FirstOrDefault(p => p.MesCorrespondiente == hoy.Month && p.AnioCorrespondiente == hoy.Year);
            if (pago != null)
            {
                if (pago.Pagado)
                    return $"{periodo}: Al día — pagado el {pago.FechaPago:dd/MM/yyyy} (${pago.MontoPagado:N2})";
                return $"{periodo}: {EstadosPN2.Texto(pago.EstadoPago)} — sin pago (recargo ${pago.RecargoAplicado:N2})";
            }

            if (!PagoAlquilerBLL.PeriodosDelContrato(contrato).Contains((hoy.Month, hoy.Year)))
                return $"{periodo}: no corresponde al período del contrato";

            EstadoPago? estado = PagoAlquilerBLL.EstadoSegunFecha(hoy.Month, hoy.Year, hoy);
            return estado == null
                ? $"{periodo}: pendiente — vence el {PagoAlquilerBLL.FechaVencimiento(hoy.Month, hoy.Year):dd/MM/yyyy}"
                : $"{periodo}: {EstadosPN2.Texto(estado.Value)} — sin pago";
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
