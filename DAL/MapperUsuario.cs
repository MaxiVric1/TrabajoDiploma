using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MapperUsuario
    {
        private Acceso acceso = new Acceso();

        public Usuario BuscarPorNombre(string nombre)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Nombre", nombre)
                };

                DataTable dt = acceso.Leer("BuscarUsuarioPorNombre", parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Usuario
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString(),
                        NombrePersona = row["nombre_persona"] != DBNull.Value ? row["nombre_persona"].ToString() : "",
                        Apellido = row["apellido"] != DBNull.Value ? row["apellido"].ToString() : "",
                        Password = row["pass"].ToString(),
                        Activo = Convert.ToBoolean(row["activo"]),
                        IntentosFallidos = row["intentos_fallidos"] != DBNull.Value ? Convert.ToInt32(row["intentos_fallidos"]) : 0,
                        BloqueadoHasta = row["bloqueado_hasta"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["bloqueado_hasta"]) : null,
                        DVH = row["dvh"] != DBNull.Value ? Convert.ToInt64(row["dvh"]) : 0,
                        IdIdioma = dt.Columns.Contains("id_idioma") && row["id_idioma"] != DBNull.Value ? (int?)Convert.ToInt32(row["id_idioma"]) : null
                    };
                }
                return null;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public Usuario BuscarPorId(int id)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", id) };
                DataTable dt = acceso.Leer("BuscarUsuarioPorId", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Usuario
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString(),
                        NombrePersona = row["nombre_persona"] != DBNull.Value ? row["nombre_persona"].ToString() : "",
                        Apellido = row["apellido"] != DBNull.Value ? row["apellido"].ToString() : "",
                        Password = row["pass"].ToString(),
                        Activo = Convert.ToBoolean(row["activo"]),
                        IntentosFallidos = row["intentos_fallidos"] != DBNull.Value ? Convert.ToInt32(row["intentos_fallidos"]) : 0,
                        BloqueadoHasta = row["bloqueado_hasta"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["bloqueado_hasta"]) : null,
                        DVH = row["dvh"] != DBNull.Value ? Convert.ToInt64(row["dvh"]) : 0,
                        IdIdioma = dt.Columns.Contains("id_idioma") && row["id_idioma"] != DBNull.Value ? (int?)Convert.ToInt32(row["id_idioma"]) : null
                    };
                }
                return null;
            }
            finally { acceso.Cerrar(); }
        }

        public int Crear(Usuario user)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Nombre", user.Nombre),
                    acceso.CrearParametro("@NombrePersona", user.NombrePersona),
                    acceso.CrearParametro("@Apellido", user.Apellido),
                    acceso.CrearParametro("@Pass", user.Password),
                    acceso.CrearParametro("@DVH", user.DVH),
                    outParam
                };

                int result = acceso.Escribir("CrearUsuario", parameters);
                if (result != -1)
                {
                    if (outParam.Value != null && outParam.Value != DBNull.Value)
                    {
                        user.Id = Convert.ToInt32(outParam.Value);
                        return user.Id;
                    }
                }
                return -1;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void ActualizarIntentos(string nombre, bool exitoso)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Nombre", nombre),
                    acceso.CrearParametro("@Exitoso", exitoso)
                };
                acceso.Escribir("ActualizarIntentosFallidos", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
        public void Actualizar(Usuario user)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", user.Id),
                    acceso.CrearParametro("@Nombre", user.Nombre),
                    acceso.CrearParametro("@NombrePersona", user.NombrePersona),
                    acceso.CrearParametro("@Apellido", user.Apellido),
                    acceso.CrearParametro("@Pass", user.Password),
                    acceso.CrearParametro("@Activo", user.Activo),
                    acceso.CrearParametro("@DVH", user.DVH)
                };
                acceso.Escribir("ActualizarUsuario", parameters);
            }
            finally { acceso.Cerrar(); }
        }

        public List<Usuario> LeerTodos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerUsuarios");
                List<Usuario> lista = new List<Usuario>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new Usuario
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString(),
                        NombrePersona = row["nombre_persona"] != DBNull.Value ? row["nombre_persona"].ToString() : "",
                        Apellido = row["apellido"] != DBNull.Value ? row["apellido"].ToString() : "",
                        Password = row["pass"].ToString(),
                        Activo = Convert.ToBoolean(row["activo"]),
                        IntentosFallidos = row["intentos_fallidos"] != DBNull.Value ? Convert.ToInt32(row["intentos_fallidos"]) : 0,
                        BloqueadoHasta = row["bloqueado_hasta"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["bloqueado_hasta"]) : null,
                        DVH = row["dvh"] != DBNull.Value ? Convert.ToInt64(row["dvh"]) : 0,
                        IdIdioma = dt.Columns.Contains("id_idioma") && row["id_idioma"] != DBNull.Value ? (int?)Convert.ToInt32(row["id_idioma"]) : null
                    });
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public void BloquearManual(string nombre, int minutos)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Nombre", nombre),
                    acceso.CrearParametro("@Minutos", minutos)
                };
                acceso.Escribir("BloquearUsuarioManual", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
