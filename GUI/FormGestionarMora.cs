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
    public partial class FormGestionarMora : Form, IObservador
    {
        // Filas de las grillas. Las propiedades [Browsable(false)] no son columnas.
        private class FilaMora
        {
            public int NroContrato { get; set; }
            public string Inquilino { get; set; }
            public string Direccion { get; set; }
            public string Periodo { get; set; }
            public decimal MontoAdeudado { get; set; }
            public int DiasEnMora { get; set; }
            [Browsable(false)] public PagoAlquiler Pago { get; set; }
        }

        private class FilaContacto
        {
            public DateTime Fecha { get; set; }
            public string Descripcion { get; set; }
            public string Analista { get; set; }
        }

        private PagoAlquilerBLL pagoBLL = new PagoAlquilerBLL();
        private ContratoBLL contratoBLL = new ContratoBLL();
        private InquilinoBLL inquilinoBLL = new InquilinoBLL();
        private ContactoCobranzaBLL contactoBLL = new ContactoCobranzaBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private List<DataGridTraducible> _grillasTraducibles = new List<DataGridTraducible>();
        private bool _cargando; // evita refrescar el detalle mientras se rearma la grilla

        public FormGestionarMora()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormGestionarMora"));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpInquilino, BE.NombreControl.FormGestionarMora_grpInquilino));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNombre, BE.NombreControl.FormGestionarMora_lblNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblApellido, BE.NombreControl.FormGestionarMora_lblApellido));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDni, BE.NombreControl.FormGestionarMora_lblDni));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblTelefono, BE.NombreControl.FormGestionarMora_lblTelefono));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblEmail, BE.NombreControl.FormGestionarMora_lblEmail));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpContrato, BE.NombreControl.FormGestionarMora_grpContrato));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDireccion, BE.NombreControl.FormGestionarMora_lblDireccion));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMontoMensual, BE.NombreControl.FormGestionarMora_lblMontoMensual));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaFin, BE.NombreControl.FormGestionarMora_lblFechaFin));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDiasMora, BE.NombreControl.FormGestionarMora_lblDiasMora));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblMontoAdeudado, BE.NombreControl.FormGestionarMora_lblMontoAdeudado));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnEscalar, BE.NombreControl.FormGestionarMora_btnEscalar));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpContactos, BE.NombreControl.FormGestionarMora_grpContactos));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFechaContacto, BE.NombreControl.FormGestionarMora_lblFechaContacto));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDescripcion, BE.NombreControl.FormGestionarMora_lblDescripcion));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRegistrarContacto, BE.NombreControl.FormGestionarMora_btnRegistrarContacto));

            _grillasTraducibles.Clear();
            _grillasTraducibles.Add(new DataGridTraducible(dgvPagos, "FormGestionarMora.dgvPagos")
                .ConColumna("NroContrato", BE.NombreControl.FormGestionarMora_dgvPagos_nro)
                .ConColumna("Inquilino", BE.NombreControl.FormGestionarMora_dgvPagos_inquilino)
                .ConColumna("Direccion", BE.NombreControl.FormGestionarMora_dgvPagos_direccion)
                .ConColumna("Periodo", BE.NombreControl.FormGestionarMora_dgvPagos_periodo)
                .ConColumna("MontoAdeudado", BE.NombreControl.FormGestionarMora_dgvPagos_montoAdeudado)
                .ConColumna("DiasEnMora", BE.NombreControl.FormGestionarMora_dgvPagos_diasMora));
            _grillasTraducibles.Add(new DataGridTraducible(dgvContactos, "FormGestionarMora.dgvContactos")
                .ConColumna("Fecha", BE.NombreControl.FormGestionarMora_dgvContactos_fecha)
                .ConColumna("Descripcion", BE.NombreControl.FormGestionarMora_dgvContactos_descripcion)
                .ConColumna("Analista", BE.NombreControl.FormGestionarMora_dgvContactos_analista));
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

        private void FormGestionarMora_Load(object sender, EventArgs e)
        {
            dtpFechaContacto.Value = DateTime.Today;
            dtpFechaContacto.MaxDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            lblActualizacion.Text = EstadosPN2.EjecutarProcesosAutomaticos();
            CargarPagos();
        }

        // Pagos En Mora, de los más atrasados a los más recientes
        private void CargarPagos(int? idPagoASeleccionar = null)
        {
            _cargando = true;
            try
            {
                DateTime hoy = DateTime.Now;
                List<FilaMora> filas = pagoBLL.LeerDetalle(EstadoPago.EnMora).Select(p => new FilaMora
                {
                    NroContrato = p.IdContrato,
                    Inquilino = p.Contrato?.Inquilino?.NombreCompleto,
                    Direccion = p.Contrato?.InmuebleAlquiler?.Direccion,
                    Periodo = p.ToString(),
                    MontoAdeudado = (p.Contrato?.MontoMensual ?? 0) + p.RecargoAplicado,
                    DiasEnMora = PagoAlquilerBLL.DiasEnMora(p.MesCorrespondiente, p.AnioCorrespondiente, hoy),
                    Pago = p
                }).OrderByDescending(f => f.DiasEnMora).ThenBy(f => f.NroContrato).ToList();

                dgvPagos.DataSource = null;
                dgvPagos.DataSource = filas;
                dgvPagos.Columns["MontoAdeudado"].DefaultCellStyle.Format = "N2";
                dgvPagos.Columns["MontoAdeudado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPagos.Columns["DiasEnMora"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                TraducirGrillas();

                if (idPagoASeleccionar.HasValue)
                {
                    foreach (DataGridViewRow row in dgvPagos.Rows)
                    {
                        if (((FilaMora)row.DataBoundItem).Pago.IdPago == idPagoASeleccionar.Value)
                        {
                            row.Selected = true;
                            dgvPagos.CurrentCell = row.Cells["NroContrato"];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los pagos en mora: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargando = false;
            }

            MostrarDetalle();
        }

        private void dgvPagos_SelectionChanged(object sender, EventArgs e)
        {
            if (!_cargando) MostrarDetalle();
        }

        private FilaMora FilaSeleccionada() => dgvPagos.CurrentRow?.DataBoundItem as FilaMora;

        private void MostrarDetalle()
        {
            FilaMora fila = FilaSeleccionada();
            lblNombreValor.Text = lblApellidoValor.Text = lblDniValor.Text = lblTelefonoValor.Text = lblEmailValor.Text = "-";
            lblDireccionValor.Text = lblMontoMensualValor.Text = lblFechaFinValor.Text = lblDiasMoraValor.Text = lblMontoAdeudadoValor.Text = "-";
            dgvContactos.DataSource = null;
            grpContrato.Enabled = grpContactos.Enabled = fila != null;
            if (fila == null) return;

            PagoAlquiler pago = fila.Pago;
            try
            {
                Contrato contrato = contratoBLL.BuscarPorId(pago.IdContrato);
                Inquilino inquilino = contrato != null ? inquilinoBLL.BuscarPorId(contrato.IdInquilino) : null;
                if (inquilino != null)
                {
                    lblNombreValor.Text = inquilino.Nombre;
                    lblApellidoValor.Text = inquilino.Apellido;
                    lblDniValor.Text = inquilino.Dni;
                    lblTelefonoValor.Text = string.IsNullOrEmpty(inquilino.Telefono) ? "-" : inquilino.Telefono;
                    lblEmailValor.Text = string.IsNullOrEmpty(inquilino.Email) ? "-" : inquilino.Email;
                }

                lblDireccionValor.Text = fila.Direccion ?? "-";
                if (contrato != null)
                {
                    lblMontoMensualValor.Text = $"${contrato.MontoMensual:N2}";
                    lblFechaFinValor.Text = contrato.FechaFin.ToString("dd/MM/yyyy");
                }
                lblDiasMoraValor.Text = $"{fila.DiasEnMora} (venció el {PagoAlquilerBLL.FechaVencimiento(pago.MesCorrespondiente, pago.AnioCorrespondiente):dd/MM/yyyy})";
                lblMontoAdeudadoValor.Text = $"${fila.MontoAdeudado:N2} (recargo ${pago.RecargoAplicado:N2})";

                CargarContactos(pago.IdContrato);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarContactos(int idContrato)
        {
            dgvContactos.DataSource = contactoBLL.LeerPorContrato(idContrato).Select(c => new FilaContacto
            {
                Fecha = c.FechaContacto,
                Descripcion = c.Descripcion,
                Analista = c.Analista
            }).ToList();
            dgvContactos.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvContactos.Columns["Fecha"].FillWeight = 20;
            dgvContactos.Columns["Analista"].FillWeight = 20;
            TraducirGrillas();
        }

        private void btnRegistrarContacto_Click(object sender, EventArgs e)
        {
            FilaMora fila = FilaSeleccionada();
            if (fila == null) return;

            try
            {
                ContactoCobranza contacto = new ContactoCobranza
                {
                    FechaContacto = dtpFechaContacto.Value.Date == DateTime.Today ? DateTime.Now : dtpFechaContacto.Value.Date,
                    Descripcion = txtDescripcion.Text,
                    IdContrato = fila.NroContrato
                };
                contactoBLL.Registrar(contacto);

                MessageBox.Show($"Contacto registrado para el contrato N° {fila.NroContrato}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDescripcion.Clear();
                dtpFechaContacto.Value = DateTime.Today;
                CargarContactos(fila.NroContrato);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el contacto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEscalar_Click(object sender, EventArgs e)
        {
            FilaMora fila = FilaSeleccionada();
            if (fila == null) return;

            DialogResult res = MessageBox.Show(
                $"¿Confirma escalar a legal el período {fila.Periodo} del contrato N° {fila.NroContrato}?\n\n" +
                $"Inquilino: {fila.Inquilino}\nDeuda: ${fila.MontoAdeudado:N2} ({fila.DiasEnMora} días en mora)\n\n" +
                "El pago pasará de En Mora a En Litigio y quedará a cargo del área legal.",
                "Confirmar escalamiento", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes) return;

            try
            {
                pagoBLL.EscalarALitigio(fila.Pago.IdPago);
                MessageBox.Show($"Período {fila.Periodo} del contrato N° {fila.NroContrato} escalado a legal (En Litigio).",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPagos(); // el pago deja de estar En Mora
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CargarPagos(fila.Pago.IdPago);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al escalar el pago: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
