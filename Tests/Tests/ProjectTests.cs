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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using EnvDTE;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;
using TestUtilities.NodejsUwp;
using TestUtilities.UI;

namespace NodejsUwp.Tests {
    [TestClass]
    public class ProjectTests {
        [ClassInitialize]
        public static void DoDeployment(TestContext context) {
            AssertListener.Initialize();
            NodejsUwpTestData.Deploy();
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void GlobalIntellisense() {
            using (var app = new VisualStudioApp()) {
                Window window;
                var openFile = OpenProjectItem(app, "server.js", out window);

                openFile.MoveCaret(6, 1);

                Keyboard.Type("process.");
                using (var session = openFile.WaitForSession<ICompletionSession>()) {

                    var completions = session.Session.CompletionSets.First().Completions.Select(x => x.InsertionText);
                    Assert.IsTrue(completions.Contains("abort"));
                    Assert.IsTrue(completions.Contains("chdir"));
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void EnterCompletion() {
            Window window;
            using (var app = new VisualStudioApp()) {
                var openFile = OpenProjectItem(app, "server.js", out window);

                openFile.MoveCaret(7, 1);
                Keyboard.Type("http.");
                System.Threading.Thread.Sleep(5000);
                Keyboard.Type("Cli\r");
                openFile.WaitForText(@"var http = require('http');

http.createServer(function (req, res) {
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    res.end('Hello World\n');
}).listen(1337);
http.ClientRequest");
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void ModuleCompletions() {
            Window window;

            using (var app = new VisualStudioApp()) {
                var openFile = OpenProjectItem(app, "intellisensemod.js", out window);

                openFile.MoveCaret(3, 1);
                Keyboard.Type("server.");
                System.Threading.Thread.Sleep(3000);
                Keyboard.Type("lis\r");
                openFile.WaitForText(@"var http = require('http');
var server = http.createServer(null); // server.listen
server.listen

var sd = require('stringdecoder');  // sd.StringDecoder();


");

                openFile.MoveCaret(6, 1);
                Keyboard.Type("sd.");
                System.Threading.Thread.Sleep(3000);
                Keyboard.Type("Str\r");
                openFile.WaitForText(@"var http = require('http');
var server = http.createServer(null); // server.listen
server.listen

var sd = require('stringdecoder');  // sd.StringDecoder();
sd.StringDecoder

");
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void TestNewProject() {
            using (var app = new VisualStudioApp()) {
                using (var newProjDialog = app.FileNewProject()) {
                    newProjDialog.FocusLanguageNode("JavaScript");
                    var nodejsApp = newProjDialog.ProjectTypes.FindItem("Basic Node.js Web Server (Windows Universal)");
                    nodejsApp.Select();

                    newProjDialog.OK();
                }

                // wait for new solution to load...            
                for (int i = 0; i < 40 && app.Dte.Solution.Projects.Count == 0; i++) {
                    System.Threading.Thread.Sleep(250);
                }

                app.SolutionExplorerTreeView.WaitForItem(
                    "Solution '" + app.Dte.Solution.Projects.Item(1).Name + "' (1 project)",
                    app.Dte.Solution.Projects.Item(1).Name,
                    "server.js"
                );
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SetAsStartupFile() {
            using (var app = new VisualStudioApp()) {
                var project = app.OpenProject(@"TestData\HelloWorld.sln");

                // wait for solution to load...
                for (int i = 0; i < 40 && app.Dte.Solution.Projects.Count == 0; i++) {
                    System.Threading.Thread.Sleep(250);
                }

                var item = app.SolutionExplorerTreeView.WaitForItem(
                    "Solution '" + app.Dte.Solution.Projects.Item(1).Name + "' (1 project)",
                    app.Dte.Solution.Projects.Item(1).Name,
                    "JavaScript1.js"
                );

                AutomationWrapper.Select(item);
                app.Dte.ExecuteCommand("Project.SetasNode.jsStartupFile");

                string startupFile = null;
                for (int i = 0; i < 40; i++) {
                    startupFile = (string)project.Properties.Item("StartupFile").Value;
                    if (startupFile == "JavaScript1.js") {
                        break;
                    }
                    System.Threading.Thread.Sleep(250);
                }
                Assert.AreEqual(startupFile, Path.Combine(Environment.CurrentDirectory, @"TestData\HelloWorld", "JavaScript1.js"));
            }
        }

        private static EditorWindow OpenProjectItem(VisualStudioApp app, string startItem, out Window window, string projectName = @"TestData\HelloWorld.sln") {
            var project = app.OpenProject(projectName, startItem);

            return OpenItem(app, startItem, project, out window);
        }

        private static EditorWindow OpenItem(VisualStudioApp app, string startItem, Project project, out Window window) {
            EnvDTE.ProjectItem item = null;
            if (startItem.IndexOf('\\') != -1) {
                var items = project.ProjectItems;
                foreach (var itemName in startItem.Split('\\')) {
                    Console.WriteLine(itemName);
                    item = items.Item(itemName);
                    items = item.ProjectItems;
                }
            } else {
                item = project.ProjectItems.Item(startItem);
            }

            Assert.IsNotNull(item);

            window = item.Open();
            window.Activate();
            return app.GetDocument(item.Document.FullName);
        }

        internal static Project OpenProjectAndRun(VisualStudioApp app, string projName, string filename, bool setStartupItem = true, bool debug = true) {
            var project = app.OpenProject(projName, filename, setStartupItem: setStartupItem);

            if (debug) {
                app.Dte.ExecuteCommand("Debug.Start");
            } else {
                app.Dte.ExecuteCommand("Debug.StartWithoutDebugging");
            }

            return project;
        }
    }
}
