using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    // Valores de las series del BCRA ya consultados a la API (caché local)
    public class IndiceBCRADAL
    {
        private Acceso acceso = new Acceso();

        public Dictionary<DateTime, decimal> Leer(int idVariable, DateTime desde, DateTime hasta)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdVariable", idVariable),
                    acceso.CrearParametro("@Desde", desde.Date),
                    acceso.CrearParametro("@Hasta", hasta.Date)
                };
                DataTable dt = acceso.Leer("LeerIndiceBCRA", parameters);
                Dictionary<DateTime, decimal> valores = new Dictionary<DateTime, decimal>();
                foreach (DataRow row in dt.Rows)
                {
                    valores[Convert.ToDateTime(row["fecha"]).Date] = Convert.ToDecimal(row["valor"]);
                }
                return valores;
            }
            finally { acceso.Cerrar(); }
        }

        // Transacción: todos los valores de la consulta o ninguno
        public bool Guardar(int idVariable, Dictionary<DateTime, decimal> valores)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();
                foreach (KeyValuePair<DateTime, decimal> valor in valores)
                {
                    List<SqlParameter> parameters = new List<SqlParameter>
                    {
                        acceso.CrearParametro("@IdVariable", idVariable),
                        acceso.CrearParametro("@Fecha", valor.Key.Date),
                        acceso.CrearParametro("@Valor", valor.Value)
                    };
                    if (acceso.Escribir("GuardarIndiceBCRA", parameters) == -1)
                    {
                        acceso.DeshacerTX();
                        return false;
                    }
                }
                acceso.ConfirmarTX();
                return true;
            }
            catch
            {
                acceso.DeshacerTX();
                throw;
            }
            finally { acceso.Cerrar(); }
        }
    }
}
