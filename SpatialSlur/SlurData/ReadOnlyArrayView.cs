using System;
using System.Collections;
using System.Collections.Generic;

using static SpatialSlur.SlurCore.CoreUtil;

/*
 * Notes
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct ReadOnlyArrayView<T> : IReadOnlyList<T>
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
            get
            {
                BoundsCheck(index, _count);
                return _source[index + _start];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public ReadOnlyArrayView(T[] source, int start, int count)
        {
            if (start < 0 || count < 0 || start + count > source.Length)
                throw new ArgumentOutOfRangeException();

            _source = source;
            _start = start;
            _count = count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public ReadOnlyArrayView(ReadOnlyArrayView<T> other, int start, int count)
            :this(other._source, other._start + start, count)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ArrayView<T>.Enumerator GetEnumerator()
        {
            return new ArrayView<T>.Enumerator(_source, _start, _count);
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
