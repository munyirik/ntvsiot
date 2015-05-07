/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

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
    }
}
