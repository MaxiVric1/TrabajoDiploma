using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ContactoCobranzaDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(ContactoCobranza contacto)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@FechaContacto", contacto.FechaContacto),
                    acceso.CrearParametro("@Descripcion", contacto.Descripcion),
                    acceso.CrearParametro("@IdContrato", contacto.IdContrato),
                    acceso.CrearParametro("@IdUsuarioAnalista", contacto.IdUsuarioAnalista),
                    outParam
                };

                int result = acceso.Escribir("CrearContactoCobranza", parameters);
                if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
                {
                    contacto.IdContacto = Convert.ToInt32(outParam.Value);
                    return contacto.IdContacto;
                }
                return -1;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public bool Actualizar(ContactoCobranza contacto)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", contacto.IdContacto),
                    acceso.CrearParametro("@FechaContacto", contacto.FechaContacto),
                    acceso.CrearParametro("@Descripcion", contacto.Descripcion)
                };
                return acceso.Escribir("ActualizarContactoCobranza", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Eliminar(int idContacto)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idContacto) };
                return acceso.Escribir("EliminarContactoCobranza", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<ContactoCobranza> LeerTodos()
        {
            return LeerLista("LeerContactosCobranza", null);
        }

        public ContactoCobranza BuscarPorId(int idContacto)
        {
            List<ContactoCobranza> lista = LeerLista("BuscarContactoCobranzaPorId", new List<SqlParameter> { acceso.CrearParametro("@Id", idContacto) });
            return lista.Count > 0 ? lista[0] : null;
        }

        // Historial del contrato, del más reciente al más antiguo, con el nombre del analista
        public List<ContactoCobranza> LeerPorContrato(int idContrato)
        {
            return LeerLista("LeerContactosPorContrato", new List<SqlParameter> { acceso.CrearParametro("@IdContrato", idContrato) });
        }

        private List<ContactoCobranza> LeerLista(string storedProcedure, List<SqlParameter> parameters)
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer(storedProcedure, parameters);
                List<ContactoCobranza> lista = new List<ContactoCobranza>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        private ContactoCobranza Mapear(DataRow row)
        {
            return new ContactoCobranza
            {
                IdContacto = Convert.ToInt32(row["id_contacto"]),
                FechaContacto = Convert.ToDateTime(row["fecha_contacto"]),
                Descripcion = row["descripcion"].ToString(),
                IdContrato = Convert.ToInt32(row["id_contrato"]),
                IdUsuarioAnalista = Convert.ToInt32(row["id_usuario_analista"]),
                Analista = row.Table.Columns.Contains("analista") ? row["analista"].ToString() : ""
            };
        }
    }
}
