using System;
using System.Collections.Generic;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class MaterialAudiovisualBLL
    {
        private MaterialAudiovisualDAL materialDAL = new MaterialAudiovisualDAL();
        private InmuebleDAL inmuebleDAL = new InmuebleDAL();

        public int Registrar(MaterialAudiovisual material)
        {
            // Si no se indica usuario de marketing, lo carga el usuario logueado
            if (material != null && material.IdUsuarioMarketing == 0 && SessionManager.Instance.EstaLogueado())
                material.IdUsuarioMarketing = SessionManager.Instance.UsuarioActual.Id;

            Validar(material);

            int result = materialDAL.Crear(material);
            if (result == -1)
                throw new Exception("No se pudo registrar el material audiovisual.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Material", $"Material {result} ({material.TipoArchivo}) cargado para el inmueble {material.IdInmueble}.");
            return result;
        }

        public void Actualizar(MaterialAudiovisual material)
        {
            Validar(material);

            if (!materialDAL.Actualizar(material))
                throw new Exception("No se pudo actualizar el material audiovisual.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Material", $"Material {material.IdMaterial} actualizado.");
        }

        public void Eliminar(int idMaterial)
        {
            if (!materialDAL.Eliminar(idMaterial))
                throw new Exception("No se pudo eliminar el material audiovisual.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Material", $"Material {idMaterial} eliminado.");
        }

        public List<MaterialAudiovisual> LeerTodos() => materialDAL.LeerTodos();

        public List<MaterialAudiovisual> LeerPorInmueble(int idInmueble) => materialDAL.LeerPorInmueble(idInmueble);

        public MaterialAudiovisual BuscarPorId(int idMaterial) => materialDAL.BuscarPorId(idMaterial);

        private void Validar(MaterialAudiovisual material)
        {
            if (material == null) throw new ArgumentNullException(nameof(material));
            if (string.IsNullOrWhiteSpace(material.TipoArchivo))
                throw new ArgumentException("El tipo de archivo es obligatorio.");
            if (string.IsNullOrWhiteSpace(material.RutaArchivo))
                throw new ArgumentException("La ruta del archivo es obligatoria.");
            if (material.IdUsuarioMarketing <= 0)
                throw new ArgumentException("El material debe tener un usuario de marketing asignado.");

            Inmueble inmueble = inmuebleDAL.BuscarPorId(material.IdInmueble);
            if (inmueble == null)
                throw new ArgumentException("El inmueble indicado no existe.");
            if (inmueble.EstadoActual == EstadoInmueble.Rechazado)
                throw new ArgumentException("No se puede cargar material para un inmueble con tasación rechazada.");

            material.TipoArchivo = material.TipoArchivo.Trim();
            material.RutaArchivo = material.RutaArchivo.Trim();
            material.TextoComercial = string.IsNullOrWhiteSpace(material.TextoComercial) ? null : material.TextoComercial.Trim();
        }
    }
}
