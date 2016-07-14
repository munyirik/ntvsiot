// Before running this code, update serialport by right-clicking on the npm node 
// (in the Solution Explorer) and selecting 'Update npm Packages'
// Doing this will enable serialport to work with this application.

var Cylon = require('cylon');

Cylon.robot({
    connections: {
        // Use the serialport list function to get the port value for your connection. 
        // If a device ID is provided as the comName, be sure to double the backslashes 
        // in the string. e.g. \\?\ should be \\\\?\\
    },

    devices: {

    },

    work: function (my) {

    }
}).start();
