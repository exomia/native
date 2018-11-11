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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Native
{
    /// <inheritdoc />
    /// <summary>
    ///     UnsafeByteArrayAllocator2 class
    /// </summary>
    public sealed unsafe class ByteArrayPoolAllocator : IDisposable
    {
        private readonly ByteArrayAllocator[] _buckets;
        private readonly byte[] _bucketCapacity;
        private readonly int _shift;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteArrayPoolAllocator" /> class.
        /// </summary>
        /// <param name="bucketCapacity">bucketCapacity</param>
        /// <param name="shift">shift</param>
        public ByteArrayPoolAllocator(byte[] bucketCapacity, int shift)
        {
            _bucketCapacity = bucketCapacity;
            _shift          = shift;
            _buckets        = new ByteArrayAllocator[bucketCapacity.Length];
        }

        /// <summary>
        ///     Allocate a new byte array
        /// </summary>
        /// <param name="size">size to allocate</param>
        public byte* Allocate(int size)
        {
            int bucketIndex = SelectBucketIndex(size);

            if (bucketIndex < _buckets.Length)
            {
                if (_buckets[bucketIndex] == null)
                {
                    _buckets[bucketIndex] = new ByteArrayAllocator(
                        1 << (_shift + bucketIndex), _bucketCapacity[bucketIndex]);
                }
                return _buckets[bucketIndex].Allocate();
            }

            return (byte*)Marshal.AllocHGlobal(size);
        }

        /// <summary>
        ///     free a byte array
        /// </summary>
        /// <param name="ptr">ptr</param>
        /// <param name="size">size of ptr</param>
        public void Free(byte* ptr, int size)
        {
            int bucketIndex = SelectBucketIndex(size);
            if (bucketIndex < _buckets.Length)
            {
                if (_buckets[bucketIndex] != null)
                {
                    _buckets[bucketIndex].Free(ptr);
                    return;
                }
                throw new InvalidOperationException("can't free a buffer which was not allocated by this system");
            }
            Marshal.FreeHGlobal(new IntPtr(ptr));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SelectBucketIndex(int size)
        {
            uint br = ((uint)size - 1) >> _shift;

            int index = 0;
            if (br > 0xFFFF)
            {
                br    >>= 16;
                index =   16;
            }
            if (br > 0xFF)
            {
                br    >>= 8;
                index +=  8;
            }
            if (br > 0xF)
            {
                br    >>= 4;
                index +=  4;
            }
            if (br > 0x3)
            {
                br    >>= 2;
                index +=  2;
            }
            if (br > 0x1)
            {
                br    >>= 1;
                index +=  1;
            }

            return index + (int)br;
        }

        #region IDisposable Support

        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                foreach (ByteArrayAllocator bucket in _buckets)
                {
                    bucket?.Dispose();
                }
                _disposedValue = true;
            }
        }

        /// <inheritdoc />
        ~ByteArrayPoolAllocator()
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