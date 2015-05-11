@echo off

if /i "%1"=="clean" goto clean

@rem Copy UWP binaries
if exist "%programfiles(x86)%\NodejsUwp" goto copytargets
xcopy /F /I ..\Setup\NodejsUwpFiles\ARM\*.* "%programfiles(x86)%\NodejsUwp\ARM"
xcopy /F /I ..\Setup\NodejsUwpFiles\x86\*.* "%programfiles(x86)%\NodejsUwp\x86"
xcopy /F /I ..\Setup\NodejsUwpFiles\x64\*.* "%programfiles(x86)%\NodejsUwp\x64"

:copytargets
@rem Copy UWP targets file
if exist "%programfiles(x86)%\MSBuild\Microsoft\VisualStudio\v14.0\Node.js Tools\Microsoft.NodejsUwp.targets" goto end
xcopy /F /I ..\Setup\NodejsUwpFiles\Microsoft.NodejsUwp.targets "%programfiles(x86)%\MSBuild\Microsoft\VisualStudio\v14.0\Node.js Tools"

goto end

:clean
@rem Delete UWP binaries
if not exist "%programfiles(x86)%\NodejsUwp" goto deletetargets
rmdir /S /Q "%programfiles(x86)%\NodejsUwp"

:deletetargets
@rem Copy UWP targets file
if not exist "%programfiles(x86)%\MSBuild\Microsoft\VisualStudio\v14.0\Node.js Tools\Microsoft.NodejsUwp.targets" goto end
del /Q "%programfiles(x86)%\MSBuild\Microsoft\VisualStudio\v14.0\Node.js Tools\Microsoft.NodejsUwp.targets"

:end