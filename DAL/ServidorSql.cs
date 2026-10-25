using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Win32;

namespace DAL
{
    // Detección automática de la instancia local de SQL Server, para que la aplicación funcione en cualquier PC
    // sin configurar el nombre de la instancia:
    //   1. Prueba primero el servidor del .config (lo escribe el instalador con la instancia elegida) y después las
    //      instancias instaladas según el registro (vistas de 64 y 32 bits).
    //   2. Prueba conectarse a cada una con el usuario de Windows actual (timeout corto) y averigua si la base existe
    //      y si ese usuario tiene acceso a ella.
    //   3. La aplicación usa la instancia que tiene la base con acceso. Si no hay ninguna, informa qué pasó en cada
    //      instancia (no responde, base inexistente, usuario sin permiso), en lugar del error genérico de SQL Server.
    //   4. Guarda el resultado: la búsqueda se hace una sola vez por ejecución.
    // De App.config se toman la base y la seguridad; la detección solo reemplaza el servidor (Data Source).
    // Todo queda registrado en LogConexion.Ruta.
    internal static class ServidorSql
    {
        // Nombre de la cadena en <connectionStrings> del .config del ejecutable (TpIngSoft.exe.config, BackupCLI.exe.config)
        internal const string NOMBRE_CONEXION = "TpIngSoft";

        // Mismo nombre que crean schema-init.sql y los demás scripts (importa si el servidor distingue mayúsculas)
        internal const string BASE_DATOS = "TpIngSoft";

        // Si App.config no tiene la cadena, se usa esta como base (el servidor lo pone la detección)
        //private const string CADENA_POR_DEFECTO = "Initial Catalog=tpingsoft;Integrated Security=True;";
        private const string CADENA_POR_DEFECTO = "Initial Catalog=" + BASE_DATOS + ";Integrated Security=True;";
        private const string CLAVE_INSTANCIAS = @"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL";
        private const string INSTANCIA_POR_DEFECTO = "MSSQLSERVER";
        // En PCs lentas (p. ej. de laboratorio) solo el inicio del protocolo puede tardar ~3 s: se deja margen.
        // Las instancias se prueban en paralelo, así que la espera total es la de la más lenta, no la suma.
        private const int SEGUNDOS_PRUEBA = 15;

        private sealed class Candidato
        {
            public string Servidor;
            public bool DelConfig;
            public bool Responde;
            public bool TieneBase;
            public bool TieneAcceso;
            public string Error;
        }

        private sealed class Exploracion
        {
            public SqlConnectionStringBuilder Plantilla;
            public string BaseDatos;
            public List<Candidato> Candidatos;
        }

        private static readonly object _bloqueo = new object();
        private static Exploracion _exploracion;
        private static string _cadena;              // elegida para la aplicación
        private static string _cadenaAdministracion; // elegida para backup / restore

        // Aplicación: instancia que tiene la base y a la que el usuario de Windows actual tiene acceso.
        public static string Detectar()
        {
            if (_cadena != null) return _cadena;
            lock (_bloqueo)
            {
                if (_cadena != null) return _cadena;
                Exploracion exploracion = Explorar();
                Candidato elegido = exploracion.Candidatos.FirstOrDefault(c => c.TieneBase && c.TieneAcceso);
                if (elegido == null)
                    throw Fallar(exploracion, paraAdministracion: false);
                _cadena = Elegir(exploracion, elegido, "aplicación");
                return _cadena;
            }
        }

        // Backup / restore (BackupCLI): la instancia que tiene la base o, si ninguna la tiene (p. ej. para restaurar
        // un backup en una PC nueva), la primera que responde.
        public static string DetectarParaAdministracion()
        {
            if (_cadenaAdministracion != null) return _cadenaAdministracion;
            lock (_bloqueo)
            {
                if (_cadenaAdministracion != null) return _cadenaAdministracion;
                Exploracion exploracion = Explorar();
                Candidato elegido = exploracion.Candidatos.FirstOrDefault(c => c.TieneBase)
                                    ?? exploracion.Candidatos.FirstOrDefault(c => c.Responde);
                if (elegido == null)
                    throw Fallar(exploracion, paraAdministracion: true);
                _cadenaAdministracion = Elegir(exploracion, elegido, "administración (backup / restore)");
                return _cadenaAdministracion;
            }
        }

        // Prueba todos los candidatos una vez. Si ninguno respondió no se guarda, para reintentar en el próximo uso.
        private static Exploracion Explorar()
        {
            if (_exploracion != null) return _exploracion;

            string delConfig = CadenaDelConfig();
            var plantilla = new SqlConnectionStringBuilder(delConfig ?? CADENA_POR_DEFECTO);
            var exploracion = new Exploracion
            {
                Plantilla = plantilla,
                BaseDatos = string.IsNullOrWhiteSpace(plantilla.InitialCatalog) ? BASE_DATOS : plantilla.InitialCatalog,
                Candidatos = new List<Candidato>()
            };

            LogConexion.Escribir("==== Detección de SQL Server ====");
            LogConexion.Escribir($"Ejecutable: {AppDomain.CurrentDomain.FriendlyName} | Usuario de Windows: {LogConexion.UsuarioWindows}");
            LogConexion.Escribir(delConfig != null
                ? $"Cadena del .config ('{NOMBRE_CONEXION}'): {LogConexion.SinDatosSensibles(delConfig)}"
                : $"El .config no tiene la cadena '{NOMBRE_CONEXION}': se usa la predeterminada ({CADENA_POR_DEFECTO})");
            LogConexion.Escribir($"Base de datos buscada: {exploracion.BaseDatos}");

            // Candidatos: primero el servidor del .config (el que eligió el instalador) y después las demás instancias
            // del registro, por si la base se movió de instancia
            string servidorConfig = delConfig != null ? plantilla.DataSource : null;
            if (!string.IsNullOrWhiteSpace(servidorConfig))
                exploracion.Candidatos.Add(new Candidato { Servidor = servidorConfig, DelConfig = true });
            List<string> instancias = InstanciasInstaladas();
            LogConexion.Escribir("Instancias en el registro: " + (instancias.Count == 0 ? "(ninguna)" : string.Join(", ", instancias)));
            foreach (string servidor in instancias)
                if (!exploracion.Candidatos.Any(c => c.Servidor.Equals(servidor, StringComparison.OrdinalIgnoreCase)))
                    exploracion.Candidatos.Add(new Candidato { Servidor = servidor });

            System.Threading.Tasks.Parallel.ForEach(exploracion.Candidatos,
                candidato => Probar(plantilla, exploracion.BaseDatos, candidato));
            foreach (Candidato candidato in exploracion.Candidatos)
            {
                LogConexion.Escribir($"Prueba {Describir(candidato)}: " + (!candidato.Responde
                    ? "NO RESPONDE. " + candidato.Error
                    : $"responde; base {exploracion.BaseDatos} {(candidato.TieneBase ? "EXISTE" : "NO EXISTE")}" +
                      (candidato.TieneBase ? $"; acceso del usuario: {(candidato.TieneAcceso ? "SÍ" : "NO")}" : "")));
            }

            if (exploracion.Candidatos.Any(c => c.Responde))
                _exploracion = exploracion;
            return exploracion;
        }

        private static string Elegir(Exploracion exploracion, Candidato elegido, string uso)
        {
            string cadena = new SqlConnectionStringBuilder(exploracion.Plantilla.ConnectionString) { DataSource = elegido.Servidor }.ConnectionString;
            LogConexion.Escribir($"Elegida para {uso}: {Describir(elegido)} | Cadena efectiva: {LogConexion.SinDatosSensibles(cadena)}");
            return cadena;
        }

        // Error para el usuario con el estado de cada instancia probada y qué hacer
        private static InvalidOperationException Fallar(Exploracion exploracion, bool paraAdministracion)
        {
            string usuario = LogConexion.UsuarioWindows;
            string baseDatos = exploracion.BaseDatos;
            var lineas = new List<string>();

            if (exploracion.Candidatos.Count == 0)
            {
                lineas.Add($"No se encontró ninguna instancia de SQL Server instalada en esta PC y el archivo .config no " +
                           $"indica la cadena de conexión '{NOMBRE_CONEXION}'.");
            }
            else
            {
                lineas.Add("No se encontró una instancia de SQL Server utilizable. Instancias probadas:");
                foreach (Candidato c in exploracion.Candidatos)
                {
                    if (!c.Responde)
                        lineas.Add($"  - {Describir(c)}: no acepta la conexión. {c.Error}");
                    else if (!c.TieneBase)
                        lineas.Add($"  - {Describir(c)}: conecta, pero la base {baseDatos} no existe en esa instancia.");
                    else
                        lineas.Add($"  - {Describir(c)}: la base {baseDatos} existe, pero el usuario {usuario} no tiene acceso a ella.");
                }
                lineas.Add("");
                if (exploracion.Candidatos.Any(c => c.TieneBase && !c.TieneAcceso))
                    lineas.Add($"Solución: vuelva a ejecutar el instalador de LandSmart (otorga el permiso a los usuarios de " +
                               $"esta PC) o pida a un administrador que ejecute permisos.sql.");
                else if (!paraAdministracion && exploracion.Candidatos.Any(c => c.Responde))
                    lineas.Add($"Solución: la base {baseDatos} no está creada. Vuelva a ejecutar el instalador de LandSmart.");
                else
                    lineas.Add("Solución: verifique que el servicio de SQL Server esté iniciado y que el usuario de Windows " +
                               "pueda iniciar sesión en él.");
            }
            lineas.Add($"Detalle técnico: {LogConexion.Ruta}");

            string mensaje = string.Join(Environment.NewLine, lineas);
            LogConexion.Escribir("SIN CONEXIÓN UTILIZABLE: " + mensaje.Replace(Environment.NewLine, " "));
            return new InvalidOperationException(mensaje);
        }

        private static string Describir(Candidato c)
        {
            return c.DelConfig ? c.Servidor + " (App.config)" : c.Servidor;
        }

        // Nombres de servidor de las instancias instaladas: "." para la instancia por defecto, ".\NOMBRE" para las con nombre
        private static List<string> InstanciasInstaladas()
        {
            var servidores = new List<string>();
            foreach (RegistryView vista in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using (RegistryKey raiz = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, vista))
                    using (RegistryKey clave = raiz.OpenSubKey(CLAVE_INSTANCIAS))
                    {
                        if (clave == null) continue;
                        foreach (string instancia in clave.GetValueNames())
                        {
                            string servidor = instancia.Equals(INSTANCIA_POR_DEFECTO, StringComparison.OrdinalIgnoreCase)
                                ? "."
                                : @".\" + instancia;
                            if (!servidores.Contains(servidor, StringComparer.OrdinalIgnoreCase))
                                servidores.Add(servidor);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Sin permiso de lectura o vista inexistente: se sigue con la otra
                    LogConexion.Escribir($"No se pudo leer el registro ({vista}): {ex.Message}");
                }
            }
            return servidores;
        }

        // Se conecta a master (la base puede no existir) y consulta si la base existe y si el usuario tiene acceso
        private static void Probar(SqlConnectionStringBuilder plantilla, string baseDatos, Candidato candidato)
        {
            var prueba = new SqlConnectionStringBuilder(plantilla.ConnectionString)
            {
                DataSource = candidato.Servidor,
                InitialCatalog = "master",
                ConnectTimeout = SEGUNDOS_PRUEBA,
                Pooling = false
            };
            const string consulta =
                "SELECT CASE WHEN DB_ID(@Base) IS NULL THEN 0 ELSE 1 END, ISNULL(HAS_DBACCESS(@Base), 0)";
            try
            {
                using (var conexion = new SqlConnection(prueba.ConnectionString))
                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Base", baseDatos);
                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        lector.Read();
                        candidato.Responde = true;
                        candidato.TieneBase = Convert.ToInt32(lector[0]) == 1;
                        candidato.TieneAcceso = candidato.TieneBase && Convert.ToInt32(lector[1]) == 1;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Número de SQL Server (p. ej. 18456 = inicio de sesión rechazado); -1 / 2 / 53 = no se encontró el servidor
                candidato.Error = LogConexion.Detalle(ex);
            }
            catch (Exception ex)
            {
                candidato.Error = ex.GetType().Name + ": " + ex.Message;
            }
        }

        private static string CadenaDelConfig()
        {
            ConnectionStringSettings cadena = ConfigurationManager.ConnectionStrings[NOMBRE_CONEXION];
            return cadena == null || string.IsNullOrWhiteSpace(cadena.ConnectionString) ? null : cadena.ConnectionString;
        }
    }
}
