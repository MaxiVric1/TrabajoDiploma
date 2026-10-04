using System;

namespace BE
{
    // Valores admitidos para MaterialAudiovisual.TipoArchivo (se guardan como texto en tipo_archivo)
    public enum TipoMaterial
    {
        Foto,
        Video,
        Recorrido360
    }

    public class MaterialAudiovisual
    {
        public int IdMaterial { get; set; }
        public string TipoArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaCarga { get; set; }
        public int IdInmueble { get; set; }
        public int IdUsuarioMarketing { get; set; }

        public override string ToString()
        {
            return $"{TipoArchivo} - {RutaArchivo}";
        }
    }
}
