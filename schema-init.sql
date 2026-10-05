IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TpIngSoft')
BEGIN
    CREATE DATABASE [TpIngSoft];
END
GO

USE [TpIngSoft];
GO

-- ====================================================
-- 1. Creación de tablas
-- ====================================================

-- Tablas del PN2 (migracion-pn2-tablas.sql y migracion-pn2-contactos.sql): referencian a Usuario, se borran primero
IF OBJECT_ID('[dbo].[ContactoCobranza]', 'U') IS NOT NULL DROP TABLE [dbo].[ContactoCobranza];
IF OBJECT_ID('[dbo].[AccionLegal]', 'U') IS NOT NULL DROP TABLE [dbo].[AccionLegal];
IF OBJECT_ID('[dbo].[PagoAlquiler]', 'U') IS NOT NULL DROP TABLE [dbo].[PagoAlquiler];
IF OBJECT_ID('[dbo].[Contrato]', 'U') IS NOT NULL DROP TABLE [dbo].[Contrato];
IF OBJECT_ID('[dbo].[InmuebleAlquiler]', 'U') IS NOT NULL DROP TABLE [dbo].[InmuebleAlquiler];
IF OBJECT_ID('[dbo].[Inquilino]', 'U') IS NOT NULL DROP TABLE [dbo].[Inquilino];

-- Tablas del PN1 (migracion-pn1-tablas.sql): referencian a Usuario, se borran primero
IF OBJECT_ID('[dbo].[MaterialAudiovisual]', 'U') IS NOT NULL DROP TABLE [dbo].[MaterialAudiovisual];
IF OBJECT_ID('[dbo].[Tasacion]', 'U') IS NOT NULL DROP TABLE [dbo].[Tasacion];
IF OBJECT_ID('[dbo].[Captacion]', 'U') IS NOT NULL DROP TABLE [dbo].[Captacion];
IF OBJECT_ID('[dbo].[Inmueble]', 'U') IS NOT NULL DROP TABLE [dbo].[Inmueble];
IF OBJECT_ID('[dbo].[Cliente]', 'U') IS NOT NULL DROP TABLE [dbo].[Cliente];
IF OBJECT_ID('[dbo].[Traducciones]', 'U') IS NOT NULL DROP TABLE [dbo].[Traducciones];
IF OBJECT_ID('[dbo].[Control]', 'U') IS NOT NULL DROP TABLE [dbo].[Control];
IF OBJECT_ID('[dbo].[UsuarioRol]', 'U') IS NOT NULL DROP TABLE [dbo].[UsuarioRol];
IF OBJECT_ID('[dbo].[RolPermiso]', 'U') IS NOT NULL DROP TABLE [dbo].[RolPermiso];
IF OBJECT_ID('[dbo].[Sesion]', 'U') IS NOT NULL DROP TABLE [dbo].[Sesion];
IF OBJECT_ID('[dbo].[Bitacora]', 'U') IS NOT NULL DROP TABLE [dbo].[Bitacora];
IF OBJECT_ID('[dbo].[Usuario_Historial]', 'U') IS NOT NULL DROP TABLE [dbo].[Usuario_Historial];
IF OBJECT_ID('[dbo].[DigitoVerificadorVertical]', 'U') IS NOT NULL DROP TABLE [dbo].[DigitoVerificadorVertical];
IF OBJECT_ID('[dbo].[Usuario]', 'U') IS NOT NULL DROP TABLE [dbo].[Usuario];
IF OBJECT_ID('[dbo].[Idioma]', 'U') IS NOT NULL DROP TABLE [dbo].[Idioma];
IF OBJECT_ID('[dbo].[Rol]', 'U') IS NOT NULL DROP TABLE [dbo].[Rol];
IF OBJECT_ID('[dbo].[Permiso]', 'U') IS NOT NULL DROP TABLE [dbo].[Permiso];
GO

CREATE TABLE [dbo].[Idioma] (
    [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [nombre] NVARCHAR(50) NOT NULL,
    [activo] BIT NOT NULL DEFAULT 1
);

CREATE TABLE [dbo].[Usuario] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [nombre] VARCHAR(50) NOT NULL UNIQUE,
    [nombre_persona] NVARCHAR(100) NULL,
    [apellido] NVARCHAR(100) NULL,
    [pass] VARCHAR(100) NOT NULL,
    [activo] BIT DEFAULT 1,
    [intentos_fallidos] INT DEFAULT 0,
    [bloqueado_hasta] DATETIME NULL,
    [dvh] BIGINT DEFAULT 0,
    [id_idioma] INT NULL FOREIGN KEY REFERENCES [dbo].[Idioma]([id])
);

CREATE TABLE [dbo].[Rol] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [nombre] VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE [dbo].[Permiso] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [nombre] VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE [dbo].[UsuarioRol] (
    [id_usuario] INT NOT NULL,
    [id_rol] INT NOT NULL,
    PRIMARY KEY ([id_usuario], [id_rol]),
    FOREIGN KEY ([id_usuario]) REFERENCES [dbo].[Usuario]([id]),
    FOREIGN KEY ([id_rol]) REFERENCES [dbo].[Rol]([id])
);

CREATE TABLE [dbo].[RolPermiso] (
    [id_padre]   INT         NOT NULL REFERENCES [dbo].[Rol]([id]),
    [id_hijo]    INT         NOT NULL,        -- FK a Rol.id o Permiso.id según tipo_hijo
    [tipo_hijo]  VARCHAR(10) NOT NULL CHECK ([tipo_hijo] IN ('Rol', 'Permiso')),
    PRIMARY KEY ([id_padre], [id_hijo], [tipo_hijo]),
    CHECK ([id_padre] <> [id_hijo] OR [tipo_hijo] = 'Permiso')
);

CREATE TABLE [dbo].[Sesion] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [id_usuario] INT NOT NULL,
    [fecha_login] DATETIME DEFAULT GETDATE(),
    [fecha_logout] DATETIME NULL,
    FOREIGN KEY ([id_usuario]) REFERENCES [dbo].[Usuario]([id])
);

CREATE TABLE [dbo].[Bitacora] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_hora] DATETIME DEFAULT GETDATE(),
    [id_usuario] INT NULL,
    [actividad] VARCHAR(100) NOT NULL,
    [info_asociada] VARCHAR(500) NULL,
    FOREIGN KEY ([id_usuario]) REFERENCES [dbo].[Usuario]([id])
);

CREATE TABLE [dbo].[Usuario_Historial] (
    [id_historial] INT IDENTITY(1,1) PRIMARY KEY,
    [id_usuario] INT NOT NULL,
    [nombre] VARCHAR(50) NOT NULL,
    [nombre_persona] NVARCHAR(100) NULL,
    [apellido] NVARCHAR(100) NULL,
    [pass] VARCHAR(100) NOT NULL,
    [activo] BIT NOT NULL,
    [dvh] BIGINT NULL,
    [fecha_cambio] DATETIME DEFAULT GETDATE(),
    [id_usuario_autor] INT NOT NULL,
    [tipo_operacion] VARCHAR(20) NOT NULL
);

CREATE TABLE [dbo].[DigitoVerificadorVertical] (
    [tabla] VARCHAR(50) PRIMARY KEY,
    [dvv] BIGINT NOT NULL
);

CREATE TABLE [dbo].[Control] (
    [id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [nombre] NVARCHAR(100) NOT NULL,
    [formulario] NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Control_Nombre_Formulario UNIQUE ([nombre], [formulario])
);

CREATE TABLE [dbo].[Traducciones] (
    [idcontrol] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Control]([id]),
    [ididioma] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Idioma]([id]),
    [texto] NVARCHAR(500) NULL,
    PRIMARY KEY ([idcontrol], [ididioma])
);
GO

-- ====================================================
-- 2. Seeds (Datos Iniciales)
-- ====================================================

-- Idiomas por defecto
INSERT INTO [dbo].[Idioma] ([nombre], [activo]) VALUES ('Español', 1);
INSERT INTO [dbo].[Idioma] ([nombre], [activo]) VALUES ('English', 1);

-- Roles
INSERT INTO [dbo].[Rol] ([nombre]) VALUES ('Admin');
INSERT INTO [dbo].[Rol] ([nombre]) VALUES ('User');

-- Permisos
INSERT INTO [dbo].[Permiso] ([nombre]) VALUES ('AccesoAdmin');
INSERT INTO [dbo].[Permiso] ([nombre]) VALUES ('GestionUsuarios');

-- Mapeo Rol-Permiso
INSERT INTO [dbo].[RolPermiso] ([id_padre], [id_hijo], [tipo_hijo]) VALUES (1, 1, 'Permiso'); -- Admin - AccesoAdmin
INSERT INTO [dbo].[RolPermiso] ([id_padre], [id_hijo], [tipo_hijo]) VALUES (1, 2, 'Permiso'); -- Admin - GestionUsuarios

-- Usuario Admin por defecto (apunta al Idioma Español = 1)
DECLARE @AdminId INT;
INSERT INTO [dbo].[Usuario] ([nombre], [nombre_persona], [apellido], [pass], [activo], [id_idioma]) VALUES ('admin', 'Administrador', 'Sistema', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, 1);
SET @AdminId = SCOPE_IDENTITY();
INSERT INTO [dbo].[UsuarioRol] ([id_usuario], [id_rol]) VALUES (@AdminId, 1);

-- Historial para Admin (tipo 'INSERT')
INSERT INTO [dbo].[Usuario_Historial] ([id_usuario], [nombre], [nombre_persona], [apellido], [pass], [activo], [dvh], [id_usuario_autor], [tipo_operacion])
VALUES (@AdminId, 'admin', 'Administrador', 'Sistema', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, 0, @AdminId, 'INSERT');

-- Usuario Normal por defecto (apunta al Idioma Español = 1)
DECLARE @UserId INT;
INSERT INTO [dbo].[Usuario] ([nombre], [nombre_persona], [apellido], [pass], [activo], [id_idioma]) VALUES ('user', 'Usuario', 'Normal', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, 1);
SET @UserId = SCOPE_IDENTITY();
INSERT INTO [dbo].[UsuarioRol] ([id_usuario], [id_rol]) VALUES (@UserId, 2);

-- Historial para User (tipo 'INSERT')
INSERT INTO [dbo].[Usuario_Historial] ([id_usuario], [nombre], [nombre_persona], [apellido], [pass], [activo], [dvh], [id_usuario_autor], [tipo_operacion])
VALUES (@UserId, 'user', 'Usuario', 'Normal', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, 0, @AdminId, 'INSERT');

-- ====================================================
-- 3. Seeds de Controles y Traducciones (Español e Inglés)
-- ====================================================
DECLARE @CtrlId INT;

-- FormLogin
INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormLogin');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Acceso al Sistema'), (@CtrlId, 2, 'System Access');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('label1', 'FormLogin');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Bienvenido'), (@CtrlId, 2, 'Welcome');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('label2', 'FormLogin');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Nombre'), (@CtrlId, 2, 'Username');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('label3', 'FormLogin');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Contraseña'), (@CtrlId, 2, 'Password');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('buttonLogin', 'FormLogin');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Login'), (@CtrlId, 2, 'Login');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('labelIdioma', 'FormLogin');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Idioma:'), (@CtrlId, 2, 'Language:');

-- FormMain
INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'TpIngSoft - Sistema de Gestión'), (@CtrlId, 2, 'TpIngSoft - Management System');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('archivoToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Archivo'), (@CtrlId, 2, 'File');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('logoutToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Cerrar Sesión'), (@CtrlId, 2, 'Logout');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('salirToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Salir'), (@CtrlId, 2, 'Exit');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('adminToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Admin'), (@CtrlId, 2, 'Admin');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('gestionUsuariosToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Gestión de Usuarios'), (@CtrlId, 2, 'User Management');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('bitacoraToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Bitácora'), (@CtrlId, 2, 'Event Log');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('controlCambiosToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Control de Cambios'), (@CtrlId, 2, 'Change Control');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('gestionPerfilesToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Gestión de Roles'), (@CtrlId, 2, 'Role Management');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('gestionIdiomasToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Gestión de Idiomas'), (@CtrlId, 2, 'Language Management');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('verificarIntegridadToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Verificar Integridad de Datos'), (@CtrlId, 2, 'Verify Data Integrity');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('lblSesionInfo', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Usuario:'), (@CtrlId, 2, 'User:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('pn1ToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'PN1'), (@CtrlId, 2, 'PN1');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('registrarCaptacionToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Registrar Captación'), (@CtrlId, 2, 'Register Listing');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('gestionarCaptacionesToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Gestionar Captaciones'), (@CtrlId, 2, 'Manage Listings');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('validarTasacionToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Validar Tasación'), (@CtrlId, 2, 'Validate Appraisal');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('cargarMaterialAudiovisualToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Cargar Material Audiovisual'), (@CtrlId, 2, 'Upload Audiovisual Material');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('publicarInmuebleToolStripMenuItem', 'FormMain');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Publicar Inmueble'), (@CtrlId, 2, 'Publish Property');

-- FormGestionUsuarios
INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Gestión de Usuarios'), (@CtrlId, 2, 'User Management');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('label1', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Usuario'), (@CtrlId, 2, 'Username');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('label2', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Contraseña'), (@CtrlId, 2, 'Password');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('lblRol', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Rol'), (@CtrlId, 2, 'Role');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnRegistrar', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Guardar'), (@CtrlId, 2, 'Save');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('grpAcciones', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Acciones'), (@CtrlId, 2, 'Actions');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('rbModoCrear', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Crear Usuario'), (@CtrlId, 2, 'Create User');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('rbModoEditar', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Editar Usuario'), (@CtrlId, 2, 'Edit User');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('chkActivo', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Activo'), (@CtrlId, 2, 'Active');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('lblTiempoBloqueo', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Bloqueo Duración:'), (@CtrlId, 2, 'Block Duration:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnBloquear', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Bloquear'), (@CtrlId, 2, 'Block');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnDesbloquear', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Desbloquear'), (@CtrlId, 2, 'Unblock');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('labelNombrePersona', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Nombre:'), (@CtrlId, 2, 'First Name:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('labelApellido', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Apellido:'), (@CtrlId, 2, 'Last Name:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvUsuarios.nombre', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Usuario'), (@CtrlId, 2, 'Username');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvUsuarios.nombrePersona', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Nombre'), (@CtrlId, 2, 'First Name');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvUsuarios.apellido', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Apellido'), (@CtrlId, 2, 'Last Name');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvUsuarios.activo', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Activo'), (@CtrlId, 2, 'Active');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvUsuarios.intentos', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Intentos Fallidos'), (@CtrlId, 2, 'Failed Attempts');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvUsuarios.bloqueadoHasta', 'FormGestionUsuarios');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Bloqueado Hasta'), (@CtrlId, 2, 'Blocked Until');

-- FormBitacora
INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Bitácora'), (@CtrlId, 2, 'Event Log');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('labelDesde', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Desde:'), (@CtrlId, 2, 'From:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('labelHasta', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Hasta:'), (@CtrlId, 2, 'To:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('labelActividad', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Actividad:'), (@CtrlId, 2, 'Activity:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('labelUsuario', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Usuario:'), (@CtrlId, 2, 'User:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnBuscar', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Buscar'), (@CtrlId, 2, 'Search');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvBitacora.id', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'ID'), (@CtrlId, 2, 'ID');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvBitacora.fecha', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Fecha'), (@CtrlId, 2, 'Date');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvBitacora.usuario', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Usuario'), (@CtrlId, 2, 'User');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvBitacora.descripcion', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Descripción'), (@CtrlId, 2, 'Description');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvBitacora.criticidad', 'FormBitacora');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Criticidad'), (@CtrlId, 2, 'Severity');

-- FormGestionRoles
INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Gestión de Roles'), (@CtrlId, 2, 'Role Management');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('grpAcciones', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Acciones'), (@CtrlId, 2, 'Actions');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('lblDetalle', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Seleccione un rol para ver su detalle.'), (@CtrlId, 2, 'Select a role to view details.');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('lblModo', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Modo:'), (@CtrlId, 2, 'Mode:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('rbModoCrear', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Crear'), (@CtrlId, 2, 'Create');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('rbModoEditar', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Editar / Eliminar'), (@CtrlId, 2, 'Edit / Delete');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('lblNombreRol', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Nombre:'), (@CtrlId, 2, 'Name:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnCrearRolRaiz', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Crear Rol Raíz'), (@CtrlId, 2, 'Create Root Role');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnCrearSubRol', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Crear Sub-Rol'), (@CtrlId, 2, 'Create Sub-Role');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnEditarNombre', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Guardar Nombre'), (@CtrlId, 2, 'Save Name');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnEliminarRol', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Eliminar Rol'), (@CtrlId, 2, 'Delete Role');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('grpAsignarPermiso', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Asignar Permiso'), (@CtrlId, 2, 'Assign Permission');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnAsignarPermiso', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Agregar Permiso'), (@CtrlId, 2, 'Add Permission');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnQuitarItem', 'FormGestionRoles');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Quitar Ítem Seleccionado del Padre'), (@CtrlId, 2, 'Remove Selected Item from Parent');

-- FormHistorialUsuario
INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Control de Cambios - Usuarios'), (@CtrlId, 2, 'Change Control - Users');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('label1', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Seleccionar Usuario:'), (@CtrlId, 2, 'Select User:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnCargarHistorial', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Ver Historial'), (@CtrlId, 2, 'View History');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnRestaurar', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Restaurar Estado'), (@CtrlId, 2, 'Restore State');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvHistorial.idHistorial', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'ID Historial'), (@CtrlId, 2, 'History ID');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvHistorial.idUsuario', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'ID Usuario'), (@CtrlId, 2, 'User ID');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvHistorial.fecha', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Fecha Cambio'), (@CtrlId, 2, 'Change Date');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvHistorial.usuario', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Usuario'), (@CtrlId, 2, 'Username');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvHistorial.nombrePersona', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Nombre'), (@CtrlId, 2, 'First Name');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvHistorial.apellido', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Apellido'), (@CtrlId, 2, 'Last Name');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvHistorial.estado', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Estado Activo'), (@CtrlId, 2, 'Active Status');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvHistorial.editorNombre', 'FormHistorialUsuario');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Modificado Por'), (@CtrlId, 2, 'Modified By');

-- FormGestionIdiomas
INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Gestión de Idiomas'), (@CtrlId, 2, 'Language Management');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('lblNuevoIdioma', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Nuevo Idioma:'), (@CtrlId, 2, 'New Language:');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnCrearIdioma', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Crear'), (@CtrlId, 2, 'Create');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnToggleActivo', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Activar / Desactivar (Eliminar)'), (@CtrlId, 2, 'Enable / Disable (Delete)');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnGuardarTraducciones', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Guardar Traducciones'), (@CtrlId, 2, 'Save Translations');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('btnAplicar', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Aplicar Idioma'), (@CtrlId, 2, 'Apply Language');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvIdiomas.id', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'ID'), (@CtrlId, 2, 'ID');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvIdiomas.nombre', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Nombre'), (@CtrlId, 2, 'Name');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvIdiomas.activo', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Activo'), (@CtrlId, 2, 'Active');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvTraducciones.formulario', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Formulario'), (@CtrlId, 2, 'Form');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvTraducciones.control', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Control'), (@CtrlId, 2, 'Control');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('dgvTraducciones.texto', 'FormGestionIdiomas');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Texto'), (@CtrlId, 2, 'Text');

-- Formularios PN1
INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormRegistrarCaptacion');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Registrar Captación'), (@CtrlId, 2, 'Register Listing');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormGestionarCaptaciones');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Gestionar Captaciones'), (@CtrlId, 2, 'Manage Listings');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormValidarTasacion');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Validar Tasación'), (@CtrlId, 2, 'Validate Appraisal');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormCargarMaterialAudiovisual');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Cargar Material Audiovisual'), (@CtrlId, 2, 'Upload Audiovisual Material');

INSERT INTO [dbo].[Control] ([nombre], [formulario]) VALUES ('(form)', 'FormPublicarInmueble');
SET @CtrlId = SCOPE_IDENTITY();
INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto]) VALUES (@CtrlId, 1, 'Publicar Inmueble'), (@CtrlId, 2, 'Publish Property');

-- Digito Verificador Vertical Inicial
INSERT INTO [dbo].[DigitoVerificadorVertical] ([tabla], [dvv]) VALUES ('Usuario', 0);
GO

-- ====================================================
-- Cálculo e inicialización de Dígitos Verificadores
-- ====================================================
IF OBJECT_ID('dbo.CalcularValorString', 'FN') IS NOT NULL DROP FUNCTION dbo.CalcularValorString;
GO
CREATE FUNCTION dbo.CalcularValorString (
    @Valor NVARCHAR(MAX),
    @PosicionAtributo INT
)
RETURNS BIGINT
AS
BEGIN
    IF @Valor IS NULL OR @Valor = ''
        RETURN 0;
        
    DECLARE @Suma BIGINT = 0;
    DECLARE @i INT = 1;
    DECLARE @Len INT = LEN(@Valor);
    
    WHILE @i <= @Len
    BEGIN
        SET @Suma = @Suma + (UNICODE(SUBSTRING(@Valor, @i, 1)) * @i * @PosicionAtributo);
        SET @i = @i + 1;
    END
    
    RETURN @Suma;
END;
GO

IF OBJECT_ID('dbo.CalcularDVH', 'FN') IS NOT NULL DROP FUNCTION dbo.CalcularDVH;
GO
CREATE FUNCTION dbo.CalcularDVH (
    @Id INT,
    @Nombre NVARCHAR(100),
    @Password NVARCHAR(200),
    @Activo BIT,
    @NombrePersona NVARCHAR(100),
    @Apellido NVARCHAR(100)
)
RETURNS BIGINT
AS
BEGIN
    DECLARE @DVH BIGINT = 0;
    SET @DVH = @DVH + dbo.CalcularValorString(CAST(@Id AS NVARCHAR(50)), 1);
    SET @DVH = @DVH + dbo.CalcularValorString(@Nombre, 2);
    SET @DVH = @DVH + dbo.CalcularValorString(@Password, 3);
    SET @DVH = @DVH + dbo.CalcularValorString(CASE WHEN @Activo = 1 THEN '1' ELSE '0' END, 4);
    SET @DVH = @DVH + dbo.CalcularValorString(ISNULL(@NombrePersona, ''), 5);
    SET @DVH = @DVH + dbo.CalcularValorString(ISNULL(@Apellido, ''), 6);
    RETURN @DVH;
END;
GO

-- 1. Calcular y actualizar DVH para los usuarios semilla
UPDATE dbo.Usuario
SET dvh = dbo.CalcularDVH(id, nombre, pass, activo, nombre_persona, apellido);

-- 2. Calcular y actualizar DVH para los registros de historial de los usuarios semilla
UPDATE dbo.Usuario_Historial
SET dvh = dbo.CalcularDVH(id_usuario, nombre, pass, activo, nombre_persona, apellido);

-- 3. Calcular e inicializar el DVV para la tabla Usuario
DECLARE @CalculatedDVV BIGINT;
SELECT @CalculatedDVV = SUM(dvh) FROM dbo.Usuario;

UPDATE dbo.DigitoVerificadorVertical
SET dvv = ISNULL(@CalculatedDVV, 0)
WHERE tabla = 'Usuario';
GO

-- Limpieza de las funciones auxiliares de cálculo
DROP FUNCTION dbo.CalcularDVH;
DROP FUNCTION dbo.CalcularValorString;
GO
