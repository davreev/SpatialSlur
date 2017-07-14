using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurMesh;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Static constructors for an HeMesh that holds particle handles on its vertices.
    /// </summary>
    /// <typeparam name="H"></typeparam>
    public static class HeMeshDynamic<H>
        where H : ParticleHandle
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
        [Serializable]
        public class V : HeVertex<V, E, F>
        {
            /// <summary></summary>
            public H Handle { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class E : Halfedge<V, E, F> { }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class F : HeFace<V, E, F> { }
    }
}
