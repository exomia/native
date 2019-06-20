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
    ///     A diagnostic.
    /// </summary>
    public static class Diagnostic
    {
        /// <summary>
        ///     The second kernel 3.
        /// </summary>
        private const string KERNEL32 = "kernel32.dll";

        /// <summary>
        ///     &lt;see href="&gt;https://msdn.microsoft.com/en-us/library/ms724400(VS.85).aspx" /&gt;
        /// </summary>
        /// <param name="lpIdleTime">   [out] The idle time. </param>
        /// <param name="lpKernelTime"> [out] The kernel time. </param>
        /// <param name="lpUserTime">   [out] The user time. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(KERNEL32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSystemTimes(
            out FILETIME lpIdleTime,
            out FILETIME lpKernelTime,
            out FILETIME lpUserTime);

        /// <summary>
        ///     &lt;see href="&gt;https://msdn.microsoft.com/en-us/library/ms683223(VS.85).aspx" /&gt;
        /// </summary>
        /// <param name="hProcess">       The process. </param>
        /// <param name="lpCreationTime"> [out] The creation time. </param>
        /// <param name="lpExitTime">     [out] The exit time. </param>
        /// <param name="lpKernelTime">   [out] The kernel time. </param>
        /// <param name="lpUserTime">     [out] The user time. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(KERNEL32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetProcessTimes(
            IntPtr       hProcess,
            out FILETIME lpCreationTime,
            out FILETIME lpExitTime,
            out FILETIME lpKernelTime,
            out FILETIME lpUserTime);

        /// <summary>
        ///     A filetime.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            /// <summary>
            ///     The low date time.
            /// </summary>
            public uint DwLowDateTime;

            /// <summary>
            ///     The high date time.
            /// </summary>
            public uint DwHighDateTime;

            /// <summary>
            ///     Gets the value.
            /// </summary>
            /// <value>
            ///     The value.
            /// </value>
            public ulong Value
            {
                get { return ((ulong)DwHighDateTime << 32) + DwLowDateTime; }
            }
        }
    }
}