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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;
using TestUtilities.NodejsUwp;
using TestUtilities.UI;
using ST = System.Threading;

namespace NodejsUwp.Tests {
    [TestClass]
    public class BasicProjectTests {
        [ClassInitialize]
        public static void DoDeployment(TestContext context) {
            AssertListener.Initialize();
            NodejsUwpTestData.Deploy();
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void LoadNodejsProject() {
            using (var app = new VisualStudioApp()) {
                app.OpenProject(@"TestData\HelloWorld.sln");

                Assert.IsTrue(app.Dte.Solution.IsOpen, "The solution is not open");
                Assert.IsTrue(app.Dte.Solution.Projects.Count == 1, String.Format("Loading project resulted in wrong number of loaded projects, expected 1, received {0}", app.Dte.Solution.Projects.Count));

                var iter = app.Dte.Solution.Projects.GetEnumerator();
                iter.MoveNext();
                Project project = (Project)iter.Current;
                Assert.AreEqual("HelloWorld.njsproj", Path.GetFileName(project.FileName), "Wrong project file name");
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void ProjectBuild() {
            using (var app = new VisualStudioApp())
            {
                var project = app.OpenProject(@"TestData\HelloWorld.sln");

                app.Dte.Solution.SolutionBuild.Build(true);

                string[] expectedOutputFiles = { "AppxManifest.xml", "HelloWorld.build.appxrecipe", "resources.pri", "startupinfo.xml" };

                string currentDir = Directory.GetCurrentDirectory();

                foreach (string s in expectedOutputFiles)
                {
                    Assert.AreEqual(true, File.Exists(string.Format("{0}\\TestData\\HelloWorld\\bin\\{1}", currentDir, s)),
                        string.Format("{0} is missing from bin output folder", s));
                }

                // TODO: 
                // Add test for rebuild
                // Add test to build all configs and platforms
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void ProjectConfiguration() {
            using (var app = new VisualStudioApp()) {
                var project = app.OpenProject(@"TestData\HelloWorld.sln");

                string[] expectedPlatforms = { "x86", "x64", "ARM" };
                string[] expectedConfigs = { "Debug", "Release" };

                var debug = project.ConfigurationManager.Item("Debug", "x86");
                Assert.AreEqual(debug.IsBuildable, true);

                // Check platforms
                for (int i = 0; i < expectedPlatforms.Length; i++)
                {
                    Assert.AreEqual(expectedPlatforms[i], ((object[])project.ConfigurationManager.PlatformNames)[i]);
                }

                // Check configurations
                for (int i = 0; i < expectedConfigs.Length; i++)
                {
                    Assert.AreEqual(expectedConfigs[i], ((object[])project.ConfigurationManager.ConfigurationRowNames)[i]);
                }
            }
        }

        private static void ToSTA(ST.ThreadStart code) {
            ST.Thread t = new ST.Thread(code);
            t.SetApartmentState(ST.ApartmentState.STA);
            t.Start();
            t.Join();
        }

        private static ProjectItem WaitForItem(Project project, string name) {
            bool found = false;
            ProjectItem item = null;
            for (int i = 0; i < 40; i++) {
                try {
                    item = project.ProjectItems.Item(name);
                    if (item != null) {
                        found = true;
                        break;
                    }
                } catch (ArgumentException) {
                }
                // wait for the edit to complete
                System.Threading.Thread.Sleep(250);
            }
            Assert.IsTrue(found);
            return item;
        }
    }
}
