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
    /// <typeparam name="TF"></typeparam>
    [Serializable]
    public partial class HeMesh<TV, TE, TF> : IHeStructure<TV,TE,TF>
        where TV : HeVertex<TV, TE, TF>
        where TE : Halfedge<TV, TE, TF>
        where TF : HeFace<TV, TE, TF>
    {
        private HeElementList<TV> _vertices;
        private HeElementList<TE> _hedges;
        private HeElementList<TF> _faces;

        private Func<TV> _newTV;
        private Func<TE> _newTE;
        private Func<TF> _newTF;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        /// <param name="faceProvider"></param>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        public HeMesh(Func<TV> vertexProvider, Func<TE> hedgeProvider, Func<TF> faceProvider, int vertexCapacity = 4, int hedgeCapacity = 4, int faceCapacity = 4)
        {
            VertexProvider = vertexProvider;
            HalfedgeProvider = hedgeProvider;
            FaceProvider = faceProvider;

            _vertices = new HeElementList<TV>(vertexCapacity);
            _hedges = new HeElementList<TE>(hedgeCapacity);
            _faces = new HeElementList<TF>(faceCapacity);
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
        public HeElementList<TF> Faces
        {
            get { return _faces; }
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
        /// 
        /// </summary>
        public Func<TF> FaceProvider
        {
            get { return _newTF; }
            set { _newTF = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// Returns true if all halfedges have a face.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                for (int i = 0; i < _hedges.Count; i += 2)
                {
                    var he = _hedges[i];
                    if (!he.IsRemoved && he.IsBoundary) return false;
                }

                return true;
            }
        }


        /// <summary>
        /// Returns the Euler number of the mesh.
        /// </summary>
        public int EulerNumber
        {
            get { return _vertices.Count - (_hedges.Count >> 1) + _faces.Count; }
        }
      

        /// <summary>
        /// Removes all elements in the mesh that have been flagged for removal.
        /// </summary>
        public void Compact()
        {
            _vertices.Compact();
            _hedges.Compact();
            _faces.Compact();
        }


        /// <summary>
        /// 
        /// </summary>
        public void TrimExcess()
        {
            _vertices.TrimExcess();
            _hedges.TrimExcess();
            _faces.TrimExcess();
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
        /// Returns true if the given face belongs to this mesh.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public bool Owns(TF face)
        {
            return _faces.Owns(face);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("HeMesh (V:{0} E:{1} F:{2})", _vertices.Count, _hedges.Count >> 1, _faces.Count);
        }


        /// <summary>
        /// Returns the number of holes in the mesh.
        /// </summary>
        /// <returns></returns>
        public int CountHoles()
        {
            int currTag = _hedges.NextTag;
            int n = 0;

            for (int i = 0; i < _hedges.Count; i++)
            {
                var he = _hedges[i];
                if (he.IsRemoved || he.Face != null || he.Tag == currTag) continue;

                do
                {
                    he.Tag = currTag;
                    he = he.NextInFace;
                } while (he.Tag != currTag);

                n++;
            }

            return n;
        }


        /// <summary>
        /// Returns the first halfedge from each hole in the mesh.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TE> GetHoles()
        {
            int currTag = _hedges.NextTag;

            for (int i = 0; i < _hedges.Count; i++)
            {
                var he = _hedges[i];
                if (he.IsRemoved || he.Face != null || he.Tag == currTag) continue;

                do
                {
                    he.Tag = currTag;
                    he = he.NextInFace;
                } while (he.Tag != currTag);

                yield return he;
            }
        }


        /// <summary>
        /// Returns the number of boundary vertices in the mesh.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryVertices()
        {
            int n = 0;

            for (int i = 0; i < _vertices.Count; i++)
            {
                var v = _vertices[i];
                if (!v.IsRemoved && v.IsBoundary) n++;
            }

            return n;
        }


        /// <summary>
        /// Returns each boundary vertex in the mesh.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TV> GetBoundaryVertices()
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                var v = _vertices[i];
                if (!v.IsRemoved && v.IsBoundary) yield return v;
            }
        }


        /// <summary>
        /// Returns the number of vertices with multiple incident boundary edges.
        /// </summary>
        /// <returns></returns>
        public int CountNonManifoldVertices()
        {
            var n = 0;

            for (int i = 0; i < _vertices.Count; i++)
            {
                var v = _vertices[i];
                if (!v.IsRemoved && !v.IsManifold) n++;
            }

            return n;
        }


        /// <summary>
        /// Returns each non-manifold vertex in the mesh.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TV> GetNonManifoldVertices()
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                var v = _vertices[i];
                if (!v.IsRemoved && !v.IsManifold) yield return v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh<TV, TE, TF> Duplicate(Action<TV, TV> setVertex, Action<TE, TE> setHedge, Action<TF, TF> setFace)
        {
            var factory = HeMeshFactory.Create(this);
            return factory.CreateCopy(this, setVertex, setHedge, setFace);
        }


        /// <summary>
        /// Appends a deep copy of the given mesh to this mesh.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="other"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        public void Append<UV, UE, UF>(HeMesh<UV, UE, UF> other, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Action<TF, UF> setFace)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
            if (ReferenceEquals(this, other))
                throw new ArgumentException("The mesh can not be appended to itself.");

            int nv = _vertices.Count;
            int nhe = _hedges.Count;
            int nf = _faces.Count;

            var otherVerts = other._vertices;
            var otherHedges = other._hedges;
            var otherFaces = other._faces;

            // append new elements
            for (int i = 0; i < otherVerts.Count; i++)
                AddVertex();

            for (int i = 0; i < otherHedges.Count; i += 2)
                AddEdge();
            
            for (int i = 0; i < otherFaces.Count; i++)
                AddFace();

            // link new vertices to new halfedges
            for (int i = 0; i < otherVerts.Count; i++)
            {
                var v0 = otherVerts[i];
                var v1 = _vertices[i + nv];
                setVertex(v1, v0);

                if (v0.IsRemoved) continue;
                v1.FirstOut = _hedges[v0.FirstOut.Index + nhe];
            }

            // link new faces to new halfedges
            for (int i = 0; i < otherFaces.Count; i++)
            {
                var f0 = otherFaces[i];
                var f1 = _faces[i + nf];
                setFace(f1, f0);
                
                if (f0.IsRemoved) continue;
                f1.First = _hedges[f0.First.Index + nhe];
            }

            // link new halfedges to eachother, new vertices, and new faces
            for (int i = 0; i < otherHedges.Count; i++)
            {
                var he0 = otherHedges[i];
                var he1 = _hedges[i + nhe];
                setHedge(he1, he0);

                if (he0.IsRemoved) continue;
                he1.PrevInFace = _hedges[he0.PrevInFace.Index + nhe];
                he1.NextInFace = _hedges[he0.NextInFace.Index + nhe];
                he1.Start = _vertices[he0.Start.Index + nv];

                if (he0.Face != null)
                    he1.Face = _faces[he0.Face.Index + nf];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh<TV, TE, TF> GetDual(Action<TV, TF> setVertex, Action<TE, TE> setHedge, Action<TF, TV> setFace)
        {
            var factory = HeMeshFactory.Create(this);
            return factory.CreateDual(this, setVertex, setHedge, setFace);
        }


        /// <summary>
        /// Appends the dual of the given mesh to this mesh.
        /// Note this method preserves indexical correspondance between primal and dual elements.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="other"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        public void AppendDual<UV, UE, UF>(HeMesh<UV, UE, UF> other, Action<TV, UF> setVertex, Action<TE, UE> setHedge, Action<TF, UV> setFace)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
            if (ReferenceEquals(this, other))
                throw new ArgumentException("Cannot append the dual of an HeMesh to itself.");

            int nv = _vertices.Count;
            int nhe = _hedges.Count;
            int nf = _faces.Count;

            var otherVerts = other._vertices;
            var otherHedges = other._hedges;
            var otherFaces = other._faces;

            // add new elements
            for (int i = 0; i < otherFaces.Count; i++)
                AddVertex();
            
            for (int i = 0; i < otherVerts.Count; i++)
                AddFace();

            // add halfedges and set their faces/vertices
            // spin each halfedge such that its face in the primal mesh corresponds with its start vertex in the dual
            for (int i = 0; i < otherHedges.Count; i += 2)
            {
                var heA0 = AddEdge();
                var heB0 = otherHedges[i];
                if (heB0.IsRemoved || heB0.IsBoundary) continue; // skip boundary edges

                var heB1 = otherHedges[i + 1];
                var vB0 = heB0.Start;
                var vB1 = heB1.Start;

                int mask = 0;
                if (vB0.IsBoundary) mask |= 1;
                if (vB1.IsBoundary) mask |= 2;
                if (mask == 3) continue; // skip invalid dual edges

                var heA1 = heA0.Twin;
                heA0.Start = _vertices[heB0.Face.Index + nv];
                heA1.Start = _vertices[heB1.Face.Index + nv];

                if ((mask & 1) == 0) heA1.Face = _faces[vB0.Index + nf]; // vB0 is interior
                if ((mask & 2) == 0) heA0.Face = _faces[vB1.Index + nf]; // vB1 is interior
            }

            // set halfedge -> halfedge refs
            for (int i = 0; i < otherHedges.Count; i++)
            {
                var heA0 = _hedges[i + nhe];
                var heB0 = otherHedges[i];
                setHedge(heA0, heB0);

                if (heA0.IsRemoved) continue;
                var heB1 = heB0.NextInFace;
                var heA1 = _hedges[heB1.Index + nhe];

                // backtrack around primal face, until dual halfedge is valid
                while (heA1.IsRemoved)
                {
                    heB1 = heB1.NextInFace;
                    heA1 = _hedges[heB1.Index + nhe];
                }

                heA1.Twin.MakeConsecutive(heA0);
            }

            // set dual face -> halfedge refs 
            // must be set before vertex refs to check for boundary invariant
            for (int i = 0; i < otherVerts.Count; i++)
            {
                var fA = _faces[i + nf];
                var vB = otherVerts[i];
                setFace(fA, vB);

                if (vB.IsRemoved || vB.IsBoundary) continue;
                fA.First = _hedges[vB.FirstOut.Twin.Index + nhe]; // can assume dual edge around interior vertex is valid
            }

            // set dual vertex -> halfedge refs
            for (int i = 0; i < otherFaces.Count; i++)
            {
                var vA = _vertices[i + nv];
                var fB = otherFaces[i];
                setVertex(vA, fB);

                if (fB.IsRemoved) continue;
                var heB = fB.First; // primal halfedge
                var heA = _hedges[heB.Index + nhe]; // corresponding dual halfedge

                // find first used dual halfedge
                while (heA.IsRemoved)
                {
                    heB = heB.NextInFace;
                    if (heB == fB.First) goto EndFor; // dual vertex has no valid halfedges
                    heA = _hedges[heB.Index + nhe];
                }

                vA.FirstOut = heA;
                vA.SetFirstToBoundary();

                EndFor:;
            }

            // cleanup any appended degree 2 faces
            for (int i = nf; i < _faces.Count; i++)
            {
                var f = _faces[i];
                if (!f.IsRemoved && f.IsDegree2)
                    CleanupDegree2Face(f.First);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="getHandle"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        /// <returns></returns>
        public HeMesh<TV, TE, TF>[] SplitDisjoint(Func<TE, ElementHandle> getHandle, Action<TV, TV> setVertex, Action<TE, TE> setHedge, Action<TF, TF> setFace)
        {
            return SplitDisjoint(HeMeshFactory.Create(this), getHandle, setVertex, setHedge, setFace);
        }


        /// <summary>
        /// Returns an array of connected components.
        /// For each edge in this mesh, returns a handle to the corresponding element within the connected component.
        /// </summary>
        public HeMesh<UV, UE, UF>[] SplitDisjoint<UV, UE, UF>(IFactory<HeMesh<UV,UE,UF>> factory, Func<TE, ElementHandle> getHandle, Action<UV, TV> setVertex, Action<UE, TE> setHedge, Action<UF, TF> setFace)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
            int ncomp = this.GetEdgeComponentIndices((he, i) => getHandle(he).ComponentIndex = i);
            var comps = new HeMesh<UV, UE, UF>[ncomp];

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
            }

            // set component halfedge->halfedge refs
            for (int i = 0; i < _hedges.Count; i++)
            {
                var heA0 = _hedges[i];
                if (heA0.IsRemoved) continue;

                var heA1 = heA0.NextInFace;
                var sd0 = getHandle(heA0);
                var sd1 = getHandle(heA1);

                // the component to which heA0 was copied
                var compHedges = comps[sd0.ComponentIndex]._hedges;

                // set refs
                var heB0 = compHedges[(sd0.ElementIndex << 1) + (i & 1)];
                var heB1 = compHedges[(sd1.ElementIndex << 1) + (heA1.Index & 1)];
                heB0.MakeConsecutive(heB1);

                setHedge(heB0, heA0);
            }

            // create component vertices
            for (int i = 0; i < _vertices.Count; i++)
            {
                var vA = _vertices[i];
                if (vA.IsRemoved) continue;

                var heA = vA.FirstOut;
                var sd = getHandle(heA);

                var comp = comps[sd.ComponentIndex];
                var vB = comp.AddVertex();

                // set vertex refs
                var heB = comp._hedges[(sd.ElementIndex << 1) + (heA.Index & 1)];
                vB.FirstOut = heB;

                foreach (var he in heB.CirculateStart)
                    he.Start = vB;

                setVertex(vB, vA);
            }

            // create component faces
            for (int i = 0; i < _faces.Count; i++)
            {
                var fA = _faces[i];
                if (fA.IsRemoved) continue;

                var heA = fA.First;
                var sd = getHandle(heA);

                var comp = comps[sd.ComponentIndex];
                var fB = comp.AddFace();

                // set face refs
                var heB = comp._hedges[(sd.ElementIndex << 1) + (heA.Index & 1)];
                fB.First = heB;

                foreach (var he in heB.CirculateFace)
                    he.Face = fB;

                setFace(fB, fA);
            }

            return comps;
        }


        #region Element Operators


        #region Edge Operators


        /// <summary>
        /// Creates a new pair of halfedges and adds them to the list.
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
        /// Creates a new pair of halfedges between the given vertices and add them to the list.
        /// Returns the halfedge starting from v0.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        internal TE AddEdge(TV v0, TV v1)
        {
            var he = AddEdge();
            he.Start = v0;
            he.Twin.Start = v1;
            return he;
        }


        /// <summary>
        /// Detatches the given edge and flags it for removal.
        /// Note that this method does not update face->halfedge refs
        /// </summary>
        /// <param name="hedge"></param>
        private void RemoveEdge(TE hedge)
        {
            hedge.Bypass();
            hedge.Twin.Bypass();
            hedge.Remove();
        }


        /// <summary>
        /// Splits the given edge at the specified paramter creating a new vertex and halfedge pair.
        /// Returns the new halfedge which starts at the new vertex.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public TE SplitEdge(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

            return SplitEdgeImpl(hedge);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        internal TE SplitEdgeImpl(TE hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            var v0 = AddVertex();
            var v1 = he1.Start;

            var he2 = AddEdge(v0, v1);
            var he3 = he2.Twin;

            // update halfedge->vertex refs
            he1.Start = v0;

            // update halfedge->face refs
            he2.Face = he0.Face;
            he3.Face = he1.Face;

            // update vertex->halfedge refs if necessary
            if (v1.FirstOut == he1)
            {
                v1.FirstOut = he3;
                v0.FirstOut = he1;
            }
            else
            {
                v0.FirstOut = he2;
            }

            // update halfedge->halfedge refs
            he2.MakeConsecutive(he0.NextInFace);
            he0.MakeConsecutive(he2);
            he1.PrevInFace.MakeConsecutive(he3);
            he3.MakeConsecutive(he1);

            return he2;
        }


        /*
        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal TE SplitEdgeImpl(TE halfedge, double t)
        {
            TE he0 = halfedge;
            TE he1 = he0.Twin;

            HeVertex v0 = he0.Start;
            HeVertex v1 = Parent.Vertices.Add(he0.PointAt(t));

            TE he2 = AddPair(v0, v1);
            TE he3 = he2.Twin;

            // update halfedge->vertex refs
            he0.Start = v1;

            // update halfedge->face refs
            he2.Face = he0.Face;
            he3.Face = he1.Face;

            // update vertex->halfedge refs if necessary
            if (v0.First == he0)
            {
                v0.First = he2;
                v1.First = he0;
            }
            else
            {
                v1.First = he3;
            }

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he0.Previous, he2);
            Halfedge.MakeConsecutive(he2, he0);
            Halfedge.MakeConsecutive(he3, he1.Next);
            Halfedge.MakeConsecutive(he1, he3);

            return he2;
        }
        */


        /// <summary>
        /// Splits an edge by adding a new vertex in the middle. 
        /// Faces adjacent to the given edge are also split at the new vertex.
        /// Returns the new halfedge outgoing from the new vertex or null on failure.
        /// Assumes triangle mesh.
        /// </summary>
        public TE SplitEdgeFace(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

            return SplitEdgeFaceImpl(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private TE SplitEdgeFaceImpl(TE hedge)
        {
            // split edge
            var he0 = SplitEdgeImpl(hedge);
            var he1 = he0.NextAtStart;

            // split left face if it exists
            if (he0.Face != null)
                SplitFaceImpl(he0, he0.NextInFace.NextInFace);

            // split right face if it exists
            if (he1.Face != null)
                SplitFaceImpl(he1, he1.NextInFace.NextInFace);

            return he0;
        }


        /// <summary>
        /// Collapses the given halfedge by merging the vertices at either end.
        /// If successful, the start vertex of the given halfedge is removed.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool CollapseEdge(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

            return CollapseEdgeImpl(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private bool CollapseEdgeImpl(TE hedge)
        {
            if (!CanCollapse(hedge))
                return false;

            var he0 = hedge; // to be removed
            var he1 = he0.Twin; // to be removed

            var v0 = he0.Start; // to be removed
            var v1 = he1.Start;

            var he2 = he0.NextInFace;
            var he3 = he1.NextInFace;

            var f0 = he0.Face;
            var f1 = he1.Face;

            // update start vertex of all halfedges starting at v0
            foreach (var he in he0.CirculateStart.Skip(1))
                he.Start = v1;

            // update outgoing halfedge from v1 if necessary
            var he4 = v0.FirstOut;

            if (he4.Face == null && he4 != he0)
                v1.FirstOut = he4;
            else if (v1.FirstOut == he1)
                v1.FirstOut = he3;

            // update halfedge-halfedge refs
            he0.PrevInFace.MakeConsecutive(he2);
            he1.PrevInFace.MakeConsecutive(he3);

            // update face->halfedge refs or handle collapsed faces/holes
            if (f0 == null)
            {
                if (he2.IsInDegree1)
                    CleanupDegree1Hole(he2);
            }
            else
            {
                if (he2.IsInDegree2)
                    CleanupDegree2Face(he2);
                else if (f0.First == he0)
                    f0.First = he2;
            }

            if (f1 == null)
            {
                if (he3.IsInDegree1)
                    CleanupDegree1Hole(he3);
            }
            else
            {
                if (he3.IsInDegree2)
                    CleanupDegree2Face(he3);
                else if (f1.First == he1)
                    f1.First = he3;
            }

            // flag elements for removal
            v0.Remove();
            he0.Remove();

            return true;
        }


        /// <summary>
        /// Returns false if vertices at either end of the given edge share more than
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private bool CanCollapse(TE hedge)
        {
            // avoids creation of non-manifold vertices
            // if (hedge.IsBridge) return false;

            var he0 = hedge;
            var he1 = hedge.PrevAtStart;

            for (int i = 0; i < 2; i++)
            {
                he0 = he0.NextAtStart;
                if (he0 == he1) return true;
            }

            var he2 = hedge;
            var he3 = he2.Twin.PrevInFace;

            for (int i = 0; i < 2; i++)
            {
                he2 = he2.NextInFace.Twin;
                if (he2 == he3) return true;
            }

            // check for common vertices
            he0 = he0.Twin;
            he1 = he1.Twin;
            int currTag = _vertices.NextTag;

            // tag verts between he0 and he1
            do
            {
                he0.Start.Tag = currTag;
                he0 = he0.NextInFace.Twin;
            } while (he0 != he1);

            // check for tags between he2 and he3
            do
            {
                if (he2.Start.Tag == currTag) return false;
                he2 = he2.NextInFace.Twin;
            } while (he2 != he3);

            return true;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private bool CanCollapse(HeMeshHalfedge hedge)
        {
            var he0 = hedge;
            var he1 = hedge.Twin;

            int n = 0;
            if (he0.IsInDegree3) n++;
            if (he1.IsInDegree3) n++;

            return Owner.Vertices.CountCommonNeighboursImpl(he0.Start, he1.Start) == n;
        }
        */


        /// <summary>
        /// 
        /// </summary>
        private void CleanupDegree2Face(TE hedge)
        {
            var he0 = hedge; // to be removed
            var he1 = he0.Twin; // to be removed
            var he2 = he0.NextInFace;
            var he3 = he1.NextInFace;

            var v0 = he0.Start;
            var v1 = he1.Start;

            var f0 = he0.Face; // to be removed
            var f1 = he1.Face;

            // update vertex->halfedge refs
            if (v0.FirstOut == he0) v0.FirstOut = he3;
            if (v1.FirstOut == he1) v1.FirstOut = he2;

            // update face->halfedge refs
            if (f1 != null && f1.First == he1) f1.First = he2;

            // update halfedge->halfedge refs
            he1.PrevInFace.MakeConsecutive(he2);
            he2.MakeConsecutive(he3);

            // update halfedge->face ref
            he2.Face = f1;

            // handle potential invalid edge
            if (!he2.IsValid) RemoveEdge(he2);

            // flag for removal
            f0.Remove();
            he0.Remove();
        }


        /// <summary>
        /// 
        /// </summary>
        private void CleanupDegree2Hole(TE hedge)
        {
            var he0 = hedge; // to be removed
            var he1 = he0.Twin; // to be removed
            var he2 = he0.NextInFace;
            var he3 = he1.NextInFace;

            var v0 = he0.Start;
            var v1 = he1.Start;

            var f1 = he1.Face;

            // update vertex->halfedge refs
            // must look for another faceless halfedge to maintain boundary invariant for v0 and v1
            if (v0.FirstOut == he0)
            {
                var he = he0.NextBoundaryAtStart;
                v0.FirstOut = (he == null) ? he3 : he;
            }

            if (v1.FirstOut == he2)
            {
                var he = he2.NextBoundaryAtStart;
                v1.FirstOut = (he == null) ? he2 : he;
            }

            // update face->halfedge refs
            if (f1.First == he1) f1.First = he2;

            // update halfedge->face refs
            he2.Face = f1;

            // update halfedge->halfedge refs
            he1.PrevInFace.MakeConsecutive(he2);
            he2.MakeConsecutive(he3);

            // flag elements for removal
            he0.Remove();
        }


        /// <summary>
        ///
        /// </summary>
        private void CleanupDegree1Hole(TE hedge)
        {
            var he0 = hedge; // to be removed
            var he1 = he0.Twin; // to be removed

            var v0 = he0.Start;
            var f1 = he1.Face;

            // update vertex->halfedge refs
            // must look for another boundary halfedge to maintain boundary invariant for v0
            if (v0.FirstOut == he0)
            {
                var he = he0.NextBoundaryAtStart;
                v0.FirstOut = (he == null) ? he1.NextInFace : he;
            }

            // update face->halfedge refs
            if (f1.First == he1) f1.First = he1.NextInFace;

            // update halfedge->halfedge refs
            he1.PrevInFace.MakeConsecutive(he1.NextInFace);

            // flag elements for removal
            he0.Remove();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool SpinEdge(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

            // halfedge must be adjacent to 2 faces
            if (hedge.IsBoundary)
                return false;

            // don't allow for the creation of valence 1 vertices
            if (hedge.IsAtDegree2 || hedge.Twin.IsAtDegree2)
                return false;

            SpinEdgeImpl(hedge);
            return true;
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        private void SpinEdgeImpl(TE hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            var he2 = he0.NextInFace;
            var he3 = he1.NextInFace;

            var v0 = he0.Start;
            var v1 = he1.Start;

            // update vertex->halfedge refs if necessary
            if (v0.FirstOut == he0) v0.FirstOut = he3;
            if (v1.FirstOut == he1) v1.FirstOut = he2;

            // update halfedge->vertex refs
            he0.Start = he3.End;
            he1.Start = he2.End;

            var f0 = he0.Face;
            var f1 = he1.Face;

            // update face->halfedge refs if necessary
            if (he2 == f0.First) f0.First = he2.NextInFace;
            if (he3 == f1.First) f1.First = he3.NextInFace;

            // update halfedge->face refs
            he2.Face = f1;
            he3.Face = f0;

            // update halfedge->halfedge refs
            he0.MakeConsecutive(he2.NextInFace);
            he1.MakeConsecutive(he3.NextInFace);
            he1.PrevInFace.MakeConsecutive(he2);
            he0.PrevInFace.MakeConsecutive(he3);
            he2.MakeConsecutive(he1);
            he3.MakeConsecutive(he0);
        }


        /// <summary>
        /// Returns the new boundary halfedge created at the detach interface.
        /// The twin of the given halfedge will have a null face reference after detaching.
        /// </summary>
        /// <param name="hedge"></param>
        public TE DetachEdge(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

            // halfedge must be adjacent to 2 faces
            if (hedge.IsBoundary)
                return null;

            return DetachEdgeImpl(hedge);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        private TE DetachEdgeImpl(TE hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            int mask = 0;
            if (he0.Start.IsBoundary) mask |= 1;
            if (he1.Start.IsBoundary) mask |= 2;

            switch (mask)
            {
                case 0:
                    return DetachEdgeInterior(he0);
                case 1:
                    return DetachEdgeMixed(he1);
                case 2:
                    return DetachEdgeMixed(he0);
                case 3:
                    return DetachEdgeBoundary(he0);
            }

            return null;
        }


        /// <summary>
        /// Neither vertex is on the mesh boundary
        /// </summary>
        private TE DetachEdgeInterior(TE hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = he1.Start;

            var he2 = AddEdge(v1, v0);
            var he3 = he2.Twin;

            var f1 = he1.Face;

            // update halfedge-face refs
            he2.Face = f1;
            he3.Face = null;
            he1.Face = null;

            //update face-halfedge ref if necessary
            if (f1.First == he1)
                f1.First = he2;

            // update halfedge-halfedge refs @ v0
            he2.MakeConsecutive(he1.NextInFace);
            he1.MakeConsecutive(he3);

            // update halfedge-halfedge refs @ v1
            he1.PrevInFace.MakeConsecutive(he2);
            he3.MakeConsecutive(he1);

            // update vertex-halfedge refs
            v0.FirstOut = he3;
            v1.FirstOut = he1;

            return he3;
        }


        /// <summary>
        /// Both vertices are on the mesh boundary
        /// </summary>
        private TE DetachEdgeBoundary(TE hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = he1.Start;
            var v2 = AddVertex();
            var v3 = AddVertex();

            var he2 = AddEdge(v3, v2);
            var he3 = he2.Twin;
            var he4 = v0.FirstOut;
            var he5 = v1.FirstOut;

            var f1 = he1.Face;

            // update halfedge-face refs
            he2.Face = f1;
            he3.Face = null;
            he1.Face = null;

            //update face-halfedge ref if necessary
            if (f1.First == he1)
                f1.First = he2;

            // update halfedge-halfedge refs @ v0
            he2.MakeConsecutive(he1.NextInFace);
            he4.PrevInFace.MakeConsecutive(he3);
            he1.MakeConsecutive(he4);

            // update halfedge-halfedge refs @ v1
            he1.PrevInFace.MakeConsecutive(he2);
            he5.PrevInFace.MakeConsecutive(he1);
            he3.MakeConsecutive(he5);

            // update vertex-halfedge refs
            v1.FirstOut = he1;
            v2.FirstOut = he3;
            v3.FirstOut = he5;

            //update halfedge-vertex refs around each new vert
            var he = he2;
            do
            {
                he.Start = v3;
                he = he.Twin.NextInFace;
            } while (he != he2);

            he = he3;
            do
            {
                he.Start = v2;
                he = he.Twin.NextInFace;
            } while (he != he3);

            return he3;
        }


        /// <summary>
        /// TV at the end of the given halfedge is on the boundary.
        /// </summary>
        private TE DetachEdgeMixed(TE hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = he1.Start;
            var v2 = AddVertex();

            var he2 = AddEdge(v2, v0);
            var he3 = he2.Twin;
            var he4 = v1.FirstOut;

            var f1 = he1.Face;

            // update halfedge-face refs
            he2.Face = f1;
            he3.Face = null;
            he1.Face = null;

            //update face-halfedge ref if necessary
            if (f1.First == he1)
                f1.First = he2;

            // update halfedge-halfedge refs @ v0
            he2.MakeConsecutive(he1.NextInFace);
            he1.PrevInFace.MakeConsecutive(he2);

            // update halfedge-halfedge refs @ v1
            he4.PrevInFace.MakeConsecutive(he1);
            he1.MakeConsecutive(he3);
            he3.MakeConsecutive(he4);

            // update vertex-halfedge refs
            v0.FirstOut = he3;
            v1.FirstOut = he1;
            v2.FirstOut = he4;

            //update halfedge-vertex refs around each new vert
            var he = he2;
            do
            {
                he.Start = v2;
                he = he.Twin.NextInFace;
            } while (he != he2);

            return he3;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public bool MergeEdges(TE he0, TE he1)
        {
            he0.UsedCheck();
            he1.UsedCheck();
            OwnsCheck(he0);
            OwnsCheck(he1);

            // both halfedges must be on boundary
            if (he0.Face != null || he1.Face != null)
                return false;

            // can't merge edges which belong to the same face
            if (he0.Twin.Face == he1.Twin.Face)
                return false;

            // TODO doesn't consider edges 
            TE he2 = he0.Next;
            TE he3 = he1.Next;

            if (he2 == he1)
                ZipEdgeImpl(he0);
            else if (he1.Next == he0)
                ZipEdgeImpl(he1);
            else
                MergeEdgesImpl(he0, he1);

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        internal void MergeEdgesImpl(TE he0, TE he1)
        {
            TE he2 = he0.Twin;
            TE he3 = he1.Twin;

            HeVertex v0 = he0.Start;
            HeVertex v1 = he1.Start;
            HeVertex v2 = he2.Start;
            HeVertex v3 = he3.Start;

            HeFace f3 = he3.Face;

            // update vertex refs for all halfedges around v1
            foreach (TE he in he1.CirculateStart.Skip(1))
                he.Start = v2;

            // update vertex refs for all halfedges around v3
            foreach (TE he in he3.CirculateStart.Skip(1))
                he.Start = v0;

            // update vertex->halfedge refs
            v0.First = he1.Next;
            v1.First = he0.Next;

            // update halfedge->face refs
            if (f3.First == he3) f3.First = he0;
            he0.Face = f3;

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he1.Previous, he0.Next);
            Halfedge.MakeConsecutive(he0.Previous, he1.Next);
            Halfedge.MakeConsecutive(he0, he3.Next);
            Halfedge.MakeConsecutive(he3.Previous, he0);

            // flag elements for removal
            he1.MakeUnused();
            he3.MakeUnused();
            v1.MakeUnused();
            v3.MakeUnused();
        }


          /// <summary>
          /// 
          /// </summary>
          /// <param name="halfedge"></param>
          public bool ZipEdge(TE halfedge)
          {
              halfedge.UsedCheck();
              OwnsCheck(halfedge);

              // halfedge must be on boundary
              if (halfedge.Face != null)
                  return false;

              // can't zip from valence 2 vertex
              if (halfedge.Next.IsFromDegree2)
                  return false;

              ZipEdgeImpl(halfedge);
              return true;
          }


          /// <summary>
          /// 
          /// </summary>
          /// <param name="he0"></param>
          internal void ZipEdgeImpl(TE halfedge)
          {
              TE he0 = halfedge;
              TE he1 = he0.Next;
              TE he2 = he1.Twin;

              HeVertex v0 = he0.Start;
              HeVertex v1 = he1.Start;
              HeVertex v2 = he2.Start;

              HeFace f2 = he2.Face;

              // update vertex refs for all halfedges around v2
              foreach (TE he in he2.CirculateStart.Skip(1))
                  he.Start = v0;

              // update vertex->halfedge refs
              v0.First = he1.Next;
              TE he3 = he2.Next.FindBoundaryAtStart(); // check for another boundary edge at v1
              v1.First = (he3 == he1) ? he0.Twin : he3;

              // update halfedge->face refs
              if (f2.First == he2) f2.First = he0;
              he0.Face = f2;

              // update halfedge->halfedge refs
              Halfedge.MakeConsecutive(he0.Previous, he1.Next);
              Halfedge.MakeConsecutive(he2.Previous, he0);
              Halfedge.MakeConsecutive(he0, he2.Next);

              // flag elements as unused
              he1.MakeUnused();
              he2.MakeUnused();
              v2.MakeUnused();
          }
          */


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
        /// 
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
        /// <returns></returns>
        public void Remove(TV vertex)
        {
            _vertices.OwnsCheck(vertex);
            vertex.RemovedCheck();
            RemoveImpl(vertex);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private void RemoveImpl(TV vertex)
        {
            var he = vertex.FirstOut;
            int n = vertex.Degree;

            for (int i = 0; i < n; i++)
            {
                if (!he.IsInHole) RemoveFaceImpl(he.Face);
                he = he.Twin.NextInFace;
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
        public bool MergeVertices(TV v0, TV v1)
        {
            _vertices.OwnsCheck(v0);
            _vertices.OwnsCheck(v1);

            v0.RemovedCheck();
            v1.RemovedCheck();

            if (v0 == v1)
                return false;

            if (!v0.IsBoundary || !v1.IsBoundary)
                return false;

            return MergeVerticesImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        private bool MergeVerticesImpl(TV v0, TV v1)
        {
            var he0 = v0.FirstOut;
            var he1 = v1.FirstOut;
            var he2 = he0.PrevInFace;
            var he3 = he1.PrevInFace;

            // if vertices are connected, just collapse the edge between them
            if (he0 == he3)
                return CollapseEdgeImpl(he0);
            else if (he1 == he2)
                return CollapseEdgeImpl(he1);

            // update halfedge->vertex refs for all edges emanating from v1
            foreach (var he in v0.OutgoingHalfedges)
                he.Start = v1;

            // update halfedge->halfedge refs
            he3.MakeConsecutive(he0);
            he2.MakeConsecutive(he1);

            // deal with potential collapse of boundary loops on either side of the merge
            if (he1.NextInFace == he2)
                CleanupDegree2Hole(he1);

            if (he0.NextInFace == he3)
                CleanupDegree2Hole(he0);

            // flag elements for removal
            v0.Remove();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        public void DetachVertex(TE he0, TE he1)
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
        public TE SplitVertex(TE he0, TE he1)
        {
            _hedges.OwnsCheck(he0);
            _hedges.OwnsCheck(he1);

            he0.RemovedCheck();
            he1.RemovedCheck();

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
        private TE SplitVertexImpl(TE he0, TE he1)
        {
            // if the same edge or vertex is degree 2 then just split the edge
            if (he0 == he1 || he0.Twin.NextInFace == he1)
                return SplitEdgeImpl(he0.Twin);

            var v0 = he0.Start;
            var v1 = AddVertex();

            var he2 = AddEdge(v0, v1);
            var he3 = he2.Twin;

            // update halfedge->face refs
            he2.Face = he0.Face;
            he3.Face = he1.Face;

            // update start vertex of all outoging edges between he0 and he1
            var he = he0;
            do
            {
                he.Start = v1;
                he = he.Twin.NextInFace;
            } while (he != he1);

            // update vertex->halfedge refs if necessary
            if (v0.FirstOut.Start == v1)
            {
                // if v0's outgoing halfedge now starts at v1, can assume v1 is now on the boundary if v0 was originally
                v1.FirstOut = v0.FirstOut;
                v0.FirstOut = he2;
            }
            else
            {
                v1.FirstOut = he3;
            }

            // update halfedge->halfedge refs
            he0.PrevInFace.MakeConsecutive(he2);
            he2.MakeConsecutive(he0);
            he1.PrevInFace.MakeConsecutive(he3);
            he3.MakeConsecutive(he1);

            return he3;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void ChamferVertex(TV vertex)
        {
            _vertices.OwnsCheck(vertex);
            vertex.RemovedCheck();
            ChamferVertexImpl(vertex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        private void ChamferVertexImpl(TV vertex)
        {
            // TODO
            throw new NotImplementedException();
        }


        #endregion


        #region Face Operators


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal TF AddFace()
        {
            var f = _newTF();
            _faces.Add(f);
            return f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vi0"></param>
        /// <param name="vi1"></param>
        /// <param name="vi2"></param>
        /// <returns></returns>
        public TF AddFace(int vi0, int vi1, int vi2)
        {
            return AddFace(new int[] { vi0, vi1, vi2 });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vi0"></param>
        /// <param name="vi1"></param>
        /// <param name="vi2"></param>
        /// <param name="vi3"></param>
        /// <returns></returns>
        public TF AddFace(int vi0, int vi1, int vi2, int vi3)
        {
            return AddFace(new int[] { vi0, vi1, vi2, vi3 });
        }
        

        /// <summary>
        /// Adds a new face to the mesh.
        /// </summary>
        /// <param name="vertexIndices"></param>
        /// <returns></returns>
        public TF AddFace(IEnumerable<int> vertexIndices)
        {
            int currTag = _vertices.NextTag;
            int n = 0;

            // check for duplicates
            foreach(var vi in vertexIndices)
            {
                var v = _vertices[vi];
                if (v.Tag == currTag) return null;
                v.Tag = currTag;
                n++;
            }

            // no degenerate faces allowed
            if (n < 3) return null;
      
            var faceVerts = new TV[n];
            vertexIndices.Select(vi => _vertices[vi]).ToArray(faceVerts);
            return AddFaceImpl(faceVerts);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public TF AddFace(TV v0, TV v1, TV v2)
        {
            return AddFace(new TV[] { v0, v1, v2 });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public TF AddFace(TV v0, TV v1, TV v2, TV v3)
        {
            return AddFace(new TV[] { v0, v1, v2, v3 });
        }


        /// <summary>
        /// Adds a new face to the mesh.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public TF AddFace(IEnumerable<TV> vertices)
        {
            int currTag = _vertices.NextTag;
            int n = 0;

            // validate vertices and check for duplicates
            foreach (var v in vertices)
            {
                _vertices.OwnsCheck(v);
                if (v.Tag == currTag) return null;
                v.Tag = currTag;
                n++;
            }

            // no degenerate faces allowed
            if (n < 3) return null;

            var faceVerts = new TV[n];
            vertices.ToArray(faceVerts);
            return AddFaceImpl(faceVerts);
        }


        /// <summary>
        /// http://pointclouds.org/blog/nvcs/martin/index.php
        /// </summary>
        /// <param name="faceVerts"></param>
        /// <returns></returns>
        private TF AddFaceImpl(TV[] faceVerts)
        {
            int n = faceVerts.Length;
            var faceHedges = new TE[n];

            // collect all existing halfedges in the new face
            for (int i = 0; i < n; i++)
            {
                var v = faceVerts[i];
                if (v.IsRemoved) continue;

                // can't create a new face with an interior vertex
                if (!v.IsBoundary) return null;

                // search for an existing halfedge between consecutive vertices
                var he = v.FindHalfedge(faceVerts[(i + 1) % n]);
                if (he == null) continue; // no existing halfedge

                // can't create a new face if the halfedge already has one
                if (he.Face != null) return null;
                faceHedges[i] = he;
            }

            /*
            // avoids creation of non-manifold vertices
            // if two consecutive new halfedges share a used vertex then that vertex will be non-manifold upon adding the face
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                if (faceHedges[i] == null && faceHedges[j] == null && !faceVerts[j].IsUnused) 
                    return null;
            }
            */

            // create the new face
            var newFace = AddFace();

            // create any missing halfedge pairs in the face loop and assign the new face
            for (int i = 0; i < n; i++)
            {
                var he = faceHedges[i];

                // if missing a halfedge, add a pair between consecutive vertices
                if (he == null)
                {
                    he = AddEdge(faceVerts[i], faceVerts[(i + 1) % n]);
                    faceHedges[i] = he;
                }

                he.Face = newFace;
            }

            // link consecutive halfedges
            for (int i = 0; i < n; i++)
            {
                var he0 = faceHedges[i];
                var he1 = faceHedges[(i + 1) % n];

                var he2 = he0.NextInFace;
                var he3 = he1.PrevInFace;
                var he4 = he0.Twin;

                var v0 = he0.Start;
                var v1 = he1.Start;

                // check if halfedges are newly created
                // new halfedges will have null previous or next refs
                int mask = 0;
                if (he2 == null) mask |= 1; // e0 is new
                if (he3 == null) mask |= 2; // e1 is new

                // 0 - neither halfedge is new
                // 1 - he0 is new, he1 is old
                // 2 - he1 is new, he0 is old
                // 3 - both halfedges are new
                switch (mask)
                {
                    case 0:
                        {
                            // neither halfedge is new
                            // if he0 and he1 aren't consecutive, then deal with non-manifold vertex as per http://www.pointclouds.org/blog/nvcs/
                            // otherwise, update the first halfedge at v1
                            if (he2 != he1)
                            {
                                var he = he1.NextBoundaryAtStart; // find the next boundary halfedge around v1 (must exist if halfedges aren't consecutive)
                                v1.FirstOut = he;

                                he.PrevInFace.MakeConsecutive(he2);
                                he3.MakeConsecutive(he);
                                he0.MakeConsecutive(he1);
                            }
                            else
                            {
                                v1.SetFirstToBoundary();
                            }

                            break;
                        }
                    case 1:
                        {
                            // he0 is new, he1 is old
                            he3.MakeConsecutive(he4);
                            v1.FirstOut = he4;
                            goto default;
                        }
                    case 2:
                        {
                            // he1 is new, he0 is old
                            he1.Twin.MakeConsecutive(he2);
                            goto default;
                        }
                    case 3:
                        {
                            // both halfedges are new
                            // deal with non-manifold case if v1 is already in use
                            if (v1.IsRemoved)
                            {
                                he1.Twin.MakeConsecutive(he4);
                            }
                            else
                            {
                                v1.FirstOut.PrevInFace.MakeConsecutive(he4);
                                he1.Twin.MakeConsecutive(v1.FirstOut);
                            }

                            v1.FirstOut = he4;
                            goto default;
                        }
                    default:
                        {
                            he0.MakeConsecutive(he1); // update refs for inner halfedges
                            break;
                        }
                }
            }

            newFace.First = faceHedges[0]; // set first halfedge in the new face
            return newFace;
        }


        /// <summary>
        /// Removes a face from the mesh as well as any invalid elements created in the process.
        /// Returns true on success.
        /// </summary>
        /// <param name="face"></param>
        public void RemoveFace(TF face)
        {
            _faces.OwnsCheck(face);
            face.RemovedCheck();
            RemoveFaceImpl(face);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        private void RemoveFaceImpl(TF face)
        {
            /*
            // avoids creation of non-manifold vertices
            foreach (HeVertex v in face.Vertices)
                if (v.IsBoundary && v.First.Twin.Face != face) return false;
            */

            // update halfedge->face refs
            var he = face.First;
            do
            {
                if (he.Twin.Face == null)
                {
                    RemoveEdge(he);
                }
                else
                {
                    he.Start.FirstOut = he;
                    he.Face = null;
                }

                he = he.NextInFace;
            } while (he.Face == face);

            // flag for removal
            face.Remove();
        }


        /// <summary>
        /// Removes a halfedge pair, merging their two adajcent faces.
        /// The face of the given halfedge is removed.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool MergeFaces(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();
            return MergeFacesImpl(hedge);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private bool MergeFacesImpl(TE hedge)
        {
            if (hedge.IsInHole)
                return MergeHoleToFace(hedge);
            else if (hedge.Twin.IsInHole)
                return MergeFaceToHole(hedge);
            else
                return MergeFaceToFace(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        private bool MergeFaceToFace(TE hedge)
        {
            var he0 = hedge;
            var he1 = hedge;

            var f1 = hedge.Twin.Face;

            // backtrack to previous non-shared halfedge in f0
            do
            {
                he0 = he0.PrevInFace;
                if (he0 == hedge) return false; // all edges in f0 are shared with f1, can't merge
            } while (he0.Twin.Face == f1);

            // advance to next non-shared halfedge in f0
            do
            {
                he1 = he1.NextInFace;
            } while (he1.Twin.Face == f1);

            // ensure single string of shared edges between f0 and f1
            {
                var he = he1;
                do
                {
                    if (he.Twin.Face == f1) return false; // multiple strings of shared edges detected, can't merge
                    he = he.NextInFace;
                } while (he != he0);
            }

            // advance to first shared halfedge
            he0 = he0.NextInFace;

            // update halfedge->face refs
            {
                var he = he1;
                do
                {
                    he.Face = f1;
                    he = he.NextInFace;
                } while (he != he0);
            }

            // remove shared edges
            {
                var he = he0;
                do
                {
                    RemoveEdge(he);
                    he = he.NextInFace;
                } while (he != he1);
            }

            // update face->halfedge ref if necessary
            if (f1.First.IsRemoved) f1.First = he1;

            // flag face as unused
            he0.Face.Remove();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        private bool MergeFaceToHole(TE hedge)
        {
            RemoveFaceImpl(hedge.Face);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        private bool MergeHoleToFace(TE hedge)
        {
            var he0 = hedge; // has null face
            var he1 = hedge;

            var f1 = hedge.Twin.Face;

            // backtrack to previous non-shared halfedge in f0
            do
            {
                he0 = he0.PrevInFace;
                if (he0 == hedge) return false; // all edges in f0 are shared with f1, can't merge
            } while (he0.Twin.Face == f1);

            // advance to next non-shared halfedge in f0
            do
            {
                he1 = he1.NextInFace;
            } while (he1.Twin.Face == f1);

            // ensure single string of shared edges between f0 and f1
            {
                var he = he1;
                do
                {
                    if (he.Twin.Face == f1) return false; // multiple strings of shared edges detected, can't merge
                    he = he.NextInFace;
                } while (he != he0);
            }

            // advance to first shared halfedge
            he0 = he0.NextInFace;

            // update halfedge->face refs and vertex->halfedge refs if necessary
            {
                var he = he1;
                do
                {
                    he.Face = f1;
                    he.Start.SetFirstToBoundary();
                    he = he.NextInFace;
                } while (he != he0);
            }

            // remove shared edges
            {
                var he = he0;
                do
                {
                    RemoveEdge(he);
                    he = he.NextInFace;
                } while (he != he1);
            }

            // update face->halfedge ref if necessary
            if (f1.First.IsRemoved) f1.First = he1;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public TF FillHole(TE hedge)
        {
            _hedges.OwnsCheck(hedge);
            hedge.RemovedCheck();

            // halfedge must be in a hole with at least 3 edges
            if (!hedge.IsInHole && hedge.IsInDegree2)
                return null;

            return FillHoleImpl(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private TF FillHoleImpl(TE hedge)
        {
            var f = AddFace();
            f.First = hedge;

            foreach (var he0 in hedge.CirculateFace)
            {
                he0.Face = f;
                he0.Start.SetFirstToBoundary();
            }

            return f;
        }


        /// <summary>
        /// Splits a face by creating a new halfedge pair between the start vertices of the given halfedges.
        /// Returns the new halfedge that shares a start vertex with he0.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public TE SplitFace(TE he0, TE he1)
        {
            _hedges.OwnsCheck(he0);
            _hedges.OwnsCheck(he1);

            he0.RemovedCheck();
            he1.RemovedCheck();

            // halfedges must be on the same face which can't be null
            if (he0.Face == null || he0.Face != he1.Face)
                return null;

            // halfedges can't be consecutive
            if (he0.NextInFace == he1 || he1.NextInFace == he0)
                return null;

            return SplitFaceImpl(he0, he1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        private TE SplitFaceImpl(TE he0, TE he1)
        {
            var f0 = he0.Face;
            var f1 = AddFace();

            var he2 = AddEdge(he0.Start, he1.Start);
            var he3 = he2.Twin;

            // set halfedge->face refs
            he3.Face = f0;
            he2.Face = f1;

            // set new halfedges as first in respective faces
            f0.First = he3;
            f1.First = he2;

            // update halfedge->halfedge refs
            he0.PrevInFace.MakeConsecutive(he2);
            he1.PrevInFace.MakeConsecutive(he3);
            he3.MakeConsecutive(he0);
            he2.MakeConsecutive(he1);

            // update face references of all halfedges in new loop
            var he = he2.NextInFace;
            do
            {
                he.Face = f1;
                he = he.NextInFace;
            } while (he != he2);

            return he2; // return halfedge adjacent to new face
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        /// <param name="mode"></param>
        public void TriangulateFace(TF face, TriangulateMode mode)
        {
            _faces.OwnsCheck(face);
            face.RemovedCheck();

            switch (mode)
            {
                case TriangulateMode.Fan:
                    TriangulateFaceFan(face.First);
                    break;
                case TriangulateMode.Strip:
                    TriangulateFaceStrip(face.First);
                    break;
                case TriangulateMode.Poke:
                    TriangulateFacePoke(face.First);
                    break;
            }
        }


        /// <summary>
        /// Starts the triangulation from the halfedge that returns the minimum value for the given selector function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="face"></param>
        /// <param name="mode"></param>
        /// <param name="selector"></param>
        public void TriangulateFace<T>(TF face, TriangulateMode mode, Func<TE, T> selector)
            where T : IComparable<T>
        {
            _faces.OwnsCheck(face);
            face.RemovedCheck();

            switch (mode)
            {
                case TriangulateMode.Fan:
                    TriangulateFaceFan(face.Halfedges.SelectMin(selector));
                    break;
                case TriangulateMode.Strip:
                    TriangulateFaceStrip(face.Halfedges.SelectMin(selector));
                    break;
                case TriangulateMode.Poke:
                    TriangulateFacePoke(face.First);
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        private void TriangulateFaceFan(TE first)
        {
            var he0 = first;
            var he1 = he0.NextInFace.NextInFace;

            while (he1.NextInFace != he0)
            {
                he0 = SplitFaceImpl(he0, he1);
                he1 = he1.NextInFace;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        private void TriangulateFaceStrip(TE first)
        {
            var he0 = first;
            var he1 = he0.NextInFace.NextInFace;

            while (he1.NextInFace != he0)
            {
                he0 = SplitFaceImpl(he0, he1).PrevInFace;
                if (he1.NextInFace == he0) break;

                he0 = SplitFaceImpl(he0, he1);
                he1 = he1.NextInFace;
            }
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        private TV TriangulateFacePoke(TE first)
        {
            var he = first;
            var v = first.Start;
            var f = first.Face;

            // add new vertex at face center
            var vc = AddVertex();

            // create new halfedges and connect to existing ones
            do
            {
                var he0 = AddEdge(he.Start, vc);
                he.PrevInFace.MakeConsecutive(he0);
                he0.Twin.MakeConsecutive(he);
                he = he.NextInFace;
            } while (he.Start != v);

            he = first; // reset to first halfedge in face
            vc.FirstOut = he.PrevInFace; // set outgoing halfedge for the central vertex

            // connect new halfedges and create new faces where necessary
            do
            {
                var he0 = he.PrevInFace;
                var he1 = he.NextInFace;
                he1.MakeConsecutive(he0);

                // create new face if necessary
                if (f == null)
                {
                    f = AddFace();
                    f.First = he;
                    he.Face = f;
                }

                // assign halfedge->face refs
                he0.Face = he1.Face = f;
                f = null;

                he = he1.Twin.NextInFace;
            } while (he.Start != v);

            return vc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        /// <param name="mode"></param>
        public void QuadrangulateFace(TF face, QuadrangulateMode mode)
        {
            _faces.OwnsCheck(face);
            face.RemovedCheck();

            switch (mode)
            {
                case QuadrangulateMode.Fan:
                    QuadrangulateFaceFan(face.First);
                    break;
                case QuadrangulateMode.Strip:
                    QuadrangulateFaceStrip(face.First);
                    break;
                case QuadrangulateMode.Poke:
                    QuadrangulateFacePoke(face.First);
                    break;
            }
        }


        /// <summary>
        /// Starts the triangulation from the halfedge that returns the minimum value for the given selector function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="face"></param>
        /// <param name="mode"></param>
        /// <param name="selector"></param>
        public void QuadrangulateFace<T>(TF face, QuadrangulateMode mode, Func<TE, T> selector)
            where T : IComparable<T>
        {
            _faces.OwnsCheck(face);
            face.RemovedCheck();

            switch (mode)
            {
                case QuadrangulateMode.Fan:
                    QuadrangulateFaceFan(face.Halfedges.SelectMin(selector));
                    break;
                case QuadrangulateMode.Strip:
                    QuadrangulateFaceStrip(face.Halfedges.SelectMin(selector));
                    break;
                case QuadrangulateMode.Poke:
                    QuadrangulateFacePoke(face.First);
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        private void QuadrangulateFaceFan(TE first)
        {
            var he0 = first;
            var he1 = he0.NextInFace.NextInFace.NextInFace;

            while (he1 != he0 && he1.NextInFace != he0)
            {
                he0 = SplitFaceImpl(he0, he1);
                he1 = he1.NextInFace.NextInFace;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        private void QuadrangulateFaceStrip(TE first)
        {
            var he0 = first;
            var he1 = he0.NextInFace.NextInFace.NextInFace;

            while (he1 != he0 && he1.NextInFace != he0)
            {
                he0 = SplitFaceImpl(he0, he1).PrevInFace;
                he1 = he1.NextInFace;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <returns></returns>
        private TV QuadrangulateFacePoke(TE first)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Reverses the winding direction of all faces in the mesh
        /// TODO move to face ops
        /// </summary>
        public void ReverseFaces()
        {
            foreach (var he in Halfedges)
            {
                var prev = he.PrevInFace;
                he.PrevInFace = he.NextInFace;
                he.NextInFace = prev;
            }
        }


        /// <summary>
        /// Orients each face such that the first halfedge returns the minimum value for the given selector function.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="parallel"></param>
        public void OrientFacesToMin<T>(Func<TE, T> selector, bool parallel = false)
            where T : IComparable<T>
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = _faces[i];
                    if (f.IsRemoved) continue;
                    f.First = f.Halfedges.SelectMin(selector);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, _faces.Count), func);
            else
                func(Tuple.Create(0, _faces.Count));
        }


        /// <summary>
        /// Orients each face such that the first halfedge returns the minimum value for the given selector function.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="parallel"></param>
        public void OrientFacesToMin(Func<TE, double> selector, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = _faces[i];
                    if (f.IsRemoved) continue;
                    f.First = f.Halfedges.SelectMin(selector);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, _faces.Count), func);
            else
                func(Tuple.Create(0, _faces.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        public void OrientFacesToBoundary(bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var f = _faces[i];
                    if (f.IsRemoved) continue;
                    f.SetFirstToBoundary();
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, _faces.Count), func);
            else
                func(Tuple.Create(0, _faces.Count));
        }


        /// <summary>
        /// Triangulates all faces in the mesh.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public void TriangulateFaces(TriangulateMode mode = TriangulateMode.Strip)
        {
            int nf = _faces.Count;

            for (int i = 0; i < nf; i++)
            {
                var f = _faces[i];
                if (!f.IsRemoved) TriangulateFace(f, mode);
            }
        }


        /// <summary>
        /// Triangulates all faces in the mesh.
        /// For each face, starts triangulation from the halfedge which returns the minimum value for the given selector function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="mode"></param>
        public void TriangulateFaces<T>(Func<TE, T> selector, TriangulateMode mode = TriangulateMode.Strip)
            where T : IComparable<T>
        {
            int nf = _faces.Count;

            for (int i = 0; i < nf; i++)
            {
                var f = _faces[i];
                if (!f.IsRemoved) TriangulateFace(f, mode, selector);
            }
        }


        /// <summary>
        /// Splits all n-gonal faces into quads (and tris where necessary).
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public void QuadrangulateFaces(QuadrangulateMode mode = QuadrangulateMode.Strip)
        {
            int nf = _faces.Count;

            for (int i = 0; i < nf; i++)
            {
                var f = _faces[i];
                if (!f.IsRemoved) QuadrangulateFace(f, mode);
            }
        }


        /// <summary>
        /// Splits all n-gonal faces into quads (and tris where necessary).
        /// For each face, starts quadrangulation from the halfedge which returns the minimum value for the given selector function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="mode"></param>
        public void QuadrangulateFaces<T>(Func<TE, T> selector, QuadrangulateMode mode = QuadrangulateMode.Strip)
            where T : IComparable<T>
        {
            int nf = _faces.Count;

            for (int i = 0; i < nf; i++)
            {
                var f = _faces[i];
                if (!f.IsRemoved) QuadrangulateFace(f, mode, selector);
            }
        }


        /// <summary>
        /// Turns all faces to create consistent directionality across the mesh where possible.
        /// Assumes quadrilateral faces.
        /// http://page.math.tu-berlin.de/~bobenko/MinimalCircle/minsurftalk.pdf
        /// </summary>
        public void UnifyFaceOrientationQuad(bool flip)
        {
            var stack = new Stack<TE>();
            int currTag = _faces.NextTag;

            for (int i = 0; i < _faces.Count; i++)
            {
                var f = _faces[i];
                if (f.IsRemoved || f.Tag == currTag) continue; // skip if unused or already visited

                // check flip
                if (flip) f.First = f.First.NextInFace;

                // flag as visited
                f.Tag = currTag;

                // add to stack
                stack.Push(f.First);
                UnifyFaceOrientationQuad(stack, currTag);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        public void UnifyFaceOrientationQuad(TF start)
        {
            _faces.OwnsCheck(start);
            start.RemovedCheck();

            var stack = new Stack<TE>();
            int currTag = _faces.NextTag;

            // flag as visited
            start.Tag = currTag;

            // add first halfedge to stack
            stack.Push(start.First);
            UnifyFaceOrientationQuad(stack, currTag);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UnifyFaceOrientationQuad(Stack<TE> stack, int currTag)
        {
            while (stack.Count > 0)
            {
                var he0 = stack.Pop();

                foreach (var he1 in AdjacentQuads(he0))
                {
                    var f1 = he1.Face;
                    if (f1 != null && f1.Tag != currTag)
                    {
                        f1.First = he1;
                        f1.Tag = currTag;
                        stack.Push(he1);
                    }
                }
            }

            IEnumerable<TE> AdjacentQuads(TE hedge)
            {
                yield return hedge.NextAtStart.NextInFace; // down
                yield return hedge.PrevInFace.PrevAtStart; // up
                yield return hedge.PrevAtStart.PrevInFace; // left
                yield return hedge.NextInFace.NextAtStart; // right
            }
        }


        #endregion


        #endregion


        #region Element Attributes


        #endregion
    }


    /// <summary>
    /// Static constructors and extension methods for generic and empty HeMeshes.
    /// </summary>
    public static class HeMesh
    {
        /// <summary></summary>
        public static readonly HeMeshFactory<V, E, F> Factory = HeMeshFactory.Create(() => new V(), () => new E(), () => new F());


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        /// <param name="faceProvider"></param>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        /// <returns></returns>
        public static HeMesh<TV,TE,TF> Create<TV, TE, TF>(Func<TV> vertexProvider, Func<TE> hedgeProvider, Func<TF> faceProvider, int vertexCapacity = 4, int hedgeCapacity = 4, int faceCapacity = 4)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            return new HeMesh<TV, TE, TF>(vertexProvider, hedgeProvider, faceProvider, vertexCapacity, hedgeCapacity, faceCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        /// <returns></returns>
        public static HeMesh<V, E, F> Create(int vertexCapacity = 4, int hedgeCapacity = 4, int faceCapacity = 4)
        {
            return Factory.Create(vertexCapacity, hedgeCapacity, faceCapacity);
        }
       

        /// <summary>
        /// Default empty vertex
        /// </summary>
        [Serializable]
        public class V : HeVertex<V, E, F> { }


        /// <summary>
        /// Default empty halfedge
        /// </summary>
        [Serializable]
        public class E : Halfedge<V, E, F> { }


        /// <summary>
        /// Default empty face
        /// </summary>
        [Serializable]
        public class F : HeFace<V, E, F> { }
    }
}


