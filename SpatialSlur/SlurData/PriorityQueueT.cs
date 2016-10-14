using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * For a max priority queue, invert the comparer delegate.
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Simple heap-based implementation of a min priority queue.
    /// Can be used as a max priority queue by inverting the comparer function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T>
    {
        private T[] _heap;
        private int _n;
        private readonly Comparison<T> _compare;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="capacity"></param>
        public PriorityQueue(IComparer<T> comparer, int capacity = 2)
            : this(comparer.Compare, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="capacity"></param>
        public PriorityQueue(Comparison<T> compare, int capacity = 2)
        {
            if (capacity < 0)
                throw new ArgumentException("The capacity can not be negative.");

            _heap = new T[capacity + 1];
            _n = 0;
            _compare = compare;
        }


        /// <summary>
        /// Returns the number of elements in the queue.
        /// </summary>
        public int Count
        {
            get { return _n; }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get { return _heap.Length; }
        }
        */


        /// <summary>
        /// Returns true if no elements in the queue.
        /// </summary>
        public bool IsEmpty
        {
            get { return _n == 0; }
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

                return _heap[1];
            }
        }


        /// <summary>
        /// Removes the minimum element from the queue and returns it.
        /// </summary>
        /// <returns></returns>
        public T RemoveMin()
        {
            T min = Min;

            // place the last item on top and sink to maintain heap invariant
            _heap[1] = _heap[_n];
            _heap[_n--] = default(T); // avoids object loitering when T is a reference type
            Sink(1);

            // shrink array capacity if appropriate
            if (_n < _heap.Length >> 2)
                Array.Resize(ref _heap, _heap.Length >> 1);

            return min;
        }


        /// <summary>
        /// Removes the minimum element from the queue by replacing it with the given item.
        /// </summary>
        /// <param name="item"></param>
        public void ReplaceMin(T item)
        {
            _heap[1] = item;
            Sink(1); // sink to maintain heap invariant
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            // increment and increase array capacity if necessary
            if (++_n == _heap.Length)
                Array.Resize(ref _heap, _heap.Length << 1);

            _heap[_n] = item;
            Swim(_n); // swim last element to maintain heap invariant
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        private void Swim(int i)
        {
            int j = i >> 1;

            while (i > 1 && HasPriority(i, j))
            {
                _heap.Swap(i, j);
                i = j;
                j >>= 1;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        private void Sink(int i)
        {
            int j = i << 1;

            while (j <= _n)
            {
                // pick the higher priority of i's children (j or j+1)
                if (j < _n && HasPriority(j + 1, j)) j++;

                // break if heap invariant is satisfied
                if (!HasPriority(j, i)) break;

                // otherwise, swap and continue
                _heap.Swap(i, j);
                i = j;
                j <<= 1;
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
            return _compare(_heap[i], _heap[j]) < 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] result = new T[_n];
            Array.Copy(_heap, 1, result, 0, _n);
            return result;
        }
    }
}
