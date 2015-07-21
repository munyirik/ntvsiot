# Copies Node.js (Chakra) to a Windows 10 IoT Core device.

# Prerequisites:
# 1. Node.js (Chakra) installed on the PC running this script.
# 2. Internet connection (to download a node.exe that matches the target device processor architecture).

# Usage:
# .\CopyNodeChakra.ps1 -arch <ARM | x86 | x64 > -ip <Device IP Address>

# Example:
# .\CopyNodeChakra.ps1 -arch ARM -ip 10.125.152.300

param([string]$arch = "", [string]$ip = "")

# Check arguments
if((0 -eq $arch.CompareTo("")))
{
  Write-Host "Error: Processor architecture not provided"-foreground "Red"
  Write-Host "Usage: .\CopyNodeChakra.ps1 -arch <ARM | x86 | x64 > -ip <Device IP Address>"
  Exit
}
if((0 -eq $ip.CompareTo("")))
{
  Write-Host "Error: Device IP address not provided"-foreground "Red"
  Write-Host "Usage: .\CopyNodeChakra.ps1 -arch <ARM | x86 | x64 > -ip <Device IP Address>"
  Exit
}

$key = 'HKCU:\SOFTWARE\Node.js (Chakra)'
$sourcePath = (Get-ItemProperty -Path $key -Name InstallPath).InstallPath
$destinationPath = "\\" + $ip + "\c$\Node.js (Chakra)\"

#
# Copy installed 'Node.js (Chakra)' directory to device
#
Write-Host "Copying" $sourcePath "to" $destinationPath "..."
Get-ChildItem $sourcePath -Recurse | Where {$_.FullName -notlike "$sourcePath" + "node.exe" -and $_.FullName -notlike "$sourcePath" + "sdk*"} |
    Copy-Item -Destination {Join-Path $destinationPath $_.FullName.Substring($sourcePath.length)} -Force
Write-Host "Copy completed" -foreground "Green"

Write-Host "" # \n

$client = New-Object System.Net.WebClient

#
# Copy node.exe to device based on processor architecture
#
$arch = $arch.ToLower()

switch($arch) {
    "arm" { $nodeExeUrl = [uri]"https://github.com/ms-iot/ntvsiot/releases/download/0.12.6.2-exe-arm/node.exe" }
    "x86" { $nodeExeUrl = [uri]"https://github.com/ms-iot/ntvsiot/releases/download/0.12.6.2-exe-x86/node.exe" }
    "x64" { $nodeExeUrl = [uri]"https://github.com/ms-iot/ntvsiot/releases/download/0.12.6.2-exe-x64/node.exe" }
    default { Write-Host "Cannot copy node.exe to device. No valid device processor architecture provided. Valid values are ARM, x86, or x64" -foreground "Red"; Exit; }
}

Write-Host "Copying" $nodeExeUrl "to" $destinationPath "..."
try {
    Register-ObjectEvent $client DownloadFileCompleted -SourceIdentifier Finished
    $destinationPath = $destinationPath + "node.exe"
    $client.DownloadFileAsync($nodeExeUrl, $destinationPath)
    Wait-Event -SourceIdentifier Finished
} finally {
    Write-Host "Copy completed" -foreground "Green"
    $client.dispose()
    Unregister-Event -SourceIdentifier Finished
    Remove-Event -SourceIdentifier Finished
}
