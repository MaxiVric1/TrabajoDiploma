using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MapperRol
    {
        private Acceso acceso = new Acceso();

        public List<Rol> LeerTodos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerRoles");
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

        public Rol LeerPorId(int id)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", id)
                };
                DataTable dt = acceso.Leer("LeerRolPorId", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Rol
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString()
                    };
                }
                return null;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public int CrearRol(Rol rol)
        {
            acceso.Abrir();
            try
            {
                SqlParameter pOut = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Nombre", rol.Nombre),
                    pOut
                };
                acceso.Escribir("CrearRol", parameters);
                return Convert.ToInt32(pOut.Value);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void ActualizarRol(Rol rol)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", rol.Id),
                    acceso.CrearParametro("@Nombre", rol.Nombre)
                };
                acceso.Escribir("ActualizarRol", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void EliminarRol(int id)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", id)
                };
                acceso.Escribir("EliminarRol", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void AsignarPermiso(int idRol, int idPermiso)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdRol", idRol),
                    acceso.CrearParametro("@IdPermiso", idPermiso)
                };
                acceso.Escribir("AsignarPermisoARol", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void RemoverPermiso(int idRol, int idPermiso)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdRol", idRol),
                    acceso.CrearParametro("@IdPermiso", idPermiso)
                };
                acceso.Escribir("RemoverPermisoDeRol", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void AsignarSubRol(int idPadre, int idHijo)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdPadre", idPadre),
                    acceso.CrearParametro("@IdHijo", idHijo)
                };
                acceso.Escribir("AsignarSubRol", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void RemoverSubRol(int idPadre, int idHijo)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdPadre", idPadre),
                    acceso.CrearParametro("@IdHijo", idHijo)
                };
                acceso.Escribir("RemoverSubRol", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<Permiso> LeerPermisosDirectos(int idRol)
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

        public List<Rol> LeerSubRolesDirectos(int idRol)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdRol", idRol)
                };
                DataTable dt = acceso.Leer("LeerSubRolesPorRol", parameters);
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

        public List<Permiso> LeerPermisos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerPermisos");
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
    }
}
