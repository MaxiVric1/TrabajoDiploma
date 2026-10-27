<#
.SYNOPSIS
    Genera Output\Instalar-LandSmart.exe: compila, descarga LocalDB, compila el instalador y lo firma.

.DESCRIPTION
    1. Compila la solución (Release x64) y DbInstaller (Release) con el MSBuild de Visual Studio.
    2. Descarga SQL Server 2022 Express LocalDB (Redist\SqlLocalDB.msi) si falta y verifica que esté firmado por Microsoft.
    3. Compila instalador.iss con Inno Setup 6.
    4. Firma el instalador con el certificado de firma de código "CN=LandSmart" del almacén del usuario. Si no existe,
       crea uno autofirmado (no exportable). En otra PC, SmartScreen puede avisar: "Más información" > "Ejecutar de
       todas formas".

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File .\compilar-instalador.ps1
#>
$ErrorActionPreference = 'Stop'
$raiz = $PSScriptRoot
$iss = Join-Path $raiz 'instalador.iss'

$version = ([regex]::Match((Get-Content $iss -Raw), '#define AppVersion "([^"]+)"')).Groups[1].Value
if (-not $version) { throw 'No se encontró AppVersion en instalador.iss.' }
$exe = Join-Path $raiz 'Output\Instalar-LandSmart.exe'

# 1. Compilación
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = if (Test-Path $vswhere) {
    & $vswhere -latest -prerelease -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
}
if (-not $msbuild) { throw 'No se encontró MSBuild (Visual Studio).' }

Write-Host 'Compilando la solución (Release x64)...' -ForegroundColor Cyan
& $msbuild (Join-Path $raiz 'TpIngSoft.sln') -t:Restore,Rebuild -p:Configuration=Release -p:Platform=x64 -v:minimal -nologo
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de la solución.' }
Write-Host 'Compilando DbInstaller (Release)...' -ForegroundColor Cyan
& $msbuild (Join-Path $raiz 'DbInstaller\DbInstaller.csproj') -t:Rebuild -p:Configuration=Release -v:minimal -nologo
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de DbInstaller.' }

# 2. SQL Server 2022 Express LocalDB (se incluye para PCs sin SQL Server)
$msi = Join-Path $raiz 'Redist\SqlLocalDB.msi'
if (-not (Test-Path $msi)) {
    Write-Host 'Descargando SQL Server 2022 Express LocalDB...' -ForegroundColor Cyan
    New-Item -ItemType Directory -Force (Split-Path $msi) | Out-Null
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest 'https://download.microsoft.com/download/3/8/d/38de7036-2433-4207-8eae-06e247e17b25/SqlLocalDB.msi' `
        -OutFile $msi -UseBasicParsing
}
$firmaMsi = Get-AuthenticodeSignature $msi
if ($firmaMsi.Status -ne 'Valid' -or $firmaMsi.SignerCertificate.Subject -notmatch 'O=Microsoft Corporation') {
    Remove-Item $msi
    throw "SqlLocalDB.msi no tiene una firma válida de Microsoft ($($firmaMsi.Status)): se borró, vuelva a ejecutar el script."
}

# 3. Inno Setup
$iscc = @("${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe", "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
          "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe") | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $iscc) { throw 'No se encontró Inno Setup 6 (ISCC.exe).' }
Write-Host 'Compilando el instalador...' -ForegroundColor Cyan
& $iscc $iss | Select-Object -Last 2
if ($LASTEXITCODE -ne 0 -or -not (Test-Path $exe)) { throw 'Falló la compilación del instalador.' }

# 4. Firma (certificado autofirmado del usuario; la clave privada no sale de este almacén)
$cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert |
    Where-Object { $_.Subject -like 'CN=LandSmart*' -and $_.HasPrivateKey -and $_.NotAfter -gt (Get-Date) } |
    Sort-Object NotAfter -Descending | Select-Object -First 1
if (-not $cert) {
    Write-Host 'Creando el certificado de firma de código autofirmado CN=LandSmart...' -ForegroundColor Cyan
    $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject 'CN=LandSmart, O=Maximiliano Vricciariello' `
        -CertStoreLocation Cert:\CurrentUser\My -KeyExportPolicy NonExportable -NotAfter (Get-Date).AddYears(3)
}
$firma = Set-AuthenticodeSignature -FilePath $exe -Certificate $cert -HashAlgorithm SHA256 `
    -TimestampServer 'http://timestamp.digicert.com' -ErrorAction SilentlyContinue
if (-not $firma -or -not $firma.SignerCertificate) {
    # Sin internet no hay sello de tiempo: se firma igual
    $firma = Set-AuthenticodeSignature -FilePath $exe -Certificate $cert -HashAlgorithm SHA256
}
if (-not $firma.SignerCertificate) { throw "Falló la firma: $($firma.StatusMessage)" }
# Autofirmado: el estado es UnknownError (la raíz no es de confianza), pero la firma queda aplicada
Write-Host ("Firmado por {0} | Estado: {1}" -f $firma.SignerCertificate.Subject, $firma.Status) -ForegroundColor Green
Write-Host "$exe (versión $version)" -ForegroundColor Green
