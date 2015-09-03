// !!BEFORE YOU RUN THIS CODE!!

// 1. Follow the steps in the "Build Serialport" section in the link below:
// http://ms-iot.github.io/content/en-US/win10/samples/NodejsWUCylon.htm
// Even though serialport (a Cylon dependency) is installed when a new 
// Cylon project is created, it needs to be rebuilt so that:
//   - It matches the processor architecture of the device you are targeting.
//   - It is UWP (Universal Windows Platform) compatible.
// The link above also has a sample code and steps to run Cylon on a Raspberry Pi 2 
// to control a servo connected to an Arduino.
// Go to http://cylonjs.com/documentation for Cylon.js documentation and more samples.

// 2. Make sure npm v3 installs Cylon and its dependencies. To install npm v3:
//   - Open a command window (as Administrator) and run 'npm install -g npm-windows-upgrade'
//   - Then run 'npm-windows-upgrade --version:3.2.2'
// If this project was not opened with npm v3 installed, you will need to uninstall and
// reinstall the packages in this project: cylon, cylon-firmata, cylon-gpio, and cylon-i2c.

// 3. Run 'npm dedupe' (in a cmd window) in the project node_modules folder. This is only
// required if Visual Studio doesn't prompt to do it automatically. 

// Using npm v3 and running 'npm dedupe' is required to avoid deployment errors caused 
// by node module paths that are too long for the target device.


var Cylon = require('cylon');

Cylon.robot({
    connections: {
        // The port name used in connections can either be a regular port name like
        // "COM1" or it can be in a format describing a USB-Serial device that has
        // not been assigned a port name. The format is:
        // USB[#|\]VID_<Vendor ID>&PID_<Product ID>[#|\]<Serial String>
        // Example:
        // USB#VID_2341&PID_0043#85436323631351311141
        // This string can be found by either:
        // 1. Calling the serialport list function.
        // 2. Running "devcon status usb*" on your Windows 10 IoT Core device (only for USB-Serial devices).
    },

    devices: {

    },

    work: function (my) {

    }
}).start();
