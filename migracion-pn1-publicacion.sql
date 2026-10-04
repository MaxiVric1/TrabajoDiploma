-- ====================================================
-- Migración: PN1 - Fechas de publicación de Inmueble
-- Permite mostrar la fecha de publicación y despublicar/republicar.
-- Un inmueble "despublicado" es uno en estado Tasado con fecha_publicacion (ya estuvo publicado).
-- No borra datos y se puede ejecutar más de una vez.
-- Ejecutar después de migracion-pn1-texto-comercial.sql (si se vuelve a correr aquel, correr este de nuevo).
-- ====================================================
USE [TpIngSoft];
GO

-- ====================================================
-- 1. Nuevas columnas
-- ====================================================

IF COL_LENGTH('dbo.Inmueble', 'fecha_publicacion') IS NULL
    ALTER TABLE [dbo].[Inmueble] ADD [fecha_publicacion] DATETIME NULL;
GO

IF COL_LENGTH('dbo.Inmueble', 'fecha_despublicacion') IS NULL
    ALTER TABLE [dbo].[Inmueble] ADD [fecha_despublicacion] DATETIME NULL;
GO

-- ====================================================
-- 2. Inmuebles ya publicados: la fecha sale de su último registro en bitácora
--    (InmuebleBLL.Publicar registra "Inmueble {id} publicado: ..."); si no hay, la fecha actual
-- ====================================================

UPDATE i
SET i.fecha_publicacion = ISNULL(b.ultima, GETDATE())
FROM [dbo].[Inmueble] i
OUTER APPLY (
    SELECT MAX(bi.fecha_hora) AS ultima
    FROM [dbo].[Bitacora] bi
    WHERE bi.actividad = N'Publicación Inmueble'
      AND bi.info_asociada LIKE 'Inmueble ' + CAST(i.id_inmueble AS VARCHAR(10)) + ' publicado%'
) b
WHERE i.estado_actual = 'Publicado' AND i.fecha_publicacion IS NULL;
GO

-- ====================================================
-- 3. Stored procedures
-- ====================================================

-- ====================================================
-- CambiarPublicacionInmueble
-- Publicar: Tasado -> Publicado (registra la fecha). Despublicar: Publicado -> Tasado.
-- Solo actúa si el inmueble está en el estado esperado: devuelve 1 fila afectada o 0.
-- ====================================================
IF OBJECT_ID('[dbo].[CambiarPublicacionInmueble]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CambiarPublicacionInmueble];
GO
CREATE PROCEDURE [dbo].[CambiarPublicacionInmueble]
    @Id INT,
    @Publicar BIT
AS BEGIN
    IF @Publicar = 1
        UPDATE Inmueble
        SET estado_actual = 'Publicado', fecha_publicacion = GETDATE(), fecha_despublicacion = NULL
        WHERE id_inmueble = @Id AND estado_actual = 'Tasado';
    ELSE
        UPDATE Inmueble
        SET estado_actual = 'Tasado', fecha_despublicacion = GETDATE()
        WHERE id_inmueble = @Id AND estado_actual = 'Publicado';
END
GO

-- ====================================================
-- LeerInmuebles
-- ====================================================
IF OBJECT_ID('[dbo].[LeerInmuebles]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerInmuebles];
GO
CREATE PROCEDURE [dbo].[LeerInmuebles]
AS BEGIN
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, texto_comercial,
           fecha_publicacion, fecha_despublicacion, id_cliente
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
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, texto_comercial,
           fecha_publicacion, fecha_despublicacion, id_cliente
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
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, texto_comercial,
           fecha_publicacion, fecha_despublicacion, id_cliente
    FROM Inmueble
    WHERE id_cliente = @IdCliente;
END
GO
