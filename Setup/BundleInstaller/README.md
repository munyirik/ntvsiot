The NTVS Bundle installer makes it convenient to Install NTVS tools for Visual Studio. Instead of users having to install the [NTVS base project](http://aka.ms/ntvslatest) and this IoT extension separately,
they can use one installer that can install both tools.

##How to build the NTVS Bundle Installer
* Copy the NTVS base installer (e.g. NTVS 1.1 Beta VS 2015.msi) and IoT extension installer (e.g. NTVS  IoT Extension Beta VS 2015.msi) to the .\BundleInstaller directory (where NTVSBundleInstaller.wxs is located).
* In NTVSBundleInstaller.wxs, ensure that the SourceFile attributes in the MsiPackage elements match the names of the MSI's.
* Run build.bat.