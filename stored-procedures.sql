-- ====================================================
-- TpIngSoft - Stored procedures (sistema base, PN1 y PN2), en su versión final
-- Cada uno se borra (DROP PROCEDURE IF EXISTS) y se vuelve a crear: se puede ejecutar más de una vez.
-- Ejecutar después de schema-init.sql y actualizacion-schema.sql (usan las columnas y tablas que agrega).
-- ====================================================
USE [TpIngSoft];
GO

-- Cada SP guarda estas opciones al crearse; los que escriben en Contrato las necesitan por su índice filtrado
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ####################################################
-- Sistema base (usuarios, sesiones, roles y permisos, bitácora, historial, dígitos verificadores, idiomas)
-- ####################################################

-- ====================================================
-- BuscarUsuarioPorNombre
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarUsuarioPorNombre];
GO


CREATE PROCEDURE [dbo].[BuscarUsuarioPorNombre]
    @Nombre VARCHAR(50)
AS
BEGIN
    SELECT id, nombre, nombre_persona, apellido, pass, activo, intentos_fallidos, bloqueado_hasta, dvh, id_idioma
    FROM Usuario 
    WHERE nombre = @Nombre AND activo = 1;
END
GO

-- ====================================================
-- BuscarUsuarioPorId
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarUsuarioPorId];
GO


CREATE PROCEDURE [dbo].[BuscarUsuarioPorId]
    @Id INT
AS
BEGIN
    SELECT id, nombre, nombre_persona, apellido, pass, activo, intentos_fallidos, bloqueado_hasta, dvh, id_idioma
    FROM Usuario 
    WHERE id = @Id;
END
GO

-- ====================================================
-- LeerUsuarios
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerUsuarios];
GO


CREATE PROCEDURE [dbo].[LeerUsuarios]
AS
BEGIN
    SELECT id, nombre, nombre_persona, apellido, pass, activo, intentos_fallidos, bloqueado_hasta, dvh, id_idioma
    FROM Usuario;
END
GO

-- ====================================================
-- CrearUsuario
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearUsuario];
GO


CREATE PROCEDURE [dbo].[CrearUsuario]
    @Nombre VARCHAR(50),
    @NombrePersona NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Pass VARCHAR(100),
    @DVH BIGINT,
    @NuevoId INT OUTPUT
AS
BEGIN
    INSERT INTO Usuario (nombre, nombre_persona, apellido, pass, activo, dvh)
    VALUES (@Nombre, @NombrePersona, @Apellido, @Pass, 1, @DVH);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarUsuario
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarUsuario];
GO


CREATE PROCEDURE [dbo].[ActualizarUsuario]
    @Id INT,
    @Nombre VARCHAR(50),
    @NombrePersona NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Pass VARCHAR(100),
    @Activo BIT,
    @DVH BIGINT
AS
BEGIN
    UPDATE Usuario
    SET nombre = @Nombre, nombre_persona = @NombrePersona, apellido = @Apellido, pass = @Pass, activo = @Activo, dvh = @DVH
    WHERE id = @Id;
END
GO

-- ====================================================
-- ActualizarIntentosFallidos
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarIntentosFallidos];
GO


CREATE PROCEDURE [dbo].[ActualizarIntentosFallidos]
    @Nombre VARCHAR(50),
    @Exitoso BIT
AS
BEGIN
    IF @Exitoso = 1
    BEGIN
        UPDATE Usuario SET intentos_fallidos = 0, bloqueado_hasta = NULL WHERE nombre = @Nombre;
    END
    ELSE
    BEGIN
        UPDATE Usuario SET intentos_fallidos = intentos_fallidos + 1 WHERE nombre = @Nombre;
        -- Bloqueo si llega a 3 intentos
        UPDATE Usuario SET bloqueado_hasta = DATEADD(MINUTE, 5, GETDATE()) 
        WHERE nombre = @Nombre AND intentos_fallidos >= 3;
    END
END
GO

-- ====================================================
-- LeerRolesPorUsuario
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerRolesPorUsuario];
GO


CREATE PROCEDURE [dbo].[LeerRolesPorUsuario]
    @IdUsuario INT
AS
BEGIN
    SELECT r.id, r.nombre
    FROM Rol r
    INNER JOIN UsuarioRol ur ON r.id = ur.id_rol
    WHERE ur.id_usuario = @IdUsuario;
END
GO

-- ====================================================
-- LeerPermisosPorRol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerPermisosPorRol];
GO


CREATE PROCEDURE [dbo].[LeerPermisosPorRol]
    @IdRol INT
AS
BEGIN
    SELECT p.id, p.nombre, 'Permiso' AS tipo
    FROM Permiso p
    INNER JOIN RolPermiso rp ON p.id = rp.id_hijo
    WHERE rp.id_padre = @IdRol AND rp.tipo_hijo = 'Permiso';
END
GO

-- ====================================================
-- CrearSesion
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearSesion];
GO


CREATE PROCEDURE [dbo].[CrearSesion]
    @IdUsuario INT,
    @NuevaSesionId INT OUTPUT
AS
BEGIN
    INSERT INTO Sesion (id_usuario, fecha_login)
    VALUES (@IdUsuario, GETDATE());
    SET @NuevaSesionId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- CerrarSesion
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CerrarSesion];
GO


CREATE PROCEDURE [dbo].[CerrarSesion]
    @IdSesion INT
AS
BEGIN
    UPDATE Sesion
    SET fecha_logout = GETDATE()
    WHERE id = @IdSesion;
END
GO

-- ====================================================
-- LeerRoles
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerRoles];
GO


CREATE PROCEDURE [dbo].[LeerRoles]
AS
BEGIN
    SELECT id, nombre 
    FROM Rol;
END
GO

-- ====================================================
-- AsignarRolUsuario
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[AsignarRolUsuario];
GO


CREATE PROCEDURE [dbo].[AsignarRolUsuario]
    @IdUsuario INT,
    @IdRol INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM UsuarioRol WHERE id_usuario = @IdUsuario AND id_rol = @IdRol)
    BEGIN
        INSERT INTO UsuarioRol (id_usuario, id_rol) 
        VALUES (@IdUsuario, @IdRol);
    END
END
GO

-- ====================================================
-- ValidarYRefrescarSesion
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ValidarYRefrescarSesion];
GO


CREATE PROCEDURE [dbo].[ValidarYRefrescarSesion]
    @IdSesion INT,
    @IdUsuario INT OUTPUT
AS
BEGIN
    SELECT @IdUsuario = id_usuario 
    FROM Sesion 
    WHERE id = @IdSesion AND fecha_logout IS NULL;

    IF @IdUsuario IS NOT NULL
    BEGIN
        UPDATE Sesion 
        SET fecha_login = GETDATE() 
        WHERE id = @IdSesion;
    END
    ELSE
    BEGIN
        SET @IdUsuario = -1;
    END
END
GO

-- ====================================================
-- InsertarBitacora
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[InsertarBitacora];
GO


CREATE PROCEDURE [dbo].[InsertarBitacora]
    @IdUsuario INT = NULL,
    @Actividad VARCHAR(100),
    @InfoAsociada VARCHAR(500) = NULL
AS
BEGIN
    INSERT INTO Bitacora (id_usuario, actividad, info_asociada)
    VALUES (@IdUsuario, @Actividad, @InfoAsociada);
END
GO

-- ====================================================
-- BuscarBitacora
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarBitacora];
GO


CREATE PROCEDURE [dbo].[BuscarBitacora]
    @IdUsuario INT = NULL,
    @Actividad VARCHAR(100) = NULL,
    @FechaDesde DATETIME = NULL,
    @FechaHasta DATETIME = NULL
AS
BEGIN
    SELECT b.id, b.fecha_hora, b.id_usuario, u.nombre as nombre_usuario, b.actividad, b.info_asociada
    FROM Bitacora b
    LEFT JOIN Usuario u ON b.id_usuario = u.id
    WHERE (@IdUsuario IS NULL OR b.id_usuario = @IdUsuario)
      AND (@Actividad IS NULL OR b.actividad LIKE '%' + @Actividad + '%')
      AND (@FechaDesde IS NULL OR b.fecha_hora >= @FechaDesde)
      AND (@FechaHasta IS NULL OR b.fecha_hora <= @FechaHasta)
    ORDER BY b.fecha_hora DESC;
END
GO

-- ====================================================
-- InsertarUsuarioHistorial
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[InsertarUsuarioHistorial];
GO


CREATE PROCEDURE [dbo].[InsertarUsuarioHistorial]
    @IdUsuario INT, 
    @Nombre VARCHAR(50), 
    @NombrePersona NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Pass VARCHAR(100), 
    @Activo BIT, 
    @DVH BIGINT,
    @IdUsuarioAutor INT, 
    @TipoOperacion VARCHAR(20)
AS 
BEGIN
    INSERT INTO Usuario_Historial (id_usuario, nombre, nombre_persona, apellido, pass, activo, dvh, id_usuario_autor, tipo_operacion)
    VALUES (@IdUsuario, @Nombre, @NombrePersona, @Apellido, @Pass, @Activo, @DVH, @IdUsuarioAutor, @TipoOperacion);
END;
GO

-- ====================================================
-- LeerHistorialUsuario
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerHistorialUsuario];
GO


CREATE PROCEDURE [dbo].[LeerHistorialUsuario]
    @IdUsuario INT
AS 
BEGIN
    SELECT * FROM Usuario_Historial WHERE id_usuario = @IdUsuario ORDER BY fecha_cambio DESC;
END;
GO

-- ====================================================
-- BuscarHistorialPorId
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarHistorialPorId];
GO


CREATE PROCEDURE [dbo].[BuscarHistorialPorId]
    @IdHistorial INT
AS 
BEGIN
    SELECT * FROM Usuario_Historial WHERE id_historial = @IdHistorial;
END;
GO

-- ====================================================
-- LeerDVV
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerDVV];
GO


CREATE PROCEDURE [dbo].[LeerDVV]
    @Tabla VARCHAR(50)
AS 
BEGIN
    SELECT dvv FROM DigitoVerificadorVertical WHERE tabla = @Tabla;
END;
GO

-- ====================================================
-- ActualizarDVV
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarDVV];
GO


CREATE PROCEDURE [dbo].[ActualizarDVV]
    @Tabla VARCHAR(50),
    @DVV BIGINT
AS 
BEGIN
    UPDATE DigitoVerificadorVertical SET dvv = @DVV WHERE tabla = @Tabla;
END;
GO

-- ====================================================
-- AsignarPermisoARol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[AsignarPermisoARol];
GO

CREATE PROCEDURE [dbo].[AsignarPermisoARol]
    @IdRol INT, @IdPermiso INT
AS BEGIN
    IF NOT EXISTS (SELECT 1 FROM RolPermiso WHERE id_padre=@IdRol AND id_hijo=@IdPermiso AND tipo_hijo='Permiso')
        INSERT INTO RolPermiso (id_padre, id_hijo, tipo_hijo) VALUES (@IdRol, @IdPermiso, 'Permiso');
END
GO

-- ====================================================
-- RemoverPermisoDeRol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[RemoverPermisoDeRol];
GO

CREATE PROCEDURE [dbo].[RemoverPermisoDeRol]
    @IdRol INT, @IdPermiso INT
AS BEGIN
    DELETE FROM RolPermiso WHERE id_padre=@IdRol AND id_hijo=@IdPermiso AND tipo_hijo='Permiso';
END
GO

-- ====================================================
-- AsignarSubRol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[AsignarSubRol];
GO

CREATE PROCEDURE [dbo].[AsignarSubRol]
    @IdPadre INT, @IdHijo INT
AS BEGIN
    IF NOT EXISTS (SELECT 1 FROM RolPermiso WHERE id_padre=@IdPadre AND id_hijo=@IdHijo AND tipo_hijo='Rol')
        INSERT INTO RolPermiso (id_padre, id_hijo, tipo_hijo) VALUES (@IdPadre, @IdHijo, 'Rol');
END
GO

-- ====================================================
-- RemoverSubRol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[RemoverSubRol];
GO

CREATE PROCEDURE [dbo].[RemoverSubRol]
    @IdPadre INT, @IdHijo INT
AS BEGIN
    DELETE FROM RolPermiso WHERE id_padre=@IdPadre AND id_hijo=@IdHijo AND tipo_hijo='Rol';
END
GO

-- ====================================================
-- LeerSubRolesPorRol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerSubRolesPorRol];
GO

CREATE PROCEDURE [dbo].[LeerSubRolesPorRol]
    @IdRol INT
AS BEGIN
    SELECT r.id, r.nombre, 'Rol' AS tipo
    FROM Rol r
    INNER JOIN RolPermiso rp ON r.id = rp.id_hijo
    WHERE rp.id_padre = @IdRol AND rp.tipo_hijo = 'Rol';
END
GO

-- ====================================================
-- CrearRol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearRol];
GO

CREATE PROCEDURE [dbo].[CrearRol]
    @Nombre NVARCHAR(100), @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Rol (nombre) VALUES (@Nombre);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarRol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarRol];
GO

CREATE PROCEDURE [dbo].[ActualizarRol]
    @Id INT, @Nombre NVARCHAR(100)
AS BEGIN
    UPDATE Rol SET nombre = @Nombre WHERE id = @Id;
END
GO

-- ====================================================
-- EliminarRol
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[EliminarRol];
GO

CREATE PROCEDURE [dbo].[EliminarRol]
    @Id INT
AS BEGIN
    DELETE FROM RolPermiso WHERE id_padre = @Id;
    DELETE FROM RolPermiso WHERE id_hijo = @Id AND tipo_hijo = 'Rol';
    DELETE FROM UsuarioRol WHERE id_rol = @Id;
    DELETE FROM Rol WHERE id = @Id;
END
GO

-- ====================================================
-- LeerRolPorId
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerRolPorId];
GO

CREATE PROCEDURE [dbo].[LeerRolPorId]
    @Id INT
AS BEGIN
    SELECT id, nombre FROM Rol WHERE id = @Id;
END
GO

-- ====================================================
-- LeerPermisos
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerPermisos];
GO

CREATE PROCEDURE [dbo].[LeerPermisos]
AS BEGIN
    SELECT id, nombre FROM Permiso;
END
GO

-- ====================================================
-- LeerIdiomasActivos
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerIdiomasActivos];
GO

CREATE PROCEDURE [dbo].[LeerIdiomasActivos]
AS BEGIN
    SELECT id, nombre, activo FROM Idioma WHERE activo = 1;
END
GO

-- ====================================================
-- LeerTodosIdiomas
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerTodosIdiomas];
GO

CREATE PROCEDURE [dbo].[LeerTodosIdiomas]
AS BEGIN
    SELECT id, nombre, activo FROM Idioma;
END
GO

-- ====================================================
-- ToggleEstadoIdioma
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ToggleEstadoIdioma];
GO

CREATE PROCEDURE [dbo].[ToggleEstadoIdioma]
    @IdIdioma INT
AS BEGIN
    UPDATE Idioma SET activo = activo ^ 1 WHERE id = @IdIdioma;
END
GO

-- ====================================================
-- LeerTraduccionesPorIdioma
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerTraduccionesPorIdioma];
GO

CREATE PROCEDURE [dbo].[LeerTraduccionesPorIdioma]
    @IdIdioma INT
AS BEGIN
    SELECT t.idcontrol, t.ididioma, c.nombre AS NombreControl, c.formulario AS Formulario, t.texto
    FROM Traducciones t
    INNER JOIN Control c ON t.idcontrol = c.id
    WHERE t.ididioma = @IdIdioma;
END
GO

-- ====================================================
-- CrearIdioma
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearIdioma];
GO

CREATE PROCEDURE [dbo].[CrearIdioma]
    @Nombre NVARCHAR(50),
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Idioma (nombre, activo) VALUES (@Nombre, 0);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarTraduccion
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarTraduccion];
GO

CREATE PROCEDURE [dbo].[ActualizarTraduccion]
    @IdControl INT,
    @IdIdioma INT,
    @Texto NVARCHAR(500)
AS BEGIN
    IF EXISTS (SELECT 1 FROM Traducciones WHERE idcontrol = @IdControl AND ididioma = @IdIdioma)
        UPDATE Traducciones SET texto = @Texto WHERE idcontrol = @IdControl AND ididioma = @IdIdioma;
    ELSE
        INSERT INTO Traducciones (idcontrol, ididioma, texto) VALUES (@IdControl, @IdIdioma, @Texto);
END
GO

-- ====================================================
-- LeerControles
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerControles];
GO

CREATE PROCEDURE [dbo].[LeerControles]
AS BEGIN
    SELECT id, nombre, formulario FROM Control;
END
GO

-- ====================================================
-- CrearControl
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearControl];
GO

CREATE PROCEDURE [dbo].[CrearControl]
    @Nombre NVARCHAR(100),
    @Formulario NVARCHAR(100),
    @NuevoId INT OUTPUT
AS BEGIN
    SELECT @NuevoId = id FROM Control WHERE nombre = @Nombre AND formulario = @Formulario;
    IF @NuevoId IS NULL
    BEGIN
        INSERT INTO Control (nombre, formulario) VALUES (@Nombre, @Formulario);
        SET @NuevoId = SCOPE_IDENTITY();
    END
END
GO

-- ====================================================
-- GuardarIdiomaUsuario
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[GuardarIdiomaUsuario];
GO

CREATE PROCEDURE [dbo].[GuardarIdiomaUsuario]
    @IdUsuario INT,
    @IdIdioma INT
AS BEGIN
    UPDATE Usuario SET id_idioma = @IdIdioma WHERE id = @IdUsuario;
END
GO

-- ====================================================
-- LeerIdiomaUsuario
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerIdiomaUsuario];
GO

CREATE PROCEDURE [dbo].[LeerIdiomaUsuario]
    @IdUsuario INT
AS BEGIN
    SELECT i.id, i.nombre, i.activo 
    FROM Idioma i
    INNER JOIN Usuario u ON u.id_idioma = i.id
    WHERE u.id = @IdUsuario;
END
GO

-- ====================================================
-- RemoverRolesUsuario
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[RemoverRolesUsuario];
GO

CREATE PROCEDURE [dbo].[RemoverRolesUsuario]
    @IdUsuario INT
AS BEGIN
    DELETE FROM UsuarioRol WHERE id_usuario = @IdUsuario;
END
GO

-- ====================================================
-- BloquearUsuarioManual
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BloquearUsuarioManual];
GO

CREATE PROCEDURE [dbo].[BloquearUsuarioManual]
    @Nombre VARCHAR(50),
    @Minutos INT
AS BEGIN
    UPDATE Usuario 
    SET bloqueado_hasta = DATEADD(MINUTE, @Minutos, GETDATE())
    WHERE nombre = @Nombre;
END
GO

-- ====================================================
-- EliminarIdioma
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[EliminarIdioma];
GO

CREATE PROCEDURE [dbo].[EliminarIdioma]
    @IdIdioma INT
AS BEGIN
    -- Soft delete: set activo = 0
    UPDATE Idioma SET activo = 0 WHERE id = @IdIdioma;
END
GO

-- ====================================================
-- ReasignarUsuariosIdioma
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ReasignarUsuariosIdioma];
GO

CREATE PROCEDURE [dbo].[ReasignarUsuariosIdioma]
    @IdIdiomaDesactivado INT,
    @IdIdiomaFallback INT
AS BEGIN
    UPDATE Usuario SET id_idioma = @IdIdiomaFallback WHERE id_idioma = @IdIdiomaDesactivado;
END
GO

-- ####################################################
-- PN1 - Captación de propiedades
-- ####################################################

-- ====================================================
-- CrearCliente
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearCliente];
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
DROP PROCEDURE IF EXISTS [dbo].[ActualizarCliente];
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
DROP PROCEDURE IF EXISTS [dbo].[EliminarCliente];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerClientes];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarClientePorId];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarClientePorDni];
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
-- CrearInmueble
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearInmueble];
GO

CREATE PROCEDURE [dbo].[CrearInmueble]
    @TipoInmueble VARCHAR(50),
    @Direccion NVARCHAR(200),
    @MetrosCuadrados DECIMAL(10,2),
    @CantidadAmbientes INT,
    @TextoComercial NVARCHAR(2000) = NULL,
    @IdCliente INT,
    @TipoPublicacion VARCHAR(10) = 'Venta',     -- 'Venta' / 'Alquiler'
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Inmueble (tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, texto_comercial, id_cliente,
                          tipo_publicacion)
    VALUES (@TipoInmueble, @Direccion, @MetrosCuadrados, @CantidadAmbientes, 'Captado', @TextoComercial, @IdCliente,
            @TipoPublicacion);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarInmueble
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarInmueble];
GO

CREATE PROCEDURE [dbo].[ActualizarInmueble]
    @Id INT,
    @TipoInmueble VARCHAR(50),
    @Direccion NVARCHAR(200),
    @MetrosCuadrados DECIMAL(10,2),
    @CantidadAmbientes INT,
    @EstadoActual VARCHAR(20),
    @TextoComercial NVARCHAR(2000),
    @IdCliente INT,
    @TipoPublicacion VARCHAR(10) = NULL        -- NULL conserva el actual
AS BEGIN
    UPDATE Inmueble
    SET tipo_inmueble = @TipoInmueble, direccion = @Direccion, metros_cuadrados = @MetrosCuadrados,
        cantidad_ambientes = @CantidadAmbientes, estado_actual = @EstadoActual,
        texto_comercial = @TextoComercial, id_cliente = @IdCliente,
        tipo_publicacion = ISNULL(@TipoPublicacion, tipo_publicacion)
    WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- ActualizarEstadoInmueble
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarEstadoInmueble];
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
DROP PROCEDURE IF EXISTS [dbo].[EliminarInmueble];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerInmuebles];
GO

CREATE PROCEDURE [dbo].[LeerInmuebles]
AS BEGIN
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, tipo_publicacion, texto_comercial,
           fecha_publicacion, fecha_despublicacion, id_cliente
    FROM Inmueble;
END
GO

-- ====================================================
-- BuscarInmueblePorId
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarInmueblePorId];
GO

CREATE PROCEDURE [dbo].[BuscarInmueblePorId]
    @Id INT
AS BEGIN
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, tipo_publicacion, texto_comercial,
           fecha_publicacion, fecha_despublicacion, id_cliente
    FROM Inmueble
    WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- LeerInmueblesPorCliente
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerInmueblesPorCliente];
GO

CREATE PROCEDURE [dbo].[LeerInmueblesPorCliente]
    @IdCliente INT
AS BEGIN
    SELECT id_inmueble, tipo_inmueble, direccion, metros_cuadrados, cantidad_ambientes, estado_actual, tipo_publicacion, texto_comercial,
           fecha_publicacion, fecha_despublicacion, id_cliente
    FROM Inmueble
    WHERE id_cliente = @IdCliente;
END
GO

-- ====================================================
-- CrearCaptacion
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearCaptacion];
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
DROP PROCEDURE IF EXISTS [dbo].[ActualizarCaptacion];
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
DROP PROCEDURE IF EXISTS [dbo].[EliminarCaptacion];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerCaptaciones];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarCaptacionPorNro];
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
-- CrearTasacion
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearTasacion];
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
DROP PROCEDURE IF EXISTS [dbo].[ActualizarTasacion];
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
-- EliminarTasacion
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[EliminarTasacion];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerTasaciones];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarTasacionPorNro];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerTasacionesPorCaptacion];
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

-- ====================================================
-- CrearMaterialAudiovisual
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearMaterialAudiovisual];
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
DROP PROCEDURE IF EXISTS [dbo].[ActualizarMaterialAudiovisual];
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
-- EliminarMaterialAudiovisual
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[EliminarMaterialAudiovisual];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerMaterialesAudiovisuales];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarMaterialAudiovisualPorId];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerMaterialPorInmueble];
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

-- ====================================================
-- ActualizarTextoComercialInmueble
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarTextoComercialInmueble];
GO

CREATE PROCEDURE [dbo].[ActualizarTextoComercialInmueble]
    @Id INT,
    @TextoComercial NVARCHAR(2000)
AS BEGIN
    UPDATE Inmueble SET texto_comercial = @TextoComercial WHERE id_inmueble = @Id;
END
GO

-- ====================================================
-- CambiarPublicacionInmueble
-- Publicar: Tasado -> Publicado (registra la fecha). Despublicar: Publicado -> Tasado.
-- Solo actúa si el inmueble está en el estado esperado: devuelve 1 fila afectada o 0.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CambiarPublicacionInmueble];
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

-- ####################################################
-- PN2 - Seguimiento de alquileres
-- ####################################################

-- ====================================================
-- CrearInquilino
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearInquilino];
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
DROP PROCEDURE IF EXISTS [dbo].[ActualizarInquilino];
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
DROP PROCEDURE IF EXISTS [dbo].[EliminarInquilino];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerInquilinos];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarInquilinoPorId];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarInquilinoPorDni];
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
-- CrearInmuebleAlquiler (@IdInmueble: inmueble del PN1 que representa; NULL en los registros anteriores al vínculo)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearInmuebleAlquiler];
GO

CREATE PROCEDURE [dbo].[CrearInmuebleAlquiler]
    @Direccion NVARCHAR(200),
    @TipoInmueble VARCHAR(50),
    @IdInmueble INT = NULL,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO InmuebleAlquiler (direccion, tipo_inmueble, id_inmueble)
    VALUES (@Direccion, @TipoInmueble, @IdInmueble);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ObtenerInmuebleAlquilerDeInmueble
-- Registro de alquiler (PN2) de un inmueble del PN1: devuelve el existente o lo crea con la dirección y el
-- tipo del inmueble. @IdInmuebleAlquiler queda NULL si el inmueble no existe.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ObtenerInmuebleAlquilerDeInmueble];
GO

CREATE PROCEDURE [dbo].[ObtenerInmuebleAlquilerDeInmueble]
    @IdInmueble INT,
    @IdInmuebleAlquiler INT OUTPUT
AS BEGIN
    SET @IdInmuebleAlquiler = (SELECT id_inmueble_alquiler FROM InmuebleAlquiler WHERE id_inmueble = @IdInmueble);
    IF @IdInmuebleAlquiler IS NULL AND EXISTS (SELECT 1 FROM Inmueble WHERE id_inmueble = @IdInmueble)
    BEGIN
        INSERT INTO InmuebleAlquiler (direccion, tipo_inmueble, id_inmueble)
        SELECT direccion, tipo_inmueble, id_inmueble FROM Inmueble WHERE id_inmueble = @IdInmueble;
        SET @IdInmuebleAlquiler = SCOPE_IDENTITY();
    END
END
GO

-- ====================================================
-- ActualizarInmuebleAlquiler
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarInmuebleAlquiler];
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
DROP PROCEDURE IF EXISTS [dbo].[EliminarInmuebleAlquiler];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerInmueblesAlquiler];
GO

CREATE PROCEDURE [dbo].[LeerInmueblesAlquiler]
AS BEGIN
    SELECT id_inmueble_alquiler, direccion, tipo_inmueble, id_inmueble
    FROM InmuebleAlquiler;
END
GO

-- ====================================================
-- BuscarInmuebleAlquilerPorId
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarInmuebleAlquilerPorId];
GO

CREATE PROCEDURE [dbo].[BuscarInmuebleAlquilerPorId]
    @Id INT
AS BEGIN
    SELECT id_inmueble_alquiler, direccion, tipo_inmueble, id_inmueble
    FROM InmuebleAlquiler
    WHERE id_inmueble_alquiler = @Id;
END
GO

-- ====================================================
-- BuscarInmuebleAlquilerPorInmueble (registro de alquiler de un inmueble del PN1)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarInmuebleAlquilerPorInmueble];
GO

CREATE PROCEDURE [dbo].[BuscarInmuebleAlquilerPorInmueble]
    @IdInmueble INT
AS BEGIN
    SELECT id_inmueble_alquiler, direccion, tipo_inmueble, id_inmueble
    FROM InmuebleAlquiler
    WHERE id_inmueble = @IdInmueble;
END
GO

-- ====================================================
-- LeerInmueblesDisponiblesParaAlquiler
-- Inmuebles del PN1 de alquiler que están Publicados y sin contrato vigente: los que se pueden alquilar
-- (Registrar Contrato)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerInmueblesDisponiblesParaAlquiler];
GO

CREATE PROCEDURE [dbo].[LeerInmueblesDisponiblesParaAlquiler]
AS BEGIN
    SELECT n.id_inmueble, n.tipo_inmueble, n.direccion, n.metros_cuadrados, n.cantidad_ambientes, n.estado_actual,
           n.tipo_publicacion, n.texto_comercial, n.fecha_publicacion, n.fecha_despublicacion, n.id_cliente
    FROM Inmueble n
    WHERE n.tipo_publicacion = 'Alquiler' AND n.estado_actual = 'Publicado'
      AND NOT EXISTS (
          SELECT 1 FROM InmuebleAlquiler i
          INNER JOIN Contrato c ON c.id_inmueble_alquiler = i.id_inmueble_alquiler
          WHERE i.id_inmueble = n.id_inmueble AND c.estado_contrato <> 'Finalizado')
    ORDER BY n.direccion;
END
GO

-- ====================================================
-- LeerInmueblesAlquilerDisponibles (registros de alquiler sin contrato vigente)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerInmueblesAlquilerDisponibles];
GO

CREATE PROCEDURE [dbo].[LeerInmueblesAlquilerDisponibles]
AS BEGIN
    SELECT i.id_inmueble_alquiler, i.direccion, i.tipo_inmueble, i.id_inmueble
    FROM InmuebleAlquiler i
    WHERE NOT EXISTS (
        SELECT 1 FROM Contrato c
        WHERE c.id_inmueble_alquiler = i.id_inmueble_alquiler AND c.estado_contrato <> 'Finalizado'
    );
END
GO

-- ====================================================
-- CrearContrato
-- Siempre crea en estado Activo y sin alerta. Si el inmueble viene del PN1 (InmuebleAlquiler.id_inmueble),
-- ese inmueble pasa de Publicado a Alquilado.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearContrato];
GO

CREATE PROCEDURE [dbo].[CrearContrato]
    @FechaInicio DATE,
    @DuracionMeses INT,
    @MontoMensual DECIMAL(18,2),
    @IdInquilino INT,
    @IdInmuebleAlquiler INT,
    @IdUsuarioAgente INT,
    @DepositoGarantia DECIMAL(18,2) = 0,
    @ComisionInmobiliaria DECIMAL(18,2) = 4, -- porcentaje retenido de cada pago
    @MetodoPago VARCHAR(20) = 'Transferencia',
    @IndiceActualizacion VARCHAR(10) = 'IPC',
    @PeriodicidadActualizacionMeses INT = 3,
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Contrato (fecha_inicio, duracion_meses, monto_mensual, estado_contrato, alerta_vencimiento,
                          id_inquilino, id_inmueble_alquiler, id_usuario_agente,
                          deposito_garantia, comision_inmobiliaria, metodo_pago,
                          indice_actualizacion, periodicidad_actualizacion_meses)
    VALUES (@FechaInicio, @DuracionMeses, @MontoMensual, 'Activo', 0,
            @IdInquilino, @IdInmuebleAlquiler, @IdUsuarioAgente,
            @DepositoGarantia, @ComisionInmobiliaria, @MetodoPago,
            @IndiceActualizacion, @PeriodicidadActualizacionMeses);
    SET @NuevoId = SCOPE_IDENTITY();

    UPDATE n SET estado_actual = 'Alquilado'
    FROM Inmueble n
    INNER JOIN InmuebleAlquiler i ON i.id_inmueble = n.id_inmueble
    WHERE i.id_inmueble_alquiler = @IdInmuebleAlquiler
      AND n.estado_actual = 'Publicado' AND n.tipo_publicacion = 'Alquiler';
END
GO

-- ====================================================
-- ActualizarContrato
-- @AvisoDesocupacion NULL conserva el aviso actual; 0 lo quita (renovación); 1 lo registra
-- (con @FechaAvisoDesocupacion, o la fecha actual si no se indica).
-- Las condiciones agregadas en el PN2 (depósito, comisión, método de pago, actualización) en NULL conservan
-- el valor actual.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarContrato];
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
    @FechaAvisoDesocupacion DATETIME = NULL,
    @DepositoGarantia DECIMAL(18,2) = NULL,
    @ComisionInmobiliaria DECIMAL(18,2) = NULL,
    @MetodoPago VARCHAR(20) = NULL,
    @IndiceActualizacion VARCHAR(10) = NULL,
    @PeriodicidadActualizacionMeses INT = NULL
AS BEGIN
    UPDATE Contrato
    SET fecha_inicio = @FechaInicio, duracion_meses = @DuracionMeses, monto_mensual = @MontoMensual,
        estado_contrato = @EstadoContrato, alerta_vencimiento = @AlertaVencimiento,
        id_inquilino = @IdInquilino, id_inmueble_alquiler = @IdInmuebleAlquiler, id_usuario_agente = @IdUsuarioAgente,
        deposito_garantia = ISNULL(@DepositoGarantia, deposito_garantia),
        comision_inmobiliaria = ISNULL(@ComisionInmobiliaria, comision_inmobiliaria),
        metodo_pago = ISNULL(@MetodoPago, metodo_pago),
        indice_actualizacion = ISNULL(@IndiceActualizacion, indice_actualizacion),
        periodicidad_actualizacion_meses = ISNULL(@PeriodicidadActualizacionMeses, periodicidad_actualizacion_meses),
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
DROP PROCEDURE IF EXISTS [dbo].[RegistrarAvisoDesocupacion];
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
-- ActualizarEstadoContrato
-- Al pasar a Finalizado, el inmueble del PN1 vinculado vuelve de Alquilado a Publicado.
-- @RescisionIniciadaPor / @PenalidadRescision: solo en una rescisión anticipada (NULL conserva lo guardado).
-- Las filas afectadas que se informan son solo las del contrato.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarEstadoContrato];
GO

CREATE PROCEDURE [dbo].[ActualizarEstadoContrato]
    @Id INT,
    @EstadoContrato VARCHAR(20),
    @AlertaVencimiento BIT,
    @RescisionIniciadaPor VARCHAR(20) = NULL,
    @PenalidadRescision DECIMAL(18,2) = NULL
AS BEGIN
    UPDATE Contrato
    SET estado_contrato = @EstadoContrato, alerta_vencimiento = @AlertaVencimiento,
        rescision_iniciada_por = ISNULL(@RescisionIniciadaPor, rescision_iniciada_por),
        penalidad_rescision = ISNULL(@PenalidadRescision, penalidad_rescision)
    WHERE id_contrato = @Id;

    IF @EstadoContrato = 'Finalizado'
    BEGIN
        SET NOCOUNT ON;
        UPDATE n SET estado_actual = 'Publicado'
        FROM Inmueble n
        INNER JOIN InmuebleAlquiler i ON i.id_inmueble = n.id_inmueble
        INNER JOIN Contrato c ON c.id_inmueble_alquiler = i.id_inmueble_alquiler
        WHERE c.id_contrato = @Id AND n.estado_actual = 'Alquilado';
        SET NOCOUNT OFF;
    END
END
GO

-- ====================================================
-- RegistrarEntregaContrato
-- Entrega del inmueble al finalizar: estado en que se recibió y destino del depósito (descuento + devolución
-- = depósito, lo controla CK_Contrato_DevolucionDeposito). Solo contratos finalizados: 0 filas si no corresponde.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[RegistrarEntregaContrato];
GO

CREATE PROCEDURE [dbo].[RegistrarEntregaContrato]
    @Id INT,
    @EstadoEntrega NVARCHAR(1000),
    @MontoDescuentoDeposito DECIMAL(18,2),
    @MontoDevolucionDeposito DECIMAL(18,2)
AS BEGIN
    UPDATE Contrato
    SET estado_entrega = @EstadoEntrega,
        monto_descuento_deposito = @MontoDescuentoDeposito,
        monto_devolucion_deposito = @MontoDevolucionDeposito
    WHERE id_contrato = @Id AND estado_contrato = 'Finalizado';
END
GO

-- ====================================================
-- EliminarContrato
-- Los garantes se borran antes (EliminarGarantesPorContrato, en la misma transacción). Si el contrato estaba
-- vigente, el inmueble del PN1 vuelve de Alquilado a Publicado. Su historial de actualizaciones se borra con él.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[EliminarContrato];
GO

CREATE PROCEDURE [dbo].[EliminarContrato]
    @Id INT
AS BEGIN
    SET NOCOUNT ON;
    UPDATE n SET estado_actual = 'Publicado'
    FROM Inmueble n
    INNER JOIN InmuebleAlquiler i ON i.id_inmueble = n.id_inmueble
    INNER JOIN Contrato c ON c.id_inmueble_alquiler = i.id_inmueble_alquiler
    WHERE c.id_contrato = @Id AND c.estado_contrato <> 'Finalizado' AND n.estado_actual = 'Alquilado';
    DELETE FROM ActualizacionContrato WHERE id_contrato = @Id;
    SET NOCOUNT OFF;

    DELETE FROM Contrato WHERE id_contrato = @Id;
END
GO

-- ====================================================
-- LeerContratos
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerContratos];
GO

CREATE PROCEDURE [dbo].[LeerContratos]
AS BEGIN
    SELECT id_contrato, fecha_inicio, duracion_meses, monto_mensual, estado_contrato, alerta_vencimiento,
           aviso_desocupacion, fecha_aviso_desocupacion,
           id_inquilino, id_inmueble_alquiler, id_usuario_agente, fecha_fin,
           deposito_garantia, comision_inmobiliaria, metodo_pago, indice_actualizacion, periodicidad_actualizacion_meses,
           rescision_iniciada_por, penalidad_rescision, estado_entrega, monto_descuento_deposito, monto_devolucion_deposito
    FROM Contrato
    ORDER BY fecha_inicio DESC;
END
GO

-- ====================================================
-- BuscarContratoPorId
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarContratoPorId];
GO

CREATE PROCEDURE [dbo].[BuscarContratoPorId]
    @Id INT
AS BEGIN
    SELECT id_contrato, fecha_inicio, duracion_meses, monto_mensual, estado_contrato, alerta_vencimiento,
           aviso_desocupacion, fecha_aviso_desocupacion,
           id_inquilino, id_inmueble_alquiler, id_usuario_agente, fecha_fin,
           deposito_garantia, comision_inmobiliaria, metodo_pago, indice_actualizacion, periodicidad_actualizacion_meses,
           rescision_iniciada_por, penalidad_rescision, estado_entrega, monto_descuento_deposito, monto_devolucion_deposito
    FROM Contrato
    WHERE id_contrato = @Id;
END
GO

-- ====================================================
-- LeerContratosDetalle (para grillas: contrato + inquilino + inmueble; @Estado NULL = todos)
-- La dirección es la del inmueble del PN1 cuando está vinculado (la que se mantiene actualizada).
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerContratosDetalle];
GO

CREATE PROCEDURE [dbo].[LeerContratosDetalle]
    @Estado VARCHAR(20) = NULL
AS BEGIN
    SELECT c.id_contrato, c.fecha_inicio, c.duracion_meses, c.monto_mensual, c.estado_contrato, c.alerta_vencimiento,
           c.aviso_desocupacion, c.fecha_aviso_desocupacion, c.fecha_fin,
           c.deposito_garantia, c.comision_inmobiliaria, c.metodo_pago, c.indice_actualizacion, c.periodicidad_actualizacion_meses,
           c.rescision_iniciada_por, c.penalidad_rescision, c.estado_entrega, c.monto_descuento_deposito, c.monto_devolucion_deposito,
           c.id_inquilino, q.nombre + ' ' + q.apellido AS inquilino, q.dni AS dni_inquilino,
           c.id_inmueble_alquiler, ISNULL(n.direccion, i.direccion) AS direccion, i.tipo_inmueble, i.id_inmueble,
           c.id_usuario_agente
    FROM Contrato c
    INNER JOIN Inquilino q ON q.id_inquilino = c.id_inquilino
    INNER JOIN InmuebleAlquiler i ON i.id_inmueble_alquiler = c.id_inmueble_alquiler
    LEFT JOIN Inmueble n ON n.id_inmueble = i.id_inmueble
    WHERE @Estado IS NULL OR c.estado_contrato = @Estado
    ORDER BY c.fecha_inicio DESC;
END
GO

-- ====================================================
-- LeerContratosPorVencer (vigentes que terminan dentro de @DiasAnticipacion días)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerContratosPorVencer];
GO

CREATE PROCEDURE [dbo].[LeerContratosPorVencer]
    @DiasAnticipacion INT = 60
AS BEGIN
    SELECT c.id_contrato, c.fecha_inicio, c.duracion_meses, c.monto_mensual, c.estado_contrato, c.alerta_vencimiento,
           c.aviso_desocupacion, c.fecha_aviso_desocupacion, c.fecha_fin,
           DATEDIFF(DAY, CAST(GETDATE() AS DATE), c.fecha_fin) AS dias_restantes,
           c.deposito_garantia, c.comision_inmobiliaria, c.metodo_pago, c.indice_actualizacion, c.periodicidad_actualizacion_meses,
           c.rescision_iniciada_por, c.penalidad_rescision, c.estado_entrega, c.monto_descuento_deposito, c.monto_devolucion_deposito,
           c.id_inquilino, q.nombre + ' ' + q.apellido AS inquilino, q.dni AS dni_inquilino, q.telefono, q.email,
           c.id_inmueble_alquiler, ISNULL(n.direccion, i.direccion) AS direccion, i.tipo_inmueble, i.id_inmueble,
           c.id_usuario_agente
    FROM Contrato c
    INNER JOIN Inquilino q ON q.id_inquilino = c.id_inquilino
    INNER JOIN InmuebleAlquiler i ON i.id_inmueble_alquiler = c.id_inmueble_alquiler
    LEFT JOIN Inmueble n ON n.id_inmueble = i.id_inmueble
    WHERE c.estado_contrato <> 'Finalizado'
      AND c.fecha_fin <= DATEADD(DAY, @DiasAnticipacion, CAST(GETDATE() AS DATE))
    ORDER BY c.fecha_fin;
END
GO

-- ====================================================
-- ActualizarVencimientoContratos
-- Activo -> Por Vencer (con alerta) si termina dentro de @DiasAnticipacion días;
-- vigentes con fecha de fin pasada -> Finalizado (sin alerta), y su inmueble del PN1 vuelve a Publicado.
-- Las filas afectadas que se informan son solo las de los contratos.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarVencimientoContratos];
GO

CREATE PROCEDURE [dbo].[ActualizarVencimientoContratos]
    @DiasAnticipacion INT = 60
AS BEGIN
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
    DECLARE @Finalizados TABLE (id_inmueble_alquiler INT);

    UPDATE Contrato
    SET estado_contrato = 'Finalizado', alerta_vencimiento = 0
    OUTPUT inserted.id_inmueble_alquiler INTO @Finalizados
    WHERE estado_contrato <> 'Finalizado'
      AND fecha_fin < @Hoy;

    UPDATE Contrato
    SET estado_contrato = 'Por Vencer', alerta_vencimiento = 1
    WHERE estado_contrato = 'Activo'
      AND fecha_fin <= DATEADD(DAY, @DiasAnticipacion, @Hoy);

    SET NOCOUNT ON;
    UPDATE n SET estado_actual = 'Publicado'
    FROM Inmueble n
    INNER JOIN InmuebleAlquiler i ON i.id_inmueble = n.id_inmueble
    WHERE i.id_inmueble_alquiler IN (SELECT id_inmueble_alquiler FROM @Finalizados) AND n.estado_actual = 'Alquilado';
    SET NOCOUNT OFF;
END
GO

-- ====================================================
-- Garante (garantes de cada contrato)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearGarante];
GO

CREATE PROCEDURE [dbo].[CrearGarante]
    @IdContrato INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni VARCHAR(20),
    @Domicilio NVARCHAR(200),
    @PropiedadGarantia NVARCHAR(200),
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO Garante (id_contrato, nombre, apellido, dni, domicilio, propiedad_garantia)
    VALUES (@IdContrato, @Nombre, @Apellido, @Dni, @Domicilio, @PropiedadGarantia);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

DROP PROCEDURE IF EXISTS [dbo].[ActualizarGarante];
GO

CREATE PROCEDURE [dbo].[ActualizarGarante]
    @Id INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Dni VARCHAR(20),
    @Domicilio NVARCHAR(200),
    @PropiedadGarantia NVARCHAR(200)
AS BEGIN
    UPDATE Garante
    SET nombre = @Nombre, apellido = @Apellido, dni = @Dni, domicilio = @Domicilio, propiedad_garantia = @PropiedadGarantia
    WHERE id_garante = @Id;
END
GO

DROP PROCEDURE IF EXISTS [dbo].[EliminarGarante];
GO

CREATE PROCEDURE [dbo].[EliminarGarante]
    @Id INT
AS BEGIN
    DELETE FROM Garante WHERE id_garante = @Id;
END
GO

DROP PROCEDURE IF EXISTS [dbo].[EliminarGarantesPorContrato];
GO

CREATE PROCEDURE [dbo].[EliminarGarantesPorContrato]
    @IdContrato INT
AS BEGIN
    DELETE FROM Garante WHERE id_contrato = @IdContrato;
END
GO

DROP PROCEDURE IF EXISTS [dbo].[LeerGarantesPorContrato];
GO

CREATE PROCEDURE [dbo].[LeerGarantesPorContrato]
    @IdContrato INT
AS BEGIN
    SELECT id_garante, id_contrato, nombre, apellido, dni, domicilio, propiedad_garantia
    FROM Garante
    WHERE id_contrato = @IdContrato
    ORDER BY apellido, nombre;
END
GO

DROP PROCEDURE IF EXISTS [dbo].[BuscarGarantePorId];
GO

CREATE PROCEDURE [dbo].[BuscarGarantePorId]
    @Id INT
AS BEGIN
    SELECT id_garante, id_contrato, nombre, apellido, dni, domicilio, propiedad_garantia
    FROM Garante
    WHERE id_garante = @Id;
END
GO

-- ====================================================
-- AplicarActualizacionContrato
-- Nuevo monto mensual por índice + su registro en el historial, juntos o ninguno. Solo si el contrato sigue
-- vigente con el monto esperado (@MontoAnterior) y esa actualización no estaba aplicada: @Aplicado = 0 si no.
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[AplicarActualizacionContrato];
GO

CREATE PROCEDURE [dbo].[AplicarActualizacionContrato]
    @IdContrato INT,
    @FechaActualizacion DATE,
    @Indice VARCHAR(10),
    @Porcentaje DECIMAL(9,4),
    @Detalle NVARCHAR(200),
    @MontoAnterior DECIMAL(18,2),
    @MontoNuevo DECIMAL(18,2),
    @IdUsuario INT = NULL,
    @Aplicado BIT OUTPUT
AS BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    SET @Aplicado = 0;
    IF EXISTS (SELECT 1 FROM ActualizacionContrato WHERE id_contrato = @IdContrato AND fecha_actualizacion = @FechaActualizacion)
        RETURN;

    BEGIN TRANSACTION;
    UPDATE Contrato SET monto_mensual = @MontoNuevo
    WHERE id_contrato = @IdContrato AND monto_mensual = @MontoAnterior AND estado_contrato <> 'Finalizado';
    IF @@ROWCOUNT = 0
    BEGIN
        ROLLBACK TRANSACTION;
        RETURN;
    END
    INSERT INTO ActualizacionContrato (id_contrato, fecha_actualizacion, indice, porcentaje, detalle, monto_anterior, monto_nuevo, id_usuario)
    VALUES (@IdContrato, @FechaActualizacion, @Indice, @Porcentaje, @Detalle, @MontoAnterior, @MontoNuevo, @IdUsuario);
    COMMIT TRANSACTION;
    SET @Aplicado = 1;
END
GO

-- ====================================================
-- LeerActualizacionesPorContrato (historial, de la más antigua a la más reciente)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerActualizacionesPorContrato];
GO

CREATE PROCEDURE [dbo].[LeerActualizacionesPorContrato]
    @IdContrato INT
AS BEGIN
    SELECT id_actualizacion, id_contrato, fecha_actualizacion, fecha_registro, indice, porcentaje, detalle,
           monto_anterior, monto_nuevo, id_usuario
    FROM ActualizacionContrato
    WHERE id_contrato = @IdContrato
    ORDER BY fecha_actualizacion;
END
GO

-- ====================================================
-- GuardarIndiceBCRA (alta o reemplazo de un valor de una serie del BCRA)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[GuardarIndiceBCRA];
GO

CREATE PROCEDURE [dbo].[GuardarIndiceBCRA]
    @IdVariable INT,
    @Fecha DATE,
    @Valor DECIMAL(19,6)
AS BEGIN
    UPDATE IndiceBCRA SET valor = @Valor, fecha_consulta = GETDATE()
    WHERE id_variable = @IdVariable AND fecha = @Fecha;
    IF @@ROWCOUNT = 0
        INSERT INTO IndiceBCRA (id_variable, fecha, valor) VALUES (@IdVariable, @Fecha, @Valor);
END
GO

-- ====================================================
-- LeerIndiceBCRA (valores guardados de una serie entre dos fechas)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerIndiceBCRA];
GO

CREATE PROCEDURE [dbo].[LeerIndiceBCRA]
    @IdVariable INT,
    @Desde DATE,
    @Hasta DATE
AS BEGIN
    SELECT id_variable, fecha, valor
    FROM IndiceBCRA
    WHERE id_variable = @IdVariable AND fecha BETWEEN @Desde AND @Hasta
    ORDER BY fecha;
END
GO

-- ====================================================
-- CrearPagoAlquiler
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearPagoAlquiler];
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
    @ComisionInmobiliaria DECIMAL(18,2) = 0, -- monto retenido de este pago
    @NuevoId INT OUTPUT
AS BEGIN
    INSERT INTO PagoAlquiler (mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
                              recargo_aplicado, id_contrato, id_usuario_analista, comision_inmobiliaria)
    VALUES (@MesCorrespondiente, @AnioCorrespondiente, @FechaPago, @MontoPagado, @EstadoPago,
            @RecargoAplicado, @IdContrato, @IdUsuarioAnalista, @ComisionInmobiliaria);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================
-- ActualizarPagoAlquiler
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarPagoAlquiler];
GO

CREATE PROCEDURE [dbo].[ActualizarPagoAlquiler]
    @Id INT,
    @MesCorrespondiente INT,
    @AnioCorrespondiente INT,
    @FechaPago DATETIME = NULL,
    @MontoPagado DECIMAL(18,2),
    @EstadoPago NVARCHAR(20),
    @RecargoAplicado DECIMAL(18,2),
    @IdUsuarioAnalista INT,
    @ComisionInmobiliaria DECIMAL(18,2) = NULL -- NULL conserva la guardada
AS BEGIN
    UPDATE PagoAlquiler
    SET mes_correspondiente = @MesCorrespondiente, anio_correspondiente = @AnioCorrespondiente,
        fecha_pago = @FechaPago, monto_pagado = @MontoPagado, estado_pago = @EstadoPago,
        recargo_aplicado = @RecargoAplicado, id_usuario_analista = @IdUsuarioAnalista,
        comision_inmobiliaria = ISNULL(@ComisionInmobiliaria, comision_inmobiliaria)
    WHERE id_pago = @Id;
END
GO

-- ====================================================
-- ActualizarEstadoPago (Gestionar Mora / Litigios: cambia el estado y el recargo)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarEstadoPago];
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
DROP PROCEDURE IF EXISTS [dbo].[EliminarPagoAlquiler];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerPagosAlquiler];
GO

CREATE PROCEDURE [dbo].[LeerPagosAlquiler]
AS BEGIN
    SELECT id_pago, mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
           recargo_aplicado, id_contrato, id_usuario_analista, comision_inmobiliaria
    FROM PagoAlquiler
    ORDER BY anio_correspondiente DESC, mes_correspondiente DESC;
END
GO

-- ====================================================
-- BuscarPagoAlquilerPorId
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarPagoAlquilerPorId];
GO

CREATE PROCEDURE [dbo].[BuscarPagoAlquilerPorId]
    @Id INT
AS BEGIN
    SELECT id_pago, mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
           recargo_aplicado, id_contrato, id_usuario_analista, comision_inmobiliaria
    FROM PagoAlquiler
    WHERE id_pago = @Id;
END
GO

-- ====================================================
-- LeerPagosPorContrato (historial de pagos de un contrato)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerPagosPorContrato];
GO

CREATE PROCEDURE [dbo].[LeerPagosPorContrato]
    @IdContrato INT
AS BEGIN
    SELECT id_pago, mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
           recargo_aplicado, id_contrato, id_usuario_analista, comision_inmobiliaria
    FROM PagoAlquiler
    WHERE id_contrato = @IdContrato
    ORDER BY anio_correspondiente DESC, mes_correspondiente DESC;
END
GO

-- ====================================================
-- BuscarPagoPorPeriodo (para Registrar Pago: si el mes ya tiene registro, se actualiza)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[BuscarPagoPorPeriodo];
GO

CREATE PROCEDURE [dbo].[BuscarPagoPorPeriodo]
    @IdContrato INT,
    @MesCorrespondiente INT,
    @AnioCorrespondiente INT
AS BEGIN
    SELECT id_pago, mes_correspondiente, anio_correspondiente, fecha_pago, monto_pagado, estado_pago,
           recargo_aplicado, id_contrato, id_usuario_analista, comision_inmobiliaria
    FROM PagoAlquiler
    WHERE id_contrato = @IdContrato
      AND mes_correspondiente = @MesCorrespondiente
      AND anio_correspondiente = @AnioCorrespondiente;
END
GO

-- ====================================================
-- LeerPagosDetalle (Gestionar Mora / Litigios: pago + contrato + inquilino + inmueble; @Estado NULL = todos)
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[LeerPagosDetalle];
GO

CREATE PROCEDURE [dbo].[LeerPagosDetalle]
    @EstadoPago NVARCHAR(20) = NULL
AS BEGIN
    SELECT p.id_pago, p.mes_correspondiente, p.anio_correspondiente, p.fecha_pago, p.monto_pagado, p.estado_pago,
           p.recargo_aplicado, p.id_contrato, p.id_usuario_analista, p.comision_inmobiliaria,
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
-- CrearAccionLegal
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearAccionLegal];
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

-- ====================================================
-- ActualizarAccionLegal
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[ActualizarAccionLegal];
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
DROP PROCEDURE IF EXISTS [dbo].[EliminarAccionLegal];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerAccionesLegales];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarAccionLegalPorId];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerAccionesPorContrato];
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

-- ====================================================
-- CrearContactoCobranza
-- ====================================================
DROP PROCEDURE IF EXISTS [dbo].[CrearContactoCobranza];
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
DROP PROCEDURE IF EXISTS [dbo].[ActualizarContactoCobranza];
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
DROP PROCEDURE IF EXISTS [dbo].[EliminarContactoCobranza];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerContactosCobranza];
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
DROP PROCEDURE IF EXISTS [dbo].[BuscarContactoCobranzaPorId];
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
DROP PROCEDURE IF EXISTS [dbo].[LeerContactosPorContrato];
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
