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
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudioTools;
using TestUtilities;
using TestUtilities.SharedProject;
using TestUtilities.UI;
using TestUtilities.NodejsUwp;
using Keyboard = TestUtilities.UI.Keyboard;

namespace NodejsUwp.Tests {
    [TestClass]
    public class RequireIntellisense : NodejsUwpProjectTest {
        public static ProjectDefinition BasicProject = RequireProject(
            Compile("server", ""),
            Compile("myapp"),

            Folder("node_modules"),
            Folder("node_modules\\Foo"),
            Compile("node_modules\\quox", ""),

            Content("node_modules\\Foo\\package.json", ""),

            Folder("SomeFolder"),
            Compile("SomeFolder\\baz", "")
        );

        [ClassInitialize]
        public static void DoDeployment(TestContext context) {
            AssertListener.Initialize();
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void BasicRequireCompletions() {
            using (var solution = BasicProject.Generate().ToVs()) {
                var server = solution.OpenItem("Require", "server.js");
                Keyboard.Type("require(");

                using (var completionSession = server.WaitForSession<ICompletionSession>()) {
                    Assert.AreEqual(1, completionSession.Session.CompletionSets.Count);

                    // we pick up built-ins, folders w/ package.json, and peers
                    AssertUtil.ContainsAtLeast(
                        completionSession.Session.GetDisplayTexts(),
                        "http",
                        "./myapp.js",
                        "./SomeFolder/baz.js",
                        "quox.js"
                    );

                    // we don't show our own file
                    AssertUtil.DoesntContain(completionSession.Session.GetDisplayTexts(), "./server.js");

                    AssertUtil.ContainsAtLeast(
                        completionSession.Session.GetInsertionTexts(),
                        "'http'",
                        "'./myapp.js'",
                        "'./SomeFolder/baz.js'",
                        "'quox.js'"
                    );

                    Keyboard.Type("htt");
                    server.WaitForText("require(htt)");

                    // we should be filtered down
                    AssertUtil.ContainsExactly(
                        completionSession.Session.GetDisplayTexts(),
                        "http",
                        "https"
                    );

                    // this should trigger completion
                    Keyboard.Type(")");
                    server.WaitForText("require('http')");
                }

                Keyboard.Backspace(8);
                server.WaitForText("require");

                Keyboard.Type("(");
                using (var completionSession = server.WaitForSession<ICompletionSession>()) {
                    Assert.AreEqual(1, completionSession.Session.CompletionSets.Count);

                    // this should dismiss the session and not complete anything
                    Keyboard.Type("'".ToString());
                    server.WaitForText("require('')");

                    Assert.IsTrue(completionSession.Session.IsDismissed);
                }

                Keyboard.Backspace(2);
                server.WaitForText("require");

                Keyboard.Type("(");
                using (var completionSession = server.WaitForSession<ICompletionSession>()) {
                    Assert.AreEqual(1, completionSession.Session.CompletionSets.Count);

                    // this should dismiss the session and not complete anything
                    Keyboard.Type("\"".ToString());
                    server.WaitForText("require(\"\")");

                    Assert.IsTrue(completionSession.Session.IsDismissed);
                }
            }
        }

        private static void TypeRequireStatementAndSelectModuleName(bool doubleQuotes, EditorWindow window) {
            Keyboard.Type(string.Format("require({0}ab{0})", doubleQuotes ? "\"" : "'"));
            window.MoveCaret(1, 10);
            window.Select(1, 10, 2);
            Keyboard.Type(Keyboard.CtrlSpace.ToString());
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RequireBuiltinModules() {
            using (var solution = BasicProject.Generate().ToVs()) {
                var server = solution.OpenItem("Require", "server.js");
                Keyboard.Type("require(");

                using (var completionSession = server.WaitForSession<ICompletionSession>()) {
                    Assert.AreEqual(1, completionSession.Session.CompletionSets.Count);

                    // we pick up built-ins, folders w/ package.json, and peers
                    AssertUtil.ContainsAtLeast(
                        completionSession.Session.GetDisplayTexts(),
                        "http",
                        "timers",
                        "module",
                        "addons",
                        "util",
                        "tls",
                        "path",
                        "fs",
                        "https",
                        "url",
                        "assert",
                        "child_process",
                        "zlib",
                        "os",
                        "cluster",
                        "tty",
                        "vm"
                    );
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void UserModuleInFolder() {
            using (var solution = BasicProject.Generate().ToVs()) {
                var server = solution.OpenItem("Require", "server.js");
                Keyboard.Type("require(");

                using (var completionSession = server.WaitForSession<ICompletionSession>()) {
                    Assert.AreEqual(1, completionSession.Session.CompletionSets.Count);

                    Keyboard.Type("./Some\t)");

                    server.WaitForText("require('./SomeFolder/baz.js')");
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddModuleExternally() {
            using (var solution = BasicProject.Generate().ToVs()) {
                var server = solution.OpenItem("Require", "server.js");

                File.WriteAllText(
                    Path.Combine(solution.Directory, "Require", "node_modules", "blah.js"),
                    "exports = function(a,b,c) { }"
                );

                System.Threading.Thread.Sleep(3000);

                Keyboard.Type("require(");

                using (var completionSession = server.WaitForSession<ICompletionSession>()) {
                    Assert.AreEqual(1, completionSession.Session.CompletionSets.Count);
                    AssertUtil.ContainsAtLeast(
                        completionSession.Session.GetDisplayTexts(),
                        "blah.js"
                    );
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SubmodulesFiles() {
            var project = Project("RequireSubmodules",
                Compile("server", ""),
                Folder("mymod"),
                Compile("mymod\\index", ""),
                Compile("mymod\\quox", "")
            );
            using (var solution = project.Generate().ToVs()) {
                var server = solution.OpenItem("RequireSubmodules", "server.js");

                server.MoveCaret(1, 1);

                Keyboard.Type("require('./mymod/q\t");
                server.WaitForText("require('./mymod/quox.js')");
            }
        }

        private static ProjectDefinition RequireProject(params ProjectContentGenerator[] items) {
            return new ProjectDefinition("Require", NodejsProject, items);
        }
    }
}
