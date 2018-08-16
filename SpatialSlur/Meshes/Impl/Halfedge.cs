
/*
 * Notes
 */

using SpatialSlur;
using System;
using System.Collections;
using System.Collections.Generic;

using SpatialSlur.Collections;

namespace SpatialSlur.Meshes.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class Halfedge<E> : Node<E>
        where E : Halfedge<E>
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<E, U> CreateEdgeProperty<U>(U[] values)
        {
            return new Property<E, U>(he => values[he.EdgeIndex], (he, u) => values[he.EdgeIndex] = u);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<E, U> CreateEdgeProperty<U>(ArrayView<U> values)
        {
            return new Property<E, U>(he => values[he.EdgeIndex], (he, u) => values[he.EdgeIndex] = u);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<E, U> CreateEdgeProperty<U>(IList<U> values)
        {
            return new Property<E, U>(he => values[he.EdgeIndex], (he, u) => values[he.EdgeIndex] = u);
        }

        #endregion


        private E _twin;
        private E _prev;
        private E _next;


        /// <summary>
        /// Returns the oppositely oriented halfedge in the pair.
        /// </summary>
        public E Twin
        {
            get { return _twin; }
            internal set { _twin = value; }
        }


        /// <summary>
        /// Returns the previous halfedge around the face or loop.
        /// </summary>
        public E Previous
        {
            get { return _prev; }
            internal set { _prev = value; }
        }


        /// <summary>
        /// Returns the next halfedge around the face or loop.
        /// </summary>
        public E Next
        {
            get { return _next; }
            internal set { _next = value; }
        }


        /// <summary>
        /// Returns the edge that this halfedge belongs to.
        /// Note that edges are implicitly represented via their first halfedge.
        /// </summary>
        public E Edge
        {
            get { return Self < _twin ? Self : _twin; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int EdgeIndex
        {
            get { return Index >> 1; }
        }


        /// <summary>
        /// Returns the previous halfedge at the start vertex.
        /// </summary>
        public E PreviousAtStart
        {
            get { return _prev._twin; }
        }


        /// <summary>
        /// Returns the next halfedge at the start vertex.
        /// </summary>
        public E NextAtStart
        {
            get { return _twin._next; }
        }


        /// <summary>
        /// Returns the previous halfedge at the end vertex.
        /// </summary>
        public E PreviousAtEnd
        {
            get { return _twin._prev; }
        }


        /// <summary>
        /// Returns the next halfedge at the end vertex.
        /// </summary>
        public E NextAtEnd
        {
            get { return _next._twin; }
        }


        /// <summary>
        /// Forward circulates through all halfedges in the loop.
        /// </summary>
        public IEnumerable<E> Circulate
        {
            get
            {
                var he = Self;

                do
                {
                    yield return he;
                    he = he._next;
                } while (he != this);
            }
        }


        /// <summary>
        /// Forward circulates through all outgoing halfedges at the start vertex.
        /// </summary>
        public IEnumerable<E> CirculateStart
        {
            get
            {
                var he = Self;

                do
                {
                    yield return he;
                    he = he.NextAtStart;
                } while (he != this);
            }
        }


        /// <summary>
        /// Forward circulates through all incoming halfedges at the end vertex.
        /// </summary>
        public IEnumerable<E> CirculateEnd
        {
            get
            {
                var he = Self;

                do
                {
                    yield return he;
                    he = he.NextAtEnd;
                } while (he != this);
            }
        }


        /// <summary>
        /// Returns true if this element is not being used by the halfedge structure.
        /// </summary>
        /// <returns></returns>
        public bool IsUnused
        {
            get { return _prev == null; }
        }


        /// <summary>
        /// Returns true if this halfedge is the first in its edge.
        /// </summary>
        public bool IsFirstInEdge
        {
            get { return (Index & 1) == 0; }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 1 vertex.
        /// </summary>
        public bool IsAtDegree1
        {
            get { return this == NextAtStart; }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 2 vertex.
        /// </summary>
        public bool IsAtDegree2
        {
            get
            {
                var he = NextAtStart;
                return this != he && PreviousAtStart == he;
            }
        }


        /// <summary>
        /// Returns true if this halfedge starts at a degree 3 vertex.
        /// </summary>
        public bool IsAtDegree3
        {
            get
            {
                var he = NextAtStart;
                return this != he && PreviousAtStart == he.NextAtStart;
            }
        }


        /// <summary>
        /// Returns true if this halfedge is in a 1-sided loop.
        /// </summary>
        public bool IsInDegree1
        {
            get { return this == _next; }
        }


        /// <summary>
        /// Returns true if this halfedge is in a 2-sided loop.
        /// </summary>
        public bool IsInDegree2
        {
            get { return this != _next && _prev == _next; }
        }


        /// <summary>
        /// Returns true if this halfedge is in a 3-sided loop.
        /// </summary>
        public bool IsInDegree3
        {
            get { return this != _next && _prev == _next._next; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void MakeUnused()
        {
            _prev = Twin._prev = null;
        }


        /// <summary>
        /// 
        /// </summary>
        internal void UnusedCheck()
        {
            const string errorMessage = "This halfedge is not in use. The operation cannot be performed.";

            if (IsUnused)
                throw new ArgumentException(errorMessage);
        }


        /// <summary>
        /// Returns the number of halfedges in the loop.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            var he = Self;
            var n = 0;

            do
            {
                n++;
                he = he.Next;
            } while (he != this);

            return n;
        }


        /// <summary>
        /// Returns the number of edges at the start vertex.
        /// </summary>
        /// <returns></returns>
        public int CountAtStart()
        {
            var he = Self;
            var n = 0;

            do
            {
                n++;
                he = he.NextAtStart;
            } while (he != this);

            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public E GetPrevious(int offset)
        {
            var he = Self;

            for (int i = 0; i < offset; i++)
                he = he._prev;

            return he;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public E GetNext(int offset)
        {
            var he = Self;

            for (int i = 0; i < offset; i++)
                he = he._next;

            return he;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public E GetPreviousAtStart(int offset)
        {
            var he = Self;

            for (int i = 0; i < offset; i++)
                he = he.PreviousAtStart;

            return he;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public E GetNextAtStart(int offset)
        {
            var he = Self;

            for (int i = 0; i < offset; i++)
                he = he.NextAtStart;

            return he;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public E GetPreviousAtEnd(int offset)
        {
            var he = Self;

            for (int i = 0; i < offset; i++)
                he = he.PreviousAtEnd;

            return he;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public E GetNextAtEnd(int offset)
        {
            var he = Self;

            for (int i = 0; i < offset; i++)
                he = he.NextAtEnd;

            return he;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsInDegree(int value)
        {
            var he = Self;

            do
            {
                if (--value < 0) return false;
                he = he._next;
            } while (he != this);

            return value == 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsAtDegree(int value)
        {
            var he = Self;

            do
            {
                if (--value < 0) return false;
                he = he.NextAtStart;
            } while (he != this);

            return value == 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        internal void MakeConsecutive(E other)
        {
            _next = other;
            other._prev = Self;
        }
    }
}
