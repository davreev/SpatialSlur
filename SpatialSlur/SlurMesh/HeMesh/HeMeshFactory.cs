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
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        /// <param name="faceProvider"></param>
        /// <returns></returns>
        public static HeMeshFactory<TV, TE, TF> Create<TV,TE,TF>(Func<TV> vertexProvider, Func<TE> hedgeProvider, Func<TF> faceProvider)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            return new HeMeshFactory<TV, TE, TF>(vertexProvider, hedgeProvider, faceProvider);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMeshFactory<TV, TE, TF> Create<TV, TE, TF>(HeMesh<TV,TE,TF> mesh)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            return new HeMeshFactory<TV, TE, TF>(mesh.VertexProvider, mesh.HalfedgeProvider, mesh.FaceProvider);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    /// <typeparam name="TF"></typeparam>
    public class HeMeshFactory<TV, TE, TF> : IHeStructureFactory<HeMesh<TV, TE, TF>, TV, TE, TF>
        where TV : HeVertex<TV, TE, TF>
        where TE : Halfedge<TV, TE, TF>
        where TF : HeFace<TV, TE, TF>
    {
        private Func<TV> _newTV;
        private Func<TE> _newTE;
        private Func<TF> _newTF;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        /// <param name="faceProvider"></param>
        public HeMeshFactory(Func<TV> vertexProvider, Func<TE> hedgeProvider, Func<TF> faceProvider)
        {
            _newTV = vertexProvider ?? throw new ArgumentNullException();
            _newTE = hedgeProvider ?? throw new ArgumentNullException();
            _newTF = faceProvider ?? throw new ArgumentNullException();
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
            return new HeMesh<TV, TE, TF>(_newTV, _newTE, _newTF, vertexCapacity, hedgeCapacity, faceCapacity);
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
        /// 
        /// </summary>
        /// <typeparam name="UV"></typeparam>
        /// <typeparam name="UE"></typeparam>
        /// <typeparam name="UF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getHandle"></param>
        /// <param name="setHandle"></param>
        /// <param name="setVertex"></param>
        /// <param name="setHedge"></param>
        /// <param name="setFace"></param>
        /// <returns></returns>
        public HeMesh<TV, TE, TF>[] CreateConnectedComponents<UV, UE, UF>(HeMesh<UV, UE, UF> mesh, Func<UE, ElementHandle> getHandle, Action<UE, ElementHandle> setHandle, Action<TV, UV> setVertex, Action<TE, UE> setHedge, Action<TF, UF> setFace)
            where UV : HeVertex<UV, UE, UF>
            where UE : Halfedge<UV, UE, UF>
            where UF : HeFace<UV, UE, UF>
        {
           return mesh.SplitDisjoint(this, getHandle, setHandle, setVertex, setHedge, setFace);
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

            var vertPos = points.RemoveDuplicates(out int[] indexMap, tolerance);
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
