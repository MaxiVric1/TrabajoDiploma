-- ====================================================
-- TpIngSoft - Permisos de Windows para usar la aplicación
-- Da acceso (db_owner sobre TpIngSoft) a:
--   - el usuario de Windows indicado (el que instaló la aplicación), y
--   - el grupo local de usuarios de Windows (BUILTIN\Users), para que cualquier usuario de la PC pueda usarla.
-- Para cada uno: crea el login si no existe, crea su usuario en la base y lo agrega a db_owner.
-- Se puede ejecutar más de una vez (no cambia nada si ya tienen acceso).
-- Uso (como administrador de SQL Server):
--   sqlcmd -S INSTANCIA -E -b -v Usuario="DOMINIO\usuario" -i permisos.sql
--   Con Usuario="SYSTEM_USER" se usa el login que ejecuta el script.
-- Ejecutar después de schema-init.sql.
-- ====================================================
USE [TpIngSoft];
GO

SET NOCOUNT ON;

BEGIN TRY
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

    DECLARE @Orden INT = 1, @Login SYSNAME, @EnBase SYSNAME, @Sql NVARCHAR(MAX);
    WHILE @Orden <= 2
    BEGIN
        SELECT @Login = [login] FROM @Logins WHERE [orden] = @Orden;
        SET @EnBase = NULL;

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

        -- 3. db_owner ("dbo", el dueño de la base, ya tiene todos los permisos y no se puede agregar a roles)
        IF @EnBase = N'dbo'
            PRINT N'Es el dueño de la base (dbo): ya tiene todos los permisos.';
        ELSE IF IS_ROLEMEMBER(N'db_owner', @EnBase) = 1
            PRINT N'Ya era miembro de db_owner: ' + @EnBase;
        ELSE
        BEGIN
            SET @Sql = N'ALTER ROLE [db_owner] ADD MEMBER ' + QUOTENAME(@EnBase) + N';';
            EXEC (@Sql);
            PRINT N'Agregado a db_owner: ' + @EnBase;
        END

        SET @Orden = @Orden + 1;
    END
END TRY
BEGIN CATCH
    -- Corta en el primer error (p. ej. cuenta de Windows inexistente) y lo devuelve a sqlcmd (-b: código de salida 1)
    THROW;
END CATCH
GO
