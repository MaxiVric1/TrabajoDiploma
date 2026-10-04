using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class InmuebleDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(Inmueble inmueble)
        {
            acceso.Abrir();
            try
            {
                return Crear(inmueble, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(Inmueble inmueble, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@TipoInmueble", inmueble.TipoInmueble),
                acceso.CrearParametro("@Direccion", inmueble.Direccion),
                acceso.CrearParametro("@MetrosCuadrados", inmueble.MetrosCuadrados),
                acceso.CrearParametro("@CantidadAmbientes", inmueble.CantidadAmbientes),
                acceso.CrearParametro("@IdCliente", inmueble.IdCliente),
                outParam
            };

            int result = acceso.Escribir("CrearInmueble", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                inmueble.IdInmueble = Convert.ToInt32(outParam.Value);
                inmueble.EstadoActual = EstadoInmueble.Captado; // el SP siempre crea en estado Captado
                return inmueble.IdInmueble;
            }
            return -1;
        }

        public bool Actualizar(Inmueble inmueble)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", inmueble.IdInmueble),
                    acceso.CrearParametro("@TipoInmueble", inmueble.TipoInmueble),
                    acceso.CrearParametro("@Direccion", inmueble.Direccion),
                    acceso.CrearParametro("@MetrosCuadrados", inmueble.MetrosCuadrados),
                    acceso.CrearParametro("@CantidadAmbientes", inmueble.CantidadAmbientes),
                    acceso.CrearParametro("@EstadoActual", inmueble.EstadoActual.ToString()),
                    acceso.CrearParametro("@IdCliente", inmueble.IdCliente)
                };
                return acceso.Escribir("ActualizarInmueble", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool ActualizarEstado(int idInmueble, EstadoInmueble estado)
        {
            acceso.Abrir();
            try
            {
                return ActualizarEstado(idInmueble, estado, acceso);
            }
            finally { acceso.Cerrar(); }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal bool ActualizarEstado(int idInmueble, EstadoInmueble estado, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Id", idInmueble),
                acceso.CrearParametro("@EstadoActual", estado.ToString())
            };
            return acceso.Escribir("ActualizarEstadoInmueble", parameters) != -1;
        }

        // Transacción: el cambio a Publicado y su registro en bitácora se confirman juntos o ninguno
        public bool Publicar(int idInmueble, Bitacora evento)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                if (!ActualizarEstado(idInmueble, EstadoInmueble.Publicado, acceso) ||
                    !new MapperBitacora().Registrar(evento, acceso))
                {
                    acceso.DeshacerTX();
                    return false;
                }

                acceso.ConfirmarTX();
                return true;
            }
            catch
            {
                acceso.DeshacerTX();
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public bool Eliminar(int idInmueble)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idInmueble) };
                return acceso.Escribir("EliminarInmueble", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Inmueble> LeerTodos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerInmuebles");
                List<Inmueble> lista = new List<Inmueble>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Inmueble> LeerPorCliente(int idCliente)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@IdCliente", idCliente) };
                DataTable dt = acceso.Leer("LeerInmueblesPorCliente", parameters);
                List<Inmueble> lista = new List<Inmueble>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public Inmueble BuscarPorId(int idInmueble)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idInmueble) };
                DataTable dt = acceso.Leer("BuscarInmueblePorId", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private Inmueble Mapear(DataRow row)
        {
            return new Inmueble
            {
                IdInmueble = Convert.ToInt32(row["id_inmueble"]),
                TipoInmueble = row["tipo_inmueble"].ToString(),
                Direccion = row["direccion"].ToString(),
                MetrosCuadrados = Convert.ToDecimal(row["metros_cuadrados"]),
                CantidadAmbientes = Convert.ToInt32(row["cantidad_ambientes"]),
                EstadoActual = (EstadoInmueble)Enum.Parse(typeof(EstadoInmueble), row["estado_actual"].ToString()),
                IdCliente = Convert.ToInt32(row["id_cliente"])
            };
        }
    }
}
