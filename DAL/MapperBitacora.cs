using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MapperBitacora
    {
        private Acceso acceso = new Acceso();

        public void Registrar(Bitacora b)
        {
            acceso.Abrir();
            try
            {
                Registrar(b, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal bool Registrar(Bitacora b, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@IdUsuario", (object)b.Usuario?.Id ?? DBNull.Value),
                acceso.CrearParametro("@Actividad", b.Actividad),
                acceso.CrearParametro("@InfoAsociada", (object)b.InfoAsociada ?? DBNull.Value)
            };

            return acceso.Escribir("InsertarBitacora", parameters) != -1;
        }

        public List<Bitacora> Buscar(int? idUsuario, string actividad, DateTime? desde, DateTime? hasta)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", (object)idUsuario ?? DBNull.Value),
                    acceso.CrearParametro("@Actividad", (object)actividad ?? DBNull.Value),
                    acceso.CrearParametro("@FechaDesde", (object)desde ?? DBNull.Value),
                    acceso.CrearParametro("@FechaHasta", (object)hasta ?? DBNull.Value)
                };

                DataTable dt = acceso.Leer("BuscarBitacora", parameters);
                List<Bitacora> lista = new List<Bitacora>();

                foreach (DataRow row in dt.Rows)
                {
                    Bitacora b = new Bitacora
                    {
                        Id = Convert.ToInt32(row["id"]),
                        FechaHora = Convert.ToDateTime(row["fecha_hora"]),
                        Actividad = row["actividad"].ToString(),
                        InfoAsociada = row["info_asociada"].ToString()
                    };

                    if (row["id_usuario"] != DBNull.Value)
                    {
                        b.Usuario = new Usuario
                        {
                            Id = Convert.ToInt32(row["id_usuario"]),
                            Nombre = row["nombre_usuario"].ToString()
                        };
                    }

                    lista.Add(b);
                }
                return lista;
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
