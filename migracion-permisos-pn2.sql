-- ====================================================
-- Migración: PN2 - Permisos, roles y usuarios por rol
-- Permisos: AccesoPN2AdministradorAlquileres, AccesoPN2AnalistaCobranzas, AccesoPN2AsesorLegal
-- Roles:    "Administrador de Alquileres", "Analista de Cobranzas", "Asesor Legal" (cada uno con su permiso).
--           Los tres pasan a ser hijos del rol Admin (Composite): Admin ve todo el menú PN2.
-- Usuarios: AdministradorAlquileres, AnalistaCobranzas, AsesorLegal (contraseñas en SHA256, como SERVICIOS.Encriptador.Hash)
-- Integridad: DVH de cada usuario nuevo con la fórmula de SERVICIOS.GestorDV.CalcularDVH y DVV de Usuario
--             actualizado sumando esos DVH (no se recalcula desde cero: así no se tapa una alteración previa).
-- Se puede ejecutar más de una vez: lo que ya existe no se toca (un usuario existente conserva su contraseña).
-- ====================================================
USE [TpIngSoft];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;
SET XACT_ABORT ON; -- cualquier error deshace todo: el DVV nunca queda desincronizado
GO

BEGIN TRAN;

-- ====================================================
-- 1. Permisos
-- ====================================================
INSERT INTO [dbo].[Permiso] ([nombre])
SELECT p.[nombre]
FROM (VALUES ('AccesoPN2AdministradorAlquileres'), ('AccesoPN2AnalistaCobranzas'), ('AccesoPN2AsesorLegal')) p([nombre])
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Permiso] x WHERE x.[nombre] = p.[nombre]);

-- ====================================================
-- 2. Roles y su permiso
-- ====================================================
DECLARE @Roles TABLE ([rol] VARCHAR(50), [permiso] VARCHAR(50));
INSERT INTO @Roles VALUES
    ('Administrador de Alquileres', 'AccesoPN2AdministradorAlquileres'),
    ('Analista de Cobranzas',       'AccesoPN2AnalistaCobranzas'),
    ('Asesor Legal',                'AccesoPN2AsesorLegal');

INSERT INTO [dbo].[Rol] ([nombre])
SELECT r.[rol] FROM @Roles r
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Rol] x WHERE x.[nombre] = r.[rol]);

INSERT INTO [dbo].[RolPermiso] ([id_padre], [id_hijo], [tipo_hijo])
SELECT ro.[id], pe.[id], 'Permiso'
FROM @Roles r
JOIN [dbo].[Rol] ro ON ro.[nombre] = r.[rol]
JOIN [dbo].[Permiso] pe ON pe.[nombre] = r.[permiso]
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RolPermiso] x
                  WHERE x.[id_padre] = ro.[id] AND x.[id_hijo] = pe.[id] AND x.[tipo_hijo] = 'Permiso');

-- Admin contiene los tres roles del PN2 (sub-roles del Composite)
INSERT INTO [dbo].[RolPermiso] ([id_padre], [id_hijo], [tipo_hijo])
SELECT ad.[id], ro.[id], 'Rol'
FROM @Roles r
JOIN [dbo].[Rol] ro ON ro.[nombre] = r.[rol]
CROSS JOIN (SELECT [id] FROM [dbo].[Rol] WHERE [nombre] = 'Admin') ad
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[RolPermiso] x
                  WHERE x.[id_padre] = ad.[id] AND x.[id_hijo] = ro.[id] AND x.[tipo_hijo] = 'Rol');

-- ====================================================
-- 3. Usuarios
-- ====================================================
DECLARE @Usuarios TABLE ([nombre] VARCHAR(50), [pass] VARCHAR(100), [nombre_persona] NVARCHAR(100),
                         [apellido] NVARCHAR(100), [rol] VARCHAR(50));
INSERT INTO @Usuarios VALUES
    -- AdministradorAlquileres123
    ('AdministradorAlquileres', '33c547ffd85494ad5f1d53d06c595ca8987bc22c7f925c60cc3a4153c4edd0fd', N'Administrador', N'Alquileres', 'Administrador de Alquileres'),
    -- AnalistaCobranzas123
    ('AnalistaCobranzas',       '7830d1704d6a2d1f2923a53a42156f3a07d12076817b6b6bc9cd52ea966dc6a4', N'Analista', N'Cobranzas', 'Analista de Cobranzas'),
    -- AsesorLegal123
    ('AsesorLegal',             '9ef67c768b149aa820db7778763b505f04c6b35f767eaee7864b451d07a99fa1', N'Asesor', N'Legal', 'Asesor Legal');

DECLARE @Creados TABLE ([id] INT, [nombre] VARCHAR(50));
DECLARE @IdIdioma INT = (SELECT TOP 1 [id] FROM [dbo].[Idioma] WHERE [nombre] = N'Español' ORDER BY [id]);

INSERT INTO [dbo].[Usuario] ([nombre], [nombre_persona], [apellido], [pass], [activo], [intentos_fallidos], [dvh], [id_idioma])
OUTPUT inserted.[id], inserted.[nombre] INTO @Creados
SELECT u.[nombre], u.[nombre_persona], u.[apellido], u.[pass], 1, 0, 0, @IdIdioma
FROM @Usuarios u
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] x WHERE x.[nombre] = u.[nombre]);

INSERT INTO [dbo].[UsuarioRol] ([id_usuario], [id_rol])
SELECT us.[id], ro.[id]
FROM @Usuarios u
JOIN [dbo].[Usuario] us ON us.[nombre] = u.[nombre]
JOIN [dbo].[Rol] ro ON ro.[nombre] = u.[rol]
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[UsuarioRol] x WHERE x.[id_usuario] = us.[id] AND x.[id_rol] = ro.[id]);

-- ====================================================
-- 4. DVH de los usuarios nuevos (misma fórmula que GestorDV.CalcularDVH):
--    suma de UNICODE(carácter) * posición del carácter (1..n) * posición del atributo, sobre
--    1 id, 2 nombre, 3 pass, 4 activo ('1'/'0'), 5 nombre_persona, 6 apellido (NULL = vacío)
-- ====================================================
;WITH Numeros AS (
    SELECT TOP (200) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [i]
    FROM sys.all_objects
)
UPDATE us
SET [dvh] = (
    SELECT ISNULL(SUM(CAST(UNICODE(SUBSTRING(v.[valor], n.[i], 1)) AS BIGINT) * n.[i] * v.[pos]), 0)
    FROM (VALUES
            (CAST(us.[id] AS NVARCHAR(200)), 1),
            (CAST(us.[nombre] AS NVARCHAR(200)), 2),
            (CAST(us.[pass] AS NVARCHAR(200)), 3),
            (CASE WHEN us.[activo] = 1 THEN N'1' ELSE N'0' END, 4),
            (ISNULL(us.[nombre_persona], N''), 5),
            (ISNULL(us.[apellido], N''), 6)
         ) v([valor], [pos])
    JOIN Numeros n ON n.[i] <= DATALENGTH(v.[valor]) / 2 -- DATALENGTH: LEN ignora espacios finales
)
FROM [dbo].[Usuario] us
JOIN @Creados c ON c.[id] = us.[id];

-- Historial de alta, como el seed de admin/user
DECLARE @IdAdmin INT = (SELECT [id] FROM [dbo].[Usuario] WHERE [nombre] = 'admin');
INSERT INTO [dbo].[Usuario_Historial] ([id_usuario], [nombre], [nombre_persona], [apellido], [pass], [activo], [dvh],
                                       [id_usuario_autor], [tipo_operacion])
SELECT us.[id], us.[nombre], us.[nombre_persona], us.[apellido], us.[pass], us.[activo], us.[dvh],
       ISNULL(@IdAdmin, us.[id]), 'INSERT'
FROM [dbo].[Usuario] us
JOIN @Creados c ON c.[id] = us.[id];

-- ====================================================
-- 5. DVV de Usuario: se le suman los DVH nuevos
-- ====================================================
DECLARE @SumaNuevos BIGINT = (SELECT ISNULL(SUM(us.[dvh]), 0) FROM [dbo].[Usuario] us JOIN @Creados c ON c.[id] = us.[id]);

IF EXISTS (SELECT 1 FROM [dbo].[DigitoVerificadorVertical] WHERE [tabla] = 'Usuario')
    UPDATE [dbo].[DigitoVerificadorVertical] SET [dvv] = [dvv] + @SumaNuevos WHERE [tabla] = 'Usuario';
ELSE
    INSERT INTO [dbo].[DigitoVerificadorVertical] ([tabla], [dvv])
    SELECT 'Usuario', ISNULL(SUM([dvh]), 0) FROM [dbo].[Usuario];

COMMIT;

DECLARE @CantidadCreados INT = (SELECT COUNT(*) FROM @Creados);
PRINT 'Usuarios PN2 creados: ' + CAST(@CantidadCreados AS VARCHAR(10));
GO
