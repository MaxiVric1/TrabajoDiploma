using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    // Valores del ICC ya descargados del INDEC (caché local)
    public class HistorialICCDAL
    {
        private Acceso acceso = new Acceso();

        // desde/hasta null = sin límite
        public List<ValorICC> Leer(DateTime? desde = null, DateTime? hasta = null)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Desde", desde?.Date),
                    acceso.CrearParametro("@Hasta", hasta?.Date)
                };
                DataTable dt = acceso.Leer("LeerHistorialICC", parameters);
                List<ValorICC> lista = new List<ValorICC>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new ValorICC
                    {
                        Periodo = Convert.ToDateTime(row["periodo"]),
                        ValorIndice = Convert.ToDecimal(row["valor_indice"]),
                        VariacionMensual = row["variacion_mensual"] != DBNull.Value ? Convert.ToDecimal(row["variacion_mensual"]) : (decimal?)null,
                        FechaConsulta = Convert.ToDateTime(row["fecha_consulta"])
                    });
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        // Transacción: todos los meses de la descarga o ninguno
        public bool Guardar(IEnumerable<ValorICC> valores)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();
                foreach (ValorICC valor in valores)
                {
                    List<SqlParameter> parameters = new List<SqlParameter>
                    {
                        acceso.CrearParametro("@Periodo", valor.Periodo.Date),
                        acceso.CrearParametro("@ValorIndice", valor.ValorIndice),
                        acceso.CrearParametro("@VariacionMensual", valor.VariacionMensual)
                    };
                    if (acceso.Escribir("GuardarHistorialICC", parameters) == -1)
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
