
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

namespace SpatialSlur.Meshes.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    [Serializable]
    public abstract class HeGraph<TV, TE> : HeStructure<TV, TE>
        where TV : HeGraph<TV, TE>.Vertex
        where TE : HeGraph<TV, TE>.Halfedge
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new abstract class Vertex : HeStructure<TV, TE>.Vertex
        {
            /// <summary>
            /// Inserts the given halfedge at this vertex
            /// </summary>
            /// <param name="hedge"></param>
            internal void Insert(TE hedge)
            {
                hedge.Start = Self;

                if (IsUnused)
                {
                    First = hedge;
                    hedge.Twin.MakeConsecutive(hedge);
                }
                else
                {
                    var he = First;
                    he.Previous.MakeConsecutive(hedge);
                    hedge.Twin.MakeConsecutive(he);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="hedge"></param>
            internal void InsertAll(TE hedge)
            {
                var he = hedge;

                // set start vertices
                do
                {
                    he.Start = Self;
                    he = he.NextAtStart;
                } while (he != hedge);

                // link into any existing halfedges
                if (IsUnused)
                {
                    First = hedge;
                    return;
                }

                he = hedge.Previous;
                First.Previous.MakeConsecutive(hedge);
                he.MakeConsecutive(First);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="K"></typeparam>
            /// <param name="getKey"></param>
            public void SortOutgoingHalfedges<K>(Func<TE, K> getKey)
                where K : IComparable<K>
            {
                var hedges = OutgoingHalfedges.OrderBy(getKey);

                var itr = hedges.GetEnumerator();
                if (!itr.MoveNext()) return;

                var first = itr.Current;
                var prev = first;
                
                while(itr.MoveNext())
                {
                    var he = itr.Current;
                    prev.Twin.MakeConsecutive(he);
                    prev = he;
                }

                prev.Twin.MakeConsecutive(first);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new abstract class Halfedge : HeStructure<TV, TE>.Halfedge
        {
            /// <summary>
            /// 
            /// </summary>
            internal void Bypass()
            {
                if (IsAtDegree1)
                {
                    Start.MakeUnused();
                    return;
                }

                var he = NextAtStart;
                Previous.MakeConsecutive(he);

                if (IsFirstAtStart)
                    Start.First = he;
            }
        }

        #endregion


        #region Static members

        /// <summary>
        /// Throws an exception if the topology of the given mesh is not valid.
        /// </summary>
        /// <param name="graph"></param>
        internal static void CheckTopology(HeGraph<TV, TE> graph)
        {
            var verts = graph.Vertices;
            var hedges = graph.Halfedges;

            // ensure halfedges are reciprocally linked
            foreach (var he in hedges)
            {
                if (he.IsUnused) continue;
                if (he.Previous.Next != he && he.Next.Previous != he) Throw();
                if (he.Start.IsUnused) Throw();
            }

            // ensure consistent start vertex during circulation
            foreach (var v in verts)
            {
                foreach (var he in v.OutgoingHalfedges)
                    if (he.Start != v) Throw();
            }

            void Throw()
            {
                throw new Exception("The topology of the given mesh is invalid");
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public HeGraph()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeGraph(int vertexCapacity, int hedgeCapacity)
            :base(vertexCapacity, hedgeCapacity)
        {
        }


        /// <summary>
        /// Removes all unused elements from the graph.
        /// </summary>
        public void Compact()
        {
            Vertices.Compact();
            Halfedges.Compact();
        }


        /// <summary>
        /// Shrinks the capacity of each element list to twice its count.
        /// </summary>
        public void TrimExcess()
        {
            Vertices.TrimExcess();
            Halfedges.TrimExcess();
        }


        /// <summary>
        /// Appends a deep copy of the given graph to this graph.
        /// Allows projection of element data to a different form.
        /// </summary>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UV"></typeparam>
        /// <param name="other"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        public void Append<UV, UE>(HeGraph<UV, UE> other, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraph<UV, UE>.Vertex
            where UE : HeGraph<UV, UE>.Halfedge
        {
            var vertsB = other.Vertices;
            var hedgesB = other.Halfedges;

            int nvA = Vertices.Count;
            int nhA = Halfedges.Count;

            // cache in case of appending to self
            int nvB = vertsB.Count;
            int nhB = hedgesB.Count;

            // append new elements
            for (int i = 0; i < nvB; i++)
                AddVertex();

            for (int i = 0; i < nhB; i += 2)
                AddEdge();

            // set vertex refs
            for (int i = 0; i < nvB; i++)
            {
                var v0 = vertsB[i];
                var v1 = Vertices[i + nvA];

                // transfer attributes
                setVertex?.Invoke(v1, v0);

                if (v0.IsUnused) continue;
                v1.First = Halfedges[v0.First.Index + nhA];
            }

            // set halfedge refs
            for (int i = 0; i < nhB; i++)
            {
                var he0 = hedgesB[i];
                var he1 = Halfedges[i + nhA];

                // transfer attributes
                setHedge?.Invoke(he1, he0);

                if (he0.IsUnused) continue;
                he1.Previous = Halfedges[he0.Previous.Index + nhA];
                he1.Next = Halfedges[he0.Next.Index + nhA];
                he1.Start = Vertices[he0.Start.Index + nvA];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        public void AppendVertexTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            int nhe = Halfedges.Count;
            int nv = Vertices.Count;

            var meshHedges = mesh.Halfedges;
            var meshVerts = mesh.Vertices;

            // append new elements
            for (int i = 0; i < meshVerts.Count; i++)
                AddVertex();

            for (int i = 0; i < meshHedges.Count; i += 2)
                AddEdge();

            // set vertex refs
            for (int i = 0; i < meshVerts.Count; i++)
            {
                var v0 = meshVerts[i];
                var v1 = Vertices[i + nv];

                // transfer attributes
                setVertex?.Invoke(v1, v0);

                if (v0.IsUnused) continue;
                v1.First = Halfedges[v0.First + nhe];
            }

            // set halfedge refs
            for (int i = 0; i < meshHedges.Count; i++)
            {
                var he0 = meshHedges[i];
                var he1 = Halfedges[i + nhe];

                // transfer attributes
                setHedge?.Invoke(he1, he0);

                if (he0.IsUnused) continue;
                he1.Previous = Halfedges[he0.Previous + nhe];
                he1.Next = Halfedges[he0.Next + nhe];
                he1.Start = Vertices[he0.Start + nv];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="setHedge"></param>
        /// <param name="setVertex"></param>
        public void AppendFaceTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UF> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            int nhe = Halfedges.Count;
            int nv = Vertices.Count;

            var meshHedges = mesh.Halfedges;
            var meshFaces = mesh.Faces;

            // append new elements
            for (int i = 0; i < meshFaces.Count; i++)
                AddVertex();

            for (int i = 0; i < meshHedges.Count; i += 2)
                AddEdge();

            // set vertex refs
            for (int i = 0; i < meshFaces.Count; i++)
            {
                var fB = meshFaces[i];
                var vA = Vertices[i + nv];

                // transfer attributes
                setVertex?.Invoke(vA, fB);

                if (fB.IsUnused) continue;
                var heB = fB.First;

                // find first interior halfedge in the face
                while (heB.Twin.Face == null)
                {
                    heB = heB.Next;
                    if (heB == fB.First) goto EndFor; // dual vertex has no valid halfedges
                }

                vA.First = Halfedges[heB + nhe];
                EndFor:;
            }

            // set halfedge refs
            for (int i = 0; i < meshHedges.Count; i++)
            {
                var heB0 = meshHedges[i];
                var heA0 = Halfedges[i + nhe];

                // transfer attributes
                setHedge?.Invoke(heA0, heB0);

                if (heB0.IsUnused || heB0.IsBoundary) continue;
                var heB1 = heB0;

                // find next interior halfedge in the face
                do heB1 = heB1.Next;
                while (heB1.Twin.Face == null && heB1 != heB0);

                heA0.Start = Vertices[heB0.Face + nv];
                heA0.Twin.MakeConsecutive(Halfedges[heB1 + nhe]);
                //heA0.MakeConsecutive(Halfedges[heB1 + nhe]); // TODO check this
            }
        }


        /// <summary>
        /// Returns the first halfedge from each loop in the graph.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TE> GetHalfedgeLoops()
        {
            int currTag = Halfedges.NextTag;

            for (int i = 0; i < Halfedges.Count; i++)
            {
                var he = Halfedges[i];
                if (he.IsUnused || he.Tag == currTag) continue;

                do
                {
                    he.Tag = currTag;
                    he = he.Next;
                } while (he.Tag != currTag);

                yield return he;
            }
        }


        #region Edge Operators

        /// <summary>
        /// Adds a new edge between the given vertices.
        /// Returns the first halfedge in the pair.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public TE AddEdge(TV v0, TV v1)
        {
            Vertices.OwnsCheck(v0);
            Vertices.OwnsCheck(v1);
            return AddEdgeImpl(v0, v1);
        }


        /// <summary>
        /// Adds a new edge between nodes at the given indices.
        /// Returns the first halfedge in the pair.
        /// </summary>
        /// <param name="vi0"></param>
        /// <param name="vi1"></param>
        /// <returns></returns>
        public TE AddEdge(int vi0, int vi1)
        {
            return AddEdgeImpl(Vertices[vi0], Vertices[vi1]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal TE AddEdgeImpl(TV v0, TV v1)
        {
            var he = AddEdge();
            v0.Insert(he);
            v1.Insert(he.Twin);
            return he;
        }


        /// <summary>
        /// Removes the given edge from the mesh.
        /// </summary>
        /// <param name="hedge"></param>
        public void RemoveEdge(TE hedge)
        {
            hedge.UnusedCheck();
            Halfedges.OwnsCheck(hedge);

            RemoveEdgeImpl(hedge);
        }


        /// <summary>
        /// Removes the given edge from the mesh.
        /// </summary>
        /// <param name="hedge"></param>
        private void RemoveEdgeImpl(TE hedge)
        {
            hedge.Bypass();
            hedge.Twin.Bypass();
            hedge.MakeUnused();
        }


        /// <summary>
        /// Collapses the given halfedge by merging the vertices at either end.
        /// The start vertex of the given halfedge is removed.
        /// </summary>
        /// <param name="hedge"></param>
        public void CollapseEdge(TE hedge)
        {
            hedge.UnusedCheck();
            Halfedges.OwnsCheck(hedge);

            CollapseEdgeImpl(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        private void CollapseEdgeImpl(TE hedge)
        {
            var v0 = hedge.Start;
            var v1 = hedge.End;
            RemoveEdgeImpl(hedge);

            // transfer remaining halfedges to v1
            if (v0.IsUnused) return;
            v1.InsertAll(v0.First);
            v0.MakeUnused();
        }


        /// <summary>
        /// Splits the given edge creating a new vertex and halfedge pair.
        /// Returns the new halfedge which starts from the new vertex.
        /// </summary>
        /// <param name="hedge"></param>
        public TE SplitEdge(TE hedge)
        {
            hedge.UnusedCheck();
            Halfedges.OwnsCheck(hedge);

            return SplitEdgeImpl(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        private TE SplitEdgeImpl(TE hedge)
        {
            return SplitEdgeImpl(hedge, AddVertex());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private TE SplitEdgeImpl(TE hedge, TV vertex)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            var v0 = vertex;
            var v1 = he1.Start;

            var he2 = AddEdge();
            var he3 = he2.Twin;

            // halfedge->vertex refs
            he1.Start = he2.Start = v0;
            he3.Start = v1;

            // vertex->halfedge refs
            v0.First = he2;
            if (v1.First == he1) v1.First = he3;

            // halfedge->halfedge refs
            if (he0.Next == he1)
            {
                he0.MakeConsecutive(he2);
                he2.MakeConsecutive(he3);
                he3.MakeConsecutive(he1);
            }
            else
            {
                he2.MakeConsecutive(he0.Next);
                he0.MakeConsecutive(he2);
                he1.Previous.MakeConsecutive(he3);
                he3.MakeConsecutive(he1);
            }

            return he2;
        }


        /// <summary>
        /// Inserts the specified number of vertices along the given edge.
        /// </summary>
        /// <param name="hedge"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public TE DivideEdge(TE hedge, int count)
        {
            hedge.UnusedCheck();
            Halfedges.OwnsCheck(hedge);

            return DivideEdgeImpl(hedge, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private TE DivideEdgeImpl(TE hedge, int count)
        {
            for (int i = 0; i < count; i++)
                hedge = SplitEdgeImpl(hedge);

            return hedge;
        }


        /// <summary>
        /// Returns the new halfedge starting at the new vertex.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public TE ZipEdges(TE he0, TE he1)
        {
            he0.UnusedCheck();
            he1.UnusedCheck();

            Halfedges.OwnsCheck(he0);
            Halfedges.OwnsCheck(he1);

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
        private TE ZipEdgesImpl(TE he0, TE he1)
        {
            var v0 = he0.Start;
            var v1 = AddVertex(); // new vertex

            he0.Bypass();
            he1.Bypass();

            var he2 = AddEdge();
            var he3 = he2.Twin;
            v0.Insert(he2);

            // update halfedge->vertex refs
            he0.Start = he1.Start = he3.Start = v1;

            // update vertex->halfedge refs
            v1.First = he3;

            // update halfedge->halfedge refs
            he0.Twin.MakeConsecutive(he1);
            he1.Twin.MakeConsecutive(he3);
            he2.MakeConsecutive(he0);

            return he3;
        }


        /// <summary>
        /// Removes all edges which start and end at the same vertex.
        /// </summary>
        public void RemoveLoops(bool parallel = false)
        {
            for (int i = 0; i < Halfedges.Count; i += 2)
            {
                var he = Halfedges[i];
                if (!he.IsUnused && he.Start == he.End) he.MakeUnused();
            }
        }


        /// <summary>
        /// Removes all duplicate edges in the mesh.
        /// An edge is considered a duplicate if it connects a pair of already connected vertices.
        /// </summary>
        public void RemoveMultiEdges()
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                var v0 = Vertices[i];
                if (v0.IsUnused) continue;

                int currTag = Vertices.NextTag;

                // remove edges to any neighbours visited more than once during circulation
                foreach (var he in v0.IncomingHalfedges)
                {
                    var v1 = he.Start;

                    if (v1.Tag == currTag)
                        he.MakeUnused();
                    else
                        v1.Tag = currTag;
                }
            }
        }

        #endregion


        #region Halfedge Operators

        /// <summary>
        /// Detaches the given halfedge from its start vertex.
        /// </summary>
        /// <param name="hedge"></param>
        public void DetachHalfedge(TE hedge)
        {
            hedge.UnusedCheck();
            Halfedges.OwnsCheck(hedge);

            if (hedge.IsAtDegree1)
                return;

            DetachHalfedgeImpl(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        private void DetachHalfedgeImpl(TE hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = AddVertex();

            // update vertex->halfedge refs
            v1.First = he0;
            if (v0.First == he0) v0.First = he1.Next;

            // update halfedge->vertex refs
            he0.Start = v1;

            // update halfedge->halfedge refs
            he0.Previous.MakeConsecutive(he1.Next);
            he1.MakeConsecutive(he0);
        }

        #endregion


        #region Vertex Operators

        /// <summary>
        /// Removes the given vertex along with all incident edges.
        /// </summary>
        /// <param name="quantity"></param>
        public void AddVertices(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                var v = NewVertex();
                Vertices.Add(v);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void RemoveVertex(TV vertex)
        {
            vertex.UnusedCheck();
            Vertices.OwnsCheck(vertex);

            RemoveVertexImpl(vertex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        private void RemoveVertexImpl(TV vertex)
        {
            foreach (var he in vertex.OutgoingHalfedges)
            {
                he.Twin.Bypass();
                he.MakeUnused();
            }

            vertex.MakeUnused();
        }


        /// <summary>
        /// Transfers halfedges from the first to the second given vertex.
        /// The first vertex is flagged as unused.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public void MergeVertices(TV v0, TV v1)
        {
            v0.UnusedCheck();
            v1.UnusedCheck();

            Vertices.OwnsCheck(v0);
            Vertices.OwnsCheck(v1);

            MergeVerticesImpl(v0, v1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        private void MergeVerticesImpl(TV v0, TV v1)
        {
            v1.InsertAll(v0.First);
            v0.MakeUnused();
        }


        /// <summary>
        /// Transfers halfedges leaving each vertex to the first vertex in the collection.
        /// All vertices except the first are flagged as unused.
        /// </summary>
        /// <param name="vertices"></param>
        public void MergeVertices(IEnumerable<TV> vertices)
        {
            var v0 = vertices.First();

            v0.UnusedCheck();
            Vertices.OwnsCheck(v0);

            foreach (var v1 in vertices.Skip(1))
            {
                v1.UnusedCheck();
                Vertices.OwnsCheck(v1);

                MergeVerticesImpl(v1, v0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void ExpandVertex(TV vertex)
        {
            // TODO implement
            throw new NotImplementedException();
        }


        /// <summary>
        /// Splits a vertex in 2 connected by a new edge.
        /// Returns the new halfedge leaving the new vertex on success and null on failure.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        public TE SplitVertex(TE he0, TE he1)
        {
            he0.UnusedCheck();
            he1.UnusedCheck();

            Halfedges.OwnsCheck(he0);
            Halfedges.OwnsCheck(he1);

            if (he0.Start != he1.Start)
                return null;

            return SplitVertexImpl(he0, he1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        private TE SplitVertexImpl(TE he0, TE he1)
        {
            if (he0 == he1 || he0.IsAtDegree2)
                return SplitEdgeImpl(he0);

            var v0 = he0.Start;
            var v1 = AddVertex(); // new vertex

            // create new halfedge pair
            var he2 = AddEdge();
            var he3 = he2.Twin;

            // update halfedge->halfedge refs
            he0.Previous.MakeConsecutive(he2);
            he2.MakeConsecutive(he0);
            he1.Previous.MakeConsecutive(he3);
            he3.MakeConsecutive(he1);

            // update halfedge->vertex refs
            he2.Start = v0;

            foreach (var he in he0.CirculateStart)
                he.Start = v1;

            // update vertex->halfedge refs
            v1.First = he3;

            if (v0.First.Start == v1)
                v0.First = he2;

            return he3;
        }


        /// <summary>
        /// Detaches all outgoing halfedges from the given vertex
        /// </summary>
        /// <param name="vertex"></param>
        public void DetachVertex(TV vertex)
        {
            vertex.UnusedCheck();
            Vertices.OwnsCheck(vertex);

            DetachVertexImpl(vertex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        private void DetachVertexImpl(TV vertex)
        {
            var he0 = vertex.First;
            var he1 = he0.NextAtStart;

            while (he1 != he0)
            {
                var he2 = he1.NextAtStart; // cache before detaching
                DetachHalfedgeImpl(he1);
                he1 = he2;
            }
        }


        /// <summary>
        /// Sorts the outgoing halfedges around each vertex.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="getKey"></param>
        /// <param name="parallel"></param>
        public void SortOutgoingHalfedges<K>(Func<TE, K> getKey, bool parallel = false)
            where K : IComparable<K>
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Vertices.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Vertices.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = Vertices[i];
                    if (v.IsUnused) continue;
                    v.SortOutgoingHalfedges(getKey);
                }
            }
        }

        #endregion
    }
}