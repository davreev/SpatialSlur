using SpatialSlur.SlurCore;
using System;
using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Base halfedge class containing common implementations used by all halfedge structures.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class Halfedge<E> : HeElement<E>
        where E : Halfedge<E>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<E, V> CreateEdgeProperty<V>(V[] values)
        {
            return new Property<E, V>(he => values[he >> 1], (he, v) => values[he >> 1] = v);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Property<E, V> CreateEdgeProperty<V>(IList<V> values)
        {
            return new Property<E, V>(he => values[he >> 1], (he, v) => values[he >> 1] = v);
        }

        #endregion


        private E _twin;
        private E _previous;
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
            get { return _previous; }
            internal set { _previous = value; }
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
        /// Returns the previous halfedge at the start vertex.
        /// </summary>
        public E PreviousAtStart
        {
            get { return _previous._twin; }
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
            get { return _twin._previous; }
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
        /// Returns the first halfedge from each pair connected to this edge.
        /// </summary>
        public IEnumerable<E> ConnectedEdges
        {
            get
            {
                if (_previous != _twin)
                {
                    var e0 = _previous.Edge;
                    var e1 = _twin._next.Edge;

                    yield return e0;
                    if (e1 != e0) yield return e1;
                }

                if (_next != _twin)
                {
                    var e0 = _next.Edge;
                    var e1 = _twin._previous.Edge;

                    yield return e0;
                    if (e1 != e0) yield return e1;
                }
            }
        }


        /// <summary>
        /// Returns true if this element is not being used by the halfedge structure i.e. its first halfedge is null.
        /// </summary>
        /// <returns></returns>
        public bool IsUnused
        {
            get { return _previous == null; }
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
            get { return this != _next && _previous == _next; }
        }


        /// <summary>
        /// Returns true if this halfedge is in a 3-sided loop.
        /// </summary>
        public bool IsInDegree3
        {
            get { return this != _next && _previous == _next._next; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void MakeUnused()
        {
            _previous = Twin._previous = null;
        }


        /// <summary>
        /// 
        /// </summary>
        internal void UnusedCheck()
        {
            const string errorMessage = "This element is not in use. The operation cannot be performed.";

            if (IsUnused)
                throw new ArgumentException(errorMessage);
        }


        /// <summary>
        /// Returns the number of edges in the loop.
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
                he = he._previous;

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
            other._previous = Self;
        }
    }


    /// <summary>
    /// Base halfedge class containing common implementations used by all halfedge structures.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class Halfedge<V, E> : Halfedge<E>
        where V : HeNode<V, E>
        where E : Halfedge<V, E>
    {
        private V _start;


        /// <summary>
        /// Returns the vertex at the start of this halfedge.
        /// </summary>
        public V Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// Returns the vertex at the end of this halfedge.
        /// </summary>
        public V End
        {
            get { return Twin._start; }
        }


        /// <summary>
        /// Returns true if this halfedge is the first at its start vertex.
        /// </summary>
        public bool IsFirstAtStart
        {
            get { return this == _start.First; }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class Halfedge<V, E, F> : Halfedge<V, E>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        private F _face;


        /// <summary>
        /// Returns the face adjacent to this halfedge.
        /// If this halfedge is adjacent to a hole, null is returned.
        /// </summary>
        public F Face
        {
            get { return _face; }
            internal set { _face = value; }
        }


        /// <summary>
        /// Returns true if this halfedge is the first in its face.
        /// </summary>
        public bool IsFirstInFace
        {
            get { return this == Face.First; }
        }


        /// <summary>
        /// Returns true if this halfedge or its twin is adjacent to a hole.
        /// </summary>
        public bool IsBoundary
        {
            get { return Face == null || Twin.Face == null; }
        }

        
        /// <summary>
        /// Returns true if this halfedge is adjacent to a hole.
        /// </summary>
        public bool IsHole
        {
            get { return Face == null; }
        }


        /// <summary>
        /// Returns true if the halfedge and its twin have different faces.
        /// </summary>
        internal bool IsManifold
        {
            get { return Face != Twin.Face; }
        }
        

        /// <summary>
        /// Returns true this halfedge spans between non-consecutive boundary vertices.
        /// </summary>
        public bool IsBridge
        {
            get { return Start.IsBoundary && End.IsBoundary && !IsBoundary; }
        }

        
        /// <summary>
        /// Returns the next boundary halfedge encountered when circulating the loop of this halfedge.
        /// If no such halfedge is found, null is returned.
        /// </summary>
        /// <returns></returns>
        public E NextBoundary
        {
            get
            {
                var he1 = Next;

                do
                {
                    if (he1.Twin.Face == null) return he1;
                    he1 = he1.Next;
                } while (he1 != this);

                return null;
            }
        }

        
        /// <summary>
        /// Returns the next faceless halfedge encountered when circulating around the start vertex of this halfedge.
        /// If no such halfedge is found, null is returned.
        /// </summary>
        /// <returns></returns>
        public E NextBoundaryAtStart
        {
            get
            {
                var he = NextAtStart;

                do
                {
                    if (he.Face == null) return he;
                    he = he.NextAtStart;
                } while (he != this);

                return null;
            }
        }


        /// <summary>
        /// Returns the next faceless halfedge encountered when circulating around the end vertex of this halfedge.
        /// If no such halfedge is found, null is returned.
        /// </summary>
        /// <returns></returns>
        public E NextBoundaryAtEnd
        {
            get
            {
                var he = NextAtEnd;

                do
                {
                    if (he.Face == null) return he;
                    he = he.NextAtEnd;
                } while (he != this);

                return null;
            }
        }

        
        /// <summary>
        /// Sets this halfedge to be the first in its face.
        /// </summary>
        public void MakeFirstInFace()
        {
            Face.First = Self;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    [Serializable]
    public abstract class Halfedge<V, E, F, G> : Halfedge<V, E, F>
        where V : HeVertex<V, E, F, G>
        where E : Halfedge<V, E, F, G>
        where F : HeFace<V, E, F, G>
        where G : HeNode<G, E>
    {
        private E _adjacent;
        private G _bundle;


        /// <summary>
        /// 
        /// </summary>
        public E Adjacent
        {
            get => _adjacent;
            internal set => _adjacent = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public G Bundle
        {
            get => _bundle;
            internal set => _bundle = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsFirstInBundle
        {
            get => this == _bundle.First;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsFirstInCluster
        {
            get => this == Start.Cluster.First;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsFirstInCell
        {
            get => this == Face.Cell.First;
        }


        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<E> CirculateBundle
        {
            get
            {
                var he = Self;

                do
                {
                    yield return he;
                    he = he.Twin._adjacent;
                } while (he != this);
            }
        }
    }
}
