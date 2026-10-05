-- ====================================================
-- Migración: PN2 - Seguimiento de Alquileres
-- Crea las tablas y stored procedures del PN2 sobre una base existente.
-- No borra datos: cada tabla se crea solo si no existe.
-- Ejecutar después de schema-init.sql y stored-procedures.sql.
-- ====================================================
USE [TpIngSoft];
GO

-- Requeridas por el índice filtrado de Contrato, tanto para crearlo como para los SPs que escriben en la
-- tabla (cada SP guarda estas opciones al crearse). SSMS las trae activas; sqlcmd no, salvo con -I.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ====================================================
-- 1. Creación de tablas
-- ====================================================

IF OBJECT_ID('[dbo].[Inquilino]', 'U') IS NULL
CREATE TABLE [dbo].[Inquilino] (
    [id_inquilino] INT IDENTITY(1,1) PRIMARY KEY,
    [nombre] NVARCHAR(100) NOT NULL,
    [apellido] NVARCHAR(100) NOT NULL,
    [dni] VARCHAR(15) NOT NULL UNIQUE,
    [telefono] VARCHAR(30) NULL,
    [email] VARCHAR(100) NULL
);
GO

IF OBJECT_ID('[dbo].[InmuebleAlquiler]', 'U') IS NULL
CREATE TABLE [dbo].[InmuebleAlquiler] (
    [id_inmueble_alquiler] INT IDENTITY(1,1) PRIMARY KEY,
    [direccion] NVARCHAR(200) NOT NULL,
    [tipo_inmueble] VARCHAR(50) NOT NULL
        CHECK ([tipo_inmueble] IN ('Casa', 'Departamento', 'Local', 'Terreno', 'Oficina'))
);
GO

-- La fecha de fin no se guarda: es DATEADD(MONTH, duracion_meses, fecha_inicio) y la calculan los SPs
IF OBJECT_ID('[dbo].[Contrato]', 'U') IS NULL
CREATE TABLE [dbo].[Contrato] (
    [id_contrato] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_inicio] DATE NOT NULL,
    [duracion_meses] INT NOT NULL CHECK ([duracion_meses] > 0),
    [monto_mensual] DECIMAL(18,2) NOT NULL CHECK ([monto_mensual] > 0),
    [estado_contrato] VARCHAR(20) NOT NULL DEFAULT 'Activo'
        CHECK ([estado_contrato] IN ('Activo', 'Por Vencer', 'Finalizado')),
    [alerta_vencimiento] BIT NOT NULL DEFAULT 0,
    [id_inquilino] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Inquilino]([id_inquilino]),
    [id_inmueble_alquiler] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[InmuebleAlquiler]([id_inmueble_alquiler]),
    [id_usuario_agente] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
);
GO

-- Un inmueble no puede tener dos contratos vigentes (Activo o Por Vencer) al mismo tiempo
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Contrato_InmuebleVigente')
    CREATE UNIQUE INDEX [UX_Contrato_InmuebleVigente] ON [dbo].[Contrato]([id_inmueble_alquiler])
    WHERE [estado_contrato] <> 'Finalizado';
GO

-- Un registro por contrato y mes: fecha_pago y monto_pagado vacíos/0 mientras el mes no se pagó
IF OBJECT_ID('[dbo].[PagoAlquiler]', 'U') IS NULL
CREATE TABLE [dbo].[PagoAlquiler] (
    [id_pago] INT IDENTITY(1,1) PRIMARY KEY,
    [mes_correspondiente] INT NOT NULL CHECK ([mes_correspondiente] BETWEEN 1 AND 12),
    [anio_correspondiente] INT NOT NULL CHECK ([anio_correspondiente] BETWEEN 2000 AND 2100),
    [fecha_pago] DATETIME NULL,
    [monto_pagado] DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK ([monto_pagado] >= 0),
    [estado_pago] NVARCHAR(20) NOT NULL DEFAULT N'Al día'
        CHECK ([estado_pago] IN (N'Al día', N'En Mora', N'En Litigio')),
    [recargo_aplicado] DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK ([recargo_aplicado] >= 0),
    [id_contrato] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Contrato]([id_contrato]),
    [id_usuario_analista] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
    CONSTRAINT [UQ_PagoAlquiler_Periodo] UNIQUE ([id_contrato], [anio_correspondiente], [mes_correspondiente])
);
GO

IF OBJECT_ID('[dbo].[AccionLegal]', 'U') IS NULL
CREATE TABLE [dbo].[AccionLegal] (
    [id_accion] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_accion] DATETIME DEFAULT GETDATE(),
    [descripcion] NVARCHAR(1000) NOT NULL,
    [tipo_accion] VARCHAR(20) NOT NULL
        CHECK ([tipo_accion] IN ('Intimacion', 'Resuelto', 'Desalojo')),
    [id_contrato] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Contrato]([id_contrato]),
    [id_usuario_legal] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
);
GO

-- ====================================================
-- 2. Stored procedures - Inquilino
-- ====================================================

-- ====================================================
-- CrearInquilino
-- ====================================================
IF OBJECT_ID('[dbo].[CrearInquilino]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearInquilino];
GO
CREATE PROCEDURE [dbo].[CrearInquilino]
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni VARCHAR(15),
    @Telefono VARCHAR(30) = NULL,
    @Email VARCHAR(100) = NULL,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Inquilino (nombre, apellido, dni, telefono, email)
    VALUES (@Nombre, @Apellido, @Dni, @Telefono, @Email);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarInquilino
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarInquilino]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarInquilino];
GO
CREATE PROCEDURE [dbo].[ActualizarInquilino]
    @Id INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni VARCHAR(15),
    @Telefono VARCHAR(30) = NULL,
    @Email VARCHAR(100) = NULL
AS BEGIN
    UPDATE Inquilino
    SET nombre = @Nombre, apellido = @Apellido, dni = @Dni, telefono = @Telefono, email = @Email
    WHERE id_inquilino = @Id;
END
GO

-- ====================================================
-- EliminarInquilino
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarInquilino]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarInquilino];
GO
CREATE PROCEDURE [dbo].[EliminarInquilino]
    @Id INT
AS BEGIN
    DELETE FROM Inquilino WHERE id_inquilino = @Id;
END
GO

-- ====================================================
-- LeerInquilinos
-- ====================================================
IF OBJECT_ID('[dbo].[LeerInquilinos]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerInquilinos];
GO
CREATE PROCEDURE [dbo].[LeerInquilinos]
AS BEGIN
    SELECT id_inquilino, nombre, apellido, dni, telefono, email
    FROM Inquilino;
END
GO

-- ====================================================
-- BuscarInquilinoPorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarInquilinoPorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarInquilinoPorId];
GO
CREATE PROCEDURE [dbo].[BuscarInquilinoPorId]
    @Id INT
AS BEGIN
    SELECT id_inquilino, nombre, apellido, dni, telefono, email
    FROM Inquilino
    WHERE id_inquilino = @Id;
END
GO

-- ====================================================
-- BuscarInquilinoPorDni
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarInquilinoPorDni]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarInquilinoPorDni];
GO
CREATE PROCEDURE [dbo].[BuscarInquilinoPorDni]
    @Dni VARCHAR(15)
AS BEGIN
    SELECT id_inquilino, nombre, apellido, dni, telefono, email
    FROM Inquilino
    WHERE dni = @Dni;
END
GO

-- ====================================================
-- 3. Stored procedures - InmuebleAlquiler
-- ====================================================

-- ====================================================
-- CrearInmuebleAlquiler
-- ====================================================
IF OBJECT_ID('[dbo].[CrearInmuebleAlquiler]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearInmuebleAlquiler];
GO
CREATE PROCEDURE [dbo].[CrearInmuebleAlquiler]
    @Direccion NVARCHAR(200),
    @TipoInmueble VARCHAR(50),
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO InmuebleAlquiler (direccion, tipo_inmueble)
    VALUES (@Direccion, @TipoInmueble);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarInmuebleAlquiler
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarInmuebleAlquiler]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarInmuebleAlquiler];
GO
CREATE PROCEDURE [dbo].[ActualizarInmuebleAlquiler]
    @Id INT,
    @Direccion NVARCHAR(200),
    @TipoInmueble VARCHAR(50)
AS BEGIN
    UPDATE InmuebleAlquiler
    SET direccion = @Direccion, tipo_inmueble = @TipoInmueble
    WHERE id_inmueble_alquiler = @Id;
END
GO

-- ====================================================
-- EliminarInmuebleAlquiler
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarInmuebleAlquiler]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarInmuebleAlquiler];
GO
CREATE PROCEDURE [dbo].[EliminarInmuebleAlquiler]
    @Id INT
AS BEGIN
    DELETE FROM InmuebleAlquiler WHERE id_inmueble_alquiler = @Id;
END
GO

-- ====================================================
-- LeerInmueblesAlquiler
-- ====================================================
IF OBJECT_ID('[dbo].[LeerInmueblesAlquiler]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerInmueblesAlquiler];
GO
CREATE PROCEDURE [dbo].[LeerInmueblesAlquiler]
AS BEGIN
    SELECT id_inmueble_alquiler, direccion, tipo_inmueble
    FROM InmuebleAlquiler;
END
GO

-- ====================================================
-- BuscarInmuebleAlquilerPorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarInmuebleAlquilerPorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarInmuebleAlquilerPorId];
GO
CREATE PROCEDURE [dbo].[BuscarInmuebleAlquilerPorId]
    @Id INT
AS BEGIN
    SELECT id_inmueble_alquiler, direccion, tipo_inmueble
    FROM InmuebleAlquiler
    WHERE id_inmueble_alquiler = @Id;
END
GO

-- ====================================================
-- LeerInmueblesAlquilerDisponibles (sin contrato vigente; para Registrar Contrato)
-- ====================================================
IF OBJECT_ID('[dbo].[LeerInmueblesAlquilerDisponibles]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerInmueblesAlquilerDisponibles];
GO
CREATE PROCEDURE [dbo].[LeerInmueblesAlquilerDisponibles]
AS BEGIN
    SELECT i.id_inmueble_alquiler, i.direccion, i.tipo_inmueble
    FROM InmuebleAlquiler i
    WHERE NOT EXISTS (
        SELECT 1 FROM Contrato c
        WHERE c.id_inmueble_alquiler = i.id_inmueble_alquiler AND c.estado_contrato <> 'Finalizado'
    );
END
GO

-- ====================================================
-- 4. Stored procedures - Contrato
-- ====================================================

-- ====================================================
-- CrearContrato
-- ====================================================
IF OBJECT_ID('[dbo].[CrearContrato]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearContrato];
GO
CREATE PROCEDURE [dbo].[CrearContrato]
    @FechaInicio DATE,
    @DuracionMeses INT,
    @MontoMensual DECIMAL(18,2),
    @IdInquilino INT,
    @IdInmuebleAlquiler INT,
    @IdUsuarioAgente INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Contrato (fecha_inicio, duracion_meses, monto_mensual, estado_contrato, alerta_vencimiento,
                          id_inquilino, id_inmueble_alquiler, id_usuario_agente)
    VALUES (@FechaInicio, @DuracionMeses, @MontoMensual, 'Activo', 0,
            @IdInquilino, @IdInmuebleAlquiler, @IdUsuarioAgente);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarContrato
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
    @IdUsuarioAgente INT
AS BEGIN
    UPDATE Contrato
    SET fecha_inicio = @FechaInicio, duracion_meses = @DuracionMeses, monto_mensual = @MontoMensual,
        estado_contrato = @EstadoContrato, alerta_vencimiento = @AlertaVencimiento,
        id_inquilino = @IdInquilino, id_inmueble_alquiler = @IdInmuebleAlquiler, id_usuario_agente = @IdUsuarioAgente
    WHERE id_contrato = @Id;
END
GO

-- ====================================================
-- ActualizarEstadoContrato
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarEstadoContrato]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarEstadoContrato];
GO
CREATE PROCEDURE [dbo].[ActualizarEstadoContrato]
    @Id INT,
    @EstadoContrato VARCHAR(20),
    @AlertaVencimiento BIT
AS BEGIN
    UPDATE Contrato
    SET estado_contrato = @EstadoContrato, alerta_vencimiento = @AlertaVencimiento
    WHERE id_contrato = @Id;
END
GO

-- ====================================================
-- EliminarContrato
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarContrato]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarContrato];
GO
CREATE PROCEDURE [dbo].[EliminarContrato]
    @Id INT
AS BEGIN
    DELETE FROM Contrato WHERE id_contrato = @Id;
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

-- ====================================================
-- ActualizarVencimientoContratos
-- Activo -> Por Vencer (con alerta) si termina dentro de @DiasAnticipacion días;
-- vigentes con fecha de fin pasada -> Finalizado (sin alerta).
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarVencimientoContratos]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarVencimientoContratos];
GO
CREATE PROCEDURE [dbo].[ActualizarVencimientoContratos]
    @DiasAnticipacion INT = 60
AS BEGIN
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

    UPDATE Contrato
    SET estado_contrato = 'Finalizado', alerta_vencimiento = 0
    WHERE estado_contrato <> 'Finalizado'
      AND DATEADD(MONTH, duracion_meses, fecha_inicio) < @Hoy;

    UPDATE Contrato
    SET estado_contrato = 'Por Vencer', alerta_vencimiento = 1
    WHERE estado_contrato = 'Activo'
      AND DATEADD(MONTH, duracion_meses, fecha_inicio) <= DATEADD(DAY, @DiasAnticipacion, @Hoy);
END
GO

-- ====================================================
-- 5. Stored procedures - PagoAlquiler
-- ====================================================

-- ====================================================
-- CrearPagoAlquiler
-- ====================================================
IF OBJECT_ID('[dbo].[CrearPagoAlquiler]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearPagoAlquiler];
GO
CREATE PROCEDURE [dbo].[CrearPagoAlquiler]
    @MesCorrespondiente INT,
    @AnioCorrespondiente INT,
    @FechaPago DATETIME = NULL,
    @MontoPagado DECIMAL(18,2),
    @EstadoPago NVARCHAR(20),
    @RecargoAplicado DECIMAL(18,2) = 0,
    @IdContrato INT,
    @IdUsuarioAnalista INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO PagoAlquiler (mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
                              recargo_aplicado, id_contrato, id_usuario_analista)
    VALUES (@MesCorrespondiente, @AnioCorrespondiente, @FechaPago, @MontoPagado, @EstadoPago,
            @RecargoAplicado, @IdContrato, @IdUsuarioAnalista);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarPagoAlquiler
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarPagoAlquiler]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarPagoAlquiler];
GO
CREATE PROCEDURE [dbo].[ActualizarPagoAlquiler]
    @Id INT,
    @MesCorrespondiente INT,
    @AnioCorrespondiente INT,
    @FechaPago DATETIME = NULL,
    @MontoPagado DECIMAL(18,2),
    @EstadoPago NVARCHAR(20),
    @RecargoAplicado DECIMAL(18,2),
    @IdUsuarioAnalista INT
AS BEGIN
    UPDATE PagoAlquiler
    SET mes_correspondiente = @MesCorrespondiente, anio_correspondiente = @AnioCorrespondiente,
        fecha_pago = @FechaPago, monto_pagado = @MontoPagado, estado_pago = @EstadoPago,
        recargo_aplicado = @RecargoAplicado, id_usuario_analista = @IdUsuarioAnalista
    WHERE id_pago = @Id;
END
GO

-- ====================================================
-- ActualizarEstadoPago (Gestionar Mora / Litigios: cambia el estado y el recargo)
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarEstadoPago]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarEstadoPago];
GO
CREATE PROCEDURE [dbo].[ActualizarEstadoPago]
    @Id INT,
    @EstadoPago NVARCHAR(20),
    @RecargoAplicado DECIMAL(18,2)
AS BEGIN
    UPDATE PagoAlquiler
    SET estado_pago = @EstadoPago, recargo_aplicado = @RecargoAplicado
    WHERE id_pago = @Id;
END
GO

-- ====================================================
-- EliminarPagoAlquiler
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarPagoAlquiler]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarPagoAlquiler];
GO
CREATE PROCEDURE [dbo].[EliminarPagoAlquiler]
    @Id INT
AS BEGIN
    DELETE FROM PagoAlquiler WHERE id_pago = @Id;
END
GO

-- ====================================================
-- LeerPagosAlquiler
-- ====================================================
IF OBJECT_ID('[dbo].[LeerPagosAlquiler]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerPagosAlquiler];
GO
CREATE PROCEDURE [dbo].[LeerPagosAlquiler]
AS BEGIN
    SELECT id_pago, mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
           recargo_aplicado, id_contrato, id_usuario_analista
    FROM PagoAlquiler
    ORDER BY anio_correspondiente DESC, mes_correspondiente DESC;
END
GO

-- ====================================================
-- BuscarPagoAlquilerPorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarPagoAlquilerPorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarPagoAlquilerPorId];
GO
CREATE PROCEDURE [dbo].[BuscarPagoAlquilerPorId]
    @Id INT
AS BEGIN
    SELECT id_pago, mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
           recargo_aplicado, id_contrato, id_usuario_analista
    FROM PagoAlquiler
    WHERE id_pago = @Id;
END
GO

-- ====================================================
-- LeerPagosPorContrato (historial de pagos de un contrato)
-- ====================================================
IF OBJECT_ID('[dbo].[LeerPagosPorContrato]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerPagosPorContrato];
GO
CREATE PROCEDURE [dbo].[LeerPagosPorContrato]
    @IdContrato INT
AS BEGIN
    SELECT id_pago, mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
           recargo_aplicado, id_contrato, id_usuario_analista
    FROM PagoAlquiler
    WHERE id_contrato = @IdContrato
    ORDER BY anio_correspondiente DESC, mes_correspondiente DESC;
END
GO

-- ====================================================
-- BuscarPagoPorPeriodo (para Registrar Pago: si el mes ya tiene registro, se actualiza)
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarPagoPorPeriodo]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarPagoPorPeriodo];
GO
CREATE PROCEDURE [dbo].[BuscarPagoPorPeriodo]
    @IdContrato INT,
    @MesCorrespondiente INT,
    @AnioCorrespondiente INT
AS BEGIN
    SELECT id_pago, mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
           recargo_aplicado, id_contrato, id_usuario_analista
    FROM PagoAlquiler
    WHERE id_contrato = @IdContrato
      AND mes_correspondiente = @MesCorrespondiente
      AND anio_correspondiente = @AnioCorrespondiente;
END
GO

-- ====================================================
-- LeerPagosDetalle (Gestionar Mora / Litigios: pago + contrato + inquilino + inmueble; @Estado NULL = todos)
-- ====================================================
IF OBJECT_ID('[dbo].[LeerPagosDetalle]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerPagosDetalle];
GO
CREATE PROCEDURE [dbo].[LeerPagosDetalle]
    @EstadoPago NVARCHAR(20) = NULL
AS BEGIN
    SELECT p.id_pago, p.mes_correspondiente, p.anio_correspondiente, p.fecha_pago, p.monto_pagado, p.estado_pago,
           p.recargo_aplicado, p.id_contrato, p.id_usuario_analista,
           c.monto_mensual, c.estado_contrato,
           q.nombre + ' ' + q.apellido AS inquilino, q.dni AS dni_inquilino, q.telefono, q.email,
           i.direccion, i.tipo_inmueble
    FROM PagoAlquiler p
    INNER JOIN Contrato c ON c.id_contrato = p.id_contrato
    INNER JOIN Inquilino q ON q.id_inquilino = c.id_inquilino
    INNER JOIN InmuebleAlquiler i ON i.id_inmueble_alquiler = c.id_inmueble_alquiler
    WHERE @EstadoPago IS NULL OR p.estado_pago = @EstadoPago
    ORDER BY p.anio_correspondiente DESC, p.mes_correspondiente DESC;
END
GO

-- ====================================================
-- 6. Stored procedures - AccionLegal
-- ====================================================

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
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO AccionLegal (fecha_accion, descripcion, tipo_accion, id_contrato, id_usuario_legal)
    VALUES (GETDATE(), @Descripcion, @TipoAccion, @IdContrato, @IdUsuarioLegal);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarAccionLegal
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarAccionLegal]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarAccionLegal];
GO
CREATE PROCEDURE [dbo].[ActualizarAccionLegal]
    @Id INT,
    @Descripcion NVARCHAR(1000),
    @TipoAccion VARCHAR(20),
    @IdUsuarioLegal INT
AS BEGIN
    UPDATE AccionLegal
    SET descripcion = @Descripcion, tipo_accion = @TipoAccion, id_usuario_legal = @IdUsuarioLegal
    WHERE id_accion = @Id;
END
GO

-- ====================================================
-- EliminarAccionLegal
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarAccionLegal]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarAccionLegal];
GO
CREATE PROCEDURE [dbo].[EliminarAccionLegal]
    @Id INT
AS BEGIN
    DELETE FROM AccionLegal WHERE id_accion = @Id;
END
GO

-- ====================================================
-- LeerAccionesLegales
-- ====================================================
IF OBJECT_ID('[dbo].[LeerAccionesLegales]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerAccionesLegales];
GO
CREATE PROCEDURE [dbo].[LeerAccionesLegales]
AS BEGIN
    SELECT id_accion, fecha_accion, descripcion, tipo_accion, id_contrato, id_usuario_legal
    FROM AccionLegal
    ORDER BY fecha_accion DESC;
END
GO

-- ====================================================
-- BuscarAccionLegalPorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarAccionLegalPorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarAccionLegalPorId];
GO
CREATE PROCEDURE [dbo].[BuscarAccionLegalPorId]
    @Id INT
AS BEGIN
    SELECT id_accion, fecha_accion, descripcion, tipo_accion, id_contrato, id_usuario_legal
    FROM AccionLegal
    WHERE id_accion = @Id;
END
GO

-- ====================================================
-- LeerAccionesPorContrato (historial legal de un contrato, para Gestionar Litigios)
-- ====================================================
IF OBJECT_ID('[dbo].[LeerAccionesPorContrato]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerAccionesPorContrato];
GO
CREATE PROCEDURE [dbo].[LeerAccionesPorContrato]
    @IdContrato INT
AS BEGIN
    SELECT id_accion, fecha_accion, descripcion, tipo_accion, id_contrato, id_usuario_legal
    FROM AccionLegal
    WHERE id_contrato = @IdContrato
    ORDER BY fecha_accion DESC;
END
GO
