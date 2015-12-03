##How to run tests
* Install Visual Studio 2015 SDK.
* Install the latest NTVS base project from [here](http://aka.ms/ntvslatest).
* Install [Repo root]\Common\Prerequisites\VSTestHost.msi. This step installs a DLL that is referenced by the test projects.
* Run PrepUwpEnv.ps1 to copy files required by the Node.js UWP project system. You may specify "-clean true" as an argument to delete existing files.
* Open [Repo root]\NodejsUwp.sln (the tests are contained in the **NodejsUwp.Tests** project).
* Go to Test (menu) -> Test Settings -> Select Test Settings File.
* Select [Repo root]\Build\default.14.0Exp.testsettings.
* Build the solution.
* Run tests from the Test Explorer window (Go to "Test" Visual Studio menu, click Window->Test Explorer).