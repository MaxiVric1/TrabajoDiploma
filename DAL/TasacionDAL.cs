using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class TasacionDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(Tasacion tasacion)
        {
            acceso.Abrir();
            try
            {
                return Crear(tasacion, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(Tasacion tasacion, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@EstadoDictamen", tasacion.EstadoDictamen.ToString()),
                acceso.CrearParametro("@MotivoRechazo", tasacion.MotivoRechazo),
                acceso.CrearParametro("@ValorFinal", tasacion.ValorFinal),
                acceso.CrearParametro("@ValorSugerido", tasacion.ValorSugerido),
                acceso.CrearParametro("@IdCaptacion", tasacion.IdCaptacion),
                acceso.CrearParametro("@IdUsuarioTasador", tasacion.IdUsuarioTasador),
                outParam
            };

            int result = acceso.Escribir("CrearTasacion", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                tasacion.NroTasacion = Convert.ToInt32(outParam.Value);
                return tasacion.NroTasacion;
            }
            return -1;
        }

        public bool Actualizar(Tasacion tasacion)
        {
            acceso.Abrir();
            try
            {
                return Actualizar(tasacion, acceso);
            }
            finally { acceso.Cerrar(); }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal bool Actualizar(Tasacion tasacion, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@NroTasacion", tasacion.NroTasacion),
                acceso.CrearParametro("@EstadoDictamen", tasacion.EstadoDictamen.ToString()),
                acceso.CrearParametro("@MotivoRechazo", tasacion.MotivoRechazo),
                acceso.CrearParametro("@ValorFinal", tasacion.ValorFinal),
                acceso.CrearParametro("@ValorSugerido", tasacion.ValorSugerido),
                acceso.CrearParametro("@IdUsuarioTasador", tasacion.IdUsuarioTasador)
            };
            return acceso.Escribir("ActualizarTasacion", parameters) != -1;
        }

        // Transacción: la tasación y el estado del inmueble se confirman juntos o ninguno.
        // nuevoEstado null = el dictamen no cambia el inmueble (Pendiente). Devuelve el número de tasación, o -1.
        public int RegistrarConDictamen(Tasacion tasacion, int idInmueble, EstadoInmueble? nuevoEstado)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                bool ok = Crear(tasacion, acceso) != -1;
                if (ok && nuevoEstado.HasValue)
                    ok = new InmuebleDAL().ActualizarEstado(idInmueble, nuevoEstado.Value, acceso);

                if (!ok)
                {
                    acceso.DeshacerTX();
                    tasacion.NroTasacion = 0; // el número asignado se revirtió
                    return -1;
                }

                acceso.ConfirmarTX();
                return tasacion.NroTasacion;
            }
            catch
            {
                acceso.DeshacerTX();
                tasacion.NroTasacion = 0;
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Transacción: igual que RegistrarConDictamen, para modificar una tasación existente
        public bool ActualizarConDictamen(Tasacion tasacion, int idInmueble, EstadoInmueble? nuevoEstado)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                bool ok = Actualizar(tasacion, acceso);
                if (ok && nuevoEstado.HasValue)
                    ok = new InmuebleDAL().ActualizarEstado(idInmueble, nuevoEstado.Value, acceso);

                if (!ok)
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

        public bool Eliminar(int nroTasacion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@NroTasacion", nroTasacion) };
                return acceso.Escribir("EliminarTasacion", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Tasacion> LeerTodos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerTasaciones");
                List<Tasacion> lista = new List<Tasacion>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Tasacion> LeerPorCaptacion(int nroCaptacion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@IdCaptacion", nroCaptacion) };
                DataTable dt = acceso.Leer("LeerTasacionesPorCaptacion", parameters);
                List<Tasacion> lista = new List<Tasacion>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public Tasacion BuscarPorNro(int nroTasacion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@NroTasacion", nroTasacion) };
                DataTable dt = acceso.Leer("BuscarTasacionPorNro", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private Tasacion Mapear(DataRow row)
        {
            return new Tasacion
            {
                NroTasacion = Convert.ToInt32(row["nro_tasacion"]),
                FechaDictamen = Convert.ToDateTime(row["fecha_dictamen"]),
                EstadoDictamen = (EstadoDictamen)Enum.Parse(typeof(EstadoDictamen), row["estado_dictamen"].ToString()),
                MotivoRechazo = row["motivo_rechazo"] != DBNull.Value ? row["motivo_rechazo"].ToString() : "",
                ValorFinal = row["valor_final"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["valor_final"]) : null,
                ValorSugerido = row.Table.Columns.Contains("valor_sugerido") && row["valor_sugerido"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["valor_sugerido"]) : null,
                IdCaptacion = Convert.ToInt32(row["id_captacion"]),
                IdUsuarioTasador = Convert.ToInt32(row["id_usuario_tasador"])
            };
        }
    }
}
