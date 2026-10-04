using System;
using System.Windows.Forms;

namespace TpIngSoft
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BE.ReporteIntegridad reporte = null;
            try
            {
                reporte = new BLL.IntegridadBLL().VerificarIntegridad();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al intentar realizar la comprobación de integridad: " + ex.Message,
                    "Error Crítico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Application.Exit();
                return;
            }

            if (reporte != null && !reporte.EsValido)
            {
                if (reporte.AdminCorrupto || reporte.DvvInvalido)
                {
                    string msg = reporte.DvvInvalido
                        ? "ERROR CRÍTICO DE INTEGRIDAD: La estructura de la tabla de usuarios ha sido alterada externamente."
                        : "ERROR CRÍTICO DE INTEGRIDAD: Un usuario Administrador ha sido vulnerado.";
                    MessageBox.Show(
                        msg + "\n\nEl inicio de sesión ha sido deshabilitado por seguridad.\nPor favor, use la herramienta externa CLI para restaurar un backup.",
                        "Error de Integridad Crítico",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Application.Exit();
                    return;
                }
                else
                {
                    SERVICIOS.SessionManager.Instance.ReporteIntegridadTemporal = reporte;
                }
            }

            BLL.UsuarioBLL usuarioBLL = new BLL.UsuarioBLL();
            if (usuarioBLL.ValidarSesionLocal() || new FormLogin().ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FormMain());
            }
        }
    }
}
