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
    using G = HeGraph3d;
    using V = HeGraph3d.Vertex;
    using E = HeGraph3d.Halfedge;


    /// <summary>
    /// Implementation with double precision vertex attributes commonly used in 3d applications.
    /// </summary>
    [Serializable]
    public class HeGraph3d : HeGraphBase<V, E>
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeGraphBase<V, E>.Vertex, IVertex3d
        {
            #region Static

            /// <summary></summary>
            public static readonly Func<V, Vec3d> GetPosition = v => v.Position;
            /// <summary></summary>
            public static readonly Func<V, Vec3d> GetNormal = v => v.Normal;

            #endregion


            /// <summary></summary>
            public Vec3d Position { get; set; }

            /// <summary></summary>
            public Vec3d Normal { get; set; }


            #region Explicit interface implementations

            /// <summary>
            /// 
            /// </summary>
            Vec2d IVertex3d.Texture
            {
                get { return new Vec2d(); }
                set { throw new NotImplementedException(); }
            }

            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Halfedge : HeGraphBase<V, E>.Halfedge
        {
        }

        #endregion


        #region Static

        /// <summary></summary>
        public static readonly HeGraph3dFactory Factory = new HeGraph3dFactory();

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
        public override string ToString()
        {
            return String.Format("HeGraph3d (V:{0} E:{1})", Vertices.Count, Halfedges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public G Duplicate()
        {
            return Factory.CreateCopy(this, Set);
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
        public void AppendVertexTopology(HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            AppendVertexTopology(mesh, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendFaceTopology(HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
           AppendFaceTopology(mesh, Set);
        }


        /// <summary> </summary>
        private static void Set(V v0, V v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary> </summary>
        private static void Set(V v0, HeMesh3d.Vertex v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary> </summary>
        private static void Set(V v, HeMesh3d.Face f)
        {
            if (!f.IsUnused)
            {
                v.Position = f.Vertices.Mean(HeMesh3d.Vertex.GetPosition);
                v.Normal = f.GetNormal(HeMesh3d.Vertex.GetPosition);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraph3dFactory : HeGraphBaseFactory<G, V, E>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        public G CreateFromVertexTopology(HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
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
        public G CreateFromFaceTopology(HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
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
