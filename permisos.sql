-- ====================================================
-- TpIngSoft - Permisos de Windows para usar la aplicación (permisos mínimos)
-- La aplicación solo ejecuta stored procedures (sin SQL dinámico): alcanza con EXECUTE sobre el esquema dbo.
-- Los SPs acceden a las tablas por encadenamiento de propiedad (mismo dueño, dbo), sin permisos sobre las tablas.
--
-- Crea el rol de base de datos LandSmartApp (EXECUTE sobre dbo) y le agrega:
--   - el usuario de Windows indicado (el que instaló la aplicación), y
--   - el grupo local de usuarios de Windows (BUILTIN\Users), para que cualquier usuario de la PC pueda usarla.
-- Para cada uno: crea el login si no existe, crea su usuario en la base y lo agrega al rol.
-- Si una versión anterior de este script los había agregado a db_owner, los quita (permiso excesivo).
-- Si no se puede dar acceso al usuario individual (p. ej. cuenta de Azure AD o Microsoft) se avisa y se sigue:
-- el script solo falla si no puede dar acceso al grupo de usuarios de la PC.
-- No otorga sysadmin ni db_owner. Se puede ejecutar más de una vez.
-- Uso (como administrador de SQL Server):
--   sqlcmd -S INSTANCIA -E -b -v Usuario="DOMINIO\usuario" -i permisos.sql
--   Con Usuario="SYSTEM_USER" se usa el login que ejecuta el script.
-- Ejecutar después de schema-init.sql y stored-procedures.sql.
-- ====================================================
USE [TpIngSoft];
GO

SET NOCOUNT ON;

BEGIN TRY
    -- Rol de la aplicación
    IF DATABASE_PRINCIPAL_ID(N'LandSmartApp') IS NULL
    BEGIN
        CREATE ROLE [LandSmartApp];
        PRINT N'Rol creado: LandSmartApp';
    END
    GRANT EXECUTE ON SCHEMA::[dbo] TO [LandSmartApp];
    PRINT N'LandSmartApp: EXECUTE sobre el esquema dbo.';

    DECLARE @Logins TABLE ([orden] INT, [login] SYSNAME);
    DECLARE @Usuario SYSNAME = LTRIM(RTRIM(N'$(Usuario)'));
    IF @Usuario = N'' OR @Usuario = N'SYSTEM_USER'
        SET @Usuario = SYSTEM_USER;
    INSERT INTO @Logins VALUES (1, @Usuario);

    -- El grupo se busca por su SID fijo (S-1-5-32-545): el nombre depende del idioma de Windows
    -- ("BUILTIN\Users" en inglés, "BUILTIN\Usuarios" en español) y CREATE LOGIN necesita el nombre local
    DECLARE @Grupo SYSNAME = SUSER_SNAME(0x01020000000000052000000021020000);
    IF @Grupo IS NULL
        THROW 50000, N'No se pudo resolver el grupo local de usuarios de Windows (S-1-5-32-545).', 1;
    INSERT INTO @Logins VALUES (2, @Grupo);

    DECLARE @Orden INT = 1, @Login SYSNAME, @EnBase SYSNAME, @Sql NVARCHAR(MAX), @Error NVARCHAR(4000);
    WHILE @Orden <= 2
    BEGIN
        SELECT @Login = [login] FROM @Logins WHERE [orden] = @Orden;
        SET @EnBase = NULL;

      -- Cada cuenta por separado: si falla la del usuario que instaló (p. ej. una cuenta de Azure AD o Microsoft,
      -- para la que SQL Server no puede crear un login), se sigue con el grupo, que cubre a todos los usuarios de la PC
      BEGIN TRY
        -- 1. Login de Windows
        IF SUSER_ID(@Login) IS NULL
        BEGIN
            SET @Sql = N'CREATE LOGIN ' + QUOTENAME(@Login) + N' FROM WINDOWS WITH DEFAULT_DATABASE = [TpIngSoft];';
            EXEC (@Sql);
            PRINT N'Login creado: ' + @Login;
        END
        ELSE
            PRINT N'El login ya existía: ' + @Login;

        -- 2. Usuario en la base (se busca por SID: puede existir con otro nombre, p. ej. "dbo" si es el dueño de la base)
        SELECT @EnBase = [name] FROM sys.database_principals WHERE [sid] = SUSER_SID(@Login);
        IF @EnBase IS NULL
        BEGIN
            SET @Sql = N'CREATE USER ' + QUOTENAME(@Login) + N' FOR LOGIN ' + QUOTENAME(@Login) + N';';
            EXEC (@Sql);
            SET @EnBase = @Login;
            PRINT N'Usuario creado en TpIngSoft: ' + @EnBase;
        END

        IF @EnBase = N'dbo'
            -- El dueño de la base ya tiene todos los permisos y no se puede agregar a roles
            PRINT N'Es el dueño de la base (dbo): ya tiene todos los permisos.';
        ELSE
        BEGIN
            -- 3. Rol de la aplicación
            IF IS_ROLEMEMBER(N'LandSmartApp', @EnBase) = 1
                PRINT N'Ya era miembro de LandSmartApp: ' + @EnBase;
            ELSE
            BEGIN
                SET @Sql = N'ALTER ROLE [LandSmartApp] ADD MEMBER ' + QUOTENAME(@EnBase) + N';';
                EXEC (@Sql);
                PRINT N'Agregado a LandSmartApp: ' + @EnBase;
            END

            -- 4. Quitar db_owner otorgado por la versión anterior de este script
            IF EXISTS (SELECT 1 FROM sys.database_role_members rm
                       JOIN sys.database_principals r ON r.principal_id = rm.role_principal_id AND r.[name] = N'db_owner'
                       JOIN sys.database_principals m ON m.principal_id = rm.member_principal_id AND m.[name] = @EnBase)
            BEGIN
                SET @Sql = N'ALTER ROLE [db_owner] DROP MEMBER ' + QUOTENAME(@EnBase) + N';';
                EXEC (@Sql);
                PRINT N'Quitado de db_owner (permiso excesivo): ' + @EnBase;
            END
        END
      END TRY
      BEGIN CATCH
        SET @Error = N'[error ' + CAST(ERROR_NUMBER() AS NVARCHAR(10)) + N'] ' + ERROR_MESSAGE();
        IF @Orden = 1
            -- Usuario individual: aviso y se continúa (el grupo le da acceso igual si pertenece a él)
            PRINT N'AVISO: no se pudo dar acceso individual a ' + @Login + N' ' + @Error +
                  N' Se continúa con el grupo de usuarios de la PC.';
        ELSE
        BEGIN
            -- Sin el grupo, los usuarios de la PC no pueden usar la aplicación: es un error
            SET @Error = N'No se pudo dar acceso al grupo de usuarios de la PC (' + @Login + N'): ' + @Error;
            THROW 50001, @Error, 1;
        END
      END CATCH

        SET @Orden = @Orden + 1;
    END
END TRY
BEGIN CATCH
    -- Error en el rol o en el grupo: se devuelve a sqlcmd (-b: código de salida 1) con el mensaje completo
    PRINT N'ERROR: [error ' + CAST(ERROR_NUMBER() AS NVARCHAR(10)) + N'] ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO
