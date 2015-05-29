##Summary
The NTVS IoT Extension (Beta) enables developers to deploy Node.js as a [Universal Windows Platform (UWP) application](https://github.com/ms-iot/node-uwp) to Windows IoT Core devices like Raspberry Pi 2. 
This extension is written as a [project subtype](https://msdn.microsoft.com/en-us/library/bb166488.aspx) of [NTVS](http://aka.ms/ntvs) and extends some if its functionality like IntelliSense and debugging.

##Try it out!
Follow the instructions [here](http://ms-iot.github.io/content/en-US/win10/samples/NodejsWU.htm) for the steps to install the required software and to build your first 'Hello World' Node.js server.
You can also try out the [Blinky sample](http://ms-iot.github.io/content/en-US/win10/samples/NodejsWUBlinky.htm) which shows the use of WinRT APIs inside a Node.js server to toggle an LED on or off.

##Known issues
Currently, only IntelliSense, Editing, and Debugging are supported from the existing [NTVS features](https://github.com/Microsoft/nodejstools/wiki). Features like npm will not work as expected for now but will be available in the future.
If you see problems related to any supported feature, feel free to open an issue. Feature requests are also welcome.

##Contributing
Coming soon.
