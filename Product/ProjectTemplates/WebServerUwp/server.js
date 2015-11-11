// !!BEFORE YOU RUN THIS CODE!!

// Connect to your device using PowerShell or SSH (see http://windowsondevices.com for a guide to do that) and run:
// reg.exe ADD "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\EmbeddedMode\ProcessLauncher" /v AllowedExecutableFilesList /t REG_MULTI_SZ /d "c:\windows\system32\xcopy.exe\0"
// This step is required to successfully deploy this app.

var http = require('http');

http.createServer(function (req, res) {
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    res.end('Hello World\n');
}).listen(1337);