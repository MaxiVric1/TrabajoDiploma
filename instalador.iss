; ====================================================
; Instalador de LandSmart (Inno Setup 6)
; Compilar desde la raíz del proyecto, después de: dotnet build -c Release -p:Platform=x64
;   "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" instalador.iss
; ====================================================

#define AppNombre "LandSmart"
#define AppVersion "1.0.0"
#define AppEditor "Maximiliano Vricciariello"
#define AppExe "TpIngSoft.exe"
; Base de datos: debe coincidir con Initial Catalog de TpIngSoft.exe.config y BackupCLI.exe.config.
; La instancia de SQL Server se detecta al instalar, con el mismo criterio que DAL/ServidorSql.cs.
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
Source: "permisos.sql"; DestDir: "{app}\BaseDeDatos"; Flags: ignoreversion

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

// ---------- Detección de la instancia de SQL Server (mismo criterio que DAL/ServidorSql.cs) ----------

const
  CLAVE_INSTANCIAS = 'SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL';
  INSTANCIA_POR_DEFECTO = 'MSSQLSERVER';
  // Códigos de salida de ProbarInstancia (sqlcmd devuelve el valor de EXIT(SELECT ...); 1 si no se conecta)
  SQL_SIN_BASE = 10;
  SQL_CON_BASE = 11;

var
  Instancia: String;   // instancia elegida: "." (por defecto) o ".\NOMBRE"

procedure AgregarInstancias(Raiz: Integer; var Servidores: TArrayOfString);
var
  Nombres: TArrayOfString;
  I, J: Integer;
  Servidor: String;
  Repetido: Boolean;
begin
  if not RegGetValueNames(Raiz, CLAVE_INSTANCIAS, Nombres) then
    Exit;
  for I := 0 to GetArrayLength(Nombres) - 1 do
  begin
    if CompareText(Nombres[I], INSTANCIA_POR_DEFECTO) = 0 then
      Servidor := '.'
    else
      Servidor := '.\' + Nombres[I];
    Repetido := False;
    for J := 0 to GetArrayLength(Servidores) - 1 do
      if CompareText(Servidores[J], Servidor) = 0 then
        Repetido := True;
    if not Repetido then
    begin
      SetArrayLength(Servidores, GetArrayLength(Servidores) + 1);
      Servidores[GetArrayLength(Servidores) - 1] := Servidor;
    end;
  end;
end;

// Instancias instaladas según el registro (vistas de 64 y 32 bits)
function InstanciasInstaladas(): TArrayOfString;
begin
  SetArrayLength(Result, 0);
  AgregarInstancias(HKLM64, Result);
  AgregarInstancias(HKLM32, Result);
end;

// SQL_CON_BASE / SQL_SIN_BASE si responde; otro valor si no responde. False si sqlcmd no se pudo iniciar.
function ProbarInstancia(const Servidor: String; var Codigo: Integer): Boolean;
begin
  Result := Exec('sqlcmd.exe', '-S "' + Servidor + '" -E -l 3 -d master -Q "EXIT(SELECT CASE WHEN DB_ID(''{#SqlBase}'') ' +
                 'IS NULL THEN ' + IntToStr(SQL_SIN_BASE) + ' ELSE ' + IntToStr(SQL_CON_BASE) + ' END)"',
                 '', SW_HIDE, ewWaitUntilTerminated, Codigo);
end;

// Prefiere la instancia que ya tiene la base; si ninguna la tiene, la primera que responde.
// Devuelve '' si ninguna responde. SinSqlcmd = True si sqlcmd no está instalado.
function DetectarInstancia(var BaseExiste: Boolean; var SinSqlcmd: Boolean): String;
var
  Servidores: TArrayOfString;
  I, Codigo: Integer;
begin
  Result := '';
  BaseExiste := False;
  SinSqlcmd := False;
  Servidores := InstanciasInstaladas();
  for I := 0 to GetArrayLength(Servidores) - 1 do
  begin
    WizardForm.StatusLabel.Caption := 'Buscando SQL Server (' + Servidores[I] + ')...';
    if not ProbarInstancia(Servidores[I], Codigo) then
    begin
      SinSqlcmd := True;
      Exit;
    end;
    if Codigo = SQL_CON_BASE then
    begin
      Result := Servidores[I];
      BaseExiste := True;
      Exit;
    end;
    if (Codigo = SQL_SIN_BASE) and (Result = '') then
      Result := Servidores[I];
  end;
end;

// ---------- Ejecución de los scripts ----------

// -f 65001: los scripts están en UTF-8 sin BOM (sin esto se rompen las tildes de las traducciones)
// -I: QUOTED_IDENTIFIER ON (índice filtrado de Contrato)
// Variables: parámetros -v de sqlcmd para el script (vacío si no usa)
function EjecutarScript(const Script, Variables: String): Boolean;
var
  Carpeta: String;
  Codigo: Integer;
begin
  Carpeta := ExpandConstant('{app}\BaseDeDatos\');
  Result := Exec('sqlcmd.exe', '-S "' + Instancia + '" -E -b -l 15 -I -f 65001 ' + Variables + ' -i "' + Carpeta + Script +
                 '.sql" -o "' + Carpeta + Script + '.log"', '', SW_HIDE, ewWaitUntilTerminated, Codigo)
            and (Codigo = 0);
  if not Result then
    SuppressibleMsgBox('Falló la ejecución de ' + Script + '.sql en ' + Instancia + ' (código ' + IntToStr(Codigo) + ').' + #13#10 +
      'Revise el detalle en ' + Carpeta + Script + '.log', mbError, MB_OK, IDOK);
end;

procedure MostrarPasosManuales(const Motivo: String);
var
  Carpeta: String;
begin
  Carpeta := ExpandConstant('{app}\BaseDeDatos\');
  SuppressibleMsgBox(Motivo + #13#10#13#10 +
    'Los archivos de {#AppNombre} se instalaron, pero falta crear la base de datos. ' +
    'Con SQL Server disponible, ejecute en una consola (reemplazando INSTANCIA, p. ej. .\SQLEXPRESS):' + #13#10#13#10 +
    'sqlcmd -S INSTANCIA -E -b -I -f 65001 -i "' + Carpeta + 'schema-init.sql"' + #13#10 +
    'sqlcmd -S INSTANCIA -E -b -I -f 65001 -i "' + Carpeta + 'stored-procedures.sql"' + #13#10 +
    'sqlcmd -S INSTANCIA -E -b -I -f 65001 -v Usuario="DOMINIO\usuario" -i "' + Carpeta + 'permisos.sql"',
    mbInformation, MB_OK, IDOK);
end;

// Usuario de Windows que abrió el instalador (DOMINIO\usuario). El instalador corre elevado: si se elevó con
// otra cuenta (p. ej. un administrador en una PC de laboratorio), SYSTEM_USER sería esa cuenta y no la que
// usa la aplicación. Se obtiene con whoami ejecutado como el usuario original; '' si no se pudo.
function UsuarioOriginal(): String;
var
  Archivo: String;
  Contenido: AnsiString;
  Codigo: Integer;
begin
  Result := '';
  // C:\ProgramData: lo puede escribir el usuario original y leer el instalador elevado
  Archivo := ExpandConstant('{commonappdata}\LandSmart-usuario.tmp');
  DeleteFile(Archivo);
  if ExecAsOriginalUser(ExpandConstant('{cmd}'), '/C whoami > "' + Archivo + '"', '', SW_HIDE,
                        ewWaitUntilTerminated, Codigo) and (Codigo = 0) and LoadStringFromFile(Archivo, Contenido) then
    Result := Trim(String(Contenido));
  DeleteFile(Archivo);
end;

// schema-init.sql BORRA y recrea todas las tablas: en una instalación nueva se ejecuta directamente;
// si la base ya existe (reinstalación o actualización) solo se recrea si el usuario lo confirma.
// stored-procedures.sql se puede ejecutar más de una vez sin perder datos: se ejecuta siempre.
// permisos.sql da acceso (db_owner) al usuario de Windows que instaló y al grupo local de usuarios de la PC
// (cualquier usuario de Windows puede usar la aplicación); también idempotente.
procedure CrearBaseDeDatos();
var
  BaseExiste, SinSqlcmd, Recrear: Boolean;
  Usuario: String;
begin
  if GetArrayLength(InstanciasInstaladas()) = 0 then
  begin
    MostrarPasosManuales('No se encontró ninguna instancia de SQL Server instalada en esta PC.');
    Exit;
  end;

  Instancia := DetectarInstancia(BaseExiste, SinSqlcmd);
  if SinSqlcmd then
  begin
    MostrarPasosManuales('No se encontró sqlcmd (se instala con SQL Server).');
    Exit;
  end;
  if Instancia = '' then
  begin
    MostrarPasosManuales('Ninguna instancia de SQL Server respondió. Verifique que el servicio esté iniciado.');
    Exit;
  end;
  Log('Instancia de SQL Server elegida: ' + Instancia);

  Recrear := not BaseExiste;
  if not Recrear then
    Recrear := SuppressibleMsgBox('La base de datos {#SqlBase} ya existe en ' + Instancia + '.' + #13#10#13#10 +
      '¿Desea recrearla? Se BORRARÁN todos sus datos (usuarios, captaciones, contratos, pagos, etc.).' + #13#10#13#10 +
      'Elija "No" para conservar los datos: solo se actualizarán los stored procedures.',
      mbConfirmation, MB_YESNO or MB_DEFBUTTON2, IDNO) = IDYES;

  if Recrear then
  begin
    WizardForm.StatusLabel.Caption := 'Creando la base de datos {#SqlBase} en ' + Instancia + '...';
    if not EjecutarScript('schema-init', '') then
      Exit;
  end;

  WizardForm.StatusLabel.Caption := 'Creando los stored procedures en ' + Instancia + '...';
  EjecutarScript('stored-procedures', '');

  // Sin el usuario original, permisos.sql usa SYSTEM_USER (la cuenta con la que corre el instalador)
  Usuario := UsuarioOriginal();
  if Usuario = '' then
    Usuario := 'SYSTEM_USER';
  Log('Usuario de Windows para los permisos: ' + Usuario);
  WizardForm.StatusLabel.Caption := 'Otorgando permisos a ' + Usuario + ' y a los usuarios de la PC...';
  EjecutarScript('permisos', '-v Usuario="' + Usuario + '"');
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
    CrearBaseDeDatos();
end;
