using System;
using System.Collections.Generic;
using System.Linq;
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
    public sealed class HeGraphVertex : HeVertex<HeGraphHalfedge, HeGraphVertex>
    {
        /// <summary>
        /// 
        /// </summary>
        internal override HeGraphVertex Self
        {
            get { return this; }
        }


        /// <summary>
        /// Returns true if the halfedge starts at a degree 1 vertex.
        /// </summary>
        public bool IsDegree1
        {
            get { return First.IsAtDegree1; }
        }


        /// <summary>
        /// Iterates over all outgoing halfedges.
        /// </summary>
        public override IEnumerable<HeGraphHalfedge> OutgoingHalfedges
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    yield return he1;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Iterates over all incoming halfedges.
        /// </summary>
        public override IEnumerable<HeGraphHalfedge> IncomingHalfedges
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    yield return he1.Twin;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Iterates over all connected vertices.
        /// </summary>
        public override IEnumerable<HeGraphVertex> ConnectedVertices
        {
            get
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    yield return he1.End;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Searches for an edge between this node and the given node.
        /// Returns the halfedge starting from this vertex or null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override HeGraphHalfedge FindHalfedgeTo(HeGraphVertex other)
        {
            var he = First;

            do
            {
                if (he.End == other) return he;
                he = he.Next;
            } while (he != First);

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            foreach (var he in OutgoingHalfedges)
            {
                he.Twin.Bypass();
                he.MakeUnused();
            }

            MakeUnused();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        internal void Insert(HeGraphHalfedge halfedge)
        {
            halfedge.Start = Self;

            if (IsUnused)
            {
                First = halfedge;
                HeGraphHalfedge.MakeConsecutive(halfedge, halfedge);
            }
            else
            {
                HeGraphHalfedge.MakeConsecutive(First.Previous, halfedge);
                HeGraphHalfedge.MakeConsecutive(halfedge, First);
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="halfedge"></param>
        internal void InsertRange(HeGraphHalfedge halfedge)
        {
            InsertRange(halfedge, halfedge);
        }


        /// <summary>
        /// Inserts all halfedges from he0 to he1 exclusive
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        internal void InsertRange(HeGraphHalfedge he0, HeGraphHalfedge he1)
        {
            // set start vertices
            {
                var he2 = he0;
                var he3 = Self;
                do
                {
                    he2.Start = he3;
                    he2 = he2.Next;
                } while (he2 != he1);
            }

            // link into any existing halfedges
            if (IsUnused)
            {
                First = he0;
            }
            else
            {
                var he2 = he1.Previous; // cache in case he0 and he1 are the same
                HeGraphHalfedge.MakeConsecutive(First.Previous, he0);
                HeGraphHalfedge.MakeConsecutive(he2, First);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="compare"></param>
        public void SortOutgoing(Comparison<HeGraphHalfedge> compare)
        {
            var hedges = OutgoingHalfedges.ToArray();
            Array.Sort(hedges, compare);

            int last = hedges.Length - 1;
            for (int i = 0; i < last; i++)
                HeGraphHalfedge.MakeConsecutive(hedges[i], hedges[i + 1]);

            HeGraphHalfedge.MakeConsecutive(hedges[last], hedges[0]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="normal"></param>
        public void SortOutgoing(IReadOnlyList<Vec3d> vertexPositions, Vec3d normal)
        {
            throw new NotImplementedException();
        }
    }
}
