
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
    public static class ListView
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ListView<T> Create<T>(List<T> source, int start, int count)
        {
            return new ListView<T>(source, start, count);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public readonly struct ListView<T>: IList<T>, IReadOnlyList<T>
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator ListView<T>(List<T> source)
        {
            return source.AsView();
        }

        #endregion


        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private List<T> _source;
            private int _index;
            private int _end;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            /// <param name="start"></param>
            /// <param name="count"></param>
            public Enumerator(List<T> source, int start, int count)
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


        private readonly List<T> _source;
        private readonly int _start;
        private readonly int _count;


        /// <summary>
        /// 
        /// </summary>
        public List<T> Source
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
        internal ListView(List<T> source, int start, int count)
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
        public ListView<T> Subview(int start, int count)
        {
            return new ListView<T>(_source, _start + start, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ReadOnlyListView<T> AsReadOnly()
        {
            return new ReadOnlyListView<T>(_source, _start, _count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_source, _start, _count);
        }


        #region Explicit Interface Implementations
        
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }


        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        
        int IList<T>.IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_source[i + _start])) return i;

            return -1;
        }

        
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }


        bool ICollection<T>.Contains(T item)
        {
            for (int i = 0; i < _count; i++)
                if (item.Equals(_source[i + _start])) return true;

            return false;
        }

        
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        
        void ICollection<T>.CopyTo(T[] destination, int destinationIndex)
        {
            for (int i = 0; i < _count; i++)
                destination[i + destinationIndex] = _source[i + _start];
        }

        
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
