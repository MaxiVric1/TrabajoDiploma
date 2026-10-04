using System;
using System.Collections.Generic;
using BE;
using DAL;

namespace BLL
{
    public class GestorBitacora
    {
        private MapperBitacora mapper = new MapperBitacora();

        // Implementación Singleton opcional para fácil acceso desde cualquier capa de BLL/GUI
        private static GestorBitacora instance;
        public static GestorBitacora Instance
        {
            get
            {
                if (instance == null) instance = new GestorBitacora();
                return instance;
            }
        }

        public void RegistrarEvento(Usuario usuario, string actividad, string infoAsociada)
        {
            Bitacora b = new Bitacora
            {
                FechaHora = DateTime.Now,
                Usuario = usuario,
                Actividad = actividad,
                InfoAsociada = infoAsociada
            };

            mapper.Registrar(b);
        }

        public List<Bitacora> BuscarEventos(int? idUsuario, string actividad, DateTime? desde, DateTime? hasta)
        {
            // Validaciones básicas de negocio si fueran necesarias
            return mapper.Buscar(idUsuario, actividad, desde, hasta);
        }
    }
}
