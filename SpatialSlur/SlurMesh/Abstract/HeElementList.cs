
/*
 * Notes
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class HeElementList<T> : IReadOnlyList<T>
        where T : HeElement
    {
        #region Static

        private const int _minCapacity = 4;

        #endregion


        private T[] _items;
        private int _count;
        private int _currTag = int.MinValue;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        internal HeElementList(int capacity)
        {
            _items = new T[Math.Max(capacity, _minCapacity)];
        }


        /// <summary>
        /// 
        /// </summary>
        internal int NextTag
        {
            get
            {
                if (_currTag == int.MaxValue) ResetTags();
                return ++_currTag;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected T[] Items
        {
            get { return _items; }
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
            get { return _items.Length; }
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
                if (index < _count) return _items[index];
                throw new IndexOutOfRangeException();
            }
            internal set
            {
                if (index < _count) _items[index] = value;
                throw new IndexOutOfRangeException();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ResetTags()
        {
            _currTag = int.MinValue;

            for (int i = 0; i < _count; i++)
                _items[i].Tag = int.MinValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ArrayView<T>.Enumerator GetEnumerator()
        {
            return _items.GetView(_count).GetEnumerator();
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
            if (_count == _items.Length)
                Array.Resize(ref _items, _items.Length << 1);

            _items[_count++] = element;
        }


        /// <summary>
        /// Reduces the capacity to twice the count.
        /// If the capacity is already less than twice the count, then this function does nothing.
        /// </summary>
        public void TrimExcess()
        {
            int max = Math.Max(_count << 1, _minCapacity);

            if (_items.Length > max)
                Array.Resize(ref _items, max);
        }


        /// <summary>
        /// Returns the number of unused elements in the list.
        /// </summary>
        public abstract int CountUnused();


        /// <summary>
        /// Removes all unused elements in the list and re-indexes the remaining.
        /// Does not change the capacity of the list.
        /// If the list has any associated attributes, be sure to compact those first.
        /// </summary>
        public abstract void Compact();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="marker"></param>
        protected void AfterCompact(int marker)
        {
            Array.Clear(_items, marker, _count - marker);
            _count = marker;
        }


        /// <summary>
        /// Removes all attributes corresponding with unused elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributes"></param>
        public abstract void CompactAttributes<A>(List<A> attributes);


        /// <summary>
        /// Moves attributes corresponding with used elements to the front of the given list.
        /// </summary>
        /// <param name="attributes"></param>
        public abstract int SwimAttributes<A>(IList<A> attributes);


        /// <summary>
        /// Moves attributes corresponding with used elements to the front of the given array.
        /// </summary>
        /// <param name="attributes"></param>
        public abstract int SwimAttributes<A>(A[] attributes);


        /// <summary>
        /// Reorders the elements in the list based on the given key.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="getKey"></param>
        public virtual void Sort<K>(Func<T, K> getKey)
            where K : IComparable<K>
        {
            int index = 0;

            // sort first
            foreach (var t in this.OrderBy(getKey))
                Items[index++] = t;

            // re-index after since indices may be used to fetch keys
            for (int i = 0; i < Count; i++)
                Items[i].Index = i;
        }


        /// <summary>
        /// Returns true if the given element belongs to this list.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Owns(T element)
        {
            return element == _items[element.Index];
        }

        
        /// <summary>
        /// Throws an exception for elements that don't belong to this list.
        /// </summary>
        /// <param name="element"></param>
        internal void OwnsCheck(T element)
        {
            const string errorMessage = "The given element must belong to this mesh.";

            if (!Owns(element))
                throw new ArgumentException(errorMessage);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        private void OwnsCheck(IEnumerable<T> elements)
        {
            foreach (var e in elements)
                OwnsCheck(e);
        }


        /// <summary>
        /// Returns unique elements from the given collection (no duplicates).
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public IEnumerable<T> GetDistinct(IEnumerable<T> elements)
        {
            OwnsCheck(elements);
            return GetDistinctImpl(elements);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetDistinctImpl(IEnumerable<T> elements)
        {
            int currTag = NextTag;

            foreach (var e in elements)
            {
                if (e.Tag == currTag) continue;
                e.Tag = currTag;
                yield return e;
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        public IEnumerable<T> GetUnion(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            return GetDistinct(elementsA.Concat(elementsB));
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        public IEnumerable<T> GetDifference(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            OwnsCheck(elementsA);
            OwnsCheck(elementsB);
            return GetDifferenceImpl(elementsA, elementsB);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetDifferenceImpl(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            int currTag = NextTag;

            // tag elements in A
            foreach (var eB in elementsB)
                eB.Tag = currTag;

            // return tagged elements in B
            foreach (var eA in elementsA)
                if (eA.Tag != currTag) yield return eA;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        public IEnumerable<T> GetIntersection(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            OwnsCheck(elementsA);
            OwnsCheck(elementsB);
            return GetIntersectionImpl(elementsA, elementsB);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetIntersectionImpl(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            int currTag = NextTag;

            // tag elements in A
            foreach (var eA in elementsA)
                eA.Tag = currTag;

            // return tagged elements in B
            foreach (var eB in elementsB)
                if (eB.Tag == currTag) yield return eB;
        }


        #region Explicit interface implementations
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
