using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class HeElementList<T> : IReadOnlyList<T> where T : HeElement
    {
        private const int MinCapacity = 4;

        private T[] _items;
        private int _count;
        private int _currTag = int.MinValue;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HeElementList(int capacity)
        {
            _items = new T[Math.Max(capacity, MinCapacity)];
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
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                return _items[index];
            }
            internal set
            {
                if (index >= _count)
                    throw new IndexOutOfRangeException();

                _items[index] = value;
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
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _items[i];
        }


        /// <summary>
        /// Explicit implementation of non-generic method.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // call generic version
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
        /// Returns the number of elements that have been flagged for removal.
        /// </summary>
        /// <returns></returns>
        public int CountRemoved()
        {
            int result = 0;

            for (int i = 0; i < _count; i++)
                if (_items[i].IsRemoved) result++;

            return result;
        }


        /// <summary>
        /// Removes all flagged elements in the list and re-indexes the remaining.
        /// Does not change the capacity of the list.
        /// If the list has any associated attributes, be sure to compact those first.
        /// </summary>
        public void Compact()
        {
            int marker = 0;

            for (int i = 0; i < _count; i++)
            {
                T element = _items[i];
                if (element.IsRemoved) continue; // skip unused elements

                element.Index = marker;
                _items[marker++] = element;
            }
            
            Array.Clear(_items, marker, _count - marker);
            _count = marker;
        }


        /// <summary>
        /// Removes all attributes corresponding with elements that have flagged for removal from the given list.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<U>(List<U> attributes)
        {
            int marker = SwimAttributes(attributes);
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// Moves all attributes corresponding with elements that haven't been flagged for removal to the front of the given list.
        /// Returns the number of attributes that haven't been flagged for removal.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public int SwimAttributes<U>(IList<U> attributes)
        {
            int marker = 0;

            for (int i = 0; i < _count; i++)
            {
                if (_items[i].IsRemoved) continue; // skip unused elements
                attributes[marker++] = attributes[i];
            }

            return marker;
        }


        /// <summary>
        /// Reduces the capacity to twice the count.
        /// If the capacity is already less than twice the count, then this does nothing.
        /// </summary>
        public void TrimExcess()
        {
            int max = Math.Max(_count << 1, MinCapacity);

            if (_items.Length > max)
                Array.Resize(ref _items, max);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="getKey"></param>
        public virtual void Sort<K>(Func<T, K> getKey)
        {
            Array.Sort(_items, 0, _count, Comparer<K>.Default);

            // reset indices
            for (int i = 0; i < _count; i++)
                _items[i].Index = i;
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
        /// Throws an exception for elements that don't belong to this mesh.
        /// </summary>
        /// <param name="element"></param>
        internal void OwnsCheck(T element)
        {
            if (!Owns(element))
                throw new ArgumentException("The given element must belong to this mesh.");
        }


        /// <summary>
        /// Returns the number of elements present in both of the given collections.
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        public int CountCommon(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            foreach (var e in elementsA.Concat(elementsB))
            {
                OwnsCheck(e);
                e.RemovedCheck();
            }

            return CountCommonImpl(elementsA, elementsB);
        }


        /// <summary>
        /// Returns the number of elements present in both of the given collections.
        /// Assumes the given elements all belong to the same mesh.
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        internal int CountCommonImpl(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            int currTag = NextTag;

            // tag elements in A
            foreach (var eA in elementsA)
                eA.Tag = currTag;

            // count tagged elements in B
            int count = 0;
            foreach (var eB in elementsB)
                if (eB.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns elements which are present in both of the given collections.
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        public IEnumerable<T> GetCommon(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            foreach (var e in elementsA.Concat(elementsB))
            {
                OwnsCheck(e);
                e.RemovedCheck();
            }

            return GetCommonImpl(elementsA, elementsB);
        }


        /// <summary>
        /// Returns elements which are present in both of the given collections.
        /// Assumes the given elements all belong to the same mesh.
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetCommonImpl(IEnumerable<T> elementsA, IEnumerable<T> elementsB)
        {
            int currTag = NextTag;

            // tag elements in A
            foreach (var eA in elementsA)
                eA.Tag = currTag;

            // return tagged elements in B
            foreach (var eB in elementsB)
                if (eB.Tag == currTag) yield return eB;
        }
    }
}
