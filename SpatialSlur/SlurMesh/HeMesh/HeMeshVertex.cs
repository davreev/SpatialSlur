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
    [Serializable]
    public sealed class HeMeshVertex : HeVertex<HeMeshHalfedge, HeMeshVertex, HeMeshFace>
    {
        /// <summary>
        /// 
        /// </summary>
        internal override HeMeshVertex Self
        {
            get { return this; }
        }


        /// <summary>
        /// Returns true if the vertex has at least 2 outgoing halfedges.
        /// </summary>
        internal bool IsValid
        {
            get { return !IsDegree1; }
        }


        /// <summary>
        /// Returns true if the vertex has 1 outgoing halfedge
        /// </summary>
        internal bool IsDegree1
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
        /// Iterates over all halfedges starting from this vertex.
        /// </summary>
        public override IEnumerable<HeMeshHalfedge> OutgoingHalfedges
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    yield return he1;
                    he1 = he1.Twin.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Iterates over all halfedges ending at this vertex.
        /// </summary>
        public override IEnumerable<HeMeshHalfedge> IncomingHalfedges
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    he1 = he1.Twin;
                    yield return he1;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        ///  Iterates over all connected vertices.
        /// </summary>
        public override IEnumerable<HeMeshVertex> ConnectedVertices
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    he1 = he1.Twin;
                    yield return he1.Start;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Searches for an edge between this node and the given node.
        /// Returns null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override HeMeshHalfedge FindHalfedgeTo(HeMeshVertex other)
        {
            var he0 = First;
            var he1 = he0;

            do
            {
                if (he1.End == other) return he1;
                he1 = he1.Twin.Next;
            } while (he1 != he0);

            return null;
        }
    }
}
