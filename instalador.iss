; ====================================================
; Instalador de LandSmart (Inno Setup 6)
; Se genera con compilar-instalador.ps1 (compila la solución y DbInstaller, descarga LocalDB, compila y firma).
; A mano: compilar la solución en Release x64 y DbInstaller\DbInstaller.csproj en Release, y después
;   "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" instalador.iss
;
; Flujo:
;   1. Verifica .NET Framework 4.7.2.
;   2. Página "Base de datos": lista las instancias de SQL Server del registro (con su versión y si ya tienen la base),
;      LocalDB (o la opción de instalarlo, incluido en el instalador) y "otro servidor" escrito a mano.
;      Al continuar inicia el servicio si está detenido y prueba la conexión con permisos de administrador de SQL.
;   3. Antes de copiar archivos (PrepareToInstall) hace todo el trabajo de la base con DbInstaller.exe (sin sqlcmd):
;      vuelve a verificar el servicio, hace una copia previa si la base existe, ejecuta los scripts, otorga los permisos
;      y hace la verificación final conectándose como el usuario que va a usar la aplicación.
;      Si algo falla revierte la base (la borra si era nueva o restaura la copia previa) y no copia ningún archivo:
;      la PC queda como estaba y el log queda en Documentos (LandSmart_instalacion_fallida.log).
;   4. Copia la aplicación y escribe el servidor elegido en TpIngSoft.exe.config y BackupCLI.exe.config.
; Instalación desatendida: LandSmart-Setup-1.0.0.exe /VERYSILENT [/SERVIDOR=.\SQLEXPRESS]
; ====================================================

#define AppNombre "LandSmart"
#define AppVersion "1.0.0"
#define AppEditor "Maximiliano Vricciariello"
#define AppExe "TpIngSoft.exe"
; Base de datos: debe coincidir con Initial Catalog de TpIngSoft.exe.config y BackupCLI.exe.config
#define SqlBase "TpIngSoft"
; SQL Server Express LocalDB se incluye solo si compilar-instalador.ps1 lo descargó
#ifexist "Redist\SqlLocalDB.msi"
  #define ConLocalDb "True"
#else
  #define ConLocalDb "False"
#endif

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
; Wizard: bienvenida, destino, base de datos, tareas, listo para instalar, progreso y finalizado
DisableWelcomePage=no
DisableDirPage=no
DisableProgramGroupPage=yes
DisableReadyPage=no
WizardStyle=modern
; x64: instala en "Program Files" (no en "Program Files (x86)") y ve el registro de SQL Server de 64 bits
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
; Program Files, el servicio de SQL Server y la creación de la base requieren administrador
PrivilegesRequired=admin
; Si la aplicación está abierta, pide cerrarla antes de reemplazar los archivos
CloseApplications=yes
RestartApplications=no
OutputDir=Output
OutputBaseFilename={#AppNombre}-Setup-{#AppVersion}
Compression=lzma2
SolidCompression=yes
; Log de Inno Setup siempre activo (%TEMP%\Setup Log aaaa-mm-dd #nnn.txt); el de la base de datos queda en
; {app}\BaseDeDatos\instalacion.log
SetupLogging=yes

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "escritorio"; Description: "Crear un acceso directo en el escritorio"; Flags: unchecked

[Files]
; Aplicación: exe, config y dlls (sin .pdb de depuración ni .xml de documentación)
Source: "GUI\bin\x64\Release\*"; DestDir: "{app}"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion
; Herramienta de backup y restore (usa las mismas BE/BLL/DAL/SERVICIOS que la aplicación)
Source: "BackupCLI\bin\x64\Release\BackupCLI.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "BackupCLI\bin\x64\Release\BackupCLI.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "Credenciales_Iniciales.txt"; DestDir: "{app}"; Flags: ignoreversion
; Scripts y cliente SQL: se usan durante la instalación (extraídos a una carpeta temporal) y al desinstalar
Source: "schema-init.sql"; DestDir: "{app}\BaseDeDatos"; Flags: ignoreversion
Source: "stored-procedures.sql"; DestDir: "{app}\BaseDeDatos"; Flags: ignoreversion
Source: "permisos.sql"; DestDir: "{app}\BaseDeDatos"; Flags: ignoreversion
Source: "DbInstaller\bin\Release\DbInstaller.exe"; DestDir: "{app}\BaseDeDatos"; Flags: ignoreversion
#if ConLocalDb == "True"
; SQL Server 2022 Express LocalDB: solo se extrae si no hay ningún SQL Server y se elige instalarlo
Source: "Redist\SqlLocalDB.msi"; DestDir: "{tmp}"; Flags: dontcopy
#endif

[Dirs]
; Carpeta alternativa de backups de BackupCLI: los usuarios sin privilegios de administrador también la usan
Name: "{commonappdata}\TpIngSoft_Backups"; Permissions: users-modify

[Icons]
Name: "{group}\{#AppNombre}"; Filename: "{app}\{#AppExe}"
Name: "{group}\Credenciales iniciales"; Filename: "{app}\Credenciales_Iniciales.txt"
Name: "{group}\Desinstalar {#AppNombre}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#AppNombre}"; Filename: "{app}\{#AppExe}"; Tasks: escritorio

[Run]
Filename: "{app}\Credenciales_Iniciales.txt"; Description: "Ver las credenciales iniciales"; Flags: postinstall shellexec skipifsilent unchecked runasoriginaluser
Filename: "{app}\{#AppExe}"; Description: "Abrir {#AppNombre}"; Flags: postinstall nowait skipifsilent runasoriginaluser

[UninstallDelete]
Type: files; Name: "{app}\BaseDeDatos\*.log"
Type: files; Name: "{app}\BaseDeDatos\instalacion.ini"

[Code]
const
  NET472_RELEASE = 461808;   // valor "Release" del registro para .NET Framework 4.7.2
  CON_LOCALDB = {#ConLocalDb};
  CLAVE_SQL = 'SOFTWARE\Microsoft\Microsoft SQL Server';
  CLAVE_INSTANCIAS = 'SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL';
  CLAVE_LOCALDB = 'SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions';
  INSTANCIA_POR_DEFECTO = 'MSSQLSERVER';
  SERVIDOR_LOCALDB = '(localdb)\MSSQLLocalDB';
  DESCARGA_SQL_EXPRESS = 'https://www.microsoft.com/sql-server/sql-server-downloads';
  // Tipos de opción de la página Base de datos
  OP_INSTANCIA = 0;
  OP_LOCALDB = 1;
  OP_INSTALAR_LOCALDB = 2;
  OP_MANUAL = 3;

var
  PaginaBase: TInputOptionWizardPage;
  EditServidor: TNewEdit;
  TextoEstado: TNewStaticText;
  OpcionTipo: array of Integer;
  OpcionServidor: TArrayOfString;
  OpcionTexto: TArrayOfString;
  CantidadInstancias: Integer;
  Explorado: Boolean;        // ya se probaron las instancias
  Elegido: Boolean;          // la instancia ya se eligió y verificó en la página
  Servidor: String;          // servidor elegido (Data Source)
  EsLocalDb: Boolean;        // LocalDB: es de cada usuario, se trabaja con el usuario que abrió el instalador
  InstalarLocalDb: Boolean;  // hay que instalar LocalDB antes de crear la base
  CarpetaTrabajo: String;    // {commonappdata}\LandSmart\Instalacion: DbInstaller, scripts y log
  ArchivoLog: String;
  RutaBackup: String;        // copia previa de la base existente (para revertir)

// ---------- .NET Framework ----------

// .NET Framework 4.7.2 o posterior (Windows 10 1803+ ya lo trae)
function InitializeSetup(): Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release)
            and (Release >= NET472_RELEASE);
  if not Result then
    SuppressibleMsgBox('{#AppNombre} requiere .NET Framework 4.7.2 o posterior.' + #13#10 +
      'Descárguelo de https://dotnet.microsoft.com/download/dotnet-framework/net472 y vuelva a ejecutar el instalador.',
      mbCriticalError, MB_OK, IDOK);
end;

// ---------- Log y DbInstaller ----------

procedure RegistrarLog(const Mensaje: String);
begin
  Log(Mensaje);
  if ArchivoLog <> '' then
    SaveStringToFile(ArchivoLog, GetDateTimeString('yyyy-mm-dd hh:nn:ss', '-', ':') + ' [Instalador] ' + Mensaje + #13#10, True);
end;

function LeerArchivo(const Archivo: String): String;
var
  Contenido: AnsiString;
begin
  Result := '';
  if LoadStringFromFile(Archivo, Contenido) then
    Result := Trim(String(Contenido));
end;

// Final del log (donde está el error), para mostrarlo en los mensajes
function FinalDelLog(): String;
begin
  Result := LeerArchivo(ArchivoLog);
  if Length(Result) > 900 then
    Result := '...' + Copy(Result, Length(Result) - 900, 901);
end;

// Carpeta de trabajo en C:\ProgramData: la puede leer y escribir también el usuario que abrió el instalador
// (la verificación final y LocalDB se ejecutan con su cuenta, no con la del administrador)
procedure PrepararTrabajo();
var
  Codigo: Integer;
begin
  if CarpetaTrabajo <> '' then
    Exit;
  CarpetaTrabajo := ExpandConstant('{commonappdata}\{#AppNombre}\Instalacion');
  ForceDirectories(CarpetaTrabajo);
  Exec(ExpandConstant('{sys}\icacls.exe'), '"' + CarpetaTrabajo + '" /grant *S-1-5-32-545:(OI)(CI)M',
       '', SW_HIDE, ewWaitUntilTerminated, Codigo);
  ArchivoLog := CarpetaTrabajo + '\instalacion.log';
  DeleteFile(ArchivoLog);
  ExtractTemporaryFile('DbInstaller.exe');
  ExtractTemporaryFile('schema-init.sql');
  ExtractTemporaryFile('stored-procedures.sql');
  ExtractTemporaryFile('permisos.sql');
  CopyFile(ExpandConstant('{tmp}\DbInstaller.exe'), CarpetaTrabajo + '\DbInstaller.exe', False);
  CopyFile(ExpandConstant('{tmp}\schema-init.sql'), CarpetaTrabajo + '\schema-init.sql', False);
  CopyFile(ExpandConstant('{tmp}\stored-procedures.sql'), CarpetaTrabajo + '\stored-procedures.sql', False);
  CopyFile(ExpandConstant('{tmp}\permisos.sql'), CarpetaTrabajo + '\permisos.sql', False);
  RegistrarLog('==== {#AppNombre} {#AppVersion} ====');
  RegistrarLog('Cuenta que ejecuta el instalador (elevada): ' + GetUserNameString());
end;

// Ejecuta DbInstaller.exe; ComoUsuario = con la cuenta que abrió el instalador (la que usa la aplicación)
function Db(const Argumentos: String; ComoUsuario: Boolean): Boolean;
var
  Exe, Linea: String;
  Codigo: Integer;
begin
  Exe := CarpetaTrabajo + '\DbInstaller.exe';
  Linea := Argumentos + ' --log "' + ArchivoLog + '"';
  if ComoUsuario then
    Result := ExecAsOriginalUser(Exe, Linea, CarpetaTrabajo, SW_HIDE, ewWaitUntilTerminated, Codigo)
  else
    Result := Exec(Exe, Linea, CarpetaTrabajo, SW_HIDE, ewWaitUntilTerminated, Codigo);
  if not Result then
    RegistrarLog('No se pudo ejecutar DbInstaller.exe: ' + SysErrorMessage(Codigo));
  Result := Result and (Codigo = 0);
end;

function Comillas(const Texto: String): String;
begin
  Result := '"' + Texto + '"';
end;

// ---------- Detección de instancias ----------

function NombreVersion(const Version: String): String;
var
  P: Integer;
  Mayor: String;
begin
  P := Pos('.', Version);
  if P > 0 then
    Mayor := Copy(Version, 1, P - 1)
  else
    Mayor := Version;
  case StrToIntDef(Mayor, 0) of
    17: Result := 'SQL Server 2025';
    16: Result := 'SQL Server 2022';
    15: Result := 'SQL Server 2019';
    14: Result := 'SQL Server 2017';
    13: Result := 'SQL Server 2016';
    12: Result := 'SQL Server 2014';
    11: Result := 'SQL Server 2012';
  else
    Result := 'SQL Server ' + Version;
  end;
end;

function IndiceDe(const Srv: String): Integer;
var
  I: Integer;
begin
  Result := -1;
  for I := 0 to GetArrayLength(OpcionServidor) - 1 do
    if (Result < 0) and (OpcionServidor[I] <> '') and (CompareText(OpcionServidor[I], Srv) = 0) then
      Result := I;
end;

procedure AgregarOpcion(Tipo: Integer; const Srv, Texto: String);
var
  N: Integer;
begin
  N := GetArrayLength(OpcionServidor);
  SetArrayLength(OpcionTipo, N + 1);
  SetArrayLength(OpcionServidor, N + 1);
  SetArrayLength(OpcionTexto, N + 1);
  OpcionTipo[N] := Tipo;
  OpcionServidor[N] := Srv;
  OpcionTexto[N] := Texto;
end;

// Instancias del registro: "." para la predeterminada (MSSQLSERVER), ".\NOMBRE" para las con nombre
procedure AgregarInstancias(Raiz: Integer);
var
  Nombres: TArrayOfString;
  I: Integer;
  Srv, Id, Version, Edicion, Texto: String;
begin
  if not RegGetValueNames(Raiz, CLAVE_INSTANCIAS, Nombres) then
    Exit;
  for I := 0 to GetArrayLength(Nombres) - 1 do
  begin
    if CompareText(Nombres[I], INSTANCIA_POR_DEFECTO) = 0 then
      Srv := '.'
    else
      Srv := '.\' + Nombres[I];
    if IndiceDe(Srv) < 0 then
    begin
      if Srv = '.' then
        Texto := '. (instancia predeterminada)'
      else
        Texto := Srv;
      Version := '';
      Edicion := '';
      if RegQueryStringValue(Raiz, CLAVE_INSTANCIAS, Nombres[I], Id) then
      begin
        RegQueryStringValue(Raiz, CLAVE_SQL + '\' + Id + '\Setup', 'Version', Version);
        RegQueryStringValue(Raiz, CLAVE_SQL + '\' + Id + '\Setup', 'Edition', Edicion);
      end;
      if Version <> '' then
        Texto := Texto + ' - ' + NombreVersion(Version);
      if Edicion <> '' then
        Texto := Texto + ' ' + Edicion;
      AgregarOpcion(OP_INSTANCIA, Srv, Texto);
    end;
  end;
end;

function LocalDbInstalado(): Boolean;
var
  Versiones: TArrayOfString;
begin
  Result := RegGetSubkeyNames(HKLM64, CLAVE_LOCALDB, Versiones) and (GetArrayLength(Versiones) > 0);
end;

function IndiceManual(): Integer;
begin
  Result := GetArrayLength(OpcionTipo) - 1;
end;

// ---------- Página Base de datos ----------

procedure EditServidorCambio(Sender: TObject);
begin
  if Trim(EditServidor.Text) <> '' then
    PaginaBase.SelectedValueIndex := IndiceManual();
end;

procedure InitializeWizard();
var
  I: Integer;
  Subtitulo: String;
begin
  AgregarInstancias(HKLM64);
  AgregarInstancias(HKLM32);
  CantidadInstancias := GetArrayLength(OpcionServidor);
  if LocalDbInstalado() then
    AgregarOpcion(OP_LOCALDB, SERVIDOR_LOCALDB, SERVIDOR_LOCALDB + ' - SQL Server Express LocalDB (base del usuario que instala)')
  else if CON_LOCALDB then
    AgregarOpcion(OP_INSTALAR_LOCALDB, SERVIDOR_LOCALDB, 'Instalar SQL Server Express LocalDB (incluido en este instalador)');
  AgregarOpcion(OP_MANUAL, '', 'Otro servidor (con otro nombre o en otra PC): escríbalo abajo');

  if CantidadInstancias = 0 then
    Subtitulo := 'No se encontró SQL Server en este equipo. Puede instalar SQL Server Express LocalDB, indicar un ' +
                 'servidor existente o cancelar e instalar SQL Server Express (' + DESCARGA_SQL_EXPRESS + ').'
  else
    Subtitulo := 'Elija la instancia donde se creará la base {#SqlBase}. Si una instancia ya tiene la base, se ' +
                 'conservan sus datos (se pregunta antes de reemplazarla).';
  PaginaBase := CreateInputOptionPage(wpSelectDir, 'Base de datos', 'Instancia de SQL Server para la base {#SqlBase}',
    Subtitulo, True, False);
  for I := 0 to GetArrayLength(OpcionTexto) - 1 do
    PaginaBase.Add(OpcionTexto[I]);
  PaginaBase.CheckListBox.Height := ScaleY(125);

  EditServidor := TNewEdit.Create(PaginaBase);
  EditServidor.Parent := PaginaBase.Surface;
  EditServidor.Left := 0;
  EditServidor.Top := PaginaBase.CheckListBox.Top + PaginaBase.CheckListBox.Height + ScaleY(6);
  EditServidor.Width := PaginaBase.SurfaceWidth;
  EditServidor.OnChange := @EditServidorCambio;

  TextoEstado := TNewStaticText.Create(PaginaBase);
  TextoEstado.Parent := PaginaBase.Surface;
  TextoEstado.Left := 0;
  TextoEstado.Top := EditServidor.Top + EditServidor.Height + ScaleY(8);
  TextoEstado.Width := PaginaBase.SurfaceWidth;
  TextoEstado.WordWrap := True;
  TextoEstado.Caption := '';
end;

// Selecciona la opción de un servidor; si no está en la lista lo escribe como "otro servidor"
function SeleccionarServidor(const Srv: String): Integer;
begin
  Result := IndiceDe(Srv);
  if Result < 0 then
  begin
    EditServidor.Text := Srv;
    Result := IndiceManual();
  end;
end;

// Prueba todas las instancias en paralelo, muestra su estado y preselecciona: /SERVIDOR, la de la instalación
// anterior, la que tiene la base, la primera que responde o LocalDB
procedure ExplorarInstancias();
var
  Lista, Archivo, Srv, Estado, Sufijo, Pedido, Previo: String;
  Lineas: TArrayOfString;
  I, J, P, ConBase, Responde, Elegir: Integer;
begin
  Explorado := True;
  PrepararTrabajo();
  ConBase := -1;
  Responde := -1;
  Lista := '';
  for I := 0 to GetArrayLength(OpcionTipo) - 1 do
    if OpcionTipo[I] = OP_INSTANCIA then
      Lista := Lista + OpcionServidor[I] + '|';

  if Lista <> '' then
  begin
    TextoEstado.Caption := 'Buscando las instancias de SQL Server y la base {#SqlBase} (puede tardar unos segundos)...';
    WizardForm.NextButton.Enabled := False;
    Archivo := CarpetaTrabajo + '\instancias.txt';
    Db('probe --servers ' + Comillas(Lista) + ' --db {#SqlBase} --out ' + Comillas(Archivo), False);
    WizardForm.NextButton.Enabled := True;
    if LoadStringsFromFile(Archivo, Lineas) then
      for J := 0 to GetArrayLength(Lineas) - 1 do
      begin
        P := Pos(#9, Lineas[J]);
        if P > 0 then
        begin
          Srv := Copy(Lineas[J], 1, P - 1);
          Estado := Copy(Lineas[J], P + 1, Length(Lineas[J]));
          I := IndiceDe(Srv);
          if I >= 0 then
          begin
            if Estado = 'CON_BASE' then
            begin
              Sufijo := '   [tiene la base {#SqlBase}]';
              if ConBase < 0 then ConBase := I;
            end
            else if Estado = 'SIN_BASE' then
              Sufijo := '   [disponible]'
            else if Estado = 'DETENIDO' then
              Sufijo := '   [servicio detenido: se iniciará]'
            else
              Sufijo := '   [no responde]';
            if (Responde < 0) and ((Estado = 'CON_BASE') or (Estado = 'SIN_BASE')) then
              Responde := I;
            PaginaBase.CheckListBox.ItemCaption[I] := OpcionTexto[I] + Sufijo;
          end;
        end;
      end;
  end;

  Pedido := Trim(ExpandConstant('{param:SERVIDOR|}'));
  Previo := GetPreviousData('Servidor', '');
  Elegir := -1;
  if Pedido <> '' then
    Elegir := SeleccionarServidor(Pedido)
  else if Previo <> '' then
    Elegir := SeleccionarServidor(Previo);
  if Elegir < 0 then
  begin
    if ConBase >= 0 then
      Elegir := ConBase
    else if Responde >= 0 then
      Elegir := Responde
    else
      Elegir := CantidadInstancias;  // LocalDB (o instalarlo); si no hay, "otro servidor"
  end;
  PaginaBase.SelectedValueIndex := Elegir;

  if CantidadInstancias = 0 then
    TextoEstado.Caption := ''
  else if ConBase >= 0 then
    TextoEstado.Caption := 'La base {#SqlBase} ya existe en ' + OpcionServidor[ConBase] + '.'
  else
    TextoEstado.Caption := 'Ninguna instancia tiene todavía la base {#SqlBase}: se creará en la que elija.';
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if (CurPageID = PaginaBase.ID) and not Explorado then
    ExplorarInstancias();
end;

// Toma la opción seleccionada en la página
procedure AsignarEleccion();
var
  Tipo: Integer;
begin
  Tipo := OpcionTipo[PaginaBase.SelectedValueIndex];
  Servidor := OpcionServidor[PaginaBase.SelectedValueIndex];
  if Tipo = OP_MANUAL then
    Servidor := Trim(EditServidor.Text);
  EsLocalDb := Pos('(localdb)', Lowercase(Servidor)) = 1;
  // Si el instalador no incluye LocalDB, nunca se instala (solo se puede usar uno ya instalado)
#if ConLocalDb == "True"
  InstalarLocalDb := (Tipo = OP_INSTALAR_LOCALDB) or (EsLocalDb and not LocalDbInstalado());
#else
  InstalarLocalDb := False;
#endif
end;

// Servicio iniciado (lo inicia si está detenido) y conexión con permisos para crear la base.
// LocalDB no tiene servicio: se inicia con la cuenta del usuario, que es la dueña de su instancia.
function PrepararServidor(var Error: String): Boolean;
begin
  Result := False;
  if EsLocalDb then
  begin
    if not Db('localdb-start', True) then
    begin
      Error := 'No se pudo iniciar SQL Server Express LocalDB.';
      Exit;
    end;
  end
  else if not Db('check-service --server ' + Comillas(Servidor), False) then
  begin
    Error := 'El servicio de SQL Server de ' + Servidor + ' no está iniciado y no se pudo iniciar.';
    Exit;
  end;
  if not Db('test-connection --server ' + Comillas(Servidor) + ' --require-admin', EsLocalDb) then
  begin
    Error := 'No se puede usar ' + Servidor + ' para crear la base de datos.';
    Exit;
  end;
  Result := True;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  Error: String;
begin
  Result := True;
  // En modo silencioso no hay a quién mostrarle mensajes: se elige y verifica en PrepareToInstall
  if (CurPageID <> PaginaBase.ID) or WizardSilent() then
    Exit;
  AsignarEleccion();
  if Servidor = '' then
  begin
    MsgBox('Escriba el nombre del servidor de SQL Server (por ejemplo PC-SERVIDOR\SQLEXPRESS).', mbError, MB_OK);
    Result := False;
    Exit;
  end;
  RegistrarLog('Instancia elegida en el asistente: ' + Servidor);
  Elegido := InstalarLocalDb;  // LocalDB se instala recién en PrepareToInstall
  if not Elegido then
  begin
    WizardForm.NextButton.Enabled := False;
    TextoEstado.Caption := 'Verificando ' + Servidor + '...';
    Elegido := PrepararServidor(Error);
    WizardForm.NextButton.Enabled := True;
    TextoEstado.Caption := '';
    if not Elegido then
      MsgBox(Error + #13#10#13#10 + FinalDelLog() + #13#10#13#10 + 'Elija otra instancia o corrija el nombre del servidor.',
             mbError, MB_OK);
  end;
  Result := Elegido;
end;

// ---------- Instalación de LocalDB ----------

function InstalarLocalDbMsi(var Error: String): Boolean;
var
  Codigo: Integer;
  LogMsi: String;
begin
  RegistrarLog('Instalando SQL Server Express LocalDB...');
  WizardForm.PageDescriptionLabel.Caption := 'Instalando SQL Server Express LocalDB...';
  ExtractTemporaryFile('SqlLocalDB.msi');
  LogMsi := CarpetaTrabajo + '\localdb-msi.log';
  Result := Exec(ExpandConstant('{sys}\msiexec.exe'), '/i ' + Comillas(ExpandConstant('{tmp}\SqlLocalDB.msi')) +
                 ' /qn /norestart IACCEPTSQLLOCALDBLICENSETERMS=YES /l*v ' + Comillas(LogMsi),
                 '', SW_HIDE, ewWaitUntilTerminated, Codigo)
            and ((Codigo = 0) or (Codigo = 3010));
  if Result then
    RegistrarLog('SQL Server Express LocalDB instalado (código ' + IntToStr(Codigo) + ').')
  else
    Error := 'No se pudo instalar SQL Server Express LocalDB (código ' + IntToStr(Codigo) + '). Log de Windows Installer: Documentos\{#AppNombre}_localdb-msi.log';
end;

// ---------- Base de datos (antes de copiar archivos) ----------

// Usuario de Windows que abrió el instalador (DOMINIO\usuario). El instalador corre elevado: si se elevó con
// otra cuenta (p. ej. un administrador en una PC de laboratorio), SYSTEM_USER sería esa cuenta y no la que
// usa la aplicación. Se obtiene con whoami ejecutado como el usuario original; '' si no se pudo.
function UsuarioOriginal(): String;
var
  Archivo: String;
  Codigo: Integer;
begin
  Result := '';
  Archivo := CarpetaTrabajo + '\usuario.txt';
  DeleteFile(Archivo);
  if ExecAsOriginalUser(ExpandConstant('{cmd}'), '/C whoami > ' + Comillas(Archivo), '', SW_HIDE,
                        ewWaitUntilTerminated, Codigo) and (Codigo = 0) then
    Result := LeerArchivo(Archivo);
  DeleteFile(Archivo);
end;

function EjecutarScript(const Script, Variables: String): Boolean;
begin
  WizardForm.PageDescriptionLabel.Caption := 'Ejecutando ' + Script + '.sql en ' + Servidor + '...';
  Result := Db('run-script --server ' + Comillas(Servidor) + ' --file ' + Comillas(CarpetaTrabajo + '\' + Script + '.sql') +
               ' ' + Variables, EsLocalDb);
end;

// Rollback de la base: la borra si la creó esta instalación o restaura la copia previa
procedure RevertirBase(BaseExistia: Boolean);
begin
  if BaseExistia then
  begin
    RegistrarLog('Revirtiendo: se restaura la copia previa ' + RutaBackup);
    if not Db('restore --server ' + Comillas(Servidor) + ' --db {#SqlBase} --file ' + Comillas(RutaBackup), EsLocalDb) then
      RegistrarLog('ATENCIÓN: no se pudo restaurar la copia previa. Restáurela con BackupCLI o SSMS desde ' + RutaBackup);
  end
  else
  begin
    RegistrarLog('Revirtiendo: se borra la base creada por esta instalación.');
    Db('drop-database --server ' + Comillas(Servidor) + ' --db {#SqlBase}', EsLocalDb);
  end;
end;

// Mensaje de error final: no se copió ningún archivo; el log se guarda en Documentos
function Fallo(const Mensaje: String): String;
var
  Destino: String;
begin
  RegistrarLog('INSTALACIÓN CANCELADA: ' + Mensaje);
  Destino := ExpandConstant('{userdocs}\{#AppNombre}_instalacion_fallida.log');
  CopyFile(ArchivoLog, Destino, False);
  if FileExists(CarpetaTrabajo + '\localdb-msi.log') then
    CopyFile(CarpetaTrabajo + '\localdb-msi.log', ExpandConstant('{userdocs}\{#AppNombre}_localdb-msi.log'), False);
  Result :=Mensaje + #13#10#13#10 + 'No se instaló ningún archivo y la base de datos quedó como estaba.' + #13#10#13#10 +
            FinalDelLog() + #13#10#13#10 + 'Log completo: ' + Destino;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  Error, Usuario, Archivo: String;
  Lineas: TArrayOfString;
  BaseExiste, Recrear: Boolean;
begin
  Result := '';
  PrepararTrabajo();

  // Instalación desatendida (o sin pasar por la página): se elige igual que la preselección de la página
  if not Elegido then
  begin
    if not Explorado then
      ExplorarInstancias();
    AsignarEleccion();
    if Servidor = '' then
    begin
      Result := Fallo('No se encontró SQL Server en este equipo. Instale SQL Server Express (' + DESCARGA_SQL_EXPRESS +
                      ') o indique el servidor con /SERVIDOR=NOMBRE.');
      Exit;
    end;
  end;
  RegistrarLog('Instancia: ' + Servidor);

  if InstalarLocalDb then
    if not InstalarLocalDbMsi(Error) then
    begin
      Result := Fallo(Error);
      Exit;
    end;

  // Segunda verificación del servicio y de la conexión, justo antes de tocar la base
  WizardForm.PageDescriptionLabel.Caption := 'Verificando SQL Server (' + Servidor + ')...';
  if not PrepararServidor(Error) then
  begin
    Result := Fallo(Error);
    Exit;
  end;

  Archivo := CarpetaTrabajo + '\elegida.txt';
  if not Db('probe --servers ' + Comillas(Servidor) + ' --db {#SqlBase} --out ' + Comillas(Archivo), EsLocalDb)
     or not LoadStringsFromFile(Archivo, Lineas) or (GetArrayLength(Lineas) = 0) then
  begin
    Result := Fallo('No se pudo consultar ' + Servidor + '.');
    Exit;
  end;
  BaseExiste := Pos(#9 + 'CON_BASE', Lineas[0]) > 0;

  Recrear := not BaseExiste;
  if BaseExiste then
  begin
    Recrear := SuppressibleMsgBox('La base de datos {#SqlBase} ya existe en ' + Servidor + '.' + #13#10#13#10 +
      '¿Desea recrearla? Se BORRARÁN todos sus datos (usuarios, captaciones, contratos, pagos, etc.).' + #13#10#13#10 +
      'Elija "No" para conservar los datos: solo se actualizarán los stored procedures y los permisos.',
      mbConfirmation, MB_YESNO or MB_DEFBUTTON2, IDNO) = IDYES;
    if Recrear then
      RegistrarLog('Se eligió recrear la base existente.')
    else
      RegistrarLog('Se eligió conservar la base existente.');

    // Copia previa: sin ella no se puede revertir, así que no se sigue
    WizardForm.PageDescriptionLabel.Caption := 'Copia de seguridad previa de {#SqlBase}...';
    if not Db('backup --server ' + Comillas(Servidor) + ' --db {#SqlBase} --out ' + Comillas(CarpetaTrabajo + '\backup.txt'), EsLocalDb) then
    begin
      Result := Fallo('No se pudo hacer la copia de seguridad previa de la base existente.');
      Exit;
    end;
    RutaBackup := LeerArchivo(CarpetaTrabajo + '\backup.txt');
  end;

  if Recrear and not EjecutarScript('schema-init', '') then
  begin
    RevertirBase(BaseExiste);
    Result := Fallo('Falló la creación de las tablas (schema-init.sql).');
    Exit;
  end;
  if not EjecutarScript('stored-procedures', '') then
  begin
    RevertirBase(BaseExiste);
    Result := Fallo('Falló la creación de los stored procedures (stored-procedures.sql).');
    Exit;
  end;

  // LocalDB es del usuario que instala (ya es el dueño); en una instancia compartida se da acceso a los usuarios de la PC
  if not EsLocalDb then
  begin
    Usuario := UsuarioOriginal();
    if Usuario = '' then
      Usuario := 'SYSTEM_USER';
    RegistrarLog('Usuario de Windows que abrió el instalador: ' + Usuario);
    if not EjecutarScript('permisos', '--var ' + Comillas('Usuario=' + Usuario)) then
    begin
      RevertirBase(BaseExiste);
      Result := Fallo('No se pudieron otorgar los permisos a los usuarios de la PC (permisos.sql).');
      Exit;
    end;
  end;

  // Verificación final: con la cuenta que va a usar la aplicación, igual que ella se conecta
  WizardForm.PageDescriptionLabel.Caption := 'Verificación final de la base de datos...';
  if not Db('verify --server ' + Comillas(Servidor) + ' --db {#SqlBase}', True) then
  begin
    RevertirBase(BaseExiste);
    Result := Fallo('La verificación final falló: la aplicación no podría usar la base de datos.');
    Exit;
  end;
  RegistrarLog('Base de datos lista.');
end;

// ---------- Después de copiar los archivos ----------

// Servicio de Windows de una instancia local ('' para LocalDB o servidores remotos)
function ServicioDe(const Srv: String): String;
begin
  Result := '';
  if Srv = '.' then
    Result := INSTANCIA_POR_DEFECTO
  else if Pos('.\', Srv) = 1 then
    Result := 'MSSQL$' + Copy(Srv, 3, Length(Srv));
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  Ini, Servicio: String;
  Codigo: Integer;
begin
  if CurStep <> ssPostInstall then
    Exit;
  // La aplicación prueba primero este servidor (si la base se mueve, igual busca en las demás instancias)
  if not Db('set-config --file ' + Comillas(ExpandConstant('{app}\{#AppExe}.config')) + ' --server ' + Comillas(Servidor), False) then
    RegistrarLog('AVISO: no se pudo escribir el servidor en {#AppExe}.config (la aplicación lo detectará sola).');
  if not Db('set-config --file ' + Comillas(ExpandConstant('{app}\BackupCLI.exe.config')) + ' --server ' + Comillas(Servidor), False) then
    RegistrarLog('AVISO: no se pudo escribir el servidor en BackupCLI.exe.config.');

  // Para la desinstalación
  Ini := ExpandConstant('{app}\BaseDeDatos\instalacion.ini');
  SetIniString('BaseDeDatos', 'Servidor', Servidor, Ini);
  if EsLocalDb then
    SetIniString('BaseDeDatos', 'LocalDB', '1', Ini)
  else
    SetIniString('BaseDeDatos', 'LocalDB', '0', Ini);
  if RutaBackup <> '' then
    SetIniString('BaseDeDatos', 'CopiaPrevia', RutaBackup, Ini);

  // El servicio de SQL Server escribe los .bak de BackupCLI en la carpeta alternativa
  Servicio := ServicioDe(Servidor);
  if Servicio <> '' then
    Exec(ExpandConstant('{sys}\icacls.exe'), Comillas(ExpandConstant('{commonappdata}\TpIngSoft_Backups')) +
         ' /grant ' + Comillas('NT SERVICE\' + Servicio + ':(OI)(CI)M'), '', SW_HIDE, ewWaitUntilTerminated, Codigo);

  RegistrarLog('Instalación terminada en ' + ExpandConstant('{app}') + '.');
  CopyFile(ArchivoLog, ExpandConstant('{app}\BaseDeDatos\instalacion.log'), False);
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
begin
  SetPreviousData(PreviousDataKey, 'Servidor', Servidor);
end;

// Borra la carpeta de trabajo (el log ya quedó en {app}\BaseDeDatos o en Documentos)
procedure DeinitializeSetup();
begin
  if CarpetaTrabajo <> '' then
    DelTree(CarpetaTrabajo, True, True, True);
end;

// ---------- Desinstalación ----------

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  Ini, Srv, LogDesinstalacion: String;
  Codigo: Integer;
begin
  if CurUninstallStep <> usUninstall then
    Exit;
  Ini := ExpandConstant('{app}\BaseDeDatos\instalacion.ini');
  Srv := GetIniString('BaseDeDatos', 'Servidor', '', Ini);
  if Srv = '' then
    Exit;
  if SuppressibleMsgBox('¿Desea borrar también la base de datos {#SqlBase} de ' + Srv + '?' + #13#10#13#10 +
       'Se perderán todos los datos. Elija "No" para conservarla (por ejemplo, para reinstalar).',
       mbConfirmation, MB_YESNO or MB_DEFBUTTON2, IDNO) <> IDYES then
    Exit;
  LogDesinstalacion := ExpandConstant('{userdocs}\{#AppNombre}_desinstalacion.log');
  if not Exec(ExpandConstant('{app}\BaseDeDatos\DbInstaller.exe'),
              'drop-database --server ' + Comillas(Srv) + ' --db {#SqlBase} --log ' + Comillas(LogDesinstalacion),
              '', SW_HIDE, ewWaitUntilTerminated, Codigo) or (Codigo <> 0) then
    SuppressibleMsgBox('No se pudo borrar la base de datos {#SqlBase} de ' + Srv + '. Detalle: ' + LogDesinstalacion,
                       mbError, MB_OK, IDOK);
end;
