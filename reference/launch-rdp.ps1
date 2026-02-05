# RDP Launcher PowerShell Script
# This script launches RDP connections with custom resolution and monitor positioning
# to work around scaling issues with HiDPI monitors (4k+)

param(
    [Parameter(Mandatory=$true)]
    [string]$ServerAddress,
    
    [Parameter(Mandatory=$false)]
    [int]$Width = 1920,
    
    [Parameter(Mandatory=$false)]
    [int]$Height = 1080,
    
    [Parameter(Mandatory=$false)]
    [int]$MonitorId = 0,
    
    [Parameter(Mandatory=$false)]
    [int]$PositionX = 0,
    
    [Parameter(Mandatory=$false)]
    [int]$PositionY = 0
)

# Create temporary RDP file
$rdpFile = [System.IO.Path]::GetTempFileName() + ".rdp"

# RDP file content with custom settings
$rdpContent = @"
screen mode id:i:2
use multimon:i:0
desktopwidth:i:$Width
desktopheight:i:$Height
session bpp:i:32
winposstr:s:0,$PositionX,$PositionY,$($PositionX + $Width),$($PositionY + $Height)
compression:i:1
keyboardhook:i:2
audiocapturemode:i:0
videoplaybackmode:i:1
connection type:i:7
networkautodetect:i:1
bandwidthautodetect:i:1
displayconnectionbar:i:1
enableworkspacereconnect:i:0
disable wallpaper:i:0
allow font smoothing:i:0
allow desktop composition:i:0
disable full window drag:i:1
disable menu anims:i:1
disable themes:i:0
disable cursor setting:i:0
bitmapcachepersistenable:i:1
full address:s:$ServerAddress
audiomode:i:0
redirectprinters:i:1
redirectcomports:i:0
redirectsmartcards:i:1
redirectclipboard:i:1
redirectposdevices:i:0
autoreconnection enabled:i:1
authentication level:i:2
prompt for credentials:i:0
negotiate security layer:i:1
remoteapplicationmode:i:0
alternate shell:s:
shell working directory:s:
gatewayhostname:s:
gatewayusagemethod:i:4
gatewaycredentialssource:i:4
gatewayprofileusagemethod:i:0
promptcredentialonce:i:0
gatewaybrokeringtype:i:0
use redirection server name:i:0
rdgiskdcproxy:i:0
kdcproxyname:s:
"@

# Write RDP content to file
Set-Content -Path $rdpFile -Value $rdpContent -Encoding ASCII

Write-Host "Launching RDP connection to $ServerAddress"
Write-Host "Resolution: ${Width}x${Height}"
Write-Host "Position: ($PositionX, $PositionY)"
Write-Host "Monitor: $MonitorId"

# Launch RDP
Start-Process "mstsc.exe" -ArgumentList $rdpFile

# Wait a bit before cleaning up the temp file
Start-Sleep -Seconds 2

# Clean up temp file
if (Test-Path $rdpFile) {
    Remove-Item $rdpFile -Force
}
