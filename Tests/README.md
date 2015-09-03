##How to run tests
* Install Visual Studio 2015 SDK.
* Install the latest NTVS base project from [here](http://aka.ms/ntvslatest).
* Install ..\Common\Prerequisites\VSTestHost.msi. This step installs a DLL that is referenced by the test projects.
* Run PrepUwpEnv.ps1 to copy files required by the Node.js UWP project system. You may specify "-clean true" as an argument to delete existing files.
* Open ..\NodejsUwp.sln and build it (the tests are contained in the **NodejsUwp.Tests** project).
* Once you have built the solution, the tests can be run from the Test Explorer window (Go to "Test" Visual Studio menu, click Window->Test Explorer).