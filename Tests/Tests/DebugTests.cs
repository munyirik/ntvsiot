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
using EnvDTE80;

namespace NodejsUwp.Tests
{
    class TargetInfo
    {
        public string IP { get; set; }

        public string Plat { get; set; }
    }

    [TestClass]
    public class DebugTests : NodejsUwpProjectTest
    {
        [ClassInitialize]
        public static void DoDeployment(TestContext context)
        {
            AssertListener.Initialize();
            NodejsUwpTestData.Deploy();
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void Breakpoint()
        {
            using (var app = new VisualStudioApp())
            {
                var project = app.OpenProject(@"TestData\HelloWorld.sln");

                TargetInfo ti = GetTargetInfo();

                // Wait for solution to load...
                for (int i = 0; i < 40 && app.Dte.Solution.Projects.Count == 0; i++)
                {
                    System.Threading.Thread.Sleep(250);
                }

                Assert.IsFalse(0 == app.Dte.Solution.Projects.Count);

                // Set platform
                foreach (SolutionConfiguration2 solConfiguration2 in app.Dte.Solution.SolutionBuild.SolutionConfigurations)
                {
                    if(String.Equals(solConfiguration2.PlatformName, ti.Plat, StringComparison.Ordinal))
                    {
                        solConfiguration2.Activate();
                        break;
                    }
                }

                // Open project properties
                var item = app.SolutionExplorerTreeView.WaitForItem(
                    "Solution '" + app.Dte.Solution.Projects.Item(1).Name + "' (1 project)",
                    app.Dte.Solution.Projects.Item(1).Name
                );
                AutomationWrapper.Select(item);

                app.Dte.ExecuteCommand("ClassViewContextMenus.ClassViewMultiselectProjectReferencesItems.Properties");

                AutomationElement doc = null;
                for (int i = 0; i < 10; i++)
                {
                    doc = app.GetDocumentTab("HelloWorld");
                    if (doc != null)
                    {
                        break;
                    }
                    doc = app.GetDocumentTab("HelloWorld.njsproj");
                    if (doc != null)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }

                Assert.IsNotNull(doc, "Failed to find project properties tab");

                // Enter IP address of target machine
                var debuggerMachineName = new TextBox(
                    new AutomationWrapper(doc).FindByAutomationId("_debuggerMachineName")
                );
                debuggerMachineName.SetFocus();
                Keyboard.ControlA();
                Keyboard.Backspace();
                Keyboard.Type(ti.IP);
                app.Dte.ExecuteCommand("File.SaveAll");

                // Build project
                app.Dte.Solution.SolutionBuild.Build(true);

                // Add breakpoint
                app.Dte.Debugger.Breakpoints.Add(String.Empty, "server.js", 3, 1, String.Empty,
                    dbgBreakpointConditionType.dbgBreakpointConditionTypeWhenTrue, String.Empty,
                    String.Empty, 1, String.Empty, 1, dbgHitCountType.dbgHitCountTypeNone);

                // F5
                app.Dte.ExecuteCommand("Debug.Start");

                // Check that breakpoint is hit
                app.WaitForMode(dbgDebugMode.dbgBreakMode, 120);
                Assert.IsTrue(app.Dte.ActiveDocument.Name.Contains("server.js"));
                Assert.IsTrue((app.Dte.ActiveDocument.Object("TextDocument") as TextDocument).Selection.ActivePoint.Line == 3);
            }
        }

        TargetInfo GetTargetInfo()
        {
            TargetInfo ti = new TargetInfo();
            ti.IP = Environment.GetEnvironmentVariable("IOT_TARGET_IP_ADDRESS").Trim();
            ti.Plat = Environment.GetEnvironmentVariable("IOT_TARGET_PLATFORM").Trim();
            return ti;
        }
    }
}
