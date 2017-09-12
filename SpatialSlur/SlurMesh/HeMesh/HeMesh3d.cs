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
    public class HeMesh3d : HeMeshBase<HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face>
    {
        /// <summary></summary>
        public static readonly HeMesh3dFactory Factory = new HeMesh3dFactory();

        // property delegates
        private static readonly Func<IVertex3d, Vec3d> _getPosition = v => v.Position;
        private static readonly Func<IVertex3d, Vec3d> _getNormal = v => v.Normal;
        private static readonly Func<IVertex3d, Vec2d> _getTexture = v => v.Texture;


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
        protected sealed override Face NewFace()
        {
            return new Face();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh3d Duplicate()
        {
            return Factory.CreateCopy(this, Set, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(HeMesh3d other)
        {
            Append(other, Set, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh3d GetDual()
        {
            return Factory.CreateDual(this, Set, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void AppendDual(HeMesh3d other)
        {
            AppendDual(other, Set, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh3d[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, Set, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public HeMesh3d[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, Set, delegate { }, delegate { }, out componentIndices, out edgeIndices);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public HeMesh3d[] SplitDisjoint(Property<Halfedge, int> componentIndex, Property<Halfedge, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, Set, delegate { }, delegate { }, componentIndex, edgeIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void WriteToOBJ(string path)
        {
            HeMeshIO.WriteToOBJ(this, path, _getPosition, _getNormal, _getTexture);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        private static void Set(Vertex v0, Vertex v1)
        {
            v0.Position = v1.Position;
            v0.Normal = v1.Normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="f"></param>
        private static void Set(Vertex v, Face f)
        {
            if (!f.IsRemoved)
            {
                v.Position = f.Vertices.Mean(_getPosition);
                v.Normal = f.GetNormal(_getPosition);
            }
        }


        #region Derived element types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Vertex : HeVertex<Vertex, Halfedge, Face>, IVertex3d
        {
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
        public class Halfedge : Halfedge<Vertex, Halfedge, Face>
        {
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public class Face : HeFace<Vertex, Halfedge, Face>
        {
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMesh3dFactory : HeMeshFactoryBase<HeMesh3d, HeMesh3d.Vertex, HeMesh3d.Halfedge, HeMesh3d.Face>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override HeMesh3d Create(int vertexCapacity, int halfedgeCapacity, int faceCapacity)
        {
            return new HeMesh3d(vertexCapacity, halfedgeCapacity, faceCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh3d CreateFromFaceVertexData<T>(IEnumerable<Vec3d> vertices, IEnumerable<T> faces)
            where T : IEnumerable<int>
        {
            return CreateFromFaceVertexData(vertices, faces, (v, p) => v.Position = p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh3d CreateFromPolygons<T>(IEnumerable<T> polygons, double tolerance = 1.0e-8)
            where T : IEnumerable<Vec3d>
        {
            return CreateFromPolygons(polygons, (v, p) => v.Position = p, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh3d CreateFromOBJ(string path)
        {
            return CreateFromOBJ(path, (v, p) => v.Position = p);
        }
    }
}
