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
    /// Static constructors and extension methods for an HeMesh with commonly used geometric properites.
    /// </summary>
    public static class HeMesh3d
    {
        /// <summary></summary>
        public static readonly HeMeshFactory<V, E, F> Factory = HeMeshFactory.Create(() => new V(), () => new E(), () => new F());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static HeMesh<V, E, F> Duplicate(this HeMesh<V, E, F> mesh)
        {
            var copy = Factory.Create(mesh.Vertices.Capacity, mesh.Halfedges.Capacity, mesh.Faces.Capacity);
            copy.Append(mesh);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="other"></param>
        public static void Append(this HeMesh<V, E, F> mesh, HeMesh<V, E, F> other)
        {
            mesh.Append(other, Set, Set, Set);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh<V,E,F> GetDual(this HeMesh<V,E,F> mesh)
        {
            var dual = Factory.Create(mesh.Faces.Capacity, mesh.Halfedges.Capacity, mesh.Vertices.Capacity);
            dual.AppendDual(mesh);
            return dual;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="other"></param>
        public static void AppendDual(this HeMesh<V, E, F> mesh, HeMesh<V, E, F> other)
        {
            mesh.AppendDual(other, Set, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="getHandle"></param>
        public static HeMesh<V, E, F>[] SplitDisjoint(this HeMesh<V, E, F> mesh, Func<E, SplitDisjointHandle> getHandle)
        {
            return mesh.SplitDisjoint(Factory, getHandle, Set, Set, Set);
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
        /// 
        /// </summary>
        /// <returns></returns>
        public static HeMesh<V, E, F> CreateFromFaceVertexData<T>(IEnumerable<Vec3d> vertices, IEnumerable<T> faces)
            where T : IEnumerable<int>
        {
            return Factory.CreateFromFaceVertexData(vertices, faces, (v, p) => v.Position = p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static HeMesh<V, E, F> CreateFromPolygons<T>(IEnumerable<T> polygons, double tolerance = 1.0e-8)
            where T : IEnumerable<Vec3d>
        {
            return Factory.CreateFromPolygons(polygons, (v, p) => v.Position = p, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static HeMesh<V, E, F> CreateFromOBJ(string path)
        {
            return Factory.CreateFromOBJ(path, (v, p) => v.Position = p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="path"></param>
        public static void WriteToOBJ(this HeMesh<V, E, F> mesh, string path)
        {
            HeMeshIO.WriteOBJ(mesh, path, v => v.Position, v => v.Normal, v => v.TexCoord);
        }


        /// <summary>
        /// 
        /// </summary>
        public class V : HeVertex<V, E, F>, IVertex3d
        {
            /// <summary></summary>
            public Vec3d Position { get; set; }
            /// <summary></summary>
            public Vec3d Normal { get; set; }
            /// <summary></summary>
            public Vec2d TexCoord { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        public class E : Halfedge<V, E, F>
        {
            /// <summary></summary>
            public double Weight;
        }


        /// <summary>
        ///
        /// </summary>
        public class F : HeFace<V, E, F>
        {
            /// <summary></summary>
            public Vec3d Normal;
        }


        #region Default Setters


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        private static void Set(V v0, V v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
            v0.TexCoord = v1.TexCoord;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        private static void Set(E he0, E he1)
        {
            he0.Weight = he1.Weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        private static void Set(F f0, F f1)
        {
            f0.Normal = f1.Normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="f"></param>
        private static void Set(V v, F f)
        {
            v.Position = f.GetBarycenter();
            v.Normal = f.Normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="f"></param>
        private static void Set(F f, V v)
        {
            f.Normal = v.Normal;
        }


        #endregion
    }
}
