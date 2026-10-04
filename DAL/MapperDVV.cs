using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class MapperDVV
    {
        private Acceso acceso = new Acceso();

        public long LeerDVV(string tabla)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Tabla", tabla)
                };
                DataTable dt = acceso.Leer("LeerDVV", parameters);
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt64(dt.Rows[0]["dvv"]);
                }
                return 0;
            }
            finally { acceso.Cerrar(); }
        }

        public void ActualizarDVV(string tabla, long valor)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Tabla", tabla),
                    acceso.CrearParametro("@DVV", valor)
                };
                acceso.Escribir("ActualizarDVV", parameters);
            }
            finally { acceso.Cerrar(); }
        }
    }
}
