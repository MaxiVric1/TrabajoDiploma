using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DbInstaller
{
    // Cliente SQL del instalador de LandSmart (instalador.iss). Reemplaza a sqlcmd, que no siempre está instalado
    // en la PC del cliente: usa ADO.NET (SqlClient) y ServiceController, que vienen con .NET Framework.
    //
    // Uso: DbInstaller <comando> [--opcion valor ...] --log <archivo>
    //   probe           --servers "a|b|c" --db X --out F  Estado de cada instancia (en paralelo): CON_BASE, SIN_BASE,
    //                                                    DETENIDO o NO_RESPONDE. Una línea por servidor, separada por tabs.
    //   check-service   --server S                        Si S es una instancia local con el servicio detenido, lo inicia
    //                                                    y espera hasta 30 s. Instancias remotas y LocalDB: no aplica.
    //   test-connection --server S [--require-admin]      Prueba la conexión; con --require-admin exige sysadmin (para
    //                                                    crear la base, los logins y los permisos).
    //   localdb-start                                     Inicia (y crea si no existe) la instancia MSSQLLocalDB del usuario.
    //   backup          --server S --db X --out F         Copia de seguridad previa (COPY_ONLY); escribe la ruta en F.
    //   restore         --server S --db X --file B        Restaura la copia previa (rollback).
    //   run-script      --server S --file F [--var K=V]   Ejecuta un script dividido en lotes por GO, como SSMS/sqlcmd.
    //   verify          --server S --db X                 Verificación final, conectándose como lo hace la aplicación.
    //   drop-database   --server S --db X                 Borra la base (rollback de una base nueva y desinstalación).
    //   set-config      --file F --server S               Escribe el servidor en la cadena 'TpIngSoft' de un .config.
    // Código de salida: 0 = OK, 1 = error (el detalle queda en el log).
    internal static class Program
    {
        private const string NOMBRE_CONEXION = "TpIngSoft";
        private const string LOCALDB_INSTANCIA = "MSSQLLocalDB";
        private const int SEGUNDOS_CONEXION = 20;
        private const int SEGUNDOS_LOTE = 120;
        private const int SEGUNDOS_SERVICIO = 30;

        private static string _log;

        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Uso: DbInstaller <comando> [--opcion valor ...] --log <archivo>");
                return 1;
            }
            string comando = args[0];
            Dictionary<string, List<string>> opciones = LeerOpciones(args);
            _log = Opcion(opciones, "log", null);
            try
            {
                Log($"> {comando} ({Environment.UserDomainName}\\{Environment.UserName})");
                switch (comando)
                {
                    case "probe": Explorar(opciones); break;
                    case "check-service": VerificarServicio(Requerida(opciones, "server")); break;
                    case "test-connection": ProbarConexion(Requerida(opciones, "server"), opciones.ContainsKey("require-admin")); break;
                    case "localdb-start": IniciarLocalDb(); break;
                    case "backup": Respaldar(Requerida(opciones, "server"), Requerida(opciones, "db"), Requerida(opciones, "out")); break;
                    case "restore": Restaurar(Requerida(opciones, "server"), Requerida(opciones, "db"), Requerida(opciones, "file")); break;
                    case "run-script": EjecutarScript(Requerida(opciones, "server"), Requerida(opciones, "file"), Variables(opciones)); break;
                    case "verify": Verificar(Requerida(opciones, "server"), Requerida(opciones, "db")); break;
                    case "drop-database": BorrarBase(Requerida(opciones, "server"), Requerida(opciones, "db")); break;
                    case "set-config": EscribirConfig(Requerida(opciones, "file"), Requerida(opciones, "server")); break;
                    default: throw new ArgumentException("Comando desconocido: " + comando);
                }
                Log($"< {comando}: OK");
                return 0;
            }
            catch (Exception ex)
            {
                Log($"ERROR en {comando}: {Detalle(ex)}");
                Console.Error.WriteLine(Detalle(ex));
                return 1;
            }
        }

        // ---------- probe ----------

        private static void Explorar(Dictionary<string, List<string>> opciones)
        {
            string baseDatos = Requerida(opciones, "db");
            string[] servidores = Requerida(opciones, "servers").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var estados = new string[servidores.Length];

            // En paralelo: la espera total es la de la instancia más lenta, no la suma de todas
            Parallel.For(0, servidores.Length, i =>
            {
                string servidor = servidores[i];
                try
                {
                    using (SqlConnection conexion = Abrir(servidor, "master"))
                    using (var comando = new SqlCommand("SELECT CASE WHEN DB_ID(@Base) IS NULL THEN 0 ELSE 1 END", conexion))
                    {
                        comando.Parameters.AddWithValue("@Base", baseDatos);
                        bool tieneBase = Convert.ToInt32(comando.ExecuteScalar()) == 1;
                        estados[i] = tieneBase ? "CON_BASE" : "SIN_BASE";
                        Log($"Prueba de {servidor}: responde; base {baseDatos} {(tieneBase ? "EXISTE" : "NO EXISTE")}");
                    }
                }
                catch (Exception ex)
                {
                    string servicio = ServicioDe(servidor);
                    ServiceControllerStatus? estado = servicio == null ? (ServiceControllerStatus?)null : EstadoServicio(servicio);
                    estados[i] = estado == ServiceControllerStatus.Stopped ? "DETENIDO" : "NO_RESPONDE";
                    Log($"Prueba de {servidor}: {estados[i]} (servicio {servicio ?? "-"}: {estado?.ToString() ?? "-"}). {Detalle(ex)}");
                }
            });

            // Solo ASCII: lo lee el script de Inno Setup
            File.WriteAllLines(Requerida(opciones, "out"),
                servidores.Select((s, i) => s + "\t" + estados[i]), Encoding.ASCII);
        }

        // ---------- servicio ----------

        // Servicio de Windows de una instancia local: MSSQLSERVER (predeterminada) o MSSQL$NOMBRE.
        // null para LocalDB (no tiene servicio, se inicia a demanda) y para servidores remotos.
        private static string ServicioDe(string servidor)
        {
            string s = servidor.Trim();
            if (s.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase)) s = s.Substring(4);
            if (s.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase)) return null;
            int coma = s.IndexOf(',');
            if (coma >= 0) s = s.Substring(0, coma);
            string[] partes = s.Split('\\');
            string host = partes[0];
            bool local = host == "." || host.Equals("(local)", StringComparison.OrdinalIgnoreCase) ||
                         host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || host == "127.0.0.1" ||
                         host.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase);
            if (!local) return null;
            string instancia = partes.Length > 1 ? partes[1] : null;
            return string.IsNullOrEmpty(instancia) || instancia.Equals("MSSQLSERVER", StringComparison.OrdinalIgnoreCase)
                ? "MSSQLSERVER"
                : "MSSQL$" + instancia;
        }

        private static ServiceControllerStatus? EstadoServicio(string servicio)
        {
            try
            {
                using (var controlador = new ServiceController(servicio)) return controlador.Status;
            }
            catch (InvalidOperationException)
            {
                return null; // el servicio no existe
            }
        }

        private static void VerificarServicio(string servidor)
        {
            string servicio = ServicioDe(servidor);
            if (servicio == null)
            {
                Log($"{servidor}: instancia remota o LocalDB, no tiene servicio local que verificar.");
                return;
            }
            using (var controlador = new ServiceController(servicio))
            {
                ServiceControllerStatus estado;
                try { estado = controlador.Status; }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException($"No existe el servicio {servicio}: la instancia {servidor} no está instalada en esta PC.");
                }
                Log($"Servicio {servicio}: {estado}");
                if (estado == ServiceControllerStatus.Running) return;

                if (estado == ServiceControllerStatus.Stopped || estado == ServiceControllerStatus.Paused)
                {
                    Log($"Iniciando el servicio {servicio}...");
                    try
                    {
                        if (estado == ServiceControllerStatus.Paused) controlador.Continue();
                        else controlador.Start();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Típico: servicio deshabilitado o sin permisos para iniciarlo
                        throw new InvalidOperationException(
                            $"No se pudo iniciar el servicio {servicio} (¿está deshabilitado o falta permiso de administrador?): {Detalle(ex)}");
                    }
                }
                try
                {
                    controlador.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(SEGUNDOS_SERVICIO));
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    controlador.Refresh();
                    throw new InvalidOperationException(
                        $"El servicio {servicio} no se inició en {SEGUNDOS_SERVICIO} segundos (estado: {controlador.Status}).");
                }
                Log($"Servicio {servicio}: iniciado.");
            }
        }

        // ---------- conexión ----------

        private static SqlConnection Abrir(string servidor, string baseDatos)
        {
            var cadena = new SqlConnectionStringBuilder
            {
                DataSource = servidor,
                InitialCatalog = baseDatos,
                IntegratedSecurity = true,
                ConnectTimeout = SEGUNDOS_CONEXION,
                Pooling = false
            };
            var conexion = new SqlConnection(cadena.ConnectionString);
            conexion.InfoMessage += (s, e) => { foreach (SqlError m in e.Errors) Log("  " + m.Message); };
            conexion.Open();
            return conexion;
        }

        private static void ProbarConexion(string servidor, bool requiereAdmin)
        {
            using (SqlConnection conexion = Abrir(servidor, "master"))
            using (var comando = new SqlCommand(
                "SELECT CAST(SERVERPROPERTY('ProductVersion') AS NVARCHAR(128)), CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128)), " +
                "SYSTEM_USER, IS_SRVROLEMEMBER('sysadmin')", conexion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                lector.Read();
                string usuario = lector.GetString(2);
                bool sysadmin = !lector.IsDBNull(3) && lector.GetInt32(3) == 1;
                Log($"Conectado a {servidor}: SQL Server {lector.GetString(0)} ({lector.GetString(1)}) como {usuario}; sysadmin: {(sysadmin ? "SÍ" : "NO")}");
                if (requiereAdmin && !sysadmin)
                    throw new InvalidOperationException(
                        $"La cuenta {usuario} se conecta a {servidor} pero no es administradora (sysadmin) de SQL Server: " +
                        "no puede crear la base de datos ni otorgar los permisos. Ejecute el instalador con una cuenta " +
                        "que sea administradora de esa instancia.");
            }
        }

        // ---------- LocalDB ----------

        private static void IniciarLocalDb()
        {
            string ejecutable = SqlLocalDbExe();
            // "start" crea la instancia automática MSSQLLocalDB si todavía no existe para este usuario
            var proceso = new ProcessStartInfo(ejecutable, "start " + LOCALDB_INSTANCIA)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using (Process p = Process.Start(proceso))
            {
                string salida = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
                p.WaitForExit();
                Log($"{ejecutable} start {LOCALDB_INSTANCIA}: código {p.ExitCode}. {salida.Trim()}");
                if (p.ExitCode != 0)
                    throw new InvalidOperationException($"No se pudo iniciar LocalDB ({LOCALDB_INSTANCIA}): {salida.Trim()}");
            }
        }

        // SqlLocalDB.exe de la versión más nueva instalada (Program Files\Microsoft SQL Server\<ver>\Tools\Binn)
        private static string SqlLocalDbExe()
        {
            string raiz = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SQL Server");
            string encontrado = Directory.Exists(raiz)
                ? Directory.GetDirectories(raiz)
                    .Select(d => Path.Combine(d, "Tools", "Binn", "SqlLocalDB.exe"))
                    .Where(File.Exists)
                    .OrderByDescending(f => int.TryParse(Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(f)))), out int v) ? v : 0)
                    .FirstOrDefault()
                : null;
            if (encontrado == null)
                throw new FileNotFoundException("No se encontró SqlLocalDB.exe: SQL Server Express LocalDB no está instalado.");
            return encontrado;
        }

        // ---------- backup / restore / drop ----------

        private static void Respaldar(string servidor, string baseDatos, string archivoSalida)
        {
            using (SqlConnection conexion = Abrir(servidor, "master"))
            {
                string carpeta;
                using (var comando = new SqlCommand("SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS NVARCHAR(512))", conexion))
                    carpeta = comando.ExecuteScalar() as string;
                if (string.IsNullOrWhiteSpace(carpeta))
                {
                    // LocalDB no tiene carpeta de backups: se usa la de los archivos de la base (SQL Server puede escribir ahí)
                    using (var comando = new SqlCommand(
                        "SELECT TOP 1 physical_name FROM sys.master_files WHERE database_id = DB_ID(@Base) AND type = 0", conexion))
                    {
                        comando.Parameters.AddWithValue("@Base", baseDatos);
                        string datos = comando.ExecuteScalar() as string;
                        carpeta = datos == null ? null : Path.GetDirectoryName(datos);
                    }
                }
                if (string.IsNullOrWhiteSpace(carpeta))
                    throw new InvalidOperationException("No se pudo determinar una carpeta para la copia de seguridad previa.");

                string ruta = Path.Combine(carpeta, $"{baseDatos}_previo_instalacion_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
                Log($"Copia de seguridad previa de {baseDatos} en {ruta}...");
                using (var comando = new SqlCommand($"BACKUP DATABASE {Corchetes(baseDatos)} TO DISK = @Ruta WITH COPY_ONLY, INIT", conexion))
                {
                    comando.CommandTimeout = 0;
                    comando.Parameters.AddWithValue("@Ruta", ruta);
                    comando.ExecuteNonQuery();
                }
                File.WriteAllText(archivoSalida, ruta, Encoding.Default);
            }
        }

        private static void Restaurar(string servidor, string baseDatos, string archivo)
        {
            using (SqlConnection conexion = Abrir(servidor, "master"))
            {
                Log($"Restaurando {baseDatos} desde {archivo}...");
                Ejecutar(conexion, $"IF DB_ID(@Base) IS NOT NULL ALTER DATABASE {Corchetes(baseDatos)} SET SINGLE_USER WITH ROLLBACK IMMEDIATE",
                         "@Base", baseDatos);
                Ejecutar(conexion, $"RESTORE DATABASE {Corchetes(baseDatos)} FROM DISK = @Ruta WITH REPLACE", "@Ruta", archivo);
                Ejecutar(conexion, $"ALTER DATABASE {Corchetes(baseDatos)} SET MULTI_USER");
            }
        }

        private static void BorrarBase(string servidor, string baseDatos)
        {
            using (SqlConnection conexion = Abrir(servidor, "master"))
            {
                List<string> logins = LoginsCreados(conexion, baseDatos);
                Ejecutar(conexion,
                    $"IF DB_ID(@Base) IS NOT NULL BEGIN " +
                    $"ALTER DATABASE {Corchetes(baseDatos)} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                    $"DROP DATABASE {Corchetes(baseDatos)}; END", "@Base", baseDatos);
                Log($"Base {baseDatos} borrada de {servidor}.");
                foreach (string login in logins)
                    BorrarLoginSinUso(conexion, login);
            }
        }

        // Logins que creó permisos.sql (anotados en la base como propiedades extendidas LandSmart.LoginCreado.N)
        private static List<string> LoginsCreados(SqlConnection conexion, string baseDatos)
        {
            var logins = new List<string>();
            using (var comando = new SqlCommand("SELECT DB_ID(@Base)", conexion))
            {
                comando.Parameters.AddWithValue("@Base", baseDatos);
                if (comando.ExecuteScalar() is DBNull) return logins;
            }
            using (var comando = new SqlCommand(
                $"SELECT CAST([value] AS NVARCHAR(128)) FROM {Corchetes(baseDatos)}.sys.extended_properties " +
                "WHERE [class] = 0 AND [name] LIKE N'LandSmart.LoginCreado.%'", conexion))
            using (SqlDataReader lector = comando.ExecuteReader())
                while (lector.Read())
                    if (!lector.IsDBNull(0)) logins.Add(lector.GetString(0));
            Log(logins.Count == 0
                ? "La base no tiene logins anotados por permisos.sql (no se borra ningún login)."
                : "Logins creados por permisos.sql: " + string.Join(", ", logins));
            return logins;
        }

        // Borra el login si ninguna otra base lo usa, no es el login actual, no es dueño de ninguna base, no tiene roles
        // de servidor ni permisos de servidor además de CONNECT SQL (es decir, si solo existía para esta aplicación)
        private static void BorrarLoginSinUso(SqlConnection conexion, string login)
        {
            const string sql = @"
DECLARE @Sid VARBINARY(85) = SUSER_SID(@Login), @Motivo NVARCHAR(400) = NULL, @Sql NVARCHAR(MAX) = N'', @Base SYSNAME;
IF @Sid IS NULL
    SET @Motivo = N'ya no existe';
ELSE IF @Sid = SUSER_SID()
    SET @Motivo = N'es el login con el que se está desinstalando';
ELSE IF EXISTS (SELECT 1 FROM sys.databases WHERE owner_sid = @Sid)
    SET @Motivo = N'es dueño de una base de datos';
ELSE IF EXISTS (SELECT 1 FROM sys.server_role_members rm JOIN sys.server_principals p ON p.principal_id = rm.member_principal_id
                WHERE p.[sid] = @Sid)
    SET @Motivo = N'pertenece a un rol de servidor';
ELSE IF EXISTS (SELECT 1 FROM sys.server_permissions sp JOIN sys.server_principals p ON p.principal_id = sp.grantee_principal_id
                WHERE p.[sid] = @Sid AND sp.[permission_name] <> N'CONNECT SQL')
    SET @Motivo = N'tiene permisos de servidor';
ELSE
BEGIN
    DECLARE @Usos TABLE ([base] SYSNAME);
    DECLARE bases CURSOR LOCAL FAST_FORWARD FOR
        SELECT [name] FROM sys.databases WHERE state_desc = N'ONLINE' AND HAS_DBACCESS([name]) = 1;
    OPEN bases;
    FETCH NEXT FROM bases INTO @Base;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Sql = N'SELECT @B FROM ' + QUOTENAME(@Base) + N'.sys.database_principals WHERE [sid] = @S';
        INSERT INTO @Usos EXEC sp_executesql @Sql, N'@B SYSNAME, @S VARBINARY(85)', @Base, @Sid;
        FETCH NEXT FROM bases INTO @Base;
    END
    CLOSE bases;
    DEALLOCATE bases;
    SELECT @Motivo = N'lo usa la base ' + MIN([base]) FROM @Usos HAVING COUNT(*) > 0;
END
IF @Motivo IS NULL
BEGIN
    SET @Sql = N'DROP LOGIN ' + QUOTENAME(@Login);
    EXEC (@Sql);
END
SELECT @Motivo;";
            try
            {
                using (var comando = new SqlCommand(sql, conexion) { CommandTimeout = SEGUNDOS_LOTE })
                {
                    comando.Parameters.AddWithValue("@Login", login);
                    object motivo = comando.ExecuteScalar();
                    Log(motivo is string texto ? $"Login {login}: se conserva ({texto})." : $"Login {login}: borrado.");
                }
            }
            catch (SqlException ex)
            {
                // La base ya se borró: un login que no se pudo borrar no es un error de la desinstalación
                Log($"AVISO: no se pudo borrar el login {login}: {Detalle(ex)}");
            }
        }

        private static void Ejecutar(SqlConnection conexion, string sql, string parametro = null, string valor = null)
        {
            using (var comando = new SqlCommand(sql, conexion) { CommandTimeout = 0 })
            {
                if (parametro != null) comando.Parameters.AddWithValue(parametro, valor);
                comando.ExecuteNonQuery();
            }
        }

        private static string Corchetes(string nombre) => "[" + nombre.Replace("]", "]]") + "]";

        // ---------- scripts ----------

        private static readonly Regex LineaGo = new Regex(@"^[ \t]*GO[ \t]*(?:--.*)?\r?$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex VariableSqlcmd = new Regex(@"\$\((\w+)\)");

        // Igual que SSMS/sqlcmd: el script (UTF-8) se divide en lotes en cada línea GO y se ejecutan en orden sobre la
        // misma conexión (así un USE vale para los lotes siguientes). Las variables $(Nombre) se reemplazan por --var.
        private static void EjecutarScript(string servidor, string archivo, Dictionary<string, string> variables)
        {
            string contenido = File.ReadAllText(archivo, Encoding.UTF8);
            contenido = VariableSqlcmd.Replace(contenido, m =>
            {
                if (variables.TryGetValue(m.Groups[1].Value, out string valor)) return valor.Replace("'", "''");
                throw new InvalidOperationException($"El script usa la variable $({m.Groups[1].Value}) y no se indicó con --var.");
            });

            string[] lotes = LineaGo.Split(contenido).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            Log($"Ejecutando {Path.GetFileName(archivo)} en {servidor} ({lotes.Length} lotes)...");
            using (SqlConnection conexion = Abrir(servidor, "master"))
            {
                int numero = 0;
                foreach (string lote in lotes)
                {
                    numero++;
                    using (var comando = new SqlCommand(lote, conexion) { CommandTimeout = SEGUNDOS_LOTE })
                    {
                        try
                        {
                            comando.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            string inicio = lote.Trim();
                            if (inicio.Length > 200) inicio = inicio.Substring(0, 200) + "...";
                            throw new InvalidOperationException(
                                $"{Path.GetFileName(archivo)}, lote {numero} de {lotes.Length}: {Detalle(ex)}{Environment.NewLine}Lote: {inicio}");
                        }
                    }
                }
            }
            Log($"{Path.GetFileName(archivo)}: ejecutado completo.");
        }

        // ---------- verificación final ----------

        // Se conecta a la base igual que la aplicación (seguridad integrada, con el usuario que la va a usar) y comprueba
        // el permiso de ejecutar los stored procedures y que el administrador inicial exista, usando el mismo SP del login.
        private static void Verificar(string servidor, string baseDatos)
        {
            using (SqlConnection conexion = Abrir(servidor, baseDatos))
            {
                using (var comando = new SqlCommand("SELECT HAS_PERMS_BY_NAME('dbo', 'SCHEMA', 'EXECUTE')", conexion))
                    if (Convert.ToInt32(comando.ExecuteScalar()) != 1)
                        throw new InvalidOperationException($"El usuario {Environment.UserDomainName}\\{Environment.UserName} no tiene permiso para ejecutar los stored procedures de {baseDatos}.");

                using (var comando = new SqlCommand("dbo.BuscarUsuarioPorNombre", conexion) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    comando.Parameters.AddWithValue("@Nombre", "admin");
                    using (SqlDataReader lector = comando.ExecuteReader())
                        if (!lector.Read())
                            throw new InvalidOperationException($"La base {baseDatos} no tiene el usuario administrador inicial (admin).");
                }
            }
            Log($"Verificación final OK: {Environment.UserDomainName}\\{Environment.UserName} accede a {baseDatos} en {servidor} y existe el usuario admin.");
        }

        // ---------- configuración de la aplicación ----------

        private static void EscribirConfig(string archivo, string servidor)
        {
            var documento = new XmlDocument { PreserveWhitespace = true };
            documento.Load(archivo);
            var nodo = documento.SelectSingleNode($"/configuration/connectionStrings/add[@name='{NOMBRE_CONEXION}']") as XmlElement;
            if (nodo == null)
                throw new InvalidOperationException($"{archivo} no tiene la cadena de conexión '{NOMBRE_CONEXION}'.");
            var cadena = new SqlConnectionStringBuilder(nodo.GetAttribute("connectionString")) { DataSource = servidor };
            nodo.SetAttribute("connectionString", cadena.ConnectionString);
            documento.Save(archivo);
            Log($"{Path.GetFileName(archivo)}: servidor configurado {servidor}.");
        }

        // ---------- utilidades ----------

        private static Dictionary<string, List<string>> LeerOpciones(string[] args)
        {
            var opciones = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            for (int i = 1; i < args.Length; i++)
            {
                if (!args[i].StartsWith("--")) continue;
                string nombre = args[i].Substring(2);
                string valor = i + 1 < args.Length && !args[i + 1].StartsWith("--") ? args[++i] : "";
                if (!opciones.TryGetValue(nombre, out List<string> valores))
                    opciones[nombre] = valores = new List<string>();
                valores.Add(valor);
            }
            return opciones;
        }

        private static string Opcion(Dictionary<string, List<string>> opciones, string nombre, string porDefecto) =>
            opciones.TryGetValue(nombre, out List<string> valores) ? valores[0] : porDefecto;

        private static string Requerida(Dictionary<string, List<string>> opciones, string nombre)
        {
            string valor = Opcion(opciones, nombre, null);
            if (string.IsNullOrWhiteSpace(valor)) throw new ArgumentException($"Falta la opción --{nombre}.");
            return valor;
        }

        private static Dictionary<string, string> Variables(Dictionary<string, List<string>> opciones)
        {
            var variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (opciones.TryGetValue("var", out List<string> valores))
                foreach (string v in valores)
                {
                    int igual = v.IndexOf('=');
                    if (igual > 0) variables[v.Substring(0, igual)] = v.Substring(igual + 1);
                }
            return variables;
        }

        private static string Detalle(Exception ex)
        {
            if (ex is SqlException sql)
                return $"[error SQL {sql.Number}, severidad {sql.Class}] {sql.Message}";
            return ex.Message;
        }

        private static readonly object _bloqueoLog = new object();

        // En la página de códigos ANSI de Windows: el log también lo escribe y lo muestra el script de Inno Setup
        private static void Log(string mensaje)
        {
            string linea = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [DbInstaller] {mensaje}";
            Console.WriteLine(linea);
            if (_log == null) return;
            lock (_bloqueoLog)
            {
                try { File.AppendAllText(_log, linea + Environment.NewLine, Encoding.Default); }
                catch (IOException) { /* el log no debe frenar la instalación */ }
            }
        }
    }
}
