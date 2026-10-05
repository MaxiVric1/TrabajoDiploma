using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class PagoAlquilerDAL
    {
        private Acceso acceso = new Acceso();

        // Texto que guarda la base (CHECK de PagoAlquiler.estado_pago)
        internal static string EstadoATexto(EstadoPago estado)
        {
            switch (estado)
            {
                case EstadoPago.AlDia: return "Al día";
                case EstadoPago.EnMora: return "En Mora";
                default: return "En Litigio";
            }
        }

        internal static EstadoPago TextoAEstado(string texto)
        {
            switch (texto)
            {
                case "Al día": return EstadoPago.AlDia;
                case "En Mora": return EstadoPago.EnMora;
                case "En Litigio": return EstadoPago.EnLitigio;
                default: throw new ArgumentException($"Estado de pago desconocido en la base: '{texto}'.");
            }
        }

        public int Crear(PagoAlquiler pago)
        {
            acceso.Abrir();
            try
            {
                return Crear(pago, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal int Crear(PagoAlquiler pago, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@MesCorrespondiente", pago.MesCorrespondiente),
                acceso.CrearParametro("@AnioCorrespondiente", pago.AnioCorrespondiente),
                acceso.CrearParametro("@FechaPago", pago.FechaPago),
                acceso.CrearParametro("@MontoPagado", pago.MontoPagado),
                acceso.CrearParametro("@EstadoPago", EstadoATexto(pago.EstadoPago)),
                acceso.CrearParametro("@RecargoAplicado", pago.RecargoAplicado),
                acceso.CrearParametro("@IdContrato", pago.IdContrato),
                acceso.CrearParametro("@IdUsuarioAnalista", pago.IdUsuarioAnalista),
                outParam
            };

            int result = acceso.Escribir("CrearPagoAlquiler", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                pago.IdPago = Convert.ToInt32(outParam.Value);
                return pago.IdPago;
            }
            return -1;
        }

        public bool Actualizar(PagoAlquiler pago)
        {
            acceso.Abrir();
            try
            {
                return Actualizar(pago, acceso);
            }
            finally { acceso.Cerrar(); }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal bool Actualizar(PagoAlquiler pago, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Id", pago.IdPago),
                acceso.CrearParametro("@MesCorrespondiente", pago.MesCorrespondiente),
                acceso.CrearParametro("@AnioCorrespondiente", pago.AnioCorrespondiente),
                acceso.CrearParametro("@FechaPago", pago.FechaPago),
                acceso.CrearParametro("@MontoPagado", pago.MontoPagado),
                acceso.CrearParametro("@EstadoPago", EstadoATexto(pago.EstadoPago)),
                acceso.CrearParametro("@RecargoAplicado", pago.RecargoAplicado),
                acceso.CrearParametro("@IdUsuarioAnalista", pago.IdUsuarioAnalista)
            };
            return acceso.Escribir("ActualizarPagoAlquiler", parameters) != -1;
        }

        public bool ActualizarEstado(int idPago, EstadoPago estado, decimal recargoAplicado)
        {
            acceso.Abrir();
            try
            {
                return ActualizarEstado(idPago, estado, recargoAplicado, acceso);
            }
            finally { acceso.Cerrar(); }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay)
        internal bool ActualizarEstado(int idPago, EstadoPago estado, decimal recargoAplicado, Acceso acceso)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Id", idPago),
                acceso.CrearParametro("@EstadoPago", EstadoATexto(estado)),
                acceso.CrearParametro("@RecargoAplicado", recargoAplicado)
            };
            return acceso.Escribir("ActualizarEstadoPago", parameters) != -1;
        }

        // Transacción: los registros nuevos (meses impagos) y los cambios de estado se guardan juntos o ninguno
        public bool GuardarLote(List<PagoAlquiler> nuevos, List<PagoAlquiler> cambiosDeEstado)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                bool ok = true;
                foreach (PagoAlquiler pago in nuevos)
                {
                    if (!ok) break;
                    ok = Crear(pago, acceso) != -1;
                }
                foreach (PagoAlquiler pago in cambiosDeEstado)
                {
                    if (!ok) break;
                    ok = ActualizarEstado(pago.IdPago, pago.EstadoPago, pago.RecargoAplicado, acceso);
                }

                if (!ok)
                {
                    acceso.DeshacerTX();
                    nuevos.ForEach(p => p.IdPago = 0); // los ids asignados se revirtieron
                    return false;
                }

                acceso.ConfirmarTX();
                return true;
            }
            catch
            {
                acceso.DeshacerTX();
                nuevos.ForEach(p => p.IdPago = 0);
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public bool Eliminar(int idPago)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idPago) };
                return acceso.Escribir("EliminarPagoAlquiler", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        public List<PagoAlquiler> LeerTodos()
        {
            return LeerLista("LeerPagosAlquiler", null);
        }

        public PagoAlquiler BuscarPorId(int idPago)
        {
            List<PagoAlquiler> lista = LeerLista("BuscarPagoAlquilerPorId", new List<SqlParameter> { acceso.CrearParametro("@Id", idPago) });
            return lista.Count > 0 ? lista[0] : null;
        }

        public List<PagoAlquiler> LeerPorContrato(int idContrato)
        {
            return LeerLista("LeerPagosPorContrato", new List<SqlParameter> { acceso.CrearParametro("@IdContrato", idContrato) });
        }

        public PagoAlquiler BuscarPorPeriodo(int idContrato, int mes, int anio)
        {
            List<PagoAlquiler> lista = LeerLista("BuscarPagoPorPeriodo", new List<SqlParameter>
            {
                acceso.CrearParametro("@IdContrato", idContrato),
                acceso.CrearParametro("@MesCorrespondiente", mes),
                acceso.CrearParametro("@AnioCorrespondiente", anio)
            });
            return lista.Count > 0 ? lista[0] : null;
        }

        // Con contrato, inquilino e inmueble. estado null = todos.
        public List<PagoAlquiler> LeerDetalle(EstadoPago? estado)
        {
            object filtro = estado.HasValue ? EstadoATexto(estado.Value) : null;
            return LeerLista("LeerPagosDetalle", new List<SqlParameter> { acceso.CrearParametro("@EstadoPago", filtro) });
        }

        private List<PagoAlquiler> LeerLista(string storedProcedure, List<SqlParameter> parameters)
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer(storedProcedure, parameters);
                List<PagoAlquiler> lista = new List<PagoAlquiler>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        private PagoAlquiler Mapear(DataRow row)
        {
            DataColumnCollection cols = row.Table.Columns;
            PagoAlquiler pago = new PagoAlquiler
            {
                IdPago = Convert.ToInt32(row["id_pago"]),
                MesCorrespondiente = Convert.ToInt32(row["mes_correspondiente"]),
                AnioCorrespondiente = Convert.ToInt32(row["anio_correspondiente"]),
                FechaPago = row["fecha_pago"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["fecha_pago"]) : null,
                MontoPagado = Convert.ToDecimal(row["monto_pagado"]),
                EstadoPago = TextoAEstado(row["estado_pago"].ToString()),
                RecargoAplicado = Convert.ToDecimal(row["recargo_aplicado"]),
                IdContrato = Convert.ToInt32(row["id_contrato"]),
                IdUsuarioAnalista = Convert.ToInt32(row["id_usuario_analista"])
            };

            // LeerPagosDetalle: datos del contrato, el inquilino (nombre ya concatenado) y el inmueble
            if (cols.Contains("monto_mensual"))
            {
                pago.Contrato = new Contrato
                {
                    IdContrato = pago.IdContrato,
                    MontoMensual = Convert.ToDecimal(row["monto_mensual"]),
                    EstadoContrato = ContratoDAL.TextoAEstado(row["estado_contrato"].ToString()),
                    Inquilino = new Inquilino
                    {
                        Nombre = row["inquilino"].ToString(),
                        Apellido = "",
                        Dni = row["dni_inquilino"].ToString(),
                        Telefono = row["telefono"] != DBNull.Value ? row["telefono"].ToString() : "",
                        Email = row["email"] != DBNull.Value ? row["email"].ToString() : ""
                    },
                    InmuebleAlquiler = new InmuebleAlquiler
                    {
                        Direccion = row["direccion"].ToString(),
                        TipoInmueble = (TipoInmueble)Enum.Parse(typeof(TipoInmueble), row["tipo_inmueble"].ToString())
                    }
                };
            }
            return pago;
        }
    }
}
