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

namespace NodejsUwp.Tests {
    [TestClass]
    public class ReferenceTests {

        [ClassInitialize]
        public static void DoDeployment(TestContext context) {
            AssertListener.Initialize();
            NodejsUwpTestData.Deploy();
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddReferenceAndBuild() {
            using (var app = new VisualStudioApp()) {
                var project = app.OpenProject(@"TestData\ProjectReference.sln");

                TargetInfo ti = TargetInfo.GetTargetInfo();

                // Wait for solution to load...
                for (int i = 0; i < 40 && app.Dte.Solution.Projects.Count == 0; i++) {
                    System.Threading.Thread.Sleep(250);
                }

                Assert.IsFalse(0 == app.Dte.Solution.Projects.Count);

                // Set platform
                foreach (SolutionConfiguration2 solConfiguration2 in app.Dte.Solution.SolutionBuild.SolutionConfigurations) {
                    if (String.Equals(solConfiguration2.PlatformName, ti.Plat, StringComparison.Ordinal)) {
                        solConfiguration2.Activate();
                        break;
                    }
                }

                // Build project
                app.Dte.Solution.SolutionBuild.Build(true);

                // Check for C# reference in the appxrecipe file
                string appxRecipePath = string.Format("{0}\\TestData\\ProjectReference\\bin\\{1}\\Debug\\ProjectReference.build.appxrecipe", 
                    Directory.GetCurrentDirectory(), ti.Plat);

                Assert.AreEqual(true, File.Exists(appxRecipePath), string.Format("ProjectReference.build.appxrecipe is missing from bin output folder"));

                string appxRecipeStr = File.ReadAllText(appxRecipePath);

                Assert.IsTrue(appxRecipeStr.Contains("CSComponent"));
            }
        }
    }
}
