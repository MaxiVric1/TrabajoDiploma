using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL;
using BE;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormBitacora : Form, IObservador
    {
        private GestorBitacora gestor = GestorBitacora.Instance;
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _dgvTraducible;

        public FormBitacora()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormBitacora"));
            _controlesTraducibles.Add(new EtiquetaTraducible(labelDesde, BE.NombreControl.FormBitacora_labelDesde));
            _controlesTraducibles.Add(new EtiquetaTraducible(labelHasta, BE.NombreControl.FormBitacora_labelHasta));
            _controlesTraducibles.Add(new EtiquetaTraducible(labelActividad, BE.NombreControl.FormBitacora_labelActividad));
            _controlesTraducibles.Add(new EtiquetaTraducible(labelUsuario, BE.NombreControl.FormBitacora_labelUsuario));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnBuscar, BE.NombreControl.FormBitacora_btnBuscar));

            _dgvTraducible = new DataGridTraducible(dgvBitacora, "FormBitacora.dgvBitacora")
                .ConColumna("Id", BE.NombreControl.FormBitacora_dgvBitacora_id)
                .ConColumna("FechaHora", BE.NombreControl.FormBitacora_dgvBitacora_fecha)
                .ConColumna("NombreUsuario", BE.NombreControl.FormBitacora_dgvBitacora_usuario)
                .ConColumna("Actividad", BE.NombreControl.FormBitacora_dgvBitacora_descripcion)
                .ConColumna("InfoAsociada", BE.NombreControl.FormBitacora_dgvBitacora_criticidad);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
            _dgvTraducible.Traducir(traducciones);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormBitacora_Load(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Now.AddDays(-7);
            dtpHasta.Value = DateTime.Now;
            CargarDatos();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                string filtroActividad = string.IsNullOrWhiteSpace(txtActividad.Text) ? null : txtActividad.Text;
                string filtroUsuario = string.IsNullOrWhiteSpace(txtUsuario.Text) ? null : txtUsuario.Text;
                
                DateTime desde = dtpDesde.Value.Date;
                DateTime hasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);

                List<Bitacora> lista = gestor.BuscarEventos(null, filtroActividad, desde, hasta);
                
                if (!string.IsNullOrEmpty(filtroUsuario))
                {
                    lista = lista.FindAll(x => x.NombreUsuario.ToLower().Contains(filtroUsuario.ToLower()));
                }

                dgvBitacora.DataSource = null;
                dgvBitacora.DataSource = lista;
                
                if (dgvBitacora.Columns["Usuario"] != null) dgvBitacora.Columns["Usuario"].Visible = false;
                if (dgvBitacora.Columns["Id"] != null) dgvBitacora.Columns["Id"].Width = 50;
                if (dgvBitacora.Columns["FechaHora"] != null) dgvBitacora.Columns["FechaHora"].Width = 120;
                if (dgvBitacora.Columns["NombreUsuario"] != null) dgvBitacora.Columns["NombreUsuario"].Width = 100;
                if (dgvBitacora.Columns["Actividad"] != null) dgvBitacora.Columns["Actividad"].Width = 150;
                if (dgvBitacora.Columns["InfoAsociada"] != null) dgvBitacora.Columns["InfoAsociada"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // Re-apply translation to grid columns after populating data
                var traducciones = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
                _dgvTraducible.Traducir(traducciones);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar bitácora: " + ex.Message);
            }
        }
    }
}
