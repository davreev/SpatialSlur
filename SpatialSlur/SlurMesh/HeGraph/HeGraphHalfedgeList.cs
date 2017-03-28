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
    /// 
    /// </summary>
    [Serializable]
    public sealed class HeGraphHalfedgeList : HalfedgeList<HeGraph, HeGraphHalfedgeList, HeGraphVertexList, HeGraphHalfedge, HeGraphVertex>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="capacity"></param>
        public HeGraphHalfedgeList(HeGraph owner, int capacity)
        {
            Initialize(owner, capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override HeGraphHalfedge CreateElement()
        {
            return new HeGraphHalfedge();
        }


        #region Topology Operators

        /// <summary>
        /// Adds a new edge between the given nodes.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public HeGraphHalfedge AddPair(HeGraphVertex v0, HeGraphVertex v1)
        {
            var verts = Owner.Vertices;
            verts.OwnsCheck(v0);
            verts.OwnsCheck(v1);
            return AddPairImpl(v0, v1);
        }


        /// <summary>
        /// Adds a new edge between nodes at the given indices.
        /// </summary>
        /// <param name="vi0"></param>
        /// <param name="vi1"></param>
        /// <returns></returns>
        public HeGraphHalfedge AddPair(int vi0, int vi1)
        {
            var verts = Owner.Vertices;
            return AddPairImpl(verts[vi0], verts[vi1]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal HeGraphHalfedge AddPairImpl(HeGraphVertex v0, HeGraphVertex v1)
        {
            var he = AddPair();
            v0.Insert(he);
            v1.Insert(he.Twin);
            return he;
        }


        /// <summary>
        /// Collapses the given halfedge by merging the vertices at either end.
        /// The start vertex of the given halfedge is removed.
        /// </summary>
        /// <param name="halfedge"></param>
        public void CollapseEdge(HeGraphHalfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);
            CollapseEdgeImpl(halfedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        internal void CollapseEdgeImpl(HeGraphHalfedge halfedge)
        {
            var v0 = halfedge.Start;
            var v1 = halfedge.End;
            halfedge.Remove();

            // transfer v0's halfedges to v1
            if (v0.IsUnused) return;
            v1.InsertRange(v0.First);
            v0.MakeUnused();
        }


        /// <summary>
        /// Splits the given edge creating a new vertex and halfedge pair.
        /// Returns the new halfedge which starts from the new vertex.
        /// </summary>
        /// <param name="halfedge"></param>
        public HeGraphHalfedge SplitEdge(HeGraphHalfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);
            return SplitEdgeImpl(halfedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        internal HeGraphHalfedge SplitEdgeImpl(HeGraphHalfedge halfedge)
        {
            var he1 = halfedge.Twin;

            var v0 = Owner.Vertices.Add();
            var v1 = he1.Start;

            var he2 = AddPair();
            var he3 = he2.Twin;

            // halfedge->vertex refs
            he1.Start = he2.Start = v0;
            he3.Start = v1;

            // update vertex->halfegde refs
            v0.First = he2;
            if (v1.First == he1) v1.First = he3;

            // update halfedge->halfedge refs
            HeGraphHalfedge.MakeConsecutive(he1.Previous, he3);
            HeGraphHalfedge.MakeConsecutive(he3, he1.Next);
            HeGraphHalfedge.MakeConsecutive(he2, he1);
            HeGraphHalfedge.MakeConsecutive(he1, he2);

            return he2;
        }


        /// <summary>
        /// Returns the new halfedge starting at the new vertex.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public HeGraphHalfedge ZipEdges(HeGraphHalfedge he0, HeGraphHalfedge he1)
        {
            he0.UsedCheck();
            he1.UsedCheck();
            OwnsCheck(he0);
            OwnsCheck(he1);

            // halfedges must start at the same vertex
            if (he0.Start != he1.Start)
                return null;

            return ZipEdgesImpl(he0, he1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        internal HeGraphHalfedge ZipEdgesImpl(HeGraphHalfedge he0, HeGraphHalfedge he1)
        {
            var v0 = he0.Start;
            var v1 = Owner.Vertices.Add(); // new vertex

            he0.Bypass();
            he1.Bypass();

            var he2 = AddPair();
            var he3 = he2.Twin;

            v0.Insert(he2);

            // update halfedge->halfedge refs
            HeGraphHalfedge.MakeConsecutive(he0, he1);
            HeGraphHalfedge.MakeConsecutive(he1, he3);
            HeGraphHalfedge.MakeConsecutive(he3, he0);

            // update halfedge->vertex refs
            he0.Start = he1.Start = he3.Start = v1;

            // update vertex->halfedge refs
            v1.First = he3;

            return he3;
        }


        /// <summary>
        /// Detaches the given halfedge from its start vertex.
        /// </summary>
        /// <param name="halfedge"></param>
        public void DetachHalfedge(HeGraphHalfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            if (halfedge.IsAtDegree1)
                return;

            DetachHalfedgeImpl(halfedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        internal void DetachHalfedgeImpl(HeGraphHalfedge halfedge)
        {
            var v0 = halfedge.Start;
            var v1 = Owner.Vertices.Add();

            // update vertex->halfedge refs
            if (v0.First == halfedge) v0.First = halfedge.Next;
            v1.First = halfedge;

            // update halfedge->vertex refs
            halfedge.Start = v1;

            // update halfedge->halfedge refs
            HeGraphHalfedge.MakeConsecutive(halfedge.Previous, halfedge.Next);
            HeGraphHalfedge.MakeConsecutive(halfedge, halfedge);
        }


        /// <summary>
        /// Removes all edges which start and end at the same vertex.
        /// </summary>
        public void RemoveLoops(bool parallel = false)
        {
            for (int i = 0; i < Count; i += 2)
            {
                var he = this[i];
                if (!he.IsUnused && he.Start == he.End) he.Remove();
            }
        }


        /// <summary>
        /// Removes all duplicate edges in the mesh.
        /// An edge is considered a duplicate if it connects a pair of already connected vertices.
        /// </summary>
        public void RemoveMultiEdges()
        {
            var verts = Owner.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v0 = verts[i];
                if (v0.IsUnused) continue;
                int currTag = verts.NextTag;

                // remove edges to any neighbours visited more than once during circulation
                foreach (var he in v0.IncomingHalfedges)
                {
                    var v1 = he.Start;

                    if (v1.Tag == currTag)
                        he.Remove();
                    else
                        v1.Tag = currTag;
                }
            }
        }

        #endregion


        #region Attributes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetEdgeDepths(IEnumerable<HeGraphHalfedge> sources, IList<int> result)
        {
            var queue = new Queue<HeGraphHalfedge>();
            result.SetRange(Int32.MaxValue, 0, Count >> 1);

            // enqueue sources and set to 0
            foreach (var he in sources)
            {
                OwnsCheck(he);
                if (he.IsUnused) continue;

                result[he.Index >> 1] = 0;
                queue.Enqueue(he);
            }

            // bfs
            while (queue.Count > 0)
            {
                var he0 = queue.Dequeue();
                int t0 = result[he0.Index >> 1] + 1;

                foreach (var he1 in he0.ConnectedPairs)
                {
                    int i1 = he1.Index >> 1;
                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        queue.Enqueue(he1);
                    }
                }
            }
        }

        #endregion
    }
}
