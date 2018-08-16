
/*
 * Notes
 */

using System.Collections.Generic;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFaceQuadrangulator
    {
        /// <summary>
        /// Iterates through each quad in the face of the given halfedge.
        /// The last 2 vertices from each quad must not belong to the previously returned quad.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="start"></param>
        /// <returns></returns>
        IEnumerable<(V, V, V, V)> GetQuads<V, E, F>(HeMesh<V, E, F>.Halfedge start)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face;


        /// <summary>
        /// Splits the face of the given halfedge into quads.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        void Quadrangulate<V, E, F>(HeMesh<V, E, F> mesh, E start)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face;
    }
}
