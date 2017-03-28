using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
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
    public sealed class HeGraphVertexList : HeVertexList<HeGraph, HeGraphHalfedgeList, HeGraphVertexList, HeGraphHalfedge, HeGraphVertex>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="capacity"></param>
        public HeGraphVertexList(HeGraph owner, int capacity)
        {
            Initialize(owner, capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override HeGraphVertex CreateElement()
        {
            return new HeGraphVertex();
        }


        /// <summary>
        /// Sorts the outgoing halfedges around each vertex.
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="parallel"></param>
        public void SortOutgoingHalfedges(Comparison<HeGraphHalfedge> compare, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                var hedges = new List<HeGraphHalfedge>();

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = this[i];
                    if (v.IsUnused) continue;

                    hedges.AddRange(v.OutgoingHalfedges);
                    hedges.Sort(compare);

                    int last = hedges.Count - 1;
                    for (int j = 0; j < last; j++) HeGraphHalfedge.MakeConsecutive(hedges[j], hedges[j + 1]);
                    HeGraphHalfedge.MakeConsecutive(hedges[last], hedges[0]);

                    hedges.Clear();
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        #region Topology Operators

        /// <summary>
        /// Transfers halfedges from the first to the second given vertex.
        /// The first vertex is flagged as unused.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public void MergeVertices(HeGraphVertex v0, HeGraphVertex v1)
        {
            v0.UsedCheck();
            v1.UsedCheck();

            OwnsCheck(v0);
            OwnsCheck(v1);

            MergeVerticesImpl(v0, v1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        internal void MergeVerticesImpl(HeGraphVertex v0, HeGraphVertex v1)
        {
            v1.InsertRange(v0.First);
            v0.MakeUnused();
        }


        /// <summary>
        /// Transfers halfedges leaving each vertex to the first vertex in the collection.
        /// All vertices except the first are flagged as unused.
        /// </summary>
        /// <param name="vertices"></param>
        public void MergeVertices(IEnumerable<HeGraphVertex> vertices)
        {
            var v0 = vertices.ElementAt(0);
            v0.UsedCheck();
            OwnsCheck(v0);

            foreach (var v in vertices.Skip(1))
            {
                v.UsedCheck();
                OwnsCheck(v);
                MergeVerticesImpl(v, v0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void ExpandVertex(HeGraphVertex vertex)
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
        public HeGraphHalfedge SplitVertex(HeGraphHalfedge he0, HeGraphHalfedge he1)
        {
            he0.UsedCheck();
            he1.UsedCheck();

            var hedges = Owner.Halfedges;
            hedges.OwnsCheck(he0);
            hedges.OwnsCheck(he1);

            if (he0.Start != he0.Start)
                return null;

            return SplitVertexImpl(he0, he1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        internal HeGraphHalfedge SplitVertexImpl(HeGraphHalfedge he0, HeGraphHalfedge he1)
        {
            if (he0 == he1 || he0.IsAtDegree2)
                return Owner.Halfedges.SplitEdgeImpl(he0);

            var v0 = he0.Start;
            var v1 = Add(); // new vertex

            // create new halfedge pair
            var he2 = Owner.Halfedges.AddPair();
            var he3 = he2.Twin;

            // update halfedge->halfedge refs
            HeGraphHalfedge.MakeConsecutive(he0.Previous, he2);
            HeGraphHalfedge.MakeConsecutive(he1.Previous, he3);
            HeGraphHalfedge.MakeConsecutive(he3, he0);
            HeGraphHalfedge.MakeConsecutive(he2, he1);

            // update halfedge->vertex refs
            he2.Start = v0;
            foreach (var he in he0.CirculateStart) he.Start = v1;

            // update vertex->halfedge refs
            v1.First = he3;
            if (v0.First.Start == v1) v0.First = he2;

            return he3;
        }


        /// <summary>
        /// Detaches all outgoing halfedges from the given vertex
        /// </summary>
        /// <param name="vertex"></param>
        public void DetachVertex(HeGraphVertex vertex)
        {
            vertex.UsedCheck();
            OwnsCheck(vertex);
            DetachVertexImpl(vertex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        internal void DetachVertexImpl(HeGraphVertex vertex)
        {
            var hedges = Owner.Halfedges;
            var he0 = vertex.First;
            var he1 = he0.Next;

            while (he1 != he0)
            {
                var he = he1.Next;
                hedges.DetachHalfedgeImpl(he1);
                he1 = he;
            }
        }

        #endregion
    } 
}