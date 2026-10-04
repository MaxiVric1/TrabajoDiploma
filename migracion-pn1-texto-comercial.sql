-- ====================================================
-- Migración: PN1 - Texto comercial en Inmueble
-- El texto comercial describe al inmueble, no a cada archivo: pasa de MaterialAudiovisual a Inmueble.
-- No pierde datos (copia el último texto cargado de cada inmueble) y se puede ejecutar más de una vez.
-- Ejecutar después de migracion-pn1-tablas.sql (si se vuelve a correr aquel, correr este de nuevo).
-- ====================================================
USE [TpIngSoft];
GO

-- ====================================================
-- 1. Nueva columna en Inmueble
-- ====================================================

IF COL_LENGTH('dbo.Inmueble', 'texto_comercial') IS NULL
    ALTER TABLE [dbo].[Inmueble] ADD [texto_comercial] NVARCHAR(2000) NULL;
GO

-- ====================================================
-- 2. Copiar el texto existente y quitar la columna de MaterialAudiovisual
--    (SQL dinámico: en una segunda ejecución la columna ya no existe y el lote no compilaría)
-- ====================================================

IF COL_LENGTH('dbo.MaterialAudiovisual', 'texto_comercial') IS NOT NULL
BEGIN
    -- Por cada inmueble sin texto, el último texto no vacío de su material
    EXEC sp_executesql N'
        UPDATE i
        SET i.texto_comercial = ultimo.texto_comercial
        FROM Inmueble i
        CROSS APPLY (
            SELECT TOP 1 m.texto_comercial
            FROM MaterialAudiovisual m
            WHERE m.id_inmueble = i.id_inmueble AND NULLIF(LTRIM(RTRIM(m.texto_comercial)), N'''') IS NOT NULL
            ORDER BY m.fecha_carga DESC, m.id_material DESC
        ) ultimo
        WHERE i.texto_comercial IS NULL;';

    EXEC sp_executesql N'ALTER TABLE [dbo].[MaterialAudiovisual] DROP COLUMN [texto_comercial];';
END
GO

-- ====================================================
-- 3. Stored procedures de Inmueble con texto_comercial
-- ====================================================

-- ====================================================
-- CrearInmueble
-- ====================================================
IF OBJECT_ID('[dbo].[CrearInmueble]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearInmueble];
GO
CREATE PROCEDURE [dbo].[CrearInmueble]
    @TipoInmueble VARCHAR(50),
    @Direccion NVARCHAR(200),
    @MetrosCuadrados DECIMAL(10,2),
    @CantidadAmbientes INT,
    @TextoComercial NVARCHAR(2000) = NULL,
    @IdCliente INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Inmueble (tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, texto_comercial, id_cliente)
    VALUES (@TipoInmueble, @Direccion, @MetrosCuadrados, @CantidadAmbientes, 'Captado', @TextoComercial, @IdCliente);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarInmueble
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarInmueble]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarInmueble];
GO
CREATE PROCEDURE [dbo].[ActualizarInmueble]
    @Id INT,
    @TipoInmueble VARCHAR(50),
    @Direccion NVARCHAR(200),
    @MetrosCuadrados DECIMAL(10,2),
    @CantidadAmbientes INT,
    @EstadoActual VARCHAR(20),
    @TextoComercial NVARCHAR(2000),
    @IdCliente INT
AS BEGIN
    UPDATE Inmueble
    SET tipo_inmueble = @TipoInmueble, direccion = @Direccion, metros_cuadrados = @MetrosCuadrados,
        cantidad_ambientes = @CantidadAmbientes, estado_actual = @EstadoActual,
        texto_comercial = @TextoComercial, id_cliente = @IdCliente
    WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- ActualizarTextoComercialInmueble
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarTextoComercialInmueble]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarTextoComercialInmueble];
GO
CREATE PROCEDURE [dbo].[ActualizarTextoComercialInmueble]
    @Id INT,
    @TextoComercial NVARCHAR(2000)
AS BEGIN
    UPDATE Inmueble SET texto_comercial = @TextoComercial WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- LeerInmuebles
-- ====================================================
IF OBJECT_ID('[dbo].[LeerInmuebles]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerInmuebles];
GO
CREATE PROCEDURE [dbo].[LeerInmuebles]
AS BEGIN
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, texto_comercial, id_cliente
    FROM Inmueble;
END
GO

-- ====================================================
-- BuscarInmueblePorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarInmueblePorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarInmueblePorId];
GO
CREATE PROCEDURE [dbo].[BuscarInmueblePorId]
    @Id INT
AS BEGIN
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, texto_comercial, id_cliente
    FROM Inmueble
    WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- LeerInmueblesPorCliente
-- ====================================================
IF OBJECT_ID('[dbo].[LeerInmueblesPorCliente]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerInmueblesPorCliente];
GO
CREATE PROCEDURE [dbo].[LeerInmueblesPorCliente]
    @IdCliente INT
AS BEGIN
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, texto_comercial, id_cliente
    FROM Inmueble
    WHERE id_cliente = @IdCliente;
END
GO

-- ====================================================
-- 4. Stored procedures de MaterialAudiovisual sin texto_comercial
-- ====================================================

-- ====================================================
-- CrearMaterialAudiovisual
-- ====================================================
IF OBJECT_ID('[dbo].[CrearMaterialAudiovisual]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearMaterialAudiovisual];
GO
CREATE PROCEDURE [dbo].[CrearMaterialAudiovisual]
    @TipoArchivo VARCHAR(20),
    @RutaArchivo NVARCHAR(500),
    @IdInmueble INT,
    @IdUsuarioMarketing INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO MaterialAudiovisual (tipo_archivo, ruta_archivo, fecha_carga, id_inmueble, id_usuario_marketing)
    VALUES (@TipoArchivo, @RutaArchivo, GETDATE(), @IdInmueble, @IdUsuarioMarketing);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarMaterialAudiovisual
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarMaterialAudiovisual]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarMaterialAudiovisual];
GO
CREATE PROCEDURE [dbo].[ActualizarMaterialAudiovisual]
    @Id INT,
    @TipoArchivo VARCHAR(20),
    @RutaArchivo NVARCHAR(500),
    @IdInmueble INT,
    @IdUsuarioMarketing INT
AS BEGIN
    UPDATE MaterialAudiovisual
    SET tipo_archivo = @TipoArchivo, ruta_archivo = @RutaArchivo,
        id_inmueble = @IdInmueble, id_usuario_marketing = @IdUsuarioMarketing
    WHERE id_material = @Id;
END
GO

-- ====================================================
-- LeerMaterialesAudiovisuales
-- ====================================================
IF OBJECT_ID('[dbo].[LeerMaterialesAudiovisuales]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerMaterialesAudiovisuales];
GO
CREATE PROCEDURE [dbo].[LeerMaterialesAudiovisuales]
AS BEGIN
    SELECT id_material, tipo_archivo, ruta_archivo, fecha_carga, id_inmueble, id_usuario_marketing
    FROM MaterialAudiovisual
    ORDER BY fecha_carga DESC;
END
GO

-- ====================================================
-- BuscarMaterialAudiovisualPorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarMaterialAudiovisualPorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarMaterialAudiovisualPorId];
GO
CREATE PROCEDURE [dbo].[BuscarMaterialAudiovisualPorId]
    @Id INT
AS BEGIN
    SELECT id_material, tipo_archivo, ruta_archivo, fecha_carga, id_inmueble, id_usuario_marketing
    FROM MaterialAudiovisual
    WHERE id_material = @Id;
END
GO

-- ====================================================
-- LeerMaterialPorInmueble
-- ====================================================
IF OBJECT_ID('[dbo].[LeerMaterialPorInmueble]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerMaterialPorInmueble];
GO
CREATE PROCEDURE [dbo].[LeerMaterialPorInmueble]
    @IdInmueble INT
AS BEGIN
    SELECT id_material, tipo_archivo, ruta_archivo, fecha_carga, id_inmueble, id_usuario_marketing
    FROM MaterialAudiovisual
    WHERE id_inmueble = @IdInmueble
    ORDER BY fecha_carga DESC;
END
GO
