# Copies Node.js (Chakra) to a Windows 10 IoT Core device.

# Prerequisites:
# 1. Node.js (Chakra) installed on the PC running this script.
# 2. Internet connection (to download a node.exe that matches the target device processor architecture).

# Usage:
# .\CopyNodeChakra.ps1 -arch <ARM | x86 | x64 > -ip <Device IP Address> -user <Username> -pass <Password>

# Example:
# .\CopyNodeChakra.ps1 -arch ARM -ip 10.125.152.300 -user Administrator -pass p@ssw0rd

param(
[string]$arch = "", 
[string]$ip = "",
[string]$username = "",
[string]$password = ""
)

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

$key = 'HKLM:\SOFTWARE\WOW6432Node\Node.js (Chakra)'
$sourcePath = (Get-ItemProperty -Path $key -Name InstallPath).InstallPath
$destinationPath = "\\" + $ip + "\c$"
$nodePath = $destinationPath + "\Node.js (Chakra)\"

#
# Copy installed 'Node.js (Chakra)' directory to device
#
if((0 -eq $password.CompareTo("")))
{
  Write-Host "Error: Password not provided"-foreground "Red"
  Write-Host "Usage: .\CopyNodeChakra.ps1 -arch <ARM | x86 | x64 > -ip <Device IP Address> -user <Username> -pass <Password>"
  Exit
}
if((0 -eq $username.CompareTo("")))
{
  Write-Host "Error: Username not provided"-foreground "Red"
  Write-Host "Usage: .\CopyNodeChakra.ps1 -arch <ARM | x86 | x64 > -ip <Device IP Address> -user <Username> -pass <Password>"
  Exit
}
$secureString  = ConvertTo-SecureString $password -AsPlainText -Force
$creds = New-Object System.Management.Automation.PSCredential ($username, $secureString)
New-PSDrive -Name IoTDrive -PSProvider FileSystem -Root $destinationPath -Credential $creds

Write-Host "Copying" $sourcePath "to" $nodePath "..."
Get-ChildItem $sourcePath -Recurse | Where {$_.FullName -notlike "$sourcePath" + "node.exe" -and $_.FullName -notlike "$sourcePath" + "chakra.dll" -and $_.FullName -notlike "$sourcePath" + "sdk*"} |
    Copy-Item -Destination {Join-Path $nodePath $_.FullName.Substring($sourcePath.length)} -Force

Write-Host "" # \n

$client = New-Object System.Net.WebClient

#
# Copy node.exe to device based on processor architecture
#
$arch = $arch.ToLower()

switch($arch) {
    "arm" { $nodeExeUrl = [uri]"https://github.com/ms-iot/ntvsiot/releases/download/0.12.7.5-exe-arm/node.exe" }
    "x86" { $nodeExeUrl = [uri]"https://github.com/ms-iot/ntvsiot/releases/download/0.12.7.5-exe-x86/node.exe" }
    "x64" { $nodeExeUrl = [uri]"https://github.com/ms-iot/ntvsiot/releases/download/0.12.7.5-exe-x64/node.exe" }
    default { Write-Host "Cannot copy node.exe to device. No valid device processor architecture provided. Valid values are ARM, x86, or x64" -foreground "Red"; Exit; }
}

Write-Host "Copying" $nodeExeUrl "to" $nodePath "..."
try {
    Register-ObjectEvent $client DownloadFileCompleted -SourceIdentifier Finished
    $nodePath = $nodePath + "node.exe"
    $client.DownloadFileAsync($nodeExeUrl, $nodePath)
    Wait-Event -SourceIdentifier Finished
} finally {
    $client.dispose()
    Unregister-Event -SourceIdentifier Finished
    Remove-Event -SourceIdentifier Finished
}
