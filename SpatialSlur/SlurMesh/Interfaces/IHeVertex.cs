using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface IHeVertex<V, E> : IHeElement
        where E : IHalfedge<V, E>
        where V : IHeVertex<V, E>
    {
        /// <summary>
        /// 
        /// </summary>
        E FirstOut { get; }


        /// <summary>
        /// 
        /// </summary>
        E FirstIn { get; }


        /// <summary>
        /// Returns the number of edges incident to this vertex.
        /// </summary>
        int Degree { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsDegree1 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsDegree2 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsDegree3 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsDegree4 { get; }


        /// <summary>
        /// Forward circulates through all vertices connected to this one.
        /// </summary>
        IEnumerable<V> ConnectedVertices { get; }


        /// <summary>
        /// Forward circulates through all halfedges starting at this vertex.
        /// </summary>
        IEnumerable<E> OutgoingHalfedges { get; }


        /// <summary>
        /// Forward circulates through all halfedges ending at this vertex.
        /// </summary>
        IEnumerable<E> IncomingHalfedges { get; }


        /// <summary>
        /// Returns true if the given vertex is connected to this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsConnectedTo(V other);


        /// <summary>
        /// Returns a halfedge from this vertex to another or null if none exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        E FindHalfedge(V other);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="F"></typeparam>
    public interface IHeVertex<V, E, F> : IHeVertex<V, E>
        where E : IHalfedge<V, E, F>
        where V : IHeVertex<V, E, F>
        where F : IHeFace<V, E, F>
    {
        /// <summary>
        /// Returns true if this vertex lies on the mesh boundary.
        /// Note that if this is true, the first halfedge has a null face reference.
        /// </summary>
        bool IsBoundary { get; }


        /// <summary>
        /// Forwards circulates through all faces surrounding this vertex.
        /// Note null faces are skipped.
        /// </summary>
        IEnumerable<F> SurroundingFaces { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="C"></typeparam>
    public interface IHeVertex<V, E, F, C> : IHeVertex<V, E, F>
       where V : IHeVertex<V, E, F, C>
       where E : IHalfedge<V, E, F, C>
       where F : IHeFace<V, E, F, C>
       where C : IHeCell<V, E, F, C>
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<C> SurroundingCells { get; }
    }
}
