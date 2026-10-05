using System;

namespace BE
{
    // Los nombres deben coincidir con el CHECK de AccionLegal.tipo_accion en la base
    public enum TipoAccion
    {
        Intimacion,
        Resuelto,
        Desalojo
    }

    public class AccionLegal
    {
        public int IdAccion { get; set; }
        public DateTime FechaAccion { get; set; }
        public string Descripcion { get; set; }
        public TipoAccion TipoAccion { get; set; }
        public int IdContrato { get; set; }
        public int IdUsuarioLegal { get; set; }

        public override string ToString()
        {
            return $"{TipoAccion} - {FechaAccion:dd/MM/yyyy}";
        }
    }
}
