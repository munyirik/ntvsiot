The NTVS Bundle installer makes it convenient to install Node.js Tools for Windows IoT. Instead of users having to install [Node.js (Chakra)](https://github.com/Microsoft/node), 
[NTVS](http://aka.ms/ntvslatest), and this IoT extension separately, they can use one installer.

##How to build the NTVS Bundle Installer
* Copy the [Node.js (Chakra) installer](https://github.com/Microsoft/node) to .\BundleInstaller directory and rename to node-chakra.msi.
* Copy the [NTVS installer](http://aka.ms/ntvslatest) to .\BundleInstaller directory and rename to NTVS.msi.
* Copy the NTVS IoT Extension installer (built from this project) to .\BundleInstaller directory and rename to NTVSIoTExtension.msi.
* Run build.bat.