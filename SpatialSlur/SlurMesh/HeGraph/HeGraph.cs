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
    public class HeGraph<TV, TE> : IHeStructure<TV, TE>
        where TV : HeVertex<TV, TE>
        where TE : Halfedge<TV, TE>
    {
        private HeElementList<TV> _vertices;
        private HeElementList<TE> _hedges;

        private Func<TV> _newTV;
        private Func<TE> _newTE;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeGraph(Func<TV> vertexProvider, Func<TE> hedgeProvider, int vertexCapacity = 4, int hedgeCapacity = 4)
        {
            VertexProvider = vertexProvider;
            HalfedgeProvider = hedgeProvider;

            _vertices = new HeElementList<TV>(vertexCapacity);
            _hedges = new HalfedgeList<TV, TE>(hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<TV> Vertices
        {
            get { return _vertices; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<TE> Halfedges
        {
            get { return _hedges; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Func<TV> VertexProvider
        {
            get { return _newTV; }
            set { _newTV = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Func<TE> HalfedgeProvider
        {
            get { return _newTE; }
            set { _newTE = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// Removes all elements that have been flagged for removal.
        /// </summary>
        public void Compact()
        {
            _vertices.Compact();
            _hedges.Compact();
        }


        /// <summary>
        /// Shrinks the capacity of each element list to twice its count.
        /// </summary>
        public void TrimExcess()
        {
            _vertices.TrimExcess();
            _hedges.TrimExcess();
        }


        /// <summary>
        /// Returns true if the given vertex belongs to this mesh.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Owns(TV vertex)
        {
            return _vertices.Owns(vertex);
        }


        /// <summary>
        /// Returns true if the given halfedge belongs to this mesh.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool Owns(TE hedge)
        {
            return _hedges.Owns(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("HeGraph (V:{0} E:{1})", _vertices.Count, _hedges.Count >> 1);
        }


        /// <summary>
        /// Returns a deep copy of this graph.
        /// </summary>
        public HeGraph<TV, TE> Duplicate(Action<TV, TV> setVertex, Action<TE, TE> setHedge)
        {
            var copy = HeGraph.Create(_newTV, _newTE, _vertices.Count, _hedges.Count);
            copy.Append(this, setVertex, setHedge);
            return copy;
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
        public void Append<UV, UE>(HeGraph<UV, UE> other, Action<TV, UV> setVertex, Action<TE, UE> setHedge)
            where UV : HeVertex<UV, UE>
            where UE : Halfedge<UV, UE>
        {
            if (ReferenceEquals(this, other))
                throw new ArgumentException("The graph can not be appended to itself.");

            int nhe = _hedges.Count;
            int nv = _vertices.Count;

            var otherHedges = other._hedges;
            var otherVerts = other._vertices;

            // append new elements
            for (int i = 0; i < otherHedges.Count; i += 2)
                AddEdge();

            for (int i = 0; i < otherVerts.Count; i++)
                AddVertex();

            // link new vertices to new halfedges
            for (int i = 0; i < otherVerts.Count; i++)
            {
                var v0 = otherVerts[i];
                var v1 = _vertices[i + nv];
                setVertex(v1, v0);

                if (v0.IsRemoved) continue;
                v1.FirstOut = _hedges[v0.FirstOut.Index + nhe];
            }

            // link new halfedges to new vertices and other new halfedges
            for (int i = 0; i < otherHedges.Count; i++)
            {
                var he0 = otherHedges[i];
                var he1 = _hedges[i + nhe];
                setHedge(he1, he0);

                if (he0.IsRemoved) continue;
                he1.PrevAtStart = _hedges[he0.PrevAtStart.Index + nhe];
                he1.NextAtStart = _hedges[he0.NextAtStart.Index + nhe];
                he1.Start = _vertices[he0.Start.Index + nv];
            }
        }


        /// <summary>
        /// Returns an array of connected components.
        /// For each edge in this graph, returns a handle to the corresponding element within the connected component.
        /// </summary>
        public HeGraph<TV, TE>[] SplitDisjoint(Func<TE, ElementHandle> getHandle, Action<TE, ElementHandle> setHandle, Action<TV, TV> setVertex, Action<TE, TE> setHedge)
        {
            return SplitDisjoint(HeGraphFactory.Create(this), getHandle, setHandle, setVertex, setHedge);
        }


        /// <summary>
        /// Returns an array of connected components.
        /// For each edge in this graph, returns a handle to the corresponding element within the connected component.
        /// </summary>
        public HeGraph<UV, UE>[] SplitDisjoint<UV, UE>(IFactory<HeGraph<UV, UE>> factory, Func<TE, ElementHandle> getHandle, Action<TE, ElementHandle> setHandle, Action<UV, TV> setVertex, Action<UE, TE> setHedge)
            where UV : HeVertex<UV, UE>
            where UE : Halfedge<UV, UE>
        {
            int ncomp = this.GetEdgeComponentIndices((he, i) => setHandle(he,new ElementHandle(i)));
            var comps = new HeGraph<UV, UE>[ncomp];

            // initialize components
            for (int i = 0; i < comps.Length; i++)
                comps[i] = factory.Create();

            // create component halfedges
            for (int i = 0; i < _hedges.Count; i += 2)
            {
                var heA = _hedges[i];
                if (heA.IsRemoved) continue;

                var sd = getHandle(heA);

                var comp = comps[sd.ComponentIndex];
                sd.ElementIndex = comp.AddEdge().Index >> 1;

                setHandle(heA, sd);
            }

            // set component halfedge->halfedge refs
            for (int i = 0; i < _hedges.Count; i++)
            {
                var heA0 = _hedges[i];
                if (heA0.IsRemoved) continue;

                var heA1 = heA0.NextAtStart;
                var h0 = getHandle(heA0);
                var h1 = getHandle(heA1);

                // the component to which heA0 was copied
                var compHedges = comps[h0.ComponentIndex]._hedges;

                // set refs
                var heB0 = compHedges[(h0.ElementIndex << 1) + (i & 1)];
                var heB1 = compHedges[(h1.ElementIndex << 1) + (heA1.Index & 1)];
                heB0.MakeConsecutive(heB1);

                setHedge(heB0, heA0);
            }

            // create component vertices
            for (int i = 0; i < _vertices.Count; i++)
            {
                var vA = _vertices[i];
                if (vA.IsRemoved) continue;

                var heA = vA.FirstOut;
                var h = getHandle(heA);

                var comp = comps[h.ComponentIndex];
                var vB = comp.AddVertex();

                // set vertex refs
                var heB = comp._hedges[(h.ElementIndex << 1) + (heA.Index & 1)];
                vB.FirstOut = heB;

                foreach (var he in heB.CirculateStart)
                    he.Start = vB;

                setVertex(vB, vA);
            }

            return comps;
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
        public void AppendVertexTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
            int nhe = _hedges.Count;
            int nv = _vertices.Count;

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
                var v1 = _vertices[i + nv];
                setVertex(v1, v0);

                if (v0.IsRemoved) continue;
                v1.FirstOut = _hedges[v0.FirstOut.Index + nhe];
            }

            // link new halfedges to eachother and new vertices
            for (int i = 0; i < meshHedges.Count; i++)
            {
                var he0 = meshHedges[i];
                var he1 = _hedges[i + nhe];
                setHedge(he1, he0);

                if (he0.IsRemoved) continue;
                he1.PrevAtStart = _hedges[he0.PrevAtStart.Index + nhe];
                he1.NextAtStart = _hedges[he0.NextAtStart.Index + nhe];
                he1.Start = _vertices[he0.Start.Index + nv];
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
        public void AppendFaceTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UF> setVertex, Action<TE, UE> setHedge)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
            int nhe = _hedges.Count;
            int nv = _vertices.Count;

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
                var vA = _vertices[i + nv];
                setVertex(vA, fB);

                if (fB.IsRemoved) continue;
                var heB = fB.First;

                // find first interior halfedge in the face
                while (heB.Twin.Face == null)
                {
                    heB = heB.NextInFace;
                    if (heB == fB.First) goto EndFor; // dual vertex has no valid halfedges
                }

                vA.FirstOut = _hedges[heB.Index + nhe];
                EndFor:;
            }

            // link new halfedges to eachother and new vertices
            for (int i = 0; i < meshHedges.Count; i++)
            {
                var heB0 = meshHedges[i];
                var heA0 = _hedges[i + nhe];
                setHedge(heA0, heB0);

                if (heB0.IsRemoved || heB0.IsBoundary) continue;
                var heB1 = heB0;

                // find next interior halfedge in the face
                do heB1 = heB1.NextInFace;
                while (heB1.Twin.Face == null && heB1 != heB0);

                heA0.Start = _vertices[heB0.Face.Index + nv];
                heA0.MakeConsecutive(_hedges[heB1.Index + nhe]);
            }
        }


        #region ElementOperators


        #region Edge Operators


        /// <summary>
        /// Creates a new pair of halfedges and adds them to the list.
        /// Returns the first halfedge in the pair.
        /// </summary>
        /// <returns></returns>
        internal TE AddEdge()
        {
            var he0 = _newTE();
            var he1 = _newTE();

            he0.Twin = he1;
            he1.Twin = he0;

            _hedges.Add(he0);
            _hedges.Add(he1);

            return he0;
        }


        /// <summary>
        /// Adds a new edge between the given nodes.
        /// Returns the first halfedge in the pair.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public TE AddEdge(TV v0, TV v1)
        {
            _vertices.OwnsCheck(v0);
            _vertices.OwnsCheck(v1);
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
            return AddEdgeImpl(_vertices[vi0], _vertices[vi1]);
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
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

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
            hedge.Remove();
        }


        /// <summary>
        /// Collapses the given halfedge by merging the vertices at either end.
        /// The start vertex of the given halfedge is removed.
        /// </summary>
        /// <param name="hedge"></param>
        public void CollapseEdge(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

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
            if (v0.IsRemoved) return;
            v1.InsertRange(v0.FirstOut);
            v0.Remove();
        }


        /// <summary>
        /// Splits the given edge creating a new vertex and halfedge pair.
        /// Returns the new halfedge which starts from the new vertex.
        /// </summary>
        /// <param name="hedge"></param>
        public TE SplitEdge(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

            return SplitEdgeImpl(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        private TE SplitEdgeImpl(TE hedge)
        {
            var he1 = hedge.Twin;

            var v0 = AddVertex();
            var v1 = he1.Start;

            var he2 = AddEdge();
            var he3 = he2.Twin;

            // halfedge->vertex refs
            he1.Start = he2.Start = v0;
            he3.Start = v1;

            // update vertex->halfegde refs
            v0.FirstOut = he2;
            if (v1.FirstOut == he1) v1.FirstOut = he3;

            // update halfedge->halfedge refs
            he1.PrevAtStart.MakeConsecutive(he3);
            he3.MakeConsecutive(he1.NextAtStart);
            he2.MakeConsecutive(he1);
            he1.MakeConsecutive(he2);

            return he2;
        }


        /// <summary>
        /// Returns the new halfedge starting at the new vertex.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public TE ZipEdges(TE he0, TE he1)
        {
            _hedges.OwnsCheck(he0);
            _hedges.OwnsCheck(he1);

            he0.RemovedCheck();
            he1.RemovedCheck();

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
            v1.FirstOut = he3;

            return he3;
        }


        /// <summary>
        /// Removes all edges which start and end at the same vertex.
        /// </summary>
        public void RemoveLoops(bool parallel = false)
        {
            for (int i = 0; i < _hedges.Count; i += 2)
            {
                var he = _hedges[i];
                if (!he.IsRemoved && he.Start == he.End) he.Remove();
            }
        }


        /// <summary>
        /// Removes all duplicate edges in the mesh.
        /// An edge is considered a duplicate if it connects a pair of already connected vertices.
        /// </summary>
        public void RemoveMultiEdges()
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                var v0 = _vertices[i];
                if (v0.IsRemoved) continue;

                int currTag = _vertices.NextTag;

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


        #region Halfedge Operators


        /// <summary>
        /// Detaches the given halfedge from its start vertex.
        /// </summary>
        /// <param name="hedge"></param>
        public void DetachHalfedge(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

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
            if (v0.FirstOut == hedge) v0.FirstOut = hedge.NextAtStart;
            v1.FirstOut = hedge;

            // update halfedge->vertex refs
            hedge.Start = v1;

            // update halfedge->halfedge refs
            hedge.PrevAtStart.MakeConsecutive(hedge.NextAtStart);
            hedge.MakeConsecutive(hedge);
        }


        #endregion


        #region Vertex Operators


        /// <summary>
        /// 
        /// </summary>
        public TV AddVertex()
        {
            var v = _newTV();
            _vertices.Add(v);
            return v;
        }


        /// <summary>
        /// Removes the given vertex along with all incident edges.
        /// </summary>
        /// <param name="quantity"></param>
        public void AddVertices(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                var v = _newTV();
                _vertices.Add(v);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void RemoveVertex(TV vertex)
        {
            _vertices.OwnsCheck(vertex);
            vertex.RemovedCheck();

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
                he.Remove();
            }

            vertex.Remove();
        }


        /// <summary>
        /// Transfers halfedges from the first to the second given vertex.
        /// The first vertex is flagged as unused.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public void MergeVertices(TV v0, TV v1)
        {
            _vertices.OwnsCheck(v0);
            _vertices.OwnsCheck(v1);

            v0.RemovedCheck();
            v1.RemovedCheck();

            MergeVerticesImpl(v0, v1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        private void MergeVerticesImpl(TV v0, TV v1)
        {
            v1.InsertRange(v0.FirstOut);
            v0.Remove();
        }


        /// <summary>
        /// Transfers halfedges leaving each vertex to the first vertex in the collection.
        /// All vertices except the first are flagged as unused.
        /// </summary>
        /// <param name="vertices"></param>
        public void MergeVertices(IEnumerable<TV> vertices)
        {
            var v0 = vertices.ElementAt(0);
            _vertices.OwnsCheck(v0);
            v0.RemovedCheck();

            foreach (var v1 in vertices.Skip(1))
            {
                _vertices.OwnsCheck(v1);
                v1.RemovedCheck();

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
            _hedges.OwnsCheck(he0);
            _hedges.OwnsCheck(he1);

            he0.RemovedCheck();
            he1.RemovedCheck();

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
            he0.PrevAtStart.MakeConsecutive(he2);
            he1.PrevAtStart.MakeConsecutive(he3);
            he3.MakeConsecutive(he0);
            he2.MakeConsecutive(he1);

            // update halfedge->vertex refs
            he2.Start = v0;
            foreach (var he in he0.CirculateStart) he.Start = v1;

            // update vertex->halfedge refs
            v1.FirstOut = he3;
            if (v0.FirstOut.Start == v1) v0.FirstOut = he2;

            return he3;
        }


        /// <summary>
        /// Detaches all outgoing halfedges from the given vertex
        /// </summary>
        /// <param name="vertex"></param>
        public void DetachVertex(TV vertex)
        {
            _vertices.OwnsCheck(vertex);
            vertex.RemovedCheck();

            DetachVertexImpl(vertex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        private void DetachVertexImpl(TV vertex)
        {
            var he0 = vertex.FirstOut;
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
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = _vertices[i];
                    if (v.IsRemoved) continue;
                    v.SortOutgoing(compare);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, _vertices.Count), func);
            else
                func(Tuple.Create(0, _vertices.Count));
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
            int currTag = _hedges.NextTag;

            for (int i = 0; i < _hedges.Count; i++)
            {
                var he = _hedges[i];
                if (he.IsRemoved || he.Tag == currTag) continue;

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
    /// Static constructors and extension methods for generic and empty HeGraphs.
    /// </summary>
    public static class HeGraph
    {
        /// <summary></summary>
        public static readonly HeGraphFactory<V, E> Factory = HeGraphFactory.Create(() => new V(), () => new E());


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public static HeGraph<TV, TE> Create<TV, TE>(Func<TV> vertexProvider, Func<TE> hedgeProvider, int vertexCapacity = 4, int hedgeCapacity = 4)
             where TV : HeVertex<TV, TE>
             where TE : Halfedge<TV, TE>
        {
            return new HeGraph<TV, TE>(vertexProvider, hedgeProvider, vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public static HeGraph<V, E> Create(int vertexCapacity = 4, int hedgeCapacity = 4)
        {
            return Factory.Create(vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HeGraph<V, E> TryCast(object obj)
        {
            return obj as HeGraph<V, E>;
        }


        /// <summary>
        /// Default empty vertex
        /// </summary>
        [Serializable]
        public class V : HeVertex<V, E> { }


        /// <summary>
        /// Default empty halfedge
        /// </summary>
        [Serializable]
        public class E : Halfedge<V, E> { }
    }
}
