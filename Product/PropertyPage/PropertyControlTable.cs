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
