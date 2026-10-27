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
    // Se guardan en ProgramData\LandSmart\Snapshots: compartida por todos los usuarios de Windows del equipo y
    // escribible sin privilegios de administrador. Al listar se lee también la carpeta de Program Files que usaban
    // las versiones anteriores (solo lectura), para no perder las actas guardadas ahí.
    public static class GestorSnapshots
    {
        public static string Carpeta { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "LandSmart", "Snapshots");
        public static string CarpetaAnterior { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "LandSmart", "Snapshots");

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
                throw new Exception("El archivo no es un Acta de Decisión válida: " + ex.Message, ex);
            }
        }

        // ---------- Archivos ----------

        // Guarda el snapshot y devuelve la ruta
        public static string Guardar(SnapshotDecision snapshot)
        {
            return Escribir(Carpeta, NombreArchivo(snapshot.Proyecto.Id, snapshot.FechaDecision), Serializar(snapshot));
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

        // Archivos del proyecto (carpeta actual y la de versiones anteriores), del más reciente al más antiguo
        public static List<string> Listar(int idProyecto)
        {
            string patron = $"Proyecto{idProyecto}_*.json";
            return new[] { Carpeta, CarpetaAnterior }
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
