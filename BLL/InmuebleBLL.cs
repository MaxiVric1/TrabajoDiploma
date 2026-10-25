using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class InmuebleBLL
    {
        private InmuebleDAL inmuebleDAL = new InmuebleDAL();
        private ClienteDAL clienteDAL = new ClienteDAL();
        private CaptacionDAL captacionDAL = new CaptacionDAL();
        private MaterialAudiovisualDAL materialDAL = new MaterialAudiovisualDAL();

        public int Registrar(Inmueble inmueble)
        {
            Validar(inmueble);

            int result = inmuebleDAL.Crear(inmueble);
            if (result == -1)
                throw new Exception("No se pudo registrar el inmueble.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Inmueble", $"Inmueble {result} registrado: {inmueble.Direccion}.");
            return result;
        }

        public void Actualizar(Inmueble inmueble)
        {
            Validar(inmueble);

            if (!inmuebleDAL.Actualizar(inmueble))
                throw new Exception("No se pudo actualizar el inmueble.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Inmueble", $"Inmueble {inmueble.IdInmueble} actualizado.");
        }

        public void CambiarEstado(int idInmueble, EstadoInmueble estado)
        {
            if (!inmuebleDAL.ActualizarEstado(idInmueble, estado))
                throw new Exception("No se pudo actualizar el estado del inmueble.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Cambio Estado Inmueble", $"Inmueble {idInmueble} pasó a estado {estado}.");
        }

        // Publicar requiere una tasación aprobada y al menos un material audiovisual cargado
        public void Publicar(int idInmueble)
        {
            Inmueble inmueble = inmuebleDAL.BuscarPorId(idInmueble);
            if (inmueble == null)
                throw new Exception("El inmueble no existe.");
            if (inmueble.EstadoActual != EstadoInmueble.Tasado)
                throw new Exception($"Solo se puede publicar un inmueble tasado. Estado actual: {inmueble.EstadoActual}.");
            if (materialDAL.LeerPorInmueble(idInmueble).Count == 0)
                throw new Exception("El inmueble no tiene material audiovisual cargado.");

            CambiarPublicacion(inmueble, true, "Publicación Inmueble", $"Inmueble {idInmueble} publicado: {inmueble.Direccion}.",
                "No se pudo publicar el inmueble. No se guardó ningún cambio.");
        }

        // Retira la publicación: Publicado -> Tasado (conserva la fecha de publicación para poder republicar)
        public void Despublicar(int idInmueble)
        {
            Inmueble inmueble = inmuebleDAL.BuscarPorId(idInmueble);
            if (inmueble == null)
                throw new Exception("El inmueble no existe.");
            if (inmueble.EstadoActual != EstadoInmueble.Publicado)
                throw new Exception($"Solo se puede despublicar un inmueble publicado. Estado actual: {inmueble.EstadoActual}.");

            CambiarPublicacion(inmueble, false, "Despublicación Inmueble", $"Inmueble {idInmueble} despublicado: {inmueble.Direccion}.",
                "No se pudo despublicar el inmueble. No se guardó ningún cambio.");
        }

        // Vuelve a publicar un inmueble despublicado: Tasado -> Publicado
        public void Republicar(int idInmueble)
        {
            Inmueble inmueble = inmuebleDAL.BuscarPorId(idInmueble);
            if (inmueble == null)
                throw new Exception("El inmueble no existe.");
            if (!inmueble.Despublicado)
                throw new Exception($"Solo se puede republicar un inmueble que fue despublicado. Estado actual: {inmueble.EstadoActual}.");
            if (materialDAL.LeerPorInmueble(idInmueble).Count == 0)
                throw new Exception("El inmueble no tiene material audiovisual cargado.");

            CambiarPublicacion(inmueble, true, "Republicación Inmueble", $"Inmueble {idInmueble} republicado: {inmueble.Direccion}.",
                "No se pudo republicar el inmueble. No se guardó ningún cambio.");
        }

        // El cambio de estado y su registro en bitácora se guardan en la misma transacción
        private void CambiarPublicacion(Inmueble inmueble, bool publicar, string actividad, string info, string mensajeError)
        {
            Bitacora evento = new Bitacora
            {
                FechaHora = DateTime.Now,
                Usuario = SessionManager.Instance.UsuarioActual,
                Actividad = actividad,
                InfoAsociada = info
            };

            if (!inmuebleDAL.CambiarPublicacion(inmueble.IdInmueble, publicar, evento))
                throw new Exception(mensajeError);
        }

        public void Eliminar(int idInmueble)
        {
            if (captacionDAL.LeerTodos().Any(c => c.IdInmueble == idInmueble))
                throw new Exception("No se puede eliminar el inmueble porque tiene captaciones asociadas.");
            if (materialDAL.LeerPorInmueble(idInmueble).Count > 0)
                throw new Exception("No se puede eliminar el inmueble porque tiene material audiovisual asociado.");

            if (!inmuebleDAL.Eliminar(idInmueble))
                throw new Exception("No se pudo eliminar el inmueble.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Inmueble", $"Inmueble {idInmueble} eliminado.");
        }

        public List<Inmueble> LeerTodos() => inmuebleDAL.LeerTodos();

        public List<Inmueble> LeerPorCliente(int idCliente) => inmuebleDAL.LeerPorCliente(idCliente);

        public List<Inmueble> LeerPorEstado(EstadoInmueble estado) => inmuebleDAL.LeerTodos().Where(i => i.EstadoActual == estado).ToList();

        public Inmueble BuscarPorId(int idInmueble) => inmuebleDAL.BuscarPorId(idInmueble);

        // validarCliente = false cuando el cliente se crea en la misma transacción y todavía no existe en la base
        // exigirUbicacion: en una captación nueva la localidad es obligatoria (para ubicarlo en el mapa) y en una casa
        // también la superficie del lote (para evaluar un desarrollo); los inmuebles anteriores no las tienen
        internal void Validar(Inmueble inmueble, bool validarCliente = true, bool exigirUbicacion = false)
        {
            if (inmueble == null) throw new ArgumentNullException(nameof(inmueble));
            if (string.IsNullOrWhiteSpace(inmueble.TipoInmueble))
                throw new ArgumentException("El tipo de inmueble es obligatorio.");
            if (string.IsNullOrWhiteSpace(inmueble.Direccion))
                throw new ArgumentException("La dirección es obligatoria.");
            if (inmueble.MetrosCuadrados <= 0)
                throw new ArgumentException("Los metros cuadrados deben ser mayores a cero.");
            if (inmueble.CantidadAmbientes <= 0)
                throw new ArgumentException("La cantidad de ambientes debe ser mayor a cero.");
            if (validarCliente && clienteDAL.BuscarPorId(inmueble.IdCliente) == null)
                throw new ArgumentException("El cliente indicado no existe.");

            inmueble.Localidad = string.IsNullOrWhiteSpace(inmueble.Localidad) ? null : inmueble.Localidad.Trim();
            if (exigirUbicacion && inmueble.Localidad == null)
                throw new ArgumentException("La localidad es obligatoria.");
            if (inmueble.Localidad != null && inmueble.Localidad.Length > 100)
                throw new ArgumentException("La localidad no puede superar los 100 caracteres.");
            if (inmueble.SuperficieLote.HasValue && inmueble.SuperficieLote <= 0)
                throw new ArgumentException("La superficie del lote debe ser mayor a cero.");
            if (exigirUbicacion && EsCasa(inmueble) && !inmueble.SuperficieLote.HasValue)
                throw new ArgumentException("La superficie del lote es obligatoria para una casa.");

            inmueble.TipoInmueble = inmueble.TipoInmueble.Trim();
            inmueble.Direccion = inmueble.Direccion.Trim();
        }

        public static bool EsCasa(Inmueble inmueble) => string.Equals(inmueble?.TipoInmueble?.Trim(), "Casa", StringComparison.OrdinalIgnoreCase);
    }
}
