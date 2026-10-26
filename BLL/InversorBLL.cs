using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Inversores de los fideicomisos (los gestiona el sector Inversiones)
    public class InversorBLL
    {
        private readonly InversorDAL inversorDAL = new InversorDAL();

        public int Registrar(Inversor inversor)
        {
            ProyectoInversionBLL.ExigirPermiso(PermisosDefinidos.AccesoPN3Inversiones, "Inversiones");
            Validar(inversor);
            if (inversorDAL.BuscarPorCuit(inversor.Cuit) != null)
                throw new ArgumentException($"Ya existe un inversor con CUIT {inversor.Cuit}.");

            int id = inversorDAL.Crear(inversor);
            if (id == -1)
                throw new Exception("No se pudo registrar el inversor.");
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Inversor", $"Inversor CUIT {inversor.Cuit} registrado.");
            return id;
        }

        public void Actualizar(Inversor inversor)
        {
            ProyectoInversionBLL.ExigirPermiso(PermisosDefinidos.AccesoPN3Inversiones, "Inversiones");
            Validar(inversor);
            Inversor otro = inversorDAL.BuscarPorCuit(inversor.Cuit);
            if (otro != null && otro.IdInversor != inversor.IdInversor)
                throw new ArgumentException($"Ya existe otro inversor con CUIT {inversor.Cuit}.");
            if (!inversorDAL.Actualizar(inversor))
                throw new Exception("No se pudo actualizar el inversor.");
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Inversor", $"Inversor CUIT {inversor.Cuit} actualizado.");
        }

        // Solo sin aportes en ningún fideicomiso
        public void Eliminar(int idInversor)
        {
            ProyectoInversionBLL.ExigirPermiso(PermisosDefinidos.AccesoPN3Inversiones, "Inversiones");
            Inversor inversor = inversorDAL.BuscarPorId(idInversor);
            if (inversor == null)
                throw new Exception("El inversor no existe.");
            if (!inversorDAL.Eliminar(idInversor))
                throw new ArgumentException("No se puede eliminar el inversor porque tiene aportes en un fideicomiso.");
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Inversor", $"Inversor CUIT {inversor.Cuit} eliminado.");
        }

        public List<Inversor> LeerTodos() => inversorDAL.LeerTodos();

        public Inversor BuscarPorId(int idInversor) => inversorDAL.BuscarPorId(idInversor);

        public Inversor BuscarPorCuit(string cuit) => inversorDAL.BuscarPorCuit(FormatearCuit(cuit) ?? cuit?.Trim());

        // ---------- CUIT ----------

        // 11 dígitos con el dígito verificador correcto (módulo 11). Acepta con o sin guiones.
        public static bool CuitValido(string cuit)
        {
            string digitos = new string((cuit ?? "").Where(char.IsDigit).ToArray());
            if (digitos.Length != 11 || (cuit ?? "").Any(c => !char.IsDigit(c) && c != '-' && c != ' ')) return false;
            int[] pesos = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = pesos.Select((p, i) => p * (digitos[i] - '0')).Sum();
            int verificador = 11 - suma % 11;
            if (verificador == 11) verificador = 0;
            if (verificador == 10) return false;
            return verificador == digitos[10] - '0';
        }

        // XX-XXXXXXXX-X (null si no tiene 11 dígitos)
        public static string FormatearCuit(string cuit)
        {
            string d = new string((cuit ?? "").Where(char.IsDigit).ToArray());
            return d.Length == 11 ? $"{d.Substring(0, 2)}-{d.Substring(2, 8)}-{d.Substring(10)}" : null;
        }

        internal void Validar(Inversor inversor)
        {
            if (inversor == null) throw new ArgumentNullException(nameof(inversor));
            if (string.IsNullOrWhiteSpace(inversor.Nombre) || string.IsNullOrWhiteSpace(inversor.Apellido))
                throw new ArgumentException("El nombre y el apellido del inversor son obligatorios.");
            if (!CuitValido(inversor.Cuit))
                throw new ArgumentException("El CUIT no es válido (11 dígitos con su dígito verificador).");
            if (!string.IsNullOrWhiteSpace(inversor.Email) && !inversor.Email.Contains("@"))
                throw new ArgumentException("El email no tiene un formato válido.");

            inversor.Nombre = inversor.Nombre.Trim();
            inversor.Apellido = inversor.Apellido.Trim();
            inversor.Cuit = FormatearCuit(inversor.Cuit);
            inversor.Email = string.IsNullOrWhiteSpace(inversor.Email) ? null : inversor.Email.Trim();
            inversor.Telefono = string.IsNullOrWhiteSpace(inversor.Telefono) ? null : inversor.Telefono.Trim();
            if (inversor.Nombre.Length > 100 || inversor.Apellido.Length > 100)
                throw new ArgumentException("El nombre y el apellido no pueden superar los 100 caracteres.");
        }
    }
}
