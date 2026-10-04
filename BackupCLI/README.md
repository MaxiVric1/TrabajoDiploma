# TpIngSoft - Utilidad de Backup y Restauración (CLI)

Este utilitario externo de línea de comandos permite administrar los resguardos y restauraciones de la base de datos `tpingsoft` de manera independiente del sistema principal, cumpliendo con los requerimientos de seguridad e integridad del sistema (DVH/DVV).

## Requisitos Previos

1. **Instancia de SQL Server**: El utilitario intenta conectarse a `localhost` (instancia por defecto de SQL Server en tu máquina) utilizando seguridad integrada de Windows (`Integrated Security=True`). Asegúrate de que el motor de base de datos esté en ejecución y que el usuario actual tenga permisos de administración (`sysadmin` o `dbcreator` / `dbowner`).
2. **Sistema Cerrado (Recomendado)**: Se recomienda que la aplicación principal (`GUI`) esté cerrada al realizar operaciones de restauración para evitar bloqueos por tablas en uso, aunque el CLI cerrará de forma forzada las conexiones activas si fuera necesario.

---

## Cómo Ejecutar el CLI

Una vez compilada la solución, puedes ejecutar la aplicación de consola desde la siguiente ubicación:
`TpIngSoft/BackupCLI/bin/Debug/BackupCLI.exe`

---

## Funcionalidades del Menú

### 1. Validar Integridad de Base de Datos (DV)
Realiza un escaneo de la tabla `Usuario` validando los Dígitos Verificadores Horizontales (DVH) de cada fila y el Dígito Verificador Vertical (DVV) general de la tabla.
* Muestra el detalle de qué registros específicos presentan problemas de integridad (si los hubiera).

### 2. Crear Nuevo Backup
Genera un respaldo completo de la base de datos actual en un archivo con extensión `.bak`.
* **Regla de Seguridad**: Antes de generar el backup, el CLI ejecuta una verificación de integridad. **Si se detecta alguna falla de integridad (DV corrupto), el backup se aborta automáticamente** para evitar respaldar un estado inconsistente.
* Los backups se almacenan con un nombre autogenerado con la fecha y hora actual (ej. `Backup_20260611_152400.bak`) dentro del subdirectorio `Backups` ubicado en la carpeta del ejecutable.

### 3. Listar Backups Disponibles
Muestra todos los archivos de respaldo (`.bak`) guardados en el directorio de backups, ordenados desde el más reciente al más antiguo, con detalles de su tamaño y fecha de creación.

### 4. Restaurar Backup
Permite seleccionar uno de los backups listados para aplicarlo sobre la base de datos actual.
* **Manejo de Conexiones**: El script de restauración pone temporalmente la base de datos en modo `SINGLE_USER WITH ROLLBACK IMMEDIATE` para forzar la desconexión de cualquier sesión activa (por ejemplo, si el sistema principal quedó abierto accidentalmente), restaura la base de datos reemplazándola con el backup y, finalmente, restablece la base de datos a modo `MULTI_USER`.

### 5. Restaurar Day Zero
Opción de recuperación extrema que destruye la base de datos `tpingsoft` y la vuelve a construir ejecutando en orden:
1. `docs/schema-init.sql` (creación de tablas, idiomas iniciales, roles, permisos y usuario administrador).
2. `docs/stored-procedures.sql` (re-creación de todos los procedimientos almacenados).
* Esta opción restablece el sistema a su estado original de "semilla", incluyendo los hashes y dígitos verificadores iniciales del administrador por defecto.

---

## Notas de Configuración

* **Directorio de Backups**: Los archivos `.bak` se crean en una subcarpeta llamada `Backups` al lado del binario. Puedes copiar manualmente archivos `.bak` allí y el CLI los listará automáticamente al presionar la opción correspondente.
* **Scripts Day Zero**: El CLI busca la carpeta `docs` del repositorio subiendo niveles de directorio de forma automática. Si ejecutas la aplicación desde una ubicación externa y no encuentra los scripts, el CLI te solicitará que ingreses manualmente la ruta absoluta a la carpeta `docs`.
