using System;
using System.Collections.Generic;
using System.Globalization;

namespace SERVICIOS
{
    // Índice del Costo de la Construcción del INDEC (edificios de vivienda en CABA y GBA), publicado como CSV mensual:
    //   periodo;nivel_general_aperturas;indice_icc
    //   1/9/2026;nivel_general;1779,72267226632
    // (fecha d/M/yyyy, separador ';', decimales con coma). Solo se toma el nivel general.
    public class ClienteICC
    {
        public const string URL = "https://www.indec.gob.ar/ftp/cuadros/economia/indice_icc_general_capitulos.csv";
        private const string APERTURA = "nivel_general";

        // Valor del índice por mes (primer día del mes). Lanza una excepción con un mensaje claro si no hay conexión
        // o si el INDEC cambió el formato del archivo.
        public Dictionary<DateTime, decimal> ObtenerNivelGeneral()
        {
            // Sin Accept: el INDEC lo sirve como application/octet-stream y responde 406 si se pide text/csv
            string csv = ClienteHttp.Get(URL, "el INDEC");
            string[] lineas = csv.Replace("\r", "").Split('\n');
            if (lineas.Length < 2 || !lineas[0].Replace("\"", "").Trim().StartsWith("periodo;nivel_general_aperturas;indice_icc"))
                throw new Exception("El INDEC cambió el formato del CSV del ICC (encabezado inesperado).");

            CultureInfo coma = CultureInfo.GetCultureInfo("es-AR");
            Dictionary<DateTime, decimal> valores = new Dictionary<DateTime, decimal>();
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Replace("\"", "").Split(';');
                if (campos.Length < 3 || campos[1].Trim() != APERTURA) continue;
                if (!DateTime.TryParseExact(campos[0].Trim(), "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime periodo) ||
                    !decimal.TryParse(campos[2].Trim(), NumberStyles.Float, coma, out decimal valor))
                    throw new Exception($"El INDEC cambió el formato del CSV del ICC (línea {i + 1}: \"{lineas[i]}\").");
                valores[new DateTime(periodo.Year, periodo.Month, 1)] = valor;
            }
            if (valores.Count == 0)
                throw new Exception("El CSV del ICC del INDEC no trae valores del nivel general.");
            return valores;
        }
    }
}
