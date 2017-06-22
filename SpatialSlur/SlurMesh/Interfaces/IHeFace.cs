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
        bool IsTriangle { get; }


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


    /// <summary>
    /// 
    /// </summary>
    public static class IHeFaceExtensions
    {
        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetBarycenter<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return face.Vertices.Mean(getPosition);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d result;

            if (face.IsTriangle)
            {
                // simplified tri case
                result = face.First.GetNormal(getPosition);
            }
            else
            {
                // general n-gon case
                result = new Vec3d();
                foreach (var he in face.Halfedges)
                    result += he.GetNormal(getPosition);
            }

            result.Unitize();
            return result;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeFace<V, E, F> face, Func<E, Vec3d> getNormal)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d result;

            if (face.IsTriangle)
                result = getNormal(face.First); // simplified tri case
            else
                result = face.Halfedges.Sum(getNormal); // general n-gon case

            result.Unitize();
            return result;
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetCircumcenter<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            var he = face.First;
            Vec3d p0 = getPosition(he.Start);

            he = he.NextInFace;
            Vec3d p1 = getPosition(he.Start);
            Vec3d p2 = getPosition(he.End);

            return p1 + GeometryUtil.GetCurvatureVector(p0 - p1, p2 - p1);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetIncenter<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            var he = face.First;
            Vec3d p0 = getPosition(he.PrevInFace.Start);
            Vec3d p1 = getPosition(he.Start);
            Vec3d p2 = getPosition(he.End);

            double d01 = p0.DistanceTo(p1);
            double d12 = p1.DistanceTo(p2);
            double d20 = p2.DistanceTo(p0);
            double pInv = 1.0 / (d01 + d12 + d20); // inverse perimeter

            return p0 * (d12 * pInv) + p1 * (d20 * pInv) + p2 * (d01 * pInv);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetIncenter<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition, Func<E, double> getLength)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            var he0 = face.First;
            var he1 = he0.NextInFace;
            var he2 = he1.NextInFace;

            Vec3d p0 = getPosition(he0.Start);
            Vec3d p1 = getPosition(he1.Start);
            Vec3d p2 = getPosition(he2.Start);

            double d01 = getLength(he0);
            double d12 = getLength(he1);
            double d20 = getLength(he2);
            double pInv = 1.0 / (d01 + d12 + d20); // inverse perimeter

            return p0 * (d12 * pInv) + p1 * (d20 * pInv) + p2 * (d01 * pInv);
        }
    }
}
