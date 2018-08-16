
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
    public interface IFaceTriangulator
    {
        /// <summary>
        /// Iterates through each triangle in the face of the given halfedge.
        /// The last vertex from each triangle must not belong to the previously returned triangle.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="start"></param>
        /// <returns></returns>
        IEnumerable<(V, V, V)> GetTriangles<V, E, F>(HeMesh<V, E, F>.Halfedge start)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face;


        /// <summary>
        /// Splits the face of the given halfedge into triangles.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        void Triangulate<V, E, F>(HeMesh<V, E, F> mesh, E start)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face;
    }
}
