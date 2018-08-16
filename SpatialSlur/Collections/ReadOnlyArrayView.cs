
/*
 * Notes
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReadOnlyArrayView
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ReadOnlyArrayView<T> Create<T>(T[] source, int start, int count)
        {
            return new ReadOnlyArrayView<T>(source, start, count);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public readonly struct ReadOnlyArrayView<T> : IReadOnlyList<T>
    {
        #region Static members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ReadOnlyArrayView<T>(T[] source)
        {
            return source.AsView();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ReadOnlyArrayView<T>(ArrayView<T> source)
        {
            return source.AsReadOnly();
        }

        #endregion


        private readonly T[] _source;
        private readonly int _start;
        private readonly int _count;


        /// <summary>
        /// 
        /// </summary>
        public int Start
        {
            get => _start;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get => _count;
        }


        /// <summary>
        /// Gets the element at the given index with respect to this view.
        /// Note that this does not perform an additional bounds check.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => _source[index + _start];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        internal ReadOnlyArrayView(T[] source, int start, int count)
        {
            if (start < 0 || count < 0 || start + count > source.Length)
                throw new ArgumentOutOfRangeException();

            _source = source;
            _start = start;
            _count = count;
        }


        /// <summary>
        /// Performs an action over this view.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parallel"></param>
        public void Action(Action<T> action, bool parallel = false)
        {
            _source.ActionRange(_start, _count, action, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ReadOnlyArrayView<T> Subview(int start, int count)
        {
            return new ReadOnlyArrayView<T>(_source, _start + start, count);
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
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
