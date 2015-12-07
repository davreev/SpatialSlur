using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Simple heap-based implementation of a min priority queue.
    /// For a max priority queue, simply invert the given comparer delegate.
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
        /// <param name="capacity"></param>
        public PriorityQueue(int capacity = 1)
            :this(Comparer<T>.Default.Compare, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="capacity"></param>
        public PriorityQueue(IComparer<T> comparer, int capacity = 1)
            : this(comparer.Compare, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare"></param>
        public PriorityQueue(Comparison<T> compare, int capacity = 1)
        {
            _heap = new T[capacity + 1];
            _n = 0;
            _compare = compare;
        }


        /// <summary>
        /// returns the number of elements in the queue
        /// </summary>
        public int Count
        {
            get { return _n; }
        }


        /// <summary>
        /// true if no elements in the queue
        /// </summary>
        public bool IsEmpty
        {
            get { return _n == 0; }
        }


        /// <summary>
        /// returns the minimum element in the queue
        /// </summary>
        public T Min
        {
            get
            {
                if (IsEmpty)
                    throw new InvalidOperationException("The queue is empty");

                return _heap[1];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            // increase array capacity if necessary
            if (_n == _heap.Length - 1)
                Array.Resize(ref _heap, _heap.Length << 1);

            _heap[++_n] = item;
            Swim(_n); // swim last element to maintain heap invariant
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T RemoveMin()
        {
            if (IsEmpty)
                throw new InvalidOperationException("The queue is empty");

            T min = _heap[1];
            _heap.Swap(1, _n);

            _heap[_n--] = default(T); // avoids object loitering when T is a reference type
            Sink(1); // sink first element to maintain heap invariant

            // decrease array capacity if oversized
            if (_n > 0 && _n == (_heap.Length - 1) >> 2)
                Array.Resize(ref _heap, _heap.Length >> 1);

            return min;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void Swim(int i)
        {
            int j = i >> 1;

            while (i > 1 && IsGreater(j, i))
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
                if (j < _n && IsGreater(j, j + 1)) j++; // pick the smaller (higher priority) of the two children
                if (!IsGreater(i, j)) break; // break if heap invariant is satisfied

                _heap.Swap(i, j);
                i = j;
                j <<= 1;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool IsGreater(int i, int j)
        {
            return _compare(_heap[i], _heap[j]) > 0;
        }
    }
}
