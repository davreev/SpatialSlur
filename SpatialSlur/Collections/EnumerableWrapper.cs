/*
 * Notes
 */

using System.Collections;
using System.Collections.Generic;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// Wrapper for value type IEnumerables to mitigate performance loss from boxing/unboxing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class EnumerableWrapper<T, U> : IEnumerable<U>
        where T : struct, IEnumerable<U>
    {
        private T _source;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public EnumerableWrapper(T source)
        {
            _source = source;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<U> GetEnumerator()
        {
            return _source.GetEnumerator();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source.GetEnumerator();
        }
    }
}
