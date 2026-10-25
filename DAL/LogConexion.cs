using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace DAL
{
    // Log de diagnóstico de la conexión a SQL Server (detección de instancia, pruebas y errores completos).
    // Se escribe en la carpeta del usuario (%LOCALAPPDATA%\LandSmart\Logs), que siempre es escribible aunque la
    // aplicación esté instalada en Program Files. Nunca incluye contraseñas.
    public static class LogConexion
    {
        private const long TAMANIO_MAXIMO = 1024 * 1024; // al superarlo se rota a conexion.log.old

        private static readonly object _bloqueo = new object();

        public static string Ruta { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LandSmart", "Logs", "conexion.log");

        // Usuario de Windows con el que se conecta la aplicación (autenticación integrada)
        public static string UsuarioWindows
        {
            get
            {
                try { return WindowsIdentity.GetCurrent().Name; }
                catch (Exception) { return Environment.UserDomainName + @"\" + Environment.UserName; }
            }
        }

        // El log nunca debe impedir que la aplicación funcione: cualquier error al escribirlo se ignora
        public static void Escribir(string mensaje)
        {
            try
            {
                lock (_bloqueo)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Ruta));
                    var archivo = new FileInfo(Ruta);
                    if (archivo.Exists && archivo.Length > TAMANIO_MAXIMO)
                    {
                        string anterior = Ruta + ".old";
                        if (File.Exists(anterior)) File.Delete(anterior);
                        File.Move(Ruta, anterior);
                    }
                    File.AppendAllText(Ruta, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {mensaje}{Environment.NewLine}", Encoding.UTF8);
                }
            }
            catch (Exception)
            {
            }
        }

        // Cadena de conexión sin datos sensibles (enmascara la contraseña si se usara autenticación de SQL Server)
        public static string SinDatosSensibles(string cadena)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(cadena);
                if (!string.IsNullOrEmpty(builder.Password))
                    builder.Password = "****";
                return builder.ConnectionString;
            }
            catch (Exception)
            {
                return "(cadena no válida)";
            }
        }

        // Todos los errores que devolvió SQL Server, con número, severidad, estado y servidor
        public static string Detalle(SqlException ex)
        {
            return string.Join(" | ", ex.Errors.Cast<SqlError>().Select(e =>
                $"[error {e.Number}, severidad {e.Class}, estado {e.State}, servidor '{e.Server}'] {e.Message}"));
        }
    }
}
