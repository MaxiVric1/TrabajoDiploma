-- ====================================================
-- TpIngSoft - Estructura y datos iniciales (instalación nueva)
-- Crea la base si no existe, BORRA y recrea todas las tablas (sistema base, PN1 y PN2) en su versión
-- final e inserta los datos iniciales: idiomas, roles y permisos (Composite), usuarios con DVH, DVV y los
-- controles y traducciones (español e inglés) de todos los formularios.
-- ATENCIÓN: se pierden todos los datos existentes.
-- Después ejecutar actualizacion-schema.sql (lleva las tablas a la versión actual: campos y tablas agregados
-- después de esta versión base) y stored-procedures.sql.
-- ====================================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TpIngSoft')
BEGIN
    CREATE DATABASE [TpIngSoft];
END
GO

USE [TpIngSoft];
GO

-- Requeridas por el índice filtrado de Contrato (SSMS las trae activas; sqlcmd no, salvo con -I)
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ====================================================
-- 1. Creación de tablas
-- ====================================================

-- Se borran en orden inverso a sus dependencias: primero PN2, después PN1 y por último el sistema base
-- Tablas del PN2: referencian a Usuario, se borran primero
-- (Garante la crea actualizacion-schema.sql; referencia a Contrato y se borra antes)
IF OBJECT_ID('[dbo].[SuscripcionFideicomiso]', 'U') IS NOT NULL DROP TABLE [dbo].[SuscripcionFideicomiso];
IF OBJECT_ID('[dbo].[Inversor]', 'U') IS NOT NULL DROP TABLE [dbo].[Inversor];
IF OBJECT_ID('[dbo].[ProyectoInversion]', 'U') IS NOT NULL DROP TABLE [dbo].[ProyectoInversion];
IF OBJECT_ID('[dbo].[HistorialICC]', 'U') IS NOT NULL DROP TABLE [dbo].[HistorialICC];
IF OBJECT_ID('[dbo].[Garante]', 'U') IS NOT NULL DROP TABLE [dbo].[Garante];
IF OBJECT_ID('[dbo].[ActualizacionContrato]', 'U') IS NOT NULL DROP TABLE [dbo].[ActualizacionContrato];
IF OBJECT_ID('[dbo].[IndiceBCRA]', 'U') IS NOT NULL DROP TABLE [dbo].[IndiceBCRA];
IF OBJECT_ID('[dbo].[ContactoCobranza]', 'U') IS NOT NULL DROP TABLE [dbo].[ContactoCobranza];
IF OBJECT_ID('[dbo].[AccionLegal]', 'U') IS NOT NULL DROP TABLE [dbo].[AccionLegal];
IF OBJECT_ID('[dbo].[PagoAlquiler]', 'U') IS NOT NULL DROP TABLE [dbo].[PagoAlquiler];
IF OBJECT_ID('[dbo].[Contrato]', 'U') IS NOT NULL DROP TABLE [dbo].[Contrato];
IF OBJECT_ID('[dbo].[InmuebleAlquiler]', 'U') IS NOT NULL DROP TABLE [dbo].[InmuebleAlquiler];
IF OBJECT_ID('[dbo].[Inquilino]', 'U') IS NOT NULL DROP TABLE [dbo].[Inquilino];

-- Tablas del PN1: referencian a Usuario, se borran primero
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

-- ----------------------------------------------------
-- Tablas del sistema base
-- ----------------------------------------------------

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

-- ----------------------------------------------------
-- Tablas del PN1 - Captación de propiedades
-- ----------------------------------------------------

CREATE TABLE [dbo].[Cliente] (
    [id_cliente] INT IDENTITY(1,1) PRIMARY KEY,
    [dni] VARCHAR(15) NOT NULL UNIQUE,
    [nombre] NVARCHAR(100) NOT NULL,
    [apellido] NVARCHAR(100) NOT NULL,
    [telefono] VARCHAR(30) NULL,
    [email] VARCHAR(100) NULL
);

-- Un inmueble "despublicado" es uno en estado Tasado con fecha_publicacion (ya estuvo publicado)
CREATE TABLE [dbo].[Inmueble] (
    [id_inmueble] INT IDENTITY(1,1) PRIMARY KEY,
    [tipo_inmueble] VARCHAR(50) NOT NULL,
    [direccion] NVARCHAR(200) NOT NULL,
    [metros_cuadrados] DECIMAL(10,2) NOT NULL CHECK ([metros_cuadrados] > 0),
    [cantidad_ambientes] INT NOT NULL CHECK ([cantidad_ambientes] > 0),
    [estado_actual] VARCHAR(20) NOT NULL DEFAULT 'Captado'
        CHECK ([estado_actual] IN ('Captado', 'Tasado', 'Rechazado', 'Publicado')),
    [id_cliente] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Cliente]([id_cliente]),
    [texto_comercial] NVARCHAR(2000) NULL,       -- describe al inmueble (no a cada archivo de material)
    [fecha_publicacion] DATETIME NULL,
    [fecha_despublicacion] DATETIME NULL
);

-- La autorización de venta no se adjunta: el agente confirma en el formulario que la recibió firmada
CREATE TABLE [dbo].[Captacion] (
    [nro_captacion] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_registro] DATETIME DEFAULT GETDATE(),
    [valor_pretendido] DECIMAL(18,2) NOT NULL CHECK ([valor_pretendido] > 0),
    [id_inmueble] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Inmueble]([id_inmueble]),
    [id_usuario_agente] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
);

CREATE TABLE [dbo].[Tasacion] (
    [nro_tasacion] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_dictamen] DATETIME DEFAULT GETDATE(),
    [estado_dictamen] VARCHAR(20) NOT NULL DEFAULT 'Pendiente'
        CHECK ([estado_dictamen] IN ('Pendiente', 'Aprobada', 'Rechazada')),
    [motivo_rechazo] NVARCHAR(500) NULL,
    [valor_final] DECIMAL(18,2) NULL CHECK ([valor_final] > 0),
    [id_captacion] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Captacion]([nro_captacion]),
    [id_usuario_tasador] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
    [valor_sugerido] DECIMAL(18,2) NULL,         -- al rechazar, valor alternativo para renegociar
    CHECK ([estado_dictamen] <> 'Rechazada' OR [motivo_rechazo] IS NOT NULL),  -- un rechazo debe tener motivo
    -- Solo una tasación rechazada puede sugerir un valor, y debe ser positivo
    CONSTRAINT [CK_Tasacion_ValorSugerido]
        CHECK ([valor_sugerido] IS NULL OR ([valor_sugerido] > 0 AND [estado_dictamen] = 'Rechazada'))
);

CREATE TABLE [dbo].[MaterialAudiovisual] (
    [id_material] INT IDENTITY(1,1) PRIMARY KEY,
    [tipo_archivo] VARCHAR(20) NOT NULL,
    [ruta_archivo] NVARCHAR(500) NOT NULL,
    [fecha_carga] DATETIME DEFAULT GETDATE(),
    [id_inmueble] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Inmueble]([id_inmueble]),
    [id_usuario_marketing] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
);

-- ----------------------------------------------------
-- Tablas del PN2 - Seguimiento de alquileres
-- ----------------------------------------------------

CREATE TABLE [dbo].[Inquilino] (
    [id_inquilino] INT IDENTITY(1,1) PRIMARY KEY,
    [nombre] NVARCHAR(100) NOT NULL,
    [apellido] NVARCHAR(100) NOT NULL,
    [dni] VARCHAR(15) NOT NULL UNIQUE,
    [telefono] VARCHAR(30) NULL,
    [email] VARCHAR(100) NULL
);

CREATE TABLE [dbo].[InmuebleAlquiler] (
    [id_inmueble_alquiler] INT IDENTITY(1,1) PRIMARY KEY,
    [direccion] NVARCHAR(200) NOT NULL,
    [tipo_inmueble] VARCHAR(50) NOT NULL
        CHECK ([tipo_inmueble] IN ('Casa', 'Departamento', 'Local', 'Terreno', 'Oficina'))
);

-- La fecha de fin no se guarda: es DATEADD(MONTH, duracion_meses, fecha_inicio) y la calculan los SPs
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
    [id_usuario_agente] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id]),
    -- Aviso de que el inquilino desocupa al vencimiento (el contrato sigue vigente hasta entonces)
    [aviso_desocupacion] BIT NOT NULL CONSTRAINT [DF_Contrato_AvisoDesocupacion] DEFAULT 0,
    [fecha_aviso_desocupacion] DATETIME NULL,
    CONSTRAINT [CK_Contrato_AvisoDesocupacion]
        CHECK (([aviso_desocupacion] = 1 AND [fecha_aviso_desocupacion] IS NOT NULL)
            OR ([aviso_desocupacion] = 0 AND [fecha_aviso_desocupacion] IS NULL))
);

-- Un inmueble no puede tener dos contratos vigentes (Activo o Por Vencer) al mismo tiempo
CREATE UNIQUE INDEX [UX_Contrato_InmuebleVigente] ON [dbo].[Contrato]([id_inmueble_alquiler])
    WHERE [estado_contrato] <> 'Finalizado';

-- Un registro por contrato y mes: fecha_pago y monto_pagado vacíos/0 mientras el mes no se pagó
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

CREATE TABLE [dbo].[AccionLegal] (
    [id_accion] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_accion] DATETIME DEFAULT GETDATE(),
    [descripcion] NVARCHAR(1000) NOT NULL,
    [tipo_accion] VARCHAR(20) NOT NULL
        CHECK ([tipo_accion] IN ('Intimacion', 'Resuelto', 'Desalojo')),
    [id_contrato] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Contrato]([id_contrato]),
    [id_usuario_legal] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
);

-- Intentos de contacto con el inquilino por pagos en mora (vinculados al contrato: tratan toda su deuda)
CREATE TABLE [dbo].[ContactoCobranza] (
    [id_contacto] INT IDENTITY(1,1) PRIMARY KEY,
    [fecha_contacto] DATETIME NOT NULL,
    [descripcion] NVARCHAR(1000) NOT NULL,
    [id_contrato] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Contrato]([id_contrato]),
    [id_usuario_analista] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Usuario]([id])
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

-- Roles y permisos del PN1, PN2 y PN3: cada rol con su permiso (BE.PermisosDefinidos) y todos como
-- hijos del rol Admin (Composite): Admin ve todos los menús de los procesos de negocio
DECLARE @RolesPN TABLE ([orden] INT, [rol] VARCHAR(50), [permiso] VARCHAR(50));
INSERT INTO @RolesPN VALUES
    (1, 'Agente Comercial',            'AccesoPN1AgenteComercial'),
    (2, 'Tasador',                     'AccesoPN1Tasador'),
    (3, 'Marketing',                   'AccesoPN1Marketing'),
    (4, 'Administrador de Alquileres', 'AccesoPN2AdministradorAlquileres'),
    (5, 'Analista de Cobranzas',       'AccesoPN2AnalistaCobranzas'),
    (6, 'Asesor Legal',                'AccesoPN2AsesorLegal'),
    (7, 'Analista de Inversiones',     'AccesoPN3Analisis'),
    (8, 'Arquitectura',                  'AccesoPN3Arquitectura'),
    (9, 'Directorio',                    'AccesoPN3Directorio'),
    (10, 'Inversiones',                'AccesoPN3Inversiones');

INSERT INTO [dbo].[Permiso] ([nombre]) SELECT [permiso] FROM @RolesPN ORDER BY [orden];
INSERT INTO [dbo].[Rol] ([nombre]) SELECT [rol] FROM @RolesPN ORDER BY [orden];

INSERT INTO [dbo].[RolPermiso] ([id_padre], [id_hijo], [tipo_hijo])
SELECT ro.[id], pe.[id], 'Permiso'
FROM @RolesPN r
JOIN [dbo].[Rol] ro ON ro.[nombre] = r.[rol]
JOIN [dbo].[Permiso] pe ON pe.[nombre] = r.[permiso];

INSERT INTO [dbo].[RolPermiso] ([id_padre], [id_hijo], [tipo_hijo])
SELECT (SELECT [id] FROM [dbo].[Rol] WHERE [nombre] = 'Admin'), ro.[id], 'Rol'
FROM @RolesPN r
JOIN [dbo].[Rol] ro ON ro.[nombre] = r.[rol];

-- Usuarios por rol (apuntan al Idioma Español = 1). Contraseña = nombre de usuario + "123",
-- guardada en SHA256 como SERVICIOS.Encriptador.Hash. El DVH se calcula al final, junto con el de todos.
DECLARE @UsuariosPN TABLE ([orden] INT, [nombre] VARCHAR(50), [pass] VARCHAR(100), [nombre_persona] NVARCHAR(100),
                           [apellido] NVARCHAR(100), [rol] VARCHAR(50));
INSERT INTO @UsuariosPN VALUES
    -- AgenteComercial123
    (1, 'AgenteComercial',         '7fbceb24964838de52eab6c38b26b3ed871c043889e419748d0d90472a9350b5', N'Agente', N'Comercial', 'Agente Comercial'),
    -- Tasador123
    (2, 'Tasador',                 '69f34ebaa2cf78e16883e4b04f4de4f713dff02d3fc6416f698cd19b0bbca786', N'Tasador', N'Sistema', 'Tasador'),
    -- Marketing123
    (3, 'Marketing',               '95eba62dc02be11317be0df90d4d80e11f59939d35e4a75e0916ebcec6e18a5e', N'Marketing', N'Sistema', 'Marketing'),
    -- AdministradorAlquileres123
    (4, 'AdministradorAlquileres', '33c547ffd85494ad5f1d53d06c595ca8987bc22c7f925c60cc3a4153c4edd0fd', N'Administrador', N'Alquileres', 'Administrador de Alquileres'),
    -- AnalistaCobranzas123
    (5, 'AnalistaCobranzas',       '7830d1704d6a2d1f2923a53a42156f3a07d12076817b6b6bc9cd52ea966dc6a4', N'Analista', N'Cobranzas', 'Analista de Cobranzas'),
    -- AsesorLegal123
    (6, 'AsesorLegal',             '9ef67c768b149aa820db7778763b505f04c6b35f767eaee7864b451d07a99fa1', N'Asesor', N'Legal', 'Asesor Legal'),
    -- AnalistaInversiones123
    (7, 'AnalistaInversiones',     'f53d7900cae1b6cf4355764d2845998c65b89740ec961e53640cf121cd6c5c5f', N'Analista', N'Inversiones', 'Analista de Inversiones'),
    -- Arquitectura123
    (8, 'Arquitectura',            '643ea49d4863cd97325107e90a0571512fb7fd612e0228bbd96114115b897bc9', N'Arquitectura', N'Sistema', 'Arquitectura'),
    -- Directorio123
    (9, 'Directorio',              '0d4ad2d25c6082f589c0f75709a7980b64a93e2c6bcf22ee347f21826e2ca369', N'Directorio', N'Sistema', 'Directorio'),
    -- Inversiones123
    (10, 'Inversiones',            'bc80595a2638cd35ba465c739cd1861b82b5ac162241fb41e5b1f4cc99af9cb1', N'Inversiones', N'Sistema', 'Inversiones');

INSERT INTO [dbo].[Usuario] ([nombre], [nombre_persona], [apellido], [pass], [activo], [id_idioma])
SELECT [nombre], [nombre_persona], [apellido], [pass], 1, 1 FROM @UsuariosPN ORDER BY [orden];

INSERT INTO [dbo].[UsuarioRol] ([id_usuario], [id_rol])
SELECT us.[id], ro.[id]
FROM @UsuariosPN u
JOIN [dbo].[Usuario] us ON us.[nombre] = u.[nombre]
JOIN [dbo].[Rol] ro ON ro.[nombre] = u.[rol];

-- Historial de alta (tipo 'INSERT'), como el de admin y user
INSERT INTO [dbo].[Usuario_Historial] ([id_usuario], [nombre], [nombre_persona], [apellido], [pass], [activo], [dvh], [id_usuario_autor], [tipo_operacion])
SELECT us.[id], us.[nombre], us.[nombre_persona], us.[apellido], us.[pass], us.[activo], 0, @AdminId, 'INSERT'
FROM @UsuariosPN u
JOIN [dbo].[Usuario] us ON us.[nombre] = u.[nombre];

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

-- ----------------------------------------------------
-- Controles y traducciones del PN1 (menú y formularios)
-- Los que ya cargó el bloque anterior (NOT EXISTS) se omiten
-- ----------------------------------------------------
DECLARE @Nuevos TABLE ([nombre] NVARCHAR(100), [formulario] NVARCHAR(100), [es] NVARCHAR(500), [en] NVARCHAR(500));
INSERT INTO @Nuevos VALUES
    ('pn1ToolStripMenuItem', 'FormMain', 'PN1', 'PN1'),
    ('registrarCaptacionToolStripMenuItem', 'FormMain', N'Registrar Captación', 'Register Listing'),
    ('gestionarCaptacionesToolStripMenuItem', 'FormMain', 'Gestionar Captaciones', 'Manage Listings'),
    ('validarTasacionToolStripMenuItem', 'FormMain', N'Validar Tasación', 'Validate Appraisal'),
    ('cargarMaterialAudiovisualToolStripMenuItem', 'FormMain', 'Cargar Material Audiovisual', 'Upload Audiovisual Material'),
    ('publicarInmuebleToolStripMenuItem', 'FormMain', 'Publicar Inmueble', 'Publish Property'),
    ('inmueblesPublicadosToolStripMenuItem', 'FormMain', 'Inmuebles Publicados', 'Published Properties'),
    ('(form)', 'FormRegistrarCaptacion', N'Registrar Captación', 'Register Listing'),
    ('(form)', 'FormGestionarCaptaciones', 'Gestionar Captaciones', 'Manage Listings'),
    ('(form)', 'FormValidarTasacion', N'Validar Tasación', 'Validate Appraisal'),
    ('(form)', 'FormCargarMaterialAudiovisual', 'Cargar Material Audiovisual', 'Upload Audiovisual Material'),
    ('(form)', 'FormPublicarInmueble', 'Publicar Inmueble', 'Publish Property'),
    ('(form)', 'FormInmueblesPublicados', 'Inmuebles Publicados', 'Published Properties'),
    -- FormRegistrarCaptacion
    ('grpCliente', 'FormRegistrarCaptacion', 'Cliente', 'Client'),
    ('lblDni', 'FormRegistrarCaptacion', 'DNI:', 'ID Number:'),
    ('btnBuscarCliente', 'FormRegistrarCaptacion', 'Buscar', 'Search'),
    ('lblNombre', 'FormRegistrarCaptacion', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormRegistrarCaptacion', 'Apellido:', 'Last Name:'),
    ('lblTelefono', 'FormRegistrarCaptacion', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormRegistrarCaptacion', 'Email:', 'Email:'),
    ('grpInmueble', 'FormRegistrarCaptacion', 'Inmueble', 'Property'),
    ('lblTipo', 'FormRegistrarCaptacion', 'Tipo:', 'Type:'),
    ('lblDireccion', 'FormRegistrarCaptacion', N'Dirección:', 'Address:'),
    ('lblMetros', 'FormRegistrarCaptacion', 'Metros cuadrados:', 'Square meters:'),
    ('lblAmbientes', 'FormRegistrarCaptacion', 'Ambientes:', 'Rooms:'),
    ('grpCaptacion', 'FormRegistrarCaptacion', N'Captación', 'Listing'),
    ('lblValorPretendido', 'FormRegistrarCaptacion', 'Valor pretendido:', 'Asking price:'),
    ('chkAutorizacionRecibida', 'FormRegistrarCaptacion', N'Autorización de Venta firmada recibida', 'Signed Sale Authorization received'),
    ('btnGuardar', 'FormRegistrarCaptacion', 'Guardar', 'Save'),
    ('btnCancelar', 'FormRegistrarCaptacion', 'Cancelar', 'Cancel'),
    -- FormGestionarCaptaciones
    ('lblFiltroEstado', 'FormGestionarCaptaciones', 'Estado:', 'Status:'),
    ('btnActualizar', 'FormGestionarCaptaciones', 'Actualizar', 'Refresh'),
    ('grpRenegociacion', 'FormGestionarCaptaciones', N'Renegociación', 'Renegotiation'),
    ('lblDictamen', 'FormGestionarCaptaciones', N'Último dictamen:', 'Latest appraisal:'),
    ('lblValorActual', 'FormGestionarCaptaciones', 'Valor pretendido actual:', 'Current asking price:'),
    ('lblMotivo', 'FormGestionarCaptaciones', 'Motivo de rechazo:', 'Rejection reason:'),
    ('lblValorSugerido', 'FormGestionarCaptaciones', 'Valor sugerido:', 'Suggested value:'),
    ('lblNuevoValor', 'FormGestionarCaptaciones', 'Nuevo valor pretendido:', 'New asking price:'),
    ('btnRenegociar', 'FormGestionarCaptaciones', 'Renegociar', 'Renegotiate'),
    ('dgvCaptaciones.nro', 'FormGestionarCaptaciones', N'Nro Captación', 'Listing No.'),
    ('dgvCaptaciones.fecha', 'FormGestionarCaptaciones', 'Fecha', 'Date'),
    ('dgvCaptaciones.cliente', 'FormGestionarCaptaciones', 'Cliente', 'Client'),
    ('dgvCaptaciones.direccion', 'FormGestionarCaptaciones', N'Dirección', 'Address'),
    ('dgvCaptaciones.tipo', 'FormGestionarCaptaciones', 'Tipo', 'Type'),
    ('dgvCaptaciones.valor', 'FormGestionarCaptaciones', 'Valor Pretendido', 'Asking Price'),
    ('dgvCaptaciones.estado', 'FormGestionarCaptaciones', 'Estado', 'Status'),
    -- FormValidarTasacion
    ('lblPendientes', 'FormValidarTasacion', N'Captaciones pendientes de tasación', 'Listings pending appraisal'),
    ('btnActualizar', 'FormValidarTasacion', 'Actualizar', 'Refresh'),
    ('grpDetalle', 'FormValidarTasacion', N'Detalle de la captación', 'Listing details'),
    ('lblCliente', 'FormValidarTasacion', 'Cliente:', 'Client:'),
    ('lblDni', 'FormValidarTasacion', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormValidarTasacion', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormValidarTasacion', 'Email:', 'Email:'),
    ('lblValorPretendido', 'FormValidarTasacion', 'Valor pretendido:', 'Asking price:'),
    ('lblDictamenAnterior', 'FormValidarTasacion', 'Dictamen anterior:', 'Previous appraisal:'),
    ('grpDictamen', 'FormValidarTasacion', 'Dictamen', 'Appraisal decision'),
    ('rbAprobar', 'FormValidarTasacion', 'Aprobar', 'Approve'),
    ('rbRechazar', 'FormValidarTasacion', 'Rechazar', 'Reject'),
    ('lblValorFinal', 'FormValidarTasacion', 'Valor final:', 'Final value:'),
    ('lblMotivo', 'FormValidarTasacion', 'Motivo de rechazo:', 'Rejection reason:'),
    ('lblValorSugerido', 'FormValidarTasacion', 'Valor sugerido (opcional):', 'Suggested value (optional):'),
    ('btnRegistrar', 'FormValidarTasacion', 'Registrar dictamen', 'Register decision'),
    ('dgvPendientes.nro', 'FormValidarTasacion', N'Nro Captación', 'Listing No.'),
    ('dgvPendientes.fecha', 'FormValidarTasacion', 'Fecha', 'Date'),
    ('dgvPendientes.cliente', 'FormValidarTasacion', 'Cliente', 'Client'),
    ('dgvPendientes.direccion', 'FormValidarTasacion', N'Dirección', 'Address'),
    ('dgvPendientes.tipo', 'FormValidarTasacion', 'Tipo', 'Type'),
    ('dgvPendientes.metros', 'FormValidarTasacion', 'Metros Cuadrados', 'Square Meters'),
    ('dgvPendientes.ambientes', 'FormValidarTasacion', 'Ambientes', 'Rooms'),
    ('dgvPendientes.valor', 'FormValidarTasacion', 'Valor Pretendido', 'Asking Price'),
    -- FormCargarMaterialAudiovisual
    ('lblInmuebles', 'FormCargarMaterialAudiovisual', N'Inmuebles con tasación aprobada', 'Properties with approved appraisal'),
    ('btnActualizar', 'FormCargarMaterialAudiovisual', 'Actualizar', 'Refresh'),
    ('grpNuevoMaterial', 'FormCargarMaterialAudiovisual', 'Nuevo material', 'New material'),
    ('lblTextoComercial', 'FormCargarMaterialAudiovisual', 'Texto comercial:', 'Marketing text:'),
    ('lblArchivos', 'FormCargarMaterialAudiovisual', 'Archivos:', 'Files:'),
    ('btnAgregarArchivo', 'FormCargarMaterialAudiovisual', 'Agregar archivo...', 'Add file...'),
    ('btnGuardar', 'FormCargarMaterialAudiovisual', 'Guardar', 'Save'),
    ('grpMaterialCargado', 'FormCargarMaterialAudiovisual', 'Material ya cargado', 'Uploaded material'),
    ('dgvInmuebles.nro', 'FormCargarMaterialAudiovisual', N'Nro Captación', 'Listing No.'),
    ('dgvInmuebles.cliente', 'FormCargarMaterialAudiovisual', 'Cliente', 'Client'),
    ('dgvInmuebles.direccion', 'FormCargarMaterialAudiovisual', N'Dirección', 'Address'),
    ('dgvInmuebles.tipo', 'FormCargarMaterialAudiovisual', 'Tipo', 'Type'),
    ('dgvInmuebles.valorFinal', 'FormCargarMaterialAudiovisual', N'Valor Final de Tasación', 'Final Appraised Value'),
    ('dgvArchivos.archivo', 'FormCargarMaterialAudiovisual', 'Archivo', 'File'),
    ('dgvArchivos.tipo', 'FormCargarMaterialAudiovisual', 'Tipo', 'Type'),
    ('dgvArchivos.quitar', 'FormCargarMaterialAudiovisual', 'Quitar', 'Remove'),
    ('dgvMaterialCargado.tipo', 'FormCargarMaterialAudiovisual', 'Tipo', 'Type'),
    ('dgvMaterialCargado.archivo', 'FormCargarMaterialAudiovisual', 'Archivo', 'File'),
    ('dgvMaterialCargado.fecha', 'FormCargarMaterialAudiovisual', 'Fecha de carga', 'Upload date'),
    -- FormPublicarInmueble
    ('lblListos', 'FormPublicarInmueble', 'Inmuebles listos para publicar', 'Properties ready to publish'),
    ('btnActualizar', 'FormPublicarInmueble', 'Actualizar', 'Refresh'),
    ('grpDetalle', 'FormPublicarInmueble', 'Detalle del inmueble', 'Property details'),
    ('lblTextoComercial', 'FormPublicarInmueble', 'Texto comercial:', 'Marketing text:'),
    ('lblMaterial', 'FormPublicarInmueble', 'Material cargado:', 'Uploaded material:'),
    ('lblVistaPrevia', 'FormPublicarInmueble', 'Vista previa:', 'Preview:'),
    ('lblSinVistaPrevia', 'FormPublicarInmueble', 'Vista previa no disponible', 'Preview not available'),
    ('btnPublicar', 'FormPublicarInmueble', 'Publicar', 'Publish'),
    ('dgvInmuebles.nro', 'FormPublicarInmueble', N'Nro Captación', 'Listing No.'),
    ('dgvInmuebles.cliente', 'FormPublicarInmueble', 'Cliente', 'Client'),
    ('dgvInmuebles.direccion', 'FormPublicarInmueble', N'Dirección', 'Address'),
    ('dgvInmuebles.tipo', 'FormPublicarInmueble', 'Tipo', 'Type'),
    ('dgvInmuebles.valorFinal', 'FormPublicarInmueble', N'Valor Final de Tasación', 'Final Appraised Value'),
    ('dgvInmuebles.archivos', 'FormPublicarInmueble', 'Archivos cargados', 'Uploaded files'),
    ('dgvMaterial.tipo', 'FormPublicarInmueble', 'Tipo', 'Type'),
    ('dgvMaterial.archivo', 'FormPublicarInmueble', 'Archivo', 'File'),
    ('dgvMaterial.fecha', 'FormPublicarInmueble', 'Fecha de carga', 'Upload date'),
    -- FormInmueblesPublicados
    ('lblPublicados', 'FormInmueblesPublicados', 'Inmuebles publicados', 'Published properties'),
    ('btnActualizar', 'FormInmueblesPublicados', 'Actualizar', 'Refresh'),
    ('grpDetalle', 'FormInmueblesPublicados', 'Detalle del inmueble', 'Property details'),
    ('lblTextoComercial', 'FormInmueblesPublicados', 'Texto comercial:', 'Marketing text:'),
    ('lblMaterial', 'FormInmueblesPublicados', 'Material cargado:', 'Uploaded material:'),
    ('lblVistaPrevia', 'FormInmueblesPublicados', 'Vista previa:', 'Preview:'),
    ('lblSinVistaPrevia', 'FormInmueblesPublicados', 'Vista previa no disponible', 'Preview not available'),
    ('btnDespublicar', 'FormInmueblesPublicados', 'Despublicar', 'Unpublish'),
    ('btnRepublicar', 'FormInmueblesPublicados', 'Republicar', 'Republish'),
    ('dgvInmuebles.nro', 'FormInmueblesPublicados', N'Nro Captación', 'Listing No.'),
    ('dgvInmuebles.cliente', 'FormInmueblesPublicados', 'Cliente', 'Client'),
    ('dgvInmuebles.direccion', 'FormInmueblesPublicados', N'Dirección', 'Address'),
    ('dgvInmuebles.tipo', 'FormInmueblesPublicados', 'Tipo', 'Type'),
    ('dgvInmuebles.valorFinal', 'FormInmueblesPublicados', N'Valor Final de Tasación', 'Final Appraised Value'),
    ('dgvInmuebles.estado', 'FormInmueblesPublicados', 'Estado', 'Status'),
    ('dgvInmuebles.fechaPublicacion', 'FormInmueblesPublicados', N'Fecha de publicación', 'Publication date'),
    ('dgvInmuebles.fechaDespublicacion', 'FormInmueblesPublicados', N'Fecha de despublicación', 'Unpublication date'),
    ('dgvMaterial.tipo', 'FormInmueblesPublicados', 'Tipo', 'Type'),
    ('dgvMaterial.archivo', 'FormInmueblesPublicados', 'Archivo', 'File'),
    ('dgvMaterial.fecha', 'FormInmueblesPublicados', 'Fecha de carga', 'Upload date');

INSERT INTO [dbo].[Control] ([nombre], [formulario])
SELECT n.[nombre], n.[formulario]
FROM @Nuevos n
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Control] c WHERE c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]);

INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto])
SELECT c.[id], 1, n.[es]
FROM @Nuevos n
JOIN [dbo].[Control] c ON c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Traducciones] t WHERE t.[idcontrol] = c.[id] AND t.[ididioma] = 1);

INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto])
SELECT c.[id], 2, n.[en]
FROM @Nuevos n
JOIN [dbo].[Control] c ON c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]
WHERE EXISTS (SELECT 1 FROM [dbo].[Idioma] WHERE [id] = 2)
  AND NOT EXISTS (SELECT 1 FROM [dbo].[Traducciones] t WHERE t.[idcontrol] = c.[id] AND t.[ididioma] = 2);
GO

-- ----------------------------------------------------
-- Controles y traducciones del PN2 (menú y formularios)
-- ----------------------------------------------------
DECLARE @Nuevos TABLE ([nombre] NVARCHAR(100), [formulario] NVARCHAR(100), [es] NVARCHAR(500), [en] NVARCHAR(500));
INSERT INTO @Nuevos VALUES
    -- Menú
    ('pn2ToolStripMenuItem', 'FormMain', 'PN2', 'PN2'),
    ('gestionarContratosToolStripMenuItem', 'FormMain', 'Gestionar Contratos', 'Manage Contracts'),
    ('registrarContratoToolStripMenuItem', 'FormMain', 'Registrar Contrato', 'Register Contract'),
    ('registrarPagoToolStripMenuItem', 'FormMain', 'Registrar Pago', 'Register Payment'),
    ('gestionarMoraToolStripMenuItem', 'FormMain', 'Gestionar Mora', 'Manage Arrears'),
    ('gestionarLitigiosToolStripMenuItem', 'FormMain', 'Gestionar Litigios', 'Manage Litigation'),
    ('contratosPorVencerToolStripMenuItem', 'FormMain', 'Contratos Por Vencer', 'Expiring Contracts'),
    -- Títulos de los formularios
    ('(form)', 'FormGestionarContratos', 'Gestionar Contratos', 'Manage Contracts'),
    ('(form)', 'FormRegistrarContrato', 'Registrar Contrato', 'Register Contract'),
    ('(form)', 'FormRegistrarPago', 'Registrar Pago', 'Register Payment'),
    ('(form)', 'FormGestionarMora', 'Gestionar Mora', 'Manage Arrears'),
    ('(form)', 'FormGestionarLitigios', 'Gestionar Litigios', 'Manage Litigation'),
    ('(form)', 'FormContratosPorVencer', 'Contratos Por Vencer', 'Expiring Contracts'),
    ('(form)', 'FormRenovarContrato', N'Registrar Renovación', 'Register Renewal'),
    -- FormRegistrarContrato
    ('grpInquilino', 'FormRegistrarContrato', 'Inquilino', 'Tenant'),
    ('lblDni', 'FormRegistrarContrato', 'DNI:', 'ID Number:'),
    ('btnBuscarInquilino', 'FormRegistrarContrato', 'Buscar', 'Search'),
    ('lblNombre', 'FormRegistrarContrato', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormRegistrarContrato', 'Apellido:', 'Last Name:'),
    ('lblTelefono', 'FormRegistrarContrato', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormRegistrarContrato', 'Email:', 'Email:'),
    ('lblInmueble', 'FormRegistrarContrato', 'Disponibles:', 'Available:'),
    ('grpContrato', 'FormRegistrarContrato', 'Contrato', 'Contract'),
    ('lblFechaInicio', 'FormRegistrarContrato', 'Fecha de inicio:', 'Start date:'),
    ('lblDuracion', 'FormRegistrarContrato', N'Duración (meses):', 'Length (months):'),
    ('lblMonto', 'FormRegistrarContrato', 'Monto mensual:', 'Monthly rent:'),
    ('lblFechaFin', 'FormRegistrarContrato', 'Finaliza:', 'Ends:'),
    ('btnCancelar', 'FormRegistrarContrato', 'Cancelar', 'Cancel'),
    -- FormGestionarContratos
    ('lblFiltroEstado', 'FormGestionarContratos', 'Estado:', 'Status:'),
    ('btnActualizar', 'FormGestionarContratos', 'Actualizar', 'Refresh'),
    ('grpInquilino', 'FormGestionarContratos', 'Inquilino', 'Tenant'),
    ('lblNombre', 'FormGestionarContratos', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormGestionarContratos', 'Apellido:', 'Last Name:'),
    ('lblDni', 'FormGestionarContratos', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormGestionarContratos', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormGestionarContratos', 'Email:', 'Email:'),
    ('grpRenovacion', 'FormGestionarContratos', N'Renovación', 'Renewal'),
    ('lblMesesExtension', 'FormGestionarContratos', 'Extender (meses):', 'Extend (months):'),
    ('lblNuevoMonto', 'FormGestionarContratos', 'Nuevo monto mensual:', 'New monthly rent:'),
    ('lblNuevoVencimiento', 'FormGestionarContratos', 'Nuevo vencimiento:', 'New end date:'),
    ('btnRenovar', 'FormGestionarContratos', 'Renovar Contrato', 'Renew Contract'),
    ('grpPagos', 'FormGestionarContratos', 'Historial de pagos', 'Payment history'),
    ('lblMesActual', 'FormGestionarContratos', 'Mes actual:', 'Current month:'),
    ('dgvContratos.nro', 'FormGestionarContratos', 'Nro Contrato', 'Contract No.'),
    ('dgvContratos.inquilino', 'FormGestionarContratos', 'Inquilino', 'Tenant'),
    ('dgvContratos.direccion', 'FormGestionarContratos', N'Dirección', 'Address'),
    ('dgvContratos.tipo', 'FormGestionarContratos', 'Tipo', 'Type'),
    ('dgvContratos.monto', 'FormGestionarContratos', 'Monto Mensual', 'Monthly Rent'),
    ('dgvContratos.fechaInicio', 'FormGestionarContratos', 'Fecha Inicio', 'Start Date'),
    ('dgvContratos.fechaFin', 'FormGestionarContratos', 'Fecha Fin', 'End Date'),
    ('dgvContratos.estado', 'FormGestionarContratos', 'Estado', 'Status'),
    ('dgvContratos.alerta', 'FormGestionarContratos', 'Alerta Vencimiento', 'Expiry Alert'),
    ('dgvPagos.mes', 'FormGestionarContratos', 'Mes', 'Month'),
    ('dgvPagos.anio', 'FormGestionarContratos', N'Año', 'Year'),
    ('dgvPagos.estado', 'FormGestionarContratos', 'Estado', 'Status'),
    ('dgvPagos.monto', 'FormGestionarContratos', 'Monto Pagado', 'Amount Paid'),
    ('dgvPagos.recargo', 'FormGestionarContratos', 'Recargo', 'Surcharge'),
    ('dgvPagos.fechaPago', 'FormGestionarContratos', 'Fecha de Pago', 'Payment Date'),
    -- FormRegistrarPago
    ('grpContrato', 'FormRegistrarPago', 'Contrato', 'Contract'),
    ('lblNroContrato', 'FormRegistrarPago', 'Nro Contrato:', 'Contract No.:'),
    ('btnBuscar', 'FormRegistrarPago', 'Buscar', 'Search'),
    ('lblInquilino', 'FormRegistrarPago', 'Inquilino:', 'Tenant:'),
    ('lblDireccion', 'FormRegistrarPago', N'Dirección:', 'Address:'),
    ('lblMontoMensual', 'FormRegistrarPago', 'Monto mensual:', 'Monthly rent:'),
    ('lblEstadoContrato', 'FormRegistrarPago', 'Estado:', 'Status:'),
    ('grpPeriodo', 'FormRegistrarPago', N'Período a pagar', 'Period to pay'),
    ('lblPeriodo', 'FormRegistrarPago', N'Período:', 'Period:'),
    ('lblEstadoMes', 'FormRegistrarPago', 'Estado del mes:', 'Month status:'),
    ('lblFechaPago', 'FormRegistrarPago', 'Fecha de pago:', 'Payment date:'),
    ('lblMontoBase', 'FormRegistrarPago', 'Alquiler:', 'Rent:'),
    ('lblRecargo', 'FormRegistrarPago', 'Recargo por mora:', 'Late fee:'),
    ('lblTotal', 'FormRegistrarPago', 'Total a pagar:', 'Total due:'),
    ('grpResumen', 'FormRegistrarPago', 'Resumen del pago', 'Payment summary'),
    ('btnRegistrar', 'FormRegistrarPago', 'Registrar Pago', 'Register Payment'),
    -- FormGestionarMora
    ('grpInquilino', 'FormGestionarMora', 'Inquilino', 'Tenant'),
    ('lblNombre', 'FormGestionarMora', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormGestionarMora', 'Apellido:', 'Last Name:'),
    ('lblDni', 'FormGestionarMora', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormGestionarMora', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormGestionarMora', 'Email:', 'Email:'),
    ('grpContrato', 'FormGestionarMora', 'Contrato y deuda', 'Contract and debt'),
    ('lblDireccion', 'FormGestionarMora', N'Dirección:', 'Address:'),
    ('lblMontoMensual', 'FormGestionarMora', 'Monto mensual:', 'Monthly rent:'),
    ('lblFechaFin', 'FormGestionarMora', 'Fin del contrato:', 'Contract end:'),
    ('lblDiasMora', 'FormGestionarMora', N'Días en mora:', 'Days overdue:'),
    ('lblMontoAdeudado', 'FormGestionarMora', 'Con recargo:', 'With surcharge:'),
    ('btnEscalar', 'FormGestionarMora', 'Escalar a Legal', 'Escalate to Legal'),
    ('grpContactos', 'FormGestionarMora', 'Contactos con el inquilino', 'Tenant contacts'),
    ('lblFechaContacto', 'FormGestionarMora', 'Fecha:', 'Date:'),
    ('lblDescripcion', 'FormGestionarMora', N'Qué se habló:', 'What was discussed:'),
    ('btnRegistrarContacto', 'FormGestionarMora', 'Registrar Contacto', 'Register Contact'),
    ('dgvPagos.nro', 'FormGestionarMora', 'Nro Contrato', 'Contract No.'),
    ('dgvPagos.inquilino', 'FormGestionarMora', 'Inquilino', 'Tenant'),
    ('dgvPagos.direccion', 'FormGestionarMora', N'Dirección', 'Address'),
    ('dgvPagos.periodo', 'FormGestionarMora', N'Período', 'Period'),
    ('dgvPagos.montoAdeudado', 'FormGestionarMora', 'Monto Adeudado', 'Amount Owed'),
    ('dgvPagos.diasMora', 'FormGestionarMora', N'Días en Mora', 'Days Overdue'),
    ('dgvContactos.fecha', 'FormGestionarMora', 'Fecha', 'Date'),
    ('dgvContactos.descripcion', 'FormGestionarMora', N'Descripción', 'Description'),
    ('dgvContactos.analista', 'FormGestionarMora', 'Analista', 'Analyst'),
    -- FormGestionarLitigios
    ('grpInquilino', 'FormGestionarLitigios', 'Inquilino', 'Tenant'),
    ('lblNombre', 'FormGestionarLitigios', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormGestionarLitigios', 'Apellido:', 'Last Name:'),
    ('lblDni', 'FormGestionarLitigios', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormGestionarLitigios', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormGestionarLitigios', 'Email:', 'Email:'),
    ('grpContrato', 'FormGestionarLitigios', 'Contrato y deuda', 'Contract and debt'),
    ('lblDireccion', 'FormGestionarLitigios', N'Dirección:', 'Address:'),
    ('lblMontoMensual', 'FormGestionarLitigios', 'Monto mensual:', 'Monthly rent:'),
    ('lblFechaFin', 'FormGestionarLitigios', 'Fin del contrato:', 'Contract end:'),
    ('lblDiasLitigio', 'FormGestionarLitigios', N'Días en litigio:', 'Days in litigation:'),
    ('lblMontoAdeudado', 'FormGestionarLitigios', 'Con recargo:', 'With surcharge:'),
    ('lblDeudaTotal', 'FormGestionarLitigios', 'Deuda en litigio del contrato:', 'Contract debt in litigation:'),
    ('grpAcciones', 'FormGestionarLitigios', 'Acciones legales', 'Legal actions'),
    ('lblFechaAccion', 'FormGestionarLitigios', 'Fecha:', 'Date:'),
    ('lblDescripcion', 'FormGestionarLitigios', N'Descripción:', 'Description:'),
    ('btnIntimacion', 'FormGestionarLitigios', N'Registrar Intimación', 'Register Formal Notice'),
    ('btnResuelto', 'FormGestionarLitigios', 'Marcar Resuelto', 'Mark Resolved'),
    ('btnDesalojo', 'FormGestionarLitigios', 'Registrar Desalojo', 'Register Eviction'),
    ('dgvPagos.nro', 'FormGestionarLitigios', 'Nro Contrato', 'Contract No.'),
    ('dgvPagos.inquilino', 'FormGestionarLitigios', 'Inquilino', 'Tenant'),
    ('dgvPagos.direccion', 'FormGestionarLitigios', N'Dirección', 'Address'),
    ('dgvPagos.periodo', 'FormGestionarLitigios', N'Período', 'Period'),
    ('dgvPagos.montoAdeudado', 'FormGestionarLitigios', 'Monto Adeudado', 'Amount Owed'),
    ('dgvPagos.diasLitigio', 'FormGestionarLitigios', N'Días en Litigio', 'Days in Litigation'),
    ('dgvAcciones.fecha', 'FormGestionarLitigios', 'Fecha', 'Date'),
    ('dgvAcciones.tipo', 'FormGestionarLitigios', 'Tipo', 'Type'),
    ('dgvAcciones.descripcion', 'FormGestionarLitigios', N'Descripción', 'Description'),
    -- FormContratosPorVencer
    ('btnActualizar', 'FormContratosPorVencer', 'Actualizar', 'Refresh'),
    ('grpInquilino', 'FormContratosPorVencer', 'Inquilino', 'Tenant'),
    ('lblNombre', 'FormContratosPorVencer', 'Nombre:', 'First Name:'),
    ('lblApellido', 'FormContratosPorVencer', 'Apellido:', 'Last Name:'),
    ('lblDni', 'FormContratosPorVencer', 'DNI:', 'ID Number:'),
    ('lblTelefono', 'FormContratosPorVencer', N'Teléfono:', 'Phone:'),
    ('lblEmail', 'FormContratosPorVencer', 'Email:', 'Email:'),
    ('grpAcciones', 'FormContratosPorVencer', 'Acciones', 'Actions'),
    ('lblObservacion', 'FormContratosPorVencer', N'Observación de la desocupación (opcional):', 'Move-out note (optional):'),
    ('btnRenovacion', 'FormContratosPorVencer', N'Registrar Renovación', 'Register Renewal'),
    ('btnDesocupacion', 'FormContratosPorVencer', N'Registrar Desocupación', 'Register Move-out'),
    ('grpContrato', 'FormContratosPorVencer', 'Contrato', 'Contract'),
    ('lblNroContrato', 'FormContratosPorVencer', 'Nro. contrato:', 'Contract No.:'),
    ('lblDireccion', 'FormContratosPorVencer', N'Dirección:', 'Address:'),
    ('lblTipo', 'FormContratosPorVencer', 'Tipo:', 'Type:'),
    ('lblFechaInicio', 'FormContratosPorVencer', 'Fecha de inicio:', 'Start date:'),
    ('lblDuracion', 'FormContratosPorVencer', N'Duración:', 'Duration:'),
    ('lblFechaFin', 'FormContratosPorVencer', 'Fecha de fin:', 'End date:'),
    ('lblDiasRestantes', 'FormContratosPorVencer', N'Días restantes:', 'Days left:'),
    ('lblMonto', 'FormContratosPorVencer', 'Monto mensual:', 'Monthly rent:'),
    ('lblEstado', 'FormContratosPorVencer', 'Estado:', 'Status:'),
    ('lblAlerta', 'FormContratosPorVencer', 'Alerta de vencim.:', 'Expiry alert:'),
    ('lblPagoActual', 'FormContratosPorVencer', 'Pago mes actual:', 'Current month:'),
    ('lblDeuda', 'FormContratosPorVencer', 'Deuda:', 'Debt:'),
    ('lblDesocupacion', 'FormContratosPorVencer', N'Desocupación:', 'Move-out:'),
    ('dgvContratos.nro', 'FormContratosPorVencer', 'Nro Contrato', 'Contract No.'),
    ('dgvContratos.inquilino', 'FormContratosPorVencer', 'Inquilino', 'Tenant'),
    ('dgvContratos.direccion', 'FormContratosPorVencer', N'Dirección', 'Address'),
    ('dgvContratos.tipo', 'FormContratosPorVencer', 'Tipo', 'Type'),
    ('dgvContratos.monto', 'FormContratosPorVencer', 'Monto Mensual', 'Monthly Rent'),
    ('dgvContratos.fechaFin', 'FormContratosPorVencer', 'Fecha Fin', 'End Date'),
    ('dgvContratos.diasRestantes', 'FormContratosPorVencer', N'Días Restantes', 'Days Left'),
    ('dgvContratos.estado', 'FormContratosPorVencer', 'Estado Contrato', 'Contract Status'),
    ('dgvContratos.estadoPago', 'FormContratosPorVencer', 'Estado Pago', 'Payment Status'),
    ('dgvContratos.desocupacion', 'FormContratosPorVencer', N'Desocupación', 'Move-out'),
    -- FormRenovarContrato
    ('lblContrato', 'FormRenovarContrato', 'Contrato:', 'Contract:'),
    ('lblVencimientoActual', 'FormRenovarContrato', 'Vencimiento actual:', 'Current end date:'),
    ('lblMontoActual', 'FormRenovarContrato', 'Monto actual:', 'Current rent:'),
    ('lblMeses', 'FormRenovarContrato', N'Nueva duración (meses):', 'New term (months):'),
    ('lblNuevoMonto', 'FormRenovarContrato', 'Nuevo monto mensual:', 'New monthly rent:'),
    ('lblNuevoVencimiento', 'FormRenovarContrato', 'Nuevo vencimiento:', 'New end date:'),
    ('btnConfirmar', 'FormRenovarContrato', 'Confirmar', 'Confirm'),
    ('btnCancelar', 'FormRenovarContrato', 'Cancelar', 'Cancel');

INSERT INTO [dbo].[Control] ([nombre], [formulario])
SELECT n.[nombre], n.[formulario]
FROM @Nuevos n
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Control] c WHERE c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]);

INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto])
SELECT c.[id], 1, n.[es]
FROM @Nuevos n
JOIN [dbo].[Control] c ON c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Traducciones] t WHERE t.[idcontrol] = c.[id] AND t.[ididioma] = 1);

INSERT INTO [dbo].[Traducciones] ([idcontrol], [ididioma], [texto])
SELECT c.[id], 2, n.[en]
FROM @Nuevos n
JOIN [dbo].[Control] c ON c.[nombre] = n.[nombre] AND c.[formulario] = n.[formulario]
WHERE EXISTS (SELECT 1 FROM [dbo].[Idioma] WHERE [id] = 2)
  AND NOT EXISTS (SELECT 1 FROM [dbo].[Traducciones] t WHERE t.[idcontrol] = c.[id] AND t.[ididioma] = 2);
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
