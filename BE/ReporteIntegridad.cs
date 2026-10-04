using System.Collections.Generic;

namespace BE
{
    public class ReporteIntegridad
    {
        public bool EsValido { get; set; } = true;
        public bool AdminCorrupto { get; set; } = false;
        public bool DvvInvalido { get; set; } = false;
        public List<string> UsuariosCorruptos { get; set; } = new List<string>();
    }
}
