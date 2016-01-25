// Before running this code, update serialport by right-clicking on the npm node 
// (in the Solution Explorer) and selecting 'Update npm Packages'
// Doing this will enable serialport to work with this application.

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
