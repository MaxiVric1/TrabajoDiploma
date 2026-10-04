using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class CaptacionDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(Captacion captacion)
        {
            acceso.Abrir();
            try
            {
                return Crear(captacion, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(Captacion captacion, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@ValorPretendido", captacion.ValorPretendido),
                acceso.CrearParametro("@IdInmueble", captacion.IdInmueble),
                acceso.CrearParametro("@IdUsuarioAgente", captacion.IdUsuarioAgente),
                outParam
            };

            int result = acceso.Escribir("CrearCaptacion", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                captacion.NroCaptacion = Convert.ToInt32(outParam.Value);
                return captacion.NroCaptacion;
            }
            return -1;
        }

        // Transacción: cliente (si es nuevo, IdCliente == 0) + inmueble + captación se confirman juntos o ninguno.
        // Devuelve el número de captación, o -1 si se revirtió.
        public int RegistrarCompleta(Cliente cliente, Inmueble inmueble, Captacion captacion)
        {
            int idClienteOriginal = cliente.IdCliente;

            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                bool ok = cliente.IdCliente != 0 || new ClienteDAL().Crear(cliente, acceso) != -1;
                if (ok)
                {
                    inmueble.IdCliente = cliente.IdCliente;
                    ok = new InmuebleDAL().Crear(inmueble, acceso) != -1;
                }
                if (ok)
                {
                    captacion.IdInmueble = inmueble.IdInmueble;
                    ok = Crear(captacion, acceso) != -1;
                }

                if (!ok)
                {
                    acceso.DeshacerTX();
                    RestaurarIds(cliente, idClienteOriginal, inmueble, captacion);
                    return -1;
                }

                acceso.ConfirmarTX();
                return captacion.NroCaptacion;
            }
            catch
            {
                acceso.DeshacerTX();
                RestaurarIds(cliente, idClienteOriginal, inmueble, captacion);
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Tras un rollback, los ids asignados no existen en la base: se limpian para que un reintento no los use
        private void RestaurarIds(Cliente cliente, int idClienteOriginal, Inmueble inmueble, Captacion captacion)
        {
            cliente.IdCliente = idClienteOriginal;
            inmueble.IdInmueble = 0;
            captacion.NroCaptacion = 0;
        }

        public bool Actualizar(Captacion captacion)
        {
            acceso.Abrir();
            try
            {
                return Actualizar(captacion, acceso);
            }
            finally { acceso.Cerrar(); }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal bool Actualizar(Captacion captacion, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@NroCaptacion", captacion.NroCaptacion),
                acceso.CrearParametro("@ValorPretendido", captacion.ValorPretendido),
                acceso.CrearParametro("@IdInmueble", captacion.IdInmueble),
                acceso.CrearParametro("@IdUsuarioAgente", captacion.IdUsuarioAgente)
            };
            return acceso.Escribir("ActualizarCaptacion", parameters) != -1;
        }

        // Transacción: nuevo valor pretendido + inmueble vuelve a Captado + registro en bitácora, juntos o ninguno
        public bool Renegociar(Captacion captacion, Bitacora evento)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                if (!Actualizar(captacion, acceso) ||
                    !new InmuebleDAL().ActualizarEstado(captacion.IdInmueble, EstadoInmueble.Captado, acceso) ||
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

        public bool Eliminar(int nroCaptacion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@NroCaptacion", nroCaptacion) };
                return acceso.Escribir("EliminarCaptacion", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Captacion> LeerTodos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerCaptaciones");
                List<Captacion> lista = new List<Captacion>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public Captacion BuscarPorNro(int nroCaptacion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@NroCaptacion", nroCaptacion) };
                DataTable dt = acceso.Leer("BuscarCaptacionPorNro", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private Captacion Mapear(DataRow row)
        {
            return new Captacion
            {
                NroCaptacion = Convert.ToInt32(row["nro_captacion"]),
                FechaRegistro = Convert.ToDateTime(row["fecha_registro"]),
                ValorPretendido = Convert.ToDecimal(row["valor_pretendido"]),
                IdInmueble = Convert.ToInt32(row["id_inmueble"]),
                IdUsuarioAgente = Convert.ToInt32(row["id_usuario_agente"])
            };
        }
    }
}
