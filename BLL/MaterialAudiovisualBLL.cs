using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    public class MaterialAudiovisualBLL
    {
        // Formatos admitidos por tipo. Fotos: solo los que Windows Forms puede mostrar en la vista previa.
        // Recorrido360 admite cualquier formato.
        public static readonly string[] EXTENSIONES_FOTO = { ".jpg", ".jpeg", ".png", ".gif" };
        public static readonly string[] EXTENSIONES_VIDEO = { ".mp4", ".avi", ".mov" };

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

        // Carga de varios archivos para un inmueble en una sola transacción; el texto comercial se guarda en cada uno
        // Guarda el texto comercial del inmueble y los archivos nuevos en una sola transacción.
        // archivos puede estar vacío para solo modificar el texto.
        public void RegistrarLote(int idInmueble, string textoComercial, List<MaterialAudiovisual> archivos)
        {
            archivos = archivos ?? new List<MaterialAudiovisual>();
            if (!SessionManager.Instance.EstaLogueado())
                throw new ArgumentException("El material debe tener un usuario de marketing asignado.");

            Inmueble inmueble = ValidarInmueble(idInmueble);

            string texto = string.IsNullOrWhiteSpace(textoComercial) ? null : textoComercial.Trim();
            if (texto != null && texto.Length > 2000)
                throw new ArgumentException("El texto comercial no puede superar los 2000 caracteres.");
            bool textoModificado = (texto ?? "") != (inmueble.TextoComercial ?? "");
            if (archivos.Count == 0 && !textoModificado)
                throw new ArgumentException("No hay cambios para guardar: agregue archivos o modifique el texto comercial.");

            foreach (MaterialAudiovisual material in archivos)
            {
                material.IdInmueble = idInmueble;
                material.IdUsuarioMarketing = SessionManager.Instance.UsuarioActual.Id;
                ValidarDatos(material);
            }

            if (!materialDAL.CrearLote(idInmueble, texto, archivos))
                throw new Exception("No se pudo guardar el material audiovisual. No se guardó ningún cambio.");

            string detalle = archivos.Count > 0 ? $"{archivos.Count} archivo(s) cargado(s)" : "sin archivos nuevos";
            if (textoModificado) detalle += ", texto comercial actualizado";
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Material",
                $"Inmueble {idInmueble}: {detalle}.");
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
            ValidarInmueble(material.IdInmueble);
            ValidarDatos(material);
        }

        // El material se carga después de la tasación aprobada (y se puede seguir agregando una vez publicado)
        private Inmueble ValidarInmueble(int idInmueble)
        {
            Inmueble inmueble = inmuebleDAL.BuscarPorId(idInmueble);
            if (inmueble == null)
                throw new ArgumentException("El inmueble indicado no existe.");
            if (inmueble.EstadoActual != EstadoInmueble.Tasado && inmueble.EstadoActual != EstadoInmueble.Publicado)
                throw new ArgumentException($"Solo se puede cargar material para inmuebles con tasación aprobada. Estado actual: {inmueble.EstadoActual}.");
            return inmueble;
        }

        private void ValidarDatos(MaterialAudiovisual material)
        {
            if (string.IsNullOrWhiteSpace(material.TipoArchivo) || !Enum.IsDefined(typeof(TipoMaterial), material.TipoArchivo.Trim()))
                throw new ArgumentException("El tipo de archivo debe ser Foto, Video o Recorrido360.");
            if (string.IsNullOrWhiteSpace(material.RutaArchivo))
                throw new ArgumentException("La ruta del archivo es obligatoria.");
            if (material.IdUsuarioMarketing <= 0)
                throw new ArgumentException("El material debe tener un usuario de marketing asignado.");

            string tipo = material.TipoArchivo.Trim();
            string archivo = Path.GetFileName(material.RutaArchivo.Trim());
            string extension = Path.GetExtension(archivo).ToLowerInvariant();
            if (tipo == TipoMaterial.Foto.ToString() && !EXTENSIONES_FOTO.Contains(extension))
                throw new ArgumentException($"El archivo \"{archivo}\" no es una foto admitida. Formatos válidos: {string.Join(", ", EXTENSIONES_FOTO)}.");
            if (tipo == TipoMaterial.Video.ToString() && !EXTENSIONES_VIDEO.Contains(extension))
                throw new ArgumentException($"El archivo \"{archivo}\" no es un video admitido. Formatos válidos: {string.Join(", ", EXTENSIONES_VIDEO)}.");

            material.TipoArchivo = tipo;
            material.RutaArchivo = material.RutaArchivo.Trim();
        }
    }
}
