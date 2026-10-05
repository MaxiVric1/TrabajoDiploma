-- ====================================================
-- Migración: menú PN2 y sus formularios
-- Agrega controles y traducciones a una base existente sin borrar datos.
-- Se puede ejecutar más de una vez.
-- ====================================================
USE [TpIngSoft];
GO

DECLARE @Nuevos TABLE ([nombre] NVARCHAR(100), [formulario] NVARCHAR(100), [es] NVARCHAR(500), [en] NVARCHAR(500));
INSERT INTO @Nuevos VALUES
    -- Menú
    ('pn2ToolStripMenuItem', 'FormMain', 'PN2', 'PN2'),
    ('gestionarContratosToolStripMenuItem', 'FormMain', 'Gestionar Contratos', 'Manage Contracts'),
    ('registrarContratoToolStripMenuItem', 'FormMain', 'Registrar Contrato', 'Register Contract'),
    ('registrarPagoToolStripMenuItem', 'FormMain', 'Registrar Pago', 'Register Payment'),
    ('gestionarMoraToolStripMenuItem', 'FormMain', 'Gestionar Mora', 'Manage Arrears'),
    ('gestionarLitigiosToolStripMenuItem', 'FormMain', 'Gestionar Litigios', 'Manage Litigation'),
    ('contratosPorVencerToolStripMenuItem', 'FormMain', 'Contratos Por Vencer', 'Expiring Contracts'),
    -- Títulos de los formularios
    ('(form)', 'FormGestionarContratos', 'Gestionar Contratos', 'Manage Contracts'),
    ('(form)', 'FormRegistrarContrato', 'Registrar Contrato', 'Register Contract'),
    ('(form)', 'FormRegistrarPago', 'Registrar Pago', 'Register Payment'),
    ('(form)', 'FormGestionarMora', 'Gestionar Mora', 'Manage Arrears'),
    ('(form)', 'FormGestionarLitigios', 'Gestionar Litigios', 'Manage Litigation'),
    ('(form)', 'FormContratosPorVencer', 'Contratos Por Vencer', 'Expiring Contracts'),
    -- FormRegistrarContrato
    ('grpInquilino', 'FormRegistrarContrato', 'Inquilino', 'Tenant'),
    ('lblDni', 'FormRegistrarContrato', 'DNI:', 'ID Number:'),
    ('btnBuscarInquilino', 'FormRegistrarContrato', 'Buscar', 'Search'),
    ('lblNombre', 'FormRegistrarContrato', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormRegistrarContrato', 'Apellido:', 'Last Name:'),
    ('lblTelefono', 'FormRegistrarContrato', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormRegistrarContrato', 'Email:', 'Email:'),
    ('grpInmueble', 'FormRegistrarContrato', 'Inmueble', 'Property'),
    ('lblInmueble', 'FormRegistrarContrato', 'Disponibles:', 'Available:'),
    ('btnNuevoInmueble', 'FormRegistrarContrato', 'Nuevo Inmueble', 'New Property'),
    ('lblNuevaDireccion', 'FormRegistrarContrato', N'Dirección / tipo:', 'Address / type:'),
    ('btnAgregarInmueble', 'FormRegistrarContrato', 'Agregar', 'Add'),
    ('btnCancelarInmueble', 'FormRegistrarContrato', 'Cancelar', 'Cancel'),
    ('grpContrato', 'FormRegistrarContrato', 'Contrato', 'Contract'),
    ('lblFechaInicio', 'FormRegistrarContrato', 'Fecha de inicio:', 'Start date:'),
    ('lblDuracion', 'FormRegistrarContrato', N'Duración (meses):', 'Length (months):'),
    ('lblMonto', 'FormRegistrarContrato', 'Monto mensual:', 'Monthly rent:'),
    ('lblFechaFin', 'FormRegistrarContrato', 'Finaliza:', 'Ends:'),
    ('btnGuardar', 'FormRegistrarContrato', 'Guardar', 'Save'),
    ('btnCancelar', 'FormRegistrarContrato', 'Cancelar', 'Cancel');

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
