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
    public class BasicPropertyTests : NodejsUwpProjectTest {
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
                Keyboard.Type("--use-logger --debug");

                solution.App.Dte.ExecuteCommand("File.SaveAll");

                var projFile = File.ReadAllText(solution.Project.FullName);
                Assert.AreNotEqual(-1, projFile.IndexOf("<DebuggerMachineName>10.11.22.33</DebuggerMachineName>"));
                Assert.AreNotEqual(-1, projFile.IndexOf("<NodeExeArguments>--use-logger --debug</NodeExeArguments>"));
            }
        }
    }
}
