
/*
 * Notes
 */

#if USING_RHINO

using System;

using Rhino.Geometry;

using SpatialSlur;
using SpatialSlur.Meshes;
using SpatialSlur.Meshes.Impl;

using static SpatialSlur.Meshes.Delegates;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeNodeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public static Line ToLine<V, E>(this HeStructure<V, E>.Halfedge hedge)
            where V : HeStructure<V, E>.Vertex, IPosition3d
            where E : HeStructure<V, E>.Halfedge
        {
            return ToLine(hedge, Position3d<V>.Get);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="hedge"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static Line ToLine<V, E>(this HeStructure<V, E>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var p0 = getPosition(hedge.Start);
            var p1 = getPosition(hedge.End);
            return new Line(p0.X, p0.Y, p0.Z, p1.X, p1.Y, p1.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Polyline ToPolyline<V, E, F>(this HeStructure<V, E, F>.Face face)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return ToPolyline(face, Position3d<V>.Get);
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
        public static Polyline ToPolyline<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
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
        public static Polyline ToPolyline<V, E, F>(this HeStructure<V, E, F>.Face face, Func<E, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
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
        /// <returns></returns>
        public static Circle GetCircumcircle<V, E, F>(this HeStructure<V, E, F>.Face face)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetCircumcircle(face, Position3d<V>.Get);
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
        public static Circle GetCircumcircle<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
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
        public static Circle GetIncircle<V, E, F>(this HeStructure<V, E, F>.Face face)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetIncircle(face, Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the incircle of a triangular face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Circle GetIncircle<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            // impl ref
            // http://mathworld.wolfram.com/Incenter.html

            var he = face.First;

            var p0 = getPosition(he.Previous.Start);
            var p1 = getPosition(he.Start);
            var p2 = getPosition(he.Next.Start);

            var d01 = p0.DistanceTo(p1);
            var d12 = p1.DistanceTo(p2);
            var d20 = p2.DistanceTo(p0);

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