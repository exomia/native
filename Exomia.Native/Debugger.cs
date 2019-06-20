#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Exomia.Native
{
    /// <summary>
    ///     Debugger utils.
    /// </summary>
    public static class Debugger
    {
        /// <summary>
        ///     detects if a native debugger is attached.
        /// </summary>
        /// <param name="hProcess">          process handle. </param>
        /// <param name="isDebuggerPresent"> [in,out] <c>true</c> if a native debugger is attached;
        ///                                  <c>false</c> otherwise. </param>
        /// <returns>
        ///     <c>true</c> if handle failure; <c>false</c> otherwise.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = false, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CheckRemoteDebuggerPresent(
            IntPtr                                   hProcess,
            [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);
    }
}