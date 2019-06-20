#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;
using System.Threading;

// ReSharper disable ArrangeRedundantParentheses
namespace Exomia.Native.Allocator
{

    
    /// <summary>
    ///     A byte array allocator. This class cannot be inherited.
    /// </summary>
    public sealed unsafe class ByteArrayAllocator : IDisposable
    {
        /// <summary>
        ///     The pointer.
        /// </summary>
        private readonly IntPtr _mPtr;

        /// <summary>
        ///     The pointer.
        /// </summary>
        private readonly byte* _ptr;

        /// <summary>
        ///     The size.
        /// </summary>
        private readonly int _size;

        /// <summary>
        ///     The capacity.
        /// </summary>
        private readonly int _capacity;

        /// <summary>
        ///     The head.
        /// </summary>
        private byte _head;

        /// <summary>
        ///     Number of.
        /// </summary>
        private int _count;

        /// <summary>
        ///     The lock.
        /// </summary>
        private SpinLock _lock;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteArrayAllocator" /> class.
        /// </summary>
        /// <param name="size">     size. </param>
        /// <param name="capacity"> (Optional) capacity. </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when one or more arguments are outside
        ///     the required range.
        /// </exception>
        public ByteArrayAllocator(int size, byte capacity = 64)
        {
            if (size <= 0) { throw new ArgumentOutOfRangeException(nameof(size)); }

            _size     = size + 2;
            _capacity = capacity;

            _mPtr = Marshal.AllocHGlobal(_size * capacity);
            _ptr  = (byte*)_mPtr;

            _head = 0;

            for (byte i = 0; i < _capacity; i++)
            {
                *(_ptr + (i * _size) + 0) = i;
                *(_ptr + (i * _size) + 1) = (byte)(i + 1);
            }

            _lock = new SpinLock(System.Diagnostics.Debugger.IsAttached);
        }

        /// <summary>
        ///     Allocate a new byte array.
        /// </summary>
        /// <returns>
        ///     Null if it fails, else a byte*.
        /// </returns>
        public byte* Allocate()
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                if (_count < _capacity)
                {
                    byte  next   = *(_ptr + (_head * _size) + 1);
                    byte* buffer = _ptr + (_head * _size) + 2;
                    _head = next;
                    _count++;
                    return buffer;
                }
            }
            finally
            {
                if (lockTaken) { _lock.Exit(false); }
            }

            byte* n = (byte*)Marshal.AllocHGlobal(_size);
            *(n + 0) = 0;
            *(n + 1) = 0;
            return n + 2;
        }

        /// <summary>
        ///     free a byte array.
        /// </summary>
        /// <param name="ptr"> [in,out] ptr. </param>
        public void Free(byte* ptr)
        {
            if (*(ptr - 1) != *(ptr - 2))
            {
                if (_count > 0)
                {
                    bool lockTaken = false;
                    try
                    {
                        _lock.Enter(ref lockTaken);

                        *(ptr - 1) = _head;      // set next on current head index
                        _head      = *(ptr - 2); // set the head now on this elements index
                        _count--;
                    }
                    finally
                    {
                        if (lockTaken) { _lock.Exit(false); }
                    }
                }
            }
            else
            {
                Marshal.FreeHGlobal(new IntPtr(ptr - 2));
            }
        }

        #region IDisposable Support

        /// <summary>
        ///     True to disposed value.
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Native.Allocator.ByteArrayAllocator
        ///     and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                Marshal.FreeHGlobal(_mPtr);
                _disposedValue = true;
            }
        }

        /// <inheritdoc />
        ~ByteArrayAllocator()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}