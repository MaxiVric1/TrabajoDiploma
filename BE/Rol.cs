using System.Collections.Generic;
using System.Linq;

namespace BE
{
    public class Rol : IComponentePerfil
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<IComponentePerfil> Permisos { get; set; } = new List<IComponentePerfil>();

        public IEnumerable<Permiso> PermisosDirectos => Permisos.OfType<Permiso>();
        public IEnumerable<Rol> SubRoles => Permisos.OfType<Rol>();

        public void AgregarPermiso(IComponentePerfil componente)
        {
            if (componente != null && !Permisos.Any(p => p.Id == componente.Id && p.GetType() == componente.GetType()))
            {
                Permisos.Add(componente);
            }
        }

        public void RemoverPermiso(IComponentePerfil componente)
        {
            if (componente != null)
            {
                var item = Permisos.FirstOrDefault(p => p.Id == componente.Id && p.GetType() == componente.GetType());
                if (item != null)
                {
                    Permisos.Remove(item);
                }
            }
        }

        public List<IComponentePerfil> ObtenerPermisos()
        {
            return Permisos;
        }

        public string ObtenerDescripcion()
        {
            return $"[Rol] {Nombre}";
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
