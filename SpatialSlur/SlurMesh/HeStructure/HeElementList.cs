using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="O"></typeparam>
    /// <typeparam name="T"></typeparam>
    [Serializable]
        public abstract class HeElementList<O, T> : IReadOnlyList<T>
        where T : HeElement
    {
        private O _owner;
        private T[] _list;
        private int _count;
        private int _currTag = int.MinValue;


        /// <summary>
        /// Returns the object that this list belongs to.
        /// </summary>
        internal O Owner
        {
            get { return _owner; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal int NextTag
        {
            get
            {
                // reset element tags on overflow
                if (_currTag == int.MaxValue)
                {
                    _currTag = int.MinValue;
                    foreach (var t in _list) t.Tag = _currTag;
                }
        
                return ++_currTag;
            }
        }
  

        /// <summary>
        /// 
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
            get { return _list.Length; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                return _list[index];
            }
            internal set
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                _list[index] = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal abstract T CreateElement();


        /// <summary>
        /// Call from concrete constructor to assign private fields.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="capacity"></param>
        protected void Initialize(O owner, int capacity)
        {
            _owner = owner;
            _list = new T[capacity];
        }

  
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _list[i];
        }


        /// <summary>
        /// Explicit implementation of non-generic method.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator(); // call generic version
        }


        /// <summary>
        /// Adds an element to the list.
        /// </summary>
        /// <param name="element"></param>
        internal void Add(T element)
        {
            element.Index = _count;
            element.Tag = _currTag;

            // resize if necessary
            if (_count == Capacity)
                Array.Resize(ref _list, Capacity << 1);

            _list[_count++] = element;
        }


        /// <summary>
        /// Returns the number of unused elements in the list.
        /// </summary>
        /// <returns></returns>
        public int CountUnused()
        {
            int result = 0;

            for (int i = 0; i < _count; i++)
                if (_list[i].IsUnused) result++;

            return result;
        }


        /// <summary>
        /// Removes all unused elements in the list and re-indexes.
        /// If the list has any associated attributes, be sure to compact those first.
        /// </summary>
        public void Compact()
        {
            int marker = 0;

            for (int i = 0; i < _count; i++)
            {
                T element = _list[i];
                if (element.IsUnused) continue; // skip unused elements

                element.Index = marker;
                _list[marker++] = element;
            }

            _count = marker;
            TrimExcess();
        }


        /// <summary>
        /// Trims the array if capacity is greater than twice the count
        /// </summary>
        private void TrimExcess()
        {
            int maxCapacity = Math.Max(_count << 1, 2);

            if (Capacity > maxCapacity)
                Array.Resize(ref _list, maxCapacity);

            // prevent object loitering
            Array.Clear(_list, _count, Capacity - _count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<U>(List<U> attributes)
        {
            int marker = 0;

            for (int i = 0; i < _count; i++)
            {
                if (!_list[i].IsUnused) // skip unused elements
                    attributes[marker++] = attributes[i];
            }

            // trim list to include only used elements
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// Moves all the attributes corresponding with used elements to the front of the given list.
        /// Returns the number elements still in use.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public int CompactAttributes<U>(IList<U> attributes)
        {
            int marker = 0;

            for (int i = 0; i < _count; i++)
            {
                if (_list[i].IsUnused) continue; // skip unused elements
                attributes[marker++] = attributes[i];
            }

            return marker;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="keys"></param>
        public void Sort<U>(U[] keys)
        {
            Array.Sort(keys, _list, 0, _count);

            // re-index sorted elements
            for (int i = 0; i < _count; i++)
                _list[i].Index = i;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare"></param>
        public void Sort(Comparison<T> compare)
        {
            Sort(Comparer<T>.Create(compare)); // converts delegate to Comparer<T> instance
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer)
        {
            Array.Sort(_list, 0, _count, comparer);
   
            // re-index sorted elements
            for (int i = 0; i < _count; i++)
                _list[i].Index = i;
        }


        /// <summary>
        /// Returns true if the given element belongs to this list.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Owns(T element)
        {
            return element == _list[element.Index];
        }


        /// <summary>
        /// Throws an exception for elements that don't belong to this mesh.
        /// </summary>
        /// <param name="element"></param>
        internal void OwnsCheck(T element)
        {
            if (!Owns(element))
                throw new ArgumentException("The given element must belong to this mesh.");
        }
    }
}
