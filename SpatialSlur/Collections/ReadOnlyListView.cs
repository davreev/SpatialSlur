
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
    public static class ReadOnlyListView
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ReadOnlyListView<T> Create<T>(List<T> source, int start, int count)
        {
            return new ReadOnlyListView<T>(source, start, count);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public readonly struct ReadOnlyListView<T> : IReadOnlyList<T>
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ReadOnlyListView<T>(List<T> source)
        {
            return source.AsView();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ReadOnlyListView<T>(ListView<T> source)
        {
            return source.AsReadOnly();
        }

        #endregion


        private readonly List<T> _source;
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
        public bool HasSource
        {
            get => _source != null;
        }


        /// <summary>
        /// Returns true if the view is still valid.
        /// Changes to the underlying list might invalidate a view.
        /// </summary>
        public bool IsValid
        {
            get => _start + _count <= _source.Count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        internal ReadOnlyListView(List<T> source, int start, int count)
        {
            if (start < 0 || count < 0)
                throw new ArgumentOutOfRangeException();

            _source = source;
            _start = start;
            _count = count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ReadOnlyListView<T> Subview(int start, int count)
        {
            return new ReadOnlyListView<T>(_source, _start + start, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ListView<T>.Enumerator GetEnumerator()
        {
            return new ListView<T>.Enumerator(_source, _start, _count);
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
