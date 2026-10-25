using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ContratoDAL
    {
        private Acceso acceso = new Acceso();

        // Texto que guarda la base (CHECK de Contrato.estado_contrato)
        internal static string EstadoATexto(EstadoContrato estado)
        {
            return estado == EstadoContrato.PorVencer ? "Por Vencer" : estado.ToString();
        }

        internal static EstadoContrato TextoAEstado(string texto)
        {
            switch (texto)
            {
                case "Activo": return EstadoContrato.Activo;
                case "Por Vencer": return EstadoContrato.PorVencer;
                case "Finalizado": return EstadoContrato.Finalizado;
                default: throw new ArgumentException($"Estado de contrato desconocido en la base: '{texto}'.");
            }
        }

        public int Crear(Contrato contrato)
        {
            acceso.Abrir();
            try
            {
                return Crear(contrato, acceso);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay). El SP siempre crea en estado Activo y sin alerta,
        // y pasa a Alquilado el inmueble del PN1 vinculado.
        internal int Crear(Contrato contrato, Acceso acceso)
        {
            SqlParameter outParam = acceso.CrearParametroOut("@NuevoId");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@FechaInicio", contrato.FechaInicio.Date),
                acceso.CrearParametro("@DuracionMeses", contrato.DuracionMeses),
                acceso.CrearParametro("@MontoMensual", contrato.MontoMensual),
                acceso.CrearParametro("@IdInquilino", contrato.IdInquilino),
                acceso.CrearParametro("@IdInmuebleAlquiler", contrato.IdInmuebleAlquiler),
                acceso.CrearParametro("@IdUsuarioAgente", contrato.IdUsuarioAgente),
                acceso.CrearParametro("@DepositoGarantia", contrato.DepositoGarantia),
                acceso.CrearParametro("@ComisionInmobiliaria", contrato.ComisionInmobiliaria),
                acceso.CrearParametro("@MetodoPago", contrato.MetodoPago.ToString()),
                acceso.CrearParametro("@IndiceActualizacion", contrato.IndiceActualizacion),
                acceso.CrearParametro("@PeriodicidadActualizacionMeses", contrato.PeriodicidadActualizacionMeses),
                outParam
            };

            int result = acceso.Escribir("CrearContrato", parameters);
            if (result != -1 && outParam.Value != null && outParam.Value != DBNull.Value)
            {
                contrato.IdContrato = Convert.ToInt32(outParam.Value);
                contrato.EstadoContrato = EstadoContrato.Activo;
                contrato.AlertaVencimiento = false;
                return contrato.IdContrato;
            }
            return -1;
        }

        public bool Actualizar(Contrato contrato)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", contrato.IdContrato),
                    acceso.CrearParametro("@FechaInicio", contrato.FechaInicio.Date),
                    acceso.CrearParametro("@DuracionMeses", contrato.DuracionMeses),
                    acceso.CrearParametro("@MontoMensual", contrato.MontoMensual),
                    acceso.CrearParametro("@EstadoContrato", EstadoATexto(contrato.EstadoContrato)),
                    acceso.CrearParametro("@AlertaVencimiento", contrato.AlertaVencimiento),
                    acceso.CrearParametro("@IdInquilino", contrato.IdInquilino),
                    acceso.CrearParametro("@IdInmuebleAlquiler", contrato.IdInmuebleAlquiler),
                    acceso.CrearParametro("@IdUsuarioAgente", contrato.IdUsuarioAgente),
                    acceso.CrearParametro("@AvisoDesocupacion", contrato.AvisoDesocupacion),
                    acceso.CrearParametro("@FechaAvisoDesocupacion", contrato.AvisoDesocupacion
                        ? (object)contrato.FechaAvisoDesocupacion ?? DBNull.Value
                        : DBNull.Value),
                    acceso.CrearParametro("@DepositoGarantia", contrato.DepositoGarantia),
                    acceso.CrearParametro("@ComisionInmobiliaria", contrato.ComisionInmobiliaria),
                    acceso.CrearParametro("@MetodoPago", contrato.MetodoPago.ToString()),
                    acceso.CrearParametro("@IndiceActualizacion", contrato.IndiceActualizacion),
                    acceso.CrearParametro("@PeriodicidadActualizacionMeses", contrato.PeriodicidadActualizacionMeses)
                };
                return acceso.Escribir("ActualizarContrato", parameters) != -1;
            }
            finally { acceso.Cerrar(); }
        }

        // Entrega del inmueble de un contrato finalizado. Devuelve false si no se actualizó (el SP ignora
        // contratos que no están finalizados).
        public bool RegistrarEntrega(int idContrato, string estadoEntrega, decimal montoDescuento, decimal montoDevolucion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    acceso.CrearParametro("@Id", idContrato),
                    acceso.CrearParametro("@EstadoEntrega", estadoEntrega),
                    acceso.CrearParametro("@MontoDescuentoDeposito", montoDescuento),
                    acceso.CrearParametro("@MontoDevolucionDeposito", montoDevolucion)
                };
                return acceso.Escribir("RegistrarEntregaContrato", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        // Marca el aviso de desocupación con la fecha actual. Devuelve false si no se actualizó
        // (el SP ignora contratos finalizados o que ya tienen aviso).
        public bool RegistrarAvisoDesocupacion(int idContrato)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idContrato) };
                return acceso.Escribir("RegistrarAvisoDesocupacion", parameters) > 0;
            }
            finally { acceso.Cerrar(); }
        }

        // Transacción: alta del contrato, sus garantes (contrato.Garantes) y, si no nace Activo (p. ej. un contrato
        // corto ya dentro del período de alerta), su estado inicial. El SP CrearContrato siempre crea en Activo y sin
        // alerta, y pasa a Alquilado el inmueble del PN1.
        // inquilinoNuevo: si se indica, se da de alta en la misma transacción y el contrato queda a su nombre.
        // idInmueble: inmueble del PN1 que se alquila; se usa (o se crea) su registro de alquiler y el contrato queda
        // sobre ese registro.
        public int CrearConEstado(Contrato contrato, Inquilino inquilinoNuevo = null, int? idInmueble = null)
        {
            EstadoContrato estado = contrato.EstadoContrato;
            bool alerta = contrato.AlertaVencimiento;
            int idInquilinoOriginal = contrato.IdInquilino;
            int idInmuebleAlquilerOriginal = contrato.IdInmuebleAlquiler;

            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                bool ok = true;
                if (inquilinoNuevo != null)
                {
                    ok = new InquilinoDAL().Crear(inquilinoNuevo, acceso) != -1;
                    contrato.IdInquilino = inquilinoNuevo.IdInquilino;
                }
                if (ok && idInmueble.HasValue)
                {
                    int idInmuebleAlquiler = new InmuebleAlquilerDAL().ObtenerDeInmueble(idInmueble.Value, acceso);
                    ok = idInmuebleAlquiler != -1;
                    contrato.IdInmuebleAlquiler = idInmuebleAlquiler;
                }
                if (ok) ok = Crear(contrato, acceso) != -1;

                GaranteDAL garanteDAL = new GaranteDAL();
                foreach (Garante garante in contrato.Garantes)
                {
                    if (!ok) break;
                    garante.IdContrato = contrato.IdContrato;
                    ok = garanteDAL.Crear(garante, acceso) != -1;
                }

                if (ok && estado != EstadoContrato.Activo)
                    ok = ActualizarEstado(contrato.IdContrato, estado, alerta, acceso);

                if (!ok)
                {
                    acceso.DeshacerTX();
                    RestaurarIds(contrato, idInquilinoOriginal, idInmuebleAlquilerOriginal, inquilinoNuevo);
                    return -1;
                }

                acceso.ConfirmarTX();
                contrato.EstadoContrato = estado;
                contrato.AlertaVencimiento = alerta;
                return contrato.IdContrato;
            }
            catch
            {
                acceso.DeshacerTX();
                RestaurarIds(contrato, idInquilinoOriginal, idInmuebleAlquilerOriginal, inquilinoNuevo);
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // Tras un rollback, los ids asignados no existen en la base: se limpian para que un reintento no los use
        private void RestaurarIds(Contrato contrato, int idInquilinoOriginal, int idInmuebleAlquilerOriginal, Inquilino inquilinoNuevo)
        {
            contrato.IdContrato = 0;
            contrato.IdInquilino = idInquilinoOriginal;
            contrato.IdInmuebleAlquiler = idInmuebleAlquilerOriginal;
            if (inquilinoNuevo != null) inquilinoNuevo.IdInquilino = 0;
            foreach (Garante garante in contrato.Garantes)
            {
                garante.IdGarante = 0;
                garante.IdContrato = 0;
            }
        }

        public bool ActualizarEstado(int idContrato, EstadoContrato estado, bool alertaVencimiento)
        {
            acceso.Abrir();
            try
            {
                return ActualizarEstado(idContrato, estado, alertaVencimiento, acceso);
            }
            finally { acceso.Cerrar(); }
        }

        // Rescisión anticipada: finaliza el contrato registrando quién la inició y la penalidad (null = sin penalidad)
        public bool Rescindir(int idContrato, IniciadorRescision iniciadaPor, decimal? penalidad)
        {
            acceso.Abrir();
            try
            {
                return ActualizarEstado(idContrato, EstadoContrato.Finalizado, false, acceso, iniciadaPor, penalidad);
            }
            finally { acceso.Cerrar(); }
        }

        // Usa una conexión ya abierta (y su transacción, si la hay). Al pasar a Finalizado, el SP devuelve a
        // Publicado el inmueble del PN1.
        internal bool ActualizarEstado(int idContrato, EstadoContrato estado, bool alertaVencimiento, Acceso acceso,
            IniciadorRescision? rescisionIniciadaPor = null, decimal? penalidadRescision = null)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                acceso.CrearParametro("@Id", idContrato),
                acceso.CrearParametro("@EstadoContrato", EstadoATexto(estado)),
                acceso.CrearParametro("@AlertaVencimiento", alertaVencimiento),
                acceso.CrearParametro("@RescisionIniciadaPor", rescisionIniciadaPor?.ToString()),
                acceso.CrearParametro("@PenalidadRescision", penalidadRescision)
            };
            return acceso.Escribir("ActualizarEstadoContrato", parameters) != -1;
        }

        // Pasa a Finalizado los vencidos y a Por Vencer (con alerta) los que terminan dentro de diasAnticipacion.
        // Devuelve la cantidad de contratos modificados, o -1 si falló.
        public int ActualizarVencimientos(int diasAnticipacion)
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@DiasAnticipacion", diasAnticipacion) };
                return acceso.Escribir("ActualizarVencimientoContratos", parameters);
            }
            finally { acceso.Cerrar(); }
        }

        // Transacción: baja de los garantes y del contrato. Si el contrato estaba vigente, el SP devuelve a
        // Publicado el inmueble del PN1.
        public bool Eliminar(int idContrato)
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();

                List<SqlParameter> parameters = new List<SqlParameter> { acceso.CrearParametro("@Id", idContrato) };
                if (!new GaranteDAL().EliminarPorContrato(idContrato, acceso) ||
                    acceso.Escribir("EliminarContrato", parameters) == -1)
                {
                    acceso.DeshacerTX();
                    return false;
                }

                acceso.ConfirmarTX();
                return true;
            }
            catch
            {
                acceso.DeshacerTX();
                throw;
            }
            finally { acceso.Cerrar(); }
        }

        public List<Contrato> LeerTodos()
        {
            return LeerLista("LeerContratos", null);
        }

        public Contrato BuscarPorId(int idContrato)
        {
            List<Contrato> lista = LeerLista("BuscarContratoPorId", new List<SqlParameter> { acceso.CrearParametro("@Id", idContrato) });
            return lista.Count > 0 ? lista[0] : null;
        }

        // Con inquilino e inmueble. estado null = todos.
        public List<Contrato> LeerDetalle(EstadoContrato? estado)
        {
            object filtro = estado.HasValue ? EstadoATexto(estado.Value) : null;
            return LeerLista("LeerContratosDetalle", new List<SqlParameter> { acceso.CrearParametro("@Estado", filtro) });
        }

        // Vigentes que terminan dentro de diasAnticipacion días, con inquilino (y su contacto) e inmueble
        public List<Contrato> LeerPorVencer(int diasAnticipacion)
        {
            return LeerLista("LeerContratosPorVencer", new List<SqlParameter> { acceso.CrearParametro("@DiasAnticipacion", diasAnticipacion) });
        }

        private List<Contrato> LeerLista(string storedProcedure, List<SqlParameter> parameters)
        {
            acceso.Abrir();
            try
            {
                DataTable dt = acceso.Leer(storedProcedure, parameters);
                List<Contrato> lista = new List<Contrato>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(Mapear(row));
                }
                return lista;
            }
            finally { acceso.Cerrar(); }
        }

        internal static Contrato Mapear(DataRow row)
        {
            DataColumnCollection cols = row.Table.Columns;
            Contrato contrato = new Contrato
            {
                IdContrato = Convert.ToInt32(row["id_contrato"]),
                FechaInicio = Convert.ToDateTime(row["fecha_inicio"]),
                DuracionMeses = Convert.ToInt32(row["duracion_meses"]),
                MontoMensual = Convert.ToDecimal(row["monto_mensual"]),
                EstadoContrato = TextoAEstado(row["estado_contrato"].ToString()),
                AlertaVencimiento = Convert.ToBoolean(row["alerta_vencimiento"]),
                IdInquilino = Convert.ToInt32(row["id_inquilino"]),
                IdInmuebleAlquiler = Convert.ToInt32(row["id_inmueble_alquiler"]),
                IdUsuarioAgente = Convert.ToInt32(row["id_usuario_agente"])
            };

            if (cols.Contains("aviso_desocupacion"))
            {
                contrato.AvisoDesocupacion = Convert.ToBoolean(row["aviso_desocupacion"]);
                contrato.FechaAvisoDesocupacion = row["fecha_aviso_desocupacion"] != DBNull.Value
                    ? Convert.ToDateTime(row["fecha_aviso_desocupacion"])
                    : (DateTime?)null;
            }

            if (cols.Contains("deposito_garantia"))
            {
                contrato.DepositoGarantia = Convert.ToDecimal(row["deposito_garantia"]);
                contrato.ComisionInmobiliaria = Convert.ToDecimal(row["comision_inmobiliaria"]);
                contrato.MetodoPago = (MetodoPago)Enum.Parse(typeof(MetodoPago), row["metodo_pago"].ToString());
                contrato.IndiceActualizacion = row["indice_actualizacion"].ToString();
                contrato.PeriodicidadActualizacionMeses = Convert.ToInt32(row["periodicidad_actualizacion_meses"]);
                contrato.RescisionIniciadaPor = row["rescision_iniciada_por"] != DBNull.Value
                    ? (IniciadorRescision)Enum.Parse(typeof(IniciadorRescision), row["rescision_iniciada_por"].ToString())
                    : (IniciadorRescision?)null;
                contrato.PenalidadRescision = DecimalONull(row["penalidad_rescision"]);
                contrato.EstadoEntrega = row["estado_entrega"] != DBNull.Value ? row["estado_entrega"].ToString() : null;
                contrato.MontoDescuentoDeposito = DecimalONull(row["monto_descuento_deposito"]);
                contrato.MontoDevolucionDeposito = DecimalONull(row["monto_devolucion_deposito"]);
            }

            // Consultas con detalle: el SP trae el nombre del inquilino ya concatenado
            if (cols.Contains("inquilino"))
            {
                contrato.Inquilino = new Inquilino
                {
                    IdInquilino = contrato.IdInquilino,
                    Nombre = row["inquilino"].ToString(),
                    Apellido = "",
                    Dni = cols.Contains("dni_inquilino") ? row["dni_inquilino"].ToString() : "",
                    Telefono = cols.Contains("telefono") && row["telefono"] != DBNull.Value ? row["telefono"].ToString() : "",
                    Email = cols.Contains("email") && row["email"] != DBNull.Value ? row["email"].ToString() : ""
                };
            }
            if (cols.Contains("direccion"))
            {
                contrato.InmuebleAlquiler = InmuebleAlquilerDAL.Mapear(row);
            }
            return contrato;
        }

        private static decimal? DecimalONull(object valor)
        {
            return valor != DBNull.Value ? Convert.ToDecimal(valor) : (decimal?)null;
        }
    }
}
