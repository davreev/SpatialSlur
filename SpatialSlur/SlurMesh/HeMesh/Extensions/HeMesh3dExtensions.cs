using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

using static SpatialSlur.SlurMesh.HeMesh3d;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class HeMeshExtensions
    {
        /// <summary> </summary>
        private static Func<IVertex3d, Vec3d> _getPosition = v => v.Position;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static HeMesh<V, E, F> Duplicate(this HeMesh<V, E, F> mesh)
        {
            return mesh.Duplicate(Set, Set, Set);
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
        public static HeMesh<V, E, F> GetDual(this HeMesh<V, E, F> mesh)
        {
            return mesh.GetDual(Set, Set, Set);
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
        /// <param name="setHandle"></param>
        /// <returns></returns>
        public static HeMesh<V, E, F>[] SplitDisjoint(this HeMesh<V, E, F> mesh, Func<E, ElementHandle> getHandle, Action<E, ElementHandle> setHandle)
        {
            return mesh.SplitDisjoint(getHandle, setHandle, Set, Set, Set);
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

        
        /// <summary> </summary>
        private static void Set(V v0, V v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary> </summary>
        private static void Set(E he0, E he1)
        {
            he0.Weight = he1.Weight;
        }


        /// <summary> </summary>
        private static void Set(F f0, F f1)
        {
            f0.Normal = f1.Normal;
        }


        /// <summary> </summary>
        private static void Set(V v, F f)
        {
            if (!f.IsRemoved) v.Position = f.Vertices.Mean(_getPosition);
            v.Normal = f.Normal;
        }


        /// <summary> </summary>
        private static void Set(F f, V v)
        {
            f.Normal = v.Normal;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static partial class HeMeshFactoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static HeMesh<V, E, F> CreateFromFaceVertexData<T>(this HeMeshFactory<V,E,F> factory, IEnumerable<Vec3d> vertices, IEnumerable<T> faces)
            where T : IEnumerable<int>
        {
            return factory.CreateFromFaceVertexData(vertices, faces, (v, p) => v.Position = p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static HeMesh<V, E, F> CreateFromPolygons<T>(this HeMeshFactory<V, E, F> factory, IEnumerable<T> polygons, double tolerance = 1.0e-8)
            where T : IEnumerable<Vec3d>
        {
            return factory.CreateFromPolygons(polygons, (v, p) => v.Position = p, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static HeMesh<V, E, F> CreateFromOBJ(this HeMeshFactory<V, E, F> factory, string path)
        {
            return factory.CreateFromOBJ(path, (v, p) => v.Position = p);
        }
    }
}
