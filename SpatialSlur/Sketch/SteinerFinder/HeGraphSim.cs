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

namespace SpatialSlur.SlurRhino.SteinerFinder
{
    /// <summary>
    /// 
    /// </summary>
    public class HeGraphSim: HeGraphBase<HeGraphSim.Vertex, HeGraphSim.Halfedge>
    {
        /// <summary></summary>
        public static readonly HeGraphSimFactory Factory = new HeGraphSimFactory();


        /// <summary>
        /// 
        /// </summary>
        public HeGraphSim()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeGraphSim(int vertexCapacity, int hedgeCapacity)
            : base(vertexCapacity, hedgeCapacity)
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
        [Serializable]
        public class Vertex : HeVertex<Vertex, Halfedge>, IVertex3d
        {
            /// <summary></summary>
            public Vec3d Position { get; set; }
            /// <summary></summary>
            public Vec3d Velocity;
            /// <summary></summary>
            public Vec3d ForceSum;
            /// <summary></summary>
            public bool IsTerminal;


            #region Explicit interface implementations

            Vec3d IVertex3d.Normal
            {
                get { return new Vec3d(); }
                set { throw new NotImplementedException(); }
            }


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
            /// <summary></summary>
            public Vec3d Tangent;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraphSimFactory : HeGraphFactoryBase<HeGraphSim, HeGraphSim.Vertex, HeGraphSim.Halfedge>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override HeGraphSim Create(int vertexCapacity, int halfedgeCapacity)
        {
            return new HeGraphSim(vertexCapacity, halfedgeCapacity);
        }
    }
}
