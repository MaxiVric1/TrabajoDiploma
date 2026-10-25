using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class Acceso
    {
        private SqlConnection conexion;
        private SqlTransaction transaccion;

        // Nombre de la cadena en <connectionStrings> del .config del ejecutable (TpIngSoft.exe.config, BackupCLI.exe.config)
        internal const string NOMBRE_CONEXION = ServidorSql.NOMBRE_CONEXION;

        // Instancia de SQL Server detectada automáticamente (App.config queda como alternativa); también la usa MapperBackup
        internal static string CadenaConexion
        {
            get
            {
                //ConnectionStringSettings cadena = ConfigurationManager.ConnectionStrings[NOMBRE_CONEXION];
                //if (cadena == null || string.IsNullOrWhiteSpace(cadena.ConnectionString))
                //    throw new ConfigurationErrorsException(
                //        $"Falta la cadena de conexión '{NOMBRE_CONEXION}' en <connectionStrings> del archivo .config de la aplicación.");
                //return cadena.ConnectionString;
                return ServidorSql.Detectar();
            }
        }

        public void Abrir()
        {
            conexion = new SqlConnection
            {
                //ConnectionString = @"Data Source=NOTE-MAX\SQLEXPRESS;Initial Catalog=tpingsoft;Integrated Security=True;"
                //ConnectionString = "Data Source=localhost;Initial Catalog=tpingsoft;Integrated Security=True;"
                //ConnectionString = CadenaConexion
                ConnectionString = ServidorSql.Detectar()
            };
            try
            {
                conexion.Open();
            }
            catch (SqlException ex)
            {
                LogConexion.Escribir($"Error al abrir la conexión ({LogConexion.SinDatosSensibles(conexion.ConnectionString)}) " +
                                     $"con el usuario {LogConexion.UsuarioWindows}: {LogConexion.Detalle(ex)}");
                throw;
            }
        }

        public void Cerrar()
        {
            // Una transacción sin confirmar se revierte al cerrar la conexión; no debe sobrevivir a la próxima apertura
            transaccion = null;
            conexion.Close();
            conexion = null;
            GC.Collect();
        }

        public void IniciarTx()
        {
            transaccion = conexion.BeginTransaction();
        }

        public void ConfirmarTX()
        {
            transaccion.Commit();
            transaccion = null;
        }

        public void DeshacerTX()
        {
            if (transaccion == null) return;
            try
            {
                transaccion.Rollback();
            }
            catch (InvalidOperationException)
            {
                // SQL Server ya abortó la transacción por un error grave: no queda nada por revertir
            }
            finally
            {
                transaccion = null;
            }
        }


        private SqlCommand CrearComando(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand cmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = sql,
                Connection = conexion
            };
            if (transaccion != null)
            {
                cmd.Transaction = transaccion;
            }
            if (parametros != null)
            {
                cmd.Parameters.AddRange(parametros.ToArray());
            }
            return cmd;
        }

        public int Escribir(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand cmd = CrearComando(sql, parametros);
            int filas;
            try
            {
                filas = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                filas = -1;
            }
            cmd.Parameters.Clear();
            return filas;
        }


        public DataTable Leer(string sql, List<SqlParameter> parametros = null)
        {
            SqlDataAdapter adaptador = new SqlDataAdapter
            {
                SelectCommand = CrearComando(sql, parametros)
            };
            DataTable tabla = new DataTable();

            adaptador.Fill(tabla);

            return tabla;

        }

        public SqlParameter CrearParametro(string nombre, object valor)
        {
            SqlParameter p = new SqlParameter
            {
                ParameterName = nombre,
                Value = valor ?? DBNull.Value
            };

            return p;
        }

        public SqlParameter CrearParametroOut(string nombre)
        {
            SqlParameter p = new SqlParameter
            {
                ParameterName = nombre,
                Direction = ParameterDirection.Output,
                DbType = DbType.Int32
            };
            return p;
        }

    }
}
