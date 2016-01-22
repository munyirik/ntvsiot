/*
    Copyright(c) Microsoft Open Technologies, Inc. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions :

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Microsoft.NodejsUwp
{
    /// <summary>
    /// Enables serialport (https://www.npmjs.com/package/serialport) to be used with a Node.js UWP application.
    /// </summary>
    [NpmHandler]
    class SerialportNpmHandler : INpmHandler
    {
        private string ProjectPath { get; set; }
        private string ZipFilePath { get; set; }
        private IVsOutputWindowPane NpmOutputPane { get; set; }
        private string Platform { get; set; }
        private const string NODE_MODULE_VERSION = "v47";

        public Uri GetPatchUri()
        {
            // Source code from: https://github.com/ms-iot/node-serialport/tree/uwp
            return new Uri("http://aka.ms/spc_zip");
        }

        public void UpdatePackage(string projPath, IVsOutputWindowPane pane, string platform)
        {
            // Check in serialport is installed
            if(!Directory.Exists(projPath + "\\node_modules\\serialport"))
            {
                return;
            }

            ProjectPath = projPath + "\\";
            ZipFilePath = ProjectPath + "serialportpatch.zip";
            NpmOutputPane = pane;
            Platform = platform;

            WebClient client = new WebClient();
            Uri uri = GetPatchUri();
            
            NodejsUwpNpmHandler.PrintOutput(string.Format(CultureInfo.CurrentCulture, "Downloading serialport UWP patch from {0}", 
                GetPatchUri().ToString()), NpmOutputPane);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadPatchCompleted);
            client.DownloadFileAsync(uri, ZipFilePath);
        }

        private void DownloadPatchCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                NodejsUwpNpmHandler.PrintOutput(e.Error.Message, NpmOutputPane);
                return;
            }

            NodejsUwpNpmHandler.PrintOutput("Download complete.", NpmOutputPane);

            NodejsUwpNpmHandler.PrintOutput("Extracting patch...", NpmOutputPane);

            string ZipFileExtractPath = ProjectPath + "serialportpatch";
            // Clean existing extracted path
            if (Directory.Exists(ZipFileExtractPath))
            {
                DirectoryInfo di = new DirectoryInfo(ZipFileExtractPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            try
            {
                ZipFile.ExtractToDirectory(ZipFilePath, ZipFileExtractPath);
            }
            catch(Exception ex)
            {
                NodejsUwpNpmHandler.PrintOutput(string.Format(CultureInfo.CurrentCulture, "Extraction failed: {0}",
                    ex.Message), NpmOutputPane);
                return;
            }
            NodejsUwpNpmHandler.PrintOutput("Extraction complete.", NpmOutputPane);


            // Apply patch to serialport.js and serialport.node
            string aoFileSrc = ZipFileExtractPath + "\\uwp\\" + Platform + "\\serialport.node";
            string aoFileDir = ProjectPath + "\\node_modules\\serialport\\build\\Release\\node-" + NODE_MODULE_VERSION + "-win32-" + Platform;
            string aoFileDest = aoFileDir + "\\serialport.node";
            string jsFileSrc = ZipFileExtractPath + "\\uwp\\serialport.js";
            string jsFileDest = ProjectPath + "\\node_modules\\serialport\\serialport.js";

            Directory.CreateDirectory(aoFileDir);
            File.Copy(aoFileSrc, aoFileDest, true);
            File.Copy(jsFileSrc, jsFileDest, true);
            NodejsUwpNpmHandler.PrintOutput("serialport UWP patch applied!", NpmOutputPane);
        }
    }
}
