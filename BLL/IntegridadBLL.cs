using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class IntegridadBLL
    {
        private MapperUsuario mapperUsuario = new MapperUsuario();
        private MapperDVV mapperDVV = new MapperDVV();

        public BE.ReporteIntegridad VerificarIntegridad()
        {
            var reporte = new BE.ReporteIntegridad();
            var usuarios = mapperUsuario.LeerTodos();
            List<long> dvhs = new List<long>();
            var mapperSeguridad = new MapperSeguridad();
            var rolBLL = new RolBLL();

            foreach (var u in usuarios)
            {
                long dvhCalculado = GestorDV.CalcularDVH(u);
                if (u.DVH != dvhCalculado)
                {
                    reporte.EsValido = false;
                    reporte.UsuariosCorruptos.Add(u.Nombre);

                    // Check if this corrupted user is an admin
                    var roles = mapperSeguridad.LeerRolesPorUsuario(u.Id);
                    foreach (var rol in roles)
                    {
                        rolBLL.CargarHijosRecursivo(rol);
                        if (TienePermisoInterno(rol, "AccesoAdmin"))
                        {
                            reporte.AdminCorrupto = true;
                        }
                    }
                }
                dvhs.Add(u.DVH);
            }

            long dvvCalculado = GestorDV.CalcularDVV(dvhs);
            long dvvAlmacenado = mapperDVV.LeerDVV("Usuario");

            if (dvvCalculado != dvvAlmacenado)
            {
                reporte.EsValido = false;
                reporte.DvvInvalido = true;
            }

            return reporte;
        }

        private bool TienePermisoInterno(BE.IComponentePerfil componente, string permissionName)
        {
            if (componente == null) return false;

            if (componente is BE.Permiso permiso)
            {
                return permiso.Nombre.Equals(permissionName, StringComparison.OrdinalIgnoreCase);
            }
            else if (componente is BE.Rol rol)
            {
                foreach (var hijo in rol.Permisos)
                {
                    if (TienePermisoInterno(hijo, permissionName))
                        return true;
                }
            }
            return false;
        }

        public void RepararIntegridad()
        {
            // Solo para uso administrativo en caso de desincronización controlada
            var usuarios = mapperUsuario.LeerTodos();
            List<long> dvhs = new List<long>();

            foreach (var u in usuarios)
            {
                u.DVH = GestorDV.CalcularDVH(u);
                mapperUsuario.Actualizar(u);
                dvhs.Add(u.DVH);
            }

            long dvvCalculado = GestorDV.CalcularDVV(dvhs);
            mapperDVV.ActualizarDVV("Usuario", dvvCalculado);
        }
    }
}
