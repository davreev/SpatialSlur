
/*
 * Notes
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct ArrayView<T> : IList<T>, IReadOnlyList<T>
    {
        #region Nested types
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public struct Enumerator : IEnumerator<T>
        {
            private T[] _source;
            private int _index;
            private int _end;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            /// <param name="start"></param>
            /// <param name="count"></param>
            public Enumerator(T[] source, int start, int count)
            {
                _source = source;
                _index = start - 1;
                _end = start + count;
            }


            /// <summary>
            /// 
            /// </summary>
            public T Current
            {
                get => _source[_index];
            }

            
            /// <summary>
            /// 
            /// </summary>
            object IEnumerator.Current
            {
                get => Current;
            }


            /// <summary>
            /// 
            /// </summary>
            public bool MoveNext()
            {
                return ++_index < _end;
            }


            /// <summary>
            /// 
            /// </summary>
            public void Reset()
            {
                throw new NotImplementedException();
            }


            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
            }
        }

        #endregion


        private readonly T[] _source;
        private readonly int _start;
        private readonly int _count;


        /// <summary>
        /// 
        /// </summary>
        public T[] Source
        {
            get { return _source; }
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
        /// Gets or sets the element at the given index with respect to this view.
        /// Note that this does not perform an additional bounds check.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => _source[index + _start];
            set => _source[index + _start] = value;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public ArrayView(T[] source, int start, int count)
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
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ArrayView<T> GetSubView(int start, int count)
        {
            return new ArrayView<T>(_source, _start + start, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ReadOnlyArrayView<T> AsReadOnly()
        {
            return new ReadOnlyArrayView<T>(_source, _start, _count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_source, _start, _count);
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int IList<T>.IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_source[i + _start])) return i;

            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.Contains(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_source[i + _start])) return true;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="destinationIndex"></param>
        void ICollection<T>.CopyTo(T[] destination, int destinationIndex)
        {
            for (int i = 0; i < _count; i++)
                destination[i + destinationIndex] = _source[i + _start];
        }


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

