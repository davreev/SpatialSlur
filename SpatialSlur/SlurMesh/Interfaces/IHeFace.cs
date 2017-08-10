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
    /// <typeparam name="F"></typeparam>
    public interface IHeFace<V, E, F> : IHeElement
        where V : IHeVertex<V, E, F>
        where E : IHalfedge<V, E, F>
        where F : IHeFace<V, E, F>
    {
        /// <summary>
        /// Returns the first halfedge in this face.
        /// </summary>
        E First { get; }


        /// <summary>
        /// Returns the number of edges in this face.
        /// </summary>
        int Degree { get; }


        /// <summary>
        /// Returns true if this face has at least 1 boundary edge.
        /// </summary>
        bool IsBoundary { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsDegree1 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsDegree2 {get;}


        /// <summary>
        /// 
        /// </summary>
        bool IsDegree3 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsDegree4 { get; }


        /// <summary>
        /// Forward circulates through all the halfedges in this face.
        /// </summary>
        IEnumerable<E> Halfedges { get; }


        /// <summary>
        /// Forward circulates through all the vertices in this face.
        /// </summary>
        IEnumerable<V> Vertices { get; }


        /// <summary>
        /// Forward circulates through all faces adjacent to this one.
        /// Note that null faces are skipped.
        /// Also if mutliple edges are shared with an adjacent face, that face will be returned multiple times.
        /// </summary>
        IEnumerable<F> AdjacentFaces { get; }


        /// <summary>
        /// Sets the first halfedge in this face to the first boundary halfedge encountered during circulation.
        /// Returns true if a boundary halfedge was found.
        /// </summary>
        bool SetFirstToBoundary();


        /// <summary>
        /// Returns the first halfedge between this face and another or null if none exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        E FindHalfedge(F other);


        /// <summary>
        /// Returns the number of boundary edges in this face.
        /// </summary>
        /// <returns></returns>
        int CountBoundaryEdges();


        /// <summary>
        /// Returns the number of boundary vertices in this face.
        /// </summary>
        /// <returns></returns>
        int CountBoundaryVertices();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="C"></typeparam>
    public interface IHeFace<V, E, F, C> : IHeFace<V, E, F>
       where V : IHeVertex<V, E, F, C>
       where E : IHalfedge<V, E, F, C>
       where F : IHeFace<V, E, F, C>
       where C : IHeCell<V, E, F, C>
    {
        /// <summary>
        /// 
        /// </summary>
        F Twin { get; }
    }
}
