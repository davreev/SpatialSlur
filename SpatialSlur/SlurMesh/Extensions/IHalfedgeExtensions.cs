using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class IHalfedgeExtensions
    {
        /// <summary>
        /// Returns the difference in value between the end vertex and the start vertex.
        /// </summary>
        public static double GetDelta<V, E>(this IHalfedge<V, E> hedge, Func<V, double> getValue)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.End) - getValue(hedge.Start);
        }


        /// <summary>
        /// Returns the difference in value between the end vertex and the start vertex.
        /// </summary>
        public static Vec2d GetDelta<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec2d> getValue)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.End) - getValue(hedge.Start);
        }


        /// <summary>
        /// Returns the difference in value between the end vertex and the start vertex.
        /// </summary>
        public static Vec3d GetDelta<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec3d> getValue)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.End) - getValue(hedge.Start);
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        public static double Lerp<V, E>(this IHalfedge<V, E> hedge, Func<V, double> getValue, double t)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return SlurMath.Lerp(getValue(hedge.Start), getValue(hedge.End), t);
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        public static Vec2d Lerp<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec2d> getValue, double t)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.Start).LerpTo(getValue(hedge.End), t);
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        public static Vec3d Lerp<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec3d> getValue, double t)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.Start).LerpTo(getValue(hedge.End), t);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        public static double GetLength<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec2d> getPosition)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getPosition(hedge.Start).DistanceTo(getPosition(hedge.End));
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        public static double GetLength<V, E>(this IHalfedge<V, E> hedge)
            where V : IHeVertex<V, E>, IVertex3d
            where E : IHalfedge<V, E>
        {
            return hedge.Start.Position.DistanceTo(hedge.End.Position);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        /// <returns></returns>
        public static double GetLength<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getPosition(hedge.Start).DistanceTo(getPosition(hedge.End));
        }


        /// <summary>
        /// Returns the angle between this halfedge and the previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec2d> getPosition)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return Vec2d.Angle(hedge.GetDelta(getPosition), hedge.PrevAtStart.GetDelta(getPosition));
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return Vec3d.Angle(hedge.GetDelta(getPosition), hedge.PrevAtStart.GetDelta(getPosition));
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static double GetCotangent<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d p = getPosition(hedge.PrevInFace.Start);

            return Vec3d.Cotangent(
                getPosition(hedge.Start) - p,
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vec3d GetNormal<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d p = getPosition(hedge.Start);

            return Vec3d.Cross(
                p - getPosition(hedge.PrevInFace.Start),
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and the center of their adjacent face.
        /// See W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static double GetArea<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Func<F, Vec3d> getCenter)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return hedge.GetArea(getPosition, getCenter(hedge.Face));
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and the given face center.
        /// See W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static double GetArea<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Vec3d faceCenter)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d p0 = getPosition(hedge.PrevInFace.Start);
            Vec3d p1 = getPosition(hedge.Start);
            Vec3d p2 = getPosition(hedge.End);

            Vec3d v0 = (p2 - p1 + p1 - p0) * 0.5;
            return Vec3d.Cross(v0, faceCenter - p1).Length * 0.5; // area of projected planar quad
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Func<F, Vec3d> getNormal)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d tangent = getPosition(hedge.End) - getPosition(hedge.Start);
            tangent.Unitize();

            Vec3d x = getNormal(hedge.Face);
            Vec3d y = Vec3d.Cross(x, tangent);

            Vec3d d = getNormal(hedge.Twin.Face);
            double t = Math.Atan2(d * y, d * x);

            t = (t < 0.0) ? t + SlurMath.TwoPI : t; // shift discontinuity to 0
            return SlurMath.Mod(t + Math.PI, SlurMath.TwoPI); // add angle bw normals and faces
        }
    }
}
