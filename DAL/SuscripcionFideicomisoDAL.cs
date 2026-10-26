using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class SuscripcionFideicomisoDAL
    {
        private Acceso acceso = new Acceso();

        // -1 si no se registró: el SP solo acepta proyectos Aprobados y sin superar el capital necesario
        public int Crear(SuscripcionFideicomiso suscripcion)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdProyecto", suscripcion.IdProyecto),
                    acceso.CrearParametro("@IdInversor", suscripcion.IdInversor),
                    acceso.CrearParametro("@MontoAportado", suscripcion.MontoAportado),
                    acceso.CrearParametro("@UnidadesAsignadas", suscripcion.UnidadesAsignadas),
                    outParam
                };
                acceso.Escribir("CrearSuscripcionFideicomiso", parameters);
                if (outParam.Value != null && outParam.Value != DBNull.Value)
                {
                    suscripcion.IdSuscripcion = Convert.ToInt32(outParam.Value);
                    return suscripcion.IdSuscripcion;
                }
                return -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Eliminar(int idSuscripcion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idSuscripcion) };
                return acceso.Escribir("EliminarSuscripcionFideicomiso", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        // Con el inversor y su participación sobre el capital necesario
        public List<SuscripcionFideicomiso> LeerPorProyecto(int idProyecto)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@IdProyecto", idProyecto) };
                DataTable dt = acceso.Leer("LeerSuscripcionesPorProyecto", parameters);
                List<SuscripcionFideicomiso> lista = new List<SuscripcionFideicomiso>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new SuscripcionFideicomiso
                    {
                        IdSuscripcion = Convert.ToInt32(row["id_suscripcion"]),
                        IdProyecto = Convert.ToInt32(row["id_proyecto"]),
                        IdInversor = Convert.ToInt32(row["id_inversor"]),
                        MontoAportado = Convert.ToDecimal(row["monto_aportado"]),
                        UnidadesAsignadas = Convert.ToInt32(row["unidades_asignadas"]),
                        FechaSuscripcion = Convert.ToDateTime(row["fecha_suscripcion"]),
                        PorcentajeParticipacion = row["porcentaje_participacion"] != DBNull.Value ? Convert.ToDecimal(row["porcentaje_participacion"]) : (decimal?)null,
                        NombreInversor = $"{row["nombre"]} {row["apellido"]}",
                        Cuit = row["cuit"].ToString()
                    });
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }
    }
}
