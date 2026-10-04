using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MapperSeguridad
    {
        private Acceso acceso = new Acceso();

        public List<Rol> LeerRolesPorUsuario(int idUsuario)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", idUsuario)
                };

                DataTable dt = acceso.Leer("LeerRolesPorUsuario", parameters);
                List<Rol> roles = new List<Rol>();

                foreach (DataRow row in dt.Rows)
                {
                    roles.Add(new Rol
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString()
                    });
                }
                return roles;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<Permiso> LeerPermisosPorRol(int idRol)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdRol", idRol)
                };

                DataTable dt = acceso.Leer("LeerPermisosPorRol", parameters);
                List<Permiso> permisos = new List<Permiso>();

                foreach (DataRow row in dt.Rows)
                {
                    permisos.Add(new Permiso
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString()
                    });
                }
                return permisos;
            }
            finally
            {
                acceso.Cerrar();
            }
        }


        public void AsignarRol(int idUsuario, int idRol)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", idUsuario),
                    acceso.CrearParametro("@IdRol", idRol)
                };
                acceso.Escribir("AsignarRolUsuario", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void LimpiarRolesUsuario(int idUsuario)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", idUsuario)
                };
                acceso.Escribir("RemoverRolesUsuario", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
