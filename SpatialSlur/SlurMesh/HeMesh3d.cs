using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
    public class HeMesh3d : HeMesh<V, E, F>
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeMesh<V, E, F>.Vertex, IVertex3d
        {
            #region Static

            /// <summary>
            /// 
            /// </summary>
            public static class Accessors
            {
                /// <summary></summary>
                public static readonly Property<V, Vec3d> Position = Property.Create<V, Vec3d>(v => v.Position, (v, p) => v.Position = p);

                /// <summary></summary>
                public static readonly Property<V, Vec3d> Normal = Property.Create<V, Vec3d>(v => v.Normal, (v, n) => v.Normal = n);
            }

            #endregion

            
            /// <summary></summary>
            public Vec3d Position { get; set; }

            /// <summary></summary>
            public Vec3d Normal { get; set; }
            

            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            public void Set(V other)
            {
                Position = other.Position;
                Normal = other.Normal;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Halfedge : HeMesh<V, E, F>.Halfedge
        {
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public new class Face : HeMesh<V, E, F>.Face
        {
        }

        #endregion


        #region Static

        /// <summary></summary>
        public static readonly HeMesh3dFactory Factory = new HeMesh3dFactory();


        /// <summary> 
        /// 
        /// </summary>
        private static void Set(V v0, V v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary> 
        /// 
        /// </summary>
        private static void Set(V vertex, F face)
        {
            if (!face.IsUnused)
                vertex.Position = face.GetBarycenter(V.Accessors.Position);
        }

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
        /// <param name="other"></param>
        public HeMesh3d(M other)
            : base(other.Vertices.Capacity, other.Halfedges.Capacity, other.Faces.Capacity)
        {
            Append(other);
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
            return String.Format("HeMesh3d (V:{0} E:{1} F:{2})", Vertices.Count, Edges.Count, Faces.Count);
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
        /// <param name="skipValid"></param>
        public void CalculateNormals(bool skipValid = false, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Vertices.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Vertices.Count);
         
            void Body(int from, int to)
            {
                for(int i = from; i < to; i++)
                {
                    var v = Vertices[i];
                    if (v.IsUnused || (skipValid && v.Normal.IsUnit())) continue;
                    v.Normal = v.GetNormal();
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMesh3dFactory : HeMeshFactory<M, V, E, F>
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
