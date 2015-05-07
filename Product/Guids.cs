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

namespace Microsoft.NodejsUwp
{
    static class GuidList
    {
        public const string NodejsUwpPkgString = "0BBF5C2B-3093-431D-8A14-47AC07CFFABF";
        public const string NodejsUwpCmdSetString = "2c965e93-53b6-4921-a543-db736f1f969b";

        public const string NodejsUwpProjectFactoryString = "00251F00-BA30-4CE4-96A2-B8A1085F37AA";
        public const string NodejsUwpProjectFlavorString = "CB6059F1-D6A1-46AC-A9F0-0DE426C8E345";
        public const string NodejsUwpPropertyPageControlString = "1BCA76F2-CEA7-4FCD-8219-BFEB1CB7D6B9";
        public const string NodejsUwpPropertyPageViewString = "05BB0C3B-BC95-4923-84ED-3A89C0DA882F";

        public static readonly Guid guidNodejsUwpCmdSet = new Guid(NodejsUwpCmdSetString);
        public static readonly Guid guidNodejsUwpProjectFactory = new Guid(NodejsUwpProjectFactoryString);
    };
}