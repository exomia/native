#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable ArrangeRedundantParentheses

namespace Exomia.Native.Allocator
{
    /// <summary>
    ///     A byte array pool allocator. This class cannot be inherited.
    /// </summary>
    public sealed unsafe class ByteArrayPoolAllocator : IDisposable
    {
        /// <summary>
        ///     The buckets.
        /// </summary>
        private readonly ByteArrayAllocator[] _buckets;

        /// <summary>
        ///     The bucket capacity.
        /// </summary>
        private readonly byte[] _bucketCapacity;

        /// <summary>
        ///     The shift.
        /// </summary>
        private readonly int _shift;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteArrayPoolAllocator" /> class.
        /// </summary>
        /// <param name="bucketCapacity"> bucketCapacity. </param>
        /// <param name="shift">          shift. </param>
        public ByteArrayPoolAllocator(byte[] bucketCapacity, int shift)
        {
            _bucketCapacity = bucketCapacity;
            _shift          = shift;
            _buckets        = new ByteArrayAllocator[bucketCapacity.Length];
        }

        /// <summary>
        ///     Allocate a new byte array.
        /// </summary>
        /// <param name="size"> size to allocate. </param>
        /// <returns>
        ///     Null if it fails, else a byte*.
        /// </returns>
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
        ///     free a byte array.
        /// </summary>
        /// <param name="ptr">  [in,out] ptr. </param>
        /// <param name="size"> size of ptr. </param>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
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

        /// <summary>
        ///     Select bucket index.
        /// </summary>
        /// <param name="size"> size to allocate. </param>
        /// <returns>
        ///     An int.
        /// </returns>
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

        /// <summary>
        ///     True to disposed value.
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        ///     Releases the unmanaged resources used by the
        ///     Exomia.Native.Allocator.ByteArrayPoolAllocator and optionally releases the managed
        ///     resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
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