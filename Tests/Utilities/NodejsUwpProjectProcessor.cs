//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************//

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
