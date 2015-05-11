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
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using TestUtilities;
using TestUtilities.UI;
using TestUtilities.NodejsUwp;
using Key = System.Windows.Input.Key;


namespace NodejsUwp.Tests {
    [TestClass]
    public sealed class BasicIntellisense : NodejsUwpProjectTest {

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void CtrlSpace() {
            var project = Project("CtrlSpace",
                Compile("server", "var http = require('http');\r\nhttp.createS")
            );

            using (var solution = project.Generate().ToVs()) {
                var server = solution.OpenItem("CtrlSpace", "server.js");

                server.MoveCaret(2, 13);

                solution.App.Dte.ExecuteCommand("Edit.CompleteWord");

                server.WaitForText("var http = require('http');\r\nhttp.createServer");
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void NodeModulesPackageJsonFolder() {
            var project = Project("ReferenceIntellisense",
                Compile("server", "require('mymod')"),
                Folder("node_modules"),
                Folder("node_modules\\mymod"),
                Folder("node_modules\\mymod\\lib"),
                Compile("node_modules\\mymod\\lib\\index", "exports.x = 42;"),
                Content("node_modules\\mymod\\package.json", "{main: './lib/index.js', name: 'mymod'}")
            );

            using (var solution = project.Generate().ToVs()) {
                var server = solution.OpenItem("ReferenceIntellisense", "server.js");

                server.MoveCaret(1, 17);

                Keyboard.Type(".x.");
                System.Threading.Thread.Sleep(3000);
                Keyboard.Type("toF\t");
                server.WaitForText("require('mymod').x.toFixed");
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SignaturesTest() {
            var project = Project("SignaturesTest",
                Compile("server", "function f(a, b, c) { }\r\n\r\n")
            );

            using (var solution = project.Generate().ToVs()) {
                var server = solution.OpenItem("SignaturesTest", "server.js");

                server.MoveCaret(3, 1);

                Keyboard.Type("f(");
                
                using (var sh = server.WaitForSession<ISignatureHelpSession>()) {
                    var session = sh.Session;
                    Assert.AreEqual("a", session.SelectedSignature.CurrentParameter.Name);
                }

                Keyboard.Backspace();
                Keyboard.Backspace();

                Keyboard.Type("new f(");

                using (var sh = server.WaitForSession<ISignatureHelpSession>()) {
                    var session = sh.Session;
                    Assert.AreEqual("a", session.SelectedSignature.CurrentParameter.Name);
                }

            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void IntellisenseAfterNewLine() {
            var project = Project("NewLineTest",
                Compile("server", "var x = 'abc';\r\n// foo\r\nx\r\nvar abc=42\r\nx")
            );

            using (var solution = project.Generate().ToVs()) {
                var server = solution.OpenItem("NewLineTest", "server.js");

                server.MoveCaret(3, 2);

                Keyboard.Type(".");

                using (var sh = server.WaitForSession<ICompletionSession>()) {
                    var session = sh.Session;
                    AssertUtil.ContainsAtLeast(session.CompletionSets[0].Completions.Select(x => x.InsertionText), "big");
                }

                server.MoveCaret(5, 2);

                Keyboard.Type(".");

                using (var sh = server.WaitForSession<ICompletionSession>()) {
                    var session = sh.Session;
                    AssertUtil.ContainsAtLeast(session.CompletionSets[0].Completions.Select(x => x.InsertionText), "big");
                }
            }
        }
    }
}
