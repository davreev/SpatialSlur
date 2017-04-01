using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public abstract class HeVertex<E, V> : HeElement
        where E : Halfedge<E, V>
        where V : HeVertex<E, V>
    {
        private E _first;


        /// <summary>
        /// 
        /// </summary>
        internal abstract V Self { get; }


        /// <summary>
        /// 
        /// </summary>
        public E First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// Returns true if this node has any incident used edges.
        /// </summary>
        public sealed override bool IsUnused
        {
            get { return _first == null; }
        }


        /// <summary>
        /// Returns the number of edges incident to this node.
        /// </summary>
        public int Degree
        {
            get { return _first.CountEdgesAtStart(); }
        }


        /// <summary>
        /// Iterates over all outgoing halfedges.
        /// </summary>
        public abstract IEnumerable<E> OutgoingHalfedges { get; }


        /// <summary>
        /// Iterates over all incoming halfedges.
        /// </summary>
        public abstract IEnumerable<E> IncomingHalfedges { get; }


        /// <summary>
        /// Iterates over all connected vertices.
        /// </summary>
        public abstract IEnumerable<V> ConnectedVertices { get; }


        /// <summary>
        /// 
        /// </summary>
        internal sealed override void MakeUnused()
        {
            _first = null;
        }


        /// <summary>
        /// Returns true if an edge exists between this node and the given node.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsConnectedTo(V other)
        {
            return FindHalfedgeTo(other) != null;
        }


        /// <summary>
        /// Searches for an edge between this node and the given node.
        /// Returns the halfedge starting from this vertex or null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract E FindHalfedgeTo(V other);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class HeVertex<E, V, F> : HeVertex<E, V>
        where E : Halfedge<E, V, F>
        where V : HeVertex<E, V, F>
        where F : HeFace<E, V, F>
    {
        /// <summary>
        /// Returns true if the vertex lies on the mesh boundary.
        /// Note that if this is true, the first halfedge must have a null face reference.
        /// </summary>
        public bool IsBoundary
        {
            get { return First.Face == null; }
        }


        /// <summary>
        /// Iterates over all surrounding faces.
        /// Note that null faces are skipped.
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
                    he1 = he1.Twin.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Sets the first halfedge to the first boundary halfedge encountered during circulation.
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

                he1 = he1.Twin.Next;
            } while (he1 != he0);

            return false;
        }


        #region Geometric Attributes

        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormal(IReadOnlyList<Vec3d> vertexPositions)
        {
            Vec3d result = new Vec3d();

            foreach (var he in OutgoingHalfedges)
            {
                if (he.Face == null) continue;
                result += he.GetNormal(vertexPositions);
            }

            result.Unitize();
            return result;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormal2(IReadOnlyList<Vec3d> halfedgeNormals)
        {
            Vec3d result = OutgoingHalfedges.Sum(halfedgeNormals);
            result.Unitize();
            return result;
        }

        #endregion
    }
}
