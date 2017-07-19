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
    /// Static constructors and extension methods for an HeGraph with commonly used geometric properites.
    /// </summary>
    public static class HeGraph3d
    {
        /// <summary></summary>
        public static readonly HeGraphFactory<V, E> Factory = HeGraphFactory.Create(() => new V(), () => new E());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public static HeGraph<V, E> Create(int vertexCapacity = 4, int hedgeCapacity = 4)
        {
            return Factory.Create(vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HeGraph<V, E> TryCast(object obj)
        {
            return obj as HeGraph<V, E>;
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class V : HeVertex<V, E>, IVertex3d
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
        public class E : Halfedge<V, E>
        {
            /// <summary></summary>
            public double Weight;
        }
    }
}
