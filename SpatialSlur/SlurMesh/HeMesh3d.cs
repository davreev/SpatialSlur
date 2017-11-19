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
    using M = HeMesh3d;
    using V = HeMesh3d.Vertex;
    using E = HeMesh3d.Halfedge;
    using F = HeMesh3d.Face;

    /// <summary>
    /// Implementation with double precision vertex attributes commonly used in 3d applications.
    /// </summary>
    [Serializable]
    public class HeMesh3d : HeMeshBase<V, E, F>
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeMeshBase<V, E, F>.Vertex, IVertex3d
        {
            #region Static

            /// <summary></summary>
            public static readonly Func<V, Vec3d> GetPosition = v => v.Position;
            /// <summary></summary>
            public static readonly Func<V, Vec3d> GetNormal = v => v.Normal;
            /// <summary></summary>
            public static readonly Func<V, Vec2d> GetTexture = v => v.Texture;

            #endregion


            /// <summary></summary>
            public Vec3d Position { get; set; }
            /// <summary></summary>
            public Vec3d Normal { get; set; }
            /// <summary></summary>
            public Vec2d Texture { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Halfedge : HeMeshBase<V, E, F>.Halfedge
        {
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public new class Face : HeMeshBase<V, E, F>.Face
        {
        }

        #endregion


        #region Static

        /// <summary></summary>
        public static readonly HeMesh3dFactory Factory = new HeMesh3dFactory();

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public HeMesh3d()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        public HeMesh3d(int vertexCapacity, int hedgeCapacity, int faceCapacity)
            : base(vertexCapacity, hedgeCapacity, faceCapacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override V NewVertex()
        {
            return new V();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override E NewHalfedge()
        {
            return new E();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override F NewFace()
        {
            return new F();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("HeMesh3d (V:{0} E:{1} F:{2})", Vertices.Count, Halfedges.Count >> 1, Faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M Duplicate()
        {
            return Factory.CreateCopy(this, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(M other)
        {
            Append(other, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M GetDual()
        {
            return Factory.CreateDual(this, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void AppendDual(M other)
        {
            AppendDual(other, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public M[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, out componentIndices, out edgeIndices, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public M[] SplitDisjoint(Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, componentIndex, edgeIndex, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void WriteToOBJ(string path)
        {
            MeshIO.WriteToObj(this, path, V.GetPosition, V.GetNormal, V.GetTexture);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        private static void Set(V v0, V v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="f"></param>
        private static void Set(V v, F f)
        {
            if (!f.IsUnused)
            {
                v.Position = f.Vertices.Mean(V.GetPosition);
                v.Normal = f.GetNormal(V.GetPosition);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMesh3dFactory : HeMeshBaseFactory<M, V, E, F>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override M Create(int vertexCapacity, int halfedgeCapacity, int faceCapacity)
        {
            return new M(vertexCapacity, halfedgeCapacity, faceCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M CreateFromFaceVertexData<T>(IEnumerable<Vec3d> vertices, IEnumerable<T> faces)
            where T : IEnumerable<int>
        {
            return CreateFromFaceVertexData(vertices, faces, (v, p) => v.Position = p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M CreateFromPolygons<T>(IEnumerable<T> polygons, double tolerance = SlurMath.ZeroTolerance)
            where T : IEnumerable<Vec3d>
        {
            return CreateFromPolygons(polygons, (v, p) => v.Position = p, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M CreateFromOBJ(string path)
        {
            return CreateFromObj(path, (v, p) => v.Position = p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public M CreateFromJson(string path)
        {
            var result = Create();
            MeshIO.ReadFromJson(path, result);
            return result;
        }
    }
}
