using System;
using System.Collections.Generic;

namespace BE
{
    public class Permiso : IComponentePerfil
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string ObtenerDescripcion()
        {
            return $"[Permiso] {Nombre}";
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
