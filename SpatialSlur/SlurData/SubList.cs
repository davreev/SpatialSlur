using System;
using System.Collections;
using System.Collections.Generic;

using SpatialSlur.SlurCore;

using static SpatialSlur.SlurCore.CoreUtil;

/*
Notes
*/

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct SubList<T>: IList<T>, IReadOnlyList<T>
    {
        private readonly IList<T> _source;
        private readonly int _start;
        private readonly int _count;


        /// <summary>
        /// 
        /// </summary>
        public int Start
        {
            get { return _start; }
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
        public bool IsReadOnly
        {
            get { return false; }
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
                BoundsCheck(index, _count);
                return _source[index + _start];
            }
            set
            {
                BoundsCheck(index, _count);
                _source[index + _start] = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public SubList(IList<T> source, int start, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if(start < 0 || start >= source.Count)
                throw new ArgumentOutOfRangeException("start");

            if (count < 0 || start + count > source.Count)
                throw new ArgumentOutOfRangeException("count");

            // avoid performance degradation from recursive referencing
            if (source is SubList<T>)
            {
                var other = (SubList<T>) source;
                _source = other._source;
                _start = start + other._start;
            }
            else
            {
                _source = source;
                _start = start;
            }

            _count = count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _source[i + _start];
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int IList<T>.IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_source[i + _start])) return i;

            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.Contains(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_source[i + _start])) return true;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="destinationIndex"></param>
        void ICollection<T>.CopyTo(T[] destination, int destinationIndex)
        {
            for (int i = 0; i < _count; i++)
                destination[i + destinationIndex] = _source[i + _start];
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
