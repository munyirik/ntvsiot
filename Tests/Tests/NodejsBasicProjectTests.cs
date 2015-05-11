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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudioTools;
using TestUtilities;
using TestUtilities.UI;
using TestUtilities.NodejsUwp;

namespace NodejsUwp.Tests {
    [TestClass]
    public class NodejsBasicProjectTests : NodejsUwpProjectTest {
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void TestProjectProperties() {
            var filename = Path.Combine(TestData.GetTempPath(), Path.GetRandomFileName());

            var project = Project("ProjectProperties",
                Compile("server"),
                Property("StartupFile", "server.js")
            );

            using (var solution = project.Generate().ToVs()) {
                var projectNode = solution.WaitForItem("ProjectProperties");
                AutomationWrapper.Select(projectNode);

                solution.App.Dte.ExecuteCommand("ClassViewContextMenus.ClassViewMultiselectProjectReferencesItems.Properties");
                AutomationElement doc = null;
                for (int i = 0; i < 10; i++) {
                    doc = solution.App.GetDocumentTab("ProjectProperties");
                    if (doc != null) {
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                Assert.IsNotNull(doc, "Failed to find project properties tab");

                var nodeExeArguments = new TextBox(
                        new AutomationWrapper(doc).FindByAutomationId("_nodeArguments")
                    );
                var debuggerMachineName = new TextBox(
                    new AutomationWrapper(doc).FindByAutomationId("_debuggerMachineName")
                );

                debuggerMachineName.SetFocus();
                Keyboard.ControlA();
                Keyboard.Backspace();
                Keyboard.Type("10.11.22.33");

                nodeExeArguments.SetFocus();
                Keyboard.ControlA();
                Keyboard.Backspace();
                Keyboard.Type("--no-console --debug");

                solution.App.Dte.ExecuteCommand("File.SaveAll");

                var projFile = File.ReadAllText(solution.Project.FullName);
                Assert.AreNotEqual(-1, projFile.IndexOf("<DebuggerMachineName>10.11.22.33</DebuggerMachineName>"));
                Assert.AreNotEqual(-1, projFile.IndexOf("<NodeExeArguments>--no-console --debug</NodeExeArguments>"));
            }
        }
    }
}
