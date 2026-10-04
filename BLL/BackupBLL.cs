using System;
using System.IO;
using System.Collections.Generic;
using DAL;

namespace BLL
{
    public class BackupBLL
    {
        private MapperBackup mapperBackup = new MapperBackup();
        private IntegridadBLL integridadBLL = new IntegridadBLL();

        public string ObtenerDirectorioDefaultBackup()
        {
            return mapperBackup.ObtenerDirectorioDefaultBackup();
        }

        public bool ValidarIntegridad(out string errorDetail)
        {
            errorDetail = "";
            try
            {
                var reporte = integridadBLL.VerificarIntegridad();
                if (!reporte.EsValido)
                {
                    var msg = new System.Text.StringBuilder("Fallo de integridad detectado:\n");
                    if (reporte.DvvInvalido)
                    {
                        msg.AppendLine("- Estructura de tabla alterada (DVV inválido)");
                    }
                    if (reporte.UsuariosCorruptos.Count > 0)
                    {
                        msg.AppendLine("- Usuarios corruptos: " + string.Join(", ", reporte.UsuariosCorruptos));
                    }
                    errorDetail = msg.ToString();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorDetail = "Error al verificar integridad: " + ex.Message;
                return false;
            }
        }

        public void RealizarBackup(string path)
        {
            string errorDetail;
            if (!ValidarIntegridad(out errorDetail))
            {
                throw new InvalidOperationException("No se puede generar un backup porque los dígitos verificadores fallaron:\n" + errorDetail);
            }

            mapperBackup.RealizarBackup(path);
        }

        public void RestaurarBackup(string path)
        {
            mapperBackup.RestaurarBackup(path);
        }

        public void RestaurarDayZero(string scriptPath, string proceduresPath)
        {
            mapperBackup.RestaurarDayZero(scriptPath, proceduresPath);
        }

        public List<string> ListarArchivosBackup()
        {
            string backupDir = ObtenerDirectorioDefaultBackup();
            return mapperBackup.ListarArchivosBackup(backupDir);
        }
    }
}
