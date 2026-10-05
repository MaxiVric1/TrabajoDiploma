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
    //   1. Lee del registro las instancias instaladas (vistas de 64 y 32 bits).
    //   2. Prueba conectarse a cada una (timeout corto).
    //   3. Prefiere la que ya tiene la base de la aplicación; si ninguna la tiene, la primera que respondió.
    //   4. Guarda el resultado: la búsqueda se hace una sola vez por ejecución.
    // De App.config se toman la base y la seguridad; la detección solo reemplaza el servidor (Data Source).
    // El servidor de App.config se prueba como un candidato más (es el único si SQL Server está en otra PC).
    // Si ningún candidato acepta la conexión, se informa el error real de cada uno (permiso, servicio detenido, etc.).
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
        private const int SEGUNDOS_PRUEBA = 3;

        private static readonly object _bloqueo = new object();
        private static string _cadena;

        // Cadena de conexión completa (servidor detectado + base y seguridad de App.config). Se calcula una sola vez.
        public static string Detectar()
        {
            if (_cadena != null) return _cadena;
            lock (_bloqueo)
            {
                if (_cadena == null)
                    _cadena = Buscar();
                return _cadena;
            }
        }

        private static string Buscar()
        {
            string delConfig = CadenaDelConfig();
            var plantilla = new SqlConnectionStringBuilder(delConfig ?? CADENA_POR_DEFECTO);
            string baseDatos = plantilla.InitialCatalog;

            // Candidatos: las instancias del registro y, al final, el servidor de App.config (si no es una de ellas)
            var candidatos = InstanciasInstaladas();
            string servidorConfig = delConfig != null ? new SqlConnectionStringBuilder(delConfig).DataSource : null;
            if (!string.IsNullOrWhiteSpace(servidorConfig) && !candidatos.Contains(servidorConfig, StringComparer.OrdinalIgnoreCase))
                candidatos.Add(servidorConfig);

            string primeraQueResponde = null;
            var errores = new List<string>();
            foreach (string servidor in candidatos)
            {
                bool? tieneLaBase = Probar(plantilla, servidor, baseDatos, out string error);
                if (tieneLaBase == null)
                {
                    string origen = servidor.Equals(servidorConfig, StringComparison.OrdinalIgnoreCase) ? " (App.config)" : "";
                    errores.Add($"  - {servidor}{origen}: {error}");
                    continue;
                }
                if (tieneLaBase.Value) return ConServidor(plantilla, servidor);
                if (primeraQueResponde == null) primeraQueResponde = servidor;
            }
            // Ninguna tiene la base (p. ej. todavía no se instaló, o se va a restaurar un backup): la primera que respondió
            if (primeraQueResponde != null)
                return ConServidor(plantilla, primeraQueResponde);

            //// Sin instancias locales que respondan: la cadena de App.config, si existe
            //if (delConfig != null) return delConfig;
            if (candidatos.Count == 0)
                throw new InvalidOperationException(
                    "No se encontró ninguna instancia de SQL Server instalada en esta PC y el archivo .config no indica " +
                    $"la cadena de conexión '{NOMBRE_CONEXION}'.");
            throw new InvalidOperationException(
                "No se pudo conectar con SQL Server. Instancias probadas:" + Environment.NewLine +
                string.Join(Environment.NewLine, errores) + Environment.NewLine +
                "Verifique que el servicio de SQL Server esté iniciado y que el usuario de Windows tenga permiso " +
                "(volver a ejecutar el instalador otorga los permisos).");
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
                catch (Exception)
                {
                    // Sin permiso de lectura o vista inexistente: se sigue con la otra
                }
            }
            return servidores;
        }

        // null si la instancia no acepta la conexión (error: el motivo real); true/false según tenga la base de la aplicación
        private static bool? Probar(SqlConnectionStringBuilder plantilla, string servidor, string baseDatos, out string error)
        {
            error = null;
            var prueba = new SqlConnectionStringBuilder(plantilla.ConnectionString)
            {
                DataSource = servidor,
                InitialCatalog = "master", // la base puede no existir todavía
                ConnectTimeout = SEGUNDOS_PRUEBA,
                Pooling = false
            };
            try
            {
                using (var conexion = new SqlConnection(prueba.ConnectionString))
                using (var comando = new SqlCommand("SELECT CASE WHEN DB_ID(@Base) IS NULL THEN 0 ELSE 1 END", conexion))
                {
                    comando.Parameters.AddWithValue("@Base", baseDatos ?? "");
                    conexion.Open();
                    return Convert.ToInt32(comando.ExecuteScalar()) == 1;
                }
            }
            catch (SqlException ex)
            {
                // Número de SQL Server (p. ej. 18456 = inicio de sesión rechazado) + mensaje; -1 / 2 / 53 = no se encontró el servidor
                error = $"[error {ex.Number}] {ex.Message}";
                return null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        private static string ConServidor(SqlConnectionStringBuilder plantilla, string servidor)
        {
            return new SqlConnectionStringBuilder(plantilla.ConnectionString) { DataSource = servidor }.ConnectionString;
        }

        private static string CadenaDelConfig()
        {
            ConnectionStringSettings cadena = ConfigurationManager.ConnectionStrings[NOMBRE_CONEXION];
            return cadena == null || string.IsNullOrWhiteSpace(cadena.ConnectionString) ? null : cadena.ConnectionString;
        }
    }
}
