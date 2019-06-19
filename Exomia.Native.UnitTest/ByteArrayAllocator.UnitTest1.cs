#region MIT License

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

using System.Threading.Tasks;
using Exomia.Native.Allocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exomia.Native.UnitTest
{
    [TestClass]
    public unsafe class ByteArrayAllocatorUnitTest1
    {
        [TestMethod]
        public void UsageTest()
        {
            ByteArrayAllocator allocator = new ByteArrayAllocator(8, 4);

            byte* p1 = allocator.Allocate();

            *(int*)(p1 + 0) = int.MaxValue;
            *(int*)(p1 + 4) = int.MaxValue;

            Assert.AreEqual(1, *(p1 - 1));
            Assert.AreEqual(0, *(p1 - 2));

            byte* p2 = allocator.Allocate();

            *(int*)(p2 + 0) = int.MaxValue;
            *(int*)(p2 + 4) = int.MaxValue;

            Assert.AreEqual(2, *(p2 - 1));
            Assert.AreEqual(1, *(p2 - 2));

            byte* p3 = allocator.Allocate();

            *(int*)(p3 + 0) = int.MaxValue;
            *(int*)(p3 + 4) = int.MaxValue;

            Assert.AreEqual(3, *(p3 - 1));
            Assert.AreEqual(2, *(p3 - 2));

            byte* p4 = allocator.Allocate();

            *(int*)(p4 + 0) = int.MaxValue;
            *(int*)(p4 + 4) = int.MaxValue;

            Assert.AreEqual(4, *(p4 - 1));
            Assert.AreEqual(3, *(p4 - 2));

            allocator.Free(p4);
            allocator.Free(p3);
            allocator.Free(p2);
            allocator.Free(p1);

            byte* p5 = allocator.Allocate();

            *(int*)(p5 + 0) = int.MaxValue;
            *(int*)(p5 + 4) = int.MaxValue;

            Assert.AreEqual(1, *(p5 - 1));
            Assert.AreEqual(0, *(p5 - 2));

            allocator.Free(p5);

            allocator.Dispose();
        }

        [TestMethod]
        public void AllocateMoreThanSpace()
        {
            ByteArrayAllocator allocator = new ByteArrayAllocator(8, 4);

            byte* p1 = allocator.Allocate();

            *(int*)(p1 + 0) = int.MaxValue;
            *(int*)(p1 + 4) = int.MaxValue;

            Assert.AreEqual(1, *(p1 - 1));
            Assert.AreEqual(0, *(p1 - 2));

            byte* p2 = allocator.Allocate();

            *(int*)(p2 + 0) = int.MaxValue;
            *(int*)(p2 + 4) = int.MaxValue;

            Assert.AreEqual(2, *(p2 - 1));
            Assert.AreEqual(1, *(p2 - 2));

            byte* p3 = allocator.Allocate();

            *(int*)(p3 + 0) = int.MaxValue;
            *(int*)(p3 + 4) = int.MaxValue;

            Assert.AreEqual(3, *(p3 - 1));
            Assert.AreEqual(2, *(p3 - 2));

            byte* p4 = allocator.Allocate();

            *(int*)(p4 + 0) = int.MaxValue;
            *(int*)(p4 + 4) = int.MaxValue;

            Assert.AreEqual(4, *(p4 - 1));
            Assert.AreEqual(3, *(p4 - 2));

            allocator.Free(p1);

            byte* p5 = allocator.Allocate();

            *(int*)(p5 + 0) = int.MaxValue;
            *(int*)(p5 + 4) = int.MaxValue;

            Assert.AreEqual(4, *(p5 - 1));
            Assert.AreEqual(0, *(p5 - 2));

            byte* p6 = allocator.Allocate();

            *(int*)(p6 + 0) = int.MaxValue;
            *(int*)(p6 + 4) = int.MaxValue;

            Assert.AreEqual(0, *(p6 - 1));
            Assert.AreEqual(0, *(p6 - 2));

            allocator.Free(p6);
            allocator.Free(p5);

            byte* p7 = allocator.Allocate();

            *(int*)(p7 + 0) = int.MaxValue;
            *(int*)(p7 + 4) = int.MaxValue;

            Assert.AreEqual(4, *(p7 - 1));
            Assert.AreEqual(0, *(p7 - 2));

            allocator.Free(p7);

            allocator.Free(p2);
            allocator.Free(p3);

            byte* p8 = allocator.Allocate();

            *(int*)(p8 + 0) = int.MaxValue;
            *(int*)(p8 + 4) = int.MaxValue;

            Assert.AreEqual(1, *(p8 - 1));
            Assert.AreEqual(2, *(p8 - 2));

            allocator.Free(p4);

            allocator.Dispose();
        }

        [TestMethod]
        public void ParallelTest()
        {
            ByteArrayAllocator allocator = new ByteArrayAllocator(8, 80);

            const int size = 100_000;
            byte*[]   ptrs = new byte*[size];

            Parallel.For(
                0, size, i =>
                {
                    ptrs[i] = allocator.Allocate();
                });

            Parallel.For(
                0, size, i =>
                {
                    allocator.Free(ptrs[i]);
                });

            allocator.Dispose();
        }
    }
}