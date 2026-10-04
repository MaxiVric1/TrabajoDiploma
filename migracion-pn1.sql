-- ====================================================
-- Migración: menú PN1 y sus formularios
-- Agrega controles y traducciones a una base existente sin borrar datos.
-- Se puede ejecutar más de una vez.
-- ====================================================
USE [TpIngSoft];
GO

DECLARE @Nuevos TABLE ([nombre] NVARCHAR(100), [formulario] NVARCHAR(100), [es] NVARCHAR(500), [en] NVARCHAR(500));
INSERT INTO @Nuevos VALUES
    ('pn1ToolStripMenuItem', 'FormMain', 'PN1', 'PN1'),
    ('registrarCaptacionToolStripMenuItem', 'FormMain', N'Registrar Captación', 'Register Listing'),
    ('gestionarCaptacionesToolStripMenuItem', 'FormMain', 'Gestionar Captaciones', 'Manage Listings'),
    ('validarTasacionToolStripMenuItem', 'FormMain', N'Validar Tasación', 'Validate Appraisal'),
    ('cargarMaterialAudiovisualToolStripMenuItem', 'FormMain', 'Cargar Material Audiovisual', 'Upload Audiovisual Material'),
    ('publicarInmuebleToolStripMenuItem', 'FormMain', 'Publicar Inmueble', 'Publish Property'),
    ('(form)', 'FormRegistrarCaptacion', N'Registrar Captación', 'Register Listing'),
    ('(form)', 'FormGestionarCaptaciones', 'Gestionar Captaciones', 'Manage Listings'),
    ('(form)', 'FormValidarTasacion', N'Validar Tasación', 'Validate Appraisal'),
    ('(form)', 'FormCargarMaterialAudiovisual', 'Cargar Material Audiovisual', 'Upload Audiovisual Material'),
    ('(form)', 'FormPublicarInmueble', 'Publicar Inmueble', 'Publish Property'),
    -- FormRegistrarCaptacion
    ('grpCliente', 'FormRegistrarCaptacion', 'Cliente', 'Client'),
    ('lblDni', 'FormRegistrarCaptacion', 'DNI:', 'ID Number:'),
    ('btnBuscarCliente', 'FormRegistrarCaptacion', 'Buscar', 'Search'),
    ('lblNombre', 'FormRegistrarCaptacion', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormRegistrarCaptacion', 'Apellido:', 'Last Name:'),
    ('lblTelefono', 'FormRegistrarCaptacion', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormRegistrarCaptacion', 'Email:', 'Email:'),
    ('grpInmueble', 'FormRegistrarCaptacion', 'Inmueble', 'Property'),
    ('lblTipo', 'FormRegistrarCaptacion', 'Tipo:', 'Type:'),
    ('lblDireccion', 'FormRegistrarCaptacion', N'Dirección:', 'Address:'),
    ('lblMetros', 'FormRegistrarCaptacion', 'Metros cuadrados:', 'Square meters:'),
    ('lblAmbientes', 'FormRegistrarCaptacion', 'Ambientes:', 'Rooms:'),
    ('grpCaptacion', 'FormRegistrarCaptacion', N'Captación', 'Listing'),
    ('lblValorPretendido', 'FormRegistrarCaptacion', 'Valor pretendido:', 'Asking price:'),
    ('chkAutorizacionRecibida', 'FormRegistrarCaptacion', N'Autorización de Venta firmada recibida', 'Signed Sale Authorization received'),
    ('btnGuardar', 'FormRegistrarCaptacion', 'Guardar', 'Save'),
    ('btnCancelar', 'FormRegistrarCaptacion', 'Cancelar', 'Cancel'),
    -- FormGestionarCaptaciones
    ('lblFiltroEstado', 'FormGestionarCaptaciones', 'Estado:', 'Status:'),
    ('btnActualizar', 'FormGestionarCaptaciones', 'Actualizar', 'Refresh'),
    ('grpRenegociacion', 'FormGestionarCaptaciones', N'Renegociación', 'Renegotiation'),
    ('lblDictamen', 'FormGestionarCaptaciones', N'Último dictamen:', 'Latest appraisal:'),
    ('lblValorActual', 'FormGestionarCaptaciones', 'Valor pretendido actual:', 'Current asking price:'),
    ('lblMotivo', 'FormGestionarCaptaciones', 'Motivo de rechazo:', 'Rejection reason:'),
    ('lblValorSugerido', 'FormGestionarCaptaciones', 'Valor sugerido:', 'Suggested value:'),
    ('lblNuevoValor', 'FormGestionarCaptaciones', 'Nuevo valor pretendido:', 'New asking price:'),
    ('btnRenegociar', 'FormGestionarCaptaciones', 'Renegociar', 'Renegotiate'),
    ('dgvCaptaciones.nro', 'FormGestionarCaptaciones', N'Nro Captación', 'Listing No.'),
    ('dgvCaptaciones.fecha', 'FormGestionarCaptaciones', 'Fecha', 'Date'),
    ('dgvCaptaciones.cliente', 'FormGestionarCaptaciones', 'Cliente', 'Client'),
    ('dgvCaptaciones.direccion', 'FormGestionarCaptaciones', N'Dirección', 'Address'),
    ('dgvCaptaciones.tipo', 'FormGestionarCaptaciones', 'Tipo', 'Type'),
    ('dgvCaptaciones.valor', 'FormGestionarCaptaciones', 'Valor Pretendido', 'Asking Price'),
    ('dgvCaptaciones.estado', 'FormGestionarCaptaciones', 'Estado', 'Status'),
    -- FormValidarTasacion
    ('lblPendientes', 'FormValidarTasacion', N'Captaciones pendientes de tasación', 'Listings pending appraisal'),
    ('btnActualizar', 'FormValidarTasacion', 'Actualizar', 'Refresh'),
    ('grpDetalle', 'FormValidarTasacion', N'Detalle de la captación', 'Listing details'),
    ('lblCliente', 'FormValidarTasacion', 'Cliente:', 'Client:'),
    ('lblDni', 'FormValidarTasacion', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormValidarTasacion', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormValidarTasacion', 'Email:', 'Email:'),
    ('lblValorPretendido', 'FormValidarTasacion', 'Valor pretendido:', 'Asking price:'),
    ('lblDictamenAnterior', 'FormValidarTasacion', 'Dictamen anterior:', 'Previous appraisal:'),
    ('grpDictamen', 'FormValidarTasacion', 'Dictamen', 'Appraisal decision'),
    ('rbAprobar', 'FormValidarTasacion', 'Aprobar', 'Approve'),
    ('rbRechazar', 'FormValidarTasacion', 'Rechazar', 'Reject'),
    ('lblValorFinal', 'FormValidarTasacion', 'Valor final:', 'Final value:'),
    ('lblMotivo', 'FormValidarTasacion', 'Motivo de rechazo:', 'Rejection reason:'),
    ('lblValorSugerido', 'FormValidarTasacion', 'Valor sugerido (opcional):', 'Suggested value (optional):'),
    ('btnRegistrar', 'FormValidarTasacion', 'Registrar dictamen', 'Register decision'),
    ('dgvPendientes.nro', 'FormValidarTasacion', N'Nro Captación', 'Listing No.'),
    ('dgvPendientes.fecha', 'FormValidarTasacion', 'Fecha', 'Date'),
    ('dgvPendientes.cliente', 'FormValidarTasacion', 'Cliente', 'Client'),
    ('dgvPendientes.direccion', 'FormValidarTasacion', N'Dirección', 'Address'),
    ('dgvPendientes.tipo', 'FormValidarTasacion', 'Tipo', 'Type'),
    ('dgvPendientes.metros', 'FormValidarTasacion', 'Metros Cuadrados', 'Square Meters'),
    ('dgvPendientes.ambientes', 'FormValidarTasacion', 'Ambientes', 'Rooms'),
    ('dgvPendientes.valor', 'FormValidarTasacion', 'Valor Pretendido', 'Asking Price'),
    -- FormCargarMaterialAudiovisual
    ('lblInmuebles', 'FormCargarMaterialAudiovisual', N'Inmuebles con tasación aprobada', 'Properties with approved appraisal'),
    ('btnActualizar', 'FormCargarMaterialAudiovisual', 'Actualizar', 'Refresh'),
    ('grpNuevoMaterial', 'FormCargarMaterialAudiovisual', 'Nuevo material', 'New material'),
    ('lblTextoComercial', 'FormCargarMaterialAudiovisual', 'Texto comercial:', 'Marketing text:'),
    ('lblArchivos', 'FormCargarMaterialAudiovisual', 'Archivos:', 'Files:'),
    ('btnAgregarArchivo', 'FormCargarMaterialAudiovisual', 'Agregar archivo...', 'Add file...'),
    ('btnGuardar', 'FormCargarMaterialAudiovisual', 'Guardar', 'Save'),
    ('grpMaterialCargado', 'FormCargarMaterialAudiovisual', 'Material ya cargado', 'Uploaded material'),
    ('dgvInmuebles.nro', 'FormCargarMaterialAudiovisual', N'Nro Captación', 'Listing No.'),
    ('dgvInmuebles.cliente', 'FormCargarMaterialAudiovisual', 'Cliente', 'Client'),
    ('dgvInmuebles.direccion', 'FormCargarMaterialAudiovisual', N'Dirección', 'Address'),
    ('dgvInmuebles.tipo', 'FormCargarMaterialAudiovisual', 'Tipo', 'Type'),
    ('dgvInmuebles.valorFinal', 'FormCargarMaterialAudiovisual', N'Valor Final de Tasación', 'Final Appraised Value'),
    ('dgvArchivos.archivo', 'FormCargarMaterialAudiovisual', 'Archivo', 'File'),
    ('dgvArchivos.tipo', 'FormCargarMaterialAudiovisual', 'Tipo', 'Type'),
    ('dgvArchivos.quitar', 'FormCargarMaterialAudiovisual', 'Quitar', 'Remove'),
    ('dgvMaterialCargado.tipo', 'FormCargarMaterialAudiovisual', 'Tipo', 'Type'),
    ('dgvMaterialCargado.archivo', 'FormCargarMaterialAudiovisual', 'Archivo', 'File'),
    ('dgvMaterialCargado.fecha', 'FormCargarMaterialAudiovisual', 'Fecha de carga', 'Upload date');

INSERT INTO [dbo].[Control] ([nombre], [formulario])
SELECT n.[nombre], n.[formulario]
FROM @Nuevos n
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Control] c WHERE c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]);

INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto])
SELECT c.[id], 1, n.[es]
FROM @Nuevos n
JOIN [dbo].[Control] c ON c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Traducciones] t WHERE t.[idcontrol] = c.[id] AND t.[ididioma] = 1);

INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto])
SELECT c.[id], 2, n.[en]
FROM @Nuevos n
JOIN [dbo].[Control] c ON c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]
WHERE EXISTS (SELECT 1 FROM [dbo].[Idioma] WHERE [id] = 2)
  AND NOT EXISTS (SELECT 1 FROM [dbo].[Traducciones] t WHERE t.[idcontrol] = c.[id] AND t.[ididioma] = 2);
GO
