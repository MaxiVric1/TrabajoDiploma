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

PRINT N'Actualización del schema terminada.';
GO
