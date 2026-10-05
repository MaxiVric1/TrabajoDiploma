using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class AccionLegalDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(AccionLegal accion)
        {
            acceso.Abrir();
            try
            {
                return Crear(accion, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(AccionLegal accion, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Descripcion", accion.Descripcion),
                acceso.CrearParametro("@TipoAccion", accion.TipoAccion.ToString()),
                acceso.CrearParametro("@IdContrato", accion.IdContrato),
                acceso.CrearParametro("@IdUsuarioLegal", accion.IdUsuarioLegal),
                outParam
            };
            // Opcional en el SP: sin fecha usa la actual
            if (accion.FechaAccion != default(DateTime))
                parameters.Add(acceso.CrearParametro("@FechaAccion", accion.FechaAccion));

            int result = acceso.Escribir("CrearAccionLegal", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                accion.IdAccion = Convert.ToInt32(outParam.Value);
                return accion.IdAccion;
            }
            return -1;
        }

        // Transacción: la acción legal y sus efectos se guardan juntos o ninguno.
        // pagos: con el estado (y recargo) que deben quedar. estadoContrato null = el contrato no cambia.
        public bool CrearConEfectos(AccionLegal accion, List<PagoAlquiler> pagos, EstadoContrato? estadoContrato, bool alertaVencimiento)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                bool ok = Crear(accion, acceso) != -1;
                PagoAlquilerDAL pagoDAL = new PagoAlquilerDAL();
                foreach (PagoAlquiler pago in pagos)
                {
                    if (!ok) break;
                    ok = pagoDAL.ActualizarEstado(pago.IdPago, pago.EstadoPago, pago.RecargoAplicado, acceso);
                }
                if (ok && estadoContrato.HasValue)
                    ok = new ContratoDAL().ActualizarEstado(accion.IdContrato, estadoContrato.Value, alertaVencimiento, acceso);

                if (!ok)
                {
                    acceso.DeshacerTX();
                    accion.IdAccion = 0; // el id asignado se revirtió
                    return false;
                }

                acceso.ConfirmarTX();
                return true;
            }
            catch
            {
                acceso.DeshacerTX();
                accion.IdAccion = 0;
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public bool Actualizar(AccionLegal accion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", accion.IdAccion),
                    acceso.CrearParametro("@Descripcion", accion.Descripcion),
                    acceso.CrearParametro("@TipoAccion", accion.TipoAccion.ToString()),
                    acceso.CrearParametro("@IdUsuarioLegal", accion.IdUsuarioLegal)
                };
                return acceso.Escribir("ActualizarAccionLegal", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Eliminar(int idAccion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idAccion) };
                return acceso.Escribir("EliminarAccionLegal", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<AccionLegal> LeerTodos()
        {
            return LeerLista("LeerAccionesLegales", null);
        }

        public AccionLegal BuscarPorId(int idAccion)
        {
            List<AccionLegal> lista = LeerLista("BuscarAccionLegalPorId", new List<SqlParameter> { acceso.CrearParametro("@Id", idAccion) });
            return lista.Count > 0 ? lista[0] : null;
        }

        public List<AccionLegal> LeerPorContrato(int idContrato)
        {
            return LeerLista("LeerAccionesPorContrato", new List<SqlParameter> { acceso.CrearParametro("@IdContrato", idContrato) });
        }

        private List<AccionLegal> LeerLista(string storedProcedure, List<SqlParameter> parameters)
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer(storedProcedure, parameters);
                List<AccionLegal> lista = new List<AccionLegal>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        private AccionLegal Mapear(DataRow row)
        {
            return new AccionLegal
            {
                IdAccion = Convert.ToInt32(row["id_accion"]),
                FechaAccion = Convert.ToDateTime(row["fecha_accion"]),
                Descripcion = row["descripcion"].ToString(),
                TipoAccion = (TipoAccion)Enum.Parse(typeof(TipoAccion), row["tipo_accion"].ToString()),
                IdContrato = Convert.ToInt32(row["id_contrato"]),
                IdUsuarioLegal = Convert.ToInt32(row["id_usuario_legal"])
            };
        }
    }
}
