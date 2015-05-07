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
using System.Runtime.InteropServices;

namespace Microsoft.NodejsUwp
{
    [Guid(GuidList.NodejsUwpPropertyPageViewString)]
    partial class NodejsUwpPropertyPageView : PageView
    {
        #region Constructors
        /// <summary>
        /// This is the runtime constructor.
        /// </summary>
        /// <param name="site">Site for the page.</param>
        public NodejsUwpPropertyPageView(IPageViewSite site)
            : base(site)
        {
            InitializeComponent();
        }
        /// <summary>
        /// This constructor is only to enable winform designers
        /// </summary>
        public NodejsUwpPropertyPageView()
        {
            InitializeComponent();
        }
        #endregion

        private PropertyControlTable propertyControlTable;

        /// <summary>
        /// This property is used to map the control on a PageView object to a property
        /// in PropertyStore object.
        /// 
        /// This property will be called in the base class's constructor, which means that
        /// the InitializeComponent has not been called and the Controls have not been
        /// initialized.
        /// </summary>
		protected override PropertyControlTable PropertyControlTable
        {
            get
            {
                if (propertyControlTable == null)
                {
                    // This is the list of properties that will be persisted and their
                    // assciation to the controls.
                    propertyControlTable = new PropertyControlTable();

                    // This means that this CustomPropertyPageView object has not been
                    // initialized.
                    if (string.IsNullOrEmpty(base.Name))
                    {
                        this.InitializeComponent();
                    }

                    // Add two Property Name / Control KeyValuePairs. 
                    propertyControlTable.Add(NodejsUwpConstants.DebuggerMachineName, _debuggerMachineName);
                    propertyControlTable.Add(NodejsUwpConstants.NodeExeArguments, _nodeArguments);
                    propertyControlTable.Add(NodejsUwpConstants.ScriptArguments, _scriptArguments);
                }
                return propertyControlTable;
            }
        }
    }
}