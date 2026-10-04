using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class MaterialAudiovisualDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(MaterialAudiovisual material)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@TipoArchivo", material.TipoArchivo),
                    acceso.CrearParametro("@RutaArchivo", material.RutaArchivo),
                    acceso.CrearParametro("@TextoComercial", material.TextoComercial),
                    acceso.CrearParametro("@IdInmueble", material.IdInmueble),
                    acceso.CrearParametro("@IdUsuarioMarketing", material.IdUsuarioMarketing),
                    outParam
                };

                int result = acceso.Escribir("CrearMaterialAudiovisual", parameters);
                if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
                {
                    material.IdMaterial = Convert.ToInt32(outParam.Value);
                    return material.IdMaterial;
                }
                return -1;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public bool Actualizar(MaterialAudiovisual material)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", material.IdMaterial),
                    acceso.CrearParametro("@TipoArchivo", material.TipoArchivo),
                    acceso.CrearParametro("@RutaArchivo", material.RutaArchivo),
                    acceso.CrearParametro("@TextoComercial", material.TextoComercial),
                    acceso.CrearParametro("@IdInmueble", material.IdInmueble),
                    acceso.CrearParametro("@IdUsuarioMarketing", material.IdUsuarioMarketing)
                };
                return acceso.Escribir("ActualizarMaterialAudiovisual", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Eliminar(int idMaterial)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idMaterial) };
                return acceso.Escribir("EliminarMaterialAudiovisual", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<MaterialAudiovisual> LeerTodos()
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer("LeerMaterialesAudiovisuales");
                List<MaterialAudiovisual> lista = new List<MaterialAudiovisual>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public List<MaterialAudiovisual> LeerPorInmueble(int idInmueble)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@IdInmueble", idInmueble) };
                DataTable dt = acceso.Leer("LeerMaterialPorInmueble", parameters);
                List<MaterialAudiovisual> lista = new List<MaterialAudiovisual>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        public MaterialAudiovisual BuscarPorId(int idMaterial)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idMaterial) };
                DataTable dt = acceso.Leer("BuscarMaterialAudiovisualPorId", parameters);
                return dt.Rows.Count > 0 ? Mapear(dt.Rows[0]) : null;
            }
            finally { acceso.Cerrar(); }
        }

        private MaterialAudiovisual Mapear(DataRow row)
        {
            return new MaterialAudiovisual
            {
                IdMaterial = Convert.ToInt32(row["id_material"]),
                TipoArchivo = row["tipo_archivo"].ToString(),
                RutaArchivo = row["ruta_archivo"].ToString(),
                TextoComercial = row["texto_comercial"] != DBNull.Value ? row["texto_comercial"].ToString() : "",
                FechaCarga = Convert.ToDateTime(row["fecha_carga"]),
                IdInmueble = Convert.ToInt32(row["id_inmueble"]),
                IdUsuarioMarketing = Convert.ToInt32(row["id_usuario_marketing"])
            };
        }
    }
}
