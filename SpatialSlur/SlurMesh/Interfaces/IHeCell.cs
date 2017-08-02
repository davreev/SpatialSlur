using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="C"></typeparam>
    public interface IHeCell<V, E, F, C> : IHeElement
        where V : IHeVertex<V, E, F, C>
        where E : IHalfedge<V, E, F, C>
        where F : IHeFace<V, E, F, C>
        where C : IHeCell<V, E, F, C>
    {
        /// <summary>
        /// Returns the first halfedge in the cell.
        /// </summary>
        E First { get; }


        /// <summary>
        /// 
        /// </summary>
        IEnumerable<E> Halfedges { get; }


        /// <summary>
        /// 
        /// </summary>
        IEnumerable<V> Vertices { get; }


        /// <summary>
        /// 
        /// </summary>
        IEnumerable<F> Faces { get; }


        /// <summary>
        /// 
        /// </summary>
        IEnumerable<F> AdjacentCells { get; }
    }
}
