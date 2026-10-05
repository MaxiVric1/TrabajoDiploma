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
    ('btnCancelar', 'FormRegistrarContrato', 'Cancelar', 'Cancel'),
    -- FormGestionarContratos
    ('lblFiltroEstado', 'FormGestionarContratos', 'Estado:', 'Status:'),
    ('btnActualizar', 'FormGestionarContratos', 'Actualizar', 'Refresh'),
    ('grpInquilino', 'FormGestionarContratos', 'Inquilino', 'Tenant'),
    ('lblNombre', 'FormGestionarContratos', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormGestionarContratos', 'Apellido:', 'Last Name:'),
    ('lblDni', 'FormGestionarContratos', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormGestionarContratos', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormGestionarContratos', 'Email:', 'Email:'),
    ('grpRenovacion', 'FormGestionarContratos', N'Renovación', 'Renewal'),
    ('lblMesesExtension', 'FormGestionarContratos', 'Extender (meses):', 'Extend (months):'),
    ('lblNuevoMonto', 'FormGestionarContratos', 'Nuevo monto mensual:', 'New monthly rent:'),
    ('lblNuevoVencimiento', 'FormGestionarContratos', 'Nuevo vencimiento:', 'New end date:'),
    ('btnRenovar', 'FormGestionarContratos', 'Renovar Contrato', 'Renew Contract'),
    ('grpPagos', 'FormGestionarContratos', 'Historial de pagos', 'Payment history'),
    ('lblMesActual', 'FormGestionarContratos', 'Mes actual:', 'Current month:'),
    ('dgvContratos.nro', 'FormGestionarContratos', 'Nro Contrato', 'Contract No.'),
    ('dgvContratos.inquilino', 'FormGestionarContratos', 'Inquilino', 'Tenant'),
    ('dgvContratos.direccion', 'FormGestionarContratos', N'Dirección', 'Address'),
    ('dgvContratos.tipo', 'FormGestionarContratos', 'Tipo', 'Type'),
    ('dgvContratos.monto', 'FormGestionarContratos', 'Monto Mensual', 'Monthly Rent'),
    ('dgvContratos.fechaInicio', 'FormGestionarContratos', 'Fecha Inicio', 'Start Date'),
    ('dgvContratos.fechaFin', 'FormGestionarContratos', 'Fecha Fin', 'End Date'),
    ('dgvContratos.estado', 'FormGestionarContratos', 'Estado', 'Status'),
    ('dgvContratos.alerta', 'FormGestionarContratos', 'Alerta Vencimiento', 'Expiry Alert'),
    ('dgvPagos.mes', 'FormGestionarContratos', 'Mes', 'Month'),
    ('dgvPagos.anio', 'FormGestionarContratos', N'Año', 'Year'),
    ('dgvPagos.estado', 'FormGestionarContratos', 'Estado', 'Status'),
    ('dgvPagos.monto', 'FormGestionarContratos', 'Monto Pagado', 'Amount Paid'),
    ('dgvPagos.recargo', 'FormGestionarContratos', 'Recargo', 'Surcharge'),
    ('dgvPagos.fechaPago', 'FormGestionarContratos', 'Fecha de Pago', 'Payment Date'),
    -- FormRegistrarPago
    ('grpContrato', 'FormRegistrarPago', 'Contrato', 'Contract'),
    ('lblNroContrato', 'FormRegistrarPago', 'Nro Contrato:', 'Contract No.:'),
    ('btnBuscar', 'FormRegistrarPago', 'Buscar', 'Search'),
    ('lblInquilino', 'FormRegistrarPago', 'Inquilino:', 'Tenant:'),
    ('lblDireccion', 'FormRegistrarPago', N'Dirección:', 'Address:'),
    ('lblMontoMensual', 'FormRegistrarPago', 'Monto mensual:', 'Monthly rent:'),
    ('lblEstadoContrato', 'FormRegistrarPago', 'Estado:', 'Status:'),
    ('grpPeriodo', 'FormRegistrarPago', N'Período a pagar', 'Period to pay'),
    ('lblPeriodo', 'FormRegistrarPago', N'Período:', 'Period:'),
    ('lblEstadoMes', 'FormRegistrarPago', 'Estado del mes:', 'Month status:'),
    ('lblFechaPago', 'FormRegistrarPago', 'Fecha de pago:', 'Payment date:'),
    ('lblMontoBase', 'FormRegistrarPago', 'Alquiler:', 'Rent:'),
    ('lblRecargo', 'FormRegistrarPago', 'Recargo (10%):', 'Surcharge (10%):'),
    ('lblTotal', 'FormRegistrarPago', 'Total a pagar:', 'Total due:'),
    ('grpResumen', 'FormRegistrarPago', 'Resumen del pago', 'Payment summary'),
    ('btnRegistrar', 'FormRegistrarPago', 'Registrar Pago', 'Register Payment'),
    -- FormGestionarMora
    ('grpInquilino', 'FormGestionarMora', 'Inquilino', 'Tenant'),
    ('lblNombre', 'FormGestionarMora', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormGestionarMora', 'Apellido:', 'Last Name:'),
    ('lblDni', 'FormGestionarMora', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormGestionarMora', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormGestionarMora', 'Email:', 'Email:'),
    ('grpContrato', 'FormGestionarMora', 'Contrato y deuda', 'Contract and debt'),
    ('lblDireccion', 'FormGestionarMora', N'Dirección:', 'Address:'),
    ('lblMontoMensual', 'FormGestionarMora', 'Monto mensual:', 'Monthly rent:'),
    ('lblFechaFin', 'FormGestionarMora', 'Fin del contrato:', 'Contract end:'),
    ('lblDiasMora', 'FormGestionarMora', N'Días en mora:', 'Days overdue:'),
    ('lblMontoAdeudado', 'FormGestionarMora', 'Con recargo:', 'With surcharge:'),
    ('btnEscalar', 'FormGestionarMora', 'Escalar a Legal', 'Escalate to Legal'),
    ('grpContactos', 'FormGestionarMora', 'Contactos con el inquilino', 'Tenant contacts'),
    ('lblFechaContacto', 'FormGestionarMora', 'Fecha:', 'Date:'),
    ('lblDescripcion', 'FormGestionarMora', N'Qué se habló:', 'What was discussed:'),
    ('btnRegistrarContacto', 'FormGestionarMora', 'Registrar Contacto', 'Register Contact'),
    ('dgvPagos.nro', 'FormGestionarMora', 'Nro Contrato', 'Contract No.'),
    ('dgvPagos.inquilino', 'FormGestionarMora', 'Inquilino', 'Tenant'),
    ('dgvPagos.direccion', 'FormGestionarMora', N'Dirección', 'Address'),
    ('dgvPagos.periodo', 'FormGestionarMora', N'Período', 'Period'),
    ('dgvPagos.montoAdeudado', 'FormGestionarMora', 'Monto Adeudado', 'Amount Owed'),
    ('dgvPagos.diasMora', 'FormGestionarMora', N'Días en Mora', 'Days Overdue'),
    ('dgvContactos.fecha', 'FormGestionarMora', 'Fecha', 'Date'),
    ('dgvContactos.descripcion', 'FormGestionarMora', N'Descripción', 'Description'),
    ('dgvContactos.analista', 'FormGestionarMora', 'Analista', 'Analyst'),
    -- FormGestionarLitigios
    ('grpInquilino', 'FormGestionarLitigios', 'Inquilino', 'Tenant'),
    ('lblNombre', 'FormGestionarLitigios', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormGestionarLitigios', 'Apellido:', 'Last Name:'),
    ('lblDni', 'FormGestionarLitigios', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormGestionarLitigios', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormGestionarLitigios', 'Email:', 'Email:'),
    ('grpContrato', 'FormGestionarLitigios', 'Contrato y deuda', 'Contract and debt'),
    ('lblDireccion', 'FormGestionarLitigios', N'Dirección:', 'Address:'),
    ('lblMontoMensual', 'FormGestionarLitigios', 'Monto mensual:', 'Monthly rent:'),
    ('lblFechaFin', 'FormGestionarLitigios', 'Fin del contrato:', 'Contract end:'),
    ('lblDiasLitigio', 'FormGestionarLitigios', N'Días en litigio:', 'Days in litigation:'),
    ('lblMontoAdeudado', 'FormGestionarLitigios', 'Con recargo:', 'With surcharge:'),
    ('lblDeudaTotal', 'FormGestionarLitigios', 'Deuda en litigio del contrato:', 'Contract debt in litigation:'),
    ('grpAcciones', 'FormGestionarLitigios', 'Acciones legales', 'Legal actions'),
    ('lblFechaAccion', 'FormGestionarLitigios', 'Fecha:', 'Date:'),
    ('lblDescripcion', 'FormGestionarLitigios', N'Descripción:', 'Description:'),
    ('btnIntimacion', 'FormGestionarLitigios', N'Registrar Intimación', 'Register Formal Notice'),
    ('btnResuelto', 'FormGestionarLitigios', 'Marcar Resuelto', 'Mark Resolved'),
    ('btnDesalojo', 'FormGestionarLitigios', 'Registrar Desalojo', 'Register Eviction'),
    ('dgvPagos.nro', 'FormGestionarLitigios', 'Nro Contrato', 'Contract No.'),
    ('dgvPagos.inquilino', 'FormGestionarLitigios', 'Inquilino', 'Tenant'),
    ('dgvPagos.direccion', 'FormGestionarLitigios', N'Dirección', 'Address'),
    ('dgvPagos.periodo', 'FormGestionarLitigios', N'Período', 'Period'),
    ('dgvPagos.montoAdeudado', 'FormGestionarLitigios', 'Monto Adeudado', 'Amount Owed'),
    ('dgvPagos.diasLitigio', 'FormGestionarLitigios', N'Días en Litigio', 'Days in Litigation'),
    ('dgvAcciones.fecha', 'FormGestionarLitigios', 'Fecha', 'Date'),
    ('dgvAcciones.tipo', 'FormGestionarLitigios', 'Tipo', 'Type'),
    ('dgvAcciones.descripcion', 'FormGestionarLitigios', N'Descripción', 'Description');

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
