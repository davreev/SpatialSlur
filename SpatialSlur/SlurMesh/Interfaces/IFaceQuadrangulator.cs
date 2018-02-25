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
    public interface IFaceQuadrangulator<V, E, F>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        /// <summary>
        /// Iterates through the vertices of each quad without modifying the topology of the mesh.
        /// The last 2 vertices from each quad must not belong to the previously returned quad.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        IEnumerable<(V, V, V, V)> GetQuads(F face);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        void Quadrangulate(F face);
    }
}
