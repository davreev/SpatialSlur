using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    using G = HeGraph3d;
    using V = HeGraph3d.Vertex;
    using E = HeGraph3d.Halfedge;


    /// <summary>
    /// Implementation with double precision vertex attributes commonly used in 3d applications.
    /// </summary>
    [Serializable]
    public class HeGraph3d : HeGraph<V, E>
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeGraph<V, E>.Vertex, IVertex3d
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
            

            /// <inheritdoc />
            public Vec3d Position { get; set; }

            /// <inheritdoc />
            public Vec3d Normal { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Halfedge : HeGraph<V, E>.Halfedge
        {
        }

        #endregion


        #region Static

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
        private static void Set(V vertex, HeMesh3d.Face face)
        {
            if (!face.IsUnused)
                vertex.Position = face.GetBarycenter(HeMesh3d.Vertex.Accessors.Position);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public HeGraph3d (int vertexCapacity = DefaultCapacity, int hedgeCapacity = DefaultCapacity)
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
            Append(other, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public G[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public G[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, out componentIndices, out edgeIndices, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public G[] SplitDisjoint(Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, componentIndex, edgeIndex, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendVertexTopology(HeMesh<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            AppendVertexTopology(mesh, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendFaceTopology(HeMesh<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
           AppendFaceTopology(mesh, Set);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraph3dFactory : HeGraphFactory<G, V, E>
    {
        /// <inheritdoc />
        public sealed override G Create(int vertexCapacity, int hedgeCapacity)
        {
            return new G(vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoints"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public G CreateFromLineSegments(IReadOnlyList<Vec3d> endPoints, double tolerance = SlurMath.ZeroTolerance, bool allowMultiEdges = false, bool allowLoops = false)
        {
            return CreateFromLineSegments(endPoints, (v, p) => v.Position = p, tolerance, allowMultiEdges, allowLoops);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public G CreateFromVertexTopology(HeMesh<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            var graph = Create(mesh.Vertices.Count, mesh.Halfedges.Count);
            graph.AppendVertexTopology(mesh);
            return graph;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public G CreateFromFaceTopology(HeMesh<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            var graph = Create(mesh.Faces.Count, mesh.Halfedges.Count);
            graph.AppendFaceTopology(mesh);
            return graph;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public G CreateFromJson(string path)
        {
            var result = Create();
            MeshIO.ReadFromJson(path, result);
            return result;
        }
    }
}
