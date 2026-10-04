using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MapperUsuarioHistorial
    {
        private Acceso acceso = new Acceso();

        public void Insertar(Usuario user, int idAutor, string tipoOperacion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", user.Id),
                    acceso.CrearParametro("@Nombre", user.Nombre),
                    acceso.CrearParametro("@NombrePersona", user.NombrePersona),
                    acceso.CrearParametro("@Apellido", user.Apellido),
                    acceso.CrearParametro("@Pass", user.Password),
                    acceso.CrearParametro("@Activo", user.Activo),
                    acceso.CrearParametro("@DVH", user.DVH),
                    acceso.CrearParametro("@IdUsuarioAutor", idAutor),
                    acceso.CrearParametro("@TipoOperacion", tipoOperacion)
                };
                acceso.Escribir("InsertarUsuarioHistorial", parameters);
            }
            finally { acceso.Cerrar(); }
        }

        public List<UsuarioHistorial> LeerHistorial(int idUsuario)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", idUsuario)
                };
                DataTable dt = acceso.Leer("LeerHistorialUsuario", parameters);
                List<UsuarioHistorial> lista = new List<UsuarioHistorial>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new UsuarioHistorial
                    {
                        IdHistorial = Convert.ToInt32(row["id_historial"]),
                        IdUsuario = Convert.ToInt32(row["id_usuario"]),
                        Nombre = row["nombre"].ToString(),
                        NombrePersona = row["nombre_persona"] != DBNull.Value ? row["nombre_persona"].ToString() : "",
                        Apellido = row["apellido"] != DBNull.Value ? row["apellido"].ToString() : "",
                        Password = row["pass"].ToString(),
                        Activo = Convert.ToBoolean(row["activo"]),
                        DVH = row["dvh"] != DBNull.Value ? Convert.ToInt64(row["dvh"]) : 0,
                        FechaCambio = Convert.ToDateTime(row["fecha_cambio"]),
                        IdUsuarioAutor = Convert.ToInt32(row["id_usuario_autor"]),
                        TipoOperacion = row["tipo_operacion"].ToString()
                    });
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public UsuarioHistorial BuscarPorId(int idHistorial)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdHistorial", idHistorial)
                };
                DataTable dt = acceso.Leer("BuscarHistorialPorId", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new UsuarioHistorial
                    {
                        IdHistorial = Convert.ToInt32(row["id_historial"]),
                        IdUsuario = Convert.ToInt32(row["id_usuario"]),
                        Nombre = row["nombre"].ToString(),
                        NombrePersona = row["nombre_persona"] != DBNull.Value ? row["nombre_persona"].ToString() : "",
                        Apellido = row["apellido"] != DBNull.Value ? row["apellido"].ToString() : "",
                        Password = row["pass"].ToString(),
                        Activo = Convert.ToBoolean(row["activo"]),
                        DVH = row["dvh"] != DBNull.Value ? Convert.ToInt64(row["dvh"]) : 0,
                        FechaCambio = Convert.ToDateTime(row["fecha_cambio"]),
                        IdUsuarioAutor = Convert.ToInt32(row["id_usuario_autor"]),
                        TipoOperacion = row["tipo_operacion"].ToString()
                    };
                }
                return null;
            }
            finally { acceso.Cerrar(); }
        }
    }
}
