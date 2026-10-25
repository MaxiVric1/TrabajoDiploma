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
                acceso.CrearParametro("@TipoPublicacion", inmueble.TipoPublicacion.ToString()),
                outParam
            };
            // Opcional en el SP: al captar todavía no hay texto, y así sigue funcionando con el SP anterior a la migración
            if (!string.IsNullOrEmpty(inmueble.TextoComercial))
                parameters.Add(acceso.CrearParametro("@TextoComercial", inmueble.TextoComercial));

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
                    acceso.CrearParametro("@TextoComercial", inmueble.TextoComercial),
                    acceso.CrearParametro("@IdCliente", inmueble.IdCliente),
                    acceso.CrearParametro("@TipoPublicacion", inmueble.TipoPublicacion.ToString())
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
        public bool ActualizarTextoComercial(int idInmueble, string textoComercial)
        {
            acceso.Abrir();
            try
            {
                return ActualizarTextoComercial(idInmueble, textoComercial, acceso);
            }
            finally { acceso.Cerrar(); }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal bool ActualizarTextoComercial(int idInmueble, string textoComercial, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Id", idInmueble),
                acceso.CrearParametro("@TextoComercial", textoComercial)
            };
            return acceso.Escribir("ActualizarTextoComercialInmueble", parameters) != -1;
        }

        internal bool ActualizarEstado(int idInmueble, EstadoInmueble estado, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Id", idInmueble),
                acceso.CrearParametro("@EstadoActual", estado.ToString())
            };
            return acceso.Escribir("ActualizarEstadoInmueble", parameters) != -1;
        }

        // Transacción: publicar (Tasado -> Publicado) o despublicar (Publicado -> Tasado) junto con su registro
        // en bitácora. El SP solo cambia el inmueble si está en el estado esperado: si otro usuario ya lo
        // cambió, no se afecta ninguna fila y se revierte todo.
        public bool CambiarPublicacion(int idInmueble, bool publicar, Bitacora evento)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", idInmueble),
                    acceso.CrearParametro("@Publicar", publicar)
                };
                if (acceso.Escribir("CambiarPublicacionInmueble", parameters) != 1 ||
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

        // De alquiler, Publicados y sin contrato vigente: los que se pueden alquilar (PN2)
        public List<Inmueble> LeerDisponiblesParaAlquiler()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerInmueblesDisponiblesParaAlquiler");
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
                TipoPublicacion = row.Table.Columns.Contains("tipo_publicacion")
                    ? (TipoPublicacion)Enum.Parse(typeof(TipoPublicacion), row["tipo_publicacion"].ToString())
                    : TipoPublicacion.Venta,
                TextoComercial = row.Table.Columns.Contains("texto_comercial") && row["texto_comercial"] != DBNull.Value ? row["texto_comercial"].ToString() : "",
                FechaPublicacion = row.Table.Columns.Contains("fecha_publicacion") && row["fecha_publicacion"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["fecha_publicacion"]) : null,
                FechaDespublicacion = row.Table.Columns.Contains("fecha_despublicacion") && row["fecha_despublicacion"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["fecha_despublicacion"]) : null,
                IdCliente = Convert.ToInt32(row["id_cliente"])
            };
        }
    }
}
