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