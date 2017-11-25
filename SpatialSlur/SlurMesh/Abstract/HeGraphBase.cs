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
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    [Serializable]
    public abstract class HeGraphBase<TV, TE> : HeStructure<TV, TE>
        where TV : HeGraphBase<TV, TE>.Vertex
        where TE : HeGraphBase<TV, TE>.Halfedge
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Vertex : HeElement<TV, TE>, IHeVertex<TV, TE>
        {
            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public int Degree
            {
                get { return First.CountEdgesAtStart(); }
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


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<TE> OutgoingHalfedges
            {
                get { return First.CirculateStart; }
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<TE> IncomingHalfedges
            {
                get { return First.Twin.CirculateEnd; }
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<TV> ConnectedVertices
            {
                get
                {
                    var he = First;

                    do
                    {
                        yield return he.End;
                        he = he.NextAtStart;
                    } while (he != First);
                }
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public bool IsDegree(int n)
            {
                return First.IsAtDegree(n);
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool IsConnectedTo(TV other)
            {
                return FindHalfedge(other) != null;
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public TE FindHalfedge(TV other)
            {
                var he = First;

                do
                {
                    if (he.End == other) return he;
                    he = he.NextAtStart;
                } while (he != First);

                return null;
            }


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
                    hedge.MakeConsecutive(hedge);
                }
                else
                {
                    var he = First;
                    he.PreviousAtStart.MakeConsecutive(hedge);
                    hedge.MakeConsecutive(he);
                }
            }


            /// <summary>
            /// Circulates the given halfedge inserting each connected halfedge at this vertex.
            /// </summary>
            /// <param name="hedge"></param>
            internal void InsertRange(TE hedge)
            {
                InsertRange(hedge, hedge);
            }


            /// <summary>
            /// Circulates the given halfedges (exclusive) inserting each connected halfedge at this vertex.
            /// </summary>
            /// <param name="he0"></param>
            /// <param name="he1"></param>
            internal void InsertRange(TE he0, TE he1)
            {
                // set start vertices
                {
                    var he2 = he0;

                    do
                    {
                        he2.Start = Self;
                        he2 = he2.NextAtStart;
                    } while (he2 != he1);
                }

                // link into any existing halfedges
                if (IsUnused)
                {
                    First = he0;
                }
                else
                {
                    var he2 = First;
                    var he3 = he1.PreviousAtStart; // cache in case he0 and he1 are the same

                    he2.PreviousAtStart.MakeConsecutive(he0);
                    he3.MakeConsecutive(he2);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="compare"></param>
            public void SortOutgoing(Comparison<TE> compare)
            {
                var hedges = OutgoingHalfedges.ToArray();
                Array.Sort(hedges, compare);

                int last = hedges.Length - 1;

                for (int i = 0; i < last; i++)
                    hedges[i].MakeConsecutive(hedges[i + 1]);

                hedges[last].MakeConsecutive(hedges[0]);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="getPosition"></param>
            /// <param name="normal"></param>
            public void SortOutgoingRadial(Func<TV, Vec3d> getPosition, Vec3d normal)
            {
                // TODO
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Halfedge : Halfedge<TV, TE>, IHalfedge<TV, TE>
        {
            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public TE PreviousAtStart
            {
                get { return Previous; }
                internal set { Previous = value; }
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public TE NextAtStart
            {
                get { return Next; }
                internal set { Next = value; }
            }


            /// <summary>
            /// Returns true if this halfedge starts at a degree 1 vertex.
            /// </summary>
            public bool IsAtDegree1
            {
                get { return this == Next; }
            }


            /// <summary>
            /// Returns true if this halfedge starts at a degree 2 vertex.
            /// </summary>
            public bool IsAtDegree2
            {
                get
                {
                    var he = Next;
                    return this != he && this == he.Next;
                }
            }


            /// <summary>
            /// Returns true if this halfedge starts at a degree 3 vertex.
            /// </summary>
            public bool IsAtDegree3
            {
                get
                {
                    var he = Next;
                    return this != he && this == he.Next.Next;
                }
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public bool IsFirstAtStart
            {
                get { return this == Start.First; }
            }

            
            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public bool IsFirstInEdge
            {
                get { return (Index & 1) == 0; }
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<TE> CirculateStart
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


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<TE> CirculateEnd
            {
                get
                {
                    var he = Twin;

                    do
                    {
                        yield return he.Twin;
                        he = he.NextAtStart;
                    } while (he != Twin);
                }
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<TE> ConnectedPairs
            {
                get { return GetConnectedPairs(Self, Twin); }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="he0"></param>
            /// <param name="he1"></param>
            /// <returns></returns>
            private static IEnumerable<TE> GetConnectedPairs(TE he0, TE he1)
            {
                if (!he0.IsAtDegree1)
                {
                    yield return he0.PreviousAtStart;
                    if (!he0.IsAtDegree2) yield return he0.NextAtStart;
                }

                if (!he1.IsAtDegree1)
                {
                    yield return he1.PreviousAtStart;
                    if (!he1.IsAtDegree2) yield return he1.NextAtStart;
                }
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public bool IsAtDegree(int n)
            {
                var he = Self;

                do
                {
                    if (--n < 0) return false;
                    he = he.NextAtStart;
                } while (he != this);

                return n == 0;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            internal void MakeConsecutive(TE other)
            {
                Next = other;
                other.Previous = Self;
            }


            /// <inheritdoc/>
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public int CountEdgesAtStart()
            {
                var he = Self;
                int count = 0;

                do
                {
                    count++;
                    he = he.NextAtStart;
                } while (he != this);

                return count;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="offset"></param>
            /// <returns></returns>
            public TE GetRelativeAtStart(int offset)
            {
                var he = Self;

                if (offset < 0)
                {
                    for (int i = 0; i < offset; i++)
                        he = he.PreviousAtStart;
                }
                else
                {
                    for (int i = 0; i < offset; i++)
                        he = he.NextAtStart;
                }

                return he;
            }


            /// <summary>
            /// 
            /// </summary>
            internal void Bypass()
            {
                var v = Start;

                if (IsAtDegree1)
                {
                    Start.MakeUnused();
                    return;
                }

                if (IsFirstAtStart) Start.First = NextAtStart;
                PreviousAtStart.MakeConsecutive(NextAtStart);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeGraphBase(int vertexCapacity = DefaultCapacity, int hedgeCapacity = DefaultCapacity)
            : base(vertexCapacity, hedgeCapacity)
        {
        }


        /// <summary>
        /// Removes all elements that have been flagged for removal.
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
        public void Append<UV, UE>(HeGraphBase<UV, UE> other, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraphBase<UV, UE>.Vertex
            where UE : HeGraphBase<UV, UE>.Halfedge
        {
            if (setVertex == null)
                setVertex = delegate { };

            if (setHedge == null)
                setHedge = delegate { };

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

            // link new vertices to new halfedges
            for (int i = 0; i < nvB; i++)
            {
                var v0 = vertsB[i];
                var v1 = Vertices[i + nvA];

                // transfer attributes
                setVertex(v1, v0);

                if (v0.IsUnused) continue;
                v1.First = Halfedges[v0.First.Index + nhA];
            }

            // link new halfedges to new vertices and other new halfedges
            for (int i = 0; i < nhB; i++)
            {
                var he0 = hedgesB[i];
                var he1 = Halfedges[i + nhA];

                // transfer attributes
                setHedge(he1, he0);

                if (he0.IsUnused) continue;
                he1.PreviousAtStart = Halfedges[he0.PreviousAtStart.Index + nhA];
                he1.NextAtStart = Halfedges[he0.NextAtStart.Index + nhA];
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
        public void AppendVertexTopology<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeMeshBase<UV, UE, UF>.Vertex
            where UE : HeMeshBase<UV, UE, UF>.Halfedge
            where UF : HeMeshBase<UV, UE, UF>.Face
        {
            if (setVertex == null)
                setVertex = delegate { };

            if (setHedge == null)
                setHedge = delegate { };

            int nhe = Halfedges.Count;
            int nv = Vertices.Count;

            var meshHedges = mesh.Halfedges;
            var meshVerts = mesh.Vertices;

            // append new elements
            for (int i = 0; i < meshVerts.Count; i++)
                AddVertex();

            for (int i = 0; i < meshHedges.Count; i += 2)
                AddEdge();

            // link new vertices to new halfedges
            for (int i = 0; i < meshVerts.Count; i++)
            {
                var v0 = meshVerts[i];
                var v1 = Vertices[i + nv];

                // transfer attributes
                setVertex(v1, v0);

                if (v0.IsUnused) continue;
                v1.First = Halfedges[v0.First.Index + nhe];
            }

            // link new halfedges to eachother and new vertices
            for (int i = 0; i < meshHedges.Count; i++)
            {
                var he0 = meshHedges[i];
                var he1 = Halfedges[i + nhe];

                // transfer attributes
                setHedge(he1, he0);

                if (he0.IsUnused) continue;
                he1.PreviousAtStart = Halfedges[he0.PreviousAtStart.Index + nhe];
                he1.NextAtStart = Halfedges[he0.NextAtStart.Index + nhe];
                he1.Start = Vertices[he0.Start.Index + nv];
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
        public void AppendFaceTopology<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UF> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeMeshBase<UV, UE, UF>.Vertex
            where UE : HeMeshBase<UV, UE, UF>.Halfedge
            where UF : HeMeshBase<UV, UE, UF>.Face
        {
            if (setVertex == null)
                setVertex = delegate { };

            if (setHedge == null)
                setHedge = delegate { };

            int nhe = Halfedges.Count;
            int nv = Vertices.Count;

            var meshHedges = mesh.Halfedges;
            var meshFaces = mesh.Faces;

            // append new elements
            for (int i = 0; i < meshFaces.Count; i++)
                AddVertex();

            for (int i = 0; i < meshHedges.Count; i += 2)
                AddEdge();

            // link new vertices to new halfedges
            for (int i = 0; i < meshFaces.Count; i++)
            {
                var fB = meshFaces[i];
                var vA = Vertices[i + nv];

                // transfer attributes
                setVertex(vA, fB);

                if (fB.IsUnused) continue;
                var heB = fB.First;

                // find first interior halfedge in the face
                while (heB.Twin.Face == null)
                {
                    heB = heB.NextInFace;
                    if (heB == fB.First) goto EndFor; // dual vertex has no valid halfedges
                }

                vA.First = Halfedges[heB.Index + nhe];
                EndFor:;
            }

            // link new halfedges to eachother and new vertices
            for (int i = 0; i < meshHedges.Count; i++)
            {
                var heB0 = meshHedges[i];
                var heA0 = Halfedges[i + nhe];
                setHedge(heA0, heB0);

                if (heB0.IsUnused || heB0.IsBoundary) continue;
                var heB1 = heB0;

                // find next interior halfedge in the face
                do heB1 = heB1.NextInFace;
                while (heB1.Twin.Face == null && heB1 != heB0);

                heA0.Start = Vertices[heB0.Face.Index + nv];
                heA0.MakeConsecutive(Halfedges[heB1.Index + nhe]);
            }
        }


        #region ElementOperators


        #region Edge Operators

        /// <summary>
        /// Adds a new edge between the given nodes.
        /// Returns the first halfedge in the pair.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public TE AddEdge(TV v0, TV v1)
        {
            Vertices.ContainsCheck(v0);
            Vertices.ContainsCheck(v1);
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
            Halfedges.ContainsCheck(hedge);
            hedge.UnusedCheck();

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
            Halfedges.ContainsCheck(hedge);
            hedge.UnusedCheck();

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

            // transfer v0's halfedges to v1
            if (v0.IsUnused) return;
            v1.InsertRange(v0.First);
            v0.MakeUnused();
        }


        /// <summary>
        /// Splits the given edge creating a new vertex and halfedge pair.
        /// Returns the new halfedge which starts from the new vertex.
        /// </summary>
        /// <param name="hedge"></param>
        public TE SplitEdge(TE hedge)
        {
            Halfedges.ContainsCheck(hedge);
            hedge.UnusedCheck();

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
            var he1 = hedge.Twin;

            var v0 = vertex;
            var v1 = he1.Start;

            var he2 = AddEdge();
            var he3 = he2.Twin;

            // halfedge->vertex refs
            he1.Start = he2.Start = v0;
            he3.Start = v1;

            // update vertex->halfegde refs
            v0.First = he2;
            if (v1.First == he1) v1.First = he3;

            // update halfedge->halfedge refs
            he1.PreviousAtStart.MakeConsecutive(he3);
            he3.MakeConsecutive(he1.NextAtStart);
            he2.MakeConsecutive(he1);
            he1.MakeConsecutive(he2);

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
            Halfedges.ContainsCheck(hedge);
            hedge.UnusedCheck();

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
            Halfedges.ContainsCheck(he0);
            Halfedges.ContainsCheck(he1);

            he0.UnusedCheck();
            he1.UnusedCheck();

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

            // update halfedge->halfedge refs
            he0.MakeConsecutive(he1);
            he1.MakeConsecutive(he3);
            he3.MakeConsecutive(he0);

            // update halfedge->vertex refs
            he0.Start = he1.Start = he3.Start = v1;

            // update vertex->halfedge refs
            v1.First = he3;

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
            Halfedges.ContainsCheck(hedge);
            hedge.UnusedCheck();

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
            var v0 = hedge.Start;
            var v1 = AddVertex();

            // update vertex->halfedge refs
            if (v0.First == hedge) v0.First = hedge.NextAtStart;
            v1.First = hedge;

            // update halfedge->vertex refs
            hedge.Start = v1;

            // update halfedge->halfedge refs
            hedge.PreviousAtStart.MakeConsecutive(hedge.NextAtStart);
            hedge.MakeConsecutive(hedge);
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
            Vertices.ContainsCheck(vertex);
            vertex.UnusedCheck();

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
            Vertices.ContainsCheck(v0);
            Vertices.ContainsCheck(v1);

            v0.UnusedCheck();
            v1.UnusedCheck();

            MergeVerticesImpl(v0, v1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        private void MergeVerticesImpl(TV v0, TV v1)
        {
            v1.InsertRange(v0.First);
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
            Vertices.ContainsCheck(v0);
            v0.UnusedCheck();

            foreach (var v1 in vertices.Skip(1))
            {
                Vertices.ContainsCheck(v1);
                v1.UnusedCheck();

                MergeVerticesImpl(v1, v0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void ExpandVertex(TV vertex)
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
        public TE SplitVertex(TE he0, TE he1)
        {
            Halfedges.ContainsCheck(he0);
            Halfedges.ContainsCheck(he1);

            he0.UnusedCheck();
            he1.UnusedCheck();

            if (he0.Start != he0.Start)
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
            he0.PreviousAtStart.MakeConsecutive(he2);
            he1.PreviousAtStart.MakeConsecutive(he3);
            he3.MakeConsecutive(he0);
            he2.MakeConsecutive(he1);

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
        public void DetachVertex(TV vertex)
        {
            Vertices.ContainsCheck(vertex);
            vertex.UnusedCheck();

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
                var he = he1.NextAtStart;
                DetachHalfedgeImpl(he1);
                he1 = he;
            }
        }


        /// <summary>
        /// Sorts the outgoing halfedges around each vertex.
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="parallel"></param>
        public void SortOutgoingHalfedges(Comparison<TE> compare, bool parallel = false)
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
                    v.SortOutgoing(compare);
                }
            }
        }
        
        #endregion

        #endregion


        #region Element Attributes

        /// <summary>
        /// Returns the first halfedge from each loop in the graph.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TE> GetEdgeLoops()
        {
            int currTag = Halfedges.NextTag;

            for (int i = 0; i < Halfedges.Count; i++)
            {
                var he = Halfedges[i];
                if (he.IsUnused || he.Tag == currTag) continue;

                do
                {
                    he.Tag = currTag;
                    he = he.Twin.NextAtStart;
                } while (he.Tag != currTag);

                yield return he;
            }
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TG"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    [Serializable]
    public abstract class HeGraphBaseFactory<TG, TV, TE> : IFactory<TG>
        where TG : HeGraphBase<TV, TE>
        where TV : HeGraphBase<TV, TE>.Vertex
        where TE : HeGraphBase<TV, TE>.Halfedge
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TG Create()
        {
            return Create(4, 4);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public abstract TG Create(int vertexCapacity, int hedgeCapacity);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public TG CreateCopy<UV, UE>(HeGraphBase<UV, UE> graph, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraphBase<UV, UE>.Vertex
            where UE : HeGraphBase<UV, UE>.Halfedge
        {
            var copy = Create(graph.Vertices.Capacity, graph.Halfedges.Capacity);
            copy.Append(graph, setVertex, setHedge);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraphBase<UV, UE> graph, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraphBase<UV, UE>.Vertex
            where UE : HeGraphBase<UV, UE>.Halfedge
        {
            return CreateConnectedComponents(graph, out int[] compIds, out int[] edgeIds, setVertex, setHedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraphBase<UV, UE> graph, out int[] componentIndices, out int[] edgeIndices, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraphBase<UV, UE>.Vertex
            where UE : HeGraphBase<UV, UE>.Halfedge
        {
            int ne = graph.Edges.Count;
            componentIndices = new int[ne];
            edgeIndices = new int[ne];

            return CreateConnectedComponents(graph, ToProp(componentIndices), ToProp(edgeIndices), setVertex, setHedge);

            Property<UE, T> ToProp<T>(T[] values)
            {
                return Property.Create<UE, T>(he => values[he >> 1], (he, i) => values[he >> 1] = i);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public TG[] CreateConnectedComponents<UV, UE>(HeGraphBase<UV, UE> graph, Property<UE, int> componentIndex, Property<UE, int> edgeIndex, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraphBase<UV, UE>.Vertex
            where UE : HeGraphBase<UV, UE>.Halfedge
        {
            if (setVertex == null)
                setVertex = delegate { };

            if (setHedge == null)
                setHedge = delegate { };

            var vertices = graph.Vertices;
            var hedges = graph.Halfedges;

            int ncomp = graph.GetEdgeComponentIndices(componentIndex.Set);
            var comps = new TG[ncomp];

            // initialize components
            for (int i = 0; i < comps.Length; i++)
                comps[i] = Create();

            // create component halfedges
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var heA = hedges[i];
                if (heA.IsUnused) continue;

                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.AddEdge();
                edgeIndex.Set(heA, heB.Index >> 1);
            }

            // set component halfedge->halfedge refs
            for (int i = 0; i < hedges.Count; i++)
            {
                var heA0 = hedges[i];
                if (heA0.IsUnused) continue;

                // the component to which heA0 was copied
                var compHedges = comps[componentIndex.Get(heA0)].Halfedges;
                var heA1 = heA0.NextAtStart;

                // set refs
                var heB0 = compHedges[(edgeIndex.Get(heA0) << 1) + (i & 1)];
                var heB1 = compHedges[(edgeIndex.Get(heA1) << 1) + (heA1.Index & 1)];
                heB0.MakeConsecutive(heB1);

                // transfer attributes
                setHedge(heB0, heA0);
            }

            // create component vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                var vA = vertices[i];
                if (vA.IsUnused) continue;

                var heA = vA.First;
                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.Halfedges[(edgeIndex.Get(heA) << 1) + (heA.Index & 1)];

                // set vertex refs
                var vB = comp.AddVertex();
                vB.First = heB;

                foreach (var he in heB.CirculateStart)
                    he.Start = vB;

                // transfer attributes
                setVertex(vB, vA);
            }

            return comps;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="setPosition"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public TG CreateFromLineSegments(IReadOnlyList<Vec3d> points, Action<TV, Vec3d> setPosition, double tolerance = SlurMath.ZeroTolerance, bool allowMultiEdges = false, bool allowLoops = false)
        {
            var vertPos = points.RemoveCoincident(out int[] indexMap, tolerance);

            var result = Create(vertPos.Count, points.Count >> 1);
            var verts = result.Vertices;
            var hedges = result.Halfedges;

            // add vertices
            for (int i = 0; i < vertPos.Count; i++)
            {
                var v = result.AddVertex();
                setPosition(v, vertPos[i]);
            }

            // add edges
            int mask = 0;
            if (allowMultiEdges) mask |= 1;
            if (allowLoops) mask |= 2;

            // 0 - neither allowed
            // 1 - no loops allowed
            // 2 - no multi-edges allowed
            // 3 - both allowed
            switch (mask)
            {
                case 0:
                    {
                        // no multi-edges or loops allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0 != v1 && (v0.IsUnused || !v0.IsConnectedTo(v1))) result.AddEdgeImpl(v0, v1);
                        }
                        break;
                    }
                case 1:
                    {
                        // no loops allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0 != v1) result.AddEdgeImpl(v0, v1);
                        }
                        break;
                    }
                case 2:
                    {
                        // no multi-edges allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0.IsUnused || !v0.IsConnectedTo(v1)) result.AddEdgeImpl(v0, v1);
                        }
                        break;
                    }
                case 3:
                    {
                        // both allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                            result.AddEdge(indexMap[i], indexMap[i + 1]);
                        break;
                    }
            }

            return result;
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
        /// <returns></returns>
        public TG CreateFromVertexTopology<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeMeshBase<UV, UE, UF>.Vertex
            where UE : HeMeshBase<UV, UE, UF>.Halfedge
            where UF : HeMeshBase<UV, UE, UF>.Face
        {
            var result = Create(mesh.Vertices.Count, mesh.Halfedges.Count);
            result.AppendVertexTopology(mesh, setVertex, setHedge);
            return result;
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
        /// <returns></returns>
        public TG CreateFromFaceTopology<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UF> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeMeshBase<UV, UE, UF>.Vertex
            where UE : HeMeshBase<UV, UE, UF>.Halfedge
            where UF : HeMeshBase<UV, UE, UF>.Face
        {
            var result = Create(mesh.Faces.Count, mesh.Halfedges.Count);
            result.AppendFaceTopology(mesh, setVertex, setHedge);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="setVertexAttributes"></param>
        /// <param name="setHedgeAttributes"></param>
        /// <returns></returns>
        public TG CreateFromJson(string path, Action<TV, object[]> setVertexAttributes = null, Action<TE, object[]> setHedgeAttributes = null)
        {
            var result = Create();
            MeshIO.ReadFromJson(path, result, setVertexAttributes, setHedgeAttributes);
            return result;
        }
    }
}