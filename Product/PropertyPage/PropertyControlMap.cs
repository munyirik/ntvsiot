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
