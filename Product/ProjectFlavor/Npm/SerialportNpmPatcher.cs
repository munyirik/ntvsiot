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
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.NodejsUwp
{
    /// <summary>
    /// Enables serialport (https://www.npmjs.com/package/serialport) to be used with a Node.js UWP application.
    /// Source code from: https://github.com/ms-iot/node-serialport/tree/uwp
    /// </summary>
    [NpmPatcherAttribute]
    class SerialportNpmPatcher : INpmPatcher
    {
        private const string NODE_MODULE_VERSION = "v47";
        private const string Name = "serialport";
        private const string PatchUri = "http://aka.ms/spc_zip";

        public void UpdatePackage(string projPath, IVsOutputWindowPane pane, string platform)
        {
            Dictionary<string, string> patchMap = new Dictionary<string, string>();
            patchMap.Add(string.Format(CultureInfo.CurrentCulture, "\\uwp\\{0}\\serialport.node", platform), string.Format(CultureInfo.CurrentCulture,
                "\\node_modules\\serialport\\build\\Release\\node-{0}-win32-{1}\\serialport.node", NODE_MODULE_VERSION, platform));
            patchMap.Add("\\uwp\\serialport.js", "\\node_modules\\serialport\\serialport.js");

            NpmPatcher npmPatcher = new NpmPatcher();
            npmPatcher.UpdatePackage(new Uri(PatchUri), projPath, pane, platform, Name, patchMap);
        }
    }
}
