namespace BLL
{
    public interface ISujeto
    {
        void Adjuntar(IObservador observador);
        void Separar(IObservador observador);
        void Notificar();
    }
}
