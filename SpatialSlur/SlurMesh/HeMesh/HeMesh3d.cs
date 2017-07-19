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
    /// Static constructors and extension methods for an HeMesh with commonly used geometric properites.
    /// </summary>
    public static class HeMesh3d
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
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HeMesh<V, E, F> TryCast(object obj)
        {
            return obj as HeMesh<V, E, F>;
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class V : HeVertex<V, E, F>, IVertex3d
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
        public class E : Halfedge<V, E, F>
        {
            /// <summary></summary>
            public double Weight;
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public class F : HeFace<V, E, F>
        {
            /// <summary></summary>
            public Vec3d Normal;
        }
    }
}
