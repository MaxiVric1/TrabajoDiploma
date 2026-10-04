-- ====================================================
-- Migración: PN1 - Captación de Propiedades
-- Crea las tablas y stored procedures del PN1 sobre una base existente.
-- No borra datos: cada tabla se crea solo si no existe.
-- Ejecutar después de schema-init.sql y stored-procedures.sql.
-- ====================================================
USE [TpIngSoft];
GO

-- ====================================================
-- 1. Creación de tablas
-- ====================================================

IF OBJECT_ID('[dbo].[Cliente]', 'U') IS NULL
CREATE TABLE [dbo].[Cliente] (
    [id_cliente] INT IDENTITY(1,1) PRIMARY KEY,
    [dni] VARCHAR(15) NOT NULL UNIQUE,
    [nombre] NVARCHAR(100) NOT NULL,
    [apellido] NVARCHAR(100) NOT NULL,
    [telefono] VARCHAR(30) NULL,
    [email] VARCHAR(100) NULL
);
GO

IF OBJECT_ID('[dbo].[Inmueble]', 'U') IS NULL
CREATE TABLE [dbo].[Inmueble] (
    [id_inmueble] INT IDENTITY(1,1) PRIMARY KEY,
    [tipo_inmueble] VARCHAR(50) NOT NULL,
    [direccion] NVARCHAR(200) NOT NULL,
    [metros_cuadrados] DECIMAL(10,2) NOT NULL CHECK ([metros_cuadrados] > 0),
    [cantidad_ambientes] INT NOT NULL CHECK ([cantidad_ambientes] > 0),
    [estado_actual] VARCHAR(20) NOT NULL DEFAULT 'Captado'
        CHECK ([estado_actual] IN ('Captado', 'Tasado', 'Rechazado', 'Publicado')),
    [id_cliente] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Cliente]([id_cliente])
);
GO

IF OBJECT_ID('[dbo].[Captacion]', 'U') IS NULL
CREATE TABLE [dbo].[Captacion] (
    [nro_captacion] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_registro] DATETIME DEFAULT GETDATE(),
    [valor_pretendido] DECIMAL(18,2) NOT NULL CHECK ([valor_pretendido] > 0),
    [id_inmueble] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Inmueble]([id_inmueble]),
    [id_usuario_agente] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
);
GO

IF OBJECT_ID('[dbo].[Tasacion]', 'U') IS NULL
CREATE TABLE [dbo].[Tasacion] (
    [nro_tasacion] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_dictamen] DATETIME DEFAULT GETDATE(),
    [estado_dictamen] VARCHAR(20) NOT NULL DEFAULT 'Pendiente'
        CHECK ([estado_dictamen] IN ('Pendiente', 'Aprobada', 'Rechazada')),
    [motivo_rechazo] NVARCHAR(500) NULL,
    [valor_final] DECIMAL(18,2) NULL CHECK ([valor_final] > 0),
    [id_captacion] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Captacion]([nro_captacion]),
    [id_usuario_tasador] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
    CHECK ([estado_dictamen] <> 'Rechazada' OR [motivo_rechazo] IS NOT NULL)  -- un rechazo debe tener motivo
);
GO

IF OBJECT_ID('[dbo].[MaterialAudiovisual]', 'U') IS NULL
CREATE TABLE [dbo].[MaterialAudiovisual] (
    [id_material] INT IDENTITY(1,1) PRIMARY KEY,
    [tipo_archivo] VARCHAR(20) NOT NULL,
    [ruta_archivo] NVARCHAR(500) NOT NULL,
    [texto_comercial] NVARCHAR(2000) NULL,
    [fecha_carga] DATETIME DEFAULT GETDATE(),
    [id_inmueble] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Inmueble]([id_inmueble]),
    [id_usuario_marketing] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
);
GO

-- ====================================================
-- 2. Stored procedures - Cliente
-- ====================================================

-- ====================================================
-- CrearCliente
-- ====================================================
IF OBJECT_ID('[dbo].[CrearCliente]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearCliente];
GO
CREATE PROCEDURE [dbo].[CrearCliente]
    @Dni VARCHAR(15),
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Telefono VARCHAR(30) = NULL,
    @Email VARCHAR(100) = NULL,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Cliente (dni, nombre, apellido, telefono, email)
    VALUES (@Dni, @Nombre, @Apellido, @Telefono, @Email);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarCliente
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarCliente]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarCliente];
GO
CREATE PROCEDURE [dbo].[ActualizarCliente]
    @Id INT,
    @Dni VARCHAR(15),
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Telefono VARCHAR(30) = NULL,
    @Email VARCHAR(100) = NULL
AS BEGIN
    UPDATE Cliente
    SET dni = @Dni, nombre = @Nombre, apellido = @Apellido, telefono = @Telefono, email = @Email
    WHERE id_cliente = @Id;
END
GO

-- ====================================================
-- EliminarCliente
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarCliente]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarCliente];
GO
CREATE PROCEDURE [dbo].[EliminarCliente]
    @Id INT
AS BEGIN
    DELETE FROM Cliente WHERE id_cliente = @Id;
END
GO

-- ====================================================
-- LeerClientes
-- ====================================================
IF OBJECT_ID('[dbo].[LeerClientes]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerClientes];
GO
CREATE PROCEDURE [dbo].[LeerClientes]
AS BEGIN
    SELECT id_cliente, dni, nombre, apellido, telefono, email
    FROM Cliente;
END
GO

-- ====================================================
-- BuscarClientePorId
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarClientePorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarClientePorId];
GO
CREATE PROCEDURE [dbo].[BuscarClientePorId]
    @Id INT
AS BEGIN
    SELECT id_cliente, dni, nombre, apellido, telefono, email
    FROM Cliente
    WHERE id_cliente = @Id;
END
GO

-- ====================================================
-- BuscarClientePorDni
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarClientePorDni]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BuscarClientePorDni];
GO
CREATE PROCEDURE [dbo].[BuscarClientePorDni]
    @Dni VARCHAR(15)
AS BEGIN
    SELECT id_cliente, dni, nombre, apellido, telefono, email
    FROM Cliente
    WHERE dni = @Dni;
END
GO

-- ====================================================
-- 3. Stored procedures - Inmueble
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
    @IdCliente INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Inmueble (tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, id_cliente)
    VALUES (@TipoInmueble, @Direccion, @MetrosCuadrados, @CantidadAmbientes, 'Captado', @IdCliente);
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
    @IdCliente INT
AS BEGIN
    UPDATE Inmueble
    SET tipo_inmueble = @TipoInmueble, direccion = @Direccion, metros_cuadrados = @MetrosCuadrados,
        cantidad_ambientes = @CantidadAmbientes, estado_actual = @EstadoActual, id_cliente = @IdCliente
    WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- ActualizarEstadoInmueble
-- ====================================================
IF OBJECT_ID('[dbo].[ActualizarEstadoInmueble]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarEstadoInmueble];
GO
CREATE PROCEDURE [dbo].[ActualizarEstadoInmueble]
    @Id INT,
    @EstadoActual VARCHAR(20)
AS BEGIN
    UPDATE Inmueble SET estado_actual = @EstadoActual WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- EliminarInmueble
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarInmueble]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarInmueble];
GO
CREATE PROCEDURE [dbo].[EliminarInmueble]
    @Id INT
AS BEGIN
    DELETE FROM Inmueble WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- LeerInmuebles
-- ====================================================
IF OBJECT_ID('[dbo].[LeerInmuebles]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerInmuebles];
GO
CREATE PROCEDURE [dbo].[LeerInmuebles]
AS BEGIN
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, id_cliente
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
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, id_cliente
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
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, id_cliente
    FROM Inmueble
    WHERE id_cliente = @IdCliente;
END
GO

-- ====================================================
-- 4. Stored procedures - Captacion
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
-- EliminarCaptacion
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarCaptacion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarCaptacion];
GO
CREATE PROCEDURE [dbo].[EliminarCaptacion]
    @NroCaptacion INT
AS BEGIN
    DELETE FROM Captacion WHERE nro_captacion = @NroCaptacion;
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

-- ====================================================
-- 5. Stored procedures - Tasacion
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
    @IdCaptacion INT,
    @IdUsuarioTasador INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Tasacion (fecha_dictamen, estado_dictamen, motivo_rechazo, valor_final, id_captacion, id_usuario_tasador)
    VALUES (GETDATE(), @EstadoDictamen, @MotivoRechazo, @ValorFinal, @IdCaptacion, @IdUsuarioTasador);
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
    @IdUsuarioTasador INT
AS BEGIN
    UPDATE Tasacion
    SET fecha_dictamen = GETDATE(), estado_dictamen = @EstadoDictamen, motivo_rechazo = @MotivoRechazo,
        valor_final = @ValorFinal, id_usuario_tasador = @IdUsuarioTasador
    WHERE nro_tasacion = @NroTasacion;
END
GO

-- ====================================================
-- EliminarTasacion
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarTasacion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarTasacion];
GO
CREATE PROCEDURE [dbo].[EliminarTasacion]
    @NroTasacion INT
AS BEGIN
    DELETE FROM Tasacion WHERE nro_tasacion = @NroTasacion;
END
GO

-- ====================================================
-- LeerTasaciones
-- ====================================================
IF OBJECT_ID('[dbo].[LeerTasaciones]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerTasaciones];
GO
CREATE PROCEDURE [dbo].[LeerTasaciones]
AS BEGIN
    SELECT nro_tasacion, fecha_dictamen, estado_dictamen, motivo_rechazo, valor_final, id_captacion, id_usuario_tasador
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
    SELECT nro_tasacion, fecha_dictamen, estado_dictamen, motivo_rechazo, valor_final, id_captacion, id_usuario_tasador
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
    SELECT nro_tasacion, fecha_dictamen, estado_dictamen, motivo_rechazo, valor_final, id_captacion, id_usuario_tasador
    FROM Tasacion
    WHERE id_captacion = @IdCaptacion
    ORDER BY fecha_dictamen DESC;
END
GO

-- ====================================================
-- 6. Stored procedures - MaterialAudiovisual
-- ====================================================

-- ====================================================
-- CrearMaterialAudiovisual
-- ====================================================
IF OBJECT_ID('[dbo].[CrearMaterialAudiovisual]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearMaterialAudiovisual];
GO
CREATE PROCEDURE [dbo].[CrearMaterialAudiovisual]
    @TipoArchivo VARCHAR(20),
    @RutaArchivo NVARCHAR(500),
    @TextoComercial NVARCHAR(2000) = NULL,
    @IdInmueble INT,
    @IdUsuarioMarketing INT,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO MaterialAudiovisual (tipo_archivo, ruta_archivo, texto_comercial, fecha_carga, id_inmueble, id_usuario_marketing)
    VALUES (@TipoArchivo, @RutaArchivo, @TextoComercial, GETDATE(), @IdInmueble, @IdUsuarioMarketing);
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
    @TextoComercial NVARCHAR(2000) = NULL,
    @IdInmueble INT,
    @IdUsuarioMarketing INT
AS BEGIN
    UPDATE MaterialAudiovisual
    SET tipo_archivo = @TipoArchivo, ruta_archivo = @RutaArchivo, texto_comercial = @TextoComercial,
        id_inmueble = @IdInmueble, id_usuario_marketing = @IdUsuarioMarketing
    WHERE id_material = @Id;
END
GO

-- ====================================================
-- EliminarMaterialAudiovisual
-- ====================================================
IF OBJECT_ID('[dbo].[EliminarMaterialAudiovisual]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarMaterialAudiovisual];
GO
CREATE PROCEDURE [dbo].[EliminarMaterialAudiovisual]
    @Id INT
AS BEGIN
    DELETE FROM MaterialAudiovisual WHERE id_material = @Id;
END
GO

-- ====================================================
-- LeerMaterialesAudiovisuales
-- ====================================================
IF OBJECT_ID('[dbo].[LeerMaterialesAudiovisuales]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerMaterialesAudiovisuales];
GO
CREATE PROCEDURE [dbo].[LeerMaterialesAudiovisuales]
AS BEGIN
    SELECT id_material, tipo_archivo, ruta_archivo, texto_comercial, fecha_carga, id_inmueble, id_usuario_marketing
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
    SELECT id_material, tipo_archivo, ruta_archivo, texto_comercial, fecha_carga, id_inmueble, id_usuario_marketing
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
    SELECT id_material, tipo_archivo, ruta_archivo, texto_comercial, fecha_carga, id_inmueble, id_usuario_marketing
    FROM MaterialAudiovisual
    WHERE id_inmueble = @IdInmueble
    ORDER BY fecha_carga DESC;
END
GO
