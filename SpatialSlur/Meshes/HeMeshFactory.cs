
/*
 * Notes
 */

using System;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    using M = HeMesh;
    using V = HeMesh.Vertex;
    using E = HeMesh.Halfedge;
    using F = HeMesh.Face;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMeshFactory : HeMeshFactory<M, V, E, F>
    {
        /// <inheritdoc />
        public sealed override M Create(int vertexCapacity, int hedgeCapacity, int faceCapacity)
        {
            return new M(vertexCapacity, hedgeCapacity, faceCapacity);
        }
    }
}
