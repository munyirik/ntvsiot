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


using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.NodejsUwp
{
    public class PropertyControlMap
    {
        // The IPageViewSite Interface is implemented by the PropertyPage class.
        private IPageViewSite pageViewSite;

        // The IPropertyPageUI Interface is implemented by the PageView Class.
        private IPropertyPageUI propertyPageUI;

        // The PropertyControlTable class stores the Control / Property Name KeyValuePairs. 
        // A KeyValuePair contains a Control of a PageView object, and a Property Name of
        // PropertyPage object.
        private PropertyControlTable propertyControlTable;
        

        public PropertyControlMap(IPageViewSite pageViewSite, 
            IPropertyPageUI propertyPageUI, PropertyControlTable propertyControlTable)
        {
            this.propertyControlTable = propertyControlTable;
            this.pageViewSite = pageViewSite;
            this.propertyPageUI = propertyPageUI;
        }

        /// <summary>
        /// Initialize the Controls on a PageView Object using the Properties of
        /// a PropertyPage object. 
        /// </summary>
        public void InitializeControls()
        {
            this.propertyPageUI.UserEditComplete -= 
                new UserEditCompleteHandler(this.propertyPageUI_UserEditComplete);
            foreach (string str in this.propertyControlTable.GetPropertyNames())
            {
                string valueForProperty = this.pageViewSite.GetValueForProperty(str);
                Control controlFromPropertyName =                
                    this.propertyControlTable.GetControlFromPropertyName(str);
                
                this.propertyPageUI.SetControlValue(
                    controlFromPropertyName, valueForProperty);

                // TODO: This if statement is temporary. --no - console is required since there isn't a way to redirect stdout/err from
                // node.js to UWP. --debug is required until something like IsDebuggerPresent can work in the UWP app or VS F5 can
                // automatically pass the flag to the UWP app.
                if (0 == string.Compare(valueForProperty, "") && 0 == string.Compare(controlFromPropertyName.Name, "_nodeArguments"))
                {
                    this.propertyPageUI.SetControlValue(
                        controlFromPropertyName, "--debug");
                    propertyPageUI_UserEditComplete(controlFromPropertyName, "--debug");
                }
            }
            this.propertyPageUI.UserEditComplete +=                
                new UserEditCompleteHandler(this.propertyPageUI_UserEditComplete);
        }

        /// <summary>
        /// Notify the PropertyPage object that a Control value is changed.
        /// </summary>
        private void propertyPageUI_UserEditComplete(Control control, string value)
        {
            string propertyNameFromControl = this.propertyControlTable.GetPropertyNameFromControl(control);
            this.pageViewSite.PropertyChanged(propertyNameFromControl, value);
        }

    }
}
