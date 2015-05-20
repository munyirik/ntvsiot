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

using System.ComponentModel.Composition;
using TestUtilities.SharedProject;
using MSBuild = Microsoft.Build.Evaluation;

namespace TestUtilities.NodejsUwp {
    [Export(typeof(IProjectProcessor))]
    [ProjectExtension(".njsproj")]
    public class NodejsUwpProjectProcessor : IProjectProcessor {
        public void PreProcess(MSBuild.Project project) {
            if (project.ProjectFileLocation.File.EndsWith(".user", System.StringComparison.OrdinalIgnoreCase)) {
                return;
            }

            project.SetProperty("ProjectTypeGuids", "{00251F00-BA30-4CE4-96A2-B8A1085F37AA};{3AF33F2E-1136-4D97-BBB7-1795711AC8B8};{9092AA53-FB77-4645-B42D-1CCCA6BD08BD}");
            project.SetProperty("VisualStudioVersion", "14.0");
            project.Xml.AddProperty("VisualStudioVersion", "14.0").Condition = "'$(VisualStudioVersion)' == ''";
            project.Xml.AddProperty("VSToolsPath", "$(MSBuildExtensionsPath32)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)").Condition = "'$(VSToolsPath)' == ''";
            project.Xml.AddImport("$(VSToolsPath)\\Node.js Tools\\Microsoft.NodejsUwp.targets");
            project.SetProperty("ProjectHome", ".");
            project.SetProperty("ProjectView", "ShowAllFiles");
            project.SetProperty("OutputPath", ".");
            project.SetProperty("AppContainerApplication", "true");
            project.SetProperty("ApplicationType", "Windows Store");
            project.SetProperty("OutpApplicationTypeRevisionutPath", "8.2");
            project.SetProperty("AppxPackage", "true");
            project.SetProperty("WindowsAppContainer", "true");
            project.SetProperty("RemoteDebugEnabled", "true");
            project.SetProperty("PlatformAware", "true");
            project.SetProperty("AvailablePlatforms", "x86,x64,ARM");
        }

        public void PostProcess(MSBuild.Project project) {
        }
    }
}
