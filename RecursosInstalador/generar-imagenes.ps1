<#
.SYNOPSIS
    Genera las imágenes del instalador a partir de GUI\Resources\logo-landsmart.png.

.DESCRIPTION
    - landsmart.ico: ícono del instalador (16, 24, 32, 48, 64 y 256 px). Solo el símbolo del logo (el texto no se lee
      en tamaños chicos), sobre una baldosa blanca redondeada.
    - wizard-grande-<escala>.bmp: banner lateral de las páginas de bienvenida y final, en los tamaños que Inno Setup
      usa para 100/125/150/175/200 % de escala de Windows. Degradado violeta de LandSmart con el logo en una tarjeta blanca.
    - wizard-chico-<escala>.bmp: imagen de arriba a la derecha de las demás páginas (símbolo del logo sobre blanco).
    Solo hace falta volver a ejecutarlo si cambia el logo.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File .\RecursosInstalador\generar-imagenes.ps1
#>
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$dir = $PSScriptRoot
$logo = New-Object System.Drawing.Bitmap (Join-Path $dir '..\GUI\Resources\logo-landsmart.png')

# Colores de la aplicación (GUI\PN3Comun.cs)
$fondo = [System.Drawing.Color]::FromArgb(38, 31, 50)
$acento = [System.Drawing.Color]::FromArgb(89, 75, 160)

# Recuadro del símbolo: lo que no es fondo claro, por encima de la franja vacía que lo separa del texto
function Recuadro([System.Drawing.Bitmap]$img, [int]$hastaY) {
    $minX = $img.Width; $minY = $img.Height; $maxX = 0; $maxY = 0
    for ($y = 0; $y -lt $hastaY; $y += 4) {
        for ($x = 0; $x -lt $img.Width; $x += 4) {
            $p = $img.GetPixel($x, $y)
            if (($p.R + $p.G + $p.B) -lt 600) {
                if ($x -lt $minX) { $minX = $x }; if ($x -gt $maxX) { $maxX = $x }
                if ($y -lt $minY) { $minY = $y }; if ($y -gt $maxY) { $maxY = $y }
            }
        }
    }
    # Cuadrado centrado con un margen del 6 %, sin llegar al texto (si no entra, se corre hacia arriba)
    $lado = [Math]::Max($maxX - $minX, $maxY - $minY) * 1.12
    $cx = ($minX + $maxX) / 2; $cy = ($minY + $maxY) / 2
    $arriba = [Math]::Min($cy - $lado / 2, $hastaY - 1 - $lado)
    return New-Object System.Drawing.RectangleF ([float]($cx - $lado / 2)), ([float]$arriba), ([float]$lado), ([float]$lado)
}

# Franja vacía entre el símbolo y el texto "LandSmart"
$corte = $logo.Height
for ($y = [int]($logo.Height * 0.4); $y -lt $logo.Height; $y += 4) {
    $vacia = $true
    for ($x = 0; $x -lt $logo.Width -and $vacia; $x += 4) { $p = $logo.GetPixel($x, $y); if (($p.R + $p.G + $p.B) -lt 600) { $vacia = $false } }
    if ($vacia) { $corte = $y; break }
}
$simbolo = Recuadro $logo $corte

function Dibujar([int]$ancho, [int]$alto, [scriptblock]$accion) {
    $bmp = New-Object System.Drawing.Bitmap $ancho, $alto, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = 'HighQualityBicubic'; $g.SmoothingMode = 'AntiAlias'; $g.PixelOffsetMode = 'HighQuality'
    & $accion $g
    $g.Dispose()
    return $bmp
}

function GuardarBmp([System.Drawing.Bitmap]$bmp, [string]$nombre) {
    # BMP de 24 bits: el formato que acepta cualquier versión de Inno Setup
    $rgb = $bmp.Clone((New-Object System.Drawing.Rectangle 0, 0, $bmp.Width, $bmp.Height), [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $rgb.Save((Join-Path $dir $nombre), [System.Drawing.Imaging.ImageFormat]::Bmp)
    $rgb.Dispose()
}

function Redondeado([float]$x, [float]$y, [float]$w, [float]$h, [float]$r) {
    $p = New-Object System.Drawing.Drawing2D.GraphicsPath
    $p.AddArc($x, $y, 2 * $r, 2 * $r, 180, 90); $p.AddArc($x + $w - 2 * $r, $y, 2 * $r, 2 * $r, 270, 90)
    $p.AddArc($x + $w - 2 * $r, $y + $h - 2 * $r, 2 * $r, 2 * $r, 0, 90); $p.AddArc($x, $y + $h - 2 * $r, 2 * $r, 2 * $r, 90, 90)
    $p.CloseFigure()
    return $p
}

# Una parte del logo escalada, con el fondo claro transparente (el borde suavizado pasa a semitransparente). El fondo
# del logo no es blanco puro: así se apoya sobre blanco sin que se note el recuadro de la imagen.
function SinFondo([System.Drawing.RectangleF]$origen, [int]$lado) {
    $bmp = Dibujar $lado $lado {
        param($g)
        $g.Clear([System.Drawing.Color]::Transparent)
        $g.DrawImage($logo, (New-Object System.Drawing.RectangleF 0, 0, $lado, $lado), $origen, [System.Drawing.GraphicsUnit]::Pixel)
    }
    for ($y = 0; $y -lt $lado; $y++) {
        for ($x = 0; $x -lt $lado; $x++) {
            $p = $bmp.GetPixel($x, $y)
            $claro = [Math]::Min($p.R, [Math]::Min($p.G, $p.B))
            if ($claro -ge 228) { $a = 0 } elseif ($claro -le 190) { $a = 255 } else { $a = [int](255 * (228 - $claro) / 38) }
            $bmp.SetPixel($x, $y, [System.Drawing.Color]::FromArgb([int]($a * $p.A / 255), $p.R, $p.G, $p.B))
        }
    }
    return $bmp
}
$todo = New-Object System.Drawing.RectangleF 0, 0, $logo.Width, $logo.Height

# ---------- Banner lateral (bienvenida y final) ----------
$grandes = @{ 100 = @(164, 314); 125 = @(192, 386); 150 = @(246, 459); 175 = @(273, 556); 200 = @(328, 604) }
$logoSinFondo = @{}
foreach ($escala in $grandes.Keys) {
    $lado = $grandes[$escala][0] * 0.72
    $logoSinFondo[$escala] = SinFondo $todo ([int]($lado - 2 * [int]($lado * 0.04)))
}
foreach ($escala in $grandes.Keys) {
    $w = $grandes[$escala][0]; $h = $grandes[$escala][1]
    $bmp = Dibujar $w $h {
        param($g)
        $degradado = New-Object System.Drawing.Drawing2D.LinearGradientBrush (New-Object System.Drawing.Point 0, 0), (New-Object System.Drawing.Point 0, $h), $fondo, $acento
        $g.FillRectangle($degradado, 0, 0, $w, $h)
        $lado = $w * 0.72
        $x = ($w - $lado) / 2; $y = $h * 0.30 - $lado / 2 + $lado * 0.35
        $tarjeta = Redondeado $x $y $lado $lado ($lado * 0.10)
        $g.FillPath([System.Drawing.Brushes]::White, $tarjeta)
        $m = [int]($lado * 0.04)
        $g.DrawImage($logoSinFondo[$escala], [int]($x + $m), [int]($y + $m))
    }
    GuardarBmp $bmp "wizard-grande-$escala.bmp"; $bmp.Dispose()
}

# ---------- Imagen chica (arriba a la derecha) ----------
$chicas = @{ 100 = @(55, 55); 125 = @(64, 68); 150 = @(83, 80); 175 = @(92, 97); 200 = @(110, 106) }
foreach ($escala in $chicas.Keys) {
    $w = $chicas[$escala][0]; $h = $chicas[$escala][1]
    $bmp = Dibujar $w $h {
        param($g)
        $g.Clear([System.Drawing.Color]::White)
        $lado = [Math]::Min($w, $h)
        $s = SinFondo $simbolo $lado
        $g.DrawImage($s, [int](($w - $lado) / 2), [int](($h - $lado) / 2)); $s.Dispose()
    }
    GuardarBmp $bmp "wizard-chico-$escala.bmp"; $bmp.Dispose()
}

# ---------- Ícono ----------
# Símbolo sobre una baldosa blanca redondeada: se ve tanto en la barra de tareas clara como en la oscura
function SimboloEnBaldosa([int]$lado) {
    $margen = [Math]::Max(1, [int]($lado * 0.08))
    $s = SinFondo $simbolo ($lado - 2 * $margen)
    $bmp = Dibujar $lado $lado {
        param($g)
        $g.Clear([System.Drawing.Color]::Transparent)
        $g.FillPath([System.Drawing.Brushes]::White, (Redondeado 0 0 ($lado - 0.5) ($lado - 0.5) ([Math]::Max(1.5, $lado * 0.18))))
        $g.DrawImage($s, $margen, $margen)
    }
    $s.Dispose()
    return $bmp
}

# Hasta 64 px: mapa de bits clásico de 32 bits (BGRA de abajo hacia arriba + máscara AND vacía); 256 px: PNG
function Dib([System.Drawing.Bitmap]$b) {
    $n = $b.Width
    $ms = New-Object System.IO.MemoryStream
    $bw = New-Object System.IO.BinaryWriter $ms
    $filaMascara = [int]([Math]::Ceiling($n / 32.0) * 4)
    $bw.Write([UInt32]40); $bw.Write([Int32]$n); $bw.Write([Int32]($n * 2)); $bw.Write([UInt16]1); $bw.Write([UInt16]32)
    $bw.Write([UInt32]0); $bw.Write([UInt32]($n * $n * 4 + $filaMascara * $n)); $bw.Write([Int32]0); $bw.Write([Int32]0)
    $bw.Write([UInt32]0); $bw.Write([UInt32]0)
    for ($y = $n - 1; $y -ge 0; $y--) {
        for ($x = 0; $x -lt $n; $x++) { $p = $b.GetPixel($x, $y); $bw.Write([byte]$p.B); $bw.Write([byte]$p.G); $bw.Write([byte]$p.R); $bw.Write([byte]$p.A) }
    }
    $bw.Write((New-Object byte[] ($filaMascara * $n)))
    $bw.Flush()
    return , $ms.ToArray()
}
$tamanos = 16, 24, 32, 48, 64, 256
$imagenes = foreach ($t in $tamanos) {
    $b = SimboloEnBaldosa $t
    if ($t -ge 256) {
        $ms = New-Object System.IO.MemoryStream
        $b.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
        $datos = $ms.ToArray()
    }
    else { $datos = Dib $b }
    $b.Dispose()
    , $datos
}
$ico = New-Object System.IO.MemoryStream
$w = New-Object System.IO.BinaryWriter $ico
$w.Write([UInt16]0); $w.Write([UInt16]1); $w.Write([UInt16]$tamanos.Count)
$offset = 6 + 16 * $tamanos.Count
for ($i = 0; $i -lt $tamanos.Count; $i++) {
    $t = $tamanos[$i]; $lado = if ($t -ge 256) { 0 } else { $t }
    $w.Write([byte]$lado); $w.Write([byte]$lado); $w.Write([byte]0); $w.Write([byte]0)
    $w.Write([UInt16]1); $w.Write([UInt16]32); $w.Write([UInt32]$imagenes[$i].Length); $w.Write([UInt32]$offset)
    $offset += $imagenes[$i].Length
}
foreach ($datos in $imagenes) { $w.Write($datos) }
$w.Flush()
[System.IO.File]::WriteAllBytes((Join-Path $dir 'landsmart.ico'), $ico.ToArray())
$w.Dispose()

$logo.Dispose()
Get-ChildItem $dir -Include *.bmp, *.ico -File -Recurse | Select-Object Name, Length
