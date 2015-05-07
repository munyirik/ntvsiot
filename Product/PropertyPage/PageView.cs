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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio;

namespace Microsoft.NodejsUwp
{ 
    public class PageView : UserControl, IPageView, IPropertyPageUI
    {
        private PropertyControlMap propertyControlMap;

        /// <summary>
        /// This property is used to map the control on a PageView object to a property
        /// in PropertyStore object.
        /// This property must be overriden.
        /// </summary>
        protected virtual PropertyControlTable PropertyControlTable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Occur if the value of a control changed.
        /// </summary>
        public event UserEditCompleteHandler UserEditComplete;
       

        [EditorBrowsable(EditorBrowsableState.Never)]
        public PageView()
        {
        }

        public PageView(IPageViewSite pageViewSite)
        {
            this.propertyControlMap = new PropertyControlMap(
                pageViewSite, this, this.PropertyControlTable);
        }

        #region IPageView members

        /// <summary>
        /// Get the size of this UserControl.
        /// </summary>
        public Size ViewSize
        {
            get
            {
                return base.Size;
            }
        }

        /// <summary>
        /// Make the PageView hide.
        /// </summary>
        public void HideView()
        {
            base.Hide();
        }

        /// <summary>
        /// Initialize this PageView object.
        /// </summary>
        /// <param name="parentControl">
        /// The parent control of this PageView object.
        /// </param>
        /// <param name="rectangle">
        /// The position of this PageView object.
        /// </param>
        public virtual void Initialize(Control parentControl, Rectangle rectangle)
        {
            base.SetBounds(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            base.Parent=parentControl;

            // Initialize the value of the Controls on this PageView object. 
            this.propertyControlMap.InitializeControls();

            // Register the event when the value of a Control changed.
            foreach (Control control in this.PropertyControlTable.GetControls())
            {
                TextBox tb = control as TextBox;
                if (tb != null)
                {
                    tb.TextChanged +=new EventHandler(this.TextBox_TextChanged);
                }
                else
                {
                    CheckBox chk = control as CheckBox;
                    if (chk != null)
                    {
                        chk.CheckedChanged+=
                            new EventHandler(this.CheckBox_CheckedChanged);
                    }
                }
            }
            this.OnInitialize();
        }

        /// <summary>
        /// Move to new position.
        /// </summary>
        public void MoveView(Rectangle rectangle)
        {
            base.Location= new Point(rectangle.X, rectangle.Y);
            base.Size=new Size(rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Pass a keystroke to the property page for processing.
        /// </summary>
        public int ProcessAccelerator(ref Message keyboardMessage)
        {
            if (Control.FromHandle(keyboardMessage.HWnd).PreProcessMessage(ref keyboardMessage))
            {
                return VSConstants.S_OK;
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// Refresh the UI.
        /// </summary>
        public void RefreshPropertyValues()
        {
            this.propertyControlMap.InitializeControls();
        }

        /// <summary>
        /// Show this PageView object.
        /// </summary>
        public void ShowView()
        {
            base.Show();
        }

        #endregion 

        #region IPropertyPageUI

        /// <summary>
        /// Get the value of a Control on this PageView object.
        /// </summary>
        public virtual string GetControlValue(Control control)
        {
            CheckBox chk = control as CheckBox;
            if (chk != null)
            {
                return chk.Checked.ToString();
            }

            TextBox tb = control as TextBox;
            if (tb == null)
            {
                throw new ArgumentOutOfRangeException();
            }
            return tb.Text;
        }

        /// <summary>
        /// Set the value of a Control on this PageView object.
        /// </summary>
        public virtual void SetControlValue(Control control, string value)
        {
            CheckBox chk = control as CheckBox;
            if (chk != null)
            {
                bool flag;
                if (!bool.TryParse(value, out flag))
                {
                    flag = false;
                }
                chk.Checked=flag;
            }
            else
            {
                TextBox tb = control as TextBox;
                if (tb != null)
                {
                    tb.Text = value;
                }
            }
        }


        #endregion

        /// <summary>
        /// Raise the UserEditComplete event.
        /// </summary>
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (this.UserEditComplete != null)
            {
                this.UserEditComplete(chk, chk.Checked.ToString());
            }
        }

        /// <summary>
        /// Raise the UserEditComplete event.
        /// </summary>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (this.UserEditComplete != null)
            {
                this.UserEditComplete(tb, tb.Text);
            }
        }

        protected virtual void OnInitialize()
        {
        }

    }
}
