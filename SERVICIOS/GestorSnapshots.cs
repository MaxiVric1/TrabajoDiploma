using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using BE;

namespace SERVICIOS
{
    // Snapshots de las decisiones del Directorio: serialización y deserialización JSON con DataContractJsonSerializer
    // (atributos DataContract / DataMember de BE.SnapshotDecision) y archivos Proyecto{id}_{yyyyMMdd}_{HHmmss}.json.
    // Se guardan en Program Files\LandSmart\Snapshots; si Windows no da permiso de escritura ahí (un usuario sin
    // privilegios de administrador), en ProgramData\LandSmart\Snapshots. Al listar se leen las dos carpetas.
    public static class GestorSnapshots
    {
        public static string CarpetaPrincipal { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "LandSmart", "Snapshots");
        public static string CarpetaAlternativa { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "LandSmart", "Snapshots");

        // Fechas legibles en el JSON (sin esto el serializador escribe "/Date(1793043022000-0300)/")
        private static readonly DataContractJsonSerializerSettings Configuracion = new DataContractJsonSerializerSettings
        {
            DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss")
        };

        public static string NombreArchivo(int idProyecto, DateTime fecha) => $"Proyecto{idProyecto}_{fecha:yyyyMMdd}_{fecha:HHmmss}.json";

        // ---------- Serialización ----------

        public static string Serializar(SnapshotDecision snapshot)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Escritor con sangría: el archivo se puede leer y revisar a mano
                using (var escritor = JsonReaderWriterFactory.CreateJsonWriter(ms, new UTF8Encoding(false), false, true, "  "))
                {
                    new DataContractJsonSerializer(typeof(SnapshotDecision), Configuracion).WriteObject(escritor, snapshot);
                    escritor.Flush();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static SnapshotDecision Deserializar(string json)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    return (SnapshotDecision)new DataContractJsonSerializer(typeof(SnapshotDecision), Configuracion).ReadObject(ms);
            }
            catch (SerializationException ex)
            {
                throw new Exception("El archivo no es un snapshot válido: " + ex.Message, ex);
            }
        }

        // ---------- Archivos ----------

        // Guarda el snapshot y devuelve la ruta; enAlternativa indica que se usó ProgramData por falta de permisos
        public static string Guardar(SnapshotDecision snapshot, out bool enAlternativa)
        {
            string json = Serializar(snapshot);
            string nombre = NombreArchivo(snapshot.Proyecto.Id, snapshot.FechaDecision);
            try
            {
                enAlternativa = false;
                return Escribir(CarpetaPrincipal, nombre, json);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
            {
                enAlternativa = true;
                return Escribir(CarpetaAlternativa, nombre, json);
            }
        }

        private static string Escribir(string carpeta, string nombre, string json)
        {
            Directory.CreateDirectory(carpeta);
            string ruta = Path.Combine(carpeta, nombre);
            // Dos decisiones en el mismo segundo no se pisan
            for (int i = 2; File.Exists(ruta); i++)
                ruta = Path.Combine(carpeta, Path.GetFileNameWithoutExtension(nombre) + "_" + i + ".json");
            File.WriteAllText(ruta, json, new UTF8Encoding(false));
            return ruta;
        }

        // Archivos del proyecto en las dos carpetas, del más reciente al más antiguo
        public static List<string> Listar(int idProyecto)
        {
            string patron = $"Proyecto{idProyecto}_*.json";
            return new[] { CarpetaPrincipal, CarpetaAlternativa }
                .Where(Directory.Exists)
                .SelectMany(c => Directory.GetFiles(c, patron))
                .OrderByDescending(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static SnapshotDecision Leer(string ruta)
        {
            SnapshotDecision snapshot = Deserializar(File.ReadAllText(ruta, Encoding.UTF8));
            snapshot.RutaArchivo = ruta;
            return snapshot;
        }
    }
}
