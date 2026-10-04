using System;
using System.Collections.Generic;
using System.Linq;
using BE;

namespace SERVICIOS
{
    public static class GestorDV
    {
        public static long CalcularDVH(Usuario user)
        {
            long dvh = 0;
            dvh += CalcularValorString(user.Id.ToString(), 1);
            dvh += CalcularValorString(user.Nombre, 2);
            dvh += CalcularValorString(user.Password, 3);
            dvh += CalcularValorString(user.Activo ? "1" : "0", 4);
            dvh += CalcularValorString(user.NombrePersona ?? "", 5);
            dvh += CalcularValorString(user.Apellido ?? "", 6);
            return dvh;
        }

        private static long CalcularValorString(string valor, int posicionAtributo)
        {
            if (string.IsNullOrEmpty(valor)) return 0;
            long suma = 0;
            for (int i = 0; i < valor.Length; i++)
            {
                // Carácter * (posición 1-based) * posición del atributo
                suma += (long)valor[i] * (i + 1) * posicionAtributo;
            }
            return suma;
        }

        public static long CalcularDVV(IEnumerable<long> dvhs)
        {
            return dvhs.Sum();
        }
    }
}
