using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MapperIdioma
    {
        private Acceso acceso = new Acceso();

        public List<Idioma> LeerTodosIdiomas()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerTodosIdiomas");
                List<Idioma> lista = new List<Idioma>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new Idioma
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString(),
                        Activo = Convert.ToBoolean(row["activo"])
                    });
                }
                return lista;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<Idioma> LeerIdiomasActivos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerIdiomasActivos");
                List<Idioma> lista = new List<Idioma>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new Idioma
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString(),
                        Activo = Convert.ToBoolean(row["activo"])
                    });
                }
                return lista;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void ToggleEstadoIdioma(int idIdioma)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdIdioma", idIdioma)
                };
                acceso.Escribir("ToggleEstadoIdioma", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<Traduccion> LeerTraduccionesPorIdioma(int idIdioma)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdIdioma", idIdioma)
                };
                DataTable dt = acceso.Leer("LeerTraduccionesPorIdioma", parameters);
                List<Traduccion> lista = new List<Traduccion>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new Traduccion
                    {
                        IdControl = Convert.ToInt32(row["idcontrol"]),
                        IdIdioma = Convert.ToInt32(row["ididioma"]),
                        NombreControl = row["NombreControl"].ToString(),
                        Formulario = row["Formulario"].ToString(),
                        Texto = row["texto"].ToString()
                    });
                }
                return lista;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public int CrearIdioma(Idioma idioma)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Nombre", idioma.Nombre),
                    outParam
                };
                int result = acceso.Escribir("CrearIdioma", parameters);
                if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
                {
                    idioma.Id = Convert.ToInt32(outParam.Value);
                    idioma.Activo = true;
                    return idioma.Id;
                }
                return -1;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void ActualizarTraduccion(int idControl, int idIdioma, string texto)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdControl", idControl),
                    acceso.CrearParametro("@IdIdioma", idIdioma),
                    acceso.CrearParametro("@Texto", texto)
                };
                acceso.Escribir("ActualizarTraduccion", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<Traduccion> LeerControles()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerControles");
                List<Traduccion> lista = new List<Traduccion>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new Traduccion
                    {
                        IdControl = Convert.ToInt32(row["id"]),
                        NombreControl = row["nombre"].ToString(),
                        Formulario = row["formulario"].ToString()
                    });
                }
                return lista;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public int CrearControl(string nombre, string formulario)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Nombre", nombre),
                    acceso.CrearParametro("@Formulario", formulario),
                    outParam
                };
                acceso.Escribir("CrearControl", parameters);
                if (outParam.Value != null && outParam.Value != DBNull.Value)
                {
                    return Convert.ToInt32(outParam.Value);
                }
                return -1;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void GuardarIdiomaUsuario(int idUsuario, int idIdioma)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", idUsuario),
                    acceso.CrearParametro("@IdIdioma", idIdioma)
                };
                acceso.Escribir("GuardarIdiomaUsuario", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public Idioma LeerIdiomaUsuario(int idUsuario)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", idUsuario)
                };
                DataTable dt = acceso.Leer("LeerIdiomaUsuario", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Idioma
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Nombre = row["nombre"].ToString(),
                        Activo = Convert.ToBoolean(row["activo"])
                    };
                }
                return null;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void ReasignarUsuariosIdioma(int idIdiomaDesactivado, int idIdiomaFallback)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdIdiomaDesactivado", idIdiomaDesactivado),
                    acceso.CrearParametro("@IdIdiomaFallback", idIdiomaFallback)
                };
                acceso.Escribir("ReasignarUsuariosIdioma", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void EliminarIdioma(int idIdioma)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdIdioma", idIdioma)
                };
                acceso.Escribir("EliminarIdioma", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
