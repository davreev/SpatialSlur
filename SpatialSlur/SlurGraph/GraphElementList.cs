using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="G"></typeparam>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class GraphElementList<G, T>:IEnumerable<T> 
        where T : GraphElement
    {
        private readonly G _graph;
        private T[] _list;
        private int _n;
        private int _currTag;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="capacity"></param>
        internal GraphElementList(G graph, int capacity)
        {
            _graph = graph;
            _list = new T[capacity];
        }


        /// <summary>
        /// 
        /// </summary>
        public G Graph
        {
            get { return _graph; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal int NextTag
        {
            get { return ++_currTag; } // consider reset of element tags on overflow
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _n; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get { return _list.Length; }
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
                if (index >= _n)
                    throw new IndexOutOfRangeException();

                return _list[index];
            }
            internal set
            {
                if (index >= _n)
                    throw new IndexOutOfRangeException();

                _list[index] = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _n; i++)
                yield return _list[i];
        }


        /// <summary>
        /// Explicit implementation of non-generic method.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator(); // call generic version
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        internal void Add(T element)
        {
            element.Index = _n;
            element.Tag = _currTag;

            // resize if necessary
            if (_n == _list.Length)
                Array.Resize(ref _list, _list.Length << 1);

            _list[_n++] = element;
        }


        /// <summary>
        /// 
        /// </summary>
        public abstract void Remove(T element);


        /// <summary>
        /// 
        /// </summary>
        public abstract void RemoveAt(int index);


        /// <summary>
        /// Removes all flagged elements in the list and re-indexes.
        /// If the list has any associated attributes, be sure to compact those first.
        /// </summary>
        public virtual void Compact()
        {
            int marker = 0;

            for (int i = 0; i < _n; i++)
            {
                T element = _list[i];
                if (element.IsUnused) continue; // skip unused elements

                element.Index = marker;
                _list[marker++] = element;
            }

            _n = marker;

            // trim array if length is greater than twice _n
            int maxLength = Math.Max(_n << 1, 2);
            if (_list.Length > maxLength)
                Array.Resize(ref _list, maxLength);

            // prevent object loitering
            Array.Clear(_list, _n, _list.Length - _n);
        }


        /// <summary>
        /// Removes all attributes corresponding with unused elements in the list.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<U>(List<U> attributes)
        {
            SizeCheck(attributes);
            int marker = 0;

            for (int i = 0; i < _n; i++)
            {
                if (!_list[i].IsUnused) // skip unused elements
                    attributes[marker++] = attributes[i];
            }

            // trim list to include only used elements
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// Moves all the attributes corresponding with used elements to the front of the given list.
        /// Returns the number elements still in use.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public int CompactAttributes<U>(IList<U> attributes)
        {
            SizeCheck(attributes);
            int marker = 0;

            for (int i = 0; i < _n; i++)
            {
                if (_list[i].IsUnused) continue; // skip unused elements
                attributes[marker++] = attributes[i];
            }

            return marker;
        }


        /// <summary>
        /// Returns true if the given element belongs to this list.
        /// </summary>
        public bool Owns(T element)
        {
            return element == _list[element.Index];
        }


        /// <summary>
        /// Throws an exception for elements that don't belong to this mesh.
        /// </summary>
        internal void OwnsCheck(T element)
        {
            if (!Owns(element))
                throw new ArgumentException("The given element must belong to this graph.");
        }


        /// <summary>
        /// Throws an exception for mismatched attribute lists. 
        /// </summary>
        internal void SizeCheck<U>(IList<U> attributes)
        {
            if (attributes.Count != _n)
                throw new ArgumentException("The number of attributes provided must match the number of elements in the graph.");
        }
    }
}
