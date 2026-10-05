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

        // inquilinoNuevo: para un inquilino que todavía no existe; se da de alta junto con el contrato en una sola
        // transacción. Si ya existe uno con ese DNI se rechaza (hay que usar el existente).
        public int Registrar(Contrato contrato, Inquilino inquilinoNuevo = null)
        {
            if (contrato != null && contrato.IdUsuarioAgente == 0 && SessionManager.Instance.EstaLogueado())
                contrato.IdUsuarioAgente = SessionManager.Instance.UsuarioActual.Id;

            if (inquilinoNuevo != null)
            {
                new InquilinoBLL().Validar(inquilinoNuevo);
                if (inquilinoDAL.BuscarPorDni(inquilinoNuevo.Dni) != null)
                    throw new ArgumentException($"Ya existe un inquilino con DNI {inquilinoNuevo.Dni}: búsquelo para usar sus datos.");
            }

            Validar(contrato, validarInquilino: inquilinoNuevo == null);
            if (!inmuebleDAL.LeerDisponibles().Any(i => i.IdInmuebleAlquiler == contrato.IdInmuebleAlquiler))
                throw new ArgumentException("El inmueble ya tiene un contrato vigente.");

            EstadoContrato estadoInicial = EstadoSegunVencimiento(contrato.FechaFin, DateTime.Now);
            if (estadoInicial == EstadoContrato.Finalizado)
                throw new ArgumentException($"Con esa fecha de inicio y duración el contrato ya estaría vencido ({contrato.FechaFin:dd/MM/yyyy}).");

            // Un contrato corto puede nacer ya dentro del período de alerta: se guarda con ese estado en la misma transacción
            contrato.EstadoContrato = estadoInicial;
            contrato.AlertaVencimiento = AlertaSegunEstado(estadoInicial);
            int result = contratoDAL.CrearConEstado(contrato, inquilinoNuevo);
            if (result == -1)
                throw new Exception("No se pudo registrar el contrato. No se guardó ningún cambio.");

            string inquilino = inquilinoNuevo != null ? $"inquilino nuevo DNI {inquilinoNuevo.Dni}" : $"inquilino {contrato.IdInquilino}";
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Contrato",
                $"Contrato N° {result}: inmueble {contrato.IdInmuebleAlquiler}, {inquilino}, " +
                $"{contrato.DuracionMeses} meses desde {contrato.FechaInicio:dd/MM/yyyy}, ${contrato.MontoMensual:N2} mensuales.");
            return result;
        }

        // Modifica condiciones del contrato; el estado y la alerta se recalculan según la nueva fecha de fin
        public void Actualizar(Contrato contrato)
        {
            Validar(contrato);
            Contrato actual = contratoDAL.BuscarPorId(contrato.IdContrato);
            if (actual == null)
                throw new Exception("El contrato no existe.");
            if (actual.EstadoContrato == EstadoContrato.Finalizado)
                throw new Exception("No se puede modificar un contrato finalizado.");
            if (contrato.IdInmuebleAlquiler != actual.IdInmuebleAlquiler &&
                !inmuebleDAL.LeerDisponibles().Any(i => i.IdInmuebleAlquiler == contrato.IdInmuebleAlquiler))
                throw new ArgumentException("El inmueble ya tiene un contrato vigente.");

            contrato.EstadoContrato = EstadoSegunVencimiento(contrato.FechaFin, DateTime.Now);
            contrato.AlertaVencimiento = AlertaSegunEstado(contrato.EstadoContrato);

            if (!contratoDAL.Actualizar(contrato))
                throw new Exception("No se pudo actualizar el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Contrato",
                $"Contrato N° {contrato.IdContrato} actualizado (estado {contrato.EstadoContrato}).");
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

        // Finalización anticipada (rescisión, desalojo): libera el inmueble para un nuevo contrato
        public void Finalizar(int idContrato)
        {
            Contrato contrato = contratoDAL.BuscarPorId(idContrato);
            if (contrato == null)
                throw new Exception("El contrato no existe.");
            if (contrato.EstadoContrato == EstadoContrato.Finalizado)
                throw new Exception("El contrato ya está finalizado.");

            if (!contratoDAL.ActualizarEstado(idContrato, EstadoContrato.Finalizado, false))
                throw new Exception("No se pudo finalizar el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Finalización Contrato",
                $"Contrato N° {idContrato} finalizado antes de su vencimiento ({contrato.FechaFin:dd/MM/yyyy}).");
        }

        public void Eliminar(int idContrato)
        {
            if (pagoDAL.LeerPorContrato(idContrato).Count > 0)
                throw new Exception("No se puede eliminar el contrato porque tiene pagos registrados.");
            if (accionDAL.LeerPorContrato(idContrato).Count > 0)
                throw new Exception("No se puede eliminar el contrato porque tiene acciones legales registradas.");

            if (!contratoDAL.Eliminar(idContrato))
                throw new Exception("No se pudo eliminar el contrato.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Contrato", $"Contrato N° {idContrato} eliminado.");
        }

        public List<Contrato> LeerTodos() => contratoDAL.LeerTodos();

        public Contrato BuscarPorId(int idContrato) => contratoDAL.BuscarPorId(idContrato);

        // Con inquilino e inmueble; estado null = todos
        public List<Contrato> LeerDetalle(EstadoContrato? estado = null) => contratoDAL.LeerDetalle(estado);

        // Vigentes que vencen dentro de DIAS_ALERTA_VENCIMIENTO días, con el contacto del inquilino
        public List<Contrato> LeerPorVencer() => contratoDAL.LeerPorVencer(DIAS_ALERTA_VENCIMIENTO);

        // validarInquilino = false cuando el inquilino se crea en la misma transacción y todavía no existe
        private void Validar(Contrato contrato, bool validarInquilino = true)
        {
            if (contrato == null) throw new ArgumentNullException(nameof(contrato));
            if (contrato.FechaInicio == default(DateTime))
                throw new ArgumentException("La fecha de inicio es obligatoria.");
            if (contrato.DuracionMeses <= 0)
                throw new ArgumentException("La duración debe ser de al menos un mes.");
            if (contrato.MontoMensual <= 0)
                throw new ArgumentException("El monto mensual debe ser mayor a cero.");
            if (contrato.IdUsuarioAgente <= 0)
                throw new ArgumentException("El contrato debe tener un agente asignado.");
            if (validarInquilino && inquilinoDAL.BuscarPorId(contrato.IdInquilino) == null)
                throw new ArgumentException("El inquilino indicado no existe.");
            if (inmuebleDAL.BuscarPorId(contrato.IdInmuebleAlquiler) == null)
                throw new ArgumentException("El inmueble indicado no existe.");

            contrato.FechaInicio = contrato.FechaInicio.Date;
        }
    }
}
