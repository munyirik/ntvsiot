##Summary
The NTVS (Node.js Tools for Visual Studio) UWP Extension enables developers to deploy Node.js as a [Universal Windows Platform (UWP) application](https://github.com/ms-iot/node-uwp-wrapper) 
to Windows 10 IoT Core devices (e.g. Raspberry Pi), Windows 10 Desktop, and Windows 10 Mobile. 
This extension is written as a [project subtype](https://msdn.microsoft.com/en-us/library/bb166488.aspx) of [NTVS](http://aka.ms/ntvs) and extends some if its functionality like IntelliSense and debugging.

##Try it out!
Follow the instructions [here](http://ms-iot.github.io/content/en-US/win10/samples/NodejsWU.htm) for the steps to install the required software and to build your first 'Hello World' Node.js server.
You can also try out the [Blinky sample](http://ms-iot.github.io/content/en-US/win10/samples/NodejsWUBlinky.htm) which shows the use of WinRT APIs inside a Node.js server to toggle an LED on or off.

##Contributing
The NTVS UWP Extension code is covered by the [MIT license](http://opensource.org/licenses/MIT). There is no formal style-guide, but contributors should try to match the style of the file they are editing. 
Feel free to reach out if you need a hand in starting to tackle an issue. For new functionality, please contact us first so we can coordinate efforts (e.g. to avoid duplication of work, roadmap fit, etc.).
Since this code is under the MIT license, all contributions will be made under that license as well. Please don’t submit anything with any other licensing statements. If you want to make a contribution 
that includes code that you received under a different license, please let us know and we can figure out what to do. Please be sure that you have the right to make your contribution, including clearance 
from your employer if applicable. You must sign the [Microsoft Contributor License Agreement (CLA)](https://cla.microsoft.com/) before submitting your pull-request. The CLA only needs to be completed once 
to cover all Microsoft OSS projects.

##Creating new issues
Please follow the guidelines below when creating new issues:
* Use a descriptive title that identifies the issue to be addressed or the requested feature (e.g., "Feature F should report ABC when XYZ is used in DEF").
* Provide a detailed description of the issue or request feature.
* For bug reports, please also:
    * Describe the expected behavior and the actual behavior.
    * Provide example code or steps that reproduce the issue.
    * Specify any relevant exception messages and stack traces.
	
##Build and debug
**Prerequisites:** Visual Studio 2015 and Visual Studio 2015 SDK need to be installed.
* Install [Repo root]\Common\Prerequisites\VSTestHost.msi
* Run [Repo root]\Prerequisites\EnableSkipVerification.reg
* Uninstall Node.js Tools for UWP Apps if it is installed.
* Install NTVS from [here](http://aka.ms/ntvslatest).
* Run [Repo root]\Tests\PrepUwpEnv.ps1 (as Administrator) to copy files required by the Node.js UWP project system. You may specify "-clean true" as an argument to delete existing files.
* Open [Repo root]\NodejsUwp.sln.
* Hit F5 (or Debug Menu -> Start Debugging) to build and start debugging.
