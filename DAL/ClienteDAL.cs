using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ClienteDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(Cliente cliente)
        {
            acceso.Abrir();
            try
            {
                return Crear(cliente, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(Cliente cliente, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Dni", cliente.Dni),
                acceso.CrearParametro("@Nombre", cliente.Nombre),
                acceso.CrearParametro("@Apellido", cliente.Apellido),
                acceso.CrearParametro("@Telefono", cliente.Telefono),
                acceso.CrearParametro("@Email", cliente.Email),
                outParam
            };

            int result = acceso.Escribir("CrearCliente", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                cliente.IdCliente = Convert.ToInt32(outParam.Value);
                return cliente.IdCliente;
            }
            return -1;
        }

        public bool Actualizar(Cliente cliente)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", cliente.IdCliente),
                    acceso.CrearParametro("@Dni", cliente.Dni),
                    acceso.CrearParametro("@Nombre", cliente.Nombre),
                    acceso.CrearParametro("@Apellido", cliente.Apellido),
                    acceso.CrearParametro("@Telefono", cliente.Telefono),
                    acceso.CrearParametro("@Email", cliente.Email)
                };
                return acceso.Escribir("ActualizarCliente", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Eliminar(int idCliente)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idCliente) };
                return acceso.Escribir("EliminarCliente", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Cliente> LeerTodos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerClientes");
                List<Cliente> lista = new List<Cliente>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public Cliente BuscarPorId(int idCliente)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idCliente) };
                DataTable dt = acceso.Leer("BuscarClientePorId", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        public Cliente BuscarPorDni(string dni)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Dni", dni) };
                DataTable dt = acceso.Leer("BuscarClientePorDni", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private Cliente Mapear(DataRow row)
        {
            return new Cliente
            {
                IdCliente = Convert.ToInt32(row["id_cliente"]),
                Dni = row["dni"].ToString(),
                Nombre = row["nombre"].ToString(),
                Apellido = row["apellido"].ToString(),
                Telefono = row["telefono"] != DBNull.Value ? row["telefono"].ToString() : "",
                Email = row["email"] != DBNull.Value ? row["email"].ToString() : ""
            };
        }
    }
}
