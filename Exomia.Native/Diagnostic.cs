#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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