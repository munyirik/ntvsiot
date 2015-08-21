# Usage:
# build.bat version=<number> [sign=true|false]

param (
    [string]$sign = "false",
    [string]$version = ""
)

$localReleaseBinDir = ".\Release"
$localReleaseLogsDir = ".\Release\Logs"
#$localDebugBinDir = ".\Debug"
#$localDebugLogsDir = ".\Debug\Logs"

# Create local release directory
if(Test-Path $localReleaseBinDir)
{
    # Clean existing files
    Remove-Item "$localReleaseBinDir\*" -Recurse
}

New-Item -ItemType Directory -Force -Path ".\Release" # For binaries
New-Item -ItemType Directory -Force -Path ".\Release\Logs" # For logs

# Get the NTVS installer
Write-Host "Copying NTVS installer to $localReleaseBinDir..."
$ReleasePath = "\\cpvsbuild\Drops\nodejstools\NTVS_Out\"
$dirs= Get-ChildItem -Path $ReleasePath | Where-Object {$_.Name -match "[0-9]\.[0-9]"} | Sort-Object -Descending
$latestDir = $ReleasePath + $dirs[0].Name + "\"
$MSIName = Get-ChildItem -Path $latestDir | Where-Object {$_.Name -match "VS 2015.msi$"}
$MSINamePath = $latestDir + $MSIName
Copy-Item -Path $MSINamePath -Destination "$localReleaseBinDir\NTVS.msi"

# Get the Node.js (Chakra) installer.
Write-Host "Copying Node.js (Chakra) installer to $localReleaseBinDir..."
$ReleasePath = "\\bpt-scratch\userfiles\node-chakra\release\node\"
$dirs= Get-ChildItem -Path $ReleasePath | Sort-Object -Descending
$latestDir = $ReleasePath + $dirs[0].Name + "\x86\UnsignedMsi\" #TODO: Change to 'SignedMsi'
$MSIName = Get-ChildItem -Path $latestDir
$MSINamePath = $latestDir + $MSIName
Copy-Item -Path $MSINamePath -Destination "$localReleaseBinDir\node-chakra.msi"

# Get the NTVS IoT Extension installery.
Write-Host "Copying NTVS IoT Extension installer to release directory..."
$ReleasePath = "\\scratch2\scratch\IoTDevX\NTVSIoT\Release\IoTExtension\1.0\"
$dirs= Get-ChildItem -Path $ReleasePath | Where-Object {$_.Name -match "[0-9]\.[0-9]"} | Sort-Object -Descending
$latestDir = $ReleasePath + $dirs[0].Name + "\"
$MSIName = Get-ChildItem -Path $latestDir | Where-Object {$_.Name -match "VS 2015.msi$"}
$MSINamePath = $latestDir + $MSIName
Copy-Item -Path $MSINamePath -Destination "$localReleaseBinDir\NTVSIoTExtension.msi"


# Determine the build number based on today's date. The date is used to get the build (and msi)
# version number by default. If "version" parameter is used in this script then that value is used.

$baseYear = 2015  # This value is used to determine the most significant digit of the build number.
$outDir = "\\scratch2\scratch\IoTDevX\NTVSIoT\Release\Bundle\"

if((0 -eq $version.CompareTo("")))
{
    $buildNumber = '{0}{1:MMdd}.{2:D2}' -f (((Get-Date).Year - $baseYear), (Get-Date), 0)
    for ($buildIndex = 0; $buildIndex -lt 10000; $buildIndex += 1) {
        $buildNumber = '{0}{1:MMdd}.{2:D2}' -f (((Get-Date).Year - $baseYear), (Get-Date), $buildIndex)
        if (-not (Test-Path $outDir\$buildNumber)) {
            break
        }
        $buildNumber = ''
    }
    if (-not $buildNumber) {
        Throw "Cannot create version number. Try another output folder."
    }
    if ([int]::Parse([regex]::Match($buildNumber, '^[0-9]+').Value) -ge 65535) {
        Throw "Build number $buildNumber is invalid. Update `$base_year in this script.
    (If the year is not yet $($baseYear + 7) then something else has gone wrong.)"
    }

    $version = $buildNumber
}

$outDir = $outDir + $version + "\"
$installerName = "Node.js Tools for Windows IoT " + $version + ".exe"

# Set NodejsToolsBundleVer for the bundle wxs file to use to set the version
$env:NodejsToolsBundleVer = $version
$env:NodejsToolsBundleRelDir = $localReleaseBinDir

Write-Host "Building the installer..."
# Build the bundle installer. This step involves calls to both candle.exe and light.exe.
$wixCandleArgs = "NTVSBundleInstaller.wxs -ext WixBalExtension -o $localReleaseBinDir\NTVSBundleInstaller.wixobj"
Start-Process -FilePath "..\..\Tools\Wix\3.9\candle.exe" -ArgumentList $wixCandleArgs -Wait -NoNewWindow -RedirectStandardOutput "$localReleaseLogsDir\wixCandle.log"

$wixLightArgs = $localReleaseBinDir + "\NTVSBundleInstaller.wixobj -ext WixBalExtension -cultures:en-us -loc Theme.wxl -out `"" + $localReleaseBinDir + "\" +$installerName + "`""
Start-Process -FilePath "..\..\Tools\Wix\3.9\light.exe" -ArgumentList $wixLightArgs -Wait -NoNewWindow -RedirectStandardOutput "$localReleaseLogsDir\wixLight.log"

# Sign the Node.js Tools for IoT installer
if ($sign -eq "true")
{
    Write-Host "Signing the installer..."
    Import-Module -Force "..\..\Build\BuildReleaseHelpers.psm1"

    $approvers = "jinglou", "sitani"
    $project_name = "Node.js Tools for Windows IoT"
    $project_url = "https://github.com/ms-iot/ntvsiot"
    $project_keywords = "NTVS IoT; Node.js Tools for IoT; Node.js"

    # First sign the Wix burn engine (installation will fail without this step)
    $wixInsigniaArgs = "-ib `"$localReleaseBinDir\Node.js Tools for Windows IoT " + $version + ".exe`" -o $localReleaseBinDir\engine.exe"
    Start-Process -FilePath "..\..\Tools\Wix\3.9\insignia.exe" -ArgumentList $wixInsigniaArgs -Wait -NoNewWindow -RedirectStandardOutput "$localReleaseLogsDir\wixInsigniaEngine.log"

    begin_sign_files "engine.exe" "$localReleaseBinDir\Signed" $approvers `
    $project_name $project_url "$project_name Wix Burn Engine" $project_keywords `
    "authenticode"
    
    # Sign the installer
    $wixInsigniaArgs = "-ab `"$localReleaseBinDir\engine.exe`"" + " `"$localReleaseBinDir\" + $installerName + "`"" + " -o " + "`"$localReleaseBinDir" + $installerName + "`""
    Start-Process -FilePath "..\..\Tools\Wix\3.9\insignia.exe" -ArgumentList $wixInsigniaArgs -Wait -NoNewWindow -RedirectStandardOutput "$localReleaseLogsDir\wixInsigniaInstaller.log"

    begin_sign_files "$localReleaseBinDir\$installerName" "$localReleaseBinDir\Signed" $approvers `
    $project_name $project_url "$project_name Node.js Tools for IoT" $project_keywords `
    "authenticode"
}

# Copy the build output to outDir
Copy-Item -Path $localReleaseBinDir -Destination $outDir -Recurse