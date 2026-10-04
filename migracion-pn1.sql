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
    ('dgvCaptaciones.estado', 'FormGestionarCaptaciones', 'Estado', 'Status');

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
