USE [TpIngSoft];
GO

-- ====================================================
-- BuscarUsuarioPorNombre
-- ====================================================
IF OBJECT_ID('[dbo].[BuscarUsuarioPorNombre]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[BuscarUsuarioPorNombre];
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
IF OBJECT_ID('[dbo].[BuscarUsuarioPorId]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[BuscarUsuarioPorId];
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
IF OBJECT_ID('[dbo].[LeerUsuarios]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[LeerUsuarios];
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
IF OBJECT_ID('[dbo].[CrearUsuario]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[CrearUsuario];
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
IF OBJECT_ID('[dbo].[ActualizarUsuario]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[ActualizarUsuario];
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
IF OBJECT_ID('[dbo].[ActualizarIntentosFallidos]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[ActualizarIntentosFallidos];
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
IF OBJECT_ID('[dbo].[LeerRolesPorUsuario]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[LeerRolesPorUsuario];
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
IF OBJECT_ID('[dbo].[LeerPermisosPorRol]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[LeerPermisosPorRol];
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
IF OBJECT_ID('[dbo].[CrearSesion]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[CrearSesion];
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
IF OBJECT_ID('[dbo].[CerrarSesion]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[CerrarSesion];
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
IF OBJECT_ID('[dbo].[LeerRoles]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[LeerRoles];
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
IF OBJECT_ID('[dbo].[AsignarRolUsuario]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[AsignarRolUsuario];
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
IF OBJECT_ID('[dbo].[ValidarYRefrescarSesion]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[ValidarYRefrescarSesion];
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
IF OBJECT_ID('[dbo].[InsertarBitacora]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[InsertarBitacora];
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
IF OBJECT_ID('[dbo].[BuscarBitacora]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[BuscarBitacora];
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
IF OBJECT_ID('[dbo].[InsertarUsuarioHistorial]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[InsertarUsuarioHistorial];
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
IF OBJECT_ID('[dbo].[LeerHistorialUsuario]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[LeerHistorialUsuario];
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
IF OBJECT_ID('[dbo].[BuscarHistorialPorId]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[BuscarHistorialPorId];
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
IF OBJECT_ID('[dbo].[LeerDVV]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[LeerDVV];
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
IF OBJECT_ID('[dbo].[ActualizarDVV]', 'P') IS NOT NULL 
    DROP PROCEDURE [dbo].[ActualizarDVV];
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
IF OBJECT_ID('[dbo].[AsignarPermisoARol]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[AsignarPermisoARol];
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
IF OBJECT_ID('[dbo].[RemoverPermisoDeRol]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[RemoverPermisoDeRol];
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
IF OBJECT_ID('[dbo].[AsignarSubRol]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[AsignarSubRol];
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
IF OBJECT_ID('[dbo].[RemoverSubRol]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[RemoverSubRol];
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
IF OBJECT_ID('[dbo].[LeerSubRolesPorRol]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerSubRolesPorRol];
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
IF OBJECT_ID('[dbo].[CrearRol]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearRol];
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
IF OBJECT_ID('[dbo].[ActualizarRol]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarRol];
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
IF OBJECT_ID('[dbo].[EliminarRol]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarRol];
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
IF OBJECT_ID('[dbo].[LeerRolPorId]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerRolPorId];
GO
CREATE PROCEDURE [dbo].[LeerRolPorId]
    @Id INT
AS BEGIN
    SELECT id, nombre FROM Rol WHERE id = @Id;
END
GO

-- ====================================================
-- ====================================================
-- LeerPermisos
-- ====================================================
IF OBJECT_ID('[dbo].[LeerPermisos]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerPermisos];
GO
CREATE PROCEDURE [dbo].[LeerPermisos]
AS BEGIN
    SELECT id, nombre FROM Permiso;
END
GO

-- ====================================================
-- LeerIdiomasActivos
-- ====================================================
IF OBJECT_ID('[dbo].[LeerIdiomasActivos]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerIdiomasActivos];
GO
CREATE PROCEDURE [dbo].[LeerIdiomasActivos]
AS BEGIN
    SELECT id, nombre, activo FROM Idioma WHERE activo = 1;
END
GO

-- ====================================================
-- LeerTodosIdiomas
-- ====================================================
IF OBJECT_ID('[dbo].[LeerTodosIdiomas]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerTodosIdiomas];
GO
CREATE PROCEDURE [dbo].[LeerTodosIdiomas]
AS BEGIN
    SELECT id, nombre, activo FROM Idioma;
END
GO

-- ====================================================
-- ToggleEstadoIdioma
-- ====================================================
IF OBJECT_ID('[dbo].[ToggleEstadoIdioma]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ToggleEstadoIdioma];
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
IF OBJECT_ID('[dbo].[LeerTraduccionesPorIdioma]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerTraduccionesPorIdioma];
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
IF OBJECT_ID('[dbo].[CrearIdioma]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearIdioma];
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
IF OBJECT_ID('[dbo].[ActualizarTraduccion]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ActualizarTraduccion];
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
IF OBJECT_ID('[dbo].[LeerControles]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerControles];
GO
CREATE PROCEDURE [dbo].[LeerControles]
AS BEGIN
    SELECT id, nombre, formulario FROM Control;
END
GO

-- ====================================================
-- CrearControl
-- ====================================================
IF OBJECT_ID('[dbo].[CrearControl]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[CrearControl];
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
IF OBJECT_ID('[dbo].[GuardarIdiomaUsuario]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[GuardarIdiomaUsuario];
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
IF OBJECT_ID('[dbo].[LeerIdiomaUsuario]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[LeerIdiomaUsuario];
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
IF OBJECT_ID('[dbo].[RemoverRolesUsuario]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[RemoverRolesUsuario];
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
IF OBJECT_ID('[dbo].[BloquearUsuarioManual]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[BloquearUsuarioManual];
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
IF OBJECT_ID('[dbo].[EliminarIdioma]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[EliminarIdioma];
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
IF OBJECT_ID('[dbo].[ReasignarUsuariosIdioma]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[ReasignarUsuariosIdioma];
GO
CREATE PROCEDURE [dbo].[ReasignarUsuariosIdioma]
    @IdIdiomaDesactivado INT,
    @IdIdiomaFallback INT
AS BEGIN
    UPDATE Usuario SET id_idioma = @IdIdiomaFallback WHERE id_idioma = @IdIdiomaDesactivado;
END
GO
