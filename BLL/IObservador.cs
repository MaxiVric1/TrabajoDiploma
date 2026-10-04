using System.Collections.Generic;

namespace BLL
{
    public interface IObservador
    {
        void Actualizar(Dictionary<string, string> traducciones);
    }
}
