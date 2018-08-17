
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
    /// <typeparam name="T"></typeparam>
    public struct ReadOnlySet<T> : IEnumerable<T>
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ReadOnlySet<T>(HashSet<T> source)
        {
            return new ReadOnlySet<T>(source);
        }

        #endregion


        private HashSet<T> _source;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public ReadOnlySet(HashSet<T> source)
        {
            _source = source;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _source.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return _source.Contains(item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HashSet<T>.Enumerator GetEnumerator()
        {
            return _source.GetEnumerator();
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
