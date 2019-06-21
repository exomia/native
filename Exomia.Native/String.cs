#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;

namespace Exomia.Native
{
    /// <summary>
    ///     native String class.
    /// </summary>
    public unsafe class String : IDisposable
    {
        /// <summary>
        ///     The pointer.
        /// </summary>
        private readonly IntPtr _mPtr;

        /// <summary>
        ///     The pointer.
        /// </summary>
        private readonly char* _ptr;

        /// <summary>
        ///     The length.
        /// </summary>
        private readonly int _length;

        /// <summary>
        ///     return the length of the current string.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Exomia.Network.Native.String" /> class.
        /// </summary>
        /// <param name="value">  The managed string value. </param>
        public String(string value)
            : this(value, 0, value.Length) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Exomia.Network.Native.String" /> class.
        /// </summary>
        /// <param name="value">  The managed string value. </param>
        /// <param name="offset"> The offset. </param>
        /// <param name="length"> The length. </param>
        public String(string value, int offset, int length)
            : this(length)
        {
            fixed (char* src = value)
            {
                Mem.Cpy(_ptr, src + offset, _length * sizeof(char));
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="String" /> class.
        /// </summary>
        /// <param name="length"> The length. </param>
        private String(int length)
        {
            _length = length;
            _mPtr   = Marshal.AllocHGlobal(length * sizeof(char));
            _ptr    = (char*)_mPtr;
        }

        /// <summary>
        ///     concat two strings together.
        /// </summary>
        /// <param name="a"> The string to process. </param>
        /// <param name="b"> The string to process. </param>
        /// <returns>
        ///     The result of the operation.
        /// </returns>
        public static String operator +(String a, String b)
        {
            String s = new String(a.Length + b.Length);
            Mem.Cpy(s._ptr, a._ptr, a.Length            * sizeof(char));
            Mem.Cpy(s._ptr + a.Length, b._ptr, b.Length * sizeof(char));
            return s;
        }

        /// <summary>
        ///     Convert to a managed string type.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     a managed string.
        /// </returns>
        public static explicit operator string(String value)
        {
            return new string(value._ptr, 0, value._length);
        }

        /// <summary>
        ///     Convert to a unmanaged string type.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     a unmanaged string.
        /// </returns>
        public static explicit operator String(string value)
        {
            return new String(value);
        }

        #region IDisposable Support

        /// <summary>
        ///     True to disposed value.
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Native.String and optionally releases
        ///     the managed resources.
        /// </summary>
        /// <param name="disposing"> True to release both managed and unmanaged resources; false to
        ///                          release only unmanaged resources. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                Marshal.FreeHGlobal(_mPtr);
                _disposedValue = true;
            }
        }

        /// <inheritdoc/>
        ~String()
        {
            Dispose(false);
        }

        
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}