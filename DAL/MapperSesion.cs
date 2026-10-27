using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MapperSesion
    {
        private Acceso acceso = new Acceso();

        public int AbrirSesion(int idUsuario)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevaSesionId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", idUsuario),
                    outParam
                };

                int result = acceso.Escribir("CrearSesion", parameters);
                if (result != -1)
                {
                    object outputValue = outParam.Value;
                    if (outputValue != null && outputValue != DBNull.Value)
                    {
                        return Convert.ToInt32(outputValue);
                    }
                }
                return -1;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void CerrarSesion(int idSesion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdSesion", idSesion)
                };

                acceso.Escribir("CerrarSesion", parameters);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

    }
}
