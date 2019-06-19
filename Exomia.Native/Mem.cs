﻿#region MIT License

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
    ///     Memory utils.
    /// </summary>
    public static unsafe class Mem
    {
        /// <summary>
        ///     The msvcrt.
        /// </summary>
        private const string MSVCRT = "msvcrt.dll";

        /// <summary>
        ///     memcpy call Copies the values of num bytes from the location pointed to by source
        ///     directly to the memory block pointed to by destination.
        /// </summary>
        /// <param name="dest">  [in,out] destination ptr. </param>
        /// <param name="src">   [in,out] source ptr. </param>
        /// <param name="count"> count of bytes to copy. </param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            MSVCRT, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void Cpy(
            void* dest,
            void* src,
            ulong count);

        /// <summary>
        ///     memcpy call Copies the values of num bytes from the location pointed to by source
        ///     directly to the memory block pointed to by destination.
        /// </summary>
        /// <param name="dest">  destination addr. </param>
        /// <param name="src">   source addr. </param>
        /// <param name="count"> count of bytes to copy. </param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            MSVCRT, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void Cpy(
            IntPtr dest,
            IntPtr src,
            ulong  count);

        /// <summary>
        ///     memcpy call Copies the values of num bytes from the location pointed to by source
        ///     directly to the memory block pointed to by destination.
        /// </summary>
        /// <param name="dest">  [in,out] destination ptr. </param>
        /// <param name="src">   [in,out] source ptr. </param>
        /// <param name="count"> count of bytes to copy. </param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            MSVCRT, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void Cpy(
            void* dest,
            void* src,
            int   count);

        /// <summary>
        ///     memcpy call Copies the values of num bytes from the location pointed to by source
        ///     directly to the memory block pointed to by destination.
        /// </summary>
        /// <param name="dest">  destination addr. </param>
        /// <param name="src">   source addr. </param>
        /// <param name="count"> count of bytes to copy. </param>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            MSVCRT, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void Cpy(
            IntPtr dest,
            IntPtr src,
            int    count);

        /// <summary>
        ///     memset call Sets the first num bytes of the block of memory pointed by ptr to the
        ///     specified value (interpreted as an unsigned char).
        /// </summary>
        /// <param name="dest">  destination addr. </param>
        /// <param name="value"> value to be set. </param>
        /// <param name="count"> count of bytes. </param>
        /// <returns>
        ///     An IntPtr.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            MSVCRT, EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr Set(
            IntPtr dest,
            int    value,
            int    count);

        /// <summary>
        ///     memset call Sets the first num bytes of the block of memory pointed by ptr to the
        ///     specified value (interpreted as an unsigned char).
        /// </summary>
        /// <param name="dest">  [in,out] destination addr. </param>
        /// <param name="value"> value to be set. </param>
        /// <param name="count"> count of bytes. </param>
        /// <returns>
        ///     Null if it fails, else a void*.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            MSVCRT, EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* Set(
            void* dest,
            int   value,
            int   count);

        /// <summary>
        ///     memcmp call Sets the first num bytes of the block of memory pointed by ptr to the
        ///     specified value (interpreted as an unsigned char).
        /// </summary>
        /// <param name="ptr1">  [in,out] ptr b1. </param>
        /// <param name="ptr2">  [in,out] ptr 2. </param>
        /// <param name="count"> bytes to compare. </param>
        /// <returns>
        ///     0 if equal.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(
            MSVCRT, EntryPoint = "memcmp", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern int Cmp(
            void* ptr1,
            void* ptr2,
            int   count);
    }
}