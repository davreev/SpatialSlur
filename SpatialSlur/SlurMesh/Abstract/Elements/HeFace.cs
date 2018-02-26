using System;
using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Base face class containing common implementations used by all halfedge structures.
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class HeFace<V, E, F> : HeNode<F, E>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        /// <summary>
        /// Circulates through all halfedges in this face.
        /// </summary>
        public IEnumerable<E> Halfedges
        {
            get { return First.Circulate; }
        }


        /// <summary>
        /// Circulates through all vertices in this face.
        /// </summary>
        public IEnumerable<V> Vertices
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    yield return he1.Start;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Circulates through all faces adjacent to this face.
        /// Note that if multiple edges are shared with an adjacent face, then that face will be returned multiple times.
        /// </summary>
        public IEnumerable<F> AdjacentFaces
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    var f = he1.Twin.Face;
                    if (f != null) yield return f;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Returns the number of edges in this face.
        /// </summary>
        public int Degree
        {
            get { return First.Count(); }
        }


        /// <summary>
        /// Returns true if this face has 1 edge.
        /// </summary>
        internal bool IsDegree1
        {
            get { return First.IsInDegree1; }
        }


        /// <summary>
        /// Returns true if this face has 2 edges.
        /// </summary>
        internal bool IsDegree2
        {
            get { return First.IsInDegree2; }
        }


        /// <summary>
        /// Returns true if this face has 3 edges.
        /// </summary>
        public bool IsDegree3
        {
            get { return First.IsInDegree3; }
        }


        /// <summary>
        /// Returns true if the face has one or more boundary edges.
        /// </summary>
        public bool IsBoundary
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    if (he1.Twin.Face == null) return true;
                    he1 = he1.Next;
                } while (he1 != he0);

                return false;
            }
        }


        /// <summary>
        /// Returns true if the number of edges in this face it equal to the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsDegree(int value)
        {
            return First.IsInDegree(value);
        }


        /// <summary>
        /// Sets the first halfedge in this face to the first boundary halfedge encountered during circulation.
        /// Returns true if a boundary halfedge was found.
        /// </summary>
        /// <returns></returns>
        public bool SetFirstToBoundary()
        {
            var he0 = First;
            var he1 = he0;

            do
            {
                if (he1.Twin.Face == null)
                {
                    First = he1;
                    return true;
                }

                he1 = he1.Next;
            } while (he1 != he0);

            return false;
        }


        /// <summary>
        /// Returns the first halfedge between this face and another or null if none exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public E FindHalfedge(F other)
        {
            var he0 = First;
            var he1 = he0;

            do
            {
                if (he1.Twin.Face == other) return he1;
                he1 = he1.Next;
            } while (he1 != he0);

            return null;
        }


        /// <summary>
        /// Returns the number of boundary edges in this face.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryEdges()
        {
            var he0 = First;
            var he1 = he0;
            int count = 0;

            do
            {
                if (he1.Twin.Face == null) count++;
                he1 = he1.Next;
            } while (he1 != he0);

            return count;
        }


        /// <summary>
        /// Returns the number of boundary vertices in this face.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryVertices()
        {
            var he0 = First;
            var he1 = he0;
            int count = 0;

            do
            {
                if (he1.Start.IsBoundary) count++;
                he1 = he1.Next;
            } while (he1 != he0);

            return count;
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
    public abstract class HeFace<V, E, F, G> : HeFace<V, E, F>
       where V : HeVertex<V, E, F, G>
       where E : Halfedge<V, E, F, G>
       where F : HeFace<V, E, F, G>
       where G : HeNode<G, E>
    {
        private G _cell;


        /// <summary>
        /// 
        /// </summary>
        public G Cell
        {
            get => _cell;
            internal set => _cell = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public F Twin
        {
            get => First.Adjacent.Face;
        }
    }
}
