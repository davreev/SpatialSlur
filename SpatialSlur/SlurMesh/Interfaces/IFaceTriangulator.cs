using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    public interface IFaceTriangulator<V, E, F>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        /// <summary>
        /// Iterates through the vertices of each triangle without modifying the topology of the mesh.
        /// The last vertex from each triangle must not belong to the previously returned triangle.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        IEnumerable<(V, V, V)> GetTriangles(F face);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        void Triangulate(F face);
    }
}
