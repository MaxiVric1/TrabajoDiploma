using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class CaptacionBLL
    {
        private CaptacionDAL captacionDAL = new CaptacionDAL();
        private InmuebleDAL inmuebleDAL = new InmuebleDAL();
        private TasacionDAL tasacionDAL = new TasacionDAL();
        private ClienteDAL clienteDAL = new ClienteDAL();
        private MaterialAudiovisualDAL materialDAL = new MaterialAudiovisualDAL();

        public int Registrar(Captacion captacion)
        {
            // Si no se indica agente, la registra el usuario logueado
            if (captacion != null && captacion.IdUsuarioAgente == 0 && SessionManager.Instance.EstaLogueado())
                captacion.IdUsuarioAgente = SessionManager.Instance.UsuarioActual.Id;

            Validar(captacion);

            int result = captacionDAL.Crear(captacion);
            if (result == -1)
                throw new Exception("No se pudo registrar la captación.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Captación", $"Captación N° {result} registrada para el inmueble {captacion.IdInmueble}.");
            return result;
        }

        public const string MENSAJE_AUTORIZACION_REQUERIDA = "Debe confirmar que recibió la Autorización de Venta firmada.";
        public const string MENSAJE_AUTORIZACION_ALQUILER_REQUERIDA = "Debe confirmar que recibió la Autorización de Alquiler firmada.";

        // La autorización que firma el propietario depende de la operación
        public static string MensajeAutorizacionRequerida(TipoPublicacion tipo) =>
            tipo == TipoPublicacion.Alquiler ? MENSAJE_AUTORIZACION_ALQUILER_REQUERIDA : MENSAJE_AUTORIZACION_REQUERIDA;

        // Caso de uso "Registrar Captación": cliente + inmueble nuevo + captación en una sola transacción.
        // Si ya hay un cliente con ese DNI se usa el existente (sus datos no se modifican).
        // No se registra una captación sin la Autorización de Venta (o de Alquiler) firmada (el agente la confirma; el
        // documento no se guarda). Para un alquiler, el valor pretendido es el alquiler mensual.
        // Resultado de ubicar en el mapa el inmueble de la última captación registrada: null si se ubicó
        public string ErrorUbicacion { get; private set; }

        public int RegistrarCaptacion(Cliente cliente, Inmueble inmueble, Captacion captacion, bool autorizacionRecibida)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (inmueble == null) throw new ArgumentNullException(nameof(inmueble));
            if (!autorizacionRecibida)
                throw new ArgumentException(MensajeAutorizacionRequerida(inmueble.TipoPublicacion));
            if (captacion == null) throw new ArgumentNullException(nameof(captacion));

            Cliente existente = clienteDAL.BuscarPorDni(cliente.Dni?.Trim());
            if (existente != null)
            {
                cliente.IdCliente = existente.IdCliente;
            }
            else
            {
                cliente.IdCliente = 0;
                new ClienteBLL().Validar(cliente);
            }

            inmueble.IdInmueble = 0;
            inmueble.IdCliente = cliente.IdCliente;
            inmueble.EstadoActual = EstadoInmueble.Captado;
            new InmuebleBLL().Validar(inmueble, validarCliente: existente != null, exigirUbicacion: true);

            if (captacion.IdUsuarioAgente == 0 && SessionManager.Instance.EstaLogueado())
                captacion.IdUsuarioAgente = SessionManager.Instance.UsuarioActual.Id;
            Validar(captacion, validarInmueble: false);

            int result = captacionDAL.RegistrarCompleta(cliente, inmueble, captacion);
            if (result == -1)
                throw new Exception("No se pudo registrar la captación. No se guardó ningún cambio.");

            string detalleCliente = existente != null ? $"cliente existente DNI {cliente.Dni}" : $"cliente nuevo DNI {cliente.Dni}";
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Captación",
                $"Captación N° {result} registrada: inmueble {inmueble.IdInmueble} ({inmueble.Direccion}, {inmueble.Localidad}) para {inmueble.TipoPublicacion}, {detalleCliente}.");

            // Ubicación en el mapa (PN3): si no hay conexión o no se encuentra, la captación queda igual registrada
            GeocodificacionBLL geo = new GeocodificacionBLL();
            ErrorUbicacion = geo.Geocodificar(inmueble) ? null : geo.UltimoError;
            return result;
        }

        // Renegociación tras un rechazo: el cliente acepta un nuevo valor, la captación vuelve a Captado
        // para que el tasador la revise otra vez. nuevoValorPretendido null = se usa el valor sugerido por el tasador.
        public void RenegociarCaptacion(int nroCaptacion, decimal? nuevoValorPretendido = null)
        {
            Captacion captacion = captacionDAL.BuscarPorNro(nroCaptacion);
            if (captacion == null)
                throw new Exception("La captación no existe.");

            Inmueble inmueble = inmuebleDAL.BuscarPorId(captacion.IdInmueble);
            if (inmueble == null || inmueble.EstadoActual != EstadoInmueble.Rechazado)
                throw new Exception($"Solo se puede renegociar una captación con el inmueble rechazado. Estado actual: {inmueble?.EstadoActual}.");

            Tasacion dictamen = new TasacionBLL().ObtenerUltimoDictamen(nroCaptacion);
            if (dictamen == null || dictamen.EstadoDictamen != EstadoDictamen.Rechazada)
                throw new Exception("El último dictamen de la captación no es un rechazo.");

            decimal nuevoValor;
            if (nuevoValorPretendido.HasValue)
                nuevoValor = nuevoValorPretendido.Value;
            else if (dictamen.ValorSugerido.HasValue)
                nuevoValor = dictamen.ValorSugerido.Value;
            else
                throw new ArgumentException("El tasador no sugirió un valor: indique el nuevo valor pretendido.");

            if (nuevoValor <= 0)
                throw new ArgumentException("El nuevo valor pretendido debe ser mayor a cero.");
            if (nuevoValor == captacion.ValorPretendido)
                throw new ArgumentException("El nuevo valor pretendido es igual al actual.");

            decimal valorAnterior = captacion.ValorPretendido;
            captacion.ValorPretendido = nuevoValor;

            string sugerencia = dictamen.ValorSugerido.HasValue ? $"valor sugerido por el tasador: {dictamen.ValorSugerido.Value:N2}" : "sin valor sugerido";
            Bitacora evento = new Bitacora
            {
                FechaHora = DateTime.Now,
                Usuario = SessionManager.Instance.UsuarioActual,
                Actividad = "Renegociación Captación",
                InfoAsociada = $"Captación N° {nroCaptacion}: valor pretendido {valorAnterior:N2} -> {nuevoValor:N2} ({sugerencia}). Inmueble {inmueble.IdInmueble} vuelve a Captado."
            };

            if (!captacionDAL.Renegociar(captacion, evento))
            {
                captacion.ValorPretendido = valorAnterior;
                throw new Exception("No se pudo renegociar la captación. No se guardó ningún cambio.");
            }
        }

        public void Actualizar(Captacion captacion)
        {
            Validar(captacion);

            if (!captacionDAL.Actualizar(captacion))
                throw new Exception("No se pudo actualizar la captación.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Captación", $"Captación N° {captacion.NroCaptacion} actualizada.");
        }

        public void Eliminar(int nroCaptacion)
        {
            if (tasacionDAL.LeerPorCaptacion(nroCaptacion).Count > 0)
                throw new Exception("No se puede eliminar la captación porque tiene tasaciones asociadas.");

            if (!captacionDAL.Eliminar(nroCaptacion))
                throw new Exception("No se pudo eliminar la captación.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Captación", $"Captación N° {nroCaptacion} eliminada.");
        }

        public List<Captacion> LeerTodos() => captacionDAL.LeerTodos();

        // Listado para grillas: cada captación con su inmueble y cliente. estado null = todas.
        public List<CaptacionResumen> LeerResumen(EstadoInmueble? estado = null)
        {
            Dictionary<int, Inmueble> inmuebles = inmuebleDAL.LeerTodos().ToDictionary(i => i.IdInmueble);
            Dictionary<int, Cliente> clientes = clienteDAL.LeerTodos().ToDictionary(c => c.IdCliente);
            // Último dictamen de cada captación (mismo criterio que TasacionBLL.ObtenerUltimoDictamen)
            Dictionary<int, Tasacion> ultimosDictamenes = tasacionDAL.LeerTodos()
                .GroupBy(t => t.IdCaptacion)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(t => t.FechaDictamen).ThenByDescending(t => t.NroTasacion).First());
            Dictionary<int, int> archivosPorInmueble = materialDAL.LeerTodos()
                .GroupBy(m => m.IdInmueble)
                .ToDictionary(g => g.Key, g => g.Count());

            List<CaptacionResumen> lista = new List<CaptacionResumen>();
            foreach (Captacion c in captacionDAL.LeerTodos())
            {
                if (!inmuebles.TryGetValue(c.IdInmueble, out Inmueble inmueble)) continue;
                if (estado.HasValue && inmueble.EstadoActual != estado.Value) continue;
                clientes.TryGetValue(inmueble.IdCliente, out Cliente cliente);
                ultimosDictamenes.TryGetValue(c.NroCaptacion, out Tasacion dictamen);

                lista.Add(new CaptacionResumen
                {
                    NroCaptacion = c.NroCaptacion,
                    FechaRegistro = c.FechaRegistro,
                    Cliente = cliente != null ? $"{cliente.Nombre} {cliente.Apellido}" : "Desconocido",
                    Direccion = inmueble.Direccion,
                    TipoInmueble = inmueble.TipoInmueble,
                    TipoPublicacion = inmueble.TipoPublicacion,
                    MetrosCuadrados = inmueble.MetrosCuadrados,
                    CantidadAmbientes = inmueble.CantidadAmbientes,
                    ValorPretendido = c.ValorPretendido,
                    ValorFinal = dictamen != null && dictamen.EstadoDictamen == EstadoDictamen.Aprobada ? dictamen.ValorFinal : null,
                    CantidadArchivos = archivosPorInmueble.TryGetValue(inmueble.IdInmueble, out int archivos) ? archivos : 0,
                    EstadoPublicacion = EstadoDePublicacion(inmueble),
                    FechaPublicacion = inmueble.FechaPublicacion,
                    FechaDespublicacion = inmueble.FechaDespublicacion,
                    EstadoInmueble = inmueble.EstadoActual,
                    IdInmueble = inmueble.IdInmueble,
                    IdCliente = inmueble.IdCliente
                });
            }
            return lista;
        }

        public const string PUBLICACION_PUBLICADO = "Publicado";
        public const string PUBLICACION_DESPUBLICADO = "Despublicado";
        public const string PUBLICACION_ALQUILADO = "Alquilado";

        // Estado de la publicación para los listados ("" si nunca se publicó). Un inmueble alquilado se publicó antes
        // y vuelve a Publicado cuando termina el contrato.
        public static string EstadoDePublicacion(Inmueble inmueble)
        {
            if (inmueble.EstadoActual == EstadoInmueble.Publicado) return PUBLICACION_PUBLICADO;
            if (inmueble.EstadoActual == EstadoInmueble.Alquilado) return PUBLICACION_ALQUILADO;
            if (inmueble.Despublicado) return PUBLICACION_DESPUBLICADO;
            return "";
        }

        // Listos para la primera publicación: Tasado, con material y nunca publicados
        // (los despublicados se republican desde FormInmueblesPublicados)
        public List<CaptacionResumen> LeerListosParaPublicar()
        {
            return LeerResumen(EstadoInmueble.Tasado).Where(r => r.CantidadArchivos > 0 && !r.FechaPublicacion.HasValue).ToList();
        }

        // Publicados, alquilados y despublicados (Tasado que ya estuvo publicado)
        public List<CaptacionResumen> LeerPublicados()
        {
            return LeerResumen().Where(r => r.EstadoPublicacion != "").ToList();
        }

        public Captacion BuscarPorNro(int nroCaptacion) => captacionDAL.BuscarPorNro(nroCaptacion);

        // validarInmueble = false cuando el inmueble se crea en la misma transacción y todavía no existe en la base
        private void Validar(Captacion captacion, bool validarInmueble = true)
        {
            if (captacion == null) throw new ArgumentNullException(nameof(captacion));
            if (captacion.ValorPretendido <= 0)
                throw new ArgumentException("El valor pretendido debe ser mayor a cero.");
            if (captacion.IdUsuarioAgente <= 0)
                throw new ArgumentException("La captación debe tener un agente asignado.");
            if (validarInmueble && inmuebleDAL.BuscarPorId(captacion.IdInmueble) == null)
                throw new ArgumentException("El inmueble indicado no existe.");
        }
    }
}
