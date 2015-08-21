# Usage:
# build.bat version=<number> [sign=true|false]

param (
    [string]$sign = "false",
    [string]$version = ""
 )

# Copy NTVS installer to current directory.
$ReleasePath = "\\cpvsbuild\Drops\nodejstools\NTVS_Out\"
$dirs= Get-ChildItem -Path $ReleasePath | Where-Object {$_.Name -match "[0-9]\.[0-9]"} | Sort-Object -Descending
$latestDir = $ReleasePath + $dirs[0].Name + "\"
$MSIName = Get-ChildItem -Path $latestDir | Where-Object {$_.Name -match "VS 2015.msi$"}
$MSINamePath = $latestDir + $MSIName
Copy-Item -Path $MSINamePath -Destination ".\NTVS.msi"

# Copy Node.js (Chakra) installer to current directory.
$ReleasePath = "\\bpt-scratch\userfiles\node-chakra\release\node\"
$dirs= Get-ChildItem -Path $ReleasePath | Sort-Object -Descending
$latestDir = $ReleasePath + $dirs[0].Name + "\x86\UnsignedMsi\" #TODO: Change to 'SignedMsi'
$MSIName = Get-ChildItem -Path $latestDir
$MSINamePath = $latestDir + $MSIName
Copy-Item -Path $MSINamePath -Destination ".\node-chakra.msi"

# Copy NTVS IoT Extension installer to current directory.
$ReleasePath = "\\scratch2\scratch\IoTDevX\NTVSIoT\Release\IoTExtension\1.0\"
$dirs= Get-ChildItem -Path $ReleasePath | Where-Object {$_.Name -match "[0-9]\.[0-9]"} | Sort-Object -Descending
$latestDir = $ReleasePath + $dirs[0].Name + "\"
$MSIName = Get-ChildItem -Path $latestDir | Where-Object {$_.Name -match "VS 2015.msi$"}
$MSINamePath = $latestDir + $MSIName
Copy-Item -Path $MSINamePath -Destination ".\NTVSIoTExtension.msi"


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


# Build the bundle installer. This step involves calls to both candle.exe and light.exe.
$wixCandleArgs = "NTVSBundleInstaller.wxs -ext WixBalExtension"
Start-Process -FilePath "..\..\Tools\Wix\3.9\candle.exe" -ArgumentList $wixCandleArgs -Wait -NoNewWindow -RedirectStandardOutput wixCandle.log

$wixLightArgs = "NTVSBundleInstaller.wixobj -ext WixBalExtension -cultures:en-us -loc Theme.wxl -out `"" + $installerName + "`""
Start-Process -FilePath "..\..\Tools\Wix\3.9\light.exe" -ArgumentList $wixLightArgs -Wait -NoNewWindow -RedirectStandardOutput wixLight.log

# Copy the build output to outDir
Copy-Item -Path (".\" + $installerName) -Destination ($outDir + "UnsignedMsi")
Copy-Item -Path ".\*.log" -Destination ($outDir + "Logs")

if ($sign -eq "true")
{
    Write-Host "Signing the bundle installer..."

    $env:bundle_dir = $PWD.Path + "\"

    # First sign the Wix burn engine (installation will fail without this step)
    $wixInsigniaArgs = "-ib `"Node.js Tools for Windows IoT " + $version + ".exe`" -o engine.exe"
    Start-Process -FilePath "..\..\Tools\Wix\3.9\insignia.exe" -ArgumentList $wixInsigniaArgs -Wait -NoNewWindow -RedirectStandardOutput wixInsigniaEngine.log
    powershell -command "& { . .\sign.ps1; begin_sign_files -files 'engine.exe' -bindir '%bundle_dir%' -outdir '%bundle_dir%SignedInstaller' -approvers 'jinglou','sitani' -projectName 'Node.js Tools for Windows IoT' -projectUrl 'https://github.com/ms-iot/ntvsiot' -jobDescription 'NTVS IoT Wix Burn Engine' -jobKeywords 'NTVS', 'Node.js' -certificates 'authenticode'}"
    
    # Sign the installer
    $wixInsigniaArgs = "-ab `".\SignedInstaller\engine.exe`"" + " `"" + $installerName + "`"" + " -o " + "`"" + $installerName + "`""
    Start-Process -FilePath "..\..\Tools\Wix\3.9\insignia.exe" -ArgumentList $wixInsigniaArgs -Wait -NoNewWindow -RedirectStandardOutput wixInsigniaInstaller.log
    powershell -command "& { . .\sign.ps1; begin_sign_files -files 'Node.js Tools for Windows IoT %NodejsToolsBundleVer%.exe' -bindir '%bundle_dir%' -outdir '%bundle_dir%SignedInstaller' -approvers 'jinglou','sitani' -projectName 'Node.js Tools for Windows IoT' -projectUrl 'https://github.com/ms-iot/ntvsiot' -jobDescription 'Node.js Tools for Windows IoT' -jobKeywords 'NTVS', 'Node.js' -certificates 'authenticode'}"
}