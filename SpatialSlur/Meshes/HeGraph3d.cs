
/*
 * Notes
 */

using System;
using System.ComponentModel;
using SpatialSlur;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    using G = HeGraph3d;
    using V = HeGraph3d.Vertex;
    using E = HeGraph3d.Halfedge;


    /// <summary>
    /// Implementation with double precision vertex attributes commonly used in 3d applications
    /// </summary>
    [Serializable]
    public class HeGraph3d : HeGraph<V, E>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeGraph<V, E>.Vertex, IPosition3d, INormal3d
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
        public new class Halfedge : HeGraph<V, E>.Halfedge, INormal3d
        {
            /// <inheritdoc />
            public Vector3d Normal { get; set; }
        }

        #endregion


        #region Static Members

        /// <summary></summary>
        public static readonly HeGraph3dFactory Factory = new HeGraph3dFactory();


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
        private static void Set(V v0, HeMesh3d.Vertex v1)
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
        private static void Set(E he0, HeMesh3d.Halfedge he1)
        {
            he0.Normal = he1.Normal;
        }


        /// <summary> 
        /// 
        /// </summary>
        private static void Set(V vertex, HeMesh3d.Face face)
        {
            if (!face.IsUnused)
                vertex.Position = face.GetBarycenter();

            vertex.Normal = face.Normal;
        }

        
        /// <summary> 
        /// 
        /// </summary>
        private static void Set(HeMesh3d.Face face, V vertex)
        {
            face.Normal = vertex.Normal;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public HeGraph3d()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public HeGraph3d (int vertexCapacity, int hedgeCapacity)
            :base(vertexCapacity, hedgeCapacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public HeGraph3d(G other)
            : base(other.Vertices.Capacity, other.Halfedges.Capacity)
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
        public override string ToString()
        {
            return String.Format("HeGraph3d (V:{0} E:{1})", Vertices.Count, Edges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(G other)
        {
            Append(other, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public G[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public G[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, out componentIndices, out edgeIndices, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public G[] SplitDisjoint(Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, componentIndex, edgeIndex, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendVertexTopology(HeMesh<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            AppendVertexTopology(mesh, Set, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendFaceTopology(HeMesh<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
           AppendFaceTopology(mesh, Set, Set);
        }
    }
}
