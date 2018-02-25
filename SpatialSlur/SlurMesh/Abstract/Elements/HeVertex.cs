using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Base vertex class containing common implementations used by all halfedge structures.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class HeVertex<V, E> : HeNode<V, E>
        where V : HeVertex<V, E>
        where E : Halfedge<V, E>
    {
        /// <summary>
        /// Circulates through all halfedges starting at this vertex.
        /// </summary>
        public IEnumerable<E> OutgoingHalfedges
        {
            get { return First.CirculateStart; }
        }


        /// <summary>
        /// Circulates through all halfedges ending at this vertex.
        /// </summary>
        public IEnumerable<E> IncomingHalfedges
        {
            get { return First.Twin.CirculateEnd; }
        }


        /// <summary>
        /// Circulates through all vertices connected to this vertex.
        /// </summary>
        public IEnumerable<V> ConnectedVertices
        {
            get
            {
                var he0 = First.Twin;
                var he1 = he0;

                do
                {
                    yield return he1.Start;
                    he1 = he1.NextAtEnd;
                } while (he1 != he0);
            }
        }

        
        /// <summary>
        /// Returns the number of edges at this vertex.
        /// </summary>
        public int Degree
        {
            get { return First.CountAtStart(); }
        }

        
        /// <summary>
        /// Returns true if this vertex has 1 outgoing halfedge.
        /// </summary>
        public bool IsDegree1
        {
            get { return First.IsAtDegree1; }
        }


        /// <summary>
        /// Returns true if the vertex has 2 outgoing halfedges.
        /// </summary>
        public bool IsDegree2
        {
            get { return First.IsAtDegree2; }
        }


        /// <summary>
        /// Returns true if the vertex has 3 outgoing halfedges.
        /// </summary>
        public bool IsDegree3
        {
            get { return First.IsAtDegree3; }
        }
        

        /// <summary>
        /// Returns true if the number of edges at this vertex is equal to the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsDegree(int value)
        {
            return First.IsAtDegree(value);
        }


        /// <summary>
        /// Returns true if this vertex is connected to the given vertex.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsConnectedTo(V other)
        {
            return FindHalfedgeTo(other) != null;
        }


        /// <summary>
        /// Returns a halfedge from this vertex to another or null if none exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public E FindHalfedgeTo(V other)
        {
            var he0 = First.Twin;
            var he1 = he0;

            do
            {
                if (he1.Start == other) return he1.Twin;
                he1 = he1.NextAtEnd;
            } while (he1 != he0);

            return null;
        }
    }


    /// <summary>
    /// Base vertex class containing common implementations used by all halfedge structures.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class HeVertex<V, E, F> : HeVertex<V, E>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        /// <summary>
        /// Circulates through all faces surrounding this vertex.
        /// Note that if multiple outgoing halfedges lie on the same face, that face will be returned multiple times.
        /// </summary>
        public IEnumerable<F> SurroundingFaces
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    var f = he1.Face;
                    if (f != null) yield return f;
                    he1 = he1.NextAtStart;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Returns true if this vertex is on the mesh boundary.
        /// </summary>
        public bool IsBoundary
        {
            get { return First.Face == null; }
        }


        /// <summary>
        /// Returns false if the vertex has more than one boundary edge (i.e. bowtie condition).
        /// </summary>
        public bool IsManifold
        {
            get
            {
                var he = First;

                // interior vertex, can assume manifold
                if (he.Face != null)
                    return true;

                // boundary vertex, check for second boundary
                he = he.NextAtStart;
                do
                {
                    if (he.Face == null) return false;
                } while (he != First);

                return true;
            }
        }


        /// <summary>
        /// Return true if this vertex is degree 2 and on the mesh boundary. 
        /// </summary>
        public bool IsCorner
        {
            get { return IsBoundary && IsDegree2; }
        }


        /// <summary>
        /// Sets the first halfedge of this vertex to the first boundary halfedge encountered during circulation.
        /// Returns true if a boundary halfedge was found.
        /// </summary>
        /// <returns></returns>
        internal bool SetFirstToBoundary()
        {
            var he0 = First;
            var he1 = he0;

            do
            {
                if (he1.Face == null)
                {
                    First = he1;
                    return true;
                }

                he1 = he1.NextAtStart;
            } while (he1 != he0);

            return false;
        }
    }
}
