
/*
 * Notes
 */

using System;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// Simple heap-based implementation of a min priority queue.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public class PriorityQueue<K, V>
        where K : IComparable<K>
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private static void GetChildren(int parent, out int left, out int right)
        {
            left = (parent << 1) + 1;
            right = left + 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private static int GetParent(int child)
        {
            return (child - 1) >> 1;
        }

        #endregion


        private (K Key, V Value)[] _items;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        public PriorityQueue()
        {
            _items = Array.Empty<(K, V)>();
            _count = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public PriorityQueue(int capacity)
        {
            _items = new (K, V)[capacity];
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
        /// Note that this perfoms a shallow copy elements in the queue.
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
            _items[0] = _items[--_count]; // replace with last item
            _items[_count] = default; // prevents object loitering
            Sink(0); // maintain heap invariant
            return min;
        }


        /// <summary>
        /// Removes the minimum element from the queue by replacing it with the given item.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public (K Key, V value) ReplaceMin(K key, V value)
        {
            var min = Min;
            _items[0] = (key, value);
            Sink(0); // maintain heap invariant
            return min;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        private void Sink(int parent)
        {
            GetChildren(parent, out int left, out int right);

            while (left < _count)
            {
                int min = (right < _count && HasPriority(right, left)) ? right : left;

                // break if heap invariant is satisfied
                if (!HasPriority(min, parent))
                    break; 

                _items.Swap(parent, min);
                parent = min;
                GetChildren(parent, out left, out right);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Insert(K key, V value)
        {
            const int minCapacity = 4;

            if (_items.Length == _count)
                Array.Resize(ref _items, Math.Max(_count << 1, minCapacity));
            
            _items[_count] = (key, value);
            Swim(_count++); // maintain heap invariant
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


        /// <summary>
        /// Removes all items from the queue.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_items, 0, _count);
            _count = 0;
        }
        

        /// <summary>
        /// Reduces the size of the internal array.
        /// </summary>
        public void TrimExcess()
        {
            int max = _count << 1;

            if (_items.Length > max)
                Array.Resize(ref _items, max);
        }
    }
}
