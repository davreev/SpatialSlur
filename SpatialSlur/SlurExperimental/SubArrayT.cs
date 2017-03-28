using System;
using System.Collections;
using System.Collections.Generic;

/*
Notes
*/

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct SubArray<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly T[] _source;
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
        /// Does not perform additional bounds check
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return _source[index + _start]; }
            set { _source[index + _start] = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public SubArray(T[] source, int start, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (start < 0 || start >= source.Length)
                throw new ArgumentOutOfRangeException("start");

            if (count < 0 || start + count > source.Length)
                throw new ArgumentOutOfRangeException("count");

            _source = source;
            _start = start;
            _count = count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public SubArray(SubArray<T> other)
        {
            _source = other._source;
            _start = other._start;
            _count = other._count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public SubArray(SubArray<T> other, int start, int count)
            :this(other._source, start + other._start, count)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_source[i + _start])) return i;

            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public int IndexOf(T item, Comparison<T> compare)
        {
            for (int i = 0; i < _count; i++)
                if (compare(item, _source[i + _start]) == 0) return i;

            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_source[i + _start])) return true;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public bool Contains(T item, Comparison<T> compare)
        {
            for (int i = 0; i < _count; i++)
                if (compare(item, _source[i + _start]) == 0) return true;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="destinationIndex"></param>
        public void CopyTo(T[] destination, int destinationIndex)
        {
            for (int i = 0; i < _count; i++)
                destination[i + destinationIndex] = _source[i + _start];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator(); // return generic version
        }
    }
}

