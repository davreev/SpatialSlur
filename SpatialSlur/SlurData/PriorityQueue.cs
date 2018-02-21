using System;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Simple heap-based implementation of a min priority queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PriorityQueue<K, V>
        where K : IComparable<K>
    {
        #region Static

        private static (K Key, V Value) _default = default((K, V));
        private const int _minCapacity = 4;

        #endregion

        
        private (K Key, V Value)[] _items;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="capacity"></param>
        public PriorityQueue(int capacity = _minCapacity)
        {
            _items = new (K, V)[Math.Max(capacity, _minCapacity)];
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
        public (K Key, V Value) Min
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
        public PriorityQueue<K, V> Duplicate()
        {
            var copy = new PriorityQueue<K, V>(Capacity);
            copy._items.SetRange(_items, _count);
            copy._count = _count;
            return copy;
        }


        /// <summary>
        /// Removes the minimum element from the queue and returns it.
        /// </summary>
        /// <returns></returns>
        public (K Key, V Value) RemoveMin()
        {
            var min = Min;
      
            _items[0] = _items[--_count]; // move last item to the top
            _items[_count] = _default; // avoids object loitering if K or V is a reference type
            Sink(0); // sink to maintain heap invariant

            return min;
        }


        /// <summary>
        /// Removes the minimum element from the queue by replacing it with the given item.
        /// </summary>
        /// <param name="item"></param>
        public void ReplaceMin(K key, V value)
        {
            _items[0] = (key, value);
            Sink(0); // maintain heap invariant
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        private void Sink(int parent)
        {
            GetChildren(parent, out int leftChild, out int rightChild);

            while (leftChild < _count)
            {
                int minChild = (rightChild < _count && HasPriority(rightChild, leftChild)) ? rightChild : leftChild;
                if (!HasPriority(minChild, parent)) break; // break if heap invariant is satisfied

                _items.Swap(parent, minChild);

                parent = minChild;
                GetChildren(parent, out leftChild, out rightChild);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="leftChild"></param>
        /// <param name="rightChild"></param>
        private void GetChildren(int parent, out int leftChild, out int rightChild)
        {
            leftChild = (parent << 1) + 1;
            rightChild = leftChild + 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Insert(K key, V value)
        {
            // increment and increase array capacity if necessary
            if (_count == _items.Length)
                Array.Resize(ref _items, _items.Length << 1);

            _items[_count] = (key, value);
            Swim(_count++); // swim last element to maintain heap invariant
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        private void Swim(int child)
        {
            int parent = GetParent(child);

            while (child > 0 && HasPriority(child, parent))
            {
                _items.Swap(child, parent);

                child = parent;
                parent = GetParent(child);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private int GetParent(int child)
        {
            return (child - 1) >> 1;
        }


        /// <summary>
        /// Returns true if item i has priority over item j.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool HasPriority(int i, int j)
        {
            return _items[i].Key.CompareTo(_items[j].Key) < 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public (K Key, V Value)[] ToArray()
        {
            return _items.GetRange(0, _count);
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public V[] ToArrayDebug()
        {
            return _items.ConvertRange(0, _count, item => item.Value);
        }
        */


        /// <summary>
        /// Trims the array if capacity is greater than twice the count.
        /// </summary>
        public void TrimExcess()
        {
            int max = Math.Max(_count << 1, _minCapacity);

            if (_items.Length > max)
                Array.Resize(ref _items, max);
        }
    }
}
