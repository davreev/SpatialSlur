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

namespace SpatialSlur.SlurRhino.GraphGrowth
{
    /// <summary>
    /// Contains HeMesh element classes used in dynamic remeshing
    /// </summary>
    public static class HeGraphGG
    {
        /// <summary></summary>
        public static readonly HeGraphFactory<V, E> Factory = HeGraphFactory.Create(() => new V(), () => new E());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        /// <returns></returns>
        public static HeGraph<V, E> Create(int vertexCapacity = 4, int hedgeCapacity = 4)
        {
            return Factory.Create(vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public class V : HeVertex<V, E>, IVertex3d
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
            public int Tag = int.MinValue;


            #region Explicit interface implementations

            
            Vec2d IVertex3d.TexCoord
            {
                get { return new Vec2d(); }
                set { throw new NotImplementedException(); }
            }


            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        public class E : Halfedge<V, E>
        {
            private double _maxLength;


            /// <summary></summary>
            public double MaxLength
            {
                get { return _maxLength; }
                set { _maxLength = Twin._maxLength = value; }
            }
            

            /// <summary></summary>
            public int Tag = int.MinValue;
        }
    }
}
