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
        /// Returns the edge that this halfedge belongs to.
        /// Note that edges are implicitly represented via their first halfedge.
        /// </summary>
        E Edge { get; }


        /// <summary>
        /// Returns true if this halfedge is the first of its edge.
        /// </summary>
        bool IsFirstInEdge { get; }
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
        E PreviousAtStart { get; }


        /// <summary>
        /// Returns the next halfedge at the start vertex of this halfedge.
        /// </summary>
        E NextAtStart { get; }


        /// <summary>
        /// Forward circulates through all halfedges outgoing from the start vertex of this halfedge.
        /// </summary>
        IEnumerable<E> CirculateStart { get; }


        /// <summary>
        /// Forward circulates through all halfedges incoming to the end vertex of this halfedge.
        /// </summary>
        IEnumerable<E> CirculateEnd { get; }

        
        /// <summary>
        /// Returns a halfedge from each pair connected to this one.
        /// </summary>
        IEnumerable<E> ConnectedPairs { get; }


        /// <summary>
        /// Returns true if this halfedge is the first at its start vertex.
        /// </summary>
        bool IsFirstAtStart { get; }


        /// <summary>
        /// Returns true if this halfedge starts at a vertex of the given degree.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        bool IsAtDegree(int n);


        /// <summary>
        /// Returns the number of halfedges at the start vertex of this halfedge.
        /// </summary>
        /// <returns></returns>
        int CountEdgesAtStart();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        E GetRelativeAtStart(int offset);
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
        E PreviousInFace { get; }


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
        /// Returns true if this halfedge or its twin is in a hole.
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
        /// Returns true if this halfedge is in a face or hole of the given degree.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        bool IsInDegree(int n);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        E GetRelativeInFace(int offset);


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
        /// Returns the edge bundle that this halfedge belongs to.
        /// Note that edge bundles are implicitly represented via their first halfedge.
        /// </summary>
        E Bundle { get; }


        /// <summary>
        /// 
        /// </summary>
        C Cell { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsFirstInBundle { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsFirstInCell { get; }
    }
}
