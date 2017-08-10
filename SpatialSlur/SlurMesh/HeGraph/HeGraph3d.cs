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
    /// Implementation with double precision vertex attributes commonly used in 3d embeddings.
    /// </summary>
    [Serializable]
    public class HeGraph3d : HeGraphBase<HeGraph3d.Vertex, HeGraph3d.Halfedge>
    {
        /// <summary></summary>
        public static readonly HeGraph3dFactory Factory;

        // property delegates
        private static readonly Func<IVertex3d, Vec3d> _getPosition = v => v.Position;
        private static readonly Func<IVertex3d, Vec3d> _getNormal = v => v.Normal;


        /// <summary>
        /// Static constructor to initialize factory instance.
        /// </summary>
        static HeGraph3d()
        {
            Factory = new HeGraph3dFactory();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        public HeGraph3d (int vertexCapacity = 4, int hedgeCapacity = 4)
            :base(vertexCapacity, hedgeCapacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override Vertex NewVertex()
        {
            return new Vertex();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override Halfedge NewHalfedge()
        {
            return new Halfedge();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeGraph3d Duplicate()
        {
            return Factory.CreateCopy(this, Set, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(HeGraph3d other)
        {
            Append(other, Set, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeGraph3d[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, Set, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public HeGraph3d[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, Set, delegate { }, out componentIndices, out edgeIndices);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public HeGraph3d[] SplitDisjoint(Property<Halfedge, int> componentIndex, Property<Halfedge, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, Set, delegate { }, componentIndex, edgeIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendVertexTopology(HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            AppendVertexTopology(mesh, Set, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendFaceTopology(HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
           AppendFaceTopology(mesh, Set, delegate { });
        }


        /// <summary> </summary>
        private static void Set(Vertex v0, Vertex v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary> </summary>
        private static void Set(Vertex v0, HeMesh3d.Vertex v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary> </summary>
        private static void Set(Vertex v, HeMesh3d.Face f)
        {
            if (!f.IsRemoved)
            {
                v.Position = f.Vertices.Mean(_getPosition);
                v.Normal = f.GetNormal(_getPosition);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Vertex : HeVertex<Vertex, Halfedge>, IVertex3d
        {
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
        public class Halfedge : Halfedge<Vertex, Halfedge>
        {
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraph3dFactory : HeGraphFactoryBase<HeGraph3d, HeGraph3d.Vertex, HeGraph3d.Halfedge>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override HeGraph3d Create(int vertexCapacity, int hedgeCapacity)
        {
            return new HeGraph3d(vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoints"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public HeGraph3d CreateFromLineSegments(IReadOnlyList<Vec3d> endPoints, double tolerance = 1.0e-8, bool allowMultiEdges = false, bool allowLoops = false)
        {
            return CreateFromLineSegments(endPoints, (v, p) => v.Position = p, tolerance, allowMultiEdges, allowLoops);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public HeGraph3d CreateFromVertexTopology(HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
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
        public HeGraph3d CreateFromFaceTopology(HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face> mesh)
        {
            var graph = Create(mesh.Faces.Count, mesh.Halfedges.Count);
            graph.AppendFaceTopology(mesh);
            return graph;
        }
    }
}
