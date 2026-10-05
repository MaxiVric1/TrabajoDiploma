using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class InquilinoDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(Inquilino inquilino)
        {
            acceso.Abrir();
            try
            {
                return Crear(inquilino, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(Inquilino inquilino, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Nombre", inquilino.Nombre),
                acceso.CrearParametro("@Apellido", inquilino.Apellido),
                acceso.CrearParametro("@Dni", inquilino.Dni),
                acceso.CrearParametro("@Telefono", inquilino.Telefono),
                acceso.CrearParametro("@Email", inquilino.Email),
                outParam
            };

            int result = acceso.Escribir("CrearInquilino", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                inquilino.IdInquilino = Convert.ToInt32(outParam.Value);
                return inquilino.IdInquilino;
            }
            return -1;
        }

        public bool Actualizar(Inquilino inquilino)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", inquilino.IdInquilino),
                    acceso.CrearParametro("@Nombre", inquilino.Nombre),
                    acceso.CrearParametro("@Apellido", inquilino.Apellido),
                    acceso.CrearParametro("@Dni", inquilino.Dni),
                    acceso.CrearParametro("@Telefono", inquilino.Telefono),
                    acceso.CrearParametro("@Email", inquilino.Email)
                };
                return acceso.Escribir("ActualizarInquilino", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Eliminar(int idInquilino)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idInquilino) };
                return acceso.Escribir("EliminarInquilino", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Inquilino> LeerTodos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerInquilinos");
                List<Inquilino> lista = new List<Inquilino>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public Inquilino BuscarPorId(int idInquilino)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idInquilino) };
                DataTable dt = acceso.Leer("BuscarInquilinoPorId", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        public Inquilino BuscarPorDni(string dni)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Dni", dni) };
                DataTable dt = acceso.Leer("BuscarInquilinoPorDni", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private Inquilino Mapear(DataRow row)
        {
            return new Inquilino
            {
                IdInquilino = Convert.ToInt32(row["id_inquilino"]),
                Nombre = row["nombre"].ToString(),
                Apellido = row["apellido"].ToString(),
                Dni = row["dni"].ToString(),
                Telefono = row["telefono"] != DBNull.Value ? row["telefono"].ToString() : "",
                Email = row["email"] != DBNull.Value ? row["email"].ToString() : ""
            };
        }
    }
}
