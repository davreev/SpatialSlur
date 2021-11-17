
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
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ReadOnlyArrayView<T>(T[] source)
        {
            return new ReadOnlyArrayView<T>(source, 0, source.Length);
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
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        internal ReadOnlyArrayView(T[] source, int start, int count)
        {
            _source = source;
            _start = start;
            _count = count;
        }


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
        /// 
        /// </summary>
        public bool IsValid
        {
            get
            {
                return
                    _source != null &&
                    _start >= 0 &&
                    _count >= 0 &&
                    _start + _count <= _source.Length;
            }
        }


        /// <summary>
        /// Returns the item at the given index.
        /// Note that this does not perform an additional bounds check.
        /// If bounds check is needed, use ItemAt() instead.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => _source[index + _start];
        }


        /// <summary>
        /// Returns the item at the given index by reference.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T ItemAt(int index)
        {
            Utilities.BoundsCheck(index, _count);
            return _source[index + _start];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> Head(int count)
        {
            return new ArrayView<T>(_source, _start, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> Tail(int count)
        {
            return new ArrayView<T>(_source, _start + _count - count, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> Segment(int start, int count)
        {
            return new ArrayView<T>(_source, _start + start, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ArrayView<T>.Enumerator GetEnumerator()
        {
            return new ArrayView<T>.Enumerator(_source, _start, _count);
        }


        #region Explicit Interface Implementations
        
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
