/*
 * Notes
 */ 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class DynamicArray
    {
        /// <summary>
        /// Expands the given array to fit the given number of items.
        /// If the length of source is greater than or equal to count, then this does nothing.
        /// </summary>
        public static void ExpandToFit<T>(ref T[] source, int count)
        {
            const int minCapacity = 4;
            int capacity = Math.Max(source.Length, minCapacity);

            while (capacity < count)
                capacity <<= 1;

            Array.Resize(ref source, capacity);
        }


        /// <summary>
        /// Shrinks the the given array to fit the given number of items.
        /// If the length of source is less than or equal to count, then this does nothing.
        /// </summary>
        public static void ShrinkToFit<T>(ref T[] source, int count)
        {
            if (count < source.Length)
                Array.Resize(ref source, count);
        }


        /// <summary>
        /// Appends the given item to the end of the given array.
        /// </summary>
        public static void Append<T>(ref T[] source, int size, in T value)
        {
            const int minCapacity = 4;

            if (source.Length == size)
                Array.Resize(ref source, Math.Max(size << 1, minCapacity));

            source[size] = value;
        }


        /// <summary>
        /// Appends the given items to the end of the source buffer.
        /// </summary>
        public static int Append<T>(ref T[] source, int size, T value, int count)
        {
            var newSize = size + count;

            ExpandToFit(ref source, newSize);
            source.SetRange(value, size, count);

            return newSize;
        }


        /// <summary>
        /// Appends the given items to the end of the source buffer.
        /// </summary>
        public static int Append<T>(ref T[] source, int size, T[] values, int count)
        {
            var newSize = size + count;

            ExpandToFit(ref source, newSize);
            source.SetRange(size, values, 0, count); // TODO: Double check that correct overload is being called

            return newSize;
        }
    }


    /// <summary>
    /// Custom implementation of a dynamic array that differs from System.Collections.Generic.List in the following ways:
    /// - Ref return indexer for better performance with structs
    /// - No additional bounds check on indexer
    /// - Accessable backing array
    /// - Settable Count property for manually reserving additional space
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicArray<T> : IList<T>, IReadOnlyList<T>
    {
        #region Static
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ArrayView<T>(DynamicArray<T> source)
        {
            return new ArrayView<T>(source._items, 0, source._count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ReadOnlyArrayView<T>(DynamicArray<T> source)
        {
            return new ReadOnlyArrayView<T>(source._items, 0, source._count);
        }

        #endregion

        private T[] _items;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        public DynamicArray()
        {
            _items = Array.Empty<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public DynamicArray(int capacity)
        {
            _items = new T[capacity];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        public DynamicArray(IEnumerable<T> sequence)
            :this()
        {
            Add(sequence);
        }


        /// <summary>
        /// 
        /// </summary>
        public T[] Items
        {
            get => _items;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get => _count;
            set
            {
                DynamicArray.ExpandToFit(ref _items, value);
                _count = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get => _items.Length;
            set => Array.Resize(ref _items, value);
        }


        /// <summary>
        /// Returns the item at the given index by reference.
        /// Note that this does not perform an additional bounds check.
        /// If bounds check is needed, use ItemAt() instead.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ref T this[int index]
        {
            get => ref _items[index];
        }


        /// <summary>
        /// Returns the item at the given index by reference.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ref T ItemAt(int index)
        {
            if (index >= _count)
                throw new IndexOutOfRangeException();

            return ref _items[index];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            DynamicArray.Append(ref _items, _count++, item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void Add(ReadOnlyArrayView<T> items)
        {
            DynamicArray.ExpandToFit(ref _items, _count + items.Count);

            var source = _items;
            var offset = _count;

            for (int i = 0; i < items.Count; i++)
                source[i + offset] = items[i];

            _count += items.Count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            if (index >= _count)
                throw new IndexOutOfRangeException();

            Add(default(T));

            // Shift all elements after index
            for (int i = _count - 1; i > index; i--)
                _items[i] = _items[i - 1];

            _items[index] = item;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(T item)
        {
            var i = IndexOf(item);

            if (i == -1)
                return false;

            RemoveAt(i);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index >= _count)
                throw new IndexOutOfRangeException();

            for (int i = index + 1; i < _count; i++)
                _items[i - 1] = _items[i];

            _items[--_count] = default;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_items[i])) return i;

            return -1;
        }


        /// <summary>
        /// Reduces the capacity to twice the count.
        /// If the capacity is already less than twice the count, then this does nothing.
        /// </summary>
        public void TrimExcess()
        {
            DynamicArray.ShrinkToFit(ref _items, _count << 1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ArrayView<T>.Enumerator GetEnumerator()
        {
            return new ArrayView<T>.Enumerator(_items, 0, _count);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_items[i])) return true;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _items.ClearRange(0, _count);
            _count = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> Head(int count)
        {
            return new ArrayView<T>(_items, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> Tail(int count)
        {
            return new ArrayView<T>(_items, _items.Length - count, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> Segment(int start, int count)
        {
            return new ArrayView<T>(_items, start, count);
        }


        #region Explicit Interface Implementations

        T IList<T>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }

        
        T IReadOnlyList<T>.this[int index]
        {
            get => this[index];
        }


        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }
        

        void ICollection<T>.CopyTo(T[] destination, int destinationIndex)
        {
            for (int i = 0; i < _count; i++)
                destination[i + destinationIndex] = _items[i];
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
