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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exomia.Native.UnitTest
{
    [TestClass]
    public unsafe class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            byte** ptr = (byte**)Marshal.AllocHGlobal(IntPtr.Size * 8);
            Mem.Set(ptr, 0, IntPtr.Size * 8);

            Assert.IsTrue(0 == *(int*)ptr);

            int* t1 = (int*)Marshal.AllocHGlobal(64);
            *(int**)(ptr + 0) = t1;

            Assert.IsTrue(0 != *(int*)ptr + 0);
            Assert.IsTrue(t1 == (int*)*(int*)ptr + 0);

            int* t2 = (int*)Marshal.AllocHGlobal(54);
            *(int**)(ptr + 1) = t2;

            byte* b1 = (byte*)t2;
            byte* b2 = *ptr + 1;

            Assert.IsTrue(b1 == b2);
        }
    }
}