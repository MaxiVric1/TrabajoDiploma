using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class InversorDAL
    {
        private Acceso acceso = new Acceso();

        public int Crear(Inversor inversor)
        {
            acceso.Abrir();
            try
            {
                SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Nombre", inversor.Nombre),
                    acceso.CrearParametro("@Apellido", inversor.Apellido),
                    acceso.CrearParametro("@Cuit", inversor.Cuit),
                    acceso.CrearParametro("@Email", inversor.Email),
                    acceso.CrearParametro("@Telefono", inversor.Telefono),
                    outParam
                };
                int result = acceso.Escribir("CrearInversor", parameters);
                if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
                {
                    inversor.IdInversor = Convert.ToInt32(outParam.Value);
                    return inversor.IdInversor;
                }
                return -1;
            }
            finally { acceso.Cerrar(); }
        }

        public bool Actualizar(Inversor inversor)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", inversor.IdInversor),
                    acceso.CrearParametro("@Nombre", inversor.Nombre),
                    acceso.CrearParametro("@Apellido", inversor.Apellido),
                    acceso.CrearParametro("@Cuit", inversor.Cuit),
                    acceso.CrearParametro("@Email", inversor.Email),
                    acceso.CrearParametro("@Telefono", inversor.Telefono)
                };
                return acceso.Escribir("ActualizarInversor", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        // false si no se eliminó (tiene aportes)
        public bool Eliminar(int idInversor)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idInversor) };
                return acceso.Escribir("EliminarInversor", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Inversor> LeerTodos()
        {
            return LeerLista("LeerInversores", null);
        }

        public Inversor BuscarPorId(int idInversor)
        {
            List<Inversor> lista = LeerLista("BuscarInversorPorId", new List<SqlParameter> { acceso.CrearParametro("@Id", idInversor) });
            return lista.Count > 0 ? lista[0] : null;
        }

        public Inversor BuscarPorCuit(string cuit)
        {
            List<Inversor> lista = LeerLista("BuscarInversorPorCuit", new List<SqlParameter> { acceso.CrearParametro("@Cuit", cuit) });
            return lista.Count > 0 ? lista[0] : null;
        }

        private List<Inversor> LeerLista(string storedProcedure, List<SqlParameter> parameters)
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer(storedProcedure, parameters);
                List<Inversor> lista = new List<Inversor>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new Inversor
                    {
                        IdInversor = Convert.ToInt32(row["id_inversor"]),
                        Nombre = row["nombre"].ToString(),
                        Apellido = row["apellido"].ToString(),
                        Cuit = row["cuit"].ToString(),
                        Email = row["email"] != DBNull.Value ? row["email"].ToString() : null,
                        Telefono = row["telefono"] != DBNull.Value ? row["telefono"].ToString() : null
                    });
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }
    }
}
