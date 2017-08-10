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
    public interface IHalfedge<E> : IHeElement
        where E : IHalfedge<E>
    {
        /// <summary>
        /// Returns the oppositely oriented halfedge in the pair.
        /// </summary>
        E Twin { get; }


        /// <summary>
        /// 
        /// </summary>
        E Next { get; }


        /// <summary>
        /// 
        /// </summary>
        E Previous { get; }


        /// <summary>
        /// Returns the first halfedge in the pair.
        /// </summary>
        E Older { get; }


        /// <summary>
        /// Returns a halfedge from each pair connected to this one.
        /// </summary>
        IEnumerable<E> ConnectedPairs { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface IHalfedge<V, E> : IHalfedge<E>
        where E : IHalfedge<V, E>
        where V : IHeVertex<V, E>
    {
        /// <summary>
        /// Returns the vertex at the start of this halfedge.
        /// </summary>
        V Start { get; }


        /// <summary>
        /// Returns the vertex at the end of this halfedge.
        /// </summary>
        V End { get; }


        /// <summary>
        /// Returns the previous halfedge at the start vertex of this halfedge.
        /// </summary>
        E PrevAtStart { get; }


        /// <summary>
        /// Returns the next halfedge at the start vertex of this halfedge.
        /// </summary>
        E NextAtStart { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsAtDegree1 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsAtDegree2 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsAtDegree3 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsAtDegree4 { get; }


        /// <summary>
        /// Forward circulates through all halfedges outgoing from the start vertex of this halfedge.
        /// </summary>
        IEnumerable<E> CirculateStart { get; }


        /// <summary>
        /// Forward circulates through all halfedges incoming to the end vertex of this halfedge.
        /// </summary>
        IEnumerable<E> CirculateEnd { get; }


        /// <summary>
        /// Returns true if this halfedge is the first at its start vertex.
        /// </summary>
        bool IsFirstAtStart { get; }


        /// <summary>
        /// Returns the number of halfedges at the start vertex of this halfedge.
        /// </summary>
        /// <returns></returns>
        int CountEdgesAtStart();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        E OffsetAtStart(int count);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="F"></typeparam>
    public interface IHalfedge<V, E, F> : IHalfedge<V, E>
        where E : IHalfedge<V, E, F>
        where V : IHeVertex<V, E, F>
        where F : IHeFace<V, E, F>
    {
        /// <summary>
        /// Returns the previous halfedge within the face.
        /// </summary>
        E PrevInFace { get; }


        /// <summary>
        /// Returns the next halfedge within the face.
        /// </summary>
        E NextInFace { get; }


        /// <summary>
        /// Returns the face adjacent to this halfedge.
        /// If this halfedge is in a hole, null is returned.
        /// </summary>
        F Face { get; }


        /// <summary>
        /// Forward circulates through all halfedges in the face of this halfedge.
        /// </summary>
        IEnumerable<E> CirculateFace { get; }


        /// <summary>
        /// Returns true if this halfedge or its twin has a null face reference.
        /// </summary>
        /// <returns></returns>
        bool IsBoundary { get; }


        /// <summary>
        /// Returns true if this halfedge has a null face reference.
        /// </summary>
        /// <returns></returns>
        bool IsHole { get; }


        /// <summary>
        /// Returns true if this halfedge is the first in its face.
        /// </summary>
        /// <returns></returns>
        bool IsFirstInFace { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsInDegree1 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsInDegree2 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsInDegree3 { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsInDegree4 { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        E OffsetInFace(int count);


        /// <summary>
        /// Returns the number of halfedges in the face of this halfedge.
        /// </summary>
        /// <returns></returns>
        int CountEdgesInFace();


        /// <summary>
        /// Returns the first faceless halfedge encountered during circulation around the start vertex of this halfedge.
        /// If no such halfedge is found, null is returned.
        /// </summary>
        E NextBoundaryAtStart { get; }


        /// <summary>
        /// Returns the first boundary halfedge encountered during circulating around the face of this halfedge.
        /// If no such halfedge is found, null is returned.
        /// </summary>
        E NextBoundaryInFace { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="C"></typeparam>
    public interface IHalfedge<V, E, F, C> : IHalfedge<V, E, F>
        where V : IHeVertex<V, E, F, C>
        where E : IHalfedge<V, E, F, C>
        where F : IHeFace<V, E, F, C>
        where C : IHeCell<V, E, F, C>
    {
        /// <summary>
        /// Returns the oppositely oriented halfedge in the adjacent cell.
        /// </summary>
        E Adjacent { get; }


        /// <summary>
        /// 
        /// </summary>
        C Cell { get; }
    }
}
