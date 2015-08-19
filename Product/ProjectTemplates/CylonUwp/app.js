// !!BEFORE YOU RUN THIS CODE!!
// Follow the steps in the "Build Serialport" section in the link below:
// http://ms-iot.github.io/content/en-US/win10/samples/NodejsWUCylon.htm
// Even though serialport (a Cylon dependency) is installed when a new 
// Cylon project is created, it needs to be rebuilt so that:
// 1. It matches the processor architecture of the device you are targeting.
// 2. It is UWP (Universal Windows Platform) compatible.

var Cylon = require('cylon');

Cylon.robot({
    connections: {
        // Uncomment the "arduino:" line below after you replace the port name.
        // The name can either be a regular port name like "COM1" or "UART2" or
        // it can be in a format describing a USB-Serial device that has not been 
        // assigned a port name. The format is:
        // USB[#|\]VID_<Vendor ID>&PID_<Product ID>[#|\]<Serial String>
        // Example:
        // USB#VID_2341&PID_0043#85436323631351311141
        // This string can be found by either:
        // 1. Calling the serialport list function.
        // 2. Running "devcon status usb*" on your Windows 10 IoT Core device (only for USB-Serial devices).
        //arduino: { adaptor: 'firmata', port: 'replace me' }
    },

    devices: {
        servo: { driver: 'servo', pin: 3 }
    },

    // The "work" will move the servo from angle 45 to 90 to 135.
    work: function (my) {
        var angle = 45;
        my.servo.angle(angle);
        every((1).second(), function () {
            angle = angle + 45;
            if (angle > 135) {
                angle = 45
            }
            my.servo.angle(angle);
        });
    }
}).start();