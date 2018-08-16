
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
    /// List implementation that supports indexing with ref returns.
    /// Placeholder until .NET framework List implementation supports the same.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RefList<T> : IList<T>, IReadOnlyList<T>
    {
        private T[] _items;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        public RefList()
        {
            _items = Array.Empty<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public RefList(int capacity)
        {
            _items = new T[capacity];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public RefList(ICollection<T> collection)
            :this(collection.Count)
        {
            AddRange(collection);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequence"></param>
        public RefList(IEnumerable<T> sequence)
            :this()
        {
            AddRange(sequence);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ref T this[int index]
        {
            get
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                return ref _items[index];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                if(value > _count)
                {
                    Array.Resize(ref _items, value << 1);
                    _count = value;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get { return _items.Length; }
            set { Array.Resize(ref _items, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        internal T[] Items
        {
            get { return _items; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            const int minCapacity = 4;

            if (_items.Length == _count)
                Array.Resize(ref _items, Math.Max(_count << 1, minCapacity));

            _items[_count++] = item;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
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

            Add(default);

            for(int i = index; i < _count; i++)
                Utilities.Swap(ref _items[i], ref item); // TODO test this to be sure
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(T item)
        {
            var i = IndexOf(item);
            if (i == -1)return false;

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
        /// If the capacity is already less than twice the count, then this function does nothing.
        /// </summary>
        public void TrimExcess()
        {
            int max = _count << 1;

            if (_items.Length > max)
                Array.Resize(ref _items, max);
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
        /// <returns></returns>
        public ArrayView<T> AsView()
        {
            return _items.AsView(_count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> AsView(int count)
        {
            return _items.AsView(count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> AsView(int start, int count)
        {
            return _items.AsView(start, count);
        }


        #region Explicit interface implementations

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
