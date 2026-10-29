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
    // PN3 - Analista de Inversiones: casas Publicadas del inventario, con el estado de su proyecto de desarrollo
    public partial class FormIdentificarOportunidades : Form, IObservador
    {
        private class FilaCasa
        {
            public string Direccion { get; set; }
            public string Localidad { get; set; }
            public decimal? M2Terreno { get; set; }
            public decimal M2Inmueble { get; set; }
            public int Ambientes { get; set; }
            public decimal? ValorPretendido { get; set; }
            public string Operacion { get; set; }
            public string EstadoProyecto { get; set; }
            [Browsable(false)] public int IdInmueble { get; set; }
            [Browsable(false)] public ProyectoInversion Proyecto { get; set; } // el vigente, o el último rechazado
        }

        // Opción del filtro por estado del proyecto: el texto se traduce con la clave (Formulario.control)
        private class OpcionFiltro
        {
            public string Clave { get; set; }
            public string Texto { get; set; }
            public Func<ProyectoInversion, bool> Incluye { get; set; }
            public override string ToString() => Texto;
        }

        private ProyectoInversionBLL proyectoBLL = new ProyectoInversionBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _grilla;
        private List<FilaCasa> _casas = new List<FilaCasa>(); // todas las casas; la grilla muestra las del filtro
        private bool _cargandoFiltro;

        public FormIdentificarOportunidades()
        {
            InitializeComponent();
            PN3Comun.EstiloGrilla(dgvCasas);
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormIdentificarOportunidades"));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCrearProyecto, BE.NombreControl.FormIdentificarOportunidades_btnCrearProyecto));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnVerAnalisis, BE.NombreControl.FormIdentificarOportunidades_btnVerAnalisis));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblFiltroEstado, BE.NombreControl.FormIdentificarOportunidades_lblFiltroEstado));
            _grilla = new DataGridTraducible(dgvCasas, "FormIdentificarOportunidades.dgvCasas")
                .ConColumna("Direccion", BE.NombreControl.FormIdentificarOportunidades_dgvCasas_direccion)
                .ConColumna("Localidad", BE.NombreControl.FormIdentificarOportunidades_dgvCasas_localidad)
                .ConColumna("M2Terreno", BE.NombreControl.FormIdentificarOportunidades_dgvCasas_m2Terreno)
                .ConColumna("M2Inmueble", BE.NombreControl.FormIdentificarOportunidades_dgvCasas_m2Inmueble)
                .ConColumna("Ambientes", BE.NombreControl.FormIdentificarOportunidades_dgvCasas_ambientes)
                .ConColumna("ValorPretendido", BE.NombreControl.FormIdentificarOportunidades_dgvCasas_valorPretendido)
                .ConColumna("Operacion", BE.NombreControl.FormIdentificarOportunidades_dgvCasas_operacion)
                .ConColumna("EstadoProyecto", BE.NombreControl.FormIdentificarOportunidades_dgvCasas_estadoProyecto);
            _controlesTraducibles.Add(_grilla);
            GestorIdioma.Instancia.Adjuntar(this);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles) control.Traducir(traducciones);
            TraducirFiltro(traducciones);
        }

        // Vuelve a cargar las opciones con el texto traducido, conservando la selección
        private void TraducirFiltro(Dictionary<string, string> traducciones)
        {
            if (traducciones == null || cmbFiltroEstado.Items.Count == 0) return;
            OpcionFiltro[] opciones = cmbFiltroEstado.Items.Cast<OpcionFiltro>().ToArray();
            foreach (OpcionFiltro o in opciones)
                if (traducciones.TryGetValue(o.Clave, out string texto) && !string.IsNullOrWhiteSpace(texto)) o.Texto = texto;
            _cargandoFiltro = true;
            int seleccionado = cmbFiltroEstado.SelectedIndex;
            cmbFiltroEstado.BeginUpdate();
            cmbFiltroEstado.Items.Clear();
            cmbFiltroEstado.Items.AddRange(opciones);
            cmbFiltroEstado.SelectedIndex = seleccionado;
            cmbFiltroEstado.EndUpdate();
            _cargandoFiltro = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormIdentificarOportunidades_Load(object sender, EventArgs e)
        {
            // El estado que cuenta es el de la columna Estado proyecto: el vigente, o el último rechazado
            _cargandoFiltro = true;
            cmbFiltroEstado.Items.AddRange(new object[]
            {
                new OpcionFiltro { Clave = BE.NombreControl.FormIdentificarOportunidades_cmbFiltroEstado_todos, Texto = "Todos", Incluye = p => true },
                new OpcionFiltro { Clave = BE.NombreControl.FormIdentificarOportunidades_cmbFiltroEstado_sinProyecto, Texto = PN3Comun.Texto(null), Incluye = p => p == null },
                new OpcionFiltro { Clave = BE.NombreControl.FormIdentificarOportunidades_cmbFiltroEstado_enAnalisis, Texto = PN3Comun.Texto(EstadoProyecto.EnAnalisis), Incluye = p => p?.Estado == EstadoProyecto.EnAnalisis },
                new OpcionFiltro { Clave = BE.NombreControl.FormIdentificarOportunidades_cmbFiltroEstado_aprobado, Texto = PN3Comun.Texto(EstadoProyecto.Aprobado), Incluye = p => p?.Estado == EstadoProyecto.Aprobado },
                new OpcionFiltro { Clave = BE.NombreControl.FormIdentificarOportunidades_cmbFiltroEstado_rechazado, Texto = PN3Comun.Texto(EstadoProyecto.Rechazado), Incluye = p => p?.Estado == EstadoProyecto.Rechazado }
            });
            cmbFiltroEstado.SelectedIndex = 0; // Todos
            _cargandoFiltro = false;
            TraducirFiltro(GestorIdioma.Instancia.ObtenerTraduccionesActuales());
            CargarCasas();
        }

        private void cmbFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_cargandoFiltro) MostrarCasas(Seleccionada()?.IdInmueble);
        }

        private void CargarCasas(int? idInmuebleASeleccionar = null)
        {
            try
            {
                List<ProyectoInversion> proyectos = proyectoBLL.LeerTodos();
                Dictionary<int, CaptacionResumen> captaciones = new CaptacionBLL().LeerResumen()
                    .GroupBy(r => r.IdInmueble).ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.NroCaptacion).First());

                _casas = new InmuebleBLL().LeerTodos()
                    .Where(i => InmuebleBLL.EsCasa(i) && i.EstadoActual == EstadoInmueble.Publicado)
                    .Select(i =>
                    {
                        ProyectoInversion proyecto = proyectos.Where(p => p.IdInmueble == i.IdInmueble)
                            .OrderBy(p => p.Estado == EstadoProyecto.Rechazado).ThenByDescending(p => p.FechaCreacion).FirstOrDefault();
                        captaciones.TryGetValue(i.IdInmueble, out CaptacionResumen captacion);
                        return new FilaCasa
                        {
                            Direccion = i.Direccion,
                            Localidad = i.Localidad,
                            M2Terreno = i.SuperficieLote,
                            M2Inmueble = i.MetrosCuadrados,
                            Ambientes = i.CantidadAmbientes,
                            ValorPretendido = captacion?.ValorPretendido,
                            Operacion = i.TipoPublicacion.ToString(),
                            EstadoProyecto = proyecto == null ? PN3Comun.Texto(null) : $"{PN3Comun.Texto(proyecto.Estado)} (N° {proyecto.IdProyecto})",
                            IdInmueble = i.IdInmueble,
                            Proyecto = proyecto
                        };
                    }).OrderBy(f => f.Localidad).ThenBy(f => f.Direccion).ToList();
            }
            catch (Exception ex)
            {
                _casas = new List<FilaCasa>();
                MessageBox.Show("Error al cargar las casas del inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MostrarCasas(idInmuebleASeleccionar);
        }

        // Muestra en la grilla las casas del estado elegido en el filtro
        private void MostrarCasas(int? idInmuebleASeleccionar = null)
        {
            Func<ProyectoInversion, bool> incluye = (cmbFiltroEstado.SelectedItem as OpcionFiltro)?.Incluye ?? (p => true);
            dgvCasas.DataSource = null;
            dgvCasas.DataSource = _casas.Where(f => incluye(f.Proyecto)).ToList();
            PN3Comun.FormatoColumnas(dgvCasas, new[] { "ValorPretendido", "M2Terreno", "M2Inmueble" });
            _grilla.Traducir(GestorIdioma.Instancia.ObtenerTraduccionesActuales());
            foreach (DataGridViewRow fila in dgvCasas.Rows)
            {
                FilaCasa f = (FilaCasa)fila.DataBoundItem;
                if (f.Proyecto?.Estado == EstadoProyecto.Aprobado) fila.DefaultCellStyle.ForeColor = PN3Comun.VERDE;
                else if (f.Proyecto?.Estado == EstadoProyecto.EnAnalisis) fila.DefaultCellStyle.ForeColor = PN3Comun.AMARILLO;
                if (idInmuebleASeleccionar == f.IdInmueble) dgvCasas.CurrentCell = fila.Cells["Direccion"];
            }
            ActualizarBotones();
        }

        private FilaCasa Seleccionada() => dgvCasas.CurrentRow?.DataBoundItem as FilaCasa;

        private void dgvCasas_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotones();
        }

        // Crear solo sin proyecto vigente (sin proyecto, o con uno rechazado); ver análisis si hay proyecto
        private void ActualizarBotones()
        {
            FilaCasa f = Seleccionada();
            bool vigente = f?.Proyecto != null && f.Proyecto.Estado != EstadoProyecto.Rechazado;
            btnCrearProyecto.Enabled = f != null && !vigente;
            btnVerAnalisis.Enabled = f?.Proyecto != null;
            lblEstadoSeleccion.Text = f == null ? "" :
                f.Proyecto == null ? $"{f.Direccion}: sin proyecto de desarrollo."
                : f.Proyecto.DatosTecnicosCompletos ? $"{f.Direccion}: Proyecto N° {f.Proyecto.IdProyecto} {PN3Comun.Texto(f.Proyecto.Estado)}, canje {f.Proyecto.PorcentajeCanje:0.##}%."
                : $"{f.Direccion}: Proyecto N° {f.Proyecto.IdProyecto} esperando los datos técnicos de Arquitectura.";
        }

        private void btnCrearProyecto_Click(object sender, EventArgs e)
        {
            FilaCasa f = Seleccionada();
            if (f == null) return;
            decimal? canje = PN3Comun.PedirNumero(this, "Crear Proyecto",
                $"Porcentaje de las unidades para el propietario de {f.Direccion} por aportar el lote (canje):",
                ProyectoInversionBLL.PORCENTAJE_CANJE_DEFECTO, 1, 99, 2);
            if (!canje.HasValue) return;
            try
            {
                int id = proyectoBLL.CrearProyecto(f.IdInmueble, canje.Value);
                MessageBox.Show($"Proyecto N° {id} creado (canje {canje:0.##}%). Queda En Análisis hasta que Arquitectura cargue los datos técnicos.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarCasas(f.IdInmueble);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear el proyecto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVerAnalisis_Click(object sender, EventArgs e)
        {
            FilaCasa f = Seleccionada();
            if (f?.Proyecto == null) return;
            if (!f.Proyecto.DatosTecnicosCompletos)
            {
                MessageBox.Show($"El Proyecto N° {f.Proyecto.IdProyecto} todavía no tiene los datos técnicos de Arquitectura: no hay análisis para mostrar.",
                    "Ver Análisis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (FormVerAnalisis frm = new FormVerAnalisis(f.Proyecto.IdProyecto))
                frm.ShowDialog(this);
        }
    }
}
