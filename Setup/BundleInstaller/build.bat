@echo off

if "%WIX%"=="" (
  echo Error: Install Wix Toolset v3.9 or newer. You may also set WIX environment variable to \PATH TO THIS NTVSIOT GIT CLONE\Tools\Wix\3.9
  goto end
)

"%WIX%bin\candle.exe" NTVSBundleInstaller.wxs -ext WixBalExtension
"%WIX%bin\light.exe" NTVSBundleInstaller.wixobj -ext WixBalExtension -cultures:en-us -loc Theme.wxl -out "NTVS Bundle VS 2015.exe"

:end