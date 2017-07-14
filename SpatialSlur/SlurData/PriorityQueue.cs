using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Simple heap-based implementation of a min priority queue.
    /// Can be used as a max priority queue by inverting the given comparison delegate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PriorityQueue<T>
    {
        private const int MinCapacity = 4;

        private readonly Comparison<T> _compare;
        private T[] _items;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="capacity"></param>
        public PriorityQueue(Comparer<T> comparer, int capacity = MinCapacity)
            : this(comparer.Compare, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="capacity"></param>
        public PriorityQueue(IComparer<T> comparer, int capacity = MinCapacity)
            : this(comparer.Compare, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="capacity"></param>
        public PriorityQueue(Comparison<T> compare, int capacity = MinCapacity)
        {
            _compare = compare ?? throw new ArgumentNullException();
            _items = new T[Math.Max(capacity, MinCapacity)];
            _count = 0;
        }


        /// <summary>
        /// Returns the number of elements in the queue.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get { return _items.Length; }
        }


        /// <summary>
        /// Returns true if no elements in the queue.
        /// </summary>
        public bool IsEmpty
        {
            get { return _count == 0; }
        }


        /// <summary>
        /// Returns the minimum element in the queue.
        /// </summary>
        public T Min
        {
            get
            {
                if (IsEmpty)
                    throw new InvalidOperationException("The queue is empty.");

                return _items[0];
            }
        }


        /// <summary>
        /// Creates a shallow copy of the internal array.
        /// </summary>
        /// <returns></returns>
        public PriorityQueue<T> Duplicate()
        {
            var copy = new PriorityQueue<T>(_compare);
            copy._items = _items.ShallowCopy();
            copy._count = _count;
            return copy;
        }


        /// <summary>
        /// Removes the minimum element from the queue and returns it.
        /// </summary>
        /// <returns></returns>
        public T RemoveMin()
        {
            T min = Min;

            // place the last item on top and sink to maintain heap invariant
            _items[0] = _items[--_count];
            _items[_count] = default(T); // avoids object loitering when T is a reference type
            Sink(0);

            return min;
        }


        /// <summary>
        /// Removes the minimum element from the queue by replacing it with the given item.
        /// </summary>
        /// <param name="item"></param>
        public void ReplaceMin(T item)
        {
            _items[0] = item;
            Sink(0); // sink to maintain heap invariant
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        private void Sink(int i)
        {
            int j0 = (i << 1) + 1; // left child
            int j1 = j0 + 1; // right child

            while (j0 < _count)
            {
                int j = (j1 < _count && HasPriority(j1, j0)) ? j1 : j0; // higher priority child

                // break if heap invariant is satisfied
                if (!HasPriority(j, i)) break;

                // otherwise, swap and continue
                _items.Swap(i, j);
                i = j;
                j0 = (i << 1) + 1;
                j1 = j0 + 1;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            // increment and increase array capacity if necessary
            if (_count == _items.Length)
                Array.Resize(ref _items, _items.Length << 1);

            _items[_count] = item;
            Swim(_count++); // swim last element to maintain heap invariant
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        private void Swim(int i)
        {
            int j = (i - 1) >> 1; // parent

            while (i > 0 && HasPriority(i, j))
            {
                _items.Swap(i, j);
                i = j;
                j = (i - 1) >> 1;
            }
        }


        /// <summary>
        /// Returns true if item i is less than item j.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool HasPriority(int i, int j)
        {
            return _compare(_items[i], _items[j]) < 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return _items.GetRange(0, _count);
        }


        /// <summary>
        /// Trims the array if capacity is greater than twice the count.
        /// </summary>
        public void TrimExcess()
        {
            int max = Math.Max(_count << 1, MinCapacity);

            if (_items.Length > max)
                Array.Resize(ref _items, max);
        }
    }
}
