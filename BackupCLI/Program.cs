using System;
using System.IO;
using System.Linq;
using BLL;

namespace BackupCLI
{
    class Program
    {
        private static BackupBLL backupBLL = new BackupBLL();
        private static string backupFolder;

        static void Main(string[] args)
        {
            try
            {
                backupFolder = backupBLL.ObtenerDirectorioDefaultBackup();
            }
            catch (Exception ex)
            {
                backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            }

            if (!Directory.Exists(backupFolder))
            {
                try
                {
                    Directory.CreateDirectory(backupFolder);
                }
                catch { }
            }

            Console.Title = "TpIngSoft - Utilidad de Backup y Restauración (CLI)";
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=========================================================");
                Console.WriteLine("       TP-ING-SOFT - GESTOR DE RESGUARDOS (CLI)         ");
                Console.WriteLine("=========================================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Directorio de backups: {backupFolder}");
                Console.WriteLine();
                Console.WriteLine("1. Validar Integridad de Base de Datos (DV)");
                Console.WriteLine("2. Crear Nuevo Backup (Valida DV antes)");
                Console.WriteLine("3. Listar Backups Disponibles");
                Console.WriteLine("4. Restaurar Backup");
                Console.WriteLine("5. Restaurar Day Zero (Scripts de Inicialización)");
                Console.WriteLine("6. Salir");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Seleccione una opción: ");
                Console.ForegroundColor = ConsoleColor.White;

                string opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        MenuValidarIntegridad();
                        break;
                    case "2":
                        MenuCrearBackup();
                        break;
                    case "3":
                        MenuListarBackups();
                        PressAnyKey();
                        break;
                    case "4":
                        MenuRestaurarBackup();
                        break;
                    case "5":
                        MenuRestaurarDayZero();
                        break;
                    case "6":
                        salir = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Opción no válida.");
                        Console.ForegroundColor = ConsoleColor.White;
                        PressAnyKey();
                        break;
                }
            }
        }

        static void MenuValidarIntegridad()
        {
            Console.Clear();
            Console.WriteLine("=== VALIDANDO INTEGRIDAD DE DATOS ===");
            string errorDetail;
            bool ok = backupBLL.ValidarIntegridad(out errorDetail);
            if (ok)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[OK] Los Dígitos Verificadores (DVH/DVV) están correctos.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[ERROR] Fallo de integridad detectado:");
                Console.WriteLine(errorDetail);
            }
            Console.ForegroundColor = ConsoleColor.White;
            PressAnyKey();
        }

        static void MenuCrearBackup()
        {
            Console.Clear();
            Console.WriteLine("=== CREAR NUEVO BACKUP ===");
            Console.WriteLine("Validando integridad antes de proceder...");
            
            string errorDetail;
            if (!backupBLL.ValidarIntegridad(out errorDetail))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[ABORTADO] No se puede realizar el backup porque la base de datos está corrupta:");
                Console.WriteLine(errorDetail);
                Console.ForegroundColor = ConsoleColor.White;
                PressAnyKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK] Validación exitosa. La base de datos es íntegra.");
            Console.ForegroundColor = ConsoleColor.White;

            string fileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            string fullPath = Path.Combine(backupFolder, fileName);

            Console.WriteLine($"\nSe generará el archivo: {fileName}");
            Console.Write("¿Confirmar creación? (S/N): ");
            string confirm = Console.ReadLine();
            if (confirm?.ToUpper() == "S")
            {
                try
                {
                    backupBLL.RealizarBackup(fullPath);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n[ÉXITO] Backup creado con éxito en:\n{fullPath}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nError al crear el backup: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }
            Console.ForegroundColor = ConsoleColor.White;
            PressAnyKey();
        }

        static void MenuListarBackups()
        {
            Console.Clear();
            Console.WriteLine("=== BACKUPS DISPONIBLES ===");
            try
            {
                var files = backupBLL.ListarArchivosBackup();
                if (files.Count == 0)
                {
                    Console.WriteLine("No se encontraron archivos de backup (.bak) en el directorio.");
                    return;
                }

                for (int i = 0; i < files.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {files[i]}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error al listar backups desde SQL Server: " + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void MenuRestaurarBackup()
        {
            Console.Clear();
            Console.WriteLine("=== RESTAURAR BACKUP ===");
            try
            {
                var files = backupBLL.ListarArchivosBackup();
                if (files.Count == 0)
                {
                    Console.WriteLine("No se encontraron archivos de backup (.bak) en el directorio.");
                    PressAnyKey();
                    return;
                }

                for (int i = 0; i < files.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {files[i]}");
                }

                Console.WriteLine();
                Console.Write("Seleccione el número de backup a restaurar (o Enter para cancelar): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int index) && index >= 1 && index <= files.Count)
                {
                    string selectedFileName = files[index - 1];
                    string fullPath = Path.Combine(backupFolder, selectedFileName);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n[ADVERTENCIA] Se va a restaurar el backup: {selectedFileName}");
                    Console.WriteLine("Esto reemplazará completamente el estado actual de la base de datos.");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("¿Está seguro de continuar? Escriba 'SI' para confirmar: ");
                    
                    if (Console.ReadLine()?.ToUpper() == "SI")
                    {
                        try
                        {
                            Console.WriteLine("\nRestaurando... Por favor espere (se cerrarán conexiones activas)...");
                            backupBLL.RestaurarBackup(fullPath);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n[ÉXITO] Base de datos restaurada correctamente.");
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nError al restaurar base de datos: " + ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nOperación cancelada.");
                    }
                }
                else
                {
                    Console.WriteLine("\nOperación cancelada.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error al listar backups desde SQL Server: " + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
            PressAnyKey();
        }

        static void MenuRestaurarDayZero()
        {
            Console.Clear();
            Console.WriteLine("=== RESTAURAR DAY ZERO ===");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[ADVERTENCIA] Esta acción ELIMINARÁ y RE-CREARÁ la base de datos utilizando los scripts de inicialización.");
            Console.WriteLine("Todos los datos actuales se perderán permanentemente.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n¿Está seguro de continuar? Escriba 'DAYZERO' para confirmar: ");

            if (Console.ReadLine()?.ToUpper() == "DAYZERO")
            {
                // Resolve script paths
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string docsDir = FindDocsDirectory(baseDir);

                if (docsDir == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nNo se pudo encontrar la carpeta 'docs' que contiene los scripts schema-init.sql y stored-procedures.sql.");
                    Console.Write("Ingrese la ruta absoluta a la carpeta 'docs' del repositorio: ");
                    docsDir = Console.ReadLine();
                }

                if (!string.IsNullOrEmpty(docsDir) && Directory.Exists(docsDir))
                {
                    string schemaPath = Path.Combine(docsDir, "schema-init.sql");
                    string spPath = Path.Combine(docsDir, "stored-procedures.sql");

                    try
                    {
                        Console.WriteLine("\nEjecutando scripts de inicialización... (Esto puede tomar unos segundos)...");
                        backupBLL.RestaurarDayZero(schemaPath, spPath);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n[ÉXITO] Base de datos re-creada y semilla de datos (Day Zero) aplicada correctamente.");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nError al restaurar Day Zero: " + ex.Message);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nRuta no válida. Operación cancelada.");
                }
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }
            PressAnyKey();
        }

        static string FindDocsDirectory(string startDir)
        {
            // 1. Hardcoded developer default path
            string defaultPath = @"C:\Users\ignac\Repos\tp-ing-soft\docs";
            if (Directory.Exists(defaultPath) && File.Exists(Path.Combine(defaultPath, "schema-init.sql")))
            {
                return defaultPath;
            }

            // 2. Relative path fallback (4 levels up from bin/Debug)
            try
            {
                string relativePath = Path.GetFullPath(Path.Combine(startDir, @"..\..\..\..\docs"));
                if (Directory.Exists(relativePath) && File.Exists(Path.Combine(relativePath, "schema-init.sql")))
                {
                    return relativePath;
                }
            }
            catch { }

            // 3. Search up levels
            string current = startDir;
            while (!string.IsNullOrEmpty(current))
            {
                string candidate = Path.Combine(current, "docs");
                if (Directory.Exists(candidate) && File.Exists(Path.Combine(candidate, "schema-init.sql")))
                {
                    return candidate;
                }
                var parent = Directory.GetParent(current);
                if (parent == null || parent.FullName == current) break;
                current = parent.FullName;
            }

            return null;
        }

        static void PressAnyKey()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey(true);
        }
    }
}
