using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Base implementation
    /// </summary>
    [Serializable]
    public sealed class HeMeshVertexList : HeVertexList<HeMesh, HeMeshHalfedgeList, HeMeshVertexList, HeMeshFaceList, HeMeshHalfedge, HeMeshVertex, HeMeshFace>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="capacity"></param>
        public HeMeshVertexList(HeMesh owner, int capacity)
        {
            Initialize(owner, capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override HeMeshVertex CreateElement()
        {
            return new HeMeshVertex();
        }


        #region Topology Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public void Remove(HeMeshVertex vertex)
        {
            vertex.UsedCheck();
            OwnsCheck(vertex);
            RemoveImpl(vertex);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal void RemoveImpl(HeMeshVertex vertex)
        {
            if (vertex.IsDegree2)
                Owner.Halfedges.CollapseEdgeImpl(vertex.First);

            var faces = Owner.Faces;
            var he = vertex.First;
            int n = vertex.Degree;

            for (int i = 0; i < n; i++)
            {
                if (!he.IsInHole) faces.RemoveImpl(he.Face);
                he = he.Twin.Next;
            }
        }


        /// <summary>
        /// Merges a pair of boundary vertices.
        /// The first vertex is flagged as unused.
        /// Note that this method may produce non-manifold vertices.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public bool MergeVertices(HeMeshVertex v0, HeMeshVertex v1)
        {
            v0.UsedCheck();
            v1.UsedCheck();

            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0 == v1)
                return false;

            if (!v0.IsBoundary || !v1.IsBoundary)
                return false;

            MergeVerticesImpl(v0, v1);
            return true;
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        internal void MergeVerticesImpl(HeMeshVertex v0, HeMeshVertex v1)
        {
            // if vertices are connected, just collapse the edge between them (check considers non-manifold case)
            var he0 = v0.FindHalfedgeTo(v1);
            if (he0 != null)
            {
                Owner.Halfedges.CollapseEdgeImpl(he0);
                return;
            }

            he0 = v0.First;
            var he1 = v1.First;

            var he2 = he0.Previous;
            var he3 = he1.Previous;

            // update halfedge->vertex refs for all edges emanating from v1
            foreach (var he in v0.OutgoingHalfedges)
                he.Start = v1;

            // update halfedge->halfedge refs
            HeMeshHalfedge.MakeConsecutive(he3, he0);
            HeMeshHalfedge.MakeConsecutive(he2, he1);

            // deal with potential collapse of boundary loops on either side of the merge
            if (he1.Next == he2)
                Owner.Halfedges.CleanupDegree2Hole(he1);

            if (he0.Next == he3)
                Owner.Halfedges.CleanupDegree2Hole(he0);

            // flag elements for removal
            v0.MakeUnused();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        public void DetachVertex(HeMeshHalfedge he0, HeMeshHalfedge he1)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Splits a vertex in 2 connected by a new edge.
        /// Returns the new halfedge leaving the new vertex on success and null on failure.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public HeMeshHalfedge SplitVertex(HeMeshHalfedge he0, HeMeshHalfedge he1)
        {
            he0.UsedCheck();
            he1.UsedCheck();

            var hedges = Owner.Halfedges;
            hedges.OwnsCheck(he0);
            hedges.OwnsCheck(he1);

            if (he0.Start != he1.Start)
                return null;

            return SplitVertexImpl(he0, he1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        internal HeMeshHalfedge SplitVertexImpl(HeMeshHalfedge he0, HeMeshHalfedge he1)
        {
            // if the same edge or vertex is degree 2 then just split the edge
            if (he0 == he1 || he0.Twin.Next == he1)
                return Owner.Halfedges.SplitEdgeImpl(he0.Twin);

            var v0 = he0.Start;
            var v1 = Add();

            var he2 = Owner.Halfedges.AddPair(v0, v1);
            var he3 = he2.Twin;

            // update halfedge->face refs
            he2.Face = he0.Face;
            he3.Face = he1.Face;

            // update start vertex of all outoging edges between he0 and he1
            var he = he0;
            do
            {
                he.Start = v1;
                he = he.Twin.Next;
            } while (he != he1);

            // update vertex->halfedge refs if necessary
            if (v0.First.Start == v1)
            {
                // if v0's outgoing halfedge now starts at v1, can assume v1 is now on the boundary if v0 was originally
                v1.First = v0.First;
                v0.First = he2;
            }
            else
            {
                v1.First = he3;
            }

            // update halfedge->halfedge refs
            HeMeshHalfedge.MakeConsecutive(he0.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he2, he0);
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he3);
            HeMeshHalfedge.MakeConsecutive(he3, he1);

            return he3;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void ChamferVertex(HeMeshVertex vertex)
        {
            vertex.UsedCheck();
            OwnsCheck(vertex);
            ChamferVertexImpl(vertex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        internal void ChamferVertexImpl(HeMeshVertex vertex)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion
    }
}
