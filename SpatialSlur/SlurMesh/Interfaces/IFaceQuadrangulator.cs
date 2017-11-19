using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        where V : IHeVertex<V, E, F>
        where E : IHalfedge<V, E, F>
        where F : IHeFace<V, E, F>
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
