
/*
 * Notes
 */

#if USING_RHINO

using System;

using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class IHeElementExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="hedge"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static Line ToLine<V, E>(this Halfedge<V, E> hedge)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            return ToLine(hedge, IPosition3d<V>.Get);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="hedge"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static Line ToLine<V, E>(this Halfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            Vec3d p0 = getPosition(hedge.Start);
            Vec3d p1 = getPosition(hedge.End);
            return new Line(p0.X, p0.Y, p0.Z, p1.X, p1.Y, p1.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static Polyline ToPolyline<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return ToPolyline(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static Polyline ToPolyline<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Polyline result = new Polyline();

            foreach (var v in face.Vertices)
            {
                var p = getPosition(v);
                result.Add(p.X, p.Y, p.Z);
            }

            result.Add(result.First);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static Polyline ToPolyline<V, E, F>(this HeFace<V, E, F> face, Func<E, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Polyline result = new Polyline();

            foreach (var he in face.Halfedges)
            {
                var p = getPosition(he);
                result.Add(p.X, p.Y, p.Z);
            }

            result.Add(result.First);
            return result;
        }


        /// <summary>
        /// Returns the circumcircle of a triangular face.
        /// Assumes the face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static Circle GetCircumcircle<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetCircumcircle(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the circumcircle of a triangular face.
        /// Assumes the face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static Circle GetCircumcircle<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var he = face.First;
            Point3d p0 = getPosition(he.Previous.Start);
            Point3d p1 = getPosition(he.Previous.Start);
            Point3d p2 = getPosition(he.Next.Start);
            return new Circle(p0, p1, p2);
        }


        /// <summary>
        /// Returns the incircle of a triangular face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Circle GetIncircle<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetIncircle(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the incircle of a triangular face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Circle GetIncircle<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var he = face.First;

            Vec3d p0 = getPosition(he.Previous.Start);
            Vec3d p1 = getPosition(he.Start);
            Vec3d p2 = getPosition(he.Next.Start);

            double d01 = p0.DistanceTo(p1);
            double d12 = p1.DistanceTo(p2);
            double d20 = p2.DistanceTo(p0);

            var p = (d01 + d12 + d20) * 0.5; // semiperimeter
            var pInv = 1.0 / p; // inverse semiperimeter
            var radius = Math.Sqrt(p * (p - d01) * (p - d12) * (p - d20)) * pInv; // triangle area (Heron's formula) / semiperimeter

            pInv *= 0.5; // inverse perimeter
            var center = p0 * (d12 * pInv) + p1 * (d20 * pInv) + p2 * (d01 * pInv);

            return new Circle(new Plane(p0, p1, (Point3d)p2), center, radius);
        }
    }
}

#endif