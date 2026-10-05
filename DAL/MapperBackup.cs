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

        // Misma cadena que la aplicación (App.config), pero conectada a master: un backup o restore
        // no puede hacerse estando conectado a la propia base
        public MapperBackup()
        {
            var cadena = new SqlConnectionStringBuilder(Acceso.CadenaConexion);
            dbName = cadena.InitialCatalog;
            if (string.IsNullOrWhiteSpace(dbName))
                throw new InvalidOperationException(
                    $"La cadena de conexión '{Acceso.NOMBRE_CONEXION}' no indica la base de datos (Initial Catalog).");

            cadena.InitialCatalog = "master";
            masterConnectionString = cadena.ConnectionString;
        }

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

            // Absolute fallback: local folder in CommonApplicationData
            string fallbackPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TpIngSoft_Backups");
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

        public void RestaurarDayZero(string scriptPath, string proceduresPath)
        {
            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                conn.Open();

                // Terminate connections if db exists
                string checkDbQuery = $"IF EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}') BEGIN ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{dbName}]; END";
                using (SqlCommand cmd = new SqlCommand(checkDbQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Read and split script by GO to execute batch-by-batch
                EjecutarSqlScript(scriptPath, conn);
                EjecutarSqlScript(proceduresPath, conn);
            }
        }

        private void EjecutarSqlScript(string filePath, SqlConnection conn)
        {
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException("Script not found: " + filePath);

            string scriptContent = System.IO.File.ReadAllText(filePath);
            
            // Normalize line endings and split by GO
            string[] commands = scriptContent.Split(new string[] { "\r\nGO", "\nGO", "GO\r\n", "GO\n", "\r\ngo", "\ngo", "go\r\n", "go\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var cmdText in commands)
            {
                if (string.IsNullOrWhiteSpace(cmdText)) continue;
                using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                {
                    cmd.ExecuteNonQuery();
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
