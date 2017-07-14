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
    public static class HeMeshDM
    {
        /// <summary></summary>
        public static readonly HeMeshFactory<V, E, F> Factory = HeMeshFactory.Create(() => new V(), () => new E(), () => new F());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        /// <returns></returns>
        public static HeMesh<V, E, F> Create(int vertexCapacity = 4, int hedgeCapacity = 4, int faceCapacity = 4)
        {
            return Factory.Create(vertexCapacity, hedgeCapacity, faceCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public class V : HeVertex<V, E, F>, IVertex3d
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
        public class E : Halfedge<V, E, F>
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
        public class F : HeFace<V, E, F>
        {
            /// <summary></summary>
            public int RefineTag = int.MinValue;
        }
    }
}
