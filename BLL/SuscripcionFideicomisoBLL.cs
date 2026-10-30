using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using DAL;
using SERVICIOS;

namespace BLL
{
    // Aportes de los inversores a los fideicomisos aprobados (sector Inversiones). El capital necesario es el costo
    // total de la obra; la suma de los aportes no puede superarlo (también lo controla el SP, en una transacción).
    public class SuscripcionFideicomisoBLL
    {
        private readonly SuscripcionFideicomisoDAL suscripcionDAL = new SuscripcionFideicomisoDAL();
        private readonly ProyectoInversionDAL proyectoDAL = new ProyectoInversionDAL();
        private readonly InversorDAL inversorDAL = new InversorDAL();

        public const decimal M2_POR_UNIDAD = 60; // superficie de una unidad típica, para pasar de m² a unidades

        // Lo que le corresponde a un aporte: los inversores se reparten los m² construibles que no van al propietario
        // por el canje, en proporción al capital. m² = construibles x (100% - canje) x aporte / capital necesario;
        // unidades = m² / M2_POR_UNIDAD redondeado (sugerencia: Inversiones puede corregirlo).
        public static (decimal Metros, int Unidades) CalcularAsignacion(ProyectoInversion proyecto, decimal capitalNecesario, decimal montoAportado)
        {
            if (proyecto?.MetrosConstruibles == null || capitalNecesario <= 0 || montoAportado <= 0) return (0, 0);
            decimal metros = Math.Round(proyecto.MetrosConstruibles.Value * (100 - proyecto.PorcentajeCanje) / 100 * montoAportado / capitalNecesario, 2);
            return (metros, (int)Math.Round(metros / M2_POR_UNIDAD, MidpointRounding.AwayFromZero));
        }

        public int AgregarSuscripcion(SuscripcionFideicomiso suscripcion)
        {
            ProyectoInversionBLL.ExigirPermiso(PermisosDefinidos.AccesoPN3Inversiones, "Inversiones");
            if (suscripcion == null) throw new ArgumentNullException(nameof(suscripcion));
            if (suscripcion.MontoAportado <= 0)
                throw new ArgumentException("El monto aportado debe ser mayor a cero.");
            if (suscripcion.UnidadesAsignadas < 0)
                throw new ArgumentException("Las unidades asignadas no pueden ser negativas.");

            ProyectoInversion proyecto = proyectoDAL.BuscarPorId(suscripcion.IdProyecto);
            if (proyecto == null)
                throw new Exception("El proyecto no existe.");
            if (proyecto.Estado != EstadoProyecto.Aprobado)
                throw new ArgumentException($"Solo se suscribe a fideicomisos de proyectos Aprobados (este está {ProyectoInversionBLL.Texto(proyecto.Estado)}).");
            Inversor inversor = inversorDAL.BuscarPorId(suscripcion.IdInversor);
            if (inversor == null)
                throw new ArgumentException("El inversor no existe.");
            decimal restante = proyecto.CapitalNecesario - proyecto.CapitalSuscripto;
            if (suscripcion.MontoAportado > restante)
                throw new ArgumentException($"El aporte supera el capital necesario: restan USD {Math.Max(0, restante):N2} " +
                                            $"de USD {proyecto.CapitalNecesario:N2}.");

            int id = suscripcionDAL.Crear(suscripcion);
            if (id == -1)
                throw new Exception("No se pudo registrar el aporte (puede que otro aporte simultáneo haya completado el capital).");

            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Suscripción Fideicomiso",
                $"Proyecto N° {proyecto.IdProyecto}: {inversor.NombreCompleto} (CUIT {inversor.Cuit}) aporta USD {suscripcion.MontoAportado:N2}" +
                (suscripcion.UnidadesAsignadas > 0 ? $" ({suscripcion.UnidadesAsignadas} unidades)." : "."));
            return id;
        }

        public void EliminarSuscripcion(int idSuscripcion)
        {
            ProyectoInversionBLL.ExigirPermiso(PermisosDefinidos.AccesoPN3Inversiones, "Inversiones");
            if (!suscripcionDAL.Eliminar(idSuscripcion))
                throw new Exception("No se pudo eliminar el aporte.");
            GestorBitacora.Instance.RegistrarEvento(SessionManager.Instance.UsuarioActual, "Baja Suscripción Fideicomiso",
                $"Aporte {idSuscripcion} eliminado.");
        }

        public List<SuscripcionFideicomiso> LeerPorProyecto(int idProyecto) => suscripcionDAL.LeerPorProyecto(idProyecto);

        // Capital necesario, suscripto, restante, % fondeado e inversores con su participación
        public ResumenFideicomiso ObtenerResumenFideicomiso(int idProyecto)
        {
            ProyectoInversion proyecto = proyectoDAL.BuscarPorId(idProyecto);
            if (proyecto == null)
                throw new Exception("El proyecto no existe.");
            List<SuscripcionFideicomiso> suscripciones = suscripcionDAL.LeerPorProyecto(idProyecto);
            return new ResumenFideicomiso
            {
                Proyecto = proyecto,
                CapitalNecesario = proyecto.CapitalNecesario,
                CapitalSuscripto = suscripciones.Sum(s => s.MontoAportado),
                Suscripciones = suscripciones
            };
        }
    }
}
