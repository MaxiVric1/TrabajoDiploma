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
--   4c. PN3: proyectos de inversión, inversores, suscripciones al fideicomiso, historial del ICC y sus roles.
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

-- Ubicación y lote (PN3): localidad para geocodificar (obligatoria en captaciones nuevas, la exige la BLL),
-- superficie del lote (obligatoria en casas, la exige la BLL: los inmuebles existentes no la tienen) y
-- coordenadas obtenidas de Nominatim
IF COL_LENGTH('dbo.Inmueble', 'localidad') IS NULL
BEGIN
    ALTER TABLE [dbo].[Inmueble] ADD [localidad] NVARCHAR(100) NULL;
    PRINT N'Inmueble: columna localidad agregada.';
END
GO

IF COL_LENGTH('dbo.Inmueble', 'superficie_lote') IS NULL
BEGIN
    ALTER TABLE [dbo].[Inmueble] ADD [superficie_lote] DECIMAL(10,2) NULL
        CONSTRAINT [CK_Inmueble_SuperficieLote] CHECK ([superficie_lote] > 0);
    PRINT N'Inmueble: columna superficie_lote agregada.';
END
GO

IF COL_LENGTH('dbo.Inmueble', 'latitud') IS NULL
BEGIN
    ALTER TABLE [dbo].[Inmueble] ADD [latitud] DECIMAL(9,6) NULL
        CONSTRAINT [CK_Inmueble_Latitud] CHECK ([latitud] BETWEEN -90 AND 90);
    ALTER TABLE [dbo].[Inmueble] ADD [longitud] DECIMAL(9,6) NULL
        CONSTRAINT [CK_Inmueble_Longitud] CHECK ([longitud] BETWEEN -180 AND 180);
    PRINT N'Inmueble: columnas latitud y longitud agregadas.';
END
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

-- Inquilino: domicilio (para el contrato); los existentes quedan sin domicilio cargado
IF COL_LENGTH('dbo.Inquilino', 'domicilio') IS NULL
BEGIN
    ALTER TABLE [dbo].[Inquilino] ADD [domicilio] NVARCHAR(200) NULL;
    PRINT N'Inquilino: columna domicilio agregada.';
END
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

-- Depósito de garantía = 1 mes de alquiler. Los contratos que ya existían cuando se agregó la columna quedaron con
-- $0: se completa con el monto del primer mes (el anterior a la primera actualización por IPC, igual que el
-- contrato en PDF). No se tocan los depósitos ya liquidados al finalizar el contrato.
UPDATE c SET [deposito_garantia] = ISNULL(
        (SELECT TOP 1 a.[monto_anterior] FROM [dbo].[ActualizacionContrato] a
         WHERE a.[id_contrato] = c.[id_contrato] ORDER BY a.[fecha_actualizacion], a.[id_actualizacion]),
        c.[monto_mensual])
FROM [dbo].[Contrato] c
WHERE c.[deposito_garantia] = 0 AND c.[monto_descuento_deposito] IS NULL AND c.[monto_devolucion_deposito] IS NULL;
IF @@ROWCOUNT > 0 PRINT N'Contratos: depósito de garantía completado con el primer mes de alquiler.';
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
-- 4c. PN3 - Evaluación de desarrollo de inversión
-- ====================================================

-- Proyecto de desarrollo en fideicomiso sobre un inmueble del inventario. Arquitectura carga los datos técnicos
-- (en CABA los consulta a EPOK: Código Urbanístico); los totales los calcula SQL Server. Montos por m² en dólares (USD).
--   metros_construibles = FLOOR(altura_maxima / 3 m por piso) x superficie_edificable_planta
--   costo_total_obra    = metros_construibles x costo_obra_m2
--   valor_total_unidades = metros_construibles x valor_venta_m2_unidades
IF OBJECT_ID('[dbo].[ProyectoInversion]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[ProyectoInversion] (
        [id_proyecto] INT IDENTITY(1,1) PRIMARY KEY,
        [id_inmueble] INT NOT NULL CONSTRAINT [FK_ProyectoInversion_Inmueble] FOREIGN KEY REFERENCES [dbo].[Inmueble]([id_inmueble]),
        [superficie_lote] DECIMAL(10,2) NULL CONSTRAINT [CK_ProyectoInversion_SuperficieLote] CHECK ([superficie_lote] > 0),
        [superficie_edificable_planta] DECIMAL(10,2) NULL CONSTRAINT [CK_ProyectoInversion_SuperficieEdificablePlanta] CHECK ([superficie_edificable_planta] > 0),
        [altura_maxima] DECIMAL(6,2) NULL CONSTRAINT [CK_ProyectoInversion_AlturaMaxima] CHECK ([altura_maxima] >= 3 AND [altura_maxima] <= 200),
        [smp] VARCHAR(20) NULL,                     -- sección-manzana-parcela (catastro de CABA)
        [distrito_urbanistico] VARCHAR(60) NULL,    -- distrito según EPOK (referencia)
        [metros_construibles] AS CAST(FLOOR([altura_maxima] / 3.0) * [superficie_edificable_planta] AS DECIMAL(12,2)) PERSISTED,
        [costo_obra_m2] DECIMAL(18,2) NULL CONSTRAINT [CK_ProyectoInversion_CostoObraM2] CHECK ([costo_obra_m2] > 0),
        [costo_total_obra] AS CAST(FLOOR([altura_maxima] / 3.0) * [superficie_edificable_planta] * [costo_obra_m2] AS DECIMAL(18,2)) PERSISTED,
        [valor_venta_m2_unidades] DECIMAL(18,2) NULL CONSTRAINT [CK_ProyectoInversion_ValorVentaM2] CHECK ([valor_venta_m2_unidades] > 0),
        [valor_total_unidades] AS CAST(FLOOR([altura_maxima] / 3.0) * [superficie_edificable_planta] * [valor_venta_m2_unidades] AS DECIMAL(18,2)) PERSISTED,
        [estado] VARCHAR(20) NOT NULL CONSTRAINT [DF_ProyectoInversion_Estado] DEFAULT 'EnAnalisis'
            CONSTRAINT [CK_ProyectoInversion_Estado] CHECK ([estado] IN ('EnAnalisis', 'Aprobado', 'Rechazado')),
        [id_usuario_arquitectura] INT NULL CONSTRAINT [FK_ProyectoInversion_Arquitectura] FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
        [id_usuario_directorio] INT NULL CONSTRAINT [FK_ProyectoInversion_Directorio] FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
        [fecha_creacion] DATETIME NOT NULL CONSTRAINT [DF_ProyectoInversion_FechaCreacion] DEFAULT GETDATE(),
        [fecha_decision] DATETIME NULL,
        [observaciones] NVARCHAR(1000) NULL,
        -- La decisión del Directorio queda registrada con quién y cuándo, y solo sobre un proyecto con datos técnicos
        CONSTRAINT [CK_ProyectoInversion_Decision] CHECK (
            ([estado] = 'EnAnalisis' AND [fecha_decision] IS NULL AND [id_usuario_directorio] IS NULL)
            OR ([estado] <> 'EnAnalisis' AND [fecha_decision] IS NOT NULL AND [id_usuario_directorio] IS NOT NULL
                AND [superficie_lote] IS NOT NULL AND [superficie_edificable_planta] IS NOT NULL AND [altura_maxima] IS NOT NULL
                AND [costo_obra_m2] IS NOT NULL AND [valor_venta_m2_unidades] IS NOT NULL))
    );
    PRINT N'Tabla ProyectoInversion creada.';
END
GO

-- Del modelo FOS/FOT al del Código Urbanístico (altura máxima y superficie edificable en planta), para las bases
-- creadas con la versión anterior. Los proyectos existentes conservan sus m² construibles (y con ellos el capital
-- de un fideicomiso ya aprobado): pisos = FOT / FOS redondeado hacia arriba, altura = pisos x 3 m y
-- planta = lote x FOT / pisos (nunca mayor que lote x FOS).
IF COL_LENGTH('dbo.ProyectoInversion', 'superficie_edificable_planta') IS NULL
BEGIN
    ALTER TABLE [dbo].[ProyectoInversion] ADD
        [superficie_edificable_planta] DECIMAL(10,2) NULL CONSTRAINT [CK_ProyectoInversion_SuperficieEdificablePlanta] CHECK ([superficie_edificable_planta] > 0),
        [altura_maxima] DECIMAL(6,2) NULL CONSTRAINT [CK_ProyectoInversion_AlturaMaxima] CHECK ([altura_maxima] >= 3 AND [altura_maxima] <= 200),
        [smp] VARCHAR(20) NULL,
        [distrito_urbanistico] VARCHAR(60) NULL;
    PRINT N'ProyectoInversion: columnas superficie_edificable_planta, altura_maxima, smp y distrito_urbanistico agregadas.';
END
GO

IF COL_LENGTH('dbo.ProyectoInversion', 'fot') IS NOT NULL
BEGIN
    EXEC(N'UPDATE [dbo].[ProyectoInversion]
           SET [superficie_edificable_planta] = ROUND([superficie_lote] * [fot] / CEILING([fot] / [fos]), 2),
               [altura_maxima] = CEILING([fot] / [fos]) * 3
           WHERE [fos] IS NOT NULL AND [fot] IS NOT NULL AND [superficie_lote] IS NOT NULL;');
    ALTER TABLE [dbo].[ProyectoInversion] DROP CONSTRAINT [CK_ProyectoInversion_Decision];
    ALTER TABLE [dbo].[ProyectoInversion] DROP COLUMN [metros_construibles], [costo_total_obra], [valor_total_unidades];
    ALTER TABLE [dbo].[ProyectoInversion] DROP CONSTRAINT [CK_ProyectoInversion_Fos], [CK_ProyectoInversion_Fot];
    ALTER TABLE [dbo].[ProyectoInversion] DROP COLUMN [fos], [fot];
    PRINT N'ProyectoInversion: FOS y FOT reemplazados por altura máxima y superficie edificable en planta.';
END
GO

IF COL_LENGTH('dbo.ProyectoInversion', 'metros_construibles') IS NULL
BEGIN
    ALTER TABLE [dbo].[ProyectoInversion] ADD
        [metros_construibles] AS CAST(FLOOR([altura_maxima] / 3.0) * [superficie_edificable_planta] AS DECIMAL(12,2)) PERSISTED,
        [costo_total_obra] AS CAST(FLOOR([altura_maxima] / 3.0) * [superficie_edificable_planta] * [costo_obra_m2] AS DECIMAL(18,2)) PERSISTED,
        [valor_total_unidades] AS CAST(FLOOR([altura_maxima] / 3.0) * [superficie_edificable_planta] * [valor_venta_m2_unidades] AS DECIMAL(18,2)) PERSISTED;
    PRINT N'ProyectoInversion: m² construibles, costo y valor recalculados con altura máxima y superficie edificable.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_ProyectoInversion_Decision')
    ALTER TABLE [dbo].[ProyectoInversion] ADD CONSTRAINT [CK_ProyectoInversion_Decision] CHECK (
        ([estado] = 'EnAnalisis' AND [fecha_decision] IS NULL AND [id_usuario_directorio] IS NULL)
        OR ([estado] <> 'EnAnalisis' AND [fecha_decision] IS NOT NULL AND [id_usuario_directorio] IS NOT NULL
            AND [superficie_lote] IS NOT NULL AND [superficie_edificable_planta] IS NOT NULL AND [altura_maxima] IS NOT NULL
            AND [costo_obra_m2] IS NOT NULL AND [valor_venta_m2_unidades] IS NOT NULL));
GO

-- Canje: porcentaje de las unidades que recibe el propietario por aportar el lote (lo define el Analista al crear
-- el proyecto)
IF COL_LENGTH('dbo.ProyectoInversion', 'porcentaje_canje') IS NULL
BEGIN
    ALTER TABLE [dbo].[ProyectoInversion] ADD [porcentaje_canje] DECIMAL(5,2) NOT NULL
        CONSTRAINT [DF_ProyectoInversion_PorcentajeCanje] DEFAULT 30
        CONSTRAINT [CK_ProyectoInversion_PorcentajeCanje] CHECK ([porcentaje_canje] > 0 AND [porcentaje_canje] < 100);
    PRINT N'ProyectoInversion: columna porcentaje_canje agregada (30% por defecto).';
END
GO

-- Comisiones de LandSmart sobre el fideicomiso (las define el Directorio mientras el proyecto está En Análisis):
-- estructuración sobre el costo de obra, administración mensual sobre el capital total del fideicomiso durante los meses de obra, y
-- venta de las unidades de los inversores
IF COL_LENGTH('dbo.ProyectoInversion', 'porcentaje_estructuracion') IS NULL
BEGIN
    ALTER TABLE [dbo].[ProyectoInversion] ADD [porcentaje_estructuracion] DECIMAL(5,2) NOT NULL
        CONSTRAINT [DF_ProyectoInversion_PorcentajeEstructuracion] DEFAULT 2
        CONSTRAINT [CK_ProyectoInversion_PorcentajeEstructuracion] CHECK ([porcentaje_estructuracion] >= 0 AND [porcentaje_estructuracion] < 100);
    PRINT N'ProyectoInversion: columna porcentaje_estructuracion agregada (2% por defecto).';
END
GO

IF COL_LENGTH('dbo.ProyectoInversion', 'porcentaje_administracion_mensual') IS NULL
BEGIN
    ALTER TABLE [dbo].[ProyectoInversion] ADD [porcentaje_administracion_mensual] DECIMAL(5,2) NOT NULL
        CONSTRAINT [DF_ProyectoInversion_PorcentajeAdministracionMensual] DEFAULT 1
        CONSTRAINT [CK_ProyectoInversion_PorcentajeAdministracionMensual] CHECK ([porcentaje_administracion_mensual] >= 0 AND [porcentaje_administracion_mensual] < 100);
    PRINT N'ProyectoInversion: columna porcentaje_administracion_mensual agregada (1% por defecto).';
END
GO

IF COL_LENGTH('dbo.ProyectoInversion', 'porcentaje_venta_unidades') IS NULL
BEGIN
    ALTER TABLE [dbo].[ProyectoInversion] ADD [porcentaje_venta_unidades] DECIMAL(5,2) NOT NULL
        CONSTRAINT [DF_ProyectoInversion_PorcentajeVentaUnidades] DEFAULT 5
        CONSTRAINT [CK_ProyectoInversion_PorcentajeVentaUnidades] CHECK ([porcentaje_venta_unidades] >= 0 AND [porcentaje_venta_unidades] < 100);
    PRINT N'ProyectoInversion: columna porcentaje_venta_unidades agregada (5% por defecto).';
END
GO

-- Un inmueble tiene como mucho un proyecto en análisis o aprobado (uno rechazado no impide evaluarlo de nuevo)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = 'UX_ProyectoInversion_InmuebleVigente')
    CREATE UNIQUE INDEX [UX_ProyectoInversion_InmuebleVigente] ON [dbo].[ProyectoInversion]([id_inmueble])
        WHERE [estado] <> 'Rechazado';
GO

IF OBJECT_ID('[dbo].[Inversor]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Inversor] (
        [id_inversor] INT IDENTITY(1,1) PRIMARY KEY,
        [nombre] NVARCHAR(100) NOT NULL,
        [apellido] NVARCHAR(100) NOT NULL,
        [cuit] VARCHAR(13) NOT NULL CONSTRAINT [UQ_Inversor_Cuit] UNIQUE, -- con o sin guiones: 20-12345678-3
        [email] VARCHAR(100) NULL,
        [telefono] VARCHAR(30) NULL
    );
    PRINT N'Tabla Inversor creada.';
END
GO

-- Aportes de los inversores a un fideicomiso aprobado. El porcentaje de participación no se guarda: se calcula
-- al leer (monto aportado / capital necesario = costo total de obra del proyecto), así no queda desactualizado.
IF OBJECT_ID('[dbo].[SuscripcionFideicomiso]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[SuscripcionFideicomiso] (
        [id_suscripcion] INT IDENTITY(1,1) PRIMARY KEY,
        [id_proyecto] INT NOT NULL CONSTRAINT [FK_Suscripcion_Proyecto] FOREIGN KEY REFERENCES [dbo].[ProyectoInversion]([id_proyecto]),
        [id_inversor] INT NOT NULL CONSTRAINT [FK_Suscripcion_Inversor] FOREIGN KEY REFERENCES [dbo].[Inversor]([id_inversor]),
        [monto_aportado] DECIMAL(18,2) NOT NULL CONSTRAINT [CK_Suscripcion_Monto] CHECK ([monto_aportado] > 0), -- USD
        [unidades_asignadas] INT NOT NULL CONSTRAINT [DF_Suscripcion_Unidades] DEFAULT 0
            CONSTRAINT [CK_Suscripcion_Unidades] CHECK ([unidades_asignadas] >= 0),
        [fecha_suscripcion] DATETIME NOT NULL CONSTRAINT [DF_Suscripcion_Fecha] DEFAULT GETDATE()
    );
    PRINT N'Tabla SuscripcionFideicomiso creada.';
END
GO

-- Índice del Costo de la Construcción (INDEC, CSV mensual): caché local de lo descargado
IF OBJECT_ID('[dbo].[HistorialICC]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[HistorialICC] (
        [id_icc] INT IDENTITY(1,1) PRIMARY KEY,
        [periodo] DATE NOT NULL CONSTRAINT [UQ_HistorialICC_Periodo] UNIQUE, -- primer día del mes
        [valor_indice] DECIMAL(10,4) NOT NULL CONSTRAINT [CK_HistorialICC_Valor] CHECK ([valor_indice] > 0),
        [variacion_mensual] DECIMAL(8,4) NULL,                                -- en %, respecto del mes anterior
        [fecha_consulta] DATETIME NOT NULL CONSTRAINT [DF_HistorialICC_FechaConsulta] DEFAULT GETDATE()
    );
    PRINT N'Tabla HistorialICC creada.';
END
GO

-- Roles, permisos y usuarios del PN3 en bases existentes (en una base nueva ya los creó schema-init.sql).
-- Mismo esquema que PN1/PN2: cada rol con su permiso, hijo de Admin, y un usuario <Rol> con contraseña <Rol>123.
DECLARE @RolesPN3 TABLE ([orden] INT, [rol] VARCHAR(50), [permiso] VARCHAR(50), [usuario] VARCHAR(50), [pass] VARCHAR(100),
                         [nombre_persona] NVARCHAR(100), [apellido] NVARCHAR(100));
INSERT INTO @RolesPN3 VALUES
    (7, 'Analista de Inversiones', 'AccesoPN3Analisis', 'AnalistaInversiones', 'f53d7900cae1b6cf4355764d2845998c65b89740ec961e53640cf121cd6c5c5f', N'Analista', N'Inversiones'),
    (8, 'Arquitectura', 'AccesoPN3Arquitectura', 'Arquitectura', '643ea49d4863cd97325107e90a0571512fb7fd612e0228bbd96114115b897bc9', N'Arquitectura', N'Sistema'),
    (9, 'Directorio', 'AccesoPN3Directorio', 'Directorio', '0d4ad2d25c6082f589c0f75709a7980b64a93e2c6bcf22ee347f21826e2ca369', N'Directorio', N'Sistema'),
    (10, 'Inversiones', 'AccesoPN3Inversiones', 'Inversiones', 'bc80595a2638cd35ba465c739cd1861b82b5ac162241fb41e5b1f4cc99af9cb1', N'Inversiones', N'Sistema');

INSERT INTO [dbo].[Permiso] ([nombre])
SELECT r.[permiso] FROM @RolesPN3 r WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Permiso] p WHERE p.[nombre] = r.[permiso]) ORDER BY r.[orden];
INSERT INTO [dbo].[Rol] ([nombre])
SELECT r.[rol] FROM @RolesPN3 r WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Rol] ro WHERE ro.[nombre] = r.[rol]) ORDER BY r.[orden];

INSERT INTO [dbo].[RolPermiso] ([id_padre], [id_hijo], [tipo_hijo])
SELECT ro.[id], pe.[id], 'Permiso'
FROM @RolesPN3 r
JOIN [dbo].[Rol] ro ON ro.[nombre] = r.[rol]
JOIN [dbo].[Permiso] pe ON pe.[nombre] = r.[permiso]
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RolPermiso] rp WHERE rp.[id_padre] = ro.[id] AND rp.[id_hijo] = pe.[id] AND rp.[tipo_hijo] = 'Permiso');

INSERT INTO [dbo].[RolPermiso] ([id_padre], [id_hijo], [tipo_hijo])
SELECT ad.[id], ro.[id], 'Rol'
FROM @RolesPN3 r
JOIN [dbo].[Rol] ro ON ro.[nombre] = r.[rol]
CROSS JOIN (SELECT [id] FROM [dbo].[Rol] WHERE [nombre] = 'Admin') ad
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RolPermiso] rp WHERE rp.[id_padre] = ad.[id] AND rp.[id_hijo] = ro.[id] AND rp.[tipo_hijo] = 'Rol');

DECLARE @UsuariosNuevos TABLE ([id] INT);
INSERT INTO [dbo].[Usuario] ([nombre], [nombre_persona], [apellido], [pass], [activo], [id_idioma])
OUTPUT inserted.[id] INTO @UsuariosNuevos
SELECT r.[usuario], r.[nombre_persona], r.[apellido], r.[pass], 1, 1
FROM @RolesPN3 r
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] u WHERE u.[nombre] = r.[usuario])
ORDER BY r.[orden];

INSERT INTO [dbo].[UsuarioRol] ([id_usuario], [id_rol])
SELECT us.[id], ro.[id]
FROM @RolesPN3 r
JOIN [dbo].[Usuario] us ON us.[nombre] = r.[usuario]
JOIN [dbo].[Rol] ro ON ro.[nombre] = r.[rol]
WHERE us.[id] IN (SELECT [id] FROM @UsuariosNuevos);

INSERT INTO [dbo].[Usuario_Historial] ([id_usuario], [nombre], [nombre_persona], [apellido], [pass], [activo], [dvh], [id_usuario_autor], [tipo_operacion])
SELECT us.[id], us.[nombre], us.[nombre_persona], us.[apellido], us.[pass], us.[activo], 0,
       (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin'), 'INSERT'
FROM [dbo].[Usuario] us
WHERE us.[id] IN (SELECT [id] FROM @UsuariosNuevos);

-- Dígitos verificadores de los usuarios nuevos (mismo cálculo que SERVICIOS.GestorDV y schema-init.sql) y DVV
IF EXISTS (SELECT 1 FROM @UsuariosNuevos)
BEGIN
    DECLARE @Dvh TABLE ([id] INT, [dvh] BIGINT);
    DECLARE @Id INT, @Texto NVARCHAR(MAX), @Pos INT, @Total BIGINT, @i INT;
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT [id] FROM @UsuariosNuevos;
    OPEN cur;
    FETCH NEXT FROM cur INTO @Id;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Total = 0;
        SET @Pos = 1;
        WHILE @Pos <= 6
        BEGIN
            SELECT @Texto = CASE @Pos
                WHEN 1 THEN CAST(u.[id] AS NVARCHAR(50))
                WHEN 2 THEN u.[nombre]
                WHEN 3 THEN u.[pass]
                WHEN 4 THEN CASE WHEN u.[activo] = 1 THEN N'1' ELSE N'0' END
                WHEN 5 THEN ISNULL(u.[nombre_persona], N'')
                ELSE ISNULL(u.[apellido], N'') END
            FROM [dbo].[Usuario] u WHERE u.[id] = @Id;
            SET @i = 1;
            WHILE @i <= LEN(@Texto)
            BEGIN
                SET @Total = @Total + UNICODE(SUBSTRING(@Texto, @i, 1)) * @i * @Pos;
                SET @i = @i + 1;
            END
            SET @Pos = @Pos + 1;
        END
        INSERT INTO @Dvh VALUES (@Id, @Total);
        FETCH NEXT FROM cur INTO @Id;
    END
    CLOSE cur;
    DEALLOCATE cur;

    UPDATE u SET [dvh] = d.[dvh] FROM [dbo].[Usuario] u JOIN @Dvh d ON d.[id] = u.[id];
    UPDATE h SET [dvh] = d.[dvh] FROM [dbo].[Usuario_Historial] h JOIN @Dvh d ON d.[id] = h.[id_usuario];
    UPDATE [dbo].[DigitoVerificadorVertical] SET [dvv] = (SELECT ISNULL(SUM([dvh]), 0) FROM [dbo].[Usuario]) WHERE [tabla] = 'Usuario';
    PRINT N'PN3: roles, permisos y usuarios creados.';
END
GO

-- ====================================================
-- 4d. Ranking de agentes: DNI de los usuarios, ventas y datos de ejemplo
-- ====================================================

-- DNI del usuario (formato 12.345.678). Nombre y apellido ya existían (nombre_persona y apellido; [nombre] es el
-- usuario de login). Único entre los que lo tienen cargado.
IF COL_LENGTH('dbo.Usuario', 'dni') IS NULL
BEGIN
    ALTER TABLE [dbo].[Usuario] ADD [dni] VARCHAR(11) NULL;
    PRINT N'Usuario: columna dni agregada.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = 'UX_Usuario_Dni')
    CREATE UNIQUE INDEX [UX_Usuario_Dni] ON [dbo].[Usuario]([dni]) WHERE [dni] IS NOT NULL;
GO

IF COL_LENGTH('dbo.Usuario_Historial', 'dni') IS NULL
    ALTER TABLE [dbo].[Usuario_Historial] ADD [dni] VARCHAR(11) NULL;
GO

-- Estado 'Vendido' (solo inmuebles en venta): se reemplaza el CHECK de estado_actual
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Inmueble_EstadoActual' AND [definition] NOT LIKE '%Vendido%')
    ALTER TABLE [dbo].[Inmueble] DROP CONSTRAINT [CK_Inmueble_EstadoActual];
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Inmueble_EstadoActual')
BEGIN
    ALTER TABLE [dbo].[Inmueble] ADD CONSTRAINT [CK_Inmueble_EstadoActual]
        CHECK ([estado_actual] IN ('Captado', 'Tasado', 'Rechazado', 'Publicado', 'Alquilado', 'Vendido'));
    PRINT N'Inmueble: estado Vendido habilitado.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE [name] = 'CK_Inmueble_VendidoEsVenta')
    ALTER TABLE [dbo].[Inmueble] ADD CONSTRAINT [CK_Inmueble_VendidoEsVenta]
        CHECK ([estado_actual] <> 'Vendido' OR [tipo_publicacion] = 'Venta');
GO

-- Venta de un inmueble publicado en venta: precio en dólares (así se operan las propiedades) y el agente de la
-- captación, que es a quien se le cuenta en el ranking
IF OBJECT_ID('[dbo].[Venta]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Venta] (
        [id_venta] INT IDENTITY(1,1) PRIMARY KEY,
        [id_inmueble] INT NOT NULL CONSTRAINT [FK_Venta_Inmueble] FOREIGN KEY REFERENCES [dbo].[Inmueble]([id_inmueble])
            CONSTRAINT [UQ_Venta_Inmueble] UNIQUE,
        [id_usuario_agente] INT NOT NULL CONSTRAINT [FK_Venta_Agente] FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
        [fecha_venta] DATE NOT NULL,
        [precio_usd] DECIMAL(18,2) NOT NULL CONSTRAINT [CK_Venta_PrecioUsd] CHECK ([precio_usd] > 0),
        [id_usuario_registro] INT NULL CONSTRAINT [FK_Venta_Registro] FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
        [fecha_registro] DATETIME NOT NULL CONSTRAINT [DF_Venta_FechaRegistro] DEFAULT GETDATE(),
        [observaciones] NVARCHAR(500) NULL
    );
    CREATE INDEX [IX_Venta_Fecha] ON [dbo].[Venta]([fecha_venta]) INCLUDE ([id_usuario_agente], [precio_usd]);
    PRINT N'Tabla Venta creada.';
END
GO

-- Nombre, apellido y DNI de los usuarios del sistema, 4 agentes comerciales más y ventas de ejemplo para el ranking.
-- Solo la primera vez (si el agente mrossi no existe): no pisa lo que se haya editado después.
-- Contraseña de los agentes nuevos: NombreApellido123 (SHA-256 como SERVICIOS.Encriptador).
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] WHERE [nombre] = 'mrossi')
BEGIN
    DECLARE @Tocados TABLE ([id] INT, [operacion] VARCHAR(20));

    DECLARE @Personas TABLE ([usuario] VARCHAR(50), [nombre_persona] NVARCHAR(100), [apellido] NVARCHAR(100), [dni] VARCHAR(11));
    INSERT INTO @Personas VALUES
        ('admin',                   N'Admin',     N'Sistema',   '00.000.001'),
        ('AgenteComercial',         N'Marcos',    N'Pérez',     '30.456.789'),
        ('Tasador',                 N'Lucía',     N'Fernández', '28.765.432'),
        ('Marketing',               N'Sofía',     N'Romero',    '33.210.987'),
        ('AdministradorAlquileres', N'Diego',     N'Álvarez',   '27.654.321'),
        ('AnalistaCobranzas',       N'Valeria',   N'Méndez',    '31.987.654'),
        ('AsesorLegal',             N'Gustavo',   N'Ruiz',      '25.432.198'),
        ('AnalistaInversiones',     N'Florencia', N'Castro',    '34.876.210'),
        ('Arquitectura',            N'Pablo',     N'Giménez',   '29.345.876'),
        ('Directorio',              N'Ricardo',   N'Benítez',   '22.109.876'),
        ('Inversiones',             N'Carolina',  N'Acosta',    '32.567.890');

    UPDATE u SET [nombre_persona] = p.[nombre_persona], [apellido] = p.[apellido], [dni] = p.[dni]
    OUTPUT inserted.[id], 'UPDATE' INTO @Tocados
    FROM [dbo].[Usuario] u
    JOIN @Personas p ON p.[usuario] = u.[nombre]
    WHERE u.[dni] IS NULL AND NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] o WHERE o.[dni] = p.[dni]);

    DECLARE @Agentes TABLE ([usuario] VARCHAR(50), [nombre_persona] NVARCHAR(100), [apellido] NVARCHAR(100), [dni] VARCHAR(11), [clave] VARCHAR(100));
    INSERT INTO @Agentes VALUES
        ('mrossi',     N'Martina',  N'Rossi',     '35.123.456', 'MartinaRossi123'),
        ('fmorales',   N'Federico', N'Morales',   '36.234.567', 'FedericoMorales123'),
        ('cherrera',   N'Camila',   N'Herrera',   '37.345.678', 'CamilaHerrera123'),
        ('lbenedetti', N'Lucas',    N'Benedetti', '38.456.789', 'LucasBenedetti123');

    INSERT INTO [dbo].[Usuario] ([nombre], [nombre_persona], [apellido], [dni], [pass], [activo], [id_idioma])
    OUTPUT inserted.[id], 'INSERT' INTO @Tocados
    SELECT a.[usuario], a.[nombre_persona], a.[apellido], a.[dni],
           LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', a.[clave]), 2)), 1, 1
    FROM @Agentes a
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] u WHERE u.[nombre] = a.[usuario] OR u.[dni] = a.[dni]);

    INSERT INTO [dbo].[UsuarioRol] ([id_usuario], [id_rol])
    SELECT us.[id], ro.[id]
    FROM @Agentes a
    JOIN [dbo].[Usuario] us ON us.[nombre] = a.[usuario]
    JOIN [dbo].[Rol] ro ON ro.[nombre] = 'Agente Comercial'
    WHERE us.[id] IN (SELECT [id] FROM @Tocados WHERE [operacion] = 'INSERT');

    -- Dígitos verificadores (mismo cálculo que SERVICIOS.GestorDV: el DNI es el atributo 7) de los usuarios tocados
    DECLARE @Dvh TABLE ([id] INT, [dvh] BIGINT);
    DECLARE @Id INT, @Texto NVARCHAR(MAX), @Pos INT, @Total BIGINT, @i INT;
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT [id] FROM @Tocados;
    OPEN cur;
    FETCH NEXT FROM cur INTO @Id;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Total = 0;
        SET @Pos = 1;
        WHILE @Pos <= 7
        BEGIN
            SELECT @Texto = CASE @Pos
                WHEN 1 THEN CAST(u.[id] AS NVARCHAR(50))
                WHEN 2 THEN u.[nombre]
                WHEN 3 THEN u.[pass]
                WHEN 4 THEN CASE WHEN u.[activo] = 1 THEN N'1' ELSE N'0' END
                WHEN 5 THEN ISNULL(u.[nombre_persona], N'')
                WHEN 6 THEN ISNULL(u.[apellido], N'')
                ELSE ISNULL(u.[dni], N'') END
            FROM [dbo].[Usuario] u WHERE u.[id] = @Id;
            SET @i = 1;
            WHILE @i <= LEN(@Texto)
            BEGIN
                SET @Total = @Total + UNICODE(SUBSTRING(@Texto, @i, 1)) * @i * @Pos;
                SET @i = @i + 1;
            END
            SET @Pos = @Pos + 1;
        END
        INSERT INTO @Dvh VALUES (@Id, @Total);
        FETCH NEXT FROM cur INTO @Id;
    END
    CLOSE cur;
    DEALLOCATE cur;
    UPDATE u SET [dvh] = d.[dvh] FROM [dbo].[Usuario] u JOIN @Dvh d ON d.[id] = u.[id];
    UPDATE [dbo].[DigitoVerificadorVertical] SET [dvv] = (SELECT ISNULL(SUM([dvh]), 0) FROM [dbo].[Usuario]) WHERE [tabla] = 'Usuario';

    -- Historial de los cambios (alta de los agentes, nombre y DNI de los existentes), con admin como autor
    INSERT INTO [dbo].[Usuario_Historial] ([id_usuario], [nombre], [nombre_persona], [apellido], [dni], [pass], [activo], [dvh], [id_usuario_autor], [tipo_operacion])
    SELECT us.[id], us.[nombre], us.[nombre_persona], us.[apellido], us.[dni], us.[pass], us.[activo], us.[dvh],
           (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin'), t.[operacion]
    FROM @Tocados t
    JOIN [dbo].[Usuario] us ON us.[id] = t.[id];

    -- Ventas de ejemplo: cada una con su cliente, inmueble (Vendido), captación del agente, tasación aprobada y
    -- venta en USD; las que no tienen precio quedan Publicadas. Las fechas son relativas al día de la instalación
    -- (días atrás) para que el ranking tenga datos en todos los períodos.
    DECLARE @Demo TABLE ([orden] INT IDENTITY(1,1), [agente] VARCHAR(50), [tipo] VARCHAR(50), [direccion] NVARCHAR(200),
                         [localidad] NVARCHAR(100), [m2] DECIMAL(10,2), [ambientes] INT, [lote] DECIMAL(10,2),
                         [precio_usd] DECIMAL(18,2), [dias] INT);
    INSERT INTO @Demo ([agente], [tipo], [direccion], [localidad], [m2], [ambientes], [lote], [precio_usd], [dias]) VALUES
        ('mrossi',          'Departamento', N'Billinghurst 1650',       N'Palermo, CABA',      72,  3, NULL, 145000, 5),
        ('mrossi',          'Casa',         N'Sucre 2840',              N'Belgrano, CABA',     190, 5, 260,  210000, 40),
        ('mrossi',          'Departamento', N'Av. Rivadavia 6200',      N'Caballito, CABA',    55,  2, NULL, 98000,  120),
        ('mrossi',          'Departamento', N'Juramento 2150',          N'Belgrano, CABA',     85,  3, NULL, 175000, 300),
        ('mrossi',          'Departamento', N'Ciudad de la Paz 1900',   N'Belgrano, CABA',     64,  3, NULL, NULL,   20),
        ('fmorales',        'Casa',         N'Olazábal 4300',           N'Villa Urquiza, CABA', 230, 6, 300, 320000, 10),
        ('fmorales',        'Casa',         N'Zapiola 1450',            N'Colegiales, CABA',   260, 6, 320,  410000, 70),
        ('fmorales',        'Departamento', N'Av. del Libertador 5800', N'Belgrano, CABA',     140, 4, NULL, 260000, 400),
        ('fmorales',        'Casa',         N'Moldes 3100',             N'Coghlan, CABA',      200, 5, 280,  NULL,   35),
        ('cherrera',        'Departamento', N'Acoyte 450',              N'Caballito, CABA',    48,  2, NULL, 89000,  2),
        ('cherrera',        'Departamento', N'Av. Corrientes 5100',     N'Almagro, CABA',      42,  2, NULL, 76000,  20),
        ('cherrera',        'Departamento', N'Gascón 900',              N'Almagro, CABA',      60,  3, NULL, 112000, 50),
        ('cherrera',        'Departamento', N'Boedo 1200',              N'Boedo, CABA',        38,  1, NULL, 68000,  95),
        ('cherrera',        'Departamento', N'Av. La Plata 300',        N'Caballito, CABA',    58,  2, NULL, 99000,  200),
        ('lbenedetti',      'Departamento', N'Av. Cabildo 3500',        N'Núñez, CABA',        70,  3, NULL, 135000, 150),
        ('lbenedetti',      'Casa',         N'Pedro Morán 3900',        N'Villa Devoto, CABA', 180, 5, 250,  NULL,   12),
        ('lbenedetti',      'Departamento', N'Nazca 2600',              N'Villa del Parque, CABA', 66, 3, NULL, NULL, 25),
        ('AgenteComercial', 'Casa',         N'Bacacay 2400',            N'Flores, CABA',       170, 5, 240,  180000, 30),
        ('AgenteComercial', 'Departamento', N'Av. Santa Fe 3300',       N'Palermo, CABA',      95,  3, NULL, 230000, 250);

    DECLARE @Tasador INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'Tasador');
    DECLARE @Registro INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'Marketing');
    IF @Tasador IS NULL SET @Tasador = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin');
    DECLARE @Orden INT, @Agente INT, @Tipo VARCHAR(50), @Direccion NVARCHAR(200), @Localidad NVARCHAR(100), @M2 DECIMAL(10,2),
            @Ambientes INT, @Lote DECIMAL(10,2), @PrecioUsd DECIMAL(18,2), @Dias INT, @Cliente INT, @Inmueble INT, @Captacion INT,
            @FechaVenta DATE, @FechaCaptacion DATETIME, @Pesos DECIMAL(18,2);
    DECLARE demo CURSOR LOCAL FAST_FORWARD FOR
        SELECT d.[orden], u.[id], d.[tipo], d.[direccion], d.[localidad], d.[m2], d.[ambientes], d.[lote], d.[precio_usd], d.[dias]
        FROM @Demo d JOIN [dbo].[Usuario] u ON u.[nombre] = d.[agente]
        ORDER BY d.[orden];
    OPEN demo;
    FETCH NEXT FROM demo INTO @Orden, @Agente, @Tipo, @Direccion, @Localidad, @M2, @Ambientes, @Lote, @PrecioUsd, @Dias;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @FechaVenta = CAST(DATEADD(DAY, -@Dias, GETDATE()) AS DATE);
        SET @FechaCaptacion = DATEADD(DAY, -60, CAST(@FechaVenta AS DATETIME));
        SET @Pesos = ISNULL(@PrecioUsd, 150000) * 1500;
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Cliente] WHERE [dni] = CAST(41000000 + @Orden AS VARCHAR(15)))
            INSERT INTO [dbo].[Cliente] ([dni], [nombre], [apellido], [telefono], [email])
            VALUES (CAST(41000000 + @Orden AS VARCHAR(15)), N'Cliente', N'Ejemplo ' + CAST(@Orden AS NVARCHAR(10)),
                    '11-4000-' + RIGHT('0000' + CAST(@Orden AS VARCHAR(4)), 4), NULL);
        SET @Cliente = (SELECT [id_cliente] FROM [dbo].[Cliente] WHERE [dni] = CAST(41000000 + @Orden AS VARCHAR(15)));

        INSERT INTO [dbo].[Inmueble] ([tipo_inmueble], [direccion], [metros_cuadrados], [cantidad_ambientes], [estado_actual], [id_cliente],
                                      [tipo_publicacion], [localidad], [superficie_lote], [texto_comercial], [fecha_publicacion])
        VALUES (@Tipo, @Direccion, @M2, @Ambientes, CASE WHEN @PrecioUsd IS NULL THEN 'Publicado' ELSE 'Vendido' END, @Cliente,
                'Venta', @Localidad, @Lote, N'Inmueble de ejemplo para el ranking de agentes.', DATEADD(DAY, 15, @FechaCaptacion));
        SET @Inmueble = SCOPE_IDENTITY();
        INSERT INTO [dbo].[Captacion] ([fecha_registro], [valor_pretendido], [id_inmueble], [id_usuario_agente])
        VALUES (@FechaCaptacion, @Pesos, @Inmueble, @Agente);
        SET @Captacion = SCOPE_IDENTITY();
        INSERT INTO [dbo].[Tasacion] ([fecha_dictamen], [estado_dictamen], [valor_final], [id_captacion], [id_usuario_tasador])
        VALUES (DATEADD(DAY, 7, @FechaCaptacion), 'Aprobada', ROUND(@Pesos * 0.95, 2), @Captacion, @Tasador);
        IF @PrecioUsd IS NOT NULL
            INSERT INTO [dbo].[Venta] ([id_inmueble], [id_usuario_agente], [fecha_venta], [precio_usd], [id_usuario_registro], [fecha_registro], [observaciones])
            VALUES (@Inmueble, @Agente, @FechaVenta, @PrecioUsd, @Registro, @FechaVenta, N'Venta de ejemplo para el ranking de agentes.');
        FETCH NEXT FROM demo INTO @Orden, @Agente, @Tipo, @Direccion, @Localidad, @M2, @Ambientes, @Lote, @PrecioUsd, @Dias;
    END
    CLOSE demo;
    DEALLOCATE demo;
    PRINT N'Ranking de agentes: nombre y DNI de los usuarios, 4 agentes comerciales y ventas de ejemplo creados.';
END
GO

-- 5 agentes comerciales más con captaciones y ventas de ejemplo, con perfiles distintos para el ranking (muchas
-- ventas baratas, pocas muy caras, intermedios y uno con una sola venta). Bloque aparte del anterior para que también
-- se carguen en las bases que ya lo ejecutaron. Solo la primera vez (si el agente vsosa no existe).
-- Contraseña: NombreApellido123 (SHA-256 como SERVICIOS.Encriptador).
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] WHERE [nombre] = 'vsosa')
BEGIN
    DECLARE @Tocados TABLE ([id] INT);

    DECLARE @Agentes TABLE ([usuario] VARCHAR(50), [nombre_persona] NVARCHAR(100), [apellido] NVARCHAR(100), [dni] VARCHAR(11), [clave] VARCHAR(100));
    INSERT INTO @Agentes VALUES
        ('vsosa',     N'Valentina', N'Sosa',     '39.567.123', 'ValentinaSosa123'),
        ('squiroga',  N'Santiago',  N'Quiroga',  '40.678.234', 'SantiagoQuiroga123'),
        ('aledesma',  N'Agustina',  N'Ledesma',  '41.789.345', 'AgustinaLedesma123'),
        ('mvillalba', N'Mateo',     N'Villalba', '42.890.456', 'MateoVillalba123'),
        ('pcabrera',  N'Paula',     N'Cabrera',  '38.901.567', 'PaulaCabrera123');

    INSERT INTO [dbo].[Usuario] ([nombre], [nombre_persona], [apellido], [dni], [pass], [activo], [id_idioma])
    OUTPUT inserted.[id] INTO @Tocados
    SELECT a.[usuario], a.[nombre_persona], a.[apellido], a.[dni],
           LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', a.[clave]), 2)), 1, 1
    FROM @Agentes a
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] u WHERE u.[nombre] = a.[usuario] OR u.[dni] = a.[dni]);

    INSERT INTO [dbo].[UsuarioRol] ([id_usuario], [id_rol])
    SELECT t.[id], ro.[id]
    FROM @Tocados t
    JOIN [dbo].[Rol] ro ON ro.[nombre] = 'Agente Comercial';

    -- Dígitos verificadores (mismo cálculo que SERVICIOS.GestorDV: el DNI es el atributo 7) de los usuarios nuevos
    DECLARE @Dvh TABLE ([id] INT, [dvh] BIGINT);
    DECLARE @Id INT, @Texto NVARCHAR(MAX), @Pos INT, @Total BIGINT, @i INT;
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT [id] FROM @Tocados;
    OPEN cur;
    FETCH NEXT FROM cur INTO @Id;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Total = 0;
        SET @Pos = 1;
        WHILE @Pos <= 7
        BEGIN
            SELECT @Texto = CASE @Pos
                WHEN 1 THEN CAST(u.[id] AS NVARCHAR(50))
                WHEN 2 THEN u.[nombre]
                WHEN 3 THEN u.[pass]
                WHEN 4 THEN CASE WHEN u.[activo] = 1 THEN N'1' ELSE N'0' END
                WHEN 5 THEN ISNULL(u.[nombre_persona], N'')
                WHEN 6 THEN ISNULL(u.[apellido], N'')
                ELSE ISNULL(u.[dni], N'') END
            FROM [dbo].[Usuario] u WHERE u.[id] = @Id;
            SET @i = 1;
            WHILE @i <= LEN(@Texto)
            BEGIN
                SET @Total = @Total + UNICODE(SUBSTRING(@Texto, @i, 1)) * @i * @Pos;
                SET @i = @i + 1;
            END
            SET @Pos = @Pos + 1;
        END
        INSERT INTO @Dvh VALUES (@Id, @Total);
        FETCH NEXT FROM cur INTO @Id;
    END
    CLOSE cur;
    DEALLOCATE cur;
    UPDATE u SET [dvh] = d.[dvh] FROM [dbo].[Usuario] u JOIN @Dvh d ON d.[id] = u.[id];
    UPDATE [dbo].[DigitoVerificadorVertical] SET [dvv] = (SELECT ISNULL(SUM([dvh]), 0) FROM [dbo].[Usuario]) WHERE [tabla] = 'Usuario';

    -- Historial: alta de los agentes, con admin como autor
    INSERT INTO [dbo].[Usuario_Historial] ([id_usuario], [nombre], [nombre_persona], [apellido], [dni], [pass], [activo], [dvh], [id_usuario_autor], [tipo_operacion])
    SELECT us.[id], us.[nombre], us.[nombre_persona], us.[apellido], us.[dni], us.[pass], us.[activo], us.[dvh],
           (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin'), 'INSERT'
    FROM @Tocados t
    JOIN [dbo].[Usuario] us ON us.[id] = t.[id];

    -- Captaciones y ventas, igual que el bloque anterior: cliente, inmueble (Vendido), captación del agente, tasación
    -- aprobada y venta en USD; sin precio queda Publicado. Fechas relativas al día de la instalación (días atrás).
    DECLARE @Demo TABLE ([orden] INT IDENTITY(1,1), [agente] VARCHAR(50), [tipo] VARCHAR(50), [direccion] NVARCHAR(200),
                         [localidad] NVARCHAR(100), [m2] DECIMAL(10,2), [ambientes] INT, [lote] DECIMAL(10,2),
                         [precio_usd] DECIMAL(18,2), [dias] INT);
    INSERT INTO @Demo ([agente], [tipo], [direccion], [localidad], [m2], [ambientes], [lote], [precio_usd], [dias]) VALUES
        -- Muchas ventas de montos bajos
        ('vsosa',     'Departamento', N'Av. San Martín 2100',     N'Villa Crespo, CABA',     40,  1, NULL, 72000,  3),
        ('vsosa',     'Departamento', N'Thames 800',              N'Villa Crespo, CABA',     45,  2, NULL, 85000,  15),
        ('vsosa',     'Departamento', N'Av. Díaz Vélez 4500',     N'Almagro, CABA',          50,  2, NULL, 92000,  45),
        ('vsosa',     'Departamento', N'Yatay 300',               N'Almagro, CABA',          36,  1, NULL, 64000,  80),
        ('vsosa',     'Departamento', N'Av. Independencia 3900',  N'Boedo, CABA',            55,  2, NULL, 98000,  160),
        ('vsosa',     'Departamento', N'Hidalgo 700',             N'Caballito, CABA',        62,  3, NULL, 118000, 380),
        -- Pocas ventas de mucho valor
        ('squiroga',  'Casa',         N'Av. Melián 2400',         N'Belgrano R, CABA',       420, 7, 600,  850000, 25),
        ('squiroga',  'Casa',         N'Paraná 1200',             N'Martínez, Buenos Aires', 310, 6, 450,  480000, 210),
        ('squiroga',  'Casa',         N'Av. Santa Fe 1800',       N'Olivos, Buenos Aires',   280, 6, 420,  NULL,   18),
        -- Intermedios
        ('aledesma',  'Departamento', N'Av. Cabildo 1500',        N'Belgrano, CABA',         78,  3, NULL, 155000, 8),
        ('aledesma',  'Casa',         N'Bauness 2300',            N'Villa Urquiza, CABA',    160, 4, 220,  195000, 60),
        ('aledesma',  'Departamento', N'Monroe 4100',             N'Villa Urquiza, CABA',    68,  3, NULL, 128000, 130),
        ('aledesma',  'Departamento', N'Triunvirato 3800',        N'Villa Urquiza, CABA',    52,  2, NULL, 62000,  420),
        ('pcabrera',  'Casa',         N'Bolívar 900',             N'San Isidro, Buenos Aires', 190, 5, 300, 240000, 55),
        ('pcabrera',  'Departamento', N'Av. Maipú 1600',          N'Vicente López, Buenos Aires', 70, 3, NULL, 125000, 140),
        ('pcabrera',  'Departamento', N'Laprida 2000',            N'Florida, Buenos Aires',  45,  2, NULL, NULL,   10),
        ('pcabrera',  'Departamento', N'Av. Mitre 500',           N'Avellaneda, Buenos Aires', 48, 2, NULL, 110000, 600),
        -- Una sola venta (para el Top 5 de los que menos vendieron)
        ('mvillalba', 'Departamento', N'Av. Rivadavia 9800',      N'Floresta, CABA',         44,  2, NULL, 72000,  95),
        ('mvillalba', 'Casa',         N'Morón 3200',              N'Flores, CABA',           150, 4, 200,  NULL,   30);

    DECLARE @Tasador INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'Tasador');
    DECLARE @Registro INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'Marketing');
    IF @Tasador IS NULL SET @Tasador = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin');
    DECLARE @Orden INT, @Agente INT, @Tipo VARCHAR(50), @Direccion NVARCHAR(200), @Localidad NVARCHAR(100), @M2 DECIMAL(10,2),
            @Ambientes INT, @Lote DECIMAL(10,2), @PrecioUsd DECIMAL(18,2), @Dias INT, @Cliente INT, @Inmueble INT, @Captacion INT,
            @FechaVenta DATE, @FechaCaptacion DATETIME, @Pesos DECIMAL(18,2), @DniCliente VARCHAR(15);
    DECLARE demo CURSOR LOCAL FAST_FORWARD FOR
        SELECT d.[orden], u.[id], d.[tipo], d.[direccion], d.[localidad], d.[m2], d.[ambientes], d.[lote], d.[precio_usd], d.[dias]
        FROM @Demo d JOIN [dbo].[Usuario] u ON u.[nombre] = d.[agente]
        ORDER BY d.[orden];
    OPEN demo;
    FETCH NEXT FROM demo INTO @Orden, @Agente, @Tipo, @Direccion, @Localidad, @M2, @Ambientes, @Lote, @PrecioUsd, @Dias;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @FechaVenta = CAST(DATEADD(DAY, -@Dias, GETDATE()) AS DATE);
        SET @FechaCaptacion = DATEADD(DAY, -60, CAST(@FechaVenta AS DATETIME));
        SET @Pesos = ISNULL(@PrecioUsd, 150000) * 1500;
        -- Clientes 43000001 en adelante (el bloque anterior usa 41000001...)
        SET @DniCliente = CAST(43000000 + @Orden AS VARCHAR(15));
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Cliente] WHERE [dni] = @DniCliente)
            INSERT INTO [dbo].[Cliente] ([dni], [nombre], [apellido], [telefono], [email])
            VALUES (@DniCliente, N'Cliente', N'Ejemplo ' + CAST(20 + @Orden AS NVARCHAR(10)),
                    '11-4300-' + RIGHT('0000' + CAST(@Orden AS VARCHAR(4)), 4), NULL);
        SET @Cliente = (SELECT [id_cliente] FROM [dbo].[Cliente] WHERE [dni] = @DniCliente);

        INSERT INTO [dbo].[Inmueble] ([tipo_inmueble], [direccion], [metros_cuadrados], [cantidad_ambientes], [estado_actual], [id_cliente],
                                      [tipo_publicacion], [localidad], [superficie_lote], [texto_comercial], [fecha_publicacion])
        VALUES (@Tipo, @Direccion, @M2, @Ambientes, CASE WHEN @PrecioUsd IS NULL THEN 'Publicado' ELSE 'Vendido' END, @Cliente,
                'Venta', @Localidad, @Lote, N'Inmueble de ejemplo para el ranking de agentes.', DATEADD(DAY, 15, @FechaCaptacion));
        SET @Inmueble = SCOPE_IDENTITY();
        INSERT INTO [dbo].[Captacion] ([fecha_registro], [valor_pretendido], [id_inmueble], [id_usuario_agente])
        VALUES (@FechaCaptacion, @Pesos, @Inmueble, @Agente);
        SET @Captacion = SCOPE_IDENTITY();
        INSERT INTO [dbo].[Tasacion] ([fecha_dictamen], [estado_dictamen], [valor_final], [id_captacion], [id_usuario_tasador])
        VALUES (DATEADD(DAY, 7, @FechaCaptacion), 'Aprobada', ROUND(@Pesos * 0.95, 2), @Captacion, @Tasador);
        IF @PrecioUsd IS NOT NULL
            INSERT INTO [dbo].[Venta] ([id_inmueble], [id_usuario_agente], [fecha_venta], [precio_usd], [id_usuario_registro], [fecha_registro], [observaciones])
            VALUES (@Inmueble, @Agente, @FechaVenta, @PrecioUsd, @Registro, @FechaVenta, N'Venta de ejemplo para el ranking de agentes.');
        FETCH NEXT FROM demo INTO @Orden, @Agente, @Tipo, @Direccion, @Localidad, @M2, @Ambientes, @Lote, @PrecioUsd, @Dias;
    END
    CLOSE demo;
    DEALLOCATE demo;
    PRINT N'Ranking de agentes: 5 agentes comerciales más con captaciones y ventas de ejemplo creados.';
END
GO

-- Ventas de ejemplo (últimos 12 meses) para los 4 primeros agentes cuando existen pero no tienen ninguna venta: p. ej.
-- si se crearon a mano desde la aplicación. Se buscan por usuario (mrossi o MartinaRossi) o por nombre y apellido.
-- A un agente que ya tiene ventas (con cualquiera de los usuarios que coinciden) no se le agrega nada, así que en las
-- bases donde el bloque anterior ya cargó las suyas este no hace nada.
DECLARE @Buscados TABLE ([clave] VARCHAR(20), [usuario] VARCHAR(50), [usuario_largo] VARCHAR(50),
                         [nombre_persona] NVARCHAR(100), [apellido] NVARCHAR(100));
INSERT INTO @Buscados VALUES
    ('rossi',     'mrossi',     'MartinaRossi',    N'Martina',  N'Rossi'),
    ('morales',   'fmorales',   'FedericoMorales', N'Federico', N'Morales'),
    ('herrera',   'cherrera',   'CamilaHerrera',   N'Camila',   N'Herrera'),
    ('benedetti', 'lbenedetti', 'LucasBenedetti',  N'Lucas',    N'Benedetti');

DECLARE @SinVentas TABLE ([clave] VARCHAR(20), [id] INT);
INSERT INTO @SinVentas ([clave], [id])
SELECT b.[clave],
       (SELECT TOP 1 u.[id] FROM [dbo].[Usuario] u
        WHERE u.[nombre] IN (b.[usuario], b.[usuario_largo])
           OR (u.[nombre_persona] = b.[nombre_persona] AND u.[apellido] = b.[apellido])
        ORDER BY CASE WHEN u.[nombre] = b.[usuario] THEN 0 WHEN u.[nombre] = b.[usuario_largo] THEN 1 ELSE 2 END, u.[id])
FROM @Buscados b
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Venta] v JOIN [dbo].[Usuario] u ON u.[id] = v.[id_usuario_agente]
                  WHERE u.[nombre] IN (b.[usuario], b.[usuario_largo])
                     OR (u.[nombre_persona] = b.[nombre_persona] AND u.[apellido] = b.[apellido]));
DELETE FROM @SinVentas WHERE [id] IS NULL;

IF EXISTS (SELECT 1 FROM @SinVentas)
BEGIN
    -- Cantidades, montos y fechas distintos para cada agente; las que no tienen precio quedan Publicadas
    DECLARE @Demo TABLE ([orden] INT IDENTITY(1,1), [agente] VARCHAR(20), [tipo] VARCHAR(50), [direccion] NVARCHAR(200),
                         [localidad] NVARCHAR(100), [m2] DECIMAL(10,2), [ambientes] INT, [lote] DECIMAL(10,2),
                         [precio_usd] DECIMAL(18,2), [dias] INT);
    INSERT INTO @Demo ([agente], [tipo], [direccion], [localidad], [m2], [ambientes], [lote], [precio_usd], [dias]) VALUES
        ('rossi',     'Departamento', N'Gorriti 4800',            N'Palermo, CABA',          68,  3, NULL, 152000, 6),
        ('rossi',     'Departamento', N'Cramer 2300',             N'Belgrano, CABA',         82,  3, NULL, 168000, 48),
        ('rossi',     'Casa',         N'Virrey del Pino 3600',    N'Belgrano, CABA',         210, 5, 280,  295000, 115),
        ('rossi',     'Departamento', N'Mendoza 2500',            N'Belgrano, CABA',         58,  2, NULL, 109000, 240),
        ('morales',   'Casa',         N'Pampa 4200',              N'Villa Urquiza, CABA',    240, 6, 310,  365000, 22),
        ('morales',   'Casa',         N'Tronador 2900',           N'Coghlan, CABA',          270, 6, 330,  445000, 160),
        ('morales',   'Departamento', N'Av. Congreso 3200',       N'Coghlan, CABA',          120, 4, NULL, 238000, 330),
        ('herrera',   'Departamento', N'Pringles 900',            N'Almagro, CABA',          40,  1, NULL, 71000,  4),
        ('herrera',   'Departamento', N'Guardia Vieja 3900',      N'Almagro, CABA',          47,  2, NULL, 83000,  33),
        ('herrera',   'Departamento', N'Av. Pedro Goyena 600',    N'Caballito, CABA',        63,  3, NULL, 118000, 75),
        ('herrera',   'Departamento', N'Senillosa 300',           N'Caballito, CABA',        52,  2, NULL, 94000,  190),
        ('herrera',   'Departamento', N'Carlos Calvo 3800',       N'Boedo, CABA',            39,  1, NULL, 66000,  290),
        ('herrera',   'Departamento', N'Av. Asamblea 1100',       N'Parque Chacabuco, CABA', 55,  2, NULL, NULL,   14),
        ('benedetti', 'Departamento', N'Av. Congreso 4900',       N'Villa Urquiza, CABA',    74,  3, NULL, 142000, 85),
        ('benedetti', 'Casa',         N'Cuenca 3100',             N'Villa del Parque, CABA', 175, 5, 240,  228000, 270),
        ('benedetti', 'Departamento', N'Av. Beiró 4600',          N'Villa Devoto, CABA',     60,  3, NULL, NULL,   28);

    DECLARE @Tasador INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'Tasador');
    DECLARE @Registro INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'Marketing');
    IF @Tasador IS NULL SET @Tasador = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin');
    DECLARE @Orden INT, @Agente INT, @Tipo VARCHAR(50), @Direccion NVARCHAR(200), @Localidad NVARCHAR(100), @M2 DECIMAL(10,2),
            @Ambientes INT, @Lote DECIMAL(10,2), @PrecioUsd DECIMAL(18,2), @Dias INT, @Cliente INT, @Inmueble INT, @Captacion INT,
            @FechaVenta DATE, @FechaCaptacion DATETIME, @Pesos DECIMAL(18,2), @DniCliente VARCHAR(15);
    DECLARE demo CURSOR LOCAL FAST_FORWARD FOR
        SELECT d.[orden], s.[id], d.[tipo], d.[direccion], d.[localidad], d.[m2], d.[ambientes], d.[lote], d.[precio_usd], d.[dias]
        FROM @Demo d JOIN @SinVentas s ON s.[clave] = d.[agente]
        ORDER BY d.[orden];
    OPEN demo;
    FETCH NEXT FROM demo INTO @Orden, @Agente, @Tipo, @Direccion, @Localidad, @M2, @Ambientes, @Lote, @PrecioUsd, @Dias;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @FechaVenta = CAST(DATEADD(DAY, -@Dias, GETDATE()) AS DATE);
        SET @FechaCaptacion = DATEADD(DAY, -60, CAST(@FechaVenta AS DATETIME));
        SET @Pesos = ISNULL(@PrecioUsd, 150000) * 1500;
        -- Clientes 44000001 en adelante (los bloques anteriores usan 41000001... y 43000001...)
        SET @DniCliente = CAST(44000000 + @Orden AS VARCHAR(15));
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Cliente] WHERE [dni] = @DniCliente)
            INSERT INTO [dbo].[Cliente] ([dni], [nombre], [apellido], [telefono], [email])
            VALUES (@DniCliente, N'Cliente', N'Ejemplo ' + CAST(40 + @Orden AS NVARCHAR(10)),
                    '11-4400-' + RIGHT('0000' + CAST(@Orden AS VARCHAR(4)), 4), NULL);
        SET @Cliente = (SELECT [id_cliente] FROM [dbo].[Cliente] WHERE [dni] = @DniCliente);

        INSERT INTO [dbo].[Inmueble] ([tipo_inmueble], [direccion], [metros_cuadrados], [cantidad_ambientes], [estado_actual], [id_cliente],
                                      [tipo_publicacion], [localidad], [superficie_lote], [texto_comercial], [fecha_publicacion])
        VALUES (@Tipo, @Direccion, @M2, @Ambientes, CASE WHEN @PrecioUsd IS NULL THEN 'Publicado' ELSE 'Vendido' END, @Cliente,
                'Venta', @Localidad, @Lote, N'Inmueble de ejemplo para el ranking de agentes.', DATEADD(DAY, 15, @FechaCaptacion));
        SET @Inmueble = SCOPE_IDENTITY();
        INSERT INTO [dbo].[Captacion] ([fecha_registro], [valor_pretendido], [id_inmueble], [id_usuario_agente])
        VALUES (@FechaCaptacion, @Pesos, @Inmueble, @Agente);
        SET @Captacion = SCOPE_IDENTITY();
        INSERT INTO [dbo].[Tasacion] ([fecha_dictamen], [estado_dictamen], [valor_final], [id_captacion], [id_usuario_tasador])
        VALUES (DATEADD(DAY, 7, @FechaCaptacion), 'Aprobada', ROUND(@Pesos * 0.95, 2), @Captacion, @Tasador);
        IF @PrecioUsd IS NOT NULL
            INSERT INTO [dbo].[Venta] ([id_inmueble], [id_usuario_agente], [fecha_venta], [precio_usd], [id_usuario_registro], [fecha_registro], [observaciones])
            VALUES (@Inmueble, @Agente, @FechaVenta, @PrecioUsd, @Registro, @FechaVenta, N'Venta de ejemplo para el ranking de agentes.');
        FETCH NEXT FROM demo INTO @Orden, @Agente, @Tipo, @Direccion, @Localidad, @M2, @Ambientes, @Lote, @PrecioUsd, @Dias;
    END
    CLOSE demo;
    DEALLOCATE demo;
    DECLARE @Asignados INT = (SELECT COUNT(*) FROM @SinVentas);
    PRINT N'Ranking de agentes: ventas de ejemplo asignadas a ' + CAST(@Asignados AS NVARCHAR(10)) +
          N' de los 4 primeros agentes (no tenían ventas).';
END
GO

-- Nombres de los clientes de ejemplo: los bloques anteriores los crean como "Cliente Ejemplo N" (DNI 41000001...,
-- 43000001... y 44000001...). Se reemplazan por nombres y apellidos inventados, en orden de DNI; también en las
-- bases que ya los tenían. Solo los que siguen con el nombre genérico (si alguien lo editó, se respeta).
DECLARE @Nombres TABLE ([orden] INT IDENTITY(1,1), [nombre] NVARCHAR(100), [apellido] NVARCHAR(100));
INSERT INTO @Nombres ([nombre], [apellido]) VALUES
    (N'Sofía', N'Álvarez'), (N'Matías', N'Domínguez'), (N'Julieta', N'Castro'), (N'Nicolás', N'Pereyra'),
    (N'Florencia', N'Suárez'), (N'Joaquín', N'Medina'), (N'Micaela', N'Ríos'), (N'Tomás', N'Aguirre'),
    (N'Camila', N'Benítez'), (N'Facundo', N'Ojeda'), (N'Victoria', N'Molina'), (N'Ignacio', N'Paz'),
    (N'Rocío', N'Vega'), (N'Bruno', N'Correa'), (N'Daniela', N'Figueroa'), (N'Leandro', N'Godoy'),
    (N'Antonella', N'Ibarra'), (N'Gonzalo', N'Navarro'), (N'Milagros', N'Ortiz'), (N'Federico', N'Ponce'),
    (N'Belén', N'Cardozo'), (N'Hernán', N'Luna'), (N'Carla', N'Bustos'), (N'Emiliano', N'Sánchez'),
    (N'Lucía', N'Moreno'), (N'Ezequiel', N'Ruiz'), (N'Agustina', N'Peralta'), (N'Marcos', N'Ferreyra'),
    (N'Natalia', N'Arias'), (N'Rodrigo', N'Vera'), (N'Paula', N'Coronel'), (N'Sebastián', N'Juárez'),
    (N'Valeria', N'Maldonado'), (N'Franco', N'Lencina'), (N'Lorena', N'Villegas'), (N'Damián', N'Escobar'),
    (N'Gabriela', N'Toledo'), (N'Maximiliano', N'Ramos'), (N'Celeste', N'Miranda'), (N'Pablo', N'Rojas'),
    (N'Romina', N'Galván'), (N'Cristian', N'Quinteros'), (N'Jimena', N'Carrizo'), (N'Lautaro', N'Méndez'),
    (N'Mariana', N'Ayala'), (N'Diego', N'Herrero'), (N'Silvina', N'Blanco'), (N'Alejandro', N'Funes'),
    (N'Verónica', N'Lucero'), (N'Andrés', N'Campos'), (N'Carolina', N'Vázquez'), (N'Santino', N'Roldán'),
    (N'Melina', N'Sequeira'), (N'Germán', N'Olmedo'), (N'Analía', N'Barrios'), (N'Fernando', N'Cáceres'),
    (N'Ailén', N'Moyano'), (N'Ramiro', N'Gutiérrez'), (N'Noelia', N'Prieto'), (N'Esteban', N'Montenegro');

WITH Genericos AS (
    SELECT c.[id_cliente], ROW_NUMBER() OVER (ORDER BY c.[dni]) AS [orden]
    FROM [dbo].[Cliente] c
    WHERE c.[nombre] = N'Cliente' AND c.[apellido] LIKE N'Ejemplo %'
      AND (c.[dni] LIKE '41______' OR c.[dni] LIKE '43______' OR c.[dni] LIKE '44______')
)
UPDATE c SET [nombre] = n.[nombre], [apellido] = n.[apellido]
FROM [dbo].[Cliente] c
JOIN Genericos g ON g.[id_cliente] = c.[id_cliente]
JOIN @Nombres n ON n.[orden] = g.[orden];
IF @@ROWCOUNT > 0 PRINT N'Clientes de ejemplo: nombres y apellidos asignados.';
GO

-- ====================================================
-- 4e. PN3: proyectos de inversión de ejemplo
-- ====================================================

-- Solo si todavía no hay ningún proyecto. Usa las casas Publicadas que cargaron los bloques de ejemplo del ranking
-- (se buscan por dirección; si alguna no está, ese proyecto no se crea): dos Aprobados con inversores y aportes, uno
-- En Análisis con los datos técnicos ya cargados (listo para el Directorio) y uno Rechazado. Fechas relativas al día
-- de la instalación. Las coordenadas (OpenStreetMap) se completan si faltan, para el mapa de los informes.
IF NOT EXISTS (SELECT 1 FROM [dbo].[ProyectoInversion])
BEGIN
    DECLARE @Proyectos TABLE ([orden] INT, [direccion] NVARCHAR(200), [latitud] DECIMAL(9,6), [longitud] DECIMAL(9,6),
                              [lote] DECIMAL(10,2), [planta] DECIMAL(10,2), [altura] DECIMAL(6,2), [costo_m2] DECIMAL(18,2),
                              [venta_m2] DECIMAL(18,2), [canje] DECIMAL(5,2), [estado] VARCHAR(20), [dias_creacion] INT,
                              [dias_decision] INT, [observaciones] NVARCHAR(1000), [id_inmueble] INT NULL, [id_proyecto] INT NULL);
    INSERT INTO @Proyectos ([orden], [direccion], [latitud], [longitud], [lote], [planta], [altura], [costo_m2], [venta_m2], [canje],
                            [estado], [dias_creacion], [dias_decision], [observaciones]) VALUES
        (1, N'Moldes 3100',       -34.555043, -58.467124, 280, 196, 15, 1150, 2900, 30, 'Aprobado',   150, 120,
            N'Aprobado: buena relación entre lo construible y el valor de venta de la zona.'),
        (2, N'Pedro Morán 3900',  -34.600957, -58.509272, 250, 175, 12, 1100, 2500, 32, 'Aprobado',   80,  45,
            N'Aprobado con el canje del 32% acordado con el propietario.'),
        (3, N'Morón 3200',        -34.625280, -58.476285, 200, 140, 9,  1050, 2300, 35, 'EnAnalisis', 20,  NULL, NULL),
        (4, N'Av. Santa Fe 1800', NULL,       NULL,       420, 250, 9,  1200, 2000, 30, 'Rechazado',  200, 170,
            N'Rechazado: con el valor de venta de la zona el margen para los inversores no justifica el riesgo.');

    UPDATE p SET [id_inmueble] = (SELECT TOP 1 i.[id_inmueble] FROM [dbo].[Inmueble] i
                                  WHERE i.[direccion] = p.[direccion] AND i.[tipo_inmueble] = 'Casa' AND i.[estado_actual] = 'Publicado'
                                  ORDER BY i.[id_inmueble])
    FROM @Proyectos p;
    DELETE FROM @Proyectos WHERE [id_inmueble] IS NULL;

    UPDATE i SET [latitud] = p.[latitud], [longitud] = p.[longitud]
    FROM [dbo].[Inmueble] i JOIN @Proyectos p ON p.[id_inmueble] = i.[id_inmueble]
    WHERE p.[latitud] IS NOT NULL AND i.[latitud] IS NULL AND i.[longitud] IS NULL;

    DECLARE @Arquitectura INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'Arquitectura');
    DECLARE @Directorio INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'Directorio');
    IF @Arquitectura IS NULL SET @Arquitectura = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin');
    IF @Directorio IS NULL SET @Directorio = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin');

    DECLARE @Orden INT, @IdProyecto INT;
    DECLARE proy CURSOR LOCAL FAST_FORWARD FOR SELECT [orden] FROM @Proyectos ORDER BY [orden];
    OPEN proy;
    FETCH NEXT FROM proy INTO @Orden;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        INSERT INTO [dbo].[ProyectoInversion] ([id_inmueble], [superficie_lote], [superficie_edificable_planta], [altura_maxima],
                                               [costo_obra_m2], [valor_venta_m2_unidades], [porcentaje_canje], [estado],
                                               [id_usuario_arquitectura], [id_usuario_directorio], [fecha_creacion], [fecha_decision], [observaciones])
        SELECT p.[id_inmueble], p.[lote], p.[planta], p.[altura], p.[costo_m2], p.[venta_m2], p.[canje], p.[estado],
               @Arquitectura, CASE WHEN p.[estado] = 'EnAnalisis' THEN NULL ELSE @Directorio END,
               DATEADD(DAY, -p.[dias_creacion], GETDATE()),
               CASE WHEN p.[dias_decision] IS NULL THEN NULL ELSE DATEADD(DAY, -p.[dias_decision], GETDATE()) END,
               p.[observaciones]
        FROM @Proyectos p WHERE p.[orden] = @Orden;
        SET @IdProyecto = SCOPE_IDENTITY();
        UPDATE @Proyectos SET [id_proyecto] = @IdProyecto WHERE [orden] = @Orden;
        FETCH NEXT FROM proy INTO @Orden;
    END
    CLOSE proy;
    DEALLOCATE proy;

    -- Inversores (CUIT con dígito verificador válido) y sus aportes a los fideicomisos aprobados: el de Moldes queda
    -- fondeado en parte y el de Pedro Morán completo (capital = m² construibles x costo de obra por m²)
    DECLARE @Inversores TABLE ([cuit] VARCHAR(13), [nombre] NVARCHAR(100), [apellido] NVARCHAR(100), [email] VARCHAR(100), [telefono] VARCHAR(30));
    INSERT INTO @Inversores VALUES
        ('20-28456123-7', N'Gabriel',  N'Fernández', 'gfernandez@mail.com', '11-5123-4567'),
        ('27-31567890-6', N'Lucía',    N'Martínez',  'lmartinez@mail.com',  '11-5234-5678'),
        ('20-25789456-9', N'Diego',    N'Romero',    'dromero@mail.com',    NULL),
        ('27-33124578-5', N'Carolina', N'Acosta',    'cacosta@mail.com',    '11-5456-7890'),
        ('20-30987654-8', N'Martín',   N'Giménez',   NULL,                  '11-5567-8901');
    INSERT INTO [dbo].[Inversor] ([nombre], [apellido], [cuit], [email], [telefono])
    SELECT v.[nombre], v.[apellido], v.[cuit], v.[email], v.[telefono]
    FROM @Inversores v
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Inversor] i WHERE i.[cuit] = v.[cuit]);

    DECLARE @Aportes TABLE ([orden] INT, [cuit] VARCHAR(13), [monto] DECIMAL(18,2), [unidades] INT, [dias] INT);
    INSERT INTO @Aportes VALUES
        (1, '20-28456123-7', 450000, 4, 110),
        (1, '27-31567890-6', 300000, 3, 95),
        (1, '20-25789456-9', 200000, 2, 60),
        (2, '27-33124578-5', 400000, 4, 40),
        (2, '20-30987654-8', 370000, 3, 30);
    INSERT INTO [dbo].[SuscripcionFideicomiso] ([id_proyecto], [id_inversor], [monto_aportado], [unidades_asignadas], [fecha_suscripcion])
    SELECT p.[id_proyecto], i.[id_inversor], a.[monto], a.[unidades], DATEADD(DAY, -a.[dias], GETDATE())
    FROM @Aportes a
    JOIN @Proyectos p ON p.[orden] = a.[orden] AND p.[estado] = 'Aprobado'
    JOIN [dbo].[Inversor] i ON i.[cuit] = a.[cuit];

    DECLARE @Creados INT = (SELECT COUNT(*) FROM @Proyectos);
    PRINT N'PN3: ' + CAST(@Creados AS NVARCHAR(10)) + N' proyectos de inversión de ejemplo creados, con inversores y aportes.';
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
    ('lblTasa', 'FormRegistrarPago', 'Tasa de mora:', 'Late interest rate:'),
    -- Domicilio del inquilino (paso 6: va en el PDF del contrato)
    ('lblDomicilio', 'FormRegistrarContrato', 'Domicilio:', 'Address:'),
    ('lblDomicilio', 'FormGestionarContratos', 'Domicilio:', 'Address:'),
    ('btnCompletarDomicilio', 'FormGestionarContratos', 'Completar domicilio', 'Add address'),
    -- PN3-1: ubicación y lote del inmueble en la captación
    ('lblLocalidad', 'FormRegistrarCaptacion', 'Localidad:', 'City:'),
    ('lblSuperficieLote', 'FormRegistrarCaptacion', N'Sup. lote (m²):', N'Lot area (m²):'),
    -- PN3-3: formularios del PN3
    ('btnCrearProyecto', 'FormIdentificarOportunidades', 'Crear Proyecto', 'Create Project'),
    ('btnVerAnalisis', 'FormIdentificarOportunidades', N'Ver Análisis', 'View Analysis'),
    ('grpInmueble', 'FormEvaluacionArquitectonica', 'Inmueble', 'Property'),
    ('lblDireccion', 'FormEvaluacionArquitectonica', N'Dirección:', 'Address:'),
    ('lblLocalidad', 'FormEvaluacionArquitectonica', 'Localidad:', 'City:'),
    ('lblTipo', 'FormEvaluacionArquitectonica', N'Tipo / operación:', 'Type / listing:'),
    ('lblSuperficie', 'FormEvaluacionArquitectonica', 'Superficie:', 'Area:'),
    ('lblCanje', 'FormEvaluacionArquitectonica', 'Canje propietario:', 'Owner swap:'),
    ('grpPropietario', 'FormEvaluacionArquitectonica', 'Propietario', 'Owner'),
    ('lblPropietario', 'FormEvaluacionArquitectonica', 'Nombre:', 'Name:'),
    ('lblDni', 'FormEvaluacionArquitectonica', 'DNI:', 'ID:'),
    ('lblTelefono', 'FormEvaluacionArquitectonica', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormEvaluacionArquitectonica', 'Email:', 'Email:'),
    ('grpDatosTecnicos', 'FormEvaluacionArquitectonica', N'Datos técnicos', 'Technical data'),
    ('lblLote', 'FormEvaluacionArquitectonica', N'Superficie lote (m²):', N'Lot area (m²):'),
    ('lblSupEdificable', 'FormEvaluacionArquitectonica', N'Sup. edificable por planta (m²):', N'Buildable area per floor (m²):'),
    ('lblAltura', 'FormEvaluacionArquitectonica', N'Altura máxima (m):', 'Maximum height (m):'),
    ('btnConsultarCaba', 'FormEvaluacionArquitectonica', 'Consultar CABA', 'Query CABA'),
    ('lblCostoM2', 'FormEvaluacionArquitectonica', N'Costo obra / m² (USD):', N'Build cost / m² (USD):'),
    ('lblValorM2', 'FormEvaluacionArquitectonica', N'Venta unid. / m² (USD):', N'Unit sale / m² (USD):'),
    ('lblMetros', 'FormEvaluacionArquitectonica', N'M² construibles:', N'Buildable m²:'),
    ('lblCostoTotal', 'FormEvaluacionArquitectonica', 'Costo total de obra:', 'Total build cost:'),
    ('lblValorTotal', 'FormEvaluacionArquitectonica', 'Valor total unidades:', 'Total units value:'),
    ('lblMargen', 'FormEvaluacionArquitectonica', 'Diferencia bruta:', 'Gross difference:'),
    ('lblIcc', 'FormEvaluacionArquitectonica', 'ICC actual (INDEC):', 'Current ICC (INDEC):'),
    ('btnGuardar', 'FormEvaluacionArquitectonica', N'Guardar datos técnicos', 'Save technical data'),
    ('grpEscenarioA', 'PanelAnalisis', 'Escenario A', 'Scenario A'),
    ('grpEscenarioB', 'PanelAnalisis', 'Escenario B: fideicomiso', 'Scenario B: trust'),
    ('grpMercado', 'PanelAnalisis', 'Datos de mercado', 'Market data'),
    ('lblMep', 'PanelAnalisis', N'Dólar MEP:', 'MEP dollar:'),
    ('lblBlue', 'PanelAnalisis', N'Dólar blue:', 'Blue dollar:'),
    ('lblOficial', 'PanelAnalisis', N'Dólar oficial (BCRA):', 'Official dollar (BCRA):'),
    ('lblIcc', 'PanelAnalisis', 'ICC (INDEC):', 'ICC (INDEC):'),
    ('lblIpc', 'PanelAnalisis', 'IPC (BCRA):', 'CPI (BCRA):'),
    ('grpConclusion', 'PanelAnalisis', N'Diferencia y recomendación', 'Difference and recommendation'),
    ('btnAprobar', 'FormDecisionDirectorio', 'Aprobar', 'Approve'),
    ('btnRechazar', 'FormDecisionDirectorio', 'Rechazar', 'Reject'),
    ('btnComisiones', 'FormDecisionDirectorio', 'Comisiones', 'Fees'),
    ('btnInforme', 'FormDecisionDirectorio', 'Informe PDF', 'PDF Report'),
    ('btnCerrar', 'FormVerAnalisis', 'Cerrar', 'Close'),
    ('grpTablero', 'FormGestionFideicomiso', 'Fondeo del fideicomiso', 'Trust funding'),
    ('lblNecesario', 'FormGestionFideicomiso', 'Capital necesario:', 'Required capital:'),
    ('lblSuscripto', 'FormGestionFideicomiso', 'Capital suscripto:', 'Subscribed capital:'),
    ('lblRestante', 'FormGestionFideicomiso', 'Capital restante:', 'Remaining capital:'),
    ('lblDolarMep', 'FormGestionFideicomiso', N'Dólar MEP:', 'MEP dollar:'),
    ('lblDolarBlue', 'FormGestionFideicomiso', N'Dólar blue:', 'Blue dollar:'),
    ('lblRestantePesos', 'FormGestionFideicomiso', 'Restante en pesos:', 'Remaining in pesos:'),
    ('grpInversores', 'FormGestionFideicomiso', 'Inversores', 'Investors'),
    ('btnAgregarInversor', 'FormGestionFideicomiso', 'Agregar Inversor', 'Add Investor'),
    ('btnInforme', 'FormGestionFideicomiso', 'Informe Inversores', 'Investor Report'),
    ('grpInversor', 'FormAgregarSuscripcion', 'Inversor', 'Investor'),
    ('lblCuit', 'FormAgregarSuscripcion', 'CUIT:', 'Tax ID (CUIT):'),
    ('btnBuscar', 'FormAgregarSuscripcion', 'Buscar', 'Search'),
    ('lblNombre', 'FormAgregarSuscripcion', 'Nombre:', 'Name:'),
    ('lblApellido', 'FormAgregarSuscripcion', 'Apellido:', 'Last Name:'),
    ('lblEmail', 'FormAgregarSuscripcion', 'Email:', 'Email:'),
    ('lblTelefono', 'FormAgregarSuscripcion', N'Teléfono:', 'Phone:'),
    ('grpAporte', 'FormAgregarSuscripcion', 'Aporte', 'Contribution'),
    ('lblMonto', 'FormAgregarSuscripcion', 'Monto (USD):', 'Amount (USD):'),
    ('lblUnidades', 'FormAgregarSuscripcion', 'Unidades asignadas:', 'Assigned units:'),
    ('btnGuardar', 'FormAgregarSuscripcion', 'Guardar Aporte', 'Save Contribution'),
    ('btnCancelar', 'FormAgregarSuscripcion', 'Cancelar', 'Cancel'),
    ('dgvCasas.direccion', 'FormIdentificarOportunidades', N'Dirección', 'Address'),
    ('dgvCasas.localidad', 'FormIdentificarOportunidades', 'Localidad', 'City'),
    ('dgvCasas.m2Terreno', 'FormIdentificarOportunidades', N'M² terreno', N'Lot m²'),
    ('dgvCasas.m2Inmueble', 'FormIdentificarOportunidades', N'M² inmueble', N'Property m²'),
    ('dgvCasas.ambientes', 'FormIdentificarOportunidades', 'Ambientes', 'Rooms'),
    ('dgvCasas.valorPretendido', 'FormIdentificarOportunidades', 'Valor pretendido', 'Asking price'),
    ('dgvCasas.operacion', 'FormIdentificarOportunidades', N'Operación', 'Listing type'),
    ('dgvCasas.estadoProyecto', 'FormIdentificarOportunidades', 'Estado proyecto', 'Project status'),
    ('dgvProyectos.nroProyecto', 'FormEvaluacionArquitectonica', N'N° Proyecto', 'Project No.'),
    ('dgvProyectos.direccion', 'FormEvaluacionArquitectonica', N'Dirección', 'Address'),
    ('dgvProyectos.localidad', 'FormEvaluacionArquitectonica', 'Localidad', 'City'),
    ('dgvProyectos.m2Terreno', 'FormEvaluacionArquitectonica', N'M² terreno', N'Lot m²'),
    ('dgvProyectos.operacion', 'FormEvaluacionArquitectonica', N'Operación', 'Listing type'),
    ('dgvProyectos.canje', 'FormEvaluacionArquitectonica', 'Canje %', 'Swap %'),
    ('dgvProyectos.datosTecnicos', 'FormEvaluacionArquitectonica', N'Datos técnicos', 'Technical data'),
    ('dgvProyectos.nroProyecto', 'FormDecisionDirectorio', N'N° Proyecto', 'Project No.'),
    ('dgvProyectos.direccion', 'FormDecisionDirectorio', N'Dirección', 'Address'),
    ('dgvProyectos.localidad', 'FormDecisionDirectorio', 'Localidad', 'City'),
    ('dgvProyectos.operacion', 'FormDecisionDirectorio', N'Operación', 'Listing type'),
    ('dgvProyectos.metrosConstruibles', 'FormDecisionDirectorio', N'M² construibles', N'Buildable m²'),
    ('dgvProyectos.costoTotalObra', 'FormDecisionDirectorio', 'Costo de obra (USD)', 'Build cost (USD)'),
    ('dgvProyectos.valorTotalUnidades', 'FormDecisionDirectorio', 'Valor unidades (USD)', 'Units value (USD)'),
    ('dgvProyectos.canje', 'FormDecisionDirectorio', 'Canje %', 'Swap %'),
    ('dgvProyectos.nroProyecto', 'FormGestionFideicomiso', N'N° Proyecto', 'Project No.'),
    ('dgvProyectos.direccion', 'FormGestionFideicomiso', N'Dirección', 'Address'),
    ('dgvProyectos.localidad', 'FormGestionFideicomiso', 'Localidad', 'City'),
    ('dgvProyectos.capitalNecesario', 'FormGestionFideicomiso', 'Capital necesario (USD)', 'Required capital (USD)'),
    ('dgvProyectos.capitalSuscripto', 'FormGestionFideicomiso', 'Suscripto (USD)', 'Subscribed (USD)'),
    ('dgvProyectos.fondeado', 'FormGestionFideicomiso', '% fondeado', '% funded'),
    ('dgvProyectos.inversores', 'FormGestionFideicomiso', 'Inversores', 'Investors'),
    ('dgvInversores.inversor', 'FormGestionFideicomiso', 'Inversor', 'Investor'),
    ('dgvInversores.cuit', 'FormGestionFideicomiso', 'CUIT', 'Tax ID'),
    ('dgvInversores.monto', 'FormGestionFideicomiso', 'Aporte (USD)', 'Contribution (USD)'),
    ('dgvInversores.participacion', 'FormGestionFideicomiso', N'Participación %', 'Share %'),
    ('dgvInversores.unidades', 'FormGestionFideicomiso', 'Unidades', 'Units'),
    ('dgvInversores.fecha', 'FormGestionFideicomiso', 'Fecha', 'Date'),
    ('pn3ToolStripMenuItem', 'FormMain', 'PN3', 'PN3'),
    ('identificarOportunidadesToolStripMenuItem', 'FormMain', 'Identificar Oportunidades', 'Identify Opportunities'),
    ('evaluacionArquitectonicaToolStripMenuItem', 'FormMain', N'Evaluación Arquitectónica', 'Architectural Assessment'),
    ('decisionDirectorioToolStripMenuItem', 'FormMain', N'Decisión del Directorio', 'Board Decision'),
    ('gestionFideicomisoToolStripMenuItem', 'FormMain', N'Gestión del Fideicomiso', 'Trust Management'),
    ('(form)', 'FormIdentificarOportunidades', 'Identificar Oportunidades', 'Identify Opportunities'),
    ('(form)', 'FormEvaluacionArquitectonica', N'Evaluación Arquitectónica', 'Architectural Assessment'),
    ('(form)', 'FormDecisionDirectorio', N'Decisión del Directorio', 'Board Decision'),
    ('(form)', 'FormVerAnalisis', N'Análisis Comparativo', 'Comparative Analysis'),
    ('(form)', 'FormGestionFideicomiso', N'Gestión del Fideicomiso', 'Trust Management'),
    ('(form)', 'FormAgregarSuscripcion', 'Agregar Inversor', 'Add Investor'),
    ('labelDni', 'FormGestionUsuarios', 'DNI:', 'ID number:'),
    ('dgvUsuarios.dni', 'FormGestionUsuarios', 'DNI', 'ID number'),
    ('btnRegistrarVenta', 'FormInmueblesPublicados', 'Registrar venta', 'Register sale'),
    ('reportesToolStripMenuItem', 'FormMain', 'Reportes', 'Reports'),
    ('rankingAgentesToolStripMenuItem', 'FormMain', 'Ranking de Agentes', 'Agent Ranking'),
    ('grpFiltros', 'FormRankingAgentes', 'Filtros', 'Filters'),
    ('lblPeriodo', 'FormRankingAgentes', N'Período:', 'Period:'),
    ('lblDesde', 'FormRankingAgentes', 'Desde:', 'From:'),
    ('lblHasta', 'FormRankingAgentes', 'Hasta:', 'To:'),
    ('lblOrden', 'FormRankingAgentes', 'Ordenar por:', 'Sort by:'),
    ('lblVista', 'FormRankingAgentes', 'Vista:', 'View:'),
    ('rbTodos', 'FormRankingAgentes', 'Todos los agentes', 'All agents'),
    ('rbMasVendieron', 'FormRankingAgentes', N'Top 5 más vendieron', 'Top 5 best sellers'),
    ('rbMenosVendieron', 'FormRankingAgentes', 'Top 5 menos vendieron', 'Top 5 lowest sellers'),
    ('btnExportarPdf', 'FormRankingAgentes', 'Exportar PDF', 'Export PDF'),
    ('cmbPeriodo.esteMes', 'FormRankingAgentes', 'Este mes', 'This month'),
    ('cmbPeriodo.esteTrimestre', 'FormRankingAgentes', 'Este trimestre', 'This quarter'),
    ('cmbPeriodo.esteAnio', 'FormRankingAgentes', N'Este año', 'This year'),
    ('cmbPeriodo.todo', 'FormRankingAgentes', 'Todo', 'All time'),
    ('cmbPeriodo.personalizado', 'FormRankingAgentes', 'Personalizado', 'Custom'),
    ('cmbOrden.cantidad', 'FormRankingAgentes', 'Cantidad de propiedades', 'Number of properties'),
    ('cmbOrden.valorTotal', 'FormRankingAgentes', 'Valor total (USD)', 'Total value (USD)'),
    ('dgvRanking.posicion', 'FormRankingAgentes', 'Pos.', 'Rank'),
    ('dgvRanking.dni', 'FormRankingAgentes', 'DNI', 'ID number'),
    ('dgvRanking.agente', 'FormRankingAgentes', 'Agente', 'Agent'),
    ('dgvRanking.cantidad', 'FormRankingAgentes', 'Propiedades vendidas', 'Properties sold'),
    ('dgvRanking.totalUsd', 'FormRankingAgentes', 'Valor total (USD)', 'Total value (USD)'),
    ('(form)', 'FormRankingAgentes', 'Ranking de Agentes', 'Agent Ranking'),
    ('btnVerSnapshot', 'FormGestionFideicomiso', N'Ver Actas de Decisión', 'View Decision Records'),
    ('btnVerActas', 'FormDecisionDirectorio', N'Ver Actas de Decisión', 'View Decision Records'),
    ('lblSnapshots', 'FormVerSnapshot', N'Actas de Decisión del inmueble:', 'Decision records of the property:'),
    ('lblDetalle', 'FormVerSnapshot', N'Análisis al momento de la decisión:', 'Analysis at the time of the decision:'),
    ('btnCerrar', 'FormVerSnapshot', 'Cerrar', 'Close'),
    ('(form)', 'FormVerSnapshot', N'Actas de Decisión', 'Decision Records'),
    -- Registrar Captación: validación de la dirección en el mapa mientras se escribe
    ('lblUbicacion.buscando', 'FormRegistrarCaptacion', N'Buscando ubicación...', N'Looking up the location...'),
    ('lblUbicacion.encontrada', 'FormRegistrarCaptacion', N'✓ Dirección encontrada en el mapa', N'✓ Address found on the map'),
    ('lblUbicacion.noEncontrada', 'FormRegistrarCaptacion', N'⚠ Dirección no encontrada, verificá que sea correcta', N'⚠ Address not found, please check that it is correct'),
    ('lblUbicacion.sinConexion', 'FormRegistrarCaptacion', N'No se pudo verificar la dirección (sin conexión)', N'Could not check the address (no connection)'),
    -- Columna Agente en las grillas del PN1 y descarga del PDF de un contrato existente
    ('dgvCaptaciones.agente', 'FormGestionarCaptaciones', 'Agente', 'Agent'),
    ('dgvInmuebles.agente', 'FormInmueblesPublicados', 'Agente', 'Agent'),
    ('btnDescargarPdf', 'FormGestionarContratos', 'Descargar contrato PDF', 'Download contract PDF'),
    -- Identificar Oportunidades: filtro por estado del proyecto
    ('lblFiltroEstado', 'FormIdentificarOportunidades', N'Estado del proyecto:', N'Project status:'),
    ('cmbFiltroEstado.todos', 'FormIdentificarOportunidades', N'Todos', N'All'),
    ('cmbFiltroEstado.sinProyecto', 'FormIdentificarOportunidades', N'Sin proyecto', N'No project'),
    ('cmbFiltroEstado.enAnalisis', 'FormIdentificarOportunidades', N'En Análisis', N'Under Analysis'),
    ('cmbFiltroEstado.aprobado', 'FormIdentificarOportunidades', N'Aprobado', N'Approved'),
    ('cmbFiltroEstado.rechazado', 'FormIdentificarOportunidades', N'Rechazado', N'Rejected'),
    -- Evaluación Arquitectónica: total construible debajo de la superficie edificable por planta ({0} m², {1} pisos, {2} m² por planta)
    ('lblConstruiblesTotales', 'FormEvaluacionArquitectonica', N'M² construibles totales: {0} m² ({1} pisos de {2} m²)', N'Total buildable m²: {0} m² ({1} floors of {2} m²)');

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

-- "Snapshot" pasó a llamarse "Acta de Decisión": se corrigen los textos que ya estaban cargados con el nombre anterior
UPDATE t SET t.[texto] = CASE t.[ididioma] WHEN 1 THEN n.[es] ELSE n.[en] END
FROM [dbo].[Traducciones] t
JOIN [dbo].[Control] c ON c.[id] = t.[idcontrol]
JOIN @Nuevos n ON n.[nombre] = c.[nombre] AND n.[formulario] = c.[formulario]
WHERE t.[ididioma] IN (1, 2) AND (t.[texto] LIKE '%napshot%' OR t.[texto] IN ('Decisiones guardadas:', 'Saved decisions:'));

-- Inicio de sesión: "Bienvenido" pasó a "Bienvenido a LandSmart" (solo si conserva el texto original: no se pisa un
-- texto cambiado desde Gestión de Idiomas)
UPDATE t SET t.[texto] = CASE t.[ididioma] WHEN 1 THEN N'Bienvenido a LandSmart' ELSE N'Welcome to LandSmart' END
FROM [dbo].[Traducciones] t
JOIN [dbo].[Control] c ON c.[id] = t.[idcontrol]
WHERE c.[nombre] = 'label1' AND c.[formulario] = 'FormLogin'
  AND ((t.[ididioma] = 1 AND t.[texto] = N'Bienvenido') OR (t.[ididioma] = 2 AND t.[texto] = N'Welcome'));

-- Evaluación Arquitectónica: la superficie edificable es por planta (solo si conserva el texto original)
UPDATE t SET t.[texto] = CASE t.[ididioma] WHEN 1 THEN N'Sup. edificable por planta (m²):' ELSE N'Buildable area per floor (m²):' END
FROM [dbo].[Traducciones] t
JOIN [dbo].[Control] c ON c.[id] = t.[idcontrol]
WHERE c.[nombre] = 'lblSupEdificable' AND c.[formulario] = 'FormEvaluacionArquitectonica'
  AND ((t.[ididioma] = 1 AND t.[texto] = N'Sup. edificable (m²):') OR (t.[ididioma] = 2 AND t.[texto] = N'Buildable area (m²):'));
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
    ('btnGuardar', 'FormRegistrarContrato'),
    -- PN3: FOS y FOT reemplazados por superficie edificable en planta y altura máxima (Código Urbanístico)
    ('lblFos', 'FormEvaluacionArquitectonica'),
    ('lblFot', 'FormEvaluacionArquitectonica');

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
