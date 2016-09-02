##Summary
The NTVS (Node.js Tools for Visual Studio) UWP Extension enables developers to deploy Node.js as a [Universal Windows Platform (UWP) application](https://github.com/ms-iot/node-uwp-wrapper) 
to Windows 10 IoT Core devices (e.g. Raspberry Pi), Windows 10 Desktop, and Windows 10 Mobile. 
This extension is written as a [project subtype](https://msdn.microsoft.com/en-us/library/bb166488.aspx) of [NTVS](http://aka.ms/ntvs) and extends some if its functionality like IntelliSense and debugging.
Within Node.js (UWP) applications, you have the ability to access UWP APIs as shown in the simple calendar server example below. This is done using the [uwp](https://www.npmjs.com/package/uwp) npm package which is deployed 
automatically with your project.

```javascript
var http = require('http');
var uwp = require("uwp");

uwp.projectNamespace("Windows");
var calendar = new Windows.Globalization.Calendar();

http.createServer(function (req, res) {
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    var date = calendar.getDateTime();
    res.end(String(date));
}).listen(1337);

uwp.close();
```

##Try it out!
Follow the instructions [here](https://developer.microsoft.com/en-us/windows/iot/samples/helloworldnode) for the steps to install the required software and to build your first 'Hello World' Node.js server.
You can also try out the [Blinky sample](https://developer.microsoft.com/en-us/windows/iot/samples/helloblinkynode) which shows the use of UWP APIs inside a Node.js server to toggle an LED on or off.

Other samples to try out:
* [Control a servo with Johnny-Five](https://developer.microsoft.com/en-us/windows/iot/samples/j5servocontroller)
* [Control a servo with Cylon](https://developer.microsoft.com/en-us/windows/iot/samples/cylonservonode)
* [Send data from a light sensor to Azure](https://developer.microsoft.com/en-us/windows/iot/samples/azuredatauploader)


##Deployment to PC and Phone
In order to deploy an app to your PC or Phone, you must first enable Embedded Mode. See [this](https://developer.microsoft.com/en-us/windows/iot/docs/embeddedmode) page for instructions on how to do that.
You also need to turn on `Developer mode`. This can be done on the `Settings -> Update & Security -> For Developers` page.


##Intellisense for UWP API
IntelliSense is available for UWP APIs through the [ES6 Intellisense](https://github.com/Microsoft/nodejstools/wiki/ES6-IntelliSense-Preview-in-NTVS-1.1) feature in NTVS 1.2.

  ![capture](https://cloud.githubusercontent.com/assets/8389594/14468105/227e9760-0093-11e6-98c2-cc4b8dcd05e3.PNG)

To enable IntelliSense in your project:
* Ensure you have the 'ES6 IntelliSense Preview' option set under `Tools -> Options -> Text Editor -> Node.js -> IntelliSense`
* Right click on your project in the Solution Explorer and select 'Open Command Prompt Here...'
* Run `npm install typings -g`
* Run `typings install dt~winrt-uwp --global`


##Installing npm packages
Instructions on how to use the NTVS npm UI can be found [here](https://github.com/Microsoft/nodejstools/wiki/npm-Integration).
If you are installing a native package or a package with native dependencies, the following is required.
In the 'Other npm arguments' textbox in the `Install New npm packages` dialog, you can enter `--target_arch=arm|x86|x64 --node_uwp_dll` to target UWP. Example:

  ![capture](https://cloud.githubusercontent.com/assets/8389594/18188096/ecab2eb4-7063-11e6-932f-d6b37aa280ea.PNG)

Note:
* The [uwp](https://github.com/microsoft/node-uwp) native addon will always be included in your project automatically so there is no need to install it.
* Existing npm packages are likely to use APIs that are banned in UWP apps. In this case support for UWP would need to be added to the package. The serialport fork
[here](https://github.com/ms-iot/node-serialport/tree/uwp) gives an example of how this can be done without affecting existing platforms.


##Creating new issues
Please follow the guidelines below when creating new issues:
* Use a descriptive title that identifies the issue to be addressed or the requested feature (e.g., "Feature F should report ABC when XYZ is used in DEF").
* Provide a detailed description of the issue or request feature.
* For bug reports, please also:
    * Describe the expected behavior and the actual behavior.
    * Provide example code or steps that reproduce the issue.
    * Specify any relevant exception messages and stack traces.
	

##Contributing
The NTVS UWP Extension code is covered by the [MIT license](http://opensource.org/licenses/MIT). There is no formal style-guide, but contributors should try to match the style of the file they are editing. 
Feel free to reach out if you need a hand in starting to tackle an issue. For new functionality, please contact us first so we can coordinate efforts (e.g. to avoid duplication of work, roadmap fit, etc.).
Since this code is under the MIT license, all contributions will be made under that license as well. Please don’t submit anything with any other licensing statements. If you want to make a contribution 
that includes code that you received under a different license, please let us know and we can figure out what to do. Please be sure that you have the right to make your contribution, including clearance 
from your employer if applicable. You must sign the [Microsoft Contributor License Agreement (CLA)](https://cla.microsoft.com/) before submitting your pull-request. The CLA only needs to be completed once 
to cover all Microsoft OSS projects.


##Debugging the Extension
As a contributor, you can use the steps below to set up your PC to build and debug the code in this repository.

**Prerequisites:** Visual Studio 2015 and Visual Studio 2015 SDK need to be installed.
* Install [Repo root]\Common\Prerequisites\VSTestHost.msi
* Run [Repo root]\Prerequisites\EnableSkipVerification.reg
* Uninstall Node.js Tools for UWP Apps if it is installed.
* Install NTVS from [here](http://aka.ms/ntvslatest).
* Run [Repo root]\Tests\PrepUwpEnv.ps1 (as Administrator) to copy files required by the Node.js UWP project system. You may specify "-clean true" as an argument to delete existing files.
* Open [Repo root]\NodejsUwp.sln.
* Hit F5 (or Debug Menu -> Start Debugging) to build and start debugging.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). 
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) 
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
