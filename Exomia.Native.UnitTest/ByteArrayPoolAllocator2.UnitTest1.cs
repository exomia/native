#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Native.Allocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exomia.Native.UnitTest
{
    [TestClass]
    public unsafe class ByteArrayPoolAllocator2UnitTest1
    {
        [TestMethod]
        public void UsageTest()
        {
            // 1 << 7 == 100_0000
            ByteArrayPool2Allocator allocator = new ByteArrayPool2Allocator(new byte[] { 128, 128, 128, 64 }, 7);

            byte* ptr1 = allocator.Allocate(50);
            byte* ptr2 = allocator.Allocate(45);
            byte* ptr3 = allocator.Allocate(64);
            byte* ptr4 = allocator.Allocate(134);
            byte* ptr5 = allocator.Allocate(512);

            //TODO: FIX FREE (buggy offset?)
            allocator.Free(ptr1, 50);
            allocator.Free(ptr2, 45);
            allocator.Free(ptr3, 64);
            allocator.Free(ptr4, 134);
            allocator.Free(ptr5, 512);

            allocator.Dispose();
        }
    }
}