using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class InmuebleAlquilerDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(InmuebleAlquiler inmueble)
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
        internal int Crear(InmuebleAlquiler inmueble, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Direccion", inmueble.Direccion),
                acceso.CrearParametro("@TipoInmueble", inmueble.TipoInmueble.ToString()),
                outParam
            };

            int result = acceso.Escribir("CrearInmuebleAlquiler", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                inmueble.IdInmuebleAlquiler = Convert.ToInt32(outParam.Value);
                return inmueble.IdInmuebleAlquiler;
            }
            return -1;
        }

        public bool Actualizar(InmuebleAlquiler inmueble)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", inmueble.IdInmuebleAlquiler),
                    acceso.CrearParametro("@Direccion", inmueble.Direccion),
                    acceso.CrearParametro("@TipoInmueble", inmueble.TipoInmueble.ToString())
                };
                return acceso.Escribir("ActualizarInmuebleAlquiler", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Eliminar(int idInmuebleAlquiler)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idInmuebleAlquiler) };
                return acceso.Escribir("EliminarInmuebleAlquiler", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<InmuebleAlquiler> LeerTodos()
        {
            return LeerLista("LeerInmueblesAlquiler");
        }

        // Inmuebles sin contrato vigente (Activo o Por Vencer)
        public List<InmuebleAlquiler> LeerDisponibles()
        {
            return LeerLista("LeerInmueblesAlquilerDisponibles");
        }

        public InmuebleAlquiler BuscarPorId(int idInmuebleAlquiler)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idInmuebleAlquiler) };
                DataTable dt = acceso.Leer("BuscarInmuebleAlquilerPorId", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private List<InmuebleAlquiler> LeerLista(string storedProcedure)
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer(storedProcedure);
                List<InmuebleAlquiler> lista = new List<InmuebleAlquiler>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        internal static InmuebleAlquiler Mapear(DataRow row)
        {
            return new InmuebleAlquiler
            {
                IdInmuebleAlquiler = Convert.ToInt32(row["id_inmueble_alquiler"]),
                Direccion = row["direccion"].ToString(),
                TipoInmueble = (TipoInmueble)Enum.Parse(typeof(TipoInmueble), row["tipo_inmueble"].ToString())
            };
        }
    }
}
