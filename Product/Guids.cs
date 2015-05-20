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