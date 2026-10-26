using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using BE;

namespace SERVICIOS
{
    // Parámetros urbanísticos de una parcela de CABA a partir de su dirección (servicios públicos del GCBA, sin
    // autenticación):
    //  1. USIG normaliza la dirección y devuelve el código de calle (y la coordenada sobre el eje de la calle)
    //  2. EPOK catastro devuelve la parcela (SMP y superficie) por código de calle y altura. Si esa altura no existe
    //     como puerta se prueban las vecinas (±2) y, por último, puntos alrededor de la coordenada, aceptando solo
    //     una parcela con puerta sobre la misma calle.
    //  3. EPOK edificabilidad (el servicio de Ciudad 3D) devuelve superficie edificable en planta, altura máxima y
    //     distrito según el Código Urbanístico
    // Los servicios del GCBA a veces responden con error 502 o tardan: cada consulta se reintenta unas veces.
    public class ClienteEpok
    {
        public const string URL_USIG = "https://servicios.usig.buenosaires.gob.ar/normalizar/?direccion={0}&geocodificar=true&srid=4326";
        public const string URL_PARCELA = "https://epok.buenosaires.gob.ar/catastro/parcela/?{0}";
        public const string URL_EDIFICABILIDAD = "https://epok.buenosaires.gob.ar/cur3d/seccion_edificabilidad/?smp={0}";
        private const int INTENTOS = 4, TIMEOUT_MS = 12000;

        // Localidades que se consultan en EPOK (el resto se carga a mano)
        public static bool EsCaba(string localidad)
        {
            if (string.IsNullOrWhiteSpace(localidad)) return false;
            string l = SinAcentos(localidad).ToUpperInvariant();
            return l.Contains("CABA") || l.Contains("CAPITAL FEDERAL") || l.Contains("CIUDAD DE BUENOS AIRES")
                   || l.Contains("CIUDAD AUTONOMA DE BUENOS AIRES");
        }

        // ---------- JSON ----------

        [DataContract] private class RespuestaUsig { [DataMember(Name = "direccionesNormalizadas")] public List<DireccionUsig> Direcciones { get; set; } }

        [DataContract]
        private class DireccionUsig
        {
            [DataMember(Name = "direccion")] public string Direccion { get; set; }
            [DataMember(Name = "cod_calle")] public int? CodigoCalle { get; set; }
            [DataMember(Name = "altura")] public int? Altura { get; set; }
            [DataMember(Name = "cod_partido")] public string Partido { get; set; }
            [DataMember(Name = "coordenadas")] public CoordenadasUsig Coordenadas { get; set; }
        }

        [DataContract] private class CoordenadasUsig { [DataMember(Name = "x")] public string X { get; set; } [DataMember(Name = "y")] public string Y { get; set; } }

        [DataContract]
        private class ParcelaEpok
        {
            [DataMember(Name = "smp")] public string Smp { get; set; }
            [DataMember(Name = "direccion")] public string Direccion { get; set; }
            [DataMember(Name = "superficie_total")] public string SuperficieTotal { get; set; }
            [DataMember(Name = "puertas")] public List<PuertaEpok> Puertas { get; set; }
        }

        [DataContract] private class PuertaEpok { [DataMember(Name = "codigo_calle")] public int CodigoCalle { get; set; } [DataMember(Name = "altura")] public int Altura { get; set; } }

        [DataContract]
        private class EdificabilidadEpok
        {
            [DataMember(Name = "sup_edificable_planta")] public decimal? SuperficieEdificablePlanta { get; set; }
            [DataMember(Name = "altura_max")] public List<decimal> AlturaMaxima { get; set; }
            [DataMember(Name = "superficie_parcela")] public decimal? SuperficieParcela { get; set; }
            [DataMember(Name = "plusvalia")] public PlusvaliaEpok Plusvalia { get; set; }
            [DataMember(Name = "distrito_especial")] public List<DistritoEspecialEpok> DistritoEspecial { get; set; }
        }

        [DataContract] private class PlusvaliaEpok { [DataMember(Name = "distrito_cpu")] public string Distrito { get; set; } }

        [DataContract] private class DistritoEspecialEpok { [DataMember(Name = "distrito_especifico")] public string Especifico { get; set; } }

        // ---------- Consulta ----------

        // direccion: calle y altura ("Gurruchaga 1800"). Lanza una excepción con un mensaje claro si no hay conexión,
        // si USIG no reconoce la dirección o si no se encuentra la parcela.
        public ParcelaCaba Consultar(string direccion)
        {
            if (string.IsNullOrWhiteSpace(direccion))
                throw new ArgumentException("El inmueble no tiene dirección.");

            // 1. USIG
            RespuestaUsig usig = Leer<RespuestaUsig>(string.Format(URL_USIG, Uri.EscapeDataString(direccion.Trim())), "USIG (normalizador de direcciones)");
            DireccionUsig normalizada = usig?.Direcciones?.FirstOrDefault(d => d.Partido == "caba" && d.CodigoCalle.HasValue && d.Altura.HasValue);
            if (normalizada == null)
                throw new ArgumentException($"USIG no reconoce \"{direccion}\" como una dirección de CABA con calle y altura.");
            int codigo = normalizada.CodigoCalle.Value, altura = normalizada.Altura.Value;

            // 2. Parcela: la altura exacta, las vecinas y, por último, alrededor de la coordenada
            ParcelaCaba resultado = new ParcelaCaba { DireccionNormalizada = normalizada.Direccion };
            ParcelaEpok parcela = null;
            foreach (int delta in new[] { 0, 1, -1, 2, -2 })
            {
                parcela = BuscarParcela($"codigo_calle={codigo}&altura={altura + delta}");
                if (parcela != null)
                {
                    resultado.AlturaConsultada = altura + delta;
                    resultado.Busqueda = delta == 0 ? "dirección exacta" : $"altura vecina {altura + delta}";
                    break;
                }
            }
            if (parcela == null && normalizada.Coordenadas != null)
            {
                parcela = BuscarAlrededor(normalizada.Coordenadas, codigo, altura);
                if (parcela != null)
                {
                    PuertaEpok puerta = parcela.Puertas.Where(p => p.CodigoCalle == codigo).OrderBy(p => Math.Abs(p.Altura - altura)).First();
                    resultado.AlturaConsultada = puerta.Altura;
                    resultado.Busqueda = $"parcela más cercana con puerta en la misma calle ({puerta.Altura})";
                }
            }
            if (parcela == null)
                throw new ArgumentException($"EPOK no encontró la parcela de {normalizada.Direccion} (ni en las alturas vecinas).");
            resultado.Smp = parcela.Smp;
            resultado.DireccionCatastral = parcela.Direccion;
            resultado.SuperficieParcela = Numero(parcela.SuperficieTotal);

            // 3. Edificabilidad
            EdificabilidadEpok e = Leer<EdificabilidadEpok>(string.Format(URL_EDIFICABILIDAD, Uri.EscapeDataString(parcela.Smp)), "EPOK (edificabilidad)");
            if (e == null || !e.SuperficieEdificablePlanta.HasValue || e.AlturaMaxima == null || e.AlturaMaxima.Count == 0)
                throw new Exception($"EPOK no tiene datos de edificabilidad para la parcela {parcela.Smp}.");
            resultado.SuperficieEdificablePlanta = Math.Round(e.SuperficieEdificablePlanta.Value, 2);
            resultado.AlturaMaxima = Math.Round(e.AlturaMaxima.Max(), 2);
            if (e.SuperficieParcela > 0) resultado.SuperficieParcela = e.SuperficieParcela;
            string especial = e.DistritoEspecial?.Select(d => d.Especifico).FirstOrDefault(d => !string.IsNullOrWhiteSpace(d));
            resultado.Distrito = string.Join(" - ", new[] { e.Plusvalia?.Distrito, especial }.Where(d => !string.IsNullOrWhiteSpace(d)));
            return resultado;
        }

        // null si EPOK no tiene una parcela con esa puerta
        private ParcelaEpok BuscarParcela(string consulta)
        {
            ParcelaEpok p = Leer<ParcelaEpok>(string.Format(URL_PARCELA, consulta), "EPOK (catastro)");
            return string.IsNullOrEmpty(p?.Smp) ? null : p;
        }

        // La coordenada de USIG cae en el eje de la calle: se prueban puntos a 12 y 20 m en 8 direcciones y se toma la
        // parcela con una puerta sobre la misma calle más cercana a la altura buscada
        private ParcelaEpok BuscarAlrededor(CoordenadasUsig c, int codigoCalle, int altura)
        {
            if (!double.TryParse(c.X, NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                !double.TryParse(c.Y, NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                return null;
            List<ParcelaEpok> candidatas = new List<ParcelaEpok>();
            foreach (int metros in new[] { 12, 20 })
                for (int k = 0; k < 8; k++)
                {
                    double angulo = k * Math.PI / 4;
                    double lng = x + metros * Math.Cos(angulo) / (111320 * Math.Cos(y * Math.PI / 180));
                    double lat = y + metros * Math.Sin(angulo) / 110540;
                    ParcelaEpok p = BuscarParcela(string.Format(CultureInfo.InvariantCulture, "lng={0:0.0000000}&lat={1:0.0000000}", lng, lat));
                    if (p?.Puertas != null && p.Puertas.Any(q => q.CodigoCalle == codigoCalle) && candidatas.All(q => q.Smp != p.Smp))
                        candidatas.Add(p);
                }
            return candidatas.OrderBy(p => p.Puertas.Where(q => q.CodigoCalle == codigoCalle).Min(q => Math.Abs(q.Altura - altura))).FirstOrDefault();
        }

        // GET con reintentos (los servicios del GCBA devuelven 502 o se cortan de vez en cuando)
        private static T Leer<T>(string url, string servicio) where T : class
        {
            Exception ultimo = null;
            for (int i = 0; i < INTENTOS; i++)
            {
                try
                {
                    string json = ClienteHttp.Get(url, servicio, TIMEOUT_MS, "application/json");
                    if (!json.TrimStart().StartsWith("{"))
                        throw new Exception($"{servicio} respondió algo que no es JSON.");
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                        return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
                }
                catch (Exception ex)
                {
                    ultimo = ex;
                    if (i < INTENTOS - 1) Thread.Sleep(2500);
                }
            }
            throw new Exception($"No se pudo consultar {servicio}: {ultimo?.Message}", ultimo);
        }

        private static decimal? Numero(string texto) =>
            decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal v) && v > 0 ? v : (decimal?)null;

        private static string SinAcentos(string texto)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in texto.Normalize(NormalizationForm.FormD))
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
            return sb.ToString();
        }
    }
}
