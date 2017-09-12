using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurMesh;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino.Remesher
{
    /// <summary>
    /// Contains HeMesh element classes used in dynamic remeshing
    /// </summary>
    [Serializable]
    public class HeMeshSim:HeMeshBase<HeMeshSim.Vertex,HeMeshSim.Halfedge, HeMeshSim.Face>
    {
        /// <summary></summary>
        public static readonly HeMeshSimFactory Factory = new HeMeshSimFactory();


        /// <summary>
        /// 
        /// </summary>
        public HeMeshSim()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        public HeMeshSim(int vertexCapacity, int hedgeCapacity, int faceCapacity)
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
        [Serializable]
        public class Vertex : HeVertex<Vertex, Halfedge, Face>, IVertex3d
        {
            /// <summary></summary>
            public Vec3d Position { get; set; }
            /// <summary></summary>
            public Vec3d Normal { get; set; }

            /// <summary></summary>
            public Vec3d Velocity;
            /// <summary></summary>
            public Vec3d MoveSum;
            /// <summary></summary>
            public double WeightSum;

            /// <summary></summary>
            public int FeatureIndex = -1;
            /// <summary></summary>
            public int DegreeCached;
            /// <summary></summary>
            public int RefineTag = int.MinValue;

            #region Explicit interface implementations


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
        public class Halfedge : Halfedge<Vertex, Halfedge, Face>
        {
            private double _targetLength;

            /// <summary></summary>
            public double TargetLength
            {
                get { return _targetLength; }
                set { _targetLength = Twin._targetLength = value; }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Face : HeFace<Vertex, Halfedge, Face>
        {
            /// <summary></summary>
            public int RefineTag = int.MinValue;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMeshSimFactory : HeMeshFactoryBase<HeMeshSim, HeMeshSim.Vertex, HeMeshSim.Halfedge, HeMeshSim.Face>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override HeMeshSim Create(int vertexCapacity, int halfedgeCapacity, int faceCapacity)
        {
            return new HeMeshSim(vertexCapacity, halfedgeCapacity, faceCapacity);
        }
    }
}
