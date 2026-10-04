using System;
using System.Linq;
using BE;

namespace SERVICIOS
{
    public class SessionManager
    {
        private static SessionManager _instance;
        private static readonly object _lock = new object();

        public BE.Usuario UsuarioActual { get; private set; }
        public int? IdSesion { get; private set; }
        public BE.ReporteIntegridad ReporteIntegridadTemporal { get; set; }

        private SessionManager() { }

        public static SessionManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SessionManager();
                    }
                    return _instance;
                }
            }
        }

        public void IniciarSesion(BE.Usuario user, int sessionId)
        {
            UsuarioActual = user;
            IdSesion = sessionId;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
            IdSesion = null;
        }

        public bool EstaLogueado()
        {
            return UsuarioActual != null;
        }

        public bool TienePermiso(string permissionName)
        {
            if (UsuarioActual == null) return false;

            foreach (var rol in UsuarioActual.Roles)
            {
                if (TienePermisoInterno(rol, permissionName))
                    return true;
            }
            return false;
        }

        private bool TienePermisoInterno(IComponentePerfil componente, string permissionName)
        {
            if (componente == null) return false;

            if (componente is Permiso permiso)
            {
                return permiso.Nombre.Equals(permissionName, StringComparison.OrdinalIgnoreCase);
            }
            else if (componente is Rol rol)
            {
                foreach (var hijo in rol.Permisos)
                {
                    if (TienePermisoInterno(hijo, permissionName))
                        return true;
                }
            }
            return false;
        }
    }
}
