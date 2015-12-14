// !!BEFORE YOU RUN THIS CODE!!

// 1. Follow the steps in the "Build Serialport" section in the link below:
// http://ms-iot.github.io/content/en-US/win10/samples/NodejsWUJ5.htm
// Even though serialport (a Johnny-Five dependency) is installed when a new 
// Johnny-Five project is created, it needs to be rebuilt so that:
//   - It matches the processor architecture of the device you are targeting.
//   - It is UWP (Universal Windows Platform) compatible.
// The link above also has a sample code and steps to run Johnny-Five on a
// Raspberry Pi 2 to control a servo connected to an Arduino.
// Go to http://johnny-five.io/ for Johnny-Five documentation and more samples.

// 2. Run 'npm dedupe' (in a cmd window) in the project node_modules folder. This is only
// required if Visual Studio doesn't prompt to do it automatically. 
// Using 'npm dedupe' (and npm v3+) is required to avoid deployment errors caused 
// by node module paths that are too long for the target device.

var five = require("johnny-five");
var board = new five.Board();

board.on("ready", function () {

});