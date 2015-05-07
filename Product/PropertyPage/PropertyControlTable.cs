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

using System.Collections.Generic;
using System.Windows.Forms;

namespace Microsoft.NodejsUwp
{
    public class PropertyControlTable
    {

        // With these two dictionaries, it is more quick to find a Control or Property Name. 
        private Dictionary<Control, string> controlNameIndex = new Dictionary<Control, string>();
        private Dictionary<string, Control> propertyNameIndex = new Dictionary<string, Control>();

        /// <summary>
        /// Add a Key Value Pair to the dictionaries.
        /// </summary>
        public void Add(string propertyName, Control control)
        {
            this.controlNameIndex.Add(control, propertyName);
            this.propertyNameIndex.Add(propertyName, control);
        }

        /// <summary>
        /// Get the Control which is mapped to a Property.
        /// </summary>
        public Control GetControlFromPropertyName(string propertyName)
        {
            Control control;
            if (this.propertyNameIndex.TryGetValue(propertyName, out control))
            {
                return control;
            }
            return null;
        }

        /// <summary>
        /// Get all Controls.
        /// </summary>
        public List<Control> GetControls()
        {
            Control[] controlArray = new Control[this.controlNameIndex.Count];
            this.controlNameIndex.Keys.CopyTo(controlArray, 0);
            return new List<Control>(controlArray);
        }

        /// <summary>
        /// Get the Property Name which is mapped to a Control.
        /// </summary>
        public string GetPropertyNameFromControl(Control control)
        {
            string str;
            if (this.controlNameIndex.TryGetValue(control, out str))
            {
                return str;
            }
            return null;
        }

        /// <summary>
        /// Get all Property Names.
        /// </summary>
        public List<string> GetPropertyNames()
        {
            string[] strArray = new string[this.propertyNameIndex.Count];
            this.propertyNameIndex.Keys.CopyTo(strArray, 0);
            return new List<string>(strArray);
        }

        /// <summary>
        /// Remove a Key Value Pair from the dictionaries.
        /// </summary>
        public void Remove(string propertyName, Control control)
        {
            this.controlNameIndex.Remove(control);
            this.propertyNameIndex.Remove(propertyName);
        }

    }
}
