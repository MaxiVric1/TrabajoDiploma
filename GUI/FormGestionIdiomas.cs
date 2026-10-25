using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BE;
using BLL;
using TpIngSoft.Traduccion;

namespace TpIngSoft
{
    public partial class FormGestionIdiomas : Form, IObservador
    {
        private readonly IdiomaBLL _idiomaBLL = new IdiomaBLL();
        private List<IControlTraducible> _controlesTraducibles = new List<IControlTraducible>();
        private DataGridTraducible _dgvIdiomasTraducible;
        private DataGridTraducible _dgvTraduccionesTraducible;
        private bool _isChangingLanguage = false;

        public FormGestionIdiomas()
        {
            InitializeComponent();
            RegistrarControlesTraducibles();
            GestorIdioma.Instancia.Adjuntar(this);
        }

        private void RegistrarControlesTraducibles()
        {
            _controlesTraducibles.Clear();
            _controlesTraducibles.Add(new EtiquetaTraducible(this, "FormGestionIdiomas"));
            _controlesTraducibles.Add(new EtiquetaTraducible(lblNuevoIdioma, BE.NombreControl.FormGestionIdiomas_lblNuevoIdioma));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnCrearIdioma, BE.NombreControl.FormGestionIdiomas_btnCrearIdioma));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnGuardarTraducciones, BE.NombreControl.FormGestionIdiomas_btnGuardarTraducciones));
            _controlesTraducibles.Add(new EtiquetaTraducible(btnAplicar, BE.NombreControl.FormGestionIdiomas_btnAplicar));

            _dgvIdiomasTraducible = new DataGridTraducible(dgvIdiomas, "FormGestionIdiomas.dgvIdiomas")
                .ConColumna("Id", BE.NombreControl.FormGestionIdiomas_dgvIdiomas_id)
                .ConColumna("Nombre", BE.NombreControl.FormGestionIdiomas_dgvIdiomas_nombre)
                .ConColumna("Activo", BE.NombreControl.FormGestionIdiomas_dgvIdiomas_activo);

            _dgvTraduccionesTraducible = new DataGridTraducible(dgvTraducciones, "FormGestionIdiomas.dgvTraducciones")
                .ConColumna("Formulario", BE.NombreControl.FormGestionIdiomas_dgvTraducciones_formulario)
                .ConColumna("Control", BE.NombreControl.FormGestionIdiomas_dgvTraducciones_control)
                .ConColumna("Texto", BE.NombreControl.FormGestionIdiomas_dgvTraducciones_texto);
        }

        public void Actualizar(Dictionary<string, string> traducciones)
        {
            foreach (var control in _controlesTraducibles)
            {
                control.Traducir(traducciones);
            }
            _dgvIdiomasTraducible.Traducir(traducciones);
            _dgvTraduccionesTraducible.Traducir(traducciones);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            GestorIdioma.Instancia.Separar(this);
            base.OnFormClosed(e);
        }

        private void FormGestionIdiomas_Load(object sender, EventArgs e)
        {
            InicializarControlesEnBD();
            RefrescarIdiomas();
        }

        private void InicializarControlesEnBD()
        {
            var list = new List<(string Form, string Ctrl)>
            {
                ("FormLogin", "(form)"),
                ("FormMain", "(form)"),
                ("FormGestionUsuarios", "(form)"),
                ("FormBitacora", "(form)"),
                ("FormGestionRoles", "(form)"),
                ("FormHistorialUsuario", "(form)"),
                ("FormGestionIdiomas", "(form)"),
                ("FormRegistrarCaptacion", "(form)"),
                ("FormGestionarCaptaciones", "(form)"),
                ("FormValidarTasacion", "(form)"),
                ("FormCargarMaterialAudiovisual", "(form)"),
                ("FormPublicarInmueble", "(form)"),
                ("FormInmueblesPublicados", "(form)"),
                ("FormGestionarContratos", "(form)"),
                ("FormRegistrarContrato", "(form)"),
                ("FormRegistrarPago", "(form)"),
                ("FormGestionarMora", "(form)"),
                ("FormGestionarLitigios", "(form)"),
                ("FormContratosPorVencer", "(form)"),
                ("FormRenovarContrato", "(form)"),

                ("FormLogin", "label1"),
                ("FormLogin", "label2"),
                ("FormLogin", "label3"),
                ("FormLogin", "buttonLogin"),
                ("FormLogin", "labelIdioma"),
                ("FormMain", "archivoToolStripMenuItem"),
                ("FormMain", "logoutToolStripMenuItem"),
                ("FormMain", "salirToolStripMenuItem"),
                ("FormMain", "adminToolStripMenuItem"),
                ("FormMain", "gestionUsuariosToolStripMenuItem"),
                ("FormMain", "bitacoraToolStripMenuItem"),
                ("FormMain", "controlCambiosToolStripMenuItem"),
                ("FormMain", "gestionPerfilesToolStripMenuItem"),
                ("FormMain", "gestionIdiomasToolStripMenuItem"),
                ("FormMain", "verificarIntegridadToolStripMenuItem"),
                ("FormMain", "lblSesionInfo"),
                ("FormMain", "pn1ToolStripMenuItem"),
                ("FormMain", "registrarCaptacionToolStripMenuItem"),
                ("FormMain", "gestionarCaptacionesToolStripMenuItem"),
                ("FormMain", "validarTasacionToolStripMenuItem"),
                ("FormMain", "cargarMaterialAudiovisualToolStripMenuItem"),
                ("FormMain", "publicarInmuebleToolStripMenuItem"),
                ("FormMain", "inmueblesPublicadosToolStripMenuItem"),
                ("FormMain", "pn2ToolStripMenuItem"),
                ("FormMain", "gestionarContratosToolStripMenuItem"),
                ("FormMain", "registrarContratoToolStripMenuItem"),
                ("FormMain", "registrarPagoToolStripMenuItem"),
                ("FormMain", "gestionarMoraToolStripMenuItem"),
                ("FormMain", "gestionarLitigiosToolStripMenuItem"),
                ("FormMain", "contratosPorVencerToolStripMenuItem"),
                ("FormGestionUsuarios", "label1"),
                ("FormGestionUsuarios", "labelNombrePersona"),
                ("FormGestionUsuarios", "labelApellido"),
                ("FormGestionUsuarios", "label2"),
                ("FormGestionUsuarios", "lblRol"),
                ("FormGestionUsuarios", "btnRegistrar"),
                ("FormBitacora", "labelDesde"),
                ("FormBitacora", "labelHasta"),
                ("FormBitacora", "labelActividad"),
                ("FormBitacora", "labelUsuario"),
                ("FormBitacora", "btnBuscar"),
                ("FormBitacora", "dgvBitacora.id"),
                ("FormBitacora", "dgvBitacora.fecha"),
                ("FormBitacora", "dgvBitacora.usuario"),
                ("FormBitacora", "dgvBitacora.descripcion"),
                ("FormBitacora", "dgvBitacora.criticidad"),
                ("FormGestionRoles", "grpAcciones"),
                ("FormGestionRoles", "lblDetalle"),
                ("FormGestionRoles", "lblModo"),
                ("FormGestionRoles", "rbModoCrear"),
                ("FormGestionRoles", "rbModoEditar"),
                ("FormGestionRoles", "lblNombreRol"),
                ("FormGestionRoles", "btnCrearRolRaiz"),
                ("FormGestionRoles", "btnCrearSubRol"),
                ("FormGestionRoles", "btnEditarNombre"),
                ("FormGestionRoles", "btnEliminarRol"),
                ("FormGestionRoles", "grpAsignarPermiso"),
                ("FormGestionRoles", "btnAsignarPermiso"),
                ("FormHistorialUsuario", "(form)"),
                ("FormHistorialUsuario", "label1"),
                ("FormHistorialUsuario", "btnCargarHistorial"),
                ("FormHistorialUsuario", "btnRestaurar"),
                ("FormHistorialUsuario", "dgvHistorial.idHistorial"),
                ("FormHistorialUsuario", "dgvHistorial.idUsuario"),
                ("FormHistorialUsuario", "dgvHistorial.fecha"),
                ("FormHistorialUsuario", "dgvHistorial.usuario"),
                ("FormHistorialUsuario", "dgvHistorial.estado"),
                ("FormHistorialUsuario", "dgvHistorial.editorNombre"),
                ("FormRegistrarCaptacion", "grpCliente"),
                ("FormRegistrarCaptacion", "lblDni"),
                ("FormRegistrarCaptacion", "btnBuscarCliente"),
                ("FormRegistrarCaptacion", "lblNombre"),
                ("FormRegistrarCaptacion", "lblApellido"),
                ("FormRegistrarCaptacion", "lblTelefono"),
                ("FormRegistrarCaptacion", "lblEmail"),
                ("FormRegistrarCaptacion", "grpInmueble"),
                ("FormRegistrarCaptacion", "lblTipo"),
                ("FormRegistrarCaptacion", "lblDireccion"),
                ("FormRegistrarCaptacion", "lblMetros"),
                ("FormRegistrarCaptacion", "lblAmbientes"),
                ("FormRegistrarCaptacion", "grpCaptacion"),
                ("FormRegistrarCaptacion", "lblValorPretendido"),
                ("FormRegistrarCaptacion", "chkAutorizacionRecibida"),
                ("FormRegistrarCaptacion", "chkAutorizacionAlquiler"),
                ("FormRegistrarCaptacion", "lblTipoPublicacion"),
                ("FormRegistrarCaptacion", "btnGuardar"),
                ("FormRegistrarCaptacion", "btnCancelar"),
                ("FormGestionarCaptaciones", "lblFiltroEstado"),
                ("FormGestionarCaptaciones", "btnActualizar"),
                ("FormGestionarCaptaciones", "grpRenegociacion"),
                ("FormGestionarCaptaciones", "lblDictamen"),
                ("FormGestionarCaptaciones", "lblValorActual"),
                ("FormGestionarCaptaciones", "lblMotivo"),
                ("FormGestionarCaptaciones", "lblValorSugerido"),
                ("FormGestionarCaptaciones", "lblNuevoValor"),
                ("FormGestionarCaptaciones", "btnRenegociar"),
                ("FormGestionarCaptaciones", "dgvCaptaciones.nro"),
                ("FormGestionarCaptaciones", "dgvCaptaciones.fecha"),
                ("FormGestionarCaptaciones", "dgvCaptaciones.cliente"),
                ("FormGestionarCaptaciones", "dgvCaptaciones.direccion"),
                ("FormGestionarCaptaciones", "dgvCaptaciones.tipo"),
                ("FormGestionarCaptaciones", "dgvCaptaciones.tipoPublicacion"),
                ("FormGestionarCaptaciones", "dgvCaptaciones.valor"),
                ("FormGestionarCaptaciones", "dgvCaptaciones.estado"),
                ("FormValidarTasacion", "lblPendientes"),
                ("FormValidarTasacion", "btnActualizar"),
                ("FormValidarTasacion", "grpDetalle"),
                ("FormValidarTasacion", "lblCliente"),
                ("FormValidarTasacion", "lblDni"),
                ("FormValidarTasacion", "lblTelefono"),
                ("FormValidarTasacion", "lblEmail"),
                ("FormValidarTasacion", "lblValorPretendido"),
                ("FormValidarTasacion", "lblDictamenAnterior"),
                ("FormValidarTasacion", "grpDictamen"),
                ("FormValidarTasacion", "rbAprobar"),
                ("FormValidarTasacion", "rbRechazar"),
                ("FormValidarTasacion", "lblValorFinal"),
                ("FormValidarTasacion", "lblMotivo"),
                ("FormValidarTasacion", "lblValorSugerido"),
                ("FormValidarTasacion", "btnRegistrar"),
                ("FormValidarTasacion", "dgvPendientes.nro"),
                ("FormValidarTasacion", "dgvPendientes.fecha"),
                ("FormValidarTasacion", "dgvPendientes.cliente"),
                ("FormValidarTasacion", "dgvPendientes.direccion"),
                ("FormValidarTasacion", "dgvPendientes.tipo"),
                ("FormValidarTasacion", "dgvPendientes.tipoPublicacion"),
                ("FormValidarTasacion", "dgvPendientes.metros"),
                ("FormValidarTasacion", "dgvPendientes.ambientes"),
                ("FormValidarTasacion", "dgvPendientes.valor"),
                ("FormCargarMaterialAudiovisual", "lblInmuebles"),
                ("FormCargarMaterialAudiovisual", "btnActualizar"),
                ("FormCargarMaterialAudiovisual", "grpNuevoMaterial"),
                ("FormCargarMaterialAudiovisual", "lblTextoComercial"),
                ("FormCargarMaterialAudiovisual", "lblArchivos"),
                ("FormCargarMaterialAudiovisual", "btnAgregarArchivo"),
                ("FormCargarMaterialAudiovisual", "btnGuardar"),
                ("FormCargarMaterialAudiovisual", "grpMaterialCargado"),
                ("FormCargarMaterialAudiovisual", "dgvInmuebles.nro"),
                ("FormCargarMaterialAudiovisual", "dgvInmuebles.cliente"),
                ("FormCargarMaterialAudiovisual", "dgvInmuebles.direccion"),
                ("FormCargarMaterialAudiovisual", "dgvInmuebles.tipo"),
                ("FormCargarMaterialAudiovisual", "dgvInmuebles.tipoPublicacion"),
                ("FormCargarMaterialAudiovisual", "dgvInmuebles.valorFinal"),
                ("FormCargarMaterialAudiovisual", "dgvArchivos.archivo"),
                ("FormCargarMaterialAudiovisual", "dgvArchivos.tipo"),
                ("FormCargarMaterialAudiovisual", "dgvArchivos.quitar"),
                ("FormCargarMaterialAudiovisual", "dgvMaterialCargado.tipo"),
                ("FormCargarMaterialAudiovisual", "dgvMaterialCargado.archivo"),
                ("FormCargarMaterialAudiovisual", "dgvMaterialCargado.fecha"),
                ("FormPublicarInmueble", "lblListos"),
                ("FormPublicarInmueble", "btnActualizar"),
                ("FormPublicarInmueble", "grpDetalle"),
                ("FormPublicarInmueble", "lblTextoComercial"),
                ("FormPublicarInmueble", "lblMaterial"),
                ("FormPublicarInmueble", "lblVistaPrevia"),
                ("FormPublicarInmueble", "lblSinVistaPrevia"),
                ("FormPublicarInmueble", "btnPublicar"),
                ("FormPublicarInmueble", "dgvInmuebles.nro"),
                ("FormPublicarInmueble", "dgvInmuebles.cliente"),
                ("FormPublicarInmueble", "dgvInmuebles.direccion"),
                ("FormPublicarInmueble", "dgvInmuebles.tipo"),
                ("FormPublicarInmueble", "dgvInmuebles.tipoPublicacion"),
                ("FormPublicarInmueble", "dgvInmuebles.valorFinal"),
                ("FormPublicarInmueble", "dgvInmuebles.archivos"),
                ("FormPublicarInmueble", "dgvMaterial.tipo"),
                ("FormPublicarInmueble", "dgvMaterial.archivo"),
                ("FormPublicarInmueble", "dgvMaterial.fecha"),
                ("FormInmueblesPublicados", "lblPublicados"),
                ("FormInmueblesPublicados", "btnActualizar"),
                ("FormInmueblesPublicados", "grpDetalle"),
                ("FormInmueblesPublicados", "lblTextoComercial"),
                ("FormInmueblesPublicados", "lblMaterial"),
                ("FormInmueblesPublicados", "lblVistaPrevia"),
                ("FormInmueblesPublicados", "lblSinVistaPrevia"),
                ("FormInmueblesPublicados", "btnDespublicar"),
                ("FormInmueblesPublicados", "btnRepublicar"),
                ("FormInmueblesPublicados", "dgvInmuebles.nro"),
                ("FormInmueblesPublicados", "dgvInmuebles.cliente"),
                ("FormInmueblesPublicados", "dgvInmuebles.direccion"),
                ("FormInmueblesPublicados", "dgvInmuebles.tipo"),
                ("FormInmueblesPublicados", "dgvInmuebles.tipoPublicacion"),
                ("FormInmueblesPublicados", "dgvInmuebles.valorFinal"),
                ("FormInmueblesPublicados", "dgvInmuebles.estado"),
                ("FormInmueblesPublicados", "dgvInmuebles.fechaPublicacion"),
                ("FormInmueblesPublicados", "dgvInmuebles.fechaDespublicacion"),
                ("FormInmueblesPublicados", "dgvMaterial.tipo"),
                ("FormInmueblesPublicados", "dgvMaterial.archivo"),
                ("FormInmueblesPublicados", "dgvMaterial.fecha"),
                ("FormRegistrarContrato", "grpInquilino"),
                ("FormRegistrarContrato", "lblDni"),
                ("FormRegistrarContrato", "btnBuscarInquilino"),
                ("FormRegistrarContrato", "lblNombre"),
                ("FormRegistrarContrato", "lblApellido"),
                ("FormRegistrarContrato", "lblTelefono"),
                ("FormRegistrarContrato", "lblEmail"),
                ("FormRegistrarContrato", "lblInmueble"),
                ("FormRegistrarContrato", "grpContrato"),
                ("FormRegistrarContrato", "lblFechaInicio"),
                ("FormRegistrarContrato", "lblDuracion"),
                ("FormRegistrarContrato", "lblMonto"),
                ("FormRegistrarContrato", "lblFechaFin"),
                ("FormRegistrarContrato", "btnCancelar"),
                ("FormRegistrarContrato", "tabInmueble"),
                ("FormRegistrarContrato", "tabInquilino"),
                ("FormRegistrarContrato", "tabGarante"),
                ("FormRegistrarContrato", "tabCondiciones"),
                ("FormRegistrarContrato", "tabResumen"),
                ("FormRegistrarContrato", "grpDatosInmueble"),
                ("FormRegistrarContrato", "lblTipoInmueble"),
                ("FormRegistrarContrato", "lblDireccion"),
                ("FormRegistrarContrato", "lblSuperficie"),
                ("FormRegistrarContrato", "lblAmbientes"),
                ("FormRegistrarContrato", "lblValorTasado"),
                ("FormRegistrarContrato", "grpPropietario"),
                ("FormRegistrarContrato", "lblPropietarioNombre"),
                ("FormRegistrarContrato", "lblPropietarioDni"),
                ("FormRegistrarContrato", "lblPropietarioTelefono"),
                ("FormRegistrarContrato", "lblPropietarioEmail"),
                ("FormRegistrarContrato", "grpGarante"),
                ("FormRegistrarContrato", "lblGaranteNombre"),
                ("FormRegistrarContrato", "lblGaranteApellido"),
                ("FormRegistrarContrato", "lblGaranteDni"),
                ("FormRegistrarContrato", "lblGaranteDomicilio"),
                ("FormRegistrarContrato", "lblPropiedadGarantia"),
                ("FormRegistrarContrato", "lblMetodoPago"),
                ("FormRegistrarContrato", "lblDeposito"),
                ("FormRegistrarContrato", "lblComision"),
                ("FormRegistrarContrato", "lblActualizacion"),
                ("FormRegistrarContrato", "lblFirma"),
                ("FormRegistrarContrato", "btnRegistrar"),
                ("FormRegistrarContrato", "btnGenerarPdf"),
                ("FormRegistrarContrato", "btnNuevoContrato"),
                ("FormRegistrarContrato", "btnAnterior"),
                ("FormRegistrarContrato", "btnSiguiente"),
                ("FormGestionarContratos", "lblFiltroEstado"),
                ("FormGestionarContratos", "btnActualizar"),
                ("FormGestionarContratos", "grpInquilino"),
                ("FormGestionarContratos", "lblNombre"),
                ("FormGestionarContratos", "lblApellido"),
                ("FormGestionarContratos", "lblDni"),
                ("FormGestionarContratos", "lblTelefono"),
                ("FormGestionarContratos", "lblEmail"),
                ("FormGestionarContratos", "grpRenovacion"),
                ("FormGestionarContratos", "lblMesesExtension"),
                ("FormGestionarContratos", "lblNuevoMonto"),
                ("FormGestionarContratos", "lblNuevoVencimiento"),
                ("FormGestionarContratos", "btnRenovar"),
                ("FormGestionarContratos", "grpPagos"),
                ("FormGestionarContratos", "lblMesActual"),
                ("FormGestionarContratos", "dgvContratos.nro"),
                ("FormGestionarContratos", "dgvContratos.inquilino"),
                ("FormGestionarContratos", "dgvContratos.direccion"),
                ("FormGestionarContratos", "dgvContratos.tipo"),
                ("FormGestionarContratos", "dgvContratos.monto"),
                ("FormGestionarContratos", "dgvContratos.fechaInicio"),
                ("FormGestionarContratos", "dgvContratos.fechaFin"),
                ("FormGestionarContratos", "dgvContratos.estado"),
                ("FormGestionarContratos", "dgvContratos.alerta"),
                ("FormGestionarContratos", "dgvPagos.mes"),
                ("FormGestionarContratos", "dgvPagos.anio"),
                ("FormGestionarContratos", "dgvPagos.estado"),
                ("FormGestionarContratos", "dgvPagos.monto"),
                ("FormGestionarContratos", "dgvPagos.recargo"),
                ("FormGestionarContratos", "dgvPagos.fechaPago"),
                ("FormGestionarContratos", "grpCondiciones"),
                ("FormGestionarContratos", "lblDeposito"),
                ("FormGestionarContratos", "lblComision"),
                ("FormGestionarContratos", "lblMetodoPago"),
                ("FormGestionarContratos", "lblIndice"),
                ("FormGestionarContratos", "lblGarante"),
                ("FormGestionarContratos", "grpRescision"),
                ("FormGestionarContratos", "lblIniciadaPor"),
                ("FormGestionarContratos", "lblMesesRestantes"),
                ("FormGestionarContratos", "lblPenalidad"),
                ("FormGestionarContratos", "btnRescindir"),
                ("FormRegistrarPago", "grpContrato"),
                ("FormRegistrarPago", "lblNroContrato"),
                ("FormRegistrarPago", "btnBuscar"),
                ("FormRegistrarPago", "lblInquilino"),
                ("FormRegistrarPago", "lblDireccion"),
                ("FormRegistrarPago", "lblMontoMensual"),
                ("FormRegistrarPago", "lblEstadoContrato"),
                ("FormRegistrarPago", "grpPeriodo"),
                ("FormRegistrarPago", "lblPeriodo"),
                ("FormRegistrarPago", "lblEstadoMes"),
                ("FormRegistrarPago", "lblFechaPago"),
                ("FormRegistrarPago", "lblMontoBase"),
                ("FormRegistrarPago", "lblRecargo"),
                ("FormRegistrarPago", "lblTotal"),
                ("FormRegistrarPago", "grpResumen"),
                ("FormRegistrarPago", "btnRegistrar"),
                ("FormRegistrarPago", "lblMora"),
                ("FormRegistrarPago", "lblComision"),
                ("FormRegistrarPago", "lblNeto"),
                ("FormGestionarMora", "grpInquilino"),
                ("FormGestionarMora", "lblNombre"),
                ("FormGestionarMora", "lblApellido"),
                ("FormGestionarMora", "lblDni"),
                ("FormGestionarMora", "lblTelefono"),
                ("FormGestionarMora", "lblEmail"),
                ("FormGestionarMora", "grpContrato"),
                ("FormGestionarMora", "lblDireccion"),
                ("FormGestionarMora", "lblMontoMensual"),
                ("FormGestionarMora", "lblFechaFin"),
                ("FormGestionarMora", "lblDiasMora"),
                ("FormGestionarMora", "lblMontoAdeudado"),
                ("FormGestionarMora", "btnEscalar"),
                ("FormGestionarMora", "grpContactos"),
                ("FormGestionarMora", "lblFechaContacto"),
                ("FormGestionarMora", "lblDescripcion"),
                ("FormGestionarMora", "btnRegistrarContacto"),
                ("FormGestionarMora", "dgvPagos.nro"),
                ("FormGestionarMora", "dgvPagos.inquilino"),
                ("FormGestionarMora", "dgvPagos.direccion"),
                ("FormGestionarMora", "dgvPagos.periodo"),
                ("FormGestionarMora", "dgvPagos.montoAdeudado"),
                ("FormGestionarMora", "dgvPagos.diasMora"),
                ("FormGestionarMora", "dgvContactos.fecha"),
                ("FormGestionarMora", "dgvContactos.descripcion"),
                ("FormGestionarMora", "dgvContactos.analista"),
                ("FormGestionarLitigios", "grpInquilino"),
                ("FormGestionarLitigios", "lblNombre"),
                ("FormGestionarLitigios", "lblApellido"),
                ("FormGestionarLitigios", "lblDni"),
                ("FormGestionarLitigios", "lblTelefono"),
                ("FormGestionarLitigios", "lblEmail"),
                ("FormGestionarLitigios", "grpContrato"),
                ("FormGestionarLitigios", "lblDireccion"),
                ("FormGestionarLitigios", "lblMontoMensual"),
                ("FormGestionarLitigios", "lblFechaFin"),
                ("FormGestionarLitigios", "lblDiasLitigio"),
                ("FormGestionarLitigios", "lblMontoAdeudado"),
                ("FormGestionarLitigios", "lblDeudaTotal"),
                ("FormGestionarLitigios", "grpAcciones"),
                ("FormGestionarLitigios", "lblFechaAccion"),
                ("FormGestionarLitigios", "lblDescripcion"),
                ("FormGestionarLitigios", "btnIntimacion"),
                ("FormGestionarLitigios", "btnResuelto"),
                ("FormGestionarLitigios", "btnDesalojo"),
                ("FormGestionarLitigios", "dgvPagos.nro"),
                ("FormGestionarLitigios", "dgvPagos.inquilino"),
                ("FormGestionarLitigios", "dgvPagos.direccion"),
                ("FormGestionarLitigios", "dgvPagos.periodo"),
                ("FormGestionarLitigios", "dgvPagos.montoAdeudado"),
                ("FormGestionarLitigios", "dgvPagos.diasLitigio"),
                ("FormGestionarLitigios", "dgvAcciones.fecha"),
                ("FormGestionarLitigios", "dgvAcciones.tipo"),
                ("FormGestionarLitigios", "dgvAcciones.descripcion"),
                ("FormContratosPorVencer", "btnActualizar"),
                ("FormContratosPorVencer", "grpInquilino"),
                ("FormContratosPorVencer", "lblNombre"),
                ("FormContratosPorVencer", "lblApellido"),
                ("FormContratosPorVencer", "lblDni"),
                ("FormContratosPorVencer", "lblTelefono"),
                ("FormContratosPorVencer", "lblEmail"),
                ("FormContratosPorVencer", "grpAcciones"),
                ("FormContratosPorVencer", "lblObservacion"),
                ("FormContratosPorVencer", "btnRenovacion"),
                ("FormContratosPorVencer", "btnDesocupacion"),
                ("FormContratosPorVencer", "grpContrato"),
                ("FormContratosPorVencer", "lblNroContrato"),
                ("FormContratosPorVencer", "lblDireccion"),
                ("FormContratosPorVencer", "lblTipo"),
                ("FormContratosPorVencer", "lblFechaInicio"),
                ("FormContratosPorVencer", "lblDuracion"),
                ("FormContratosPorVencer", "lblFechaFin"),
                ("FormContratosPorVencer", "lblDiasRestantes"),
                ("FormContratosPorVencer", "lblMonto"),
                ("FormContratosPorVencer", "lblEstado"),
                ("FormContratosPorVencer", "lblAlerta"),
                ("FormContratosPorVencer", "lblPagoActual"),
                ("FormContratosPorVencer", "lblDeuda"),
                ("FormContratosPorVencer", "lblDesocupacion"),
                ("FormContratosPorVencer", "dgvContratos.nro"),
                ("FormContratosPorVencer", "dgvContratos.inquilino"),
                ("FormContratosPorVencer", "dgvContratos.direccion"),
                ("FormContratosPorVencer", "dgvContratos.tipo"),
                ("FormContratosPorVencer", "dgvContratos.monto"),
                ("FormContratosPorVencer", "dgvContratos.fechaFin"),
                ("FormContratosPorVencer", "dgvContratos.diasRestantes"),
                ("FormContratosPorVencer", "dgvContratos.estado"),
                ("FormContratosPorVencer", "dgvContratos.estadoPago"),
                ("FormContratosPorVencer", "dgvContratos.desocupacion"),
                ("FormRenovarContrato", "lblContrato"),
                ("FormRenovarContrato", "lblVencimientoActual"),
                ("FormRenovarContrato", "lblMontoActual"),
                ("FormRenovarContrato", "lblMeses"),
                ("FormRenovarContrato", "lblNuevoMonto"),
                ("FormRenovarContrato", "lblNuevoVencimiento"),
                ("FormRenovarContrato", "btnConfirmar"),
                ("FormRenovarContrato", "btnCancelar"),
                ("FormGestionIdiomas", "(form)"),
                ("FormGestionIdiomas", "lblNuevoIdioma"),
                ("FormGestionIdiomas", "btnCrearIdioma"),
                ("FormGestionIdiomas", "btnToggleActivo"),
                ("FormGestionIdiomas", "btnGuardarTraducciones"),
                ("FormGestionIdiomas", "btnAplicar"),
                ("FormGestionIdiomas", "dgvIdiomas.id"),
                ("FormGestionIdiomas", "dgvIdiomas.nombre"),
                ("FormGestionIdiomas", "dgvIdiomas.activo"),
                ("FormGestionIdiomas", "dgvTraducciones.formulario"),
                ("FormGestionIdiomas", "dgvTraducciones.control"),
                ("FormGestionIdiomas", "dgvTraducciones.texto")
            };

            foreach (var item in list)
            {
                _idiomaBLL.RegistrarControl(item.Ctrl, item.Form);
            }
        }

        private void RefrescarIdiomas()
        {
            _isChangingLanguage = true;
            try
            {
                var todosIdiomas = _idiomaBLL.ObtenerTodosIdiomas();
                var activeLanguages = _idiomaBLL.ObtenerIdiomasActivos();
                
                dgvIdiomas.DataSource = null;
                dgvIdiomas.DataSource = todosIdiomas;

                if (GestorIdioma.Instancia.IdiomaActual != null && dgvIdiomas.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvIdiomas.Rows)
                    {
                        var id = (Idioma)row.DataBoundItem;
                        if (id.Id == GestorIdioma.Instancia.IdiomaActual.Id)
                        {
                            row.Selected = true;
                            dgvIdiomas.CurrentCell = row.Cells[0];
                            break;
                        }
                    }
                }

                var trans = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
                _dgvIdiomasTraducible.Traducir(trans);
            }
            finally
            {
                _isChangingLanguage = false;
            }

            CargarTraducciones();
        }

        private void CargarTraducciones()
        {
            if (dgvIdiomas.CurrentRow == null) return;
            var idiomaSeleccionado = (Idioma)dgvIdiomas.CurrentRow.DataBoundItem;
            int idIdioma = idiomaSeleccionado.Id;

            List<BE.Traduccion> todosControles = _idiomaBLL.ObtenerControles();
            var cache = _idiomaBLL.ObtenerTraduccionesCacheadas(idIdioma);

            List<RenglonTraduccion> renglones = new List<RenglonTraduccion>();
            foreach (var control in todosControles)
            {
                string key = (control.NombreControl == "(form)" || string.IsNullOrEmpty(control.NombreControl))
                    ? control.Formulario
                    : $"{control.Formulario}.{control.NombreControl}";
                string texto = "";
                if (cache.TryGetValue(key, out string t))
                {
                    texto = t;
                }

                renglones.Add(new RenglonTraduccion
                {
                    IdControl = control.IdControl,
                    Formulario = control.Formulario,
                    Control = control.NombreControl,
                    Texto = texto
                });
            }

            dgvTraducciones.DataSource = null;
            dgvTraducciones.DataSource = new BindingList<RenglonTraduccion>(renglones);

            if (dgvTraducciones.Columns["IdControl"] != null) dgvTraducciones.Columns["IdControl"].Visible = false;
            if (dgvTraducciones.Columns["Formulario"] != null) dgvTraducciones.Columns["Formulario"].ReadOnly = true;
            if (dgvTraducciones.Columns["Control"] != null) dgvTraducciones.Columns["Control"].ReadOnly = true;
            if (dgvTraducciones.Columns["Texto"] != null) dgvTraducciones.Columns["Texto"].Width = 200;

            var trans = GestorIdioma.Instancia.ObtenerTraduccionesActuales();
            _dgvTraduccionesTraducible.Traducir(trans);
        }

        private void dgvIdiomas_SelectionChanged(object sender, EventArgs e)
        {
            if (_isChangingLanguage) return;
            CargarTraducciones();
        }

        private void btnCrearIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNuevoIdioma.Text.Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("Por favor ingrese un nombre.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int id = _idiomaBLL.CrearIdioma(nombre);
                if (id != -1)
                {
                    MessageBox.Show("Idioma creado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNuevoIdioma.Clear();
                    RefrescarIdiomas();
                }
                else
                {
                    MessageBox.Show("Error al crear el idioma.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarTraducciones_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvIdiomas.CurrentRow == null) return;
                var idiomaSeleccionado = (Idioma)dgvIdiomas.CurrentRow.DataBoundItem;
                int idIdioma = idiomaSeleccionado.Id;

                var bindingList = dgvTraducciones.DataSource as BindingList<RenglonTraduccion>;
                if (bindingList != null)
                {
                    foreach (var renglon in bindingList)
                    {
                        _idiomaBLL.GuardarTraduccion(renglon.IdControl, idIdioma, renglon.Texto);
                    }
                    MessageBox.Show("Traducciones guardadas con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    if (GestorIdioma.Instancia.IdiomaActual != null && GestorIdioma.Instancia.IdiomaActual.Id == idIdioma)
                    {
                        GestorIdioma.Instancia.CambiarIdioma(GestorIdioma.Instancia.IdiomaActual);
                    }
                    
                    CargarTraducciones();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            if (dgvIdiomas.CurrentRow != null)
            {
                var seleccionado = (Idioma)dgvIdiomas.CurrentRow.DataBoundItem;
                if (!seleccionado.Activo)
                {
                    MessageBox.Show("No se puede aplicar un idioma que se encuentra desactivado.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                GestorIdioma.Instancia.CambiarIdioma(seleccionado);
                
                if (SERVICIOS.SessionManager.Instance.EstaLogueado())
                {
                    _idiomaBLL.GuardarIdiomaUsuario(SERVICIOS.SessionManager.Instance.UsuarioActual.Id, seleccionado.Id);
                    SERVICIOS.SessionManager.Instance.UsuarioActual.IdIdioma = seleccionado.Id;
                }

                MessageBox.Show($"Idioma '{seleccionado.Nombre}' aplicado al sistema.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnToggleActivo_Click(object sender, EventArgs e)
        {
            if (dgvIdiomas.CurrentRow != null)
            {
                var idiomaSeleccionado = (Idioma)dgvIdiomas.CurrentRow.DataBoundItem;
                try
                {
                    // Check if we are deactivating the currently active language
                    bool isCurrentlyActiveSessionLanguage = GestorIdioma.Instancia.IdiomaActual != null && 
                                                            GestorIdioma.Instancia.IdiomaActual.Id == idiomaSeleccionado.Id;

                    string actionType = idiomaSeleccionado.Activo ? "desactivar (eliminar)" : "activar";
                    DialogResult confirm = MessageBox.Show($"¿Está seguro que desea {actionType} el idioma '{idiomaSeleccionado.Nombre}'?", "Confirmar Acción", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes) return;

                    _idiomaBLL.ToggleEstadoIdioma(idiomaSeleccionado.Id);
                    MessageBox.Show($"El estado del idioma '{idiomaSeleccionado.Nombre}' ha sido modificado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (isCurrentlyActiveSessionLanguage && idiomaSeleccionado.Activo)
                    {
                        // The active language was deactivated! Default to any other active language
                        var activos = _idiomaBLL.ObtenerIdiomasActivos();
                        if (activos.Count > 0)
                        {
                            var nuevoActivo = activos[0];
                            GestorIdioma.Instancia.CambiarIdioma(nuevoActivo);
                            if (SERVICIOS.SessionManager.Instance.EstaLogueado())
                            {
                                _idiomaBLL.GuardarIdiomaUsuario(SERVICIOS.SessionManager.Instance.UsuarioActual.Id, nuevoActivo.Id);
                                SERVICIOS.SessionManager.Instance.UsuarioActual.IdIdioma = nuevoActivo.Id;
                            }
                            MessageBox.Show($"El idioma activo fue desactivado. Se ha cambiado automáticamente a '{nuevoActivo.Nombre}'.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    RefrescarIdiomas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cambiar el estado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un idioma de la grilla para cambiar su estado.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    public class RenglonTraduccion
    {
        public int IdControl { get; set; }
        public string Formulario { get; set; }
        public string Control { get; set; }
        public string Texto { get; set; }
    }
}
