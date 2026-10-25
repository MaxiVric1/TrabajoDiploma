using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Garantes de un contrato. Al registrar el contrato se guardan junto con él (ContratoBLL.Registrar);
    // acá se agregan, modifican o quitan después, solo mientras el contrato está vigente.
    public class GaranteBLL
    {
        private GaranteDAL garanteDAL = new GaranteDAL();
        private ContratoDAL contratoDAL = new ContratoDAL();

        public int Agregar(Garante garante)
        {
            Validar(garante);
            Contrato contrato = ContratoVigente(garante.IdContrato);
            ValidarDniUnico(garante, contrato);

            int result = garanteDAL.Crear(garante);
            if (result == -1)
                throw new Exception("No se pudo registrar el garante.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Alta Garante",
                $"Contrato N° {garante.IdContrato}: garante DNI {garante.Dni} agregado.");
            return result;
        }

        public void Actualizar(Garante garante)
        {
            Validar(garante);
            Garante actual = garanteDAL.BuscarPorId(garante.IdGarante);
            if (actual == null)
                throw new Exception("El garante no existe.");
            garante.IdContrato = actual.IdContrato; // un garante no cambia de contrato
            Contrato contrato = ContratoVigente(garante.IdContrato);
            ValidarDniUnico(garante, contrato);

            if (!garanteDAL.Actualizar(garante))
                throw new Exception("No se pudo actualizar el garante.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Modificación Garante",
                $"Contrato N° {garante.IdContrato}: garante DNI {garante.Dni} actualizado.");
        }

        public void Eliminar(int idGarante)
        {
            Garante garante = garanteDAL.BuscarPorId(idGarante);
            if (garante == null)
                throw new Exception("El garante no existe.");
            ContratoVigente(garante.IdContrato);

            if (!garanteDAL.Eliminar(idGarante))
                throw new Exception("No se pudo eliminar el garante.");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Garante",
                $"Contrato N° {garante.IdContrato}: garante DNI {garante.Dni} eliminado.");
        }

        public List<Garante> LeerPorContrato(int idContrato) => garanteDAL.LeerPorContrato(idContrato);

        public Garante BuscarPorId(int idGarante) => garanteDAL.BuscarPorId(idGarante);

        // Datos obligatorios y largos máximos de la tabla Garante
        internal void Validar(Garante garante)
        {
            if (garante == null) throw new ArgumentNullException(nameof(garante));
            if (string.IsNullOrWhiteSpace(garante.Dni))
                throw new ArgumentException("El DNI del garante es obligatorio.");
            if (string.IsNullOrWhiteSpace(garante.Nombre) || string.IsNullOrWhiteSpace(garante.Apellido))
                throw new ArgumentException("El nombre y el apellido del garante son obligatorios.");
            if (string.IsNullOrWhiteSpace(garante.Domicilio))
                throw new ArgumentException("El domicilio del garante es obligatorio.");
            if (string.IsNullOrWhiteSpace(garante.PropiedadGarantia))
                throw new ArgumentException("La propiedad que respalda la garantía es obligatoria.");

            garante.Dni = garante.Dni.Trim();
            garante.Nombre = garante.Nombre.Trim();
            garante.Apellido = garante.Apellido.Trim();
            garante.Domicilio = garante.Domicilio.Trim();
            garante.PropiedadGarantia = garante.PropiedadGarantia.Trim();

            if (garante.Dni.Length > 20)
                throw new ArgumentException("El DNI del garante no puede superar los 20 caracteres.");
            if (garante.Nombre.Length > 100 || garante.Apellido.Length > 100)
                throw new ArgumentException("El nombre y el apellido del garante no pueden superar los 100 caracteres.");
            if (garante.Domicilio.Length > 200 || garante.PropiedadGarantia.Length > 200)
                throw new ArgumentException("El domicilio y la propiedad en garantía no pueden superar los 200 caracteres.");
        }

        private Contrato ContratoVigente(int idContrato)
        {
            Contrato contrato = contratoDAL.BuscarPorId(idContrato);
            if (contrato == null)
                throw new Exception("El contrato no existe.");
            if (!contrato.Vigente)
                throw new ArgumentException("Los garantes solo se pueden modificar en un contrato Activo o Por Vencer.");
            return contrato;
        }

        // Un contrato no puede tener dos veces al mismo garante, y el inquilino no puede ser su propio garante
        private void ValidarDniUnico(Garante garante, Contrato contrato)
        {
            if (garanteDAL.LeerPorContrato(contrato.IdContrato).Any(g => g.Dni == garante.Dni && g.IdGarante != garante.IdGarante))
                throw new ArgumentException($"El contrato ya tiene un garante con DNI {garante.Dni}.");
            Inquilino inquilino = new InquilinoDAL().BuscarPorId(contrato.IdInquilino);
            if (inquilino != null && inquilino.Dni == garante.Dni)
                throw new ArgumentException("El inquilino no puede ser garante de su propio contrato.");
        }
    }
}
