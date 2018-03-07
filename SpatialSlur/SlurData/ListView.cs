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
    public struct ListView<T>: IList<T>, IReadOnlyList<T>
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private List<T> _source;
            private int _index;
            private int _end;
            private T _current;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            /// <param name="start"></param>
            /// <param name="count"></param>
            public Enumerator(List<T> source, int start, int count)
            {
                _source = source;
                _index = start;
                _end = start + count;
                _current = default(T);
            }


            /// <summary>
            /// 
            /// </summary>
            public T Current
            {
                get => _current;
            }


            /// <summary>
            /// 
            /// </summary>
            object IEnumerator.Current
            {
                get => _current;
            }


            /// <summary>
            /// 
            /// </summary>
            public bool MoveNext()
            {
                if (_index < _end)
                {
                    _current = _source[_index++];
                    return true;
                }

                return false;
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


        private readonly List<T> _source;
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
        /// 
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
            set
            {
                BoundsCheck(index, _count);
                _source[index + _start] = value;
            }
        }

        
        /// <summary>
        /// Returns true if the view is still valid.
        /// Changes to the underlying list might invalidate a view.
        /// </summary>
        public bool IsValid
        {
            get { return _start + _count <= _source.Count; }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public ListView(List<T> source, int start, int count)
        {
            if (start < 0 || count < 0 || start + count > source.Count)
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
        public ListView(ListView<T> other, int start, int count)
            :this(other._source, other._start + start, count)
        {
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
