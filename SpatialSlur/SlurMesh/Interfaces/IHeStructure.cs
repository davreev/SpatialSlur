using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface IHeStructure<E>
        where E : HeElement, IHalfedge<E>
    {
        /// <summary>
        /// 
        /// </summary>
        HalfedgeList<E> Halfedges { get; }


        /// <summary>
        /// 
        /// </summary>
        HeEdgeList<E> Edges { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface IHeStructure<V, E> : IHeStructure<E>
        where V : HeElement, IHeVertex<V, E>
        where E : HeElement, IHalfedge<V, E>
    {
        /// <summary>
        /// 
        /// </summary>
        HeElementList<V> Vertices { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="F"></typeparam>
    public interface IHeStructure<V, E, F> : IHeStructure<V, E>
        where E : HeElement, IHalfedge<V, E, F>
        where V : HeElement, IHeVertex<V, E, F>
        where F : HeElement, IHeFace<V, E, F>
    {
        /// <summary>
        /// 
        /// </summary>
        HeElementList<F> Faces { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="C"></typeparam>
    public interface IHeStructure<V, E, F, C> : IHeStructure<V, E, F>
        where V : HeElement, IHeVertex<V, E, F, C>
        where E : HeElement, IHalfedge<V, E, F, C>
        where F : HeElement, IHeFace<V, E, F, C>
        where C : HeElement, IHeCell<V, E, F, C>
    {
        /// <summary>
        /// 
        /// </summary>
        HeElementList<C> Cells { get; }
    }
}
