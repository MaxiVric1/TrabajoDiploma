using System;
using System.Data.SqlClient;

namespace DAL
{
    public class MapperBackup
    {
        //private string masterConnectionString = @"Data Source=NOTE-MAX\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;";
        //private string masterConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=True;";
        //private string dbName = "tpingsoft";
        private readonly string masterConnectionString;
        private readonly string dbName;

        // Misma instancia y base que la aplicación, pero conectada a master: un backup o restore
        // no puede hacerse estando conectado a la propia base. A diferencia de la aplicación, acepta una
        // instancia donde la base todavía no existe (restaurar un backup en una PC nueva).
        public MapperBackup()
        {
            //var cadena = new SqlConnectionStringBuilder(Acceso.CadenaConexion);
            var cadena = new SqlConnectionStringBuilder(ServidorSql.DetectarParaAdministracion());
            dbName = cadena.InitialCatalog;
            if (string.IsNullOrWhiteSpace(dbName))
                throw new InvalidOperationException(
                    $"La cadena de conexión '{Acceso.NOMBRE_CONEXION}' no indica la base de datos (Initial Catalog).");

            cadena.InitialCatalog = "master";
            masterConnectionString = cadena.ConnectionString;
        }

        // C:\ProgramData\LandSmart\Backups
        public static string CarpetaBackupsCompartida =>
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "LandSmart", "Backups");

        public string ObtenerDirectorioDefaultBackup()
        {
            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                conn.Open();

                // Try ServerProperty (SQL Server 2019+)
                string query1 = "SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS NVARCHAR(512))";
                using (SqlCommand cmd = new SqlCommand(query1, conn))
                {
                    try
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value && !string.IsNullOrWhiteSpace(result.ToString()))
                        {
                            return result.ToString();
                        }
                    }
                    catch { }
                }

                // Fallback to registry query
                string query2 = @"
                    DECLARE @BackupDirectory NVARCHAR(512);
                    EXEC master.dbo.xp_instance_regread
                        N'HKEY_LOCAL_MACHINE',
                        N'Software\Microsoft\MSSQLServer\MSSQLServer',
                        N'BackupDirectory',
                        @BackupDirectory OUTPUT;
                    SELECT @BackupDirectory;";
                using (SqlCommand cmd = new SqlCommand(query2, conn))
                {
                    try
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value && !string.IsNullOrWhiteSpace(result.ToString()))
                        {
                            return result.ToString();
                        }
                    }
                    catch { }
                }
            }

            // Absolute fallback: carpeta compartida de LandSmart (el instalador le da permiso de modificación a los usuarios
            // y al servicio de SQL Server)
            string fallbackPath = CarpetaBackupsCompartida;
            if (!System.IO.Directory.Exists(fallbackPath))
            {
                System.IO.Directory.CreateDirectory(fallbackPath);
            }
            return fallbackPath;
        }

        public void RealizarBackup(string path)
        {
            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                conn.Open();
                string query = $"BACKUP DATABASE [{dbName}] TO DISK = @path WITH FORMAT, INIT";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@path", path);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RestaurarBackup(string path)
        {
            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                conn.Open();

                // Put db in single user mode to force disconnect active sessions
                string querySingle = $"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                using (SqlCommand cmd = new SqlCommand(querySingle, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                try
                {
                    string queryRestore = $"RESTORE DATABASE [{dbName}] FROM DISK = @path WITH REPLACE";
                    using (SqlCommand cmd = new SqlCommand(queryRestore, conn))
                    {
                        cmd.Parameters.AddWithValue("@path", path);
                        cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    // Restore db to multi user mode
                    string queryMulti = $"ALTER DATABASE [{dbName}] SET MULTI_USER";
                    using (SqlCommand cmd = new SqlCommand(queryMulti, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // Scripts de Day Zero, en el mismo orden que el instalador
        public static readonly string[] ScriptsDayZero = { "schema-init.sql", "actualizacion-schema.sql", "stored-procedures.sql", "permisos.sql" };

        // Borra la base y la recrea como una instalación nueva: tablas y datos iniciales, actualización del esquema,
        // stored procedures y permisos (rol LandSmartApp para el usuario que ejecuta y los usuarios de la PC).
        // En LocalDB no se ejecuta permisos.sql (la instancia es del usuario, igual que en el instalador).
        // Devuelve los mensajes "AVISO:" de los scripts.
        public System.Collections.Generic.List<string> RestaurarDayZero(string carpetaScripts)
        {
            foreach (string script in ScriptsDayZero)
                if (!System.IO.File.Exists(System.IO.Path.Combine(carpetaScripts, script)))
                    throw new System.IO.FileNotFoundException("No se encontró el script " + script + " en " + carpetaScripts);

            var avisos = new System.Collections.Generic.List<string>();
            bool esLocalDb = new SqlConnectionStringBuilder(masterConnectionString).DataSource.Trim()
                .StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase);
            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                conn.InfoMessage += (s, e) =>
                {
                    foreach (SqlError m in e.Errors)
                        if (m.Message.StartsWith("AVISO", StringComparison.OrdinalIgnoreCase)) avisos.Add(m.Message);
                };
                conn.Open();

                // Terminate connections if db exists
                string checkDbQuery = $"IF EXISTS (SELECT * FROM sys.databases WHERE name = @db) BEGIN ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{dbName}]; END";
                using (SqlCommand cmd = new SqlCommand(checkDbQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@db", dbName);
                    cmd.ExecuteNonQuery();
                }

                foreach (string script in ScriptsDayZero)
                {
                    if (script == "permisos.sql" && esLocalDb) continue;
                    EjecutarSqlScript(System.IO.Path.Combine(carpetaScripts, script), conn);
                }
            }
            return avisos;
        }

        private static readonly System.Text.RegularExpressions.Regex LineaGo = new System.Text.RegularExpressions.Regex(
            @"^[ \t]*GO[ \t]*(?:--.*)?\r?$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);

        // Igual que sqlcmd/DbInstaller: lotes separados por líneas GO, en la misma conexión. La variable $(Usuario)
        // de permisos.sql se reemplaza por SYSTEM_USER (el usuario de Windows que ejecuta BackupCLI).
        private void EjecutarSqlScript(string filePath, SqlConnection conn)
        {
            string scriptContent = System.IO.File.ReadAllText(filePath, System.Text.Encoding.UTF8).Replace("$(Usuario)", "SYSTEM_USER");
            string[] lotes = LineaGo.Split(scriptContent);
            for (int i = 0; i < lotes.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lotes[i])) continue;
                using (SqlCommand cmd = new SqlCommand(lotes[i], conn) { CommandTimeout = 300 })
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        throw new InvalidOperationException($"{System.IO.Path.GetFileName(filePath)}, lote {i + 1}: {ex.Message}", ex);
                    }
                }
            }
        }

        public System.Collections.Generic.List<string> ListarArchivosBackup(string directoryPath)
        {
            var archivos = new System.Collections.Generic.List<string>();
            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                conn.Open();

                string tempTable = @"
                    CREATE TABLE #TempFiles (
                        subdirectory NVARCHAR(512),
                        depth INT,
                        [file] INT
                    );";
                using (SqlCommand cmd = new SqlCommand(tempTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                try
                {
                    string insertExec = "INSERT INTO #TempFiles EXEC master.sys.xp_dirtree @path, 1, 1;";
                    using (SqlCommand cmd = new SqlCommand(insertExec, conn))
                    {
                        cmd.Parameters.AddWithValue("@path", directoryPath);
                        cmd.ExecuteNonQuery();
                    }

                    string selectQuery = "SELECT subdirectory FROM #TempFiles WHERE [file] = 1 AND subdirectory LIKE '%.bak' ORDER BY subdirectory DESC;";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                archivos.Add(reader["subdirectory"].ToString());
                            }
                        }
                    }
                }
                finally
                {
                    using (SqlCommand cmd = new SqlCommand("DROP TABLE #TempFiles;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return archivos;
        }
    }
}
