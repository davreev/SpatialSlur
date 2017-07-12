using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ReadOnlySubArray<T> : IReadOnlyList<T>
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
        /// Note that this does not perform an additional bounds check.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return _source[index + _start]; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public ReadOnlySubArray(T[] source, int start, int count)
        {
            if (start < 0 || start >= source.Length)
                throw new ArgumentOutOfRangeException("start");

            if (count < 0 || start + count > source.Length)
                throw new ArgumentOutOfRangeException("count");

            _source = source ?? throw new ArgumentNullException();
            _start = start;
            _count = count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public ReadOnlySubArray(ReadOnlySubArray<T> other)
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
        public ReadOnlySubArray(ReadOnlySubArray<T> other, int start, int count)
            :this(other._source, start + other._start, count)
        {
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
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator(); // return generic version
        }


        #endregion
    }
}
