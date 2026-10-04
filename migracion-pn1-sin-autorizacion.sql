-- ====================================================
-- Migración: PN1 - Quitar ruta_archivo_autorizacion de Captacion
-- La autorización de venta ya no se adjunta: el agente solo confirma en el formulario que la recibió firmada.
-- Se puede ejecutar más de una vez.
-- ====================================================
USE [TpIngSoft];
GO

-- ====================================================
-- 1. Eliminar la columna
-- ====================================================

IF COL_LENGTH('dbo.Captacion', 'ruta_archivo_autorizacion') IS NOT NULL
    ALTER TABLE [dbo].[Captacion] DROP COLUMN [ruta_archivo_autorizacion];
GO

-- ====================================================
-- 2. Stored procedures de Captacion sin la columna
-- ====================================================

-- ====================================================
-- CrearCaptacion
-- ====================================================
IF OBJECT_ID('[dbo].[CrearCaptacion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearCaptacion];
GO
CREATE PROCEDURE [dbo].[CrearCaptacion]
    @ValorPretendido DECIMAL(18,2),
    @IdInmueble INT,
    @IdUsuarioAgente INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Captacion (fecha_registro, valor_pretendido, id_inmueble, id_usuario_agente)
    VALUES (GETDATE(), @ValorPretendido, @IdInmueble, @IdUsuarioAgente);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarCaptacion
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarCaptacion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarCaptacion];
GO
CREATE PROCEDURE [dbo].[ActualizarCaptacion]
    @NroCaptacion INT,
    @ValorPretendido DECIMAL(18,2),
    @IdInmueble INT,
    @IdUsuarioAgente INT
AS BEGIN
    UPDATE Captacion
    SET valor_pretendido = @ValorPretendido, id_inmueble = @IdInmueble, id_usuario_agente = @IdUsuarioAgente
    WHERE nro_captacion = @NroCaptacion;
END
GO

-- ====================================================
-- LeerCaptaciones
-- ====================================================
IF OBJECT_ID('[dbo].[LeerCaptaciones]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerCaptaciones];
GO
CREATE PROCEDURE [dbo].[LeerCaptaciones]
AS BEGIN
    SELECT nro_captacion, fecha_registro, valor_pretendido, id_inmueble, id_usuario_agente
    FROM Captacion
    ORDER BY fecha_registro DESC;
END
GO

-- ====================================================
-- BuscarCaptacionPorNro
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarCaptacionPorNro]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarCaptacionPorNro];
GO
CREATE PROCEDURE [dbo].[BuscarCaptacionPorNro]
    @NroCaptacion INT
AS BEGIN
    SELECT nro_captacion, fecha_registro, valor_pretendido, id_inmueble, id_usuario_agente
    FROM Captacion
    WHERE nro_captacion = @NroCaptacion;
END
GO
