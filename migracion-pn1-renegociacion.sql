-- ====================================================
-- Migración: PN1 - Renegociación de captaciones
-- Agrega valor_sugerido a Tasacion: al rechazar, el tasador puede sugerir un valor alternativo.
-- No borra datos y se puede ejecutar más de una vez.
-- Ejecutar después de migracion-pn1-tablas.sql (si se vuelve a correr aquel, correr este de nuevo).
-- ====================================================
USE [TpIngSoft];
GO

-- ====================================================
-- 1. Nueva columna
-- ====================================================

IF COL_LENGTH('dbo.Tasacion', 'valor_sugerido') IS NULL
    ALTER TABLE [dbo].[Tasacion] ADD [valor_sugerido] DECIMAL(18,2) NULL;
GO

-- Solo una tasación rechazada puede sugerir un valor, y debe ser positivo
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Tasacion_ValorSugerido')
    ALTER TABLE [dbo].[Tasacion] ADD CONSTRAINT [CK_Tasacion_ValorSugerido]
        CHECK ([valor_sugerido] IS NULL OR ([valor_sugerido] > 0 AND [estado_dictamen] = 'Rechazada'));
GO

-- ====================================================
-- 2. Stored procedures de Tasacion actualizados
-- ====================================================

-- ====================================================
-- CrearTasacion
-- ====================================================
IF OBJECT_ID('[dbo].[CrearTasacion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearTasacion];
GO
CREATE PROCEDURE [dbo].[CrearTasacion]
    @EstadoDictamen VARCHAR(20),
    @MotivoRechazo NVARCHAR(500) = NULL,
    @ValorFinal DECIMAL(18,2) = NULL,
    @ValorSugerido DECIMAL(18,2) = NULL,
    @IdCaptacion INT,
    @IdUsuarioTasador INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Tasacion (fecha_dictamen, estado_dictamen, motivo_rechazo, valor_final, valor_sugerido, id_captacion, id_usuario_tasador)
    VALUES (GETDATE(), @EstadoDictamen, @MotivoRechazo, @ValorFinal, @ValorSugerido, @IdCaptacion, @IdUsuarioTasador);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarTasacion
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarTasacion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarTasacion];
GO
CREATE PROCEDURE [dbo].[ActualizarTasacion]
    @NroTasacion INT,
    @EstadoDictamen VARCHAR(20),
    @MotivoRechazo NVARCHAR(500) = NULL,
    @ValorFinal DECIMAL(18,2) = NULL,
    @ValorSugerido DECIMAL(18,2) = NULL,
    @IdUsuarioTasador INT
AS BEGIN
    UPDATE Tasacion
    SET fecha_dictamen = GETDATE(), estado_dictamen = @EstadoDictamen, motivo_rechazo = @MotivoRechazo,
        valor_final = @ValorFinal, valor_sugerido = @ValorSugerido, id_usuario_tasador = @IdUsuarioTasador
    WHERE nro_tasacion = @NroTasacion;
END
GO

-- ====================================================
-- LeerTasaciones
-- ====================================================
IF OBJECT_ID('[dbo].[LeerTasaciones]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerTasaciones];
GO
CREATE PROCEDURE [dbo].[LeerTasaciones]
AS BEGIN
    SELECT nro_tasacion, fecha_dictamen, estado_dictamen, motivo_rechazo, valor_final, valor_sugerido, id_captacion, id_usuario_tasador
    FROM Tasacion
    ORDER BY fecha_dictamen DESC;
END
GO

-- ====================================================
-- BuscarTasacionPorNro
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarTasacionPorNro]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarTasacionPorNro];
GO
CREATE PROCEDURE [dbo].[BuscarTasacionPorNro]
    @NroTasacion INT
AS BEGIN
    SELECT nro_tasacion, fecha_dictamen, estado_dictamen, motivo_rechazo, valor_final, valor_sugerido, id_captacion, id_usuario_tasador
    FROM Tasacion
    WHERE nro_tasacion = @NroTasacion;
END
GO

-- ====================================================
-- LeerTasacionesPorCaptacion
-- ====================================================
IF OBJECT_ID('[dbo].[LeerTasacionesPorCaptacion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerTasacionesPorCaptacion];
GO
CREATE PROCEDURE [dbo].[LeerTasacionesPorCaptacion]
    @IdCaptacion INT
AS BEGIN
    SELECT nro_tasacion, fecha_dictamen, estado_dictamen, motivo_rechazo, valor_final, valor_sugerido, id_captacion, id_usuario_tasador
    FROM Tasacion
    WHERE id_captacion = @IdCaptacion
    ORDER BY fecha_dictamen DESC, nro_tasacion DESC;
END
GO
