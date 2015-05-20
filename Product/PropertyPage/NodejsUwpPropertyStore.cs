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
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.NodejsUwp
{ 
    public class NodejsUwpPropertyStore : IDisposable, IPropertyStore
    {
        private bool disposed = false;

        private List<NodejsUwpProjectFlavorCfg> configs =
            new List<NodejsUwpProjectFlavorCfg>();

        public event StoreChangedDelegate StoreChanged;

        #region IPropertyStore Members
        /// <summary>
        /// Use the data passed in to initialize the Properties. 
        /// </summary>
        /// <param name="dataObject">
        /// This is normally only one our configuration object, which means that 
        /// there will be only one elements in configs.
        /// If it is null, we should release it.
        /// </param>
        public void Initialize(object[] dataObjects)
        {
            // If we are editing multiple configuration at once, we may get multiple objects.
            foreach (object dataObject in dataObjects)
            {
                if (dataObject is IVsCfg)
                {
                    // This should be our configuration object, so retrive the specific
                    // class so we can access its properties.
                    NodejsUwpProjectFlavorCfg config = NodejsUwpProjectFlavorCfg
                        .GetPropertyPageFromIVsCfg((IVsCfg)dataObject);

                    if (!configs.Contains(config))
                    {
                        configs.Add(config);
                    }
                }
            }
        }

        /// <summary>
        /// Set the value of the specified property in storage.
        /// </summary>
        /// <param name="propertyName">Name of the property to set.</param>
        /// <param name="propertyValue">Value to set the property to.</param>
        public void Persist(string propertyName, string propertyValue)
        {
            // If the value is null, make it empty.
            if (propertyValue == null)
            {
                propertyValue = String.Empty;
            }

            foreach (NodejsUwpProjectFlavorCfg config in configs)
            {
                // Set the property
                config[propertyName] = propertyValue;
            }
            if (StoreChanged != null)
            {
                StoreChanged();
            }
        }

        /// <summary>
        /// Retreive the value of the specified property from storage
        /// </summary>
        /// <param name="propertyName">Name of the property to retrieve</param>
        /// <returns></returns>
        public string PropertyValue(string propertyName)
        {
            string value = null;
            if (configs.Count > 0)
                value = configs[0][propertyName];
            foreach (NodejsUwpProjectFlavorCfg config in configs)
            {
                if (config[propertyName] != value)
                {
                    // multiple config with different value for the property
                    value = String.Empty;
                    break;
                }
            }

            return value;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Protect from being called multiple times.
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                configs.Clear();
            }
            disposed = true;
        }
        #endregion
    }
}
