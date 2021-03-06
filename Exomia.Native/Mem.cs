﻿#region License

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