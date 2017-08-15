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
        /// 
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
