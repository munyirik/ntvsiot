# This script will set up the targets file and Node.js device binaries on your PC.
# Run this script before debugging the NTVS IoT Extension project.

param([string]$clean = "false")

$targetsDestination = [environment]::GetEnvironmentVariable("ProgramFiles(x86)") + "\MSBuild\Microsoft\VisualStudio\v14.0\Node.js Tools\Microsoft.NodejsUwp.targets"
$binaryDestination = [environment]::GetEnvironmentVariable("ProgramFiles(x86)") + "\NodejsUwp\"

# Clean existing files if specified
if($clean -eq "true") 
{
  Remove-Item $targetsDestination
  Remove-Item "$binaryDestination*" -Recurse
}

# Download the Node.js UWP device binaries
$client = New-Object System.Net.WebClient

$nodejsUwpZipUrl = [uri]"https://github.com/ms-iot/ntvsiot/releases/download/0.12.7.4-NodejsUwp/NodejsUwp.zip"
$currentDir = $PSScriptRoot + "\"

Write-Host "Copying $nodejsUwpZipUrl to $currentDir ..."
try {
    Register-ObjectEvent $client DownloadFileCompleted -SourceIdentifier Finished
    $localZipPath = $currentDir + "NodejsUwp.zip"
    $client.DownloadFileAsync($nodejsUwpZipUrl, $localZipPath)
    Wait-Event -SourceIdentifier Finished
} finally {
    Write-Host "$nodejsUwpZipUrl download completed" -foreground "Green"
    $client.dispose()
    Unregister-Event -SourceIdentifier Finished
    Remove-Event -SourceIdentifier Finished
}

# Unzip binaries to destination
Add-Type -assembly "system.io.compression.filesystem"
[io.compression.zipfile]::ExtractToDirectory($localZipPath, $binaryDestination)

# Delete temporary downloaded zip file
Remove-Item $localZipPath

# Copy targets file
Copy-Item -Path "..\Setup\NodejsUwpFiles\Microsoft.NodejsUwp.targets" -Destination $targetsDestination