
/*
 * Notes 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StackSet<T> : IEnumerable<T>
    {
        private Stack<T> _stack;
        private HashSet<T> _set;


        /// <summary>
        /// 
        /// </summary>
        public StackSet()
        {
            _stack = new Stack<T>();
            _set = new HashSet<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        public StackSet(IEqualityComparer<T> comparer)
        {
            _stack = new Stack<T>();
            _set = new HashSet<T>(comparer);
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _stack.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return _stack.Peek();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Push(T item)
        {
            if (_set.Add(item))
            {
                _stack.Push(item);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            var item = _stack.Pop();
            _set.Remove(item);
            return item;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return _set.Contains(item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Stack<T>.Enumerator GetEnumerator()
        {
            return _stack.GetEnumerator();
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
