using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class ContratoBLL
    {
        // Días antes del vencimiento en que el contrato pasa a Por Vencer y se activa alerta_vencimiento
        public const int DIAS_ALERTA_VENCIMIENTO = 60;

        private ContratoDAL contratoDAL = new ContratoDAL();
        private InquilinoDAL inquilinoDAL = new InquilinoDAL();
        private InmuebleAlquilerDAL inmuebleDAL = new InmuebleAlquilerDAL();
        private InmuebleDAL inmueblePN1DAL = new InmuebleDAL();
        private GaranteDAL garanteDAL = new GaranteDAL();
        private PagoAlquilerDAL pagoDAL = new PagoAlquilerDAL();
        private AccionLegalDAL accionDAL = new AccionLegalDAL();

        // ---------- Reglas de vencimiento (mismo criterio que el SP ActualizarVencimientoContratos) ----------

        // Finalizado si ya terminó; Por Vencer si termina dentro de DIAS_ALERTA_VENCIMIENTO días; si no, Activo
        public static EstadoContrato EstadoSegunVencimiento(DateTime fechaFin, DateTime hoy)
        {
            if (fechaFin.Date < hoy.Date) return EstadoContrato.Finalizado;
            if (fechaFin.Date <= hoy.Date.AddDays(DIAS_ALERTA_VENCIMIENTO)) return EstadoContrato.PorVencer;
            return EstadoContrato.Activo;
        }

        // La alerta queda activa solo mientras el contrato está Por Vencer
        public static bool AlertaSegunEstado(EstadoContrato estado) => estado == EstadoContrato.PorVencer;

        public static int DiasRestantes(Contrato contrato, DateTime hoy) => (contrato.FechaFin.Date - hoy.Date).Days;

        // ---------- Operaciones ----------

        // Firma del contrato sobre un inmueble del PN1 (idInmueble): debe ser de alquiler, estar Publicado y sin
        // contrato vigente. En una sola transacción se guardan el contrato, sus garantes (contrato.Garantes) y, si
        // hace falta, el registro de alquiler del inmueble; el inmueble pasa a Alquilado.
        // inquilinoNuevo: para un inquilino que todavía no existe; se da de alta en la misma transacción. Si ya
        // existe uno con ese DNI se rechaza (hay que usar el existente).
        public int Registrar(Contrato contrato, int idInmueble, Inquilino inquilinoNuevo = null)
        {
            if (contrato != null && contrato.IdUsuarioAgente == 0 && SessionManager.Instance.EstaLogueado())
                contrato.IdUsuarioAgente = SessionManager.Instance.UsuarioActual.Id;

            if (inquilinoNuevo != null)
            {
                new InquilinoBLL().Validar(inquilinoNuevo);
                if (inquilinoDAL.BuscarPorDni(inquilinoNuevo.Dni) != null)
                    throw new ArgumentException($"Ya existe un inquilino con DNI {inquilinoNuevo.Dni}: búsquelo para usar sus datos.");
            }

            Validar(contrato, validarInquilino: inquilinoNuevo == null, validarInmueble: false);
            Inmueble inmueble = ValidarInmuebleParaAlquilar(idInmueble);
            ValidarGarantes(contrato, inquilinoNuevo != null ? inquilinoNuevo.Dni : inquilinoDAL.BuscarPorId(contrato.IdInquilino)?.Dni);

            EstadoContrato estadoInicial = EstadoSegunVencimiento(contrato.FechaFin, DateTime.Now);
            if (estadoInicial == EstadoContrato.Finalizado)
                throw new ArgumentException($"Con esa fecha de inicio y duración el contrato ya estaría vencido ({contrato.FechaFin:dd/MM/yyyy}).");

            // Un contrato corto puede nacer ya dentro del período de alerta: se guarda con ese estado en la misma transacción
            contrato.EstadoContrato = estadoInicial;
            contrato.AlertaVencimiento = AlertaSegunEstado(estadoInicial);
            int result = contratoDAL.CrearConEstado(contrato, inquilinoNuevo, idInmueble);
            if (result == -1)
                throw new Exception("No se pudo registrar el contrato. No se guardó ningún cambio.");

            string inquilino = inquilinoNuevo != null ? $"inquilino nuevo DNI {inquilinoNuevo.Dni}" : $"inquilino {contrato.IdInquilino}";
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Contrato",
                $"Contrato N° {result}: inmueble {idInmueble} ({inmueble.Direccion}, pasa a Alquilado), {inquilino}, " +
                $"{contrato.Garantes.Count} garante(s), {contrato.DuracionMeses} meses desde {contrato.FechaInicio:dd/MM/yyyy}, " +
                $"${contrato.MontoMensual:N2} mensuales, depósito ${contrato.DepositoGarantia:N2}, comisión ${contrato.ComisionInmobiliaria:N2}, " +
                $"pago por {contrato.MetodoPago}.");
            return result;
        }

        // Modifica condiciones del contrato; el estado y la alerta se recalculan según la nueva fecha de fin.
        // El inmueble no se puede cambiar (su estado en el PN1 depende del contrato).
        public void Actualizar(Contrato contrato)
        {
            Validar(contrato);
            Contrato actual = contratoDAL.BuscarPorId(contrato.IdContrato);
            if (actual == null)
                throw new Exception("El contrato no existe.");
            if (actual.EstadoContrato == EstadoContrato.Finalizado)
                throw new Exception("No se puede modificar un contrato finalizado.");
            if (contrato.IdInmuebleAlquiler != actual.IdInmuebleAlquiler)
                throw new ArgumentException("No se puede cambiar el inmueble de un contrato: finalícelo y registre uno nuevo.");

            contrato.EstadoContrato = EstadoSegunVencimiento(contrato.FechaFin, DateTime.Now);
            contrato.AlertaVencimiento = AlertaSegunEstado(contrato.EstadoContrato);
            // El aviso de desocupación solo cambia con RegistrarDesocupacion y Renovar
            contrato.AvisoDesocupacion = actual.AvisoDesocupacion;
            contrato.FechaAvisoDesocupacion = actual.FechaAvisoDesocupacion;

            if (!contratoDAL.Actualizar(contrato))
                throw new Exception("No se pudo actualizar el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Contrato",
                $"Contrato N° {contrato.IdContrato} actualizado (estado {contrato.EstadoContrato}).");
        }

        // Renovación: extiende el mismo contrato mesesAdicionales meses (corre la fecha de fin) con un nuevo monto
        // mensual. Solo para contratos vigentes. El estado y la alerta se recalculan según la nueva fecha de fin
        // (un contrato Por Vencer que se extiende vuelve a Activo). Si había aviso de desocupación, queda sin efecto
        // (el inquilino se queda). Devuelve el contrato actualizado.
        public Contrato Renovar(int idContrato, int mesesAdicionales, decimal nuevoMontoMensual)
        {
            if (mesesAdicionales <= 0)
                throw new ArgumentException("La renovación debe extender el contrato al menos un mes.");
            if (nuevoMontoMensual <= 0)
                throw new ArgumentException("El monto mensual debe ser mayor a cero.");

            Contrato contrato = contratoDAL.BuscarPorId(idContrato);
            if (contrato == null)
                throw new Exception("El contrato no existe.");
            if (!contrato.Vigente)
                throw new ArgumentException("Solo se puede renovar un contrato Activo o Por Vencer.");

            DateTime finAnterior = contrato.FechaFin;
            decimal montoAnterior = contrato.MontoMensual;
            bool teniaAviso = contrato.AvisoDesocupacion;

            contrato.DuracionMeses += mesesAdicionales;
            contrato.MontoMensual = nuevoMontoMensual;
            contrato.EstadoContrato = EstadoSegunVencimiento(contrato.FechaFin, DateTime.Now);
            contrato.AlertaVencimiento = AlertaSegunEstado(contrato.EstadoContrato);
            contrato.AvisoDesocupacion = false;
            contrato.FechaAvisoDesocupacion = null;

            if (!contratoDAL.Actualizar(contrato))
                throw new Exception("No se pudo renovar el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Renovación Contrato",
                $"Contrato N° {idContrato} renovado {mesesAdicionales} meses: vence {finAnterior:dd/MM/yyyy} -> {contrato.FechaFin:dd/MM/yyyy}, " +
                $"monto ${montoAnterior:N2} -> ${nuevoMontoMensual:N2} (estado {contrato.EstadoContrato})" +
                (teniaAviso ? "; se anula el aviso de desocupación." : "."));
            return contrato;
        }

        // Proceso de vencimientos: Activo -> Por Vencer (alerta) a 60 días; vencidos -> Finalizado.
        // Devuelve la cantidad de contratos modificados.
        public int ActualizarVencimientos()
        {
            int modificados = contratoDAL.ActualizarVencimientos(DIAS_ALERTA_VENCIMIENTO);
            if (modificados == -1)
                throw new Exception("No se pudieron actualizar los vencimientos de contratos.");

            if (modificados > 0)
                GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Vencimiento Contratos",
                    $"{modificados} contrato(s) actualizados por vencimiento (alerta a {DIAS_ALERTA_VENCIMIENTO} días).");
            return modificados;
        }

        // Finalización anticipada (desalojo u otra causa sin rescisión): libera el inmueble para un nuevo contrato
        // y el inmueble del PN1 vuelve a Publicado
        public void Finalizar(int idContrato)
        {
            Contrato contrato = ContratoVigente(idContrato);

            if (!contratoDAL.ActualizarEstado(idContrato, EstadoContrato.Finalizado, false))
                throw new Exception("No se pudo finalizar el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Finalización Contrato",
                $"Contrato N° {idContrato} finalizado antes de su vencimiento ({contrato.FechaFin:dd/MM/yyyy}).");
        }

        // Rescisión anticipada: finaliza el contrato registrando quién la inició y la penalidad (null o 0 = sin
        // penalidad). El inmueble del PN1 vuelve a Publicado.
        public void Rescindir(int idContrato, IniciadorRescision iniciadaPor, decimal? penalidad)
        {
            if (!Enum.IsDefined(typeof(IniciadorRescision), iniciadaPor))
                throw new ArgumentException("Indique quién inició la rescisión.");
            if (penalidad < 0)
                throw new ArgumentException("La penalidad no puede ser negativa.");
            Contrato contrato = ContratoVigente(idContrato);

            if (!contratoDAL.Rescindir(idContrato, iniciadaPor, penalidad))
                throw new Exception("No se pudo rescindir el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Rescisión Contrato",
                $"Contrato N° {idContrato} rescindido por el {iniciadaPor.ToString().ToLower()} antes de su vencimiento " +
                $"({contrato.FechaFin:dd/MM/yyyy})" + (penalidad > 0 ? $", penalidad ${penalidad:N2}." : ", sin penalidad."));
        }

        // Entrega del inmueble de un contrato finalizado: estado en que se recibió y cuánto del depósito se descuenta;
        // el resto se devuelve al inquilino. Se registra una sola vez. Devuelve el monto a devolver.
        public decimal RegistrarEntrega(int idContrato, string estadoEntrega, decimal montoDescuento)
        {
            if (string.IsNullOrWhiteSpace(estadoEntrega))
                throw new ArgumentException("Describa el estado en que se entregó el inmueble.");
            estadoEntrega = estadoEntrega.Trim();
            if (estadoEntrega.Length > 1000)
                throw new ArgumentException("La descripción de la entrega no puede superar los 1000 caracteres.");

            Contrato contrato = contratoDAL.BuscarPorId(idContrato);
            if (contrato == null)
                throw new Exception("El contrato no existe.");
            if (contrato.Vigente)
                throw new ArgumentException("La entrega del inmueble se registra cuando el contrato está finalizado.");
            if (contrato.EstadoEntrega != null)
                throw new ArgumentException($"El Contrato N° {idContrato} ya tiene registrada la entrega del inmueble.");
            if (montoDescuento < 0 || montoDescuento > contrato.DepositoGarantia)
                throw new ArgumentException($"El descuento debe estar entre $0 y el depósito de garantía (${contrato.DepositoGarantia:N2}).");

            decimal devolucion = contrato.DepositoGarantia - montoDescuento;
            if (!contratoDAL.RegistrarEntrega(idContrato, estadoEntrega, montoDescuento, devolucion))
                throw new Exception("No se pudo registrar la entrega del inmueble.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Entrega Inmueble",
                $"Contrato N° {idContrato}: depósito ${contrato.DepositoGarantia:N2}, descuento ${montoDescuento:N2}, " +
                $"devolución ${devolucion:N2}.");
            return devolucion;
        }

        // Baja del contrato junto con sus garantes (una transacción). Si estaba vigente, el inmueble del PN1 vuelve
        // a Publicado.
        public void Eliminar(int idContrato)
        {
            if (pagoDAL.LeerPorContrato(idContrato).Count > 0)
                throw new Exception("No se puede eliminar el contrato porque tiene pagos registrados.");
            if (accionDAL.LeerPorContrato(idContrato).Count > 0)
                throw new Exception("No se puede eliminar el contrato porque tiene acciones legales registradas.");

            int garantes = garanteDAL.LeerPorContrato(idContrato).Count;
            if (!contratoDAL.Eliminar(idContrato))
                throw new Exception("No se pudo eliminar el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Contrato",
                $"Contrato N° {idContrato} eliminado junto con {garantes} garante(s).");
        }

        // Aviso de que el inquilino desocupa el inmueble al vencimiento: marca aviso_desocupacion con la fecha actual.
        // El estado no cambia (el contrato sigue vigente hasta que vence). La observación queda en la bitácora.
        public void RegistrarDesocupacion(int idContrato, string observacion)
        {
            Contrato contrato = contratoDAL.BuscarPorId(idContrato);
            if (contrato == null)
                throw new Exception("El contrato no existe.");
            if (!contrato.Vigente)
                throw new ArgumentException("Solo se puede registrar la desocupación de un contrato Activo o Por Vencer.");
            if (contrato.AvisoDesocupacion)
                throw new ArgumentException($"El Contrato N° {idContrato} ya tiene registrado un aviso de desocupación " +
                    $"(desde el {contrato.FechaAvisoDesocupacion:dd/MM/yyyy}).");

            if (!contratoDAL.RegistrarAvisoDesocupacion(idContrato))
                throw new Exception("No se pudo registrar el aviso de desocupación.");

            string info = $"Contrato N° {idContrato}: el inquilino desocupará el inmueble al vencimiento ({contrato.FechaFin:dd/MM/yyyy}).";
            if (!string.IsNullOrWhiteSpace(observacion))
                info += " Observación: " + observacion.Trim();
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Desocupación Contrato", info);
        }

        public List<Contrato> LeerTodos() => contratoDAL.LeerTodos();

        public Contrato BuscarPorId(int idContrato) => contratoDAL.BuscarPorId(idContrato);

        // Con inquilino e inmueble; estado null = todos
        public List<Contrato> LeerDetalle(EstadoContrato? estado = null) => contratoDAL.LeerDetalle(estado);

        // Vigentes que vencen dentro de DIAS_ALERTA_VENCIMIENTO días, con el contacto del inquilino
        public List<Contrato> LeerPorVencer() => contratoDAL.LeerPorVencer(DIAS_ALERTA_VENCIMIENTO);

        private Contrato ContratoVigente(int idContrato)
        {
            Contrato contrato = contratoDAL.BuscarPorId(idContrato);
            if (contrato == null)
                throw new Exception("El contrato no existe.");
            if (contrato.EstadoContrato == EstadoContrato.Finalizado)
                throw new Exception("El contrato ya está finalizado.");
            return contrato;
        }

        // validarInquilino = false cuando el inquilino se crea en la misma transacción y todavía no existe;
        // validarInmueble = false al registrar (el registro de alquiler sale del inmueble del PN1 en la transacción)
        private void Validar(Contrato contrato, bool validarInquilino = true, bool validarInmueble = true)
        {
            if (contrato == null) throw new ArgumentNullException(nameof(contrato));
            if (contrato.FechaInicio == default(DateTime))
                throw new ArgumentException("La fecha de inicio es obligatoria.");
            if (contrato.DuracionMeses <= 0)
                throw new ArgumentException("La duración debe ser de al menos un mes.");
            if (contrato.MontoMensual <= 0)
                throw new ArgumentException("El monto mensual debe ser mayor a cero.");
            if (contrato.DepositoGarantia < 0)
                throw new ArgumentException("El depósito de garantía no puede ser negativo.");
            if (contrato.ComisionInmobiliaria < 0)
                throw new ArgumentException("La comisión inmobiliaria no puede ser negativa.");
            if (!Enum.IsDefined(typeof(MetodoPago), contrato.MetodoPago))
                throw new ArgumentException("El método de pago no es válido.");
            if (contrato.IndiceActualizacion != "IPC")
                throw new ArgumentException("El índice de actualización debe ser IPC.");
            if (contrato.PeriodicidadActualizacionMeses < 1 || contrato.PeriodicidadActualizacionMeses > 12)
                throw new ArgumentException("La actualización del monto debe ser cada 1 a 12 meses.");
            if (contrato.IdUsuarioAgente <= 0)
                throw new ArgumentException("El contrato debe tener un agente asignado.");
            if (validarInquilino && inquilinoDAL.BuscarPorId(contrato.IdInquilino) == null)
                throw new ArgumentException("El inquilino indicado no existe.");
            if (validarInmueble && inmuebleDAL.BuscarPorId(contrato.IdInmuebleAlquiler) == null)
                throw new ArgumentException("El inmueble indicado no existe.");

            contrato.FechaInicio = contrato.FechaInicio.Date;
        }

        // El inmueble del PN1 que se alquila: de alquiler, Publicado y sin contrato vigente
        private Inmueble ValidarInmuebleParaAlquilar(int idInmueble)
        {
            Inmueble inmueble = inmueblePN1DAL.BuscarPorId(idInmueble);
            if (inmueble == null)
                throw new ArgumentException("El inmueble indicado no existe.");
            if (inmueble.TipoPublicacion != TipoPublicacion.Alquiler)
                throw new ArgumentException("El inmueble está publicado para venta, no para alquiler.");
            if (inmueble.EstadoActual == EstadoInmueble.Alquilado)
                throw new ArgumentException("El inmueble ya está alquilado.");
            if (inmueble.EstadoActual != EstadoInmueble.Publicado)
                throw new ArgumentException("Solo se puede alquilar un inmueble Publicado.");
            if (!inmueblePN1DAL.LeerDisponiblesParaAlquiler().Any(i => i.IdInmueble == idInmueble))
                throw new ArgumentException("El inmueble ya tiene un contrato vigente.");
            return inmueble;
        }

        // Garantes del contrato que se registra: datos completos, sin repetir DNI y distintos del inquilino
        private void ValidarGarantes(Contrato contrato, string dniInquilino)
        {
            if (contrato.Garantes == null) contrato.Garantes = new List<Garante>();
            GaranteBLL garanteBLL = new GaranteBLL();
            foreach (Garante garante in contrato.Garantes)
            {
                garanteBLL.Validar(garante);
                if (garante.Dni == dniInquilino?.Trim())
                    throw new ArgumentException("El inquilino no puede ser garante de su propio contrato.");
            }
            string repetido = contrato.Garantes.GroupBy(g => g.Dni).Where(g => g.Count() > 1).Select(g => g.Key).FirstOrDefault();
            if (repetido != null)
                throw new ArgumentException($"El garante con DNI {repetido} está cargado más de una vez.");
        }
    }
}
