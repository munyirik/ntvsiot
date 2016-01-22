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

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.NodejsUwp
{
    /// <summary>
    /// Interface for classes that are responsible for applying
    /// UWP patches to installed npm packages.
    /// </summary>
    interface INpmHandler
    {
        /// <summary>
        /// Gets the patch (zipe file).
        /// </summary>
        /// <returns></returns>
        Uri GetPatchUri();

        /// <summary>
        /// Replaces and/or adds files to existing npm package to enable it
        /// to work in Node.js UWP application.
        /// </summary>
        /// <param name="projPath">Path of the project with the node_modules folder</param>
        /// <param name="pane">Visual Studio output window</param>
        /// <param name="platform">x86, x64, or ARM</param>
        void UpdatePackage(string projPath, IVsOutputWindowPane pane, string platform);
    }
}
