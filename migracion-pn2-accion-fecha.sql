-- ====================================================
-- Migración: PN2 - Fecha de las acciones legales
-- CrearAccionLegal acepta la fecha de la acción (p. ej. una intimación hecha ayer y cargada hoy).
-- @FechaAccion es opcional: si no se indica se usa la fecha actual, como antes.
-- Se puede ejecutar más de una vez.
-- Ejecutar después de migracion-pn2-tablas.sql (si se vuelve a correr aquel, correr este de nuevo).
-- ====================================================
USE [TpIngSoft];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ====================================================
-- CrearAccionLegal
-- ====================================================
IF OBJECT_ID('[dbo].[CrearAccionLegal]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearAccionLegal];
GO
CREATE PROCEDURE [dbo].[CrearAccionLegal]
    @Descripcion NVARCHAR(1000),
    @TipoAccion VARCHAR(20),
    @IdContrato INT,
    @IdUsuarioLegal INT,
    @FechaAccion DATETIME = NULL,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO AccionLegal (fecha_accion, descripcion, tipo_accion, id_contrato, id_usuario_legal)
    VALUES (ISNULL(@FechaAccion, GETDATE()), @Descripcion, @TipoAccion, @IdContrato, @IdUsuarioLegal);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO
