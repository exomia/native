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

#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Native.Collections
{
    /// <summary>
    ///     a faster list for unmanaged types
    /// </summary>
    /// <typeparam name="T">unmanaged</typeparam>
    public unsafe class List2<T> where T : unmanaged
    {
        private const int DEFAULT_CAPACITY = 8;
        private const int MAX_CAPACITY = 0X7FEFFFFF;

        private int _count;
        private IntPtr _mItems;
        private T* _items;
        private int _capacity;

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _capacity; }
            set
            {
                if (value < _count)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(Capacity)} must be greater than the current size.");
                }
                if (value != _capacity)
                {
                    if (value > 0)
                    {
                        IntPtr mNewItems = Marshal.AllocHGlobal(value * sizeof(T));
                        if (_count > 0) { Mem.Cpy((T*)mNewItems, _items, _count * sizeof(T)); }
                        Marshal.FreeHGlobal(_mItems);
                        _mItems   = mNewItems;
                        _items    = (T*)_mItems;
                        _capacity = value;
                    }
                    else { _items = null; }
                }
            }
        }

        /// <summary>
        ///     returns the current number of itms in this list
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        ///     returns the item at the specified index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>the item at index</returns>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _items[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _items[index] = value; }
        }

        /// <summary>
        ///     returns the item at the specified index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>the item at index</returns>
        public T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _items[index]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _items[index] = value; }
        }

        public List2()
        {
            _items    = null;
            _capacity = 0;
        }

        public List2(int capacity)
        {
            if (capacity < 0) { throw new ArgumentOutOfRangeException(nameof(capacity)); }
            _mItems = capacity == 0 ? IntPtr.Zero : Marshal.AllocHGlobal(capacity * sizeof(T));
            _items  = (T*)_mItems;

            _capacity = capacity;
        }

        public List2(IEnumerable<T> collection)
        {
            if (collection == null) { throw new ArgumentNullException(nameof(collection)); }

            _count = 0;
            if (collection is ICollection<T> c)
            {
                _mItems = Marshal.AllocHGlobal(c.Count * sizeof(T));
            }
            else { _mItems = Marshal.AllocHGlobal(DEFAULT_CAPACITY * sizeof(T)); }

            _items = (T*)_mItems;

            using (IEnumerator<T> en = collection.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int index)
        {
            return ref *(_items + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(uint index)
        {
            return ref *(_items + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T item)
        {
            if (_count == _capacity) { EnsureCapacity(_count + 1); }
            *(_items + _count++) = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, in T item)
        {
            if (_count == _capacity) { EnsureCapacity(_count + 1); }
            if (index < _count) { Mem.Cpy(_items + index + 1, _items + index, (_count - index) * sizeof(T)); }
            *(_items + index) = item;
            _count++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(in T item)
        {
            int index = IndexOf(item, 0, _count);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            _count--;
            if (index < _count) { Mem.Cpy(_items + index, _items + index + 1, (_count - index) * sizeof(T)); }

            //*(_items + _count) = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(int index, int count)
        {
            if (count > 0)
            {
                _count -= count;
                if (index < _count)
                {
                    Mem.Cpy(_items + index, _items + index + count, (_count - index) * sizeof(T));
                }

                //Mem.Set(_items + _count, 0, count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (_count > 0)
            {
                Mem.Set(_items, 0, _count);
                _count = 0;
            }
        }

        public int IndexOf(in T item, int startIndex, int count)
        {
            int num = startIndex + count;
            fixed (T* b = &item)
            {
                for (int i = startIndex; i < num; i++)
                {
                    if (Mem.Cmp(_items + i, b, sizeof(T)) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public bool Contains(in T item)
        {
            return IndexOf(item, 0, _count) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List2<T> GetRange(int index, int count)
        {
            List2<T> list = new List2<T>(count);
            Mem.Cpy(list._items, _items, count * sizeof(T));
            list._count = count;
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            T[] array = new T[_count];
            fixed (T* ptr = array)
            {
                Mem.Cpy(ptr, _items, _count * sizeof(T));
            }
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray(int index)
        {
            T[] array = new T[_count - index];
            fixed (T* ptr = array)
            {
                Mem.Cpy(ptr, _items + index, (_count - index) * sizeof(T));
            }
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray(int index, int count)
        {
            T[] array = new T[count];
            fixed (T* ptr = array)
            {
                Mem.Cpy(ptr, _items + index, count * sizeof(T));
            }
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int min)
        {
            if (_capacity < min)
            {
                int newCapacity = _capacity == 0 ? DEFAULT_CAPACITY : _capacity * 2;
                if (newCapacity > MAX_CAPACITY) { newCapacity = MAX_CAPACITY; }
                if (newCapacity < min) { newCapacity          = min; }

                IntPtr mNewItems = Marshal.AllocHGlobal(newCapacity * sizeof(T));
                if (_count > 0) { Mem.Cpy((T*)mNewItems, _items, _count * sizeof(T)); }
                Marshal.FreeHGlobal(_mItems);
                _mItems = mNewItems;
                _items  = (T*)_mItems;
            }
        }
    }
}