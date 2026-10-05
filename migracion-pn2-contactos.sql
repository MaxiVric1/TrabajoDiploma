-- ====================================================
-- Migración: PN2 - Contactos de cobranza
-- Registro de los intentos de contacto con el inquilino por pagos en mora (Gestionar Mora).
-- Se vinculan al contrato: un contacto trata toda la deuda del inquilino, aunque adeude varios meses.
-- No borra datos y se puede ejecutar más de una vez.
-- Ejecutar después de migracion-pn2-tablas.sql.
-- ====================================================
USE [TpIngSoft];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ====================================================
-- 1. Tabla
-- ====================================================

IF OBJECT_ID('[dbo].[ContactoCobranza]', 'U') IS NULL
CREATE TABLE [dbo].[ContactoCobranza] (
    [id_contacto] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_contacto] DATETIME NOT NULL,
    [descripcion] NVARCHAR(1000) NOT NULL,
    [id_contrato] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Contrato]([id_contrato]),
    [id_usuario_analista] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
);
GO

-- ====================================================
-- 2. Stored procedures - ContactoCobranza
-- ====================================================

-- ====================================================
-- CrearContactoCobranza
-- ====================================================
IF OBJECT_ID('[dbo].[CrearContactoCobranza]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearContactoCobranza];
GO
CREATE PROCEDURE [dbo].[CrearContactoCobranza]
    @FechaContacto DATETIME,
    @Descripcion NVARCHAR(1000),
    @IdContrato INT,
    @IdUsuarioAnalista INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO ContactoCobranza (fecha_contacto, descripcion, id_contrato, id_usuario_analista)
    VALUES (@FechaContacto, @Descripcion, @IdContrato, @IdUsuarioAnalista);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarContactoCobranza
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarContactoCobranza]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarContactoCobranza];
GO
CREATE PROCEDURE [dbo].[ActualizarContactoCobranza]
    @Id INT,
    @FechaContacto DATETIME,
    @Descripcion NVARCHAR(1000)
AS BEGIN
    UPDATE ContactoCobranza
    SET fecha_contacto = @FechaContacto, descripcion = @Descripcion
    WHERE id_contacto = @Id;
END
GO

-- ====================================================
-- EliminarContactoCobranza
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarContactoCobranza]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarContactoCobranza];
GO
CREATE PROCEDURE [dbo].[EliminarContactoCobranza]
    @Id INT
AS BEGIN
    DELETE FROM ContactoCobranza WHERE id_contacto = @Id;
END
GO

-- ====================================================
-- LeerContactosCobranza
-- ====================================================
IF OBJECT_ID('[dbo].[LeerContactosCobranza]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerContactosCobranza];
GO
CREATE PROCEDURE [dbo].[LeerContactosCobranza]
AS BEGIN
    SELECT id_contacto, fecha_contacto, descripcion, id_contrato, id_usuario_analista
    FROM ContactoCobranza
    ORDER BY fecha_contacto DESC;
END
GO

-- ====================================================
-- BuscarContactoCobranzaPorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarContactoCobranzaPorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarContactoCobranzaPorId];
GO
CREATE PROCEDURE [dbo].[BuscarContactoCobranzaPorId]
    @Id INT
AS BEGIN
    SELECT id_contacto, fecha_contacto, descripcion, id_contrato, id_usuario_analista
    FROM ContactoCobranza
    WHERE id_contacto = @Id;
END
GO

-- ====================================================
-- LeerContactosPorContrato (historial de contactos de un contrato, con el nombre del analista)
-- ====================================================
IF OBJECT_ID('[dbo].[LeerContactosPorContrato]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerContactosPorContrato];
GO
CREATE PROCEDURE [dbo].[LeerContactosPorContrato]
    @IdContrato INT
AS BEGIN
    SELECT cc.id_contacto, cc.fecha_contacto, cc.descripcion, cc.id_contrato, cc.id_usuario_analista,
           u.nombre AS analista
    FROM ContactoCobranza cc
    INNER JOIN Usuario u ON u.id = cc.id_usuario_analista
    WHERE cc.id_contrato = @IdContrato
    ORDER BY cc.fecha_contacto DESC;
END
GO
