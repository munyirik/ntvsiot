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

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.NodejsUwp
{
    [Guid(GuidList.NodejsUwpProjectFlavorString)]
    class NodejsUwpProjectFlavor : 
        FlavoredProjectBase,
        IVsProjectFlavorCfgProvider
    {
        internal NodejsUwpPackage _package;
        private IVsProjectFlavorCfgProvider _innerVsProjectFlavorCfgProvider = null;

        protected override void Close()
        {
            base.Close();
            if (_innerVsProjectFlavorCfgProvider != null)
            {
                if (Marshal.IsComObject(_innerVsProjectFlavorCfgProvider))
                {
                    Marshal.ReleaseComObject(_innerVsProjectFlavorCfgProvider);
                }
                _innerVsProjectFlavorCfgProvider = null;
            }
        }

        protected override void SetInnerProject(IntPtr innerIUnknown)
        {
            object objectForIUnknown = null;
            objectForIUnknown = Marshal.GetObjectForIUnknown(innerIUnknown);
            if (base.serviceProvider == null)
            {
                base.serviceProvider = _package;
            }
            base.SetInnerProject(innerIUnknown);
            _innerVsProjectFlavorCfgProvider = objectForIUnknown as IVsProjectFlavorCfgProvider;
        }

        #region IVsProjectFlavorCfgProvider Members

        public int CreateProjectFlavorCfg(IVsCfg pBaseProjectCfg, out IVsProjectFlavorCfg ppFlavorCfg)
        {
            IVsProjectFlavorCfg cfg;
            ErrorHandler.ThrowOnFailure(
                _innerVsProjectFlavorCfgProvider.CreateProjectFlavorCfg(
                    pBaseProjectCfg,
                    out cfg
                )
            );

            ppFlavorCfg = new NodejsUwpProjectFlavorCfg(this, pBaseProjectCfg, cfg);
            return VSConstants.S_OK;
        }

        #endregion

        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            switch ((__VSHPROPID2)propId)
            {
                case __VSHPROPID2.VSHPROPID_CfgPropertyPagesCLSIDList:
                    {
                        var res = base.GetProperty(itemId, propId, out property);
                        
                        property += ';' + typeof(NodejsUwpPropertyPage).GUID.ToString("B");

                        property = RemovePropertyPagesFromList((string)property, CfgSpecificPropertyPagesToRemove);
                        return res;
                    }
                case __VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList:
                    {
                        var res = base.GetProperty(itemId, propId, out property);
                        property = RemovePropertyPagesFromList((string)property, PropertyPagesToRemove);
                        return res;
                    }
            }

            return base.GetProperty(itemId, propId, out property);
        }

        internal static string[] CfgSpecificPropertyPagesToRemove = new[] {
            "{A553AD0B-2F9E-4BCE-95B3-9A1F7074BC27}",   // Package/Publish Web 
            "{9AB2347D-948D-4CD2-8DBE-F15F0EF78ED3}",   // Package/Publish SQL 
        };

        internal static string[] PropertyPagesToRemove = new[] {
            "{8C0201FE-8ECA-403C-92A3-1BC55F031979}",   // typeof(DeployPropertyPageComClass)
            "{ED3B544C-26D8-4348-877B-A1F7BD505ED9}",   // typeof(DatabaseDeployPropertyPageComClass)
            "{909D16B3-C8E8-43D1-A2B8-26EA0D4B6B57}",   // Microsoft.VisualStudio.Web.Application.WebPropertyPage
            "{379354F2-BBB3-4BA9-AA71-FBE7B0E5EA94}",   // Microsoft.VisualStudio.Web.Application.SilverlightLinksPage
            "{62E8E091-6914-498E-A47B-6F198DC1873D}"    // Base Node.js project
        };

        internal string RemovePropertyPagesFromList(string propertyPagesList, string[] pagesToRemove)
        {
            if (pagesToRemove != null)
            {
                propertyPagesList = propertyPagesList.ToUpper(CultureInfo.InvariantCulture);
                foreach (string s in pagesToRemove)
                {
                    int index = propertyPagesList.IndexOf(s, StringComparison.Ordinal);
                    if (index != -1)
                    {
                        // Guids are separated by ';' so if we remove the last one also remove the last ';'
                        int index2 = index + s.Length + 1;
                        if (index2 >= propertyPagesList.Length)
                            propertyPagesList = propertyPagesList.Substring(0, index).TrimEnd(';');
                        else
                            propertyPagesList = propertyPagesList.Substring(0, index) + propertyPagesList.Substring(index2);
                    }
                }
            }
            return propertyPagesList;
        }

        protected override int QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            // If we detect an npm update command from the base project (NTVS), we will:
            // 1. Run "npm dedupe" to reduce chances of deploying packages with
            //    paths that are too long.
            // 2. Patch native addons with UWP versions of the addons if they exist.
            // For #2, eventually a better solution may be to have npm and the base project 
            // handle this. i.e. get to the point where "npm install serialport --winplat=uwp" 
            // is supported.
            // For now this solution works well since we do not need to modify the base project.

            if (pguidCmdGroup == GuidList.NodejsNpmCmdSet && 
                prgCmds[0].cmdID == 0x302) // 0x302 is the value of cmdidNpmUpdateModules in https://github.com/Microsoft/nodejstools/blob/master/Nodejs/Product/Nodejs/PkgCmdId.cs
            {              
                EnvDTE.DTE dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
                Array projs = (Array)dte.ActiveSolutionProjects;
                if(projs.Length > 0)
                {
                    EnvDTE.Project activeProj = (EnvDTE.Project)projs.GetValue(0);
                    string projectPath = activeProj.FullName.Substring(0, activeProj.FullName.LastIndexOf('\\'));
                    NpmHandler npmHandler = new NpmHandler();

                    // Run npm dedupe
                    npmHandler.RunNpmDedupe(projectPath);

                    // Patch packages
                    npmHandler.UpdateNpmPackages(projectPath);
                }
            }

            return base.QueryStatusCommand(itemid, ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
