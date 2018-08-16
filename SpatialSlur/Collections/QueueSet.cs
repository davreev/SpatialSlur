
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
    public class QueueSet<T> : IEnumerable<T>
    {
        private Queue<T> _queue;
        private HashSet<T> _set;

        
        /// <summary>
        /// 
        /// </summary>
        public QueueSet()
        {
            _queue = new Queue<T>();
            _set = new HashSet<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        public QueueSet(IEqualityComparer<T> comparer)
        {
            _queue = new Queue<T>();
            _set = new HashSet<T>(comparer);
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _queue.Count; }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return _queue.Peek();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Enqueue(T item)
        {
            if (_set.Add(item))
            {
                _queue.Enqueue(item);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            var item = _queue.Dequeue();
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
        public Queue<T>.Enumerator GetEnumerator()
        {
            return _queue.GetEnumerator();
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
