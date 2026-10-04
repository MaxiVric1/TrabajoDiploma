using System;
using System.Collections.Generic;
using BE;
using DAL;
using SERVICIOS;
using System.IO;
using System.Linq;

namespace BLL
{
    public class UsuarioBLL
    {
        private MapperUsuario mapperUsuario = new MapperUsuario();
        private MapperSeguridad mapperSeguridad = new MapperSeguridad();
        private MapperSesion mapperSesion = new MapperSesion();
        private MapperUsuarioHistorial mapperHistorial = new MapperUsuarioHistorial();
        private MapperDVV mapperDVV = new MapperDVV();
        private const string SESSION_FILE = "session.dat";

        public bool Login(string nombre, string pass)
        {
            // La verificación de integridad se hace en el arranque (Program.cs)
            BE.Usuario user = mapperUsuario.BuscarPorNombre(nombre);
            // ... resto del login
            if (user != null)
            {
                if (user.BloqueadoHasta.HasValue && user.BloqueadoHasta.Value > DateTime.Now)
                    throw new Exception("Usuario bloqueado temporalmente.");

                if (user.Password == Encriptador.Hash(pass))
                {
                    mapperUsuario.ActualizarIntentos(nombre, true);
                    CargarDatosUsuario(user);

                    int sessionId = mapperSesion.AbrirSesion(user.Id);
                    if (sessionId != -1)
                    {
                        SessionManager.Instance.IniciarSesion(user, sessionId);
                        
                        IdiomaBLL idiomaBLL = new IdiomaBLL();
                        Idioma idioma = null;
                        if (user.IdIdioma.HasValue)
                        {
                            idioma = idiomaBLL.LeerIdiomaUsuario(user.Id);
                        }
                        if (idioma == null || !idioma.Activo)
                        {
                            List<Idioma> activos = idiomaBLL.ObtenerIdiomasActivos();
                            if (activos.Count > 0)
                            {
                                idioma = activos[0];
                                idiomaBLL.GuardarIdiomaUsuario(user.Id, idioma.Id);
                                user.IdIdioma = idioma.Id;
                            }
                        }
                        if (idioma != null)
                        {
                            GestorIdioma.Instancia.CambiarIdioma(idioma);
                        }

                        GuardarSesionLocal(sessionId);
                        GestorBitacora.Instance.RegistrarEvento(user, "Login", "Inicio de sesión exitoso");
                        return true;
                    }
                }
                else
                {
                    mapperUsuario.ActualizarIntentos(nombre, false);
                }
            }
            
            GestorBitacora.Instance.RegistrarEvento(null, "Login Fallido", "Intento de acceso: " + nombre);
            return false;
        }

        private void CargarDatosUsuario(Usuario user)
        {
            user.Roles = mapperSeguridad.LeerRolesPorUsuario(user.Id);
            RolBLL rolBLL = new RolBLL();
            foreach (var rol in user.Roles)
            {
                rolBLL.CargarHijosRecursivo(rol);
            }
        }

        public void Logout()
        {
            if (SessionManager.Instance.EstaLogueado())
            {
                Usuario user = SessionManager.Instance.UsuarioActual;
                int sessionId = SessionManager.Instance.IdSesion.Value;
                
                mapperSesion.CerrarSesion(sessionId);
                BorrarSesionLocal();
                GestorBitacora.Instance.RegistrarEvento(user, "Logout", "Cierre de sesión");
                SessionManager.Instance.CerrarSesion();
            }
        }

        public int Registrar(BE.Usuario user, int idRol = 2)
        {
            if (string.IsNullOrEmpty(user.Nombre) || string.IsNullOrEmpty(user.Password))
                return -1;

            if (mapperUsuario.BuscarPorNombre(user.Nombre) != null)
            {
                Usuario editor = SessionManager.Instance.EstaLogueado() ? SessionManager.Instance.UsuarioActual : null;
                GestorBitacora.Instance.RegistrarEvento(editor, "Alta Fallida", "Usuario existente: " + user.Nombre);
                return -2;
            }

            user.Password = Encriptador.Hash(user.Password);
            
            // T07: Calcular DVH inicial
            user.DVH = GestorDV.CalcularDVH(user);

            int result = mapperUsuario.Crear(user);
            if (result != -1)
            {
                user.Id = result;
                // Re-calcular con ID real para el DVH final
                user.DVH = GestorDV.CalcularDVH(user);
                mapperUsuario.Actualizar(user);

                // T07: Recalcular DVV
                ActualizarDVV();

                // T06b: Guardar en historial la CREACIÓN
                Usuario editor = SessionManager.Instance.EstaLogueado() ? SessionManager.Instance.UsuarioActual : null;
                mapperHistorial.Insertar(user, editor?.Id ?? 0, "INSERT");

                mapperSeguridad.AsignarRol(result, idRol);
                GestorBitacora.Instance.RegistrarEvento(editor, "Alta", "Usuario creado: " + user.Nombre);
            }
            return result;
        }

        public void Actualizar(Usuario user)
        {
            Usuario autor = SessionManager.Instance.UsuarioActual;

            // T07: Recalcular DVH
            user.DVH = GestorDV.CalcularDVH(user);
            mapperUsuario.Actualizar(user);

            // T07: Recalcular DVV
            ActualizarDVV();

            // T06b: Guardar snapshot en historial DESPUÉS del cambio
            mapperHistorial.Insertar(user, autor?.Id ?? 0, "UPDATE");

            GestorBitacora.Instance.RegistrarEvento(autor, "Modificación", $"Usuario {user.Nombre} actualizado.");
        }

        private void ActualizarDVV()
        {
            var todos = mapperUsuario.LeerTodos();
            long dvv = GestorDV.CalcularDVV(todos.Select(x => x.DVH));
            mapperDVV.ActualizarDVV("Usuario", dvv);
        }

        public List<UsuarioHistorial> LeerHistorial(int idUsuario)
        {
            var historial = mapperHistorial.LeerHistorial(idUsuario);
            var usuarios = LeerTodos();
            foreach (var h in historial)
            {
                var autor = usuarios.FirstOrDefault(u => u.Id == h.IdUsuarioAutor);
                h.EditorNombre = autor != null ? autor.Nombre : "Desconocido";
            }
            return historial;
        }

        public void Restaurar(int idHistorial)
        {
            UsuarioHistorial backup = mapperHistorial.BuscarPorId(idHistorial);
            if (backup != null)
            {
                Usuario actual = mapperUsuario.BuscarPorId(backup.IdUsuario);
                if (actual != null &&
                    actual.NombrePersona == backup.NombrePersona &&
                    actual.Apellido == backup.Apellido &&
                    actual.Activo == backup.Activo)
                {
                    throw new Exception("El usuario ya se encuentra en el estado seleccionado.");
                }

                Usuario u = new Usuario
                {
                    Id = actual.Id,
                    Nombre = actual.Nombre, // Mantener login actual
                    Password = actual.Password, // Mantener password actual
                    NombrePersona = backup.NombrePersona,
                    Apellido = backup.Apellido,
                    Activo = backup.Activo,
                    IdIdioma = actual.IdIdioma,
                    IntentosFallidos = actual.IntentosFallidos,
                    BloqueadoHasta = actual.BloqueadoHasta
                };
                Actualizar(u);
            }
        }

        public void ActualizarUsuarioYRol(Usuario user, int idRol)
        {
            Usuario actual = mapperUsuario.BuscarPorId(user.Id);
            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = actual.Password;
            }
            else
            {
                user.Password = Encriptador.Hash(user.Password);
            }

            // Update user details
            Actualizar(user);

            // Update role
            mapperSeguridad.LimpiarRolesUsuario(user.Id);
            mapperSeguridad.AsignarRol(user.Id, idRol);
        }

        public void DesbloquearUsuario(string nombre)
        {
            mapperUsuario.ActualizarIntentos(nombre, true);
            Usuario autor = SessionManager.Instance.UsuarioActual;
            GestorBitacora.Instance.RegistrarEvento(autor, "Desbloqueo", $"Usuario {nombre} desbloqueado manualmente.");
        }

        public void BloquearUsuario(string nombre, int minutos)
        {
            mapperUsuario.BloquearManual(nombre, minutos);
            Usuario autor = SessionManager.Instance.UsuarioActual;
            GestorBitacora.Instance.RegistrarEvento(autor, "Bloqueo", $"Usuario {nombre} bloqueado manualmente por {minutos} minutos.");
        }

        public List<Rol> ObtenerRoles() => new RolBLL().ObtenerTodos();

        public List<Usuario> LeerTodos() => mapperUsuario.LeerTodos();

        public void CargarRoles(Usuario user)
        {
            CargarDatosUsuario(user);
        }

        public void GuardarSesionLocal(int id) => File.WriteAllText(SESSION_FILE, id.ToString());

        public void BorrarSesionLocal() { if (File.Exists(SESSION_FILE)) File.Delete(SESSION_FILE); }

        public bool ValidarSesionLocal()
        {
            if (File.Exists(SESSION_FILE))
            {
                if (int.TryParse(File.ReadAllText(SESSION_FILE), out int id))
                {
                    int idUsuario = mapperSesion.ValidarYRefrescarSesion(id);
                    if (idUsuario != -1)
                    {
                        Usuario user = mapperUsuario.BuscarPorId(idUsuario);
                        if (user != null)
                        {
                            CargarDatosUsuario(user);
                            SessionManager.Instance.IniciarSesion(user, id);

                             IdiomaBLL idiomaBLL = new IdiomaBLL();
                             Idioma idioma = null;
                             if (user.IdIdioma.HasValue)
                             {
                                 idioma = idiomaBLL.LeerIdiomaUsuario(user.Id);
                             }
                             if (idioma == null || !idioma.Activo)
                             {
                                 List<Idioma> activos = idiomaBLL.ObtenerIdiomasActivos();
                                 if (activos.Count > 0)
                                 {
                                     idioma = activos[0];
                                     idiomaBLL.GuardarIdiomaUsuario(user.Id, idioma.Id);
                                     user.IdIdioma = idioma.Id;
                                 }
                             }
                             if (idioma != null)
                             {
                                 GestorIdioma.Instancia.CambiarIdioma(idioma);
                             }

                            return true;
                        }
                    }
                }
                BorrarSesionLocal();
            }
            return false;
        }
    }
}
