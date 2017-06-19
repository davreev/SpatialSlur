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
    /// Static constructors and extension methods for an HeGraph with extendable user defined properties.
    /// </summary>
    public static partial class HeGraphBRG
    {
        /// <summary></summary>
        public static readonly HeGraphFactory<V, E> Factory = HeGraphFactory.Create(() => new V(), () => new E());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public static HeGraph<V,E> Create(int vertexCapacity = 4, int hedgeCapacity = 4)
        {
            return Factory.Create(vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        public static HeGraph<V, E> Duplicate(this HeGraph<V, E> graph)
        {
            return Factory.CreateFromOther(graph,
                (v0, v1) => v0.Data.Set(v1.Data),
                (he0, he1) => he0.Data.Set(he1.Data)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        public class V : HeVertex<V, E>
        {
            /// <summary></summary>
            public Dictionary<string, object> Data { get; } = new Dictionary<string, object>
            {
                { "x", 0.0 },
                { "y", 0.0 },
                { "z", 0.0 }
            };
        }


        /// <summary>
        /// 
        /// </summary>
        public class E : Halfedge<V, E>
        {
            /// <summary></summary>
            public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        }
    }
}
