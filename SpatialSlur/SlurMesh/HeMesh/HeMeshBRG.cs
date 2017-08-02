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
    public static class HeMeshBRG
    {
        /// <summary></summary>
        public static readonly HeMeshFactory<V, E, F> Factory;


        /// <summary>
        /// Static constructor to initialize factory instance.
        /// </summary>
        static HeMeshBRG()
        {
            var provider = HeElementProvider.Create(() => new V(), () => new E(), () => new F());
            Factory = HeMeshFactory.Create(provider);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        /// <returns></returns>
        public static HeMesh<V,E,F> Create(int vertexCapacity = 4, int hedgeCapacity = 4, int faceCapacity = 4)
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
        public class V : HeVertex<V, E, F>
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
        [Serializable]
        public class E : Halfedge<V, E, F>
        {
            /// <summary></summary>
            public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public class F : HeFace<V, E, F>
        {
            /// <summary></summary>
            public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        }
    }
}
