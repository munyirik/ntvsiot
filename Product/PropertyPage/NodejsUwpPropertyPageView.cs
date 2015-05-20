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