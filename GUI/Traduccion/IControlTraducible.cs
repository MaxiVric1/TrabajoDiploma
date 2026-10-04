using System.Collections.Generic;

namespace TpIngSoft.Traduccion
{
    public interface IControlTraducible
    {
        string NombreControl { get; }
        void Traducir(Dictionary<string, string> traducciones);
    }
}
