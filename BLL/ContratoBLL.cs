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

        // ---------- Condiciones económicas ----------

        public const int MESES_DEPOSITO = 1;                        // depósito de garantía = 1 mes de alquiler
        public const decimal PORCENTAJE_COMISION = 4m;              // comisión: 4% que se retiene de cada pago mensual
        public const int PERIODICIDAD_IPC_MESES = 3;                // actualización por IPC cada 3 meses
        public const decimal PORCENTAJE_PENALIDAD_RESCISION = 0.10m; // rescisión = 10% del canon restante

        public static decimal CalcularDeposito(decimal montoMensual) => Math.Round(montoMensual * MESES_DEPOSITO, 2);

        // Sobre el valor total del contrato (monto mensual x duración)
        // Comisión de un pago (porcentaje del contrato sobre lo que paga el inquilino) y neto al propietario
        public static decimal CalcularComision(decimal montoPagado, decimal porcentaje) => Math.Round(montoPagado * porcentaje / 100, 2);

        public static decimal NetoPropietario(decimal montoPagado, decimal porcentaje) => montoPagado - CalcularComision(montoPagado, porcentaje);

        // Meses del contrato posteriores al mes en curso: los que quedan sin devengar si se rescinde hoy
        public static int MesesRestantes(Contrato contrato, DateTime hoy)
        {
            DateTime mesEnCurso = new DateTime(hoy.Year, hoy.Month, 1);
            return PagoAlquilerBLL.PeriodosDelContrato(contrato).Count(p => new DateTime(p.Anio, p.Mes, 1) > mesEnCurso);
        }

        public static decimal CanonRestante(Contrato contrato, DateTime hoy) => contrato.MontoMensual * MesesRestantes(contrato, hoy);

        public static decimal CalcularPenalidadRescision(Contrato contrato, DateTime hoy) =>
            Math.Round(CanonRestante(contrato, hoy) * PORCENTAJE_PENALIDAD_RESCISION, 2);

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
                if (inquilinoNuevo.Domicilio == null)
                    throw new ArgumentException("El domicilio del inquilino es obligatorio: figura en el contrato.");
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
                $"${contrato.MontoMensual:N2} mensuales, depósito ${contrato.DepositoGarantia:N2}, comisión {contrato.ComisionInmobiliaria:0.##}% mensual, " +
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

        // Rescisión anticipada: finaliza el contrato registrando quién la inició y la penalidad, que se calcula sola
        // (PORCENTAJE_PENALIDAD_RESCISION del canon restante, a cargo de quien la inicia). El inmueble del PN1 vuelve
        // a Publicado. Devuelve la penalidad aplicada.
        public decimal Rescindir(int idContrato, IniciadorRescision iniciadaPor)
        {
            if (!Enum.IsDefined(typeof(IniciadorRescision), iniciadaPor))
                throw new ArgumentException("Indique quién inició la rescisión.");
            Contrato contrato = ContratoVigente(idContrato);
            int mesesRestantes = MesesRestantes(contrato, DateTime.Now);
            decimal penalidad = CalcularPenalidadRescision(contrato, DateTime.Now);

            if (!contratoDAL.Rescindir(idContrato, iniciadaPor, penalidad))
                throw new Exception("No se pudo rescindir el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Rescisión Contrato",
                $"Contrato N° {idContrato} rescindido por el {iniciadaPor.ToString().ToLower()} antes de su vencimiento " +
                $"({contrato.FechaFin:dd/MM/yyyy}): {mesesRestantes} mes(es) restantes de ${contrato.MontoMensual:N2}, " +
                $"penalidad ${penalidad:N2} ({PORCENTAJE_PENALIDAD_RESCISION:P0} del canon restante).");
            return penalidad;
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

        // ---------- Documento PDF ----------

        // La firma es el nombre y apellido del inquilino tipeados (en cualquier orden; sin distinguir mayúsculas,
        // acentos ni espacios de más)
        public static bool FirmaCoincide(string firma, Inquilino inquilino)
        {
            if (string.IsNullOrWhiteSpace(firma) || inquilino == null) return false;
            string f = Normalizar(firma);
            return f == Normalizar(inquilino.Nombre + " " + inquilino.Apellido) || f == Normalizar(inquilino.Apellido + " " + inquilino.Nombre);
        }

        private static string Normalizar(string texto)
        {
            string sinAcentos = new string(texto.Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray());
            return string.Join(" ", sinAcentos.ToLowerInvariant().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        // Genera el contrato registrado en PDF con los datos guardados: encabezado con el logo de LandSmart, partes,
        // inmueble, condiciones económicas, duración, cláusulas y firmas. La firma del locatario es su nombre y apellido
        // tipeados en el formulario y debe coincidir con los del inquilino.
        public void GenerarPdf(int idContrato, string firmaInquilino, string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta))
                throw new ArgumentException("Indique dónde guardar el PDF.");
            Contrato contrato = contratoDAL.BuscarPorId(idContrato);
            if (contrato == null)
                throw new Exception("El contrato no existe.");
            Inquilino inquilino = inquilinoDAL.BuscarPorId(contrato.IdInquilino);
            if (!FirmaCoincide(firmaInquilino, inquilino))
                throw new ArgumentException($"La firma debe ser el nombre y apellido del inquilino ({inquilino?.Nombre} {inquilino?.Apellido}).");

            InmuebleAlquiler alquiler = inmuebleDAL.BuscarPorId(contrato.IdInmuebleAlquiler);
            Inmueble inmueble = alquiler?.IdInmueble != null ? inmueblePN1DAL.BuscarPorId(alquiler.IdInmueble.Value) : null;
            Cliente propietario = inmueble != null ? new ClienteDAL().BuscarPorId(inmueble.IdCliente) : null;
            List<Garante> garantes = garanteDAL.LeerPorContrato(idContrato);
            // El monto inicial es el anterior a la primera actualización por IPC (si ya hubo alguna)
            List<ActualizacionContrato> actualizaciones = new ActualizacionContratoDAL().LeerPorContrato(idContrato);
            decimal montoInicial = actualizaciones.Count > 0 ? actualizaciones[0].MontoAnterior : contrato.MontoMensual;
            string nombrePropietario = propietario != null ? $"{propietario.Nombre} {propietario.Apellido}" : "-";
            string nombreInquilino = $"{inquilino.Nombre} {inquilino.Apellido}";

            DocumentoPdf pdf = new DocumentoPdf($"LandSmart - Contrato de locación N° {idContrato}");
            pdf.Encabezado(Logo(), "CONTRATO DE LOCACIÓN", $"Contrato N° {idContrato}", $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");

            pdf.Subtitulo("1. Las partes");
            pdf.Campo("Locador (propietario)", propietario != null ? $"{nombrePropietario}, DNI {propietario.Dni}" : "-");
            pdf.Campo("Locatario (inquilino)", $"{nombreInquilino}, DNI {inquilino.Dni}");
            pdf.Campo("Domicilio del locatario", inquilino.Domicilio);
            foreach (Garante g in garantes)
            {
                pdf.Campo("Garante", $"{g.Nombre} {g.Apellido}, DNI {g.Dni}");
                pdf.Campo("Domicilio del garante", g.Domicilio);
                pdf.Campo("Propiedad en garantía", g.PropiedadGarantia);
            }
            pdf.Espacio(3);
            pdf.Parrafo("LandSmart actúa como inmobiliaria intermediaria entre las partes: administra el contrato, percibe los " +
                        "pagos del locatario y rinde al locador lo cobrado, descontada su comisión.");

            pdf.Subtitulo("2. El inmueble");
            pdf.Campo("Dirección", inmueble?.Direccion ?? alquiler?.Direccion);
            pdf.Campo("Tipo", inmueble?.TipoInmueble ?? alquiler?.TipoInmueble.ToString());
            pdf.Campo("Ambientes", inmueble != null ? inmueble.CantidadAmbientes.ToString() : "-");
            pdf.Campo("Superficie", inmueble != null ? $"{inmueble.MetrosCuadrados:N0} m²" : "-");

            pdf.Subtitulo("3. Condiciones económicas");
            pdf.Campo("Monto mensual inicial", $"${montoInicial:N2}, pagadero del 1 al {PagoAlquilerBLL.DIA_VENCIMIENTO} de cada mes");
            pdf.Campo("Depósito de garantía", $"${contrato.DepositoGarantia:N2} ({MESES_DEPOSITO} mes de alquiler)");
            pdf.Campo("Comisión inmobiliaria", $"{contrato.ComisionInmobiliaria:0.##}% mensual, retenido de cada pago " +
                $"(${CalcularComision(montoInicial, contrato.ComisionInmobiliaria):N2} sobre ${montoInicial:N2})");
            pdf.Campo("Actualización", $"Trimestral por {contrato.IndiceActualizacion} (BCRA), cada {contrato.PeriodicidadActualizacionMeses} meses");
            pdf.Campo("Mora", "Intereses según la TIM, art. 768 inc. c CCyC (BCRA)");

            pdf.Subtitulo("4. Duración");
            pdf.Campo("Fecha de inicio", contrato.FechaInicio.ToString("dd/MM/yyyy"));
            pdf.Campo("Fecha de finalización", contrato.FechaFin.ToString("dd/MM/yyyy"));
            pdf.Campo("Duración", $"{contrato.DuracionMeses} meses");
            pdf.Campo("Método de pago", contrato.MetodoPago.ToString());

            pdf.Subtitulo("5. Cláusulas");
            pdf.Parrafo($"Rescisión anticipada: la parte que rescinda el contrato antes de su vencimiento abonará una penalidad del " +
                        $"{PORCENTAJE_PENALIDAD_RESCISION:P0} del canon restante, conforme al art. 1221 del Código Civil y Comercial de la Nación.");
            pdf.Espacio(3);
            pdf.Parrafo("Reparaciones: las reparaciones estructurales del inmueble están a cargo del locador; las originadas en el " +
                        "desgaste por el uso normal y las mejoras menores, a cargo del locatario.");
            pdf.Espacio(3);
            pdf.Parrafo($"Actualización: el alquiler se actualiza automáticamente cada {contrato.PeriodicidadActualizacionMeses} meses según la " +
                        $"variación acumulada del {contrato.IndiceActualizacion} publicada por el BCRA.");
            pdf.Espacio(3);
            pdf.Parrafo($"Mora: el pago posterior al día {PagoAlquilerBLL.DIA_VENCIMIENTO} de cada mes devenga intereses moratorios a la Tasa " +
                        "de Intereses Moratorios (TIM) que publica el BCRA (art. 768 inc. c CCyC), desde el vencimiento hasta el día del pago.");

            pdf.Subtitulo("6. Firmas", reservar: 130); // el título va en la misma página que las firmas
            pdf.Espacio(6);
            pdf.Firmas(
                ("El LOCADOR:", null, nombrePropietario),
                ("El LOCATARIO:", firmaInquilino.Trim(), $"{nombreInquilino} - DNI {inquilino.Dni}"),
                ("Por LandSmart:", null, "Firma y aclaración"));

            pdf.Guardar(ruta);
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "PDF Contrato",
                $"Contrato N° {idContrato}: PDF generado y firmado por el inquilino.");
        }

        // Logo de LandSmart incrustado en BLL.dll (GUI\Resources\logo-landsmart.png); null si no estuviera
        private static byte[] Logo()
        {
            using (System.IO.Stream recurso = typeof(ContratoBLL).Assembly.GetManifestResourceStream("BLL.logo-landsmart.png"))
            {
                if (recurso == null) return null;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    recurso.CopyTo(ms);
                    return ms.ToArray();
                }
            }
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
            if (contrato.ComisionInmobiliaria < 0 || contrato.ComisionInmobiliaria > 100)
                throw new ArgumentException("La comisión inmobiliaria es un porcentaje entre 0 y 100.");
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

        // Garantes del contrato que se registra: al menos uno, con datos completos, sin repetir DNI y distintos
        // del inquilino
        private void ValidarGarantes(Contrato contrato, string dniInquilino)
        {
            if (contrato.Garantes == null || contrato.Garantes.Count == 0)
                throw new ArgumentException("El contrato debe tener al menos un garante.");
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
