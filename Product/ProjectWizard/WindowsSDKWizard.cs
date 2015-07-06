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

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TemplateWizard;
using Microsoft.VisualStudioTools;
using Microsoft.Win32;

namespace Microsoft.NodejsUwp.ProjectWizard
{
    public sealed class WindowsSDKWizard : IWizard
    {
        public void ProjectFinishedGenerating(EnvDTE.Project project) {}
        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem) { }
        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem) { }
        public void RunFinished() { }

        public void RunStarted(
            object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind,
            object[] customParams)
        {
            string winSDKVersion = string.Empty;
            try
            {
                string keyValue = string.Empty;
                // Attempt to get the installation folder of the Windows 10 SDK
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows Kits\Installed Roots");
                if (null != key)
                {
                    keyValue = (string)key.GetValue("KitsRoot10") + "Include";
                }
                // Get the latest SDK version from the name of the directory in the Include path of the SDK installation
                if (!string.IsNullOrEmpty(keyValue))
                {
                    string dirName = Directory.GetDirectories(keyValue, "10.*").OrderByDescending(x => x).FirstOrDefault();
                    winSDKVersion = Path.GetFileName(dirName);
                }
            } catch(Exception ex)
            {
                if (ex.IsCriticalException())
                {
                    throw;
                }
            }
            
            if(string.IsNullOrEmpty(winSDKVersion))
            {
                winSDKVersion = "10.0.0.0"; // Default value to put in project file
            }

            replacementsDictionary.Add("$winsdkversion$", winSDKVersion);
            replacementsDictionary.Add("$winsdkminversion$", winSDKVersion);
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}