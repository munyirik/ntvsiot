@echo off

set bundle_dir=%~dp0

if "%WIX%"=="" (
  echo Error: Install Wix Toolset v3.9 or newer. You may also set WIX environment variable to \PATH TO THIS NTVSIOT GIT CLONE\Tools\Wix\3.9
  goto end
)

"%WIX%bin\candle.exe" NTVSBundleInstaller.wxs -ext WixBalExtension
"%WIX%bin\light.exe" NTVSBundleInstaller.wixobj -ext WixBalExtension -cultures:en-us -loc Theme.wxl -out "NTVS IoT Installer.exe"

echo Signing the bundle installer...
"%WIX%bin\insignia.exe" -ib "NTVS IoT Installer.exe" -o engine.exe
powershell -command "& { . .\sign.ps1; begin_sign_files -files 'engine.exe' -bindir '%bundle_dir%' -outdir '%bundle_dir%SignedInstaller' -approvers 'jinglou','sitani' -projectName 'NTVS IoT' -projectUrl 'https://github.com/ms-iot/ntvsiot' -jobDescription 'NTVS IoT Wix Burn Engine' -jobKeywords 'NTVS', 'Node.js' -certificates 'authenticode'}"
"%WIX%bin\insignia.exe" -ab "%bundle_dir%SignedInstaller\engine.exe" "NTVS IoT Installer.exe" -o "NTVS IoT Installer.exe"
powershell -command "& { . .\sign.ps1; begin_sign_files -files 'NTVS IoT Installer.exe' -bindir '%bundle_dir%' -outdir '%bundle_dir%SignedInstaller' -approvers 'jinglou','sitani' -projectName 'NTVS IoT' -projectUrl 'https://github.com/ms-iot/ntvsiot' -jobDescription 'NTVS IoT' -jobKeywords 'NTVS', 'Node.js' -certificates 'authenticode'}"

:end