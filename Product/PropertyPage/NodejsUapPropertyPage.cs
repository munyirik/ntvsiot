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
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
//using Microsoft.VisualStudioTools;
//using Microsoft.VisualStudioTools.Project;

namespace Microsoft.NodejsUap
{
    [Guid(GuidList.NodejsUapPropertyPageControlString)]
    class NodejsUapPropertyPage : PropertyPage
    {
        #region Overriden Properties and Methods

        /// <summary>
        /// Help keyword that should be associated with the page
        /// </summary>
        protected override string HelpKeyword
        {
            // TODO: Put your help keyword here
            get { return String.Empty; }
        }

        /// <summary>
        /// Title of the property page.
        /// </summary>
        public override string Title
        {
            get { return "General"; }
        }

        /// <summary>
        /// Provide the view of our properties.
        /// </summary>
        /// <returns></returns>
        protected override IPageView GetNewPageView()
        {
            return new NodejsUapPropertyPageView(this);
        }

        /// <summary>
        /// Use a store implementation designed for flavors.
        /// </summary>
        /// <returns>Store for our properties</returns>
        protected override IPropertyStore GetNewPropertyStore()
        {
            return new NodejsUapPropertyStore();
        }

        #endregion
    }
}
