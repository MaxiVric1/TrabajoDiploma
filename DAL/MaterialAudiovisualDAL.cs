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
                return Crear(material, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(MaterialAudiovisual material, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@TipoArchivo", material.TipoArchivo),
                acceso.CrearParametro("@RutaArchivo", material.RutaArchivo),
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

        // Transacción: el texto comercial del inmueble y todos los archivos de una carga se guardan juntos o ninguno.
        // materiales puede estar vacío (solo se actualiza el texto).
        public bool CrearLote(int idInmueble, string textoComercial, List<MaterialAudiovisual> materiales)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                if (!new InmuebleDAL().ActualizarTextoComercial(idInmueble, textoComercial, acceso))
                {
                    acceso.DeshacerTX();
                    return false;
                }

                foreach (MaterialAudiovisual material in materiales)
                {
                    if (Crear(material, acceso) == -1)
                    {
                        acceso.DeshacerTX();
                        materiales.ForEach(m => m.IdMaterial = 0); // los ids asignados se revirtieron
                        return false;
                    }
                }

                acceso.ConfirmarTX();
                return true;
            }
            catch
            {
                acceso.DeshacerTX();
                materiales.ForEach(m => m.IdMaterial = 0);
                throw;
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
                FechaCarga = Convert.ToDateTime(row["fecha_carga"]),
                IdInmueble = Convert.ToInt32(row["id_inmueble"]),
                IdUsuarioMarketing = Convert.ToInt32(row["id_usuario_marketing"])
            };
        }
    }
}
