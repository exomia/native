﻿#region License

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
    ///     A byte array pool 2 allocator. This class cannot be inherited.
    /// </summary>
    public sealed unsafe class ByteArrayPool2Allocator : IDisposable
    {
        /// <summary>
        ///     The nullptr.
        /// </summary>
        private static readonly byte* s_nullptr = (byte*)0;

        /// <summary>
        ///     The pointer.
        /// </summary>
        private readonly byte** _ptr;

        /// <summary>
        ///     The shift.
        /// </summary>
        private readonly int _shift;

        /// <summary>
        ///     The bucket capacity.
        /// </summary>
        private readonly byte[] _bucketCapacity;

        /// <summary>
        ///     The bucket head.
        /// </summary>
        private readonly byte[] _bucketHead;

        /// <summary>
        ///     Number of buckets.
        /// </summary>
        private readonly byte[] _bucketCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteArrayPool2Allocator" /> class.
        /// </summary>
        /// <param name="bucketCapacity"> bucketCapacity. </param>
        /// <param name="shift">          shift. </param>
        public ByteArrayPool2Allocator(byte[] bucketCapacity, int shift)
        {
            _bucketCapacity = bucketCapacity;
            _shift          = shift;

            _ptr = (byte**)Marshal.AllocHGlobal(bucketCapacity.Length * IntPtr.Size);

            Mem.Set(_ptr, 0, bucketCapacity.Length * IntPtr.Size);

            _bucketHead  = new byte[bucketCapacity.Length];
            _bucketCount = new byte[bucketCapacity.Length];
        }

        /// <summary>
        ///     Initializes the bucket.
        /// </summary>
        /// <param name="ptr">      [in,out] ptr. </param>
        /// <param name="size">     size of ptr. </param>
        /// <param name="capacity"> The capacity. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitializeBucket(byte* ptr, int size, int capacity)
        {
            for (byte i = 0; i < capacity; i++)
            {
                *(ptr + (i * size) + 0) = i;
                *(ptr + (i * size) + 1) = (byte)(i + 1);
            }
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

            if (bucketIndex < _bucketCapacity.Length)
            {
                int bucketSize = 1 << (_shift + bucketIndex);
                if (*(_ptr + bucketIndex) == s_nullptr)
                {
                    *(_ptr + bucketIndex) =
                        (byte*)Marshal.AllocHGlobal((bucketSize + 2) * _bucketCapacity[bucketIndex]);
                    InitializeBucket(*(_ptr + bucketIndex), bucketSize + 2, _bucketCapacity[bucketIndex]);
                }

                if (_bucketCount[bucketIndex] < _bucketCapacity[bucketIndex])
                {
                    byte* bucket = *(_ptr                                             + bucketIndex);
                    byte  next   = *(bucket + (_bucketHead[bucketIndex] * bucketSize) + 1);
                    byte* buffer = bucket + (_bucketHead[bucketIndex] * bucketSize) + 2;
                    _bucketHead[bucketIndex] = next;
                    _bucketCount[bucketIndex]++;
                    return buffer;
                }

                byte* n = (byte*)Marshal.AllocHGlobal(size + 2);
                *(n + 0) = 0;
                *(n + 1) = 0;
                return n + 2;
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
            if (bucketIndex < _bucketCapacity.Length)
            {
                if (*(_ptr + bucketIndex) != s_nullptr)
                {
                    if (*(ptr - 1) != *(ptr - 2))
                    {
                        if (_bucketCount[bucketIndex] > 0)
                        {
                            *(ptr - 1) = _bucketHead[bucketIndex]; // set next on current head index
                            _bucketHead[bucketIndex] =
                                *(ptr - 2); // set the head now on this elements index
                            _bucketCount[bucketIndex]--;
                        }
                    }
                    else
                    {
                        Marshal.FreeHGlobal(new IntPtr(ptr - 2));
                    }
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
        ///     Exomia.Native.Allocator.ByteArrayPool2Allocator and optionally releases the managed
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
                for (int i = 0; i < _bucketCapacity.Length; i++)
                {
                    if (*(_ptr + i) != s_nullptr)
                    {
                        Marshal.FreeHGlobal((IntPtr)(*(_ptr + i)));
                    }
                }
                Marshal.FreeHGlobal((IntPtr)_ptr);

                _disposedValue = true;
            }
        }

        /// <inheritdoc />
        ~ByteArrayPool2Allocator()
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