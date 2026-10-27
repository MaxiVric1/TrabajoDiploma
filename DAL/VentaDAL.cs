using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class VentaDAL
    {
        private Acceso acceso = new Acceso();

        // El inmueble pasa a Vendido en la misma transacción; -1 si no está publicado en venta o ya tiene venta
        public int Registrar(Venta venta)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@IdInmueble", venta.IdInmueble),
                    acceso.CrearParametro("@FechaVenta", venta.FechaVenta.Date),
                    acceso.CrearParametro("@PrecioUsd", venta.PrecioUsd),
                    acceso.CrearParametro("@IdUsuarioRegistro", venta.IdUsuarioRegistro),
                    acceso.CrearParametro("@Observaciones", venta.Observaciones),
                    outParam
                };
                int result = acceso.Escribir("RegistrarVenta", parameters);
                if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
                {
                    venta.IdVenta = Convert.ToInt32(outParam.Value);
                    return venta.IdVenta;
                }
                return -1;
            }
            finally { acceso.Cerrar(); }
        }

        // Ventas por agente entre dos fechas (null = sin límite), sin ordenar
        public List<RankingAgente> LeerRanking(DateTime? desde, DateTime? hasta)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Desde", desde?.Date),
                    acceso.CrearParametro("@Hasta", hasta?.Date)
                };
                DataTable dt = acceso.Leer("LeerRankingAgentes", parameters);
                List<RankingAgente> lista = new List<RankingAgente>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new RankingAgente
                    {
                        IdUsuario = Convert.ToInt32(row["id_usuario"]),
                        Usuario = row["usuario"].ToString(),
                        NombrePersona = row["nombre_persona"] != DBNull.Value ? row["nombre_persona"].ToString() : null,
                        Apellido = row["apellido"] != DBNull.Value ? row["apellido"].ToString() : null,
                        Dni = row["dni"] != DBNull.Value ? row["dni"].ToString() : null,
                        Cantidad = Convert.ToInt32(row["cantidad"]),
                        TotalUsd = Convert.ToDecimal(row["total_usd"])
                    });
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }
    }
}
