-- ====================================================
-- TpIngSoft - Actualización del schema (PN1, PN2 y PN3)
-- Lleva cualquier base, recién creada con schema-init.sql o ya existente con datos, a la versión actual de las tablas.
-- Cada cambio se aplica solo si falta: se puede ejecutar siempre y más de una vez, sin perder datos.
-- El instalador lo ejecuta en toda instalación, entre schema-init.sql y stored-procedures.sql.
--
-- Cambios:
--   1. Inmueble: tipo_publicacion ('Venta' / 'Alquiler', por defecto 'Venta') y estado 'Alquilado'
--      (Captado -> Tasado -> Publicado -> Alquilado -> Publicado al finalizar el contrato).
--   2. InmuebleAlquiler: vínculo opcional con el Inmueble del PN1 (id_inmueble). Los inmuebles de alquiler
--      cargados antes quedan sin vínculo y siguen funcionando.
--   3. Contrato: fecha_fin (calculada y guardada), depósito de garantía, comisión, método de pago, actualización
--      por índice (IPC cada 3 meses), rescisión y entrega del inmueble con devolución del depósito.
--   4. Garante: garantes de cada contrato.
--   4b. APIs del BCRA: historial de actualizaciones de monto por IPC y valores consultados a la API (IPC y TIM).
--   5. Traducciones (español e inglés) de los controles agregados a los formularios; baja de las de los quitados.
-- ====================================================
USE [TpIngSoft];
GO

-- Requeridas por los índices filtrados (Contrato e InmuebleAlquiler)
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
GO

-- ====================================================
-- 1. Inmueble (PN1)
-- ====================================================

IF COL_LENGTH('dbo.Inmueble', 'tipo_publicacion') IS NULL
BEGIN
    -- Los inmuebles existentes eran todos de venta
    ALTER TABLE [dbo].[Inmueble] ADD [tipo_publicacion] VARCHAR(10) NOT NULL
        CONSTRAINT [DF_Inmueble_TipoPublicacion] DEFAULT 'Venta';
    PRINT N'Inmueble: columna tipo_publicacion agregada (existentes = Venta).';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Inmueble_TipoPublicacion')
    ALTER TABLE [dbo].[Inmueble] ADD CONSTRAINT [CK_Inmueble_TipoPublicacion]
        CHECK ([tipo_publicacion] IN ('Venta', 'Alquiler'));
GO

-- Estado 'Alquilado': se reemplaza el CHECK original de estado_actual (creado sin nombre en schema-init.sql)
-- por uno con nombre que incluye el estado nuevo
DECLARE @CheckEstado SYSNAME = (
    SELECT TOP 1 cc.[name]
    FROM sys.check_constraints cc
    JOIN sys.columns c ON c.[object_id] = cc.parent_object_id AND c.column_id = cc.parent_column_id
    WHERE cc.parent_object_id = OBJECT_ID('dbo.Inmueble') AND c.[name] = 'estado_actual'
      AND cc.[name] <> 'CK_Inmueble_EstadoActual');
IF @CheckEstado IS NOT NULL
BEGIN
    DECLARE @Sql NVARCHAR(400) = N'ALTER TABLE [dbo].[Inmueble] DROP CONSTRAINT ' + QUOTENAME(@CheckEstado) + N';';
    EXEC (@Sql);
    PRINT N'Inmueble: CHECK de estado_actual anterior quitado (' + @CheckEstado + N').';
END
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Inmueble_EstadoActual')
BEGIN
    ALTER TABLE [dbo].[Inmueble] ADD CONSTRAINT [CK_Inmueble_EstadoActual]
        CHECK ([estado_actual] IN ('Captado', 'Tasado', 'Rechazado', 'Publicado', 'Alquilado'));
    PRINT N'Inmueble: estado Alquilado habilitado.';
END
GO

-- Solo un inmueble de alquiler puede estar Alquilado
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Inmueble_AlquiladoEsAlquiler')
    ALTER TABLE [dbo].[Inmueble] ADD CONSTRAINT [CK_Inmueble_AlquiladoEsAlquiler]
        CHECK ([estado_actual] <> 'Alquilado' OR [tipo_publicacion] = 'Alquiler');
GO

-- ====================================================
-- 2. InmuebleAlquiler (PN2) -> Inmueble (PN1)
-- ====================================================

IF COL_LENGTH('dbo.InmuebleAlquiler', 'id_inmueble') IS NULL
BEGIN
    ALTER TABLE [dbo].[InmuebleAlquiler] ADD [id_inmueble] INT NULL
        CONSTRAINT [FK_InmuebleAlquiler_Inmueble] FOREIGN KEY REFERENCES [dbo].[Inmueble]([id_inmueble]);
    PRINT N'InmuebleAlquiler: vínculo id_inmueble con el PN1 agregado (existentes sin vínculo).';
END
GO

-- Cada inmueble del PN1 tiene como mucho un registro de alquiler (los sin vínculo no cuentan)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = 'UX_InmuebleAlquiler_Inmueble')
    CREATE UNIQUE INDEX [UX_InmuebleAlquiler_Inmueble] ON [dbo].[InmuebleAlquiler]([id_inmueble])
        WHERE [id_inmueble] IS NOT NULL;
GO

-- ====================================================
-- 3. Contrato (PN2)
-- ====================================================

-- Fecha de fin: calculada por SQL Server y guardada (PERSISTED); siempre coincide con inicio + duración,
-- también después de una renovación
IF COL_LENGTH('dbo.Contrato', 'fecha_fin') IS NULL
BEGIN
    ALTER TABLE [dbo].[Contrato] ADD [fecha_fin] AS DATEADD(MONTH, [duracion_meses], [fecha_inicio]) PERSISTED;
    PRINT N'Contrato: fecha_fin agregada (calculada y guardada).';
END
GO

IF COL_LENGTH('dbo.Contrato', 'deposito_garantia') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [deposito_garantia] DECIMAL(18,2) NOT NULL
        CONSTRAINT [DF_Contrato_DepositoGarantia] DEFAULT 0
        CONSTRAINT [CK_Contrato_DepositoGarantia] CHECK ([deposito_garantia] >= 0);
GO

IF COL_LENGTH('dbo.Contrato', 'comision_inmobiliaria') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [comision_inmobiliaria] DECIMAL(18,2) NOT NULL
        CONSTRAINT [DF_Contrato_ComisionInmobiliaria] DEFAULT 0
        CONSTRAINT [CK_Contrato_ComisionInmobiliaria] CHECK ([comision_inmobiliaria] >= 0);
GO

-- Los contratos existentes quedan con Transferencia
IF COL_LENGTH('dbo.Contrato', 'metodo_pago') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [metodo_pago] VARCHAR(20) NOT NULL
        CONSTRAINT [DF_Contrato_MetodoPago] DEFAULT 'Transferencia'
        CONSTRAINT [CK_Contrato_MetodoPago] CHECK ([metodo_pago] IN ('Transferencia', 'Efectivo', 'Cheque'));
GO

-- Actualización del monto por índice: hoy siempre IPC cada 3 meses. Se guardan en el contrato para que una
-- política distinta en el futuro no cambie los contratos ya firmados.
IF COL_LENGTH('dbo.Contrato', 'indice_actualizacion') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [indice_actualizacion] VARCHAR(10) NOT NULL
        CONSTRAINT [DF_Contrato_IndiceActualizacion] DEFAULT 'IPC'
        CONSTRAINT [CK_Contrato_IndiceActualizacion] CHECK ([indice_actualizacion] IN ('IPC'));
GO

IF COL_LENGTH('dbo.Contrato', 'periodicidad_actualizacion_meses') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [periodicidad_actualizacion_meses] INT NOT NULL
        CONSTRAINT [DF_Contrato_PeriodicidadActualizacion] DEFAULT 3
        CONSTRAINT [CK_Contrato_PeriodicidadActualizacion] CHECK ([periodicidad_actualizacion_meses] BETWEEN 1 AND 12);
GO

-- Rescisión anticipada: quién la inició y la penalidad (solo en contratos finalizados)
IF COL_LENGTH('dbo.Contrato', 'rescision_iniciada_por') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [rescision_iniciada_por] VARCHAR(20) NULL
        CONSTRAINT [CK_Contrato_RescisionIniciadaPor] CHECK ([rescision_iniciada_por] IN ('Inquilino', 'Propietario'));
GO

IF COL_LENGTH('dbo.Contrato', 'penalidad_rescision') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [penalidad_rescision] DECIMAL(18,2) NULL;
GO

-- Entrega del inmueble al finalizar: estado en que se recibió y destino del depósito
IF COL_LENGTH('dbo.Contrato', 'estado_entrega') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [estado_entrega] NVARCHAR(1000) NULL;
GO

IF COL_LENGTH('dbo.Contrato', 'monto_descuento_deposito') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [monto_descuento_deposito] DECIMAL(18,2) NULL;
GO

IF COL_LENGTH('dbo.Contrato', 'monto_devolucion_deposito') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [monto_devolucion_deposito] DECIMAL(18,2) NULL;
GO

-- Coherencia de la rescisión: solo en contratos finalizados; la penalidad requiere una rescisión y no es negativa
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Contrato_Rescision')
    ALTER TABLE [dbo].[Contrato] ADD CONSTRAINT [CK_Contrato_Rescision] CHECK (
        ([rescision_iniciada_por] IS NULL OR [estado_contrato] = 'Finalizado')
        AND ([penalidad_rescision] IS NULL OR ([rescision_iniciada_por] IS NOT NULL AND [penalidad_rescision] >= 0)));
GO

-- Coherencia del depósito al finalizar: descuento y devolución se informan juntos, no son negativos y
-- suman el depósito
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Contrato_DevolucionDeposito')
    ALTER TABLE [dbo].[Contrato] ADD CONSTRAINT [CK_Contrato_DevolucionDeposito] CHECK (
        ([monto_descuento_deposito] IS NULL AND [monto_devolucion_deposito] IS NULL)
        OR ([monto_descuento_deposito] >= 0 AND [monto_devolucion_deposito] >= 0
            AND [monto_descuento_deposito] + [monto_devolucion_deposito] = [deposito_garantia]));
GO

-- Comisión inmobiliaria: es un PORCENTAJE que la inmobiliaria retiene de cada pago mensual (no un monto único).
-- Una sola vez (mientras el valor por defecto siga siendo 0): por defecto pasa a 4%, los contratos sin comisión
-- cargada toman el 4% y los que tuvieran un monto guardado (versiones de prueba anteriores) se convierten al
-- porcentaje equivalente sobre el total del contrato.
IF EXISTS (SELECT 1 FROM sys.default_constraints
           WHERE [name] = 'DF_Contrato_ComisionInmobiliaria' AND [definition] IN ('((0))', '((0.00))'))
BEGIN
    ALTER TABLE [dbo].[Contrato] DROP CONSTRAINT [DF_Contrato_ComisionInmobiliaria];
    ALTER TABLE [dbo].[Contrato] ADD CONSTRAINT [DF_Contrato_ComisionInmobiliaria] DEFAULT 4 FOR [comision_inmobiliaria];
    UPDATE [dbo].[Contrato]
    SET [comision_inmobiliaria] = CASE
            WHEN [comision_inmobiliaria] = 0 THEN 4
            ELSE ROUND([comision_inmobiliaria] * 100 / ([monto_mensual] * [duracion_meses]), 2) END
    WHERE [comision_inmobiliaria] = 0 OR [comision_inmobiliaria] > 100;
    PRINT N'Contrato: comisión inmobiliaria convertida a porcentaje mensual (4% por defecto).';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Contrato_ComisionPorcentaje')
    ALTER TABLE [dbo].[Contrato] ADD CONSTRAINT [CK_Contrato_ComisionPorcentaje]
        CHECK ([comision_inmobiliaria] BETWEEN 0 AND 100);
GO

-- PagoAlquiler: comisión retenida en cada pago (monto en pesos); el neto al propietario es monto_pagado - comisión
IF COL_LENGTH('dbo.PagoAlquiler', 'comision_inmobiliaria') IS NULL
BEGIN
    ALTER TABLE [dbo].[PagoAlquiler] ADD [comision_inmobiliaria] DECIMAL(18,2) NOT NULL
        CONSTRAINT [DF_PagoAlquiler_ComisionInmobiliaria] DEFAULT 0
        CONSTRAINT [CK_PagoAlquiler_ComisionInmobiliaria] CHECK ([comision_inmobiliaria] >= 0);
    PRINT N'PagoAlquiler: columna comision_inmobiliaria agregada.';
END
GO

-- Pagos ya registrados sin comisión calculada: se completa con el porcentaje de su contrato (los de 0% quedan en 0)
UPDATE p SET [comision_inmobiliaria] = ROUND(p.[monto_pagado] * c.[comision_inmobiliaria] / 100, 2)
FROM [dbo].[PagoAlquiler] p
JOIN [dbo].[Contrato] c ON c.[id_contrato] = p.[id_contrato]
WHERE p.[fecha_pago] IS NOT NULL AND p.[monto_pagado] > 0 AND p.[comision_inmobiliaria] = 0 AND c.[comision_inmobiliaria] > 0;
GO

-- ====================================================
-- 4. Garante (PN2)
-- ====================================================

IF OBJECT_ID('[dbo].[Garante]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Garante] (
        [id_garante] INT IDENTITY(1,1) PRIMARY KEY,
        [id_contrato] INT NOT NULL CONSTRAINT [FK_Garante_Contrato] FOREIGN KEY REFERENCES [dbo].[Contrato]([id_contrato]),
        [nombre] NVARCHAR(100) NOT NULL,
        [apellido] NVARCHAR(100) NOT NULL,
        [dni] VARCHAR(20) NOT NULL,
        [domicilio] NVARCHAR(200) NOT NULL,
        [propiedad_garantia] NVARCHAR(200) NOT NULL, -- inmueble que respalda la garantía (dirección / datos registrales)
        -- Un contrato puede tener varios garantes, pero no la misma persona dos veces
        CONSTRAINT [UQ_Garante_ContratoDni] UNIQUE ([id_contrato], [dni])
    );
    PRINT N'Tabla Garante creada.';
END
GO

-- ====================================================
-- 4b. APIs del BCRA (PN2)
-- ====================================================

-- Historial de actualizaciones del monto mensual por índice. La actualización rige desde el período que
-- empieza en fecha_actualizacion (los meses anteriores conservan el monto anterior).
IF OBJECT_ID('[dbo].[ActualizacionContrato]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ActualizacionContrato] (
        [id_actualizacion] INT IDENTITY(1,1) PRIMARY KEY,
        [id_contrato] INT NOT NULL CONSTRAINT [FK_ActualizacionContrato_Contrato] FOREIGN KEY REFERENCES [dbo].[Contrato]([id_contrato]),
        [fecha_actualizacion] DATE NOT NULL,              -- primer día del período desde el que rige
        [fecha_registro] DATETIME NOT NULL CONSTRAINT [DF_ActualizacionContrato_FechaRegistro] DEFAULT GETDATE(),
        [indice] VARCHAR(10) NOT NULL CONSTRAINT [DF_ActualizacionContrato_Indice] DEFAULT 'IPC',
        [porcentaje] DECIMAL(9,4) NOT NULL,               -- variación acumulada aplicada, en %
        [detalle] NVARCHAR(200) NULL,                     -- valores mensuales usados
        [monto_anterior] DECIMAL(18,2) NOT NULL CONSTRAINT [CK_ActualizacionContrato_MontoAnterior] CHECK ([monto_anterior] > 0),
        [monto_nuevo] DECIMAL(18,2) NOT NULL CONSTRAINT [CK_ActualizacionContrato_MontoNuevo] CHECK ([monto_nuevo] > 0),
        [id_usuario] INT NULL CONSTRAINT [FK_ActualizacionContrato_Usuario] FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
        CONSTRAINT [UQ_ActualizacionContrato_Fecha] UNIQUE ([id_contrato], [fecha_actualizacion])
    );
    PRINT N'Tabla ActualizacionContrato creada.';
END
GO

-- Valores de las series del BCRA ya consultados (27 = inflación mensual, 1197 = TIM art. 768 CCC): evitan
-- consultar la API de nuevo y permiten trabajar sin conexión con lo ya descargado
IF OBJECT_ID('[dbo].[IndiceBCRA]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[IndiceBCRA] (
        [id_variable] INT NOT NULL,
        [fecha] DATE NOT NULL,
        [valor] DECIMAL(19,6) NOT NULL,
        [fecha_consulta] DATETIME NOT NULL CONSTRAINT [DF_IndiceBCRA_FechaConsulta] DEFAULT GETDATE(),
        CONSTRAINT [PK_IndiceBCRA] PRIMARY KEY ([id_variable], [fecha])
    );
    PRINT N'Tabla IndiceBCRA creada.';
END
GO

-- ====================================================
-- 5. Traducciones de los controles nuevos (español e inglés)
--    Van acá y no en schema-init.sql para que también las reciban las bases existentes.
--    Solo se agregan las que faltan: no pisa textos editados desde Gestión de Idiomas.
-- ====================================================

DECLARE @Nuevos TABLE ([nombre] NVARCHAR(100), [formulario] NVARCHAR(100), [es] NVARCHAR(500), [en] NVARCHAR(500));
INSERT INTO @Nuevos VALUES
    -- PN1: tipo de publicación (venta / alquiler)
    ('lblTipoPublicacion', 'FormRegistrarCaptacion', N'Operación:', 'Listing type:'),
    ('chkAutorizacionAlquiler', 'FormRegistrarCaptacion', N'Autorización de Alquiler firmada recibida', 'Signed Rental Authorization received'),
    ('dgvCaptaciones.tipoPublicacion', 'FormGestionarCaptaciones', N'Operación', 'Listing type'),
    ('dgvPendientes.tipoPublicacion', 'FormValidarTasacion', N'Operación', 'Listing type'),
    ('dgvInmuebles.tipoPublicacion', 'FormCargarMaterialAudiovisual', N'Operación', 'Listing type'),
    ('dgvInmuebles.tipoPublicacion', 'FormPublicarInmueble', N'Operación', 'Listing type'),
    ('dgvInmuebles.tipoPublicacion', 'FormInmueblesPublicados', N'Operación', 'Listing type'),
    -- PN2 (paso 4): FormRegistrarContrato
    ('tabInmueble', 'FormRegistrarContrato', 'Inmueble y Propietario', 'Property and Owner'),
    ('tabInquilino', 'FormRegistrarContrato', 'Inquilino', 'Tenant'),
    ('tabGarante', 'FormRegistrarContrato', 'Garante', 'Guarantor'),
    ('tabCondiciones', 'FormRegistrarContrato', 'Condiciones', 'Terms'),
    ('tabResumen', 'FormRegistrarContrato', 'Resumen y PDF', 'Summary and PDF'),
    ('grpDatosInmueble', 'FormRegistrarContrato', 'Datos del inmueble', 'Property details'),
    ('lblTipoInmueble', 'FormRegistrarContrato', 'Tipo:', 'Type:'),
    ('lblDireccion', 'FormRegistrarContrato', N'Dirección:', 'Address:'),
    ('lblSuperficie', 'FormRegistrarContrato', 'Superficie:', 'Area:'),
    ('lblAmbientes', 'FormRegistrarContrato', 'Ambientes:', 'Rooms:'),
    ('lblValorTasado', 'FormRegistrarContrato', 'Valor tasado:', 'Appraised value:'),
    ('grpPropietario', 'FormRegistrarContrato', 'Propietario', 'Owner'),
    ('lblPropietarioNombre', 'FormRegistrarContrato', 'Nombre:', 'Name:'),
    ('lblPropietarioDni', 'FormRegistrarContrato', 'DNI:', 'ID:'),
    ('lblPropietarioTelefono', 'FormRegistrarContrato', N'Teléfono:', 'Phone:'),
    ('lblPropietarioEmail', 'FormRegistrarContrato', 'Email:', 'Email:'),
    ('grpGarante', 'FormRegistrarContrato', 'Garante (obligatorio)', 'Guarantor (required)'),
    ('lblGaranteNombre', 'FormRegistrarContrato', 'Nombre:', 'First Name:'),
    ('lblGaranteApellido', 'FormRegistrarContrato', 'Apellido:', 'Last Name:'),
    ('lblGaranteDni', 'FormRegistrarContrato', 'DNI:', 'ID:'),
    ('lblGaranteDomicilio', 'FormRegistrarContrato', 'Domicilio:', 'Address:'),
    ('lblPropiedadGarantia', 'FormRegistrarContrato', N'Propiedad en garantía:', 'Collateral property:'),
    ('lblMetodoPago', 'FormRegistrarContrato', N'Método de pago:', 'Payment method:'),
    ('lblDeposito', 'FormRegistrarContrato', N'Depósito de garantía:', 'Security deposit:'),
    ('lblComision', 'FormRegistrarContrato', N'Comisión inmobiliaria:', 'Agency fee:'),
    ('lblActualizacion', 'FormRegistrarContrato', N'Actualización:', 'Rent adjustment:'),
    ('lblFirma', 'FormRegistrarContrato', 'Firma del inquilino (nombre y apellido):', 'Tenant signature (full name):'),
    ('btnRegistrar', 'FormRegistrarContrato', 'Registrar Contrato', 'Register Contract'),
    ('btnGenerarPdf', 'FormRegistrarContrato', 'Generar PDF', 'Generate PDF'),
    ('btnNuevoContrato', 'FormRegistrarContrato', 'Nuevo Contrato', 'New Contract'),
    ('btnAnterior', 'FormRegistrarContrato', '< Anterior', '< Back'),
    ('btnSiguiente', 'FormRegistrarContrato', 'Siguiente >', 'Next >'),
    -- PN2 (paso 4): FormGestionarContratos
    ('grpCondiciones', 'FormGestionarContratos', 'Condiciones', 'Terms'),
    ('lblDeposito', 'FormGestionarContratos', N'Depósito:', 'Deposit:'),
    ('lblComision', 'FormGestionarContratos', N'Comisión:', 'Agency fee:'),
    ('lblMetodoPago', 'FormGestionarContratos', N'Método de pago:', 'Payment method:'),
    ('lblIndice', 'FormGestionarContratos', N'Actualización:', 'Adjustment:'),
    ('lblGarante', 'FormGestionarContratos', 'Garante:', 'Guarantor:'),
    ('grpRescision', 'FormGestionarContratos', N'Rescisión', 'Early termination'),
    ('lblIniciadaPor', 'FormGestionarContratos', 'Iniciada por:', 'Initiated by:'),
    ('lblMesesRestantes', 'FormGestionarContratos', 'Canon restante:', 'Remaining rent:'),
    ('lblPenalidad', 'FormGestionarContratos', 'Penalidad:', 'Penalty:'),
    ('btnRescindir', 'FormGestionarContratos', 'Rescindir Contrato', 'Terminate Contract'),
    -- PN2 (paso 4): FormRegistrarPago
    ('lblMora', 'FormRegistrarPago', 'Mora:', 'Arrears:'),
    -- Comisión inmobiliaria mensual: desglose de cada pago
    ('lblComision', 'FormRegistrarPago', N'Comisión:', 'Agency fee:'),
    ('lblNeto', 'FormRegistrarPago', 'Neto propietario:', 'Net to owner:'),
    -- Mora con la TIM del BCRA (paso 5)
    ('lblTasa', 'FormRegistrarPago', 'Tasa de mora:', 'Late interest rate:');

INSERT INTO [dbo].[Control] ([nombre], [formulario])
SELECT n.[nombre], n.[formulario]
FROM @Nuevos n
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Control] c WHERE c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]);

INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto])
SELECT c.[id], 1, n.[es]
FROM @Nuevos n
JOIN [dbo].[Control] c ON c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]
WHERE EXISTS (SELECT 1 FROM [dbo].[Idioma] WHERE [id] = 1)
  AND NOT EXISTS (SELECT 1 FROM [dbo].[Traducciones] t WHERE t.[idcontrol] = c.[id] AND t.[ididioma] = 1);

INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto])
SELECT c.[id], 2, n.[en]
FROM @Nuevos n
JOIN [dbo].[Control] c ON c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]
WHERE EXISTS (SELECT 1 FROM [dbo].[Idioma] WHERE [id] = 2)
  AND NOT EXISTS (SELECT 1 FROM [dbo].[Traducciones] t WHERE t.[idcontrol] = c.[id] AND t.[ididioma] = 2);
GO

-- Controles que se quitaron de los formularios: sus textos ya no se usan
DECLARE @Quitados TABLE ([nombre] NVARCHAR(100), [formulario] NVARCHAR(100));
INSERT INTO @Quitados VALUES
    -- PN2: los inmuebles de alquiler vienen del PN1, ya no se cargan desde Registrar Contrato
    ('btnNuevoInmueble', 'FormRegistrarContrato'),
    ('lblNuevaDireccion', 'FormRegistrarContrato'),
    ('btnAgregarInmueble', 'FormRegistrarContrato'),
    ('btnCancelarInmueble', 'FormRegistrarContrato'),
    -- PN2 (paso 4): Registrar Contrato en pestañas
    ('grpInmueble', 'FormRegistrarContrato'),
    ('btnGuardar', 'FormRegistrarContrato');

DELETE t FROM [dbo].[Traducciones] t
JOIN [dbo].[Control] c ON c.[id] = t.[idcontrol]
JOIN @Quitados q ON q.[nombre] = c.[nombre] AND q.[formulario] = c.[formulario];

DELETE c FROM [dbo].[Control] c
JOIN @Quitados q ON q.[nombre] = c.[nombre] AND q.[formulario] = c.[formulario];
GO

-- Textos que cambiaron: solo si siguen con el texto original (no se pisan los editados en Gestión de Idiomas)
UPDATE t SET [texto] = CASE t.[ididioma] WHEN 1 THEN N'Recargo por mora:' ELSE N'Late fee:' END
FROM [dbo].[Traducciones] t
JOIN [dbo].[Control] c ON c.[id] = t.[idcontrol]
WHERE c.[nombre] = 'lblRecargo' AND c.[formulario] = 'FormRegistrarPago'
  AND t.[texto] IN (N'Recargo (10%):', N'Surcharge (10%):');
GO

PRINT N'Actualización del schema terminada.';
GO
