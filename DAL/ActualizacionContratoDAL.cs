using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ActualizacionContratoDAL
    {
        private Acceso acceso = new Acceso();

        // Nuevo monto del contrato + registro en el historial, en una sola transacción (dentro del SP). Devuelve
        // false si no se aplicó: el contrato ya no está vigente, su monto cambió o esa actualización ya existe.
        public bool Aplicar(ActualizacionContrato actualizacion)
        {
            acceso.Abrir();
            try
            {
                SqlParameter aplicado = new SqlParameter("@Aplicado", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdContrato", actualizacion.IdContrato),
                    acceso.CrearParametro("@FechaActualizacion", actualizacion.FechaActualizacion.Date),
                    acceso.CrearParametro("@Indice", actualizacion.Indice),
                    acceso.CrearParametro("@Porcentaje", actualizacion.Porcentaje),
                    acceso.CrearParametro("@Detalle", actualizacion.Detalle),
                    acceso.CrearParametro("@MontoAnterior", actualizacion.MontoAnterior),
                    acceso.CrearParametro("@MontoNuevo", actualizacion.MontoNuevo),
                    acceso.CrearParametro("@IdUsuario", actualizacion.IdUsuario),
                    aplicado
                };
                acceso.Escribir("AplicarActualizacionContrato", parameters);
                return aplicado.Value != null && aplicado.Value != DBNull.Value && Convert.ToBoolean(aplicado.Value);
            }
            finally { acceso.Cerrar(); }
        }

        // De la más antigua a la más reciente
        public List<ActualizacionContrato> LeerPorContrato(int idContrato)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@IdContrato", idContrato) };
                DataTable dt = acceso.Leer("LeerActualizacionesPorContrato", parameters);
                List<ActualizacionContrato> lista = new List<ActualizacionContrato>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new ActualizacionContrato
                    {
                        IdActualizacion = Convert.ToInt32(row["id_actualizacion"]),
                        IdContrato = Convert.ToInt32(row["id_contrato"]),
                        FechaActualizacion = Convert.ToDateTime(row["fecha_actualizacion"]),
                        FechaRegistro = Convert.ToDateTime(row["fecha_registro"]),
                        Indice = row["indice"].ToString(),
                        Porcentaje = Convert.ToDecimal(row["porcentaje"]),
                        Detalle = row["detalle"] != DBNull.Value ? row["detalle"].ToString() : null,
                        MontoAnterior = Convert.ToDecimal(row["monto_anterior"]),
                        MontoNuevo = Convert.ToDecimal(row["monto_nuevo"]),
                        IdUsuario = row["id_usuario"] != DBNull.Value ? Convert.ToInt32(row["id_usuario"]) : (int?)null
                    });
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }
    }
}
