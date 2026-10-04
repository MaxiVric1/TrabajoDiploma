using System;
using System.Collections.Generic;
using BE;

namespace BLL
{
    public class GestorIdioma : ISujeto
    {
        private static GestorIdioma _instance;
        private static readonly object _lock = new object();

        private readonly IdiomaBLL idiomaBLL = new IdiomaBLL();
        private readonly List<IObservador> _observadores = new List<IObservador>();

        public Idioma IdiomaActual { get; private set; }

        private GestorIdioma() { }

        public static GestorIdioma Instancia
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GestorIdioma();
                    }
                    return _instance;
                }
            }
        }

        public void Adjuntar(IObservador observador)
        {
            lock (_lock)
            {
                if (!_observadores.Contains(observador))
                {
                    _observadores.Add(observador);
                    if (IdiomaActual != null)
                    {
                        observador.Actualizar(ObtenerTraduccionesActuales());
                    }
                }
            }
        }

        public void Separar(IObservador observador)
        {
            lock (_lock)
            {
                if (_observadores.Contains(observador))
                {
                    _observadores.Remove(observador);
                }
            }
        }

        public void Notificar()
        {
            List<IObservador> tempObservadores;
            Dictionary<string, string> traducciones;

            lock (_lock)
            {
                tempObservadores = new List<IObservador>(_observadores);
                traducciones = ObtenerTraduccionesActuales();
            }

            foreach (var obs in tempObservadores)
            {
                try
                {
                    obs.Actualizar(traducciones);
                }
                catch
                {
                    // Fail-safe to avoid blocking notifications if an observer is disposed
                }
            }
        }

        public void CambiarIdioma(Idioma idioma)
        {
            if (idioma == null) return;

            lock (_lock)
            {
                IdiomaActual = idioma;
            }
            Notificar();
        }

        public Dictionary<string, string> ObtenerTraduccionesActuales()
        {
            if (IdiomaActual == null)
            {
                return new Dictionary<string, string>();
            }
            return idiomaBLL.ObtenerTraduccionesCacheadas(IdiomaActual.Id);
        }
    }
}
