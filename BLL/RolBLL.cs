using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;

namespace BLL
{
    public class RolBLL
    {
        private MapperRol mapperRol = new MapperRol();
        private MapperUsuario mapperUsuario = new MapperUsuario();
        private MapperSeguridad mapperSeguridad = new MapperSeguridad();

        public int CrearRol(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del rol no puede estar vacío.");

            Rol nuevo = new Rol { Nombre = nombre.Trim() };
            return mapperRol.CrearRol(nuevo);
        }

        public void ActualizarRol(Rol rol)
        {
            if (rol == null) throw new ArgumentNullException(nameof(rol));
            if (string.IsNullOrWhiteSpace(rol.Nombre))
                throw new ArgumentException("El nombre del rol no puede estar vacío.");

            rol.Nombre = rol.Nombre.Trim();
            mapperRol.ActualizarRol(rol);
        }

        public void EliminarRol(int idRol)
        {
            // Validar que no tenga usuarios asignados
            List<Usuario> usuarios = mapperUsuario.LeerTodos();
            foreach (var u in usuarios)
            {
                List<Rol> rolesUsuario = mapperSeguridad.LeerRolesPorUsuario(u.Id);
                if (rolesUsuario.Any(r => r.Id == idRol))
                {
                    throw new Exception($"No se puede eliminar el rol '{idRol}' porque está asignado al usuario '{u.Nombre}'.");
                }
            }

            mapperRol.EliminarRol(idRol);
        }

        public List<Rol> ObtenerTodos()
        {
            List<Rol> roles = mapperRol.LeerTodos();
            foreach (var rol in roles)
            {
                CargarHijosRecursivo(rol);
            }
            return roles;
        }

        public Rol ObtenerConHijos(int idRol)
        {
            Rol rol = mapperRol.LeerPorId(idRol);
            if (rol != null)
            {
                CargarHijosRecursivo(rol);
            }
            return rol;
        }

        public List<Permiso> ObtenerPermisosDisponibles()
        {
            return mapperRol.LeerPermisos();
        }

        public void AsignarHijo(int idRolPadre, IComponentePerfil hijo)
        {
            if (hijo == null) throw new ArgumentNullException(nameof(hijo));

            if (hijo is Permiso permiso)
            {
                mapperRol.AsignarPermiso(idRolPadre, permiso.Id);
            }
            else if (hijo is Rol subrol)
            {
                if (TendriaCiclo(idRolPadre, subrol.Id))
                {
                    throw new InvalidOperationException("No se puede asignar el rol porque generaría una relación cíclica.");
                }
                mapperRol.AsignarSubRol(idRolPadre, subrol.Id);
            }
        }

        public void RemoverHijo(int idRolPadre, IComponentePerfil hijo)
        {
            if (hijo == null) throw new ArgumentNullException(nameof(hijo));

            if (hijo is Permiso permiso)
            {
                mapperRol.RemoverPermiso(idRolPadre, permiso.Id);
            }
            else if (hijo is Rol subrol)
            {
                mapperRol.RemoverSubRol(idRolPadre, subrol.Id);
            }
        }

        public bool TendriaCiclo(int idPadreCandidate, int idHijoCandidate)
        {
            if (idPadreCandidate == idHijoCandidate) return true;

            // Carga el rol hijo con su estructura completa
            Rol hijo = ObtenerConHijos(idHijoCandidate);
            return BuscarEnDescendientes(hijo, idPadreCandidate);
        }

        private bool BuscarEnDescendientes(Rol actual, int idBuscado)
        {
            if (actual == null || actual.Permisos == null) return false;

            foreach (var item in actual.Permisos)
            {
                if (item is Rol subrol)
                {
                    if (subrol.Id == idBuscado) return true;
                    if (BuscarEnDescendientes(subrol, idBuscado)) return true;
                }
            }
            return false;
        }

        public void CargarHijosRecursivo(Rol rolPadre, HashSet<int> visitados = null)
        {
            if (visitados == null) visitados = new HashSet<int>();
            if (visitados.Contains(rolPadre.Id)) return;
            visitados.Add(rolPadre.Id);

            // Limpiar lista antes de cargar para evitar duplicados
            rolPadre.Permisos.Clear();

            // Cargar permisos directos (hojas)
            List<Permiso> permisos = mapperRol.LeerPermisosDirectos(rolPadre.Id);
            foreach (var p in permisos)
            {
                rolPadre.AgregarPermiso(p);
            }

            // Cargar subroles directos (compuestos) y recurrir
            List<Rol> subroles = mapperRol.LeerSubRolesDirectos(rolPadre.Id);
            foreach (var r in subroles)
            {
                rolPadre.AgregarPermiso(r);
                CargarHijosRecursivo(r, new HashSet<int>(visitados));
            }
        }
    }
}
