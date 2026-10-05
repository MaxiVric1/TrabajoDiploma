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
    // Si no se detecta ninguna instancia (p. ej. SQL Server en otra PC), se usa la cadena de App.config tal cual.
    internal static class ServidorSql
    {
        // Nombre de la cadena en <connectionStrings> del .config del ejecutable (TpIngSoft.exe.config, BackupCLI.exe.config)
        internal const string NOMBRE_CONEXION = "TpIngSoft";

        // Si App.config no tiene la cadena, se usa esta como base (el servidor lo pone la detección)
        private const string CADENA_POR_DEFECTO = "Initial Catalog=tpingsoft;Integrated Security=True;";
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

            string primeraQueResponde = null;
            foreach (string servidor in InstanciasInstaladas())
            {
                bool? tieneLaBase = Probar(plantilla, servidor, baseDatos);
                if (tieneLaBase == null) continue; // no respondió
                if (tieneLaBase.Value) return ConServidor(plantilla, servidor);
                if (primeraQueResponde == null) primeraQueResponde = servidor;
            }
            if (primeraQueResponde != null)
                return ConServidor(plantilla, primeraQueResponde);

            // Sin instancias locales que respondan: la cadena de App.config, si existe
            if (delConfig != null) return delConfig;
            throw new InvalidOperationException(
                "No se encontró ninguna instancia de SQL Server que responda en esta PC y el archivo .config no indica " +
                $"la cadena de conexión '{NOMBRE_CONEXION}'. Verifique que el servicio de SQL Server esté iniciado.");
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

        // null si la instancia no responde; true/false según tenga la base de la aplicación
        private static bool? Probar(SqlConnectionStringBuilder plantilla, string servidor, string baseDatos)
        {
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
            catch (Exception)
            {
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
