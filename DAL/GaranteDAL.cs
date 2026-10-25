using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class GaranteDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(Garante garante)
        {
            acceso.Abrir();
            try
            {
                return Crear(garante, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(Garante garante, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@IdContrato", garante.IdContrato),
                acceso.CrearParametro("@Nombre", garante.Nombre),
                acceso.CrearParametro("@Apellido", garante.Apellido),
                acceso.CrearParametro("@Dni", garante.Dni),
                acceso.CrearParametro("@Domicilio", garante.Domicilio),
                acceso.CrearParametro("@PropiedadGarantia", garante.PropiedadGarantia),
                outParam
            };

            int result = acceso.Escribir("CrearGarante", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                garante.IdGarante = Convert.ToInt32(outParam.Value);
                return garante.IdGarante;
            }
            return -1;
        }

        public bool Actualizar(Garante garante)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", garante.IdGarante),
                    acceso.CrearParametro("@Nombre", garante.Nombre),
                    acceso.CrearParametro("@Apellido", garante.Apellido),
                    acceso.CrearParametro("@Dni", garante.Dni),
                    acceso.CrearParametro("@Domicilio", garante.Domicilio),
                    acceso.CrearParametro("@PropiedadGarantia", garante.PropiedadGarantia)
                };
                return acceso.Escribir("ActualizarGarante", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Eliminar(int idGarante)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idGarante) };
                return acceso.Escribir("EliminarGarante", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay): parte de la baja del contrato
        internal bool EliminarPorContrato(int idContrato, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@IdContrato", idContrato) };
            return acceso.Escribir("EliminarGarantesPorContrato", parameters) != -1;
        }

        public List<Garante> LeerPorContrato(int idContrato)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@IdContrato", idContrato) };
                DataTable dt = acceso.Leer("LeerGarantesPorContrato", parameters);
                List<Garante> lista = new List<Garante>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public Garante BuscarPorId(int idGarante)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idGarante) };
                DataTable dt = acceso.Leer("BuscarGarantePorId", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private static Garante Mapear(DataRow row)
        {
            return new Garante
            {
                IdGarante = Convert.ToInt32(row["id_garante"]),
                IdContrato = Convert.ToInt32(row["id_contrato"]),
                Nombre = row["nombre"].ToString(),
                Apellido = row["apellido"].ToString(),
                Dni = row["dni"].ToString(),
                Domicilio = row["domicilio"].ToString(),
                PropiedadGarantia = row["propiedad_garantia"].ToString()
            };
        }
    }
}
