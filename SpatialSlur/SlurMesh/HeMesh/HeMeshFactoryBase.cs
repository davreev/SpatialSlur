using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <typeparam name="TM"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    /// <typeparam name="TF"></typeparam>
    [Serializable]
    public abstract class HeMeshFactoryBase<TM, TV, TE, TF> : IFactory<TM>
        where TM : HeMeshBase<TV, TE, TF>
        where TV : HeVertex<TV, TE, TF>
        where TE : Halfedge<TV, TE, TF>
        where TF : HeFace<TV, TE, TF>
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
        public TM CreateCopy<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Action<TF, UF> setFace)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
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
        public TM CreateDual<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UF> setVertex, Action<TE, UE> setHedge, Action<TF, UV> setFace)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
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
        public TM[] CreateConnectedComponents<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Action<TF, UF> setFace)
          where UV : HeVertex<UV, UE, UF>
          where UE : Halfedge<UV, UE, UF>
          where UF : HeFace<UV, UE, UF>
        {
            return CreateConnectedComponents(mesh, setVertex, setHedge, setFace, out int[] compIds, out int[] edgeIds);

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
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public TM[] CreateConnectedComponents<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Action<TF, UF> setFace, out int[] componentIndices, out int[] edgeIndices)
          where UV : HeVertex<UV, UE, UF>
          where UE : Halfedge<UV, UE, UF>
          where UF : HeFace<UV, UE, UF>
        {
            int ne = mesh.Edges.Count;
            componentIndices = new int[ne];
            edgeIndices = new int[ne];

            return CreateConnectedComponents(mesh, setVertex, setHedge, setFace, ToProp(componentIndices), ToProp(edgeIndices));

            Property<UE, T> ToProp<T>(T[] values)
            {
                return Property.Create<UE, T>(he => values[he >> 1], (he, i) => values[he >> 1] = i);
            }
        }


        /// <summary>
        /// Action delegates specify how attributes of parent elements are mapped to attributes of component elements.
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public TM[] CreateConnectedComponents<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Action<TF, UF> setFace, Property<UE, int> componentIndex, Property<UE, int> edgeIndex)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
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
                if (heA.IsRemoved) continue;

                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.AddEdge();
                edgeIndex.Set(heA, heB.Index >> 1);
            }

            // set component halfedge->halfedge refs
            for (int i = 0; i < hedges.Count; i++)
            {
                var heA0 = hedges[i];
                if (heA0.IsRemoved) continue;

                // the component to which heA0 was copied
                var compHedges = comps[componentIndex.Get(heA0)].Halfedges;
                var heA1 = heA0.NextInFace;

                // set refs
                var heB0 = compHedges[(edgeIndex.Get(heA0) << 1) + (i & 1)];
                var heB1 = compHedges[(edgeIndex.Get(heA1) << 1) + (heA1.Index & 1)];
                heB0.MakeConsecutive(heB1);
                setHedge(heB0, heA0);
            }

            // create component vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                var vA = vertices[i];
                if (vA.IsRemoved) continue;

                var heA = vA.FirstOut;
                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.Halfedges[(edgeIndex.Get(heA) << 1) + (heA.Index & 1)];

                // set vertex refs
                var vB = comp.AddVertex();
                vB.FirstOut = heB;

                foreach (var he in heB.CirculateStart)
                    he.Start = vB;

                setVertex(vB, vA);
            }

            // create component faces
            for (int i = 0; i < faces.Count; i++)
            {
                var fA = faces[i];
                if (fA.IsRemoved) continue;

                var heA = fA.First;
                var comp = comps[componentIndex.Get(heA)];
                var heB = comp.Halfedges[(edgeIndex.Get(heA) << 1) + (heA.Index & 1)];

                // set face refs
                var fB = comp.AddFace();
                fB.First = heB;

                foreach (var he in heB.CirculateFace)
                    he.Face = fB;

                setFace(fB, fA);
            }

            return comps;
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
        public TM CreateFromPolygons<T>(IEnumerable<T> polygons, Action<TV, Vec3d> setPosition, double tolerance = 1.0e-8)
        where T : IEnumerable<Vec3d>
        {
            List<Vec3d> points = new List<Vec3d>();
            List<int> sizes = new List<int>();

            // get all polygon points
            foreach (var poly in polygons)
            {
                int n = points.Count;
                points.AddRange(poly);
                sizes.Add(points.Count - n);
            }

            var vertPos = points.RemoveCoincident(out int[] indexMap, tolerance);
            return CreateFromFaceVertexData(vertPos, indexMap.Segment(sizes), setPosition);
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
        public TM CreateFromFaceVertexData<T>(IEnumerable<Vec3d> vertices, IEnumerable<T> faces, Action<TV, Vec3d> setPosition)
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
        public TM CreateFromOBJ(string path, Action<TV, Vec3d> setPosition)
        {
            var result = Create();
            HeMeshIO.ReadFromOBJ(path, result, setPosition);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="setPosition"></param>
        /// <param name="setNormal"></param>
        /// <returns></returns>
        public TM CreatePrimitive(PrimitiveType primitive, Action<TV, Vec3d> setPosition, Action<TV, Vec3d> setNormal)
        {
            throw new NotImplementedException();
        }


        #region Weaving

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
        public TM CreateWeave<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Func<UV, Vec3d> getPosition, Func<UE, double> getScale, Func<UE, Vec3d> getNormal, Func<UF, Vec3d> getCenter, Action<TV, Vec3d> setPosition)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
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
        private void CreateWeaveGeometry<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, TM newMesh, Func<UV, Vec3d> getPosition, Func<UE, double> getScale, Func<UE, Vec3d> getNormal, Func<UF, Vec3d> getCenter, Action<TV, Vec3d> setPosition)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
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
                Vec3d p0 = getPosition(he0.Start);
                Vec3d p1 = getPosition(he1.Start);

                Vec3d p = (p0 + p1) * 0.5;
                var t = getScale(he0);

                p0 = Vec3d.Lerp(p, p0, t);
                p1 = Vec3d.Lerp(p, p1, t);
                Vec3d p2 = (f0 == null) ? new Vec3d() : Vec3d.Lerp(p, getCenter(f0), t);
                Vec3d p3 = (f1 == null) ? new Vec3d() : Vec3d.Lerp(p, getCenter(f1), t);

                // set vertex positions
                Vec3d d = he0.IsBoundary ? Vec3d.Zero : getNormal(he0);
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
        private void CreateWeaveTopology<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, TM newMesh)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
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
                int j1 = he0.NextInFace.Index << 2;

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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            // TODO implement generalized version which accepts interpolation of arbitrary vertex attributes

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

        #endregion


        #region Thickening

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
        public TM CreateBevelledDual<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Func<UV, Vec3d> getPosition, Func<UV, double> getScale, Func<UF, Vec3d> getCenter, Action<TV, Vec3d> setPosition)
        where UV : HeVertex<UV, UE, UF>
        where UE : Halfedge<UV, UE, UF>
        where UF : HeFace<UV, UE, UF>
        {
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
        public TM CreateFramedDual<UV, UE, UF>(HeMeshBase<UV, UE, UF> mesh, Func<UV, Vec3d> getPosition, Func<UV, double> getScale, Func<UF, Vec3d> getCenter, Action<TV, Vec3d> setPosition)
        where UV : HeVertex<UV, UE, UF>
        where UE : Halfedge<UV, UE, UF>
        where UF : HeFace<UV, UE, UF>
        {
            int ne = mesh.Edges.Count;
            var result = Create(ne << 3, ne << 4, ne << 3);

            //CreateWeaveGeometry(mesh, result, getPosition, getScale, getNormal, getCenter, setPosition);
            //CreateWeaveTopology(mesh, result);
            return result;
        }

        #endregion

    }
}
