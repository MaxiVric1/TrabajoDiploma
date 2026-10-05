-- ====================================================
-- Migración: PN2 - Aviso de desocupación de contratos
-- Agrega a Contrato el aviso de que el inquilino desocupa el inmueble al vencimiento (y su fecha).
-- El aviso no cambia el estado: el contrato sigue vigente hasta que vence. Una renovación lo deja sin efecto.
-- Se puede ejecutar más de una vez.
-- Ejecutar después de migracion-pn2-tablas.sql (si se vuelve a correr aquel, correr este de nuevo).
-- ====================================================
USE [TpIngSoft];
GO

-- Requeridas por el índice filtrado de Contrato (ver migracion-pn2-tablas.sql)
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ====================================================
-- 1. Columnas
-- ====================================================
IF COL_LENGTH('dbo.Contrato', 'aviso_desocupacion') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [aviso_desocupacion] BIT NOT NULL
        CONSTRAINT [DF_Contrato_AvisoDesocupacion] DEFAULT 0;
GO

IF COL_LENGTH('dbo.Contrato', 'fecha_aviso_desocupacion') IS NULL
    ALTER TABLE [dbo].[Contrato] ADD [fecha_aviso_desocupacion] DATETIME NULL;
GO

-- Con aviso hay fecha; sin aviso, no
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Contrato_AvisoDesocupacion')
    ALTER TABLE [dbo].[Contrato] ADD CONSTRAINT [CK_Contrato_AvisoDesocupacion]
        CHECK (([aviso_desocupacion] = 1 AND [fecha_aviso_desocupacion] IS NOT NULL)
            OR ([aviso_desocupacion] = 0 AND [fecha_aviso_desocupacion] IS NULL));
GO

-- ====================================================
-- 2. Stored procedures
-- ====================================================

-- ====================================================
-- ActualizarContrato
-- @AvisoDesocupacion NULL conserva el aviso actual; 0 lo quita (renovación); 1 lo registra
-- (con @FechaAvisoDesocupacion, o la fecha actual si no se indica).
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarContrato]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarContrato];
GO
CREATE PROCEDURE [dbo].[ActualizarContrato]
    @Id INT,
    @FechaInicio DATE,
    @DuracionMeses INT,
    @MontoMensual DECIMAL(18,2),
    @EstadoContrato VARCHAR(20),
    @AlertaVencimiento BIT,
    @IdInquilino INT,
    @IdInmuebleAlquiler INT,
    @IdUsuarioAgente INT,
    @AvisoDesocupacion BIT = NULL,
    @FechaAvisoDesocupacion DATETIME = NULL
AS BEGIN
    UPDATE Contrato
    SET fecha_inicio = @FechaInicio, duracion_meses = @DuracionMeses, monto_mensual = @MontoMensual,
        estado_contrato = @EstadoContrato, alerta_vencimiento = @AlertaVencimiento,
        id_inquilino = @IdInquilino, id_inmueble_alquiler = @IdInmuebleAlquiler, id_usuario_agente = @IdUsuarioAgente,
        aviso_desocupacion = ISNULL(@AvisoDesocupacion, aviso_desocupacion),
        fecha_aviso_desocupacion = CASE
            WHEN @AvisoDesocupacion IS NULL THEN fecha_aviso_desocupacion
            WHEN @AvisoDesocupacion = 1 THEN ISNULL(@FechaAvisoDesocupacion, GETDATE())
            ELSE NULL END
    WHERE id_contrato = @Id;
END
GO

-- ====================================================
-- RegistrarAvisoDesocupacion
-- Solo contratos vigentes y sin aviso previo: 0 filas afectadas si no corresponde.
-- ====================================================
IF OBJECT_ID('[dbo].[RegistrarAvisoDesocupacion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[RegistrarAvisoDesocupacion];
GO
CREATE PROCEDURE [dbo].[RegistrarAvisoDesocupacion]
    @Id INT
AS BEGIN
    UPDATE Contrato
    SET aviso_desocupacion = 1, fecha_aviso_desocupacion = GETDATE()
    WHERE id_contrato = @Id
      AND estado_contrato <> 'Finalizado'
      AND aviso_desocupacion = 0;
END
GO

-- ====================================================
-- LeerContratos
-- ====================================================
IF OBJECT_ID('[dbo].[LeerContratos]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerContratos];
GO
CREATE PROCEDURE [dbo].[LeerContratos]
AS BEGIN
    SELECT id_contrato, fecha_inicio, duracion_meses, monto_mensual, estado_contrato, alerta_vencimiento,
           aviso_desocupacion, fecha_aviso_desocupacion,
           id_inquilino, id_inmueble_alquiler, id_usuario_agente,
           DATEADD(MONTH, duracion_meses, fecha_inicio) AS fecha_fin
    FROM Contrato
    ORDER BY fecha_inicio DESC;
END
GO

-- ====================================================
-- BuscarContratoPorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarContratoPorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarContratoPorId];
GO
CREATE PROCEDURE [dbo].[BuscarContratoPorId]
    @Id INT
AS BEGIN
    SELECT id_contrato, fecha_inicio, duracion_meses, monto_mensual, estado_contrato, alerta_vencimiento,
           aviso_desocupacion, fecha_aviso_desocupacion,
           id_inquilino, id_inmueble_alquiler, id_usuario_agente,
           DATEADD(MONTH, duracion_meses, fecha_inicio) AS fecha_fin
    FROM Contrato
    WHERE id_contrato = @Id;
END
GO

-- ====================================================
-- LeerContratosDetalle (para grillas: contrato + inquilino + inmueble; @Estado NULL = todos)
-- ====================================================
IF OBJECT_ID('[dbo].[LeerContratosDetalle]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerContratosDetalle];
GO
CREATE PROCEDURE [dbo].[LeerContratosDetalle]
    @Estado VARCHAR(20) = NULL
AS BEGIN
    SELECT c.id_contrato, c.fecha_inicio, c.duracion_meses, c.monto_mensual, c.estado_contrato, c.alerta_vencimiento,
           c.aviso_desocupacion, c.fecha_aviso_desocupacion,
           DATEADD(MONTH, c.duracion_meses, c.fecha_inicio) AS fecha_fin,
           c.id_inquilino, q.nombre + ' ' + q.apellido AS inquilino, q.dni AS dni_inquilino,
           c.id_inmueble_alquiler, i.direccion, i.tipo_inmueble, c.id_usuario_agente
    FROM Contrato c
    INNER JOIN Inquilino q ON q.id_inquilino = c.id_inquilino
    INNER JOIN InmuebleAlquiler i ON i.id_inmueble_alquiler = c.id_inmueble_alquiler
    WHERE @Estado IS NULL OR c.estado_contrato = @Estado
    ORDER BY c.fecha_inicio DESC;
END
GO

-- ====================================================
-- LeerContratosPorVencer (vigentes que terminan dentro de @DiasAnticipacion días)
-- ====================================================
IF OBJECT_ID('[dbo].[LeerContratosPorVencer]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerContratosPorVencer];
GO
CREATE PROCEDURE [dbo].[LeerContratosPorVencer]
    @DiasAnticipacion INT = 60
AS BEGIN
    SELECT c.id_contrato, c.fecha_inicio, c.duracion_meses, c.monto_mensual, c.estado_contrato, c.alerta_vencimiento,
           c.aviso_desocupacion, c.fecha_aviso_desocupacion,
           DATEADD(MONTH, c.duracion_meses, c.fecha_inicio) AS fecha_fin,
           DATEDIFF(DAY, CAST(GETDATE() AS DATE), DATEADD(MONTH, c.duracion_meses, c.fecha_inicio)) AS dias_restantes,
           c.id_inquilino, q.nombre + ' ' + q.apellido AS inquilino, q.dni AS dni_inquilino, q.telefono, q.email,
           c.id_inmueble_alquiler, i.direccion, i.tipo_inmueble, c.id_usuario_agente
    FROM Contrato c
    INNER JOIN Inquilino q ON q.id_inquilino = c.id_inquilino
    INNER JOIN InmuebleAlquiler i ON i.id_inmueble_alquiler = c.id_inmueble_alquiler
    WHERE c.estado_contrato <> 'Finalizado'
      AND DATEADD(MONTH, c.duracion_meses, c.fecha_inicio) <= DATEADD(DAY, @DiasAnticipacion, CAST(GETDATE() AS DATE))
    ORDER BY fecha_fin;
END
GO
