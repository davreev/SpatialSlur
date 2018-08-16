
/*
 * Notes
 */

using System;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    using G = HeGraph;
    using V = HeGraph.Vertex;
    using E = HeGraph.Halfedge;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraphFactory : HeGraphFactory<G, V, E>
    {
        /// <inheritdoc />
        public sealed override G Create(int vertexCapacity, int hedgeCapacity)
        {
            return new G(vertexCapacity, hedgeCapacity);
        }
    }
}
