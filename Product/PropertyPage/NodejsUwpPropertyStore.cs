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
