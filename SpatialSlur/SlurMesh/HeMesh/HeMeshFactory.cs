using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * 
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeMeshFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static HeMeshFactory<TV, TE, TF> Create<TV, TE, TF>(HeElementProvider<TV, TE, TF> provider)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            return new HeMeshFactory<TV, TE, TF>(provider);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    /// <typeparam name="TF"></typeparam>
    [Serializable]
    public class HeMeshFactory<TV, TE, TF> : IFactory<HeMesh<TV,TE,TF>>
        where TV : HeVertex<TV, TE, TF>
        where TE : Halfedge<TV, TE, TF>
        where TF : HeFace<TV, TE, TF>
    {
        private HeElementProvider<TV, TE, TF> _provider;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public HeMeshFactory(HeElementProvider<TV, TE, TF> provider)
        {
            _provider = provider ?? throw new ArgumentNullException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh<TV, TE, TF> Create()
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
        public HeMesh<TV, TE, TF> Create(int vertexCapacity, int hedgeCapacity, int faceCapacity)
        {
            return new HeMesh<TV, TE, TF>(_provider, vertexCapacity, hedgeCapacity, faceCapacity);
        }


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
        public HeMesh<TV, TE, TF> CreateCopy<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Action<TF, UF> setFace)
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
        public HeMesh<TV, TE, TF> CreateDual<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UF> setVertex, Action<TE, UE> setHedge, Action<TF, UV> setFace)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
            var dual = Create(mesh.Vertices.Capacity, mesh.Halfedges.Capacity, mesh.Faces.Capacity);
            dual.AppendDual(mesh, setVertex, setHedge, setFace);
            return dual;
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
        public HeMesh<TV, TE, TF>[] CreateConnectedComponents<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Action<TF, UF> setFace, Property<UE, int> componentIndex, Property<UE, int> edgeIndex)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
           return mesh.SplitDisjoint(this, setVertex, setHedge, setFace, componentIndex, edgeIndex);
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
        public HeMesh<TV, TE, TF> CreateFromPolygons<T>(IEnumerable<T> polygons, Action<TV, Vec3d> setPosition, double tolerance = 1.0e-8)
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
        public HeMesh<TV, TE, TF> CreateFromFaceVertexData<T>(IEnumerable<Vec3d> vertices, IEnumerable<T> faces, Action<TV, Vec3d> setPosition)
            where T : IEnumerable<int>
        {
            var result = Create();

            // add vertices
            foreach(var p in vertices)
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
        public HeMesh<TV, TE, TF> CreateFromOBJ(string path, Action<TV, Vec3d> setPosition)
        {
            var result = Create();
            HeMeshIO.ReadOBJ(path, result, setPosition);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="primitive"></param>
        /// <param name="setPosition"></param>
        /// <param name="setNormal"></param>
        /// <returns></returns>
        public HeMesh<TV,TE,TF> CreatePrimitive(PrimitiveType primitive, Action<TV, Vec3d> setPosition, Action<TV, Vec3d> setNormal)
        {
            throw new NotImplementedException();
        }
    }
}
