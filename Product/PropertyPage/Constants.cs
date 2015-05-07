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

namespace Microsoft.NodejsUwp
{
    public static class Constants
    { 
        #region Following constants are used in IPropertyPage::Show Method.

        public const int SW_SHOW = 5;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_HIDE = 0;

        #endregion

        #region Following constants are used in IPropertyPageSite::OnStatusChange Method.

        /// <summary>
        /// The values in the pages have changed, so the state of the
        /// Apply button should be updated.
        /// </summary>
        public const int PROPPAGESTATUS_DIRTY = 0x1;

        /// <summary>
        /// Now is an appropriate time to apply changes.
        /// </summary>
        public const int PROPPAGESTATUS_VALIDATE = 0x2;

        #endregion
    }
}