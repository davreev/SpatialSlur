

/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Meshes.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TM"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    /// <typeparam name="TF"></typeparam>
    [Serializable]
    public abstract class HeMeshFactory<TM, TV, TE, TF>
        where TM : HeMesh<TV, TE, TF>
        where TV : HeMesh<TV, TE, TF>.Vertex
        where TE : HeMesh<TV, TE, TF>.Halfedge
        where TF : HeMesh<TV, TE, TF>.Face
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TM Create()
        {
            return Create(4, 4, 4);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        /// <returns></returns>
        public abstract TM Create(int vertexCapacity, int hedgeCapacity, int faceCapacity);


        /// <summary>
        /// Action delegates specify how attributes of original elements are mapped to attributes of copied elements.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        /// <returns></returns>
        public TM CreateCopy<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null, Action<TF, UF> setFace = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            var copy = Create(mesh.Vertices.Capacity, mesh.Halfedges.Capacity, mesh.Faces.Capacity);
            copy.Append(mesh, setVertex, setHedge, setFace);
            return copy;
        }


        /// <summary>
        /// Returns the dual of the given mesh.
        /// Action delegates specify how attributes of primal elements are mapped to attributes of dual elements.
        /// Note this method preserves indexical correspondance between primal and dual elements.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        /// <returns></returns>
        public TM CreateDual<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UF> setVertex = null, Action<TE, UE> setHedge = null, Action<TF, UV> setFace = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            var dual = Create(mesh.Vertices.Capacity, mesh.Halfedges.Capacity, mesh.Faces.Capacity);
            dual.AppendDual(mesh, setVertex, setHedge, setFace);
            return dual;
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
        /// <param name="setFace"></param>
        /// <returns></returns>
        public TM[] CreateConnectedComponents<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null, Action<TF, UF> setFace = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            return CreateConnectedComponents(mesh, out int[] compIds, out int[] edgeIds, setVertex, setHedge, setFace);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        /// <returns></returns>
        public TM[] CreateConnectedComponents<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, out int[] componentIndices, out int[] edgeIndices, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null, Action<TF, UF> setFace = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            int ne = mesh.Edges.Count;
            componentIndices = new int[ne];
            edgeIndices = new int[ne];

            return CreateConnectedComponents(
                mesh,
                HeMesh<UV, UE, UF>.Edge.CreateProperty(componentIndices),
                HeMesh<UV, UE, UF>.Edge.CreateProperty(edgeIndices), 
                setVertex, 
                setHedge, 
                setFace
                );
        }


        /// <summary>
        /// Action delegates specify how attributes of parent elements are mapped to attributes of component elements.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        /// <returns></returns>
        public TM[] CreateConnectedComponents<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Property<UE, int> componentIndex, Property<UE, int> edgeIndex, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null, Action<TF, UF> setFace = null)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            var vertices = mesh.Vertices;
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;

            int ncomp = mesh.GetEdgeComponentIndices(componentIndex.Set);
            var comps = new TM[ncomp];

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
                var heA1 = heA0.Next;

                // set refs
                var heB0 = compHedges[(edgeIndex.Get(heA0) << 1) + (i & 1)];
                var heB1 = compHedges[(edgeIndex.Get(heA1) << 1) + (heA1.Index & 1)];
                heB0.MakeConsecutive(heB1);

                // transfer attributes
                setHedge?.Invoke(heB0, heA0);
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
                setVertex?.Invoke(vB, vA);
            }

            // create component faces
            for (int i = 0; i < faces.Count; i++)
            {
                var fA = faces[i];
                if (fA.IsUnused) continue;

                var heA = fA.First;
                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.Halfedges[(edgeIndex.Get(heA) << 1) + (heA.Index & 1)];

                // set face refs
                var fB = comp.AddFace();
                fB.First = heB;

                foreach (var he in heB.Circulate)
                    he.Face = fB;

                // transfer attributes
                setFace?.Invoke(fB, fA);
            }

            return comps;
        }


        /// <summary>
        /// Assumes the halfedges of the given graph are correctly sorted around each vertex.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <param name="graph"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <returns></returns>
        public TM CreateFromGraph<UV, UE>(HeGraph<UV, UE> graph, Action<TV, UV> setVertex = null, Action<TE, UE> setHedge = null)
            where UV : HeGraph<UV, UE>.Vertex
            where UE : HeGraph<UV, UE>.Halfedge
        {
            // TODO implement
            throw new NotImplementedException();
        }


        /// <summary>
        /// Creates a new mesh from polygon soup.
        /// Note that this method assumes consistent faces windings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="polygons"></param>
        /// <param name="setPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public TM CreateFromPolygons<T>(IEnumerable<T> polygons, Action<TV, Vector3d> setPosition, double tolerance = D.ZeroTolerance)
            where T : IEnumerable<Vector3d>
        {
            var vertices = polygons.SelectMany(p => p).RemoveCoincident(out List<int> indexMap, tolerance);
            var faces = polygons.Select(p => p.Count());

            return CreateFromFaceVertexData(vertices, indexMap.Batch(faces), setPosition);
        }


        /// <summary>
        /// Creates a new mesh from face-vertex information.
        /// Note that this method assumes consistent faces windings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vertices"></param>
        /// <param name="faces"></param>
        /// <param name="setPosition"></param>
        /// <returns></returns>
        public TM CreateFromFaceVertexData<T>(IEnumerable<Vector3d> vertices, IEnumerable<T> faces, Action<TV, Vector3d> setPosition)
            where T : IEnumerable<int>
        {
            var result = Create();

            // add vertices
            foreach (var p in vertices)
            {
                var v = result.AddVertex();
                setPosition(v, p);
            }

            // add faces
            foreach (var f in faces)
                result.AddFace(f);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="setPosition"></param>
        /// <returns></returns>
        public TM CreateFromObj(string path, Action<TV, Vector3d> setPosition)
        {
            var result = Create();
            Interop.Meshes.ReadFromObj(path, result, setPosition);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="setVertexAttributes"></param>
        /// <param name="setHedgeAttributes"></param>
        /// <param name="setFaceAttributes"></param>
        /// <returns></returns>
        public TM CreateFromJson(string path, Action<TV, object[]> setVertexAttributes = null, Action<TE, object[]> setHedgeAttributes = null, Action<TF, object[]> setFaceAttributes = null)
        {
            var result = Create();
            Interop.Meshes.ReadFromJson(path, result, setVertexAttributes, setHedgeAttributes, setFaceAttributes);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TM CreateTetrahedron(Action<TV, Vector3d> setPosition, Action<TV, Vector3d> setNormal)
        {
            var result = CreateTetrahedron();
            var verts = result.Vertices;

            var n = new Vector3d(1.0);
            n.Unitize();

            var v = verts[0];
            setPosition(v, new Vector3d(-0.5, -0.5, -0.5));
            setNormal(v, new Vector3d(-n.X, -n.Y, -n.Z));

            v = verts[1];
            setPosition(v, new Vector3d(0.5, 0.5, -0.5));
            setNormal(v, new Vector3d(n.X, n.Y, -n.Z));

            v = verts[2];
            setPosition(v, new Vector3d(0.5, -0.5, 0.5));
            setNormal(v, new Vector3d(n.X, -n.Y, n.Z));

            v = verts[3];
            setPosition(v, new Vector3d(-0.5, 0.5, 0.5));
            setNormal(v, new Vector3d(-n.X, n.Y, n.Z));

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private TM CreateTetrahedron()
        {
            var result = Create(4, 12, 4);

            var verts = result.Vertices;
            var hedges = result.Halfedges;
            var faces = result.Faces;
            
            for (int i = 0; i < 4; i++)
                result.AddVertex();

            for (int i = 0; i < 6; i++)
                result.AddEdge();

            for (int i = 0; i < 4; i++)
                result.AddFace();

            verts[0].First = hedges[0];
            verts[1].First = hedges[2];
            verts[2].First = hedges[4];
            verts[3].First = hedges[6];

            faces[0].First = hedges[0];
            faces[1].First = hedges[3];
            faces[2].First = hedges[4];
            faces[3].First = hedges[7];

            SetHedge(hedges[0], 9, 2, 0, 0);
            SetHedge(hedges[1], 10, 7, 1, 3);

            SetHedge(hedges[2], 0, 9, 1, 0);
            SetHedge(hedges[3], 5, 11, 2, 1);

            SetHedge(hedges[4], 8, 6, 2, 2);
            SetHedge(hedges[5], 11, 3, 3, 1);
    
            SetHedge(hedges[6], 4, 8, 3, 2);
            SetHedge(hedges[7], 1, 10, 0, 3);
            
            SetHedge(hedges[8], 4, 6, 0, 2);
            SetHedge(hedges[9], 2, 0, 2, 0);
            
            SetHedge(hedges[10], 7, 1, 3, 3);
            SetHedge(hedges[11], 3, 5, 1, 1);

            void SetHedge(TE hedge, int prev, int next, int start, int face)
            {
                hedge.Previous = hedges[prev];
                hedge.Next = hedges[next];
                hedge.Start = verts[start];
                hedge.Face = faces[face];
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapX"></param>
        /// <param name="wrapY"></param>
        /// <returns></returns>
        public TM CreateGrid(int countX, int countY, bool wrapX = false, bool wrapY = false)
        {
            throw new NotImplementedException();
        }


        #region WIP

        /// <summary>
        /// If using external buffers to store vertex attributes, the number of vertices in the resulting mesh equals 8 times the number of edges in the given mesh.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getPosition"></param>
        /// <param name="getScale"></param>
        /// <param name="getNormal"></param>
        /// <param name="getCenter"></param>
        /// <param name="setPosition"></param>
        /// <returns></returns>
        public TM CreateWeave<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Func<UV, Vector3d> getPosition, Func<UE, double> getScale, Func<UE, Vector3d> getNormal, Func<UF, Vector3d> getCenter, Action<TV, Vector3d> setPosition)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            int ne = mesh.Edges.Count;
            var result = Create(ne << 3, ne << 4, ne << 3);

            CreateWeaveGeometry(mesh, result, getPosition, getScale, getNormal, getCenter, setPosition);
            CreateWeaveTopology(mesh, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateWeaveGeometry<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, TM newMesh, Func<UV, Vector3d> getPosition, Func<UE, double> getScale, Func<UE, Vector3d> getNormal, Func<UF, Vector3d> getCenter, Action<TV, Vector3d> setPosition)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            var edges = mesh.Edges;
            int ne = edges.Count;

            // bulk add new vertices
            newMesh.AddVertices(ne << 3);
            var newVerts = newMesh.Vertices;

            // add vertices (8 per halfedge pair in m0)
            for (int i = 0; i < ne; i++)
            {
                var he0 = edges[i];
                var he1 = he0.Twin;

                var f0 = he0.Face;
                var f1 = he1.Face;

                // scale points to mid point of edge
                Vector3d p0 = getPosition(he0.Start);
                Vector3d p1 = getPosition(he1.Start);

                Vector3d p = (p0 + p1) * 0.5;
                var t = getScale(he0);

                p0 = Vector3d.Lerp(p, p0, t);
                p1 = Vector3d.Lerp(p, p1, t);
                Vector3d p2 = (f0 == null) ? new Vector3d() : Vector3d.Lerp(p, getCenter(f0), t);
                Vector3d p3 = (f1 == null) ? new Vector3d() : Vector3d.Lerp(p, getCenter(f1), t);

                // set vertex positions
                Vector3d d = he0.IsBoundary ? Vector3d.Zero : getNormal(he0);
                int j = i << 3;

                setPosition(newVerts[j], p0 - d);
                setPosition(newVerts[j + 1], p2 - d);
                setPosition(newVerts[j + 2], p0 + d);
                setPosition(newVerts[j + 3], p2 + d);

                setPosition(newVerts[j + 4], p1 - d);
                setPosition(newVerts[j + 5], p3 - d);
                setPosition(newVerts[j + 6], p1 + d);
                setPosition(newVerts[j + 7], p3 + d);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateWeaveTopology<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, TM newMesh)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            var hedges = mesh.Halfedges;
            int nhe = hedges.Count;

            // add node faces
            for (int i = 0; i < nhe; i += 2)
            {
                var he0 = hedges[i];
                var he1 = hedges[i + 1];
                int j = i << 2;

                if (he0.Face == null)
                {
                    newMesh.AddFace(j, j + 5, j + 4);
                }
                else if (he1.Face == null)
                {
                    newMesh.AddFace(j + 4, j + 1, j);
                }
                else
                {
                    newMesh.AddFace(j + 5, j + 4, j + 1, j);
                    newMesh.AddFace(j + 7, j + 6, j + 3, j + 2);
                }
            }

            // add edge faces
            for (int i = 0; i < nhe; i++)
            {
                var he0 = hedges[i];
                if (he0.Face == null) continue;

                int j0 = i << 2;
                int j1 = he0.Next.Index << 2;

                if (he0.IsBoundary)
                {
                    if ((i & 1) == 0)
                        newMesh.AddFace(j0 + 1, j0 + 4, j1, j1 + 1); // even edges
                    else
                        newMesh.AddFace(j0 + 1, j0 - 4, j1, j1 + 1); // odd edges
                }
                else
                {
                    if ((i & 1) == 0)
                        newMesh.AddFace(j0 + 3, j0 + 6, j1, j1 + 1);  // even edges
                    else
                        newMesh.AddFace(j0 + 3, j0 - 2, j1, j1 + 1); // odd edges
                }
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        private static void CreateWeaveGeometry<V, E, F>(HeMesh<V, E, F> mesh, HeMesh<V, E, F> newMesh, Func<E, double> getScale, Func<E, Vec3d> getNormal, Func<F, Vec3d> getCenter, Action<V, V, double, V> lerp, Action<IEnumerable<V>, V> mean, Action<V, Vec3d> translate)
            where V : HeMeshBase<V, E, F>.Vertex
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            // TODO generalized implementation that handles interpolation of arbitrary vertex attributes

            var edges = mesh.Edges;
            int ne = edges.Count;

            // bulk add new vertices
            newMesh.AddVertices(ne << 3);
            var newVerts = newMesh.Vertices;

            // temp verts to store interpolated attributes
            var ev = mesh.ElementProvider.NewVertex();
            var fv0 = mesh.ElementProvider.NewVertex();
            var fv1 = mesh.ElementProvider.NewVertex();

            // add vertices (8 per halfedge pair in m0)
            for (int i = 0; i < ne; i++)
            {
                var he0 = edges[i];
                var he1 = he0.Twin;

                var f0 = he0.Face;
                var f1 = he1.Face;

                var v0 = he0.Start;
                var v1 = he1.Start;

                // vertex at middle of edge
                lerp(v0, v1, 0.5, ev);

                // scale points to mid point of edge
 
                var t = getScale(he0);

                p0 = Vec3d.Lerp(p, p0, t);
                p1 = Vec3d.Lerp(p, p1, t);
                Vec3d p2 = (f0 == null) ? new Vec3d() : Vec3d.Lerp(p, getCenter(f0), t);
                Vec3d p3 = (f1 == null) ? new Vec3d() : Vec3d.Lerp(p, getCenter(f1), t);

                // set vertex positions
                Vec3d d = he0.IsBoundary ? Vec3d.Zero : getNormal(he0);
                int j = i << 3;

                position.Set(newVerts[j], p0 - d);
                position.Set(newVerts[j + 1], p2 - d);
                position.Set(newVerts[j + 2], p0 + d);
                position.Set(newVerts[j + 3], p2 + d);

                position.Set(newVerts[j + 4], p1 - d);
                position.Set(newVerts[j + 5], p3 - d);
                position.Set(newVerts[j + 6], p1 + d);
                position.Set(newVerts[j + 7], p3 + d);
            }
        }
        */


        /// <summary>
        /// If using external buffers to store vertex attributes, the number of vertices in the resulting mesh equals the number of halfedges in the given mesh.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getPosition"></param>
        /// <param name="getScale"></param>
        /// <param name="getCenter"></param>
        /// <param name="setPosition"></param>
        /// <returns></returns>
        public TM CreateBevelledDual<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Func<UV, Vector3d> getPosition, Func<UV, double> getScale, Func<UF, Vector3d> getCenter, Action<TV, Vector3d> setPosition)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            // TODO implement
            throw new NotImplementedException();

            int ne = mesh.Edges.Count;
            var result = Create(ne << 3, ne << 4, ne << 3);

            //CreateWeaveGeometry(mesh, result, getPosition, getScale, getNormal, getCenter, setPosition);
            //CreateWeaveTopology(mesh, result);
            return result;
        }


        /// <summary>
        /// If using external buffers to store vertex attributes, the number of vertices in the resulting mesh the equals the sum of the number of vertices and halfedges in the given mesh.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getPosition"></param>
        /// <param name="getScale"></param>
        /// <param name="getCenter"></param>
        /// <param name="setPosition"></param>
        /// <returns></returns>
        public TM CreateFramedDual<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Func<UV, Vector3d> getPosition, Func<UV, double> getScale, Func<UF, Vector3d> getCenter, Action<TV, Vector3d> setPosition)
            where UV : HeMesh<UV, UE, UF>.Vertex
            where UE : HeMesh<UV, UE, UF>.Halfedge
            where UF : HeMesh<UV, UE, UF>.Face
        {
            // TODO implement
            throw new NotImplementedException();

            int ne = mesh.Edges.Count;
            var result = Create(ne << 3, ne << 4, ne << 3);

            //CreateWeaveGeometry(mesh, result, getPosition, getScale, getNormal, getCenter, setPosition);
            //CreateWeaveTopology(mesh, result);
            return result;
        }

        #endregion
    }
}
