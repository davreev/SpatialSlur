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
    /// Static constructors for an HeGraph that holds particle handles on its vertices.
    /// </summary>
    public static class HeGraphDynamic<H>
            where H : ParticleHandle
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
        [Serializable]
        public class V : HeVertex<V, E>
        {
            /// <summary></summary>
            public H Handle { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class E : Halfedge<V, E> { }
    }
}