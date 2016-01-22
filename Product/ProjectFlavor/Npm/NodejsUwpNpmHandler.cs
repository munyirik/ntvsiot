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

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Microsoft.NodejsUwp
{
    /// <summary>
    /// Attribute to identify classes that are npm handlers
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class NpmHandler : System.Attribute{ }

    public class NodejsUwpNpmHandler
    {
        /// <summary>
        /// Package manager window pane GUID that ships with VS2015
        /// </summary>
        Guid VSPackageManagerPaneGuid = new Guid("C7E31C31-1451-4E05-B6BE-D11B6829E8BB");

        /// <summary>
        /// Package manager pane object
        /// </summary>
        IVsOutputWindowPane NpmOutputPane;

        /// <summary>
        /// Timeout for npm command completion in ms
        /// </summary>
        const int TIMEOUT = 20000;

        /// <summary>
        /// Prints messages to the package manager output window
        /// </summary>
        /// <param name="message">Message to print</param>
        /// <param name="pane">Pane object</param>
        public static void PrintOutput(string message, IVsOutputWindowPane pane)
        {
            if(pane != null)
            {
                ErrorHandler.ThrowOnFailure(pane.OutputString(message + "\n"));
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public NodejsUwpNpmHandler()
        {
            IVsOutputWindow output = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (output != null)
            {
                ErrorHandler.ThrowOnFailure(output.GetPane(ref VSPackageManagerPaneGuid, out NpmOutputPane));
            }
        }

        /// <summary>
        /// This method will run npm dedupe in the node_modules folder of the project
        /// </summary>
        public void RunNpmDedupe(string projPath)
        {
            Process process = new Process();

            try {
                process.StartInfo.FileName = "npm.cmd";
                process.StartInfo.Arguments = "dedupe";
                process.StartInfo.WorkingDirectory = projPath + "\\node_modules";
                PrintOutput("Running npm dedupe", this.NpmOutputPane);
                process.Start();
                process.WaitForExit(TIMEOUT);
            }
            catch(Exception e)
            {
                PrintOutput(string.Format(CultureInfo.CurrentCulture, "Running npm dedupe failed: {0}",
                    e.Message), this.NpmOutputPane);
            }
        }

        /// <summary>
        /// This method gets called when the IoT Extension detects an npm command from
        /// the base project (NTVS). It will enumerate the npm handlers in this assembly
        /// and use them to apply UWP patches to installed npm packages as required.
        /// </summary>
        /// <param name="projPath">Path of the Node.js UWP project</param>
        /// <param name="platform">x86, x64, or ARM</param>
        public void UpdateNpmPackages(string projPath, string platform)
        {
            IEnumerable<Type> handlers = GetNpmHandlers();
            foreach(Type h in handlers)
            {
                INpmHandler npmHandler = (INpmHandler)Activator.CreateInstance(h);
                npmHandler.UpdatePackage(projPath, this.NpmOutputPane, platform);
            }
        }

        /// <summary>
        /// Enumerates the npm handlers in this assembly
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Type> GetNpmHandlers()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(NpmHandler), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
