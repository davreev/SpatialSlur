
/*
 * Notes
 */

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    using M = HeMesh3d;
    using V = HeMesh3d.Vertex;
    using E = HeMesh3d.Halfedge;
    using F = HeMesh3d.Face;

    /// <summary>
    /// Implementation with double precision vertex attributes commonly used in 3d applications
    /// </summary>
    [Serializable]
    public class HeMesh3d : HeMesh<V, E, F>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeMesh<V, E, F>.Vertex, IPosition3d, INormal3d
        {
            /// <inheritdoc />
            public Vector3d Position { get; set; }

            /// <inheritdoc />
            public Vector3d Normal { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Halfedge : HeMesh<V, E, F>.Halfedge, INormal3d
        {
            /// <inheritdoc />
            public Vector3d Normal { get; set; }
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public new class Face : HeMesh<V, E, F>.Face, INormal3d
        {
            /// <inheritdoc />
            public Vector3d Normal { get; set; }
        }

        #endregion


        #region Static Members

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
        private static void Set(E he0, E he1)
        {
            he0.Normal = he1.Normal;
        }


        /// <summary> 
        /// 
        /// </summary>
        private static void Set(F f0, F f1)
        {
            f0.Normal = f1.Normal;
        }


        /// <summary> 
        /// 
        /// </summary>
        private static void Set(V vertex, F face)
        {
            if (!face.IsUnused)
                vertex.Position = face.GetBarycenter();

            vertex.Normal = face.Normal;
        }


        /// <summary> 
        /// 
        /// </summary>
        private static void Set(F face, V vertex)
        {
            face.Normal = vertex.Normal;
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


        /// <inheritdoc />
        protected sealed override V NewVertex()
        {
            return new V();
        }


        /// <inheritdoc />
        protected sealed override E NewHalfedge()
        {
            return new E();
        }


        /// <inheritdoc />
        protected sealed override F NewFace()
        {
            return new F();
        }


        /// <inheritdoc />
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
            Append(other, Set, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M GetDual()
        {
            return Factory.CreateDual(this, Set, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void AppendDual(M other)
        {
            AppendDual(other, Set, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, Set, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public M[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, out componentIndices, out edgeIndices, Set, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public M[] SplitDisjoint(Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, componentIndex, edgeIndex, Set, Set, Set);
        }

        
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skipValid"></param>
        public void UpdateVertexNormals(bool skipValid = false, bool parallel = false)
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
        */
    }
}
