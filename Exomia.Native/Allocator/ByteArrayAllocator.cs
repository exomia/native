#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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
using System.Threading;

namespace Exomia.Native.Allocator
{
    // ReSharper disable ArrangeRedundantParentheses

    /// <inheritdoc />
    /// <summary>
    ///     UnsafeByteArrayAllocator class
    /// </summary>
    public sealed unsafe class ByteArrayAllocator : IDisposable
    {
        private readonly IntPtr _mPtr;
        private readonly byte* _ptr;

        private readonly int _size;
        private readonly int _capacity;
        private byte _head;
        private int _count;

        private SpinLock _lock;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteArrayAllocator" /> class.
        /// </summary>
        /// <param name="size">size</param>
        /// <param name="capacity">capacity</param>
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
                *(_ptr + i * _size + 0) = i;
                *(_ptr + i * _size + 1) = (byte)(i + 1);
            }

            _lock = new SpinLock(System.Diagnostics.Debugger.IsAttached);
        }

        /// <summary>
        ///     Allocate a new byte array
        /// </summary>
        public byte* Allocate()
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                if (_count < _capacity)
                {
                    byte next = *(_ptr + _head * _size + 1);
                    byte* buffer = _ptr + _head * _size + 2;
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
        ///     free a byte array
        /// </summary>
        /// <param name="ptr">ptr</param>
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

                        *(ptr - 1) = _head; // set next on current head index
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

        private bool _disposedValue;

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