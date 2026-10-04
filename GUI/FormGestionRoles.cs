using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;
using BLL;

using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormGestionRoles : Form, IObservador
    {
        private RolBLL rolBLL = new RolBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();

        public FormGestionRoles()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormGestionRoles"));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpAcciones, BE.NombreControl.FormGestionRoles_grpAcciones));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblDetalle, BE.NombreControl.FormGestionRoles_lblDetalle));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblModo, BE.NombreControl.FormGestionRoles_lblModo));
            _controlesTraducibles.Add(new EtiquetaTraducible(rbModoCrear, BE.NombreControl.FormGestionRoles_rbModoCrear));
            _controlesTraducibles.Add(new EtiquetaTraducible(rbModoEditar, BE.NombreControl.FormGestionRoles_rbModoEditar));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNombreRol, BE.NombreControl.FormGestionRoles_lblNombreRol));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCrearRolRaiz, BE.NombreControl.FormGestionRoles_btnCrearRolRaiz));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCrearSubRol, BE.NombreControl.FormGestionRoles_btnCrearSubRol));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnEditarNombre, BE.NombreControl.FormGestionRoles_btnEditarNombre));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnEliminarRol, BE.NombreControl.FormGestionRoles_btnEliminarRol));
            _controlesTraducibles.Add(new EtiquetaTraducible(grpAsignarPermiso, BE.NombreControl.FormGestionRoles_grpAsignarPermiso));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnAsignarPermiso, BE.NombreControl.FormGestionRoles_btnAsignarPermiso));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnQuitarItem, BE.NombreControl.FormGestionRoles_btnQuitarItem));
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormGestionRoles_Load(object sender, EventArgs e)
        {
            CargarTreeView();
            ActualizarControles(null);
        }

        private void CargarTreeView()
        {
            try
            {
                tvPerfiles.Nodes.Clear();
                List<Rol> roles = rolBLL.ObtenerTodos();

                foreach (var rol in roles)
                {
                    TreeNode rootNode = new TreeNode(rol.ObtenerDescripcion()) { Tag = rol };
                    tvPerfiles.Nodes.Add(rootNode);

                    foreach (var hijo in rol.Permisos)
                    {
                        CargarNodosRecursivo(hijo, rootNode);
                    }
                }

                tvPerfiles.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los perfiles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarNodosRecursivo(IComponentePerfil nodo, TreeNode nodoPadre)
        {
            TreeNode childNode = new TreeNode(nodo.ObtenerDescripcion()) { Tag = nodo };
            nodoPadre.Nodes.Add(childNode);

            if (nodo is Rol rol)
            {
                foreach (var hijo in rol.Permisos)
                {
                    CargarNodosRecursivo(hijo, childNode);
                }
            }
        }

        private void tvPerfiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                IComponentePerfil seleccionado = e.Node.Tag as IComponentePerfil;
                ActualizarControles(seleccionado);
            }
            else
            {
                ActualizarControles(null);
            }
        }

        private void rbModo_CheckedChanged(object sender, EventArgs e)
        {
            if (rbModoCrear.Checked)
            {
                txtNombreRol.Text = "";
            }
            IComponentePerfil seleccionado = tvPerfiles.SelectedNode?.Tag as IComponentePerfil;
            ActualizarControles(seleccionado);
        }

        private void ActualizarControles(IComponentePerfil seleccionado)
        {
            if (rbModoCrear.Checked)
            {
                // Visibilidad de botones en modo Crear
                btnCrearRolRaiz.Visible = true;
                btnCrearSubRol.Visible = true;
                btnEditarNombre.Visible = false;
                btnEliminarRol.Visible = false;
                grpAsignarPermiso.Visible = false;
                btnQuitarItem.Visible = false;

                // Controles habilitados en modo Crear
                txtNombreRol.Enabled = true;
                btnCrearRolRaiz.Enabled = true;
                btnCrearSubRol.Enabled = (seleccionado is Rol);

                if (seleccionado == null)
                {
                    lblDetalle.Text = "Detalle: Selección vacía";
                }
                else
                {
                    lblDetalle.Text = seleccionado.ObtenerDescripcion();
                }
            }
            else // rbModoEditar.Checked
            {
                // Visibilidad de botones en modo Editar/Eliminar
                btnCrearRolRaiz.Visible = false;
                btnCrearSubRol.Visible = false;
                btnEditarNombre.Visible = true;
                btnEliminarRol.Visible = true;
                grpAsignarPermiso.Visible = true;
                btnQuitarItem.Visible = true;

                if (seleccionado == null)
                {
                    lblDetalle.Text = "Detalle: Selección vacía";
                    txtNombreRol.Text = "";
                    txtNombreRol.Enabled = false;
                    btnEditarNombre.Enabled = false;
                    btnEliminarRol.Enabled = false;
                    grpAsignarPermiso.Enabled = false;
                    btnQuitarItem.Enabled = false;
                    cmbPermisos.DataSource = null;
                }
                else if (seleccionado is Permiso permiso)
                {
                    lblDetalle.Text = permiso.ObtenerDescripcion();
                    txtNombreRol.Text = "";
                    txtNombreRol.Enabled = false;
                    btnEditarNombre.Enabled = false;
                    btnEliminarRol.Enabled = false;
                    grpAsignarPermiso.Enabled = false;
                    btnQuitarItem.Enabled = tvPerfiles.SelectedNode.Parent != null;
                    cmbPermisos.DataSource = null;
                }
                else if (seleccionado is Rol rol)
                {
                    lblDetalle.Text = rol.ObtenerDescripcion();
                    txtNombreRol.Text = rol.Nombre;
                    txtNombreRol.Enabled = true;
                    btnEditarNombre.Enabled = true;
                    btnEliminarRol.Enabled = true;
                    grpAsignarPermiso.Enabled = true;
                    btnQuitarItem.Enabled = tvPerfiles.SelectedNode.Parent != null;

                    CargarComboAsignables(rol);
                }
            }
        }

        private void CargarComboAsignables(Rol rolSeleccionado)
        {
            try
            {
                List<ComponenteWrapper> itemsAsignables = new List<ComponenteWrapper>();

                // 1. Agregar todos los permisos del sistema
                List<Permiso> permisos = rolBLL.ObtenerPermisosDisponibles();
                foreach (var p in permisos)
                {
                    itemsAsignables.Add(new ComponenteWrapper(p));
                }

                // 2. Agregar todos los roles excepto el seleccionado y los que generen ciclos
                List<Rol> todosRoles = rolBLL.ObtenerTodos();
                foreach (var r in todosRoles)
                {
                    if (r.Id != rolSeleccionado.Id && !rolBLL.TendriaCiclo(rolSeleccionado.Id, r.Id))
                    {
                        itemsAsignables.Add(new ComponenteWrapper(r));
                    }
                }

                cmbPermisos.DataSource = itemsAsignables;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ítems asignables: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCrearRolRaiz_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Escriba un nombre para el rol.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                rolBLL.CrearRol(nombre);
                CargarTreeView();
                txtNombreRol.Text = "";
                MessageBox.Show("Rol raíz creado exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear rol raíz: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCrearSubRol_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = tvPerfiles.SelectedNode;
            if (selectedNode == null || !(selectedNode.Tag is Rol padre)) return;

            string nombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Escriba un nombre para el sub-rol.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int nuevoId = rolBLL.CrearRol(nombre);
                Rol subrol = rolBLL.ObtenerConHijos(nuevoId);
                rolBLL.AsignarHijo(padre.Id, subrol);

                CargarTreeView();
                txtNombreRol.Text = "";
                MessageBox.Show($"Sub-rol '{nombre}' creado y asignado exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear sub-rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditarNombre_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = tvPerfiles.SelectedNode;
            if (selectedNode == null || !(selectedNode.Tag is Rol rol)) return;

            string nuevoNombre = txtNombreRol.Text.Trim();
            if (string.IsNullOrEmpty(nuevoNombre))
            {
                MessageBox.Show("El nombre del rol no puede estar vacío.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                rol.Nombre = nuevoNombre;
                rolBLL.ActualizarRol(rol);
                CargarTreeView();
                MessageBox.Show("Nombre actualizado exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar nombre: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminarRol_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = tvPerfiles.SelectedNode;
            if (selectedNode == null || !(selectedNode.Tag is Rol rol)) return;

            var result = MessageBox.Show($"¿Está seguro de eliminar el rol '{rol.Nombre}'? Se quitarán todas sus relaciones.", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                rolBLL.EliminarRol(rol.Id);
                CargarTreeView();
                ActualizarControles(null);
                MessageBox.Show("Rol eliminado exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar rol: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAsignarPermiso_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = tvPerfiles.SelectedNode;
            if (selectedNode == null || !(selectedNode.Tag is Rol padre)) return;

            ComponenteWrapper wrapper = cmbPermisos.SelectedItem as ComponenteWrapper;
            if (wrapper == null) return;

            try
            {
                rolBLL.AsignarHijo(padre.Id, wrapper.Componente);
                CargarTreeView();
                MessageBox.Show("Ítem asignado exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuitarItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = tvPerfiles.SelectedNode;
            if (selectedNode == null || selectedNode.Parent == null) return;

            Rol padre = selectedNode.Parent.Tag as Rol;
            IComponentePerfil hijo = selectedNode.Tag as IComponentePerfil;
            if (padre == null || hijo == null) return;

            var result = MessageBox.Show($"¿Desea desasignar '{hijo.Nombre}' del rol '{padre.Nombre}'?", "Confirmar desasignación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                rolBLL.RemoverHijo(padre.Id, hijo);
                CargarTreeView();
                ActualizarControles(null);
                MessageBox.Show("Ítem desasignado exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desasignar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Wrapper helper class to handle beautiful combobox list display using IComponentePerfil
        private class ComponenteWrapper
        {
            public IComponentePerfil Componente { get; }

            public ComponenteWrapper(IComponentePerfil componente)
            {
                Componente = componente;
            }

            public override string ToString()
            {
                return Componente.ObtenerDescripcion();
            }
        }
    }
}
