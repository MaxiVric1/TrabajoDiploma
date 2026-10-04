using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormHistorialUsuario : Form, IObservador
    {
        private UsuarioBLL usuarioBLL = new UsuarioBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _dgvTraducible;

        public FormHistorialUsuario()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormHistorialUsuario"));
            _controlesTraducibles.Add(new EtiquetaTraducible(label1, BE.NombreControl.FormHistorialUsuario_label1));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCargarHistorial, BE.NombreControl.FormHistorialUsuario_btnCargarHistorial));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnRestaurar, BE.NombreControl.FormHistorialUsuario_btnRestaurar));

            _dgvTraducible = new DataGridTraducible(dgvHistorial, "FormHistorialUsuario.dgvHistorial")
                .ConColumna("IdHistorial", BE.NombreControl.FormHistorialUsuario_dgvHistorial_idHistorial)
                .ConColumna("IdUsuario", BE.NombreControl.FormHistorialUsuario_dgvHistorial_idUsuario)
                .ConColumna("Nombre", BE.NombreControl.FormHistorialUsuario_dgvHistorial_usuario)
                .ConColumna("NombrePersona", BE.NombreControl.FormHistorialUsuario_dgvHistorial_nombrePersona)
                .ConColumna("Apellido", BE.NombreControl.FormHistorialUsuario_dgvHistorial_apellido)
                .ConColumna("Activo", BE.NombreControl.FormHistorialUsuario_dgvHistorial_estado)
                .ConColumna("FechaCambio", BE.NombreControl.FormHistorialUsuario_dgvHistorial_fecha)
                .ConColumna("EditorNombre", BE.NombreControl.FormHistorialUsuario_dgvHistorial_editorNombre);
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

        private void FormHistorialUsuario_Load(object sender, EventArgs e)
        {
            RefrescarUsuarios();
        }

        private void RefrescarUsuarios()
        {
            cmbUsuarios.DataSource = null;
            cmbUsuarios.DisplayMember = "Nombre";
            cmbUsuarios.ValueMember = "Id";
            cmbUsuarios.DataSource = usuarioBLL.LeerTodos();
        }

        private void btnCargarHistorial_Click(object sender, EventArgs e)
        {
            if (cmbUsuarios.SelectedValue != null)
            {
                int idUsuario = (int)cmbUsuarios.SelectedValue;
                dgvHistorial.DataSource = null;
                dgvHistorial.DataSource = usuarioBLL.LeerHistorial(idUsuario);
                
                if (dgvHistorial.Columns["Password"] != null) dgvHistorial.Columns["Password"].Visible = false;
                if (dgvHistorial.Columns["DVH"] != null) dgvHistorial.Columns["DVH"].Visible = false;
                if (dgvHistorial.Columns["IdUsuarioAutor"] != null) dgvHistorial.Columns["IdUsuarioAutor"].Visible = false;

                // Re-apply column translations
                var traducciones = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
                _dgvTraducible.Traducir(traducciones);
            }
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            if (dgvHistorial.CurrentRow != null)
            {
                var historial = (UsuarioHistorial)dgvHistorial.CurrentRow.DataBoundItem;
                
                DialogResult dr = MessageBox.Show($"¿Desea restaurar al usuario '{historial.Nombre}' al estado del {historial.FechaCambio}?\n\nNota: Se restaurarán únicamente los campos descriptivos (Nombre, Apellido) y el estado Activo. Las credenciales de acceso (Nombre de usuario y Contraseña) se mantendrán intactas por seguridad.", 
                    "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        usuarioBLL.Restaurar(historial.IdHistorial);
                        MessageBox.Show("Usuario restaurado con éxito (Campos descriptivos y estado Activo). Se ha generado un nuevo registro en el historial y se han actualizado los DVs.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnCargarHistorial_Click(null, null); // Refrescar grilla
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al restaurar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un registro del historial.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
