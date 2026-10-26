using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ProyectoInversionDAL
    {
        private Acceso acceso = new Acceso();

        // Nace En Análisis; la superficie del lote se toma del inmueble si no se indica
        public int Crear(ProyectoInversion proyecto)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdInmueble", proyecto.IdInmueble),
                    acceso.CrearParametro("@SuperficieLote", proyecto.SuperficieLote),
                    acceso.CrearParametro("@Observaciones", proyecto.Observaciones),
                    acceso.CrearParametro("@PorcentajeCanje", proyecto.PorcentajeCanje),
                    acceso.CrearParametro("@PorcentajeEstructuracion", proyecto.PorcentajeEstructuracion),
                    acceso.CrearParametro("@PorcentajeAdministracionMensual", proyecto.PorcentajeAdministracionMensual),
                    acceso.CrearParametro("@PorcentajeVentaUnidades", proyecto.PorcentajeVentaUnidades),
                    outParam
                };
                int result = acceso.Escribir("CrearProyectoInversion", parameters);
                if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
                {
                    proyecto.IdProyecto = Convert.ToInt32(outParam.Value);
                    proyecto.Estado = EstadoProyecto.EnAnalisis;
                    return proyecto.IdProyecto;
                }
                return -1;
            }
            finally { acceso.Cerrar(); }
        }

        // false si no se actualizó (el SP solo modifica proyectos En Análisis)
        public bool ActualizarDatosTecnicos(ProyectoInversion proyecto)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", proyecto.IdProyecto),
                    acceso.CrearParametro("@SuperficieLote", proyecto.SuperficieLote),
                    acceso.CrearParametro("@Fos", proyecto.Fos),
                    acceso.CrearParametro("@Fot", proyecto.Fot),
                    acceso.CrearParametro("@CostoObraM2", proyecto.CostoObraM2),
                    acceso.CrearParametro("@ValorVentaM2Unidades", proyecto.ValorVentaM2Unidades),
                    acceso.CrearParametro("@IdUsuarioArquitectura", proyecto.IdUsuarioArquitectura),
                    acceso.CrearParametro("@Observaciones", proyecto.Observaciones)
                };
                return acceso.Escribir("ActualizarDatosTecnicosProyecto", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        // Las tres comisiones de LandSmart; false si no se actualizó (el SP solo modifica proyectos En Análisis)
        public bool ActualizarComisiones(int idProyecto, decimal estructuracion, decimal administracionMensual, decimal ventaUnidades)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", idProyecto),
                    acceso.CrearParametro("@PorcentajeEstructuracion", estructuracion),
                    acceso.CrearParametro("@PorcentajeAdministracionMensual", administracionMensual),
                    acceso.CrearParametro("@PorcentajeVentaUnidades", ventaUnidades)
                };
                return acceso.Escribir("ActualizarComisionesProyecto", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        // Aprobado o Rechazado; false si no se registró (el SP solo decide sobre proyectos En Análisis)
        public bool RegistrarDecision(int idProyecto, EstadoProyecto estado, int idUsuarioDirectorio, string observaciones)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", idProyecto),
                    acceso.CrearParametro("@Estado", estado.ToString()),
                    acceso.CrearParametro("@IdUsuarioDirectorio", idUsuarioDirectorio),
                    acceso.CrearParametro("@Observaciones", observaciones)
                };
                return acceso.Escribir("RegistrarDecisionProyecto", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        // false si no se eliminó (solo En Análisis y sin suscripciones)
        public bool Eliminar(int idProyecto)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idProyecto) };
                return acceso.Escribir("EliminarProyectoInversion", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        // estado null = todos
        public List<ProyectoInversion> LeerTodos(EstadoProyecto? estado = null)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Estado", estado?.ToString()) };
                DataTable dt = acceso.Leer("LeerProyectosInversion", parameters);
                List<ProyectoInversion> lista = new List<ProyectoInversion>();
                foreach (DataRow row in dt.Rows) lista.Add(Mapear(row));
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public ProyectoInversion BuscarPorId(int idProyecto)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idProyecto) };
                DataTable dt = acceso.Leer("BuscarProyectoInversionPorId", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private static ProyectoInversion Mapear(DataRow row)
        {
            return new ProyectoInversion
            {
                IdProyecto = Convert.ToInt32(row["id_proyecto"]),
                IdInmueble = Convert.ToInt32(row["id_inmueble"]),
                SuperficieLote = DecimalONull(row["superficie_lote"]),
                Fos = DecimalONull(row["fos"]),
                Fot = DecimalONull(row["fot"]),
                MetrosConstruibles = DecimalONull(row["metros_construibles"]),
                CostoObraM2 = DecimalONull(row["costo_obra_m2"]),
                CostoTotalObra = DecimalONull(row["costo_total_obra"]),
                ValorVentaM2Unidades = DecimalONull(row["valor_venta_m2_unidades"]),
                ValorTotalUnidades = DecimalONull(row["valor_total_unidades"]),
                PorcentajeCanje = Convert.ToDecimal(row["porcentaje_canje"]),
                PorcentajeEstructuracion = Convert.ToDecimal(row["porcentaje_estructuracion"]),
                PorcentajeAdministracionMensual = Convert.ToDecimal(row["porcentaje_administracion_mensual"]),
                PorcentajeVentaUnidades = Convert.ToDecimal(row["porcentaje_venta_unidades"]),
                Estado = (EstadoProyecto)Enum.Parse(typeof(EstadoProyecto), row["estado"].ToString()),
                IdUsuarioArquitectura = row["id_usuario_arquitectura"] != DBNull.Value ? Convert.ToInt32(row["id_usuario_arquitectura"]) : (int?)null,
                IdUsuarioDirectorio = row["id_usuario_directorio"] != DBNull.Value ? Convert.ToInt32(row["id_usuario_directorio"]) : (int?)null,
                FechaCreacion = Convert.ToDateTime(row["fecha_creacion"]),
                FechaDecision = row["fecha_decision"] != DBNull.Value ? Convert.ToDateTime(row["fecha_decision"]) : (DateTime?)null,
                Observaciones = row["observaciones"] != DBNull.Value ? row["observaciones"].ToString() : null,
                Direccion = row["direccion"].ToString(),
                Localidad = row["localidad"] != DBNull.Value ? row["localidad"].ToString() : null,
                TipoInmueble = row["tipo_inmueble"].ToString(),
                TipoPublicacion = (TipoPublicacion)Enum.Parse(typeof(TipoPublicacion), row["tipo_publicacion"].ToString()),
                Latitud = DecimalONull(row["latitud"]),
                Longitud = DecimalONull(row["longitud"]),
                CapitalSuscripto = Convert.ToDecimal(row["capital_suscripto"]),
                CantidadInversores = Convert.ToInt32(row["inversores"])
            };
        }

        private static decimal? DecimalONull(object valor) => valor != DBNull.Value ? Convert.ToDecimal(valor) : (decimal?)null;
    }
}
