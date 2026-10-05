; ====================================================
; Instalador de LandSmart (Inno Setup 6)
; Compilar desde la raíz del proyecto, después de: dotnet build -c Release -p:Platform=x64
;   "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" instalador.iss
; ====================================================

#define AppNombre "LandSmart"
#define AppVersion "1.0.0"
#define AppEditor "Maximiliano Vricciariello"
#define AppExe "TpIngSoft.exe"
; Instancia de SQL Server y base: deben coincidir con la cadena de conexión de TpIngSoft.exe.config y BackupCLI.exe.config
#define SqlInstancia ".\SQLEXPRESS"
#define SqlBase "TpIngSoft"

[Setup]
; AppId identifica la aplicación para actualizaciones y desinstalación: no cambiarlo entre versiones
AppId={{3BA235EB-E2A7-4614-A946-487629E30CD4}
AppName={#AppNombre}
AppVersion={#AppVersion}
AppVerName={#AppNombre} {#AppVersion}
AppPublisher={#AppEditor}
DefaultDirName={commonpf}\{#AppNombre}
DefaultGroupName={#AppNombre}
UninstallDisplayIcon={app}\{#AppExe}
; Wizard: bienvenida, destino, listo para instalar, progreso y finalizado
DisableWelcomePage=no
DisableDirPage=no
DisableProgramGroupPage=yes
DisableReadyPage=no
WizardStyle=modern
; x64: instala en "Program Files" (no en "Program Files (x86)")
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
; Program Files y la creación de la base requieren administrador
PrivilegesRequired=admin
OutputDir=Output
OutputBaseFilename={#AppNombre}-Setup-{#AppVersion}
Compression=lzma2
SolidCompression=yes

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Files]
; Aplicación: exe, config y dlls (sin .pdb de depuración ni .xml de documentación)
Source: "GUI\bin\x64\Release\*"; DestDir: "{app}"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion
; Herramienta de backup y restore (usa las mismas BE/BLL/DAL/SERVICIOS que la aplicación)
Source: "BackupCLI\bin\x64\Release\BackupCLI.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "BackupCLI\bin\x64\Release\BackupCLI.exe.config"; DestDir: "{app}"; Flags: ignoreversion
; Scripts de la base de datos (se ejecutan al final de la instalación)
Source: "schema-init.sql"; DestDir: "{app}\BaseDeDatos"; Flags: ignoreversion
Source: "stored-procedures.sql"; DestDir: "{app}\BaseDeDatos"; Flags: ignoreversion

[Icons]
; Solo en el menú Inicio: sin acceso directo en el escritorio
Name: "{group}\{#AppNombre}"; Filename: "{app}\{#AppExe}"
Name: "{group}\Desinstalar {#AppNombre}"; Filename: "{uninstallexe}"

[UninstallDelete]
; Logs de sqlcmd generados durante la instalación (la base de datos no se borra al desinstalar)
Type: files; Name: "{app}\BaseDeDatos\*.log"

[Code]
const
  NET472_RELEASE = 461808;   // valor "Release" del registro para .NET Framework 4.7.2

// .NET Framework 4.7.2 o posterior (Windows 10 1803+ ya lo trae)
function InitializeSetup(): Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release)
            and (Release >= NET472_RELEASE);
  if not Result then
    SuppressibleMsgBox('{#AppNombre} requiere .NET Framework 4.7.2 o posterior.' + #13#10 +
      'Instálelo desde el sitio de Microsoft y vuelva a ejecutar el instalador.',
      mbCriticalError, MB_OK, IDOK);
end;

// Ejecuta sqlcmd contra la instancia. Devuelve False si sqlcmd no se pudo iniciar (no está instalado).
function EjecutarSqlcmd(const Parametros: String; var Codigo: Integer): Boolean;
begin
  Result := Exec('sqlcmd.exe', '-S "{#SqlInstancia}" -E -b -l 15 ' + Parametros, '',
                 SW_HIDE, ewWaitUntilTerminated, Codigo);
end;

// -f 65001: los scripts están en UTF-8 sin BOM (sin esto se rompen las tildes de las traducciones)
// -I: QUOTED_IDENTIFIER ON (índice filtrado de Contrato)
function EjecutarScript(const Script: String): Boolean;
var
  Carpeta: String;
  Codigo: Integer;
begin
  Carpeta := ExpandConstant('{app}\BaseDeDatos\');
  Result := EjecutarSqlcmd('-I -f 65001 -i "' + Carpeta + Script + '.sql" -o "' + Carpeta + Script + '.log"', Codigo)
            and (Codigo = 0);
  if not Result then
    SuppressibleMsgBox('Falló la ejecución de ' + Script + '.sql (código ' + IntToStr(Codigo) + ').' + #13#10 +
      'Revise el detalle en ' + Carpeta + Script + '.log', mbError, MB_OK, IDOK);
end;

procedure MostrarPasosManuales(const Motivo: String);
var
  Carpeta: String;
begin
  Carpeta := ExpandConstant('{app}\BaseDeDatos\');
  SuppressibleMsgBox(Motivo + #13#10#13#10 +
    'Los archivos de {#AppNombre} se instalaron, pero falta crear la base de datos. ' +
    'Con SQL Server disponible, ejecute en una consola:' + #13#10#13#10 +
    'sqlcmd -S {#SqlInstancia} -E -b -I -f 65001 -i "' + Carpeta + 'schema-init.sql"' + #13#10 +
    'sqlcmd -S {#SqlInstancia} -E -b -I -f 65001 -i "' + Carpeta + 'stored-procedures.sql"',
    mbInformation, MB_OK, IDOK);
end;

// schema-init.sql BORRA y recrea todas las tablas: en una instalación nueva se ejecuta directamente;
// si la base ya existe (reinstalación o actualización) solo se recrea si el usuario lo confirma.
// stored-procedures.sql se puede ejecutar más de una vez sin perder datos: se ejecuta siempre.
procedure CrearBaseDeDatos();
var
  Codigo: Integer;
  Recrear: Boolean;
begin
  WizardForm.StatusLabel.Caption := 'Conectando con SQL Server ({#SqlInstancia})...';
  if not EjecutarSqlcmd('-Q "SELECT 1"', Codigo) then
  begin
    MostrarPasosManuales('No se encontró sqlcmd (se instala con SQL Server).');
    Exit;
  end;
  if Codigo <> 0 then
  begin
    MostrarPasosManuales('No se pudo conectar con SQL Server en {#SqlInstancia}.');
    Exit;
  end;

  // Código 1 si la base ya existe
  EjecutarSqlcmd('-Q "IF DB_ID(''{#SqlBase}'') IS NOT NULL RAISERROR(''existe'', 16, 1)"', Codigo);
  Recrear := Codigo = 0;
  if not Recrear then
    Recrear := SuppressibleMsgBox('La base de datos {#SqlBase} ya existe en {#SqlInstancia}.' + #13#10#13#10 +
      '¿Desea recrearla? Se BORRARÁN todos sus datos (usuarios, captaciones, contratos, pagos, etc.).' + #13#10#13#10 +
      'Elija "No" para conservar los datos: solo se actualizarán los stored procedures.',
      mbConfirmation, MB_YESNO or MB_DEFBUTTON2, IDNO) = IDYES;

  if Recrear then
  begin
    WizardForm.StatusLabel.Caption := 'Creando la base de datos {#SqlBase}...';
    if not EjecutarScript('schema-init') then
      Exit;
  end;

  WizardForm.StatusLabel.Caption := 'Creando los stored procedures...';
  EjecutarScript('stored-procedures');
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
    CrearBaseDeDatos();
end;
