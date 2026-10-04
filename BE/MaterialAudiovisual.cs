using System;

namespace BE
{
    public class MaterialAudiovisual
    {
        public int IdMaterial { get; set; }
        public string TipoArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public string TextoComercial { get; set; }
        public DateTime FechaCarga { get; set; }
        public int IdInmueble { get; set; }
        public int IdUsuarioMarketing { get; set; }

        public override string ToString()
        {
            return $"{TipoArchivo} - {RutaArchivo}";
        }
    }
}
