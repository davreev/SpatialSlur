
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using SpatialSlur;
using SpatialSlur.Meshes.Impl;

using static SpatialSlur.Meshes.Delegates;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public static class HalfedgeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static E NearestMin<V, E, K>(this HeStructure<V, E>.Halfedge start, Func<E, K> getKey)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GraphSearch.NearestMin(start.Self, GetConnected<V, E>, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static E NearestMax<V, E, K>(this HeStructure<V, E>.Halfedge start, Func<E, K> getKey)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GraphSearch.NearestMax(start.Self, GetConnected<V, E>, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<E> WalkToMin<V, E, K>(this HeStructure<V, E>.Halfedge start, Func<E, K> getKey)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GraphSearch.WalkToMin(start.Self, GetConnected<V, E>, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<E> WalkToMax<V, E, K>(this HeStructure<V, E>.Halfedge start, Func<E, K> getKey)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GraphSearch.WalkToMax(start.Self, GetConnected<V, E>, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<E> GetConnected<V, E>(E hedge)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            yield return hedge.Previous;
            yield return hedge.Next;

            hedge = hedge.Twin;

            yield return hedge.Previous;
            yield return hedge.Next;
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        public static double GetLength<V, E>(this HeStructure<V, E>.Halfedge hedge)
            where V : HeStructure<V, E>.Vertex, IPosition3d
            where E : HeStructure<V, E>.Halfedge
        {
            return GetLength(hedge, Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        /// <returns></returns>
        public static double GetLength<V, E>(this HeStructure<V, E>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            return getPosition(hedge.Start).DistanceTo(getPosition(hedge.End));
        }


        /// <summary>
        /// Returns the angle between this halfedge and the previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this HeStructure<V, E>.Halfedge hedge)
            where V : HeStructure<V, E>.Vertex, IPosition3d
            where E : HeStructure<V, E>.Halfedge
        {
            return GetAngle(hedge, Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this HeStructure<V, E>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var p = getPosition(hedge.Start);

            return Vector3d.Angle(
                getPosition(hedge.Previous.Start) - p,
                getPosition(hedge.Next.Start) - p
                );
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// </summary>
        public static double GetCotangent<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetCotangent(hedge, Position3d<V>.Get);
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// </summary>
        public static double GetCotangent<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            Vector3d p = getPosition(hedge.Previous.Start);

            return Vector3d.Cotangent(
                getPosition(hedge.Start) - p,
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// Calculates the cotangent weight for the given edge.
        /// Assumes adjacent faces are triangular.
        /// </summary>
        /// <param name="hedge"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetEdgeCotanWeight<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var twin = hedge.Twin;
            var p0 = getPosition(hedge.Start);
            var p1 = getPosition(twin.Start);
            var w = 0.0;

            if (hedge.Face != null)
            {
                var p2 = getPosition(hedge.Previous.Start);
                w += Vector3d.Cotangent(p0 - p2, p1 - p2);
            }

            if (twin.Face != null)
            {
                var p3 = getPosition(twin.Previous.Start);
                w += Vector3d.Cotangent(p0 - p3, p1 - p3);
            }

            return w * 0.5;
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vector3d GetNormal<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetNormal(hedge, Position3d<V>.Get);
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vector3d GetNormal<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            Vector3d p = getPosition(hedge.Start);

            return Vector3d.Cross(
                p - getPosition(hedge.Previous.Start),
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public static Orient3d GetFrame<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return Orient3d.CreateFromPoints(
                getPosition(hedge.Start),
                getPosition(hedge.Next.Start),
                getPosition(hedge.Previous.Start)
                );
        }


        /// <summary>
        /// Calculates the unit length area-weighted edge normal.
        /// </summary>
        public static Vector3d GetEdgeNormal<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetEdgeNormal(hedge, Position3d<V>.Get);
        }


        /// <summary>
        /// Calculates the unit length area-weighted edge normal.
        /// </summary>
        public static Vector3d GetEdgeNormal<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var twin = hedge.Twin;
            Vector3d p0 = getPosition(hedge.Start);

            Vector3d d1 = (getPosition(hedge.Previous.Start) + getPosition(hedge.Next.End)) * 0.5 - p0;
            Vector3d d2 = (getPosition(twin.Previous.Start) + getPosition(twin.Next.End)) * 0.5 - p0;
            Vector3d d3 = getPosition(twin.Start) - p0;

            return (Vector3d.Cross(d1, d3) + Vector3d.Cross(d3, d2)).Unit;
        }


        /// <summary>
        /// Returns the area of the given halfedge as per V in https://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleAreasCheatSheet.pdf
        /// Assumes the halfedge is in a triangular face.
        /// </summary>
        public static double GetArea<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetArea(hedge, Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the area of the given halfedge as per V in https://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleAreasCheatSheet.pdf
        /// Assumes the halfedge is in a triangular face.
        /// </summary>
        public static double GetArea<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var p0 = getPosition(hedge.Start);
            var p1 = getPosition(hedge.Next.Start);
            var p2 = getPosition(hedge.Previous.Start);
            return p0.SquareDistanceTo(p1) * Vector3d.Cotangent(p0 - p2, p1 - p2) * 0.25;
        }


        /// <summary>
        /// Returns the area of the given halfedge as per D in https://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleAreasCheatSheet.pdf
        /// Assumes adjacent faces are triangular.
        /// </summary>
        public static double GetEdgeArea<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetEdgeArea(hedge, Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the area of the given halfedge as per D in https://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleAreasCheatSheet.pdf
        /// Assumes adjacent faces are triangular.
        /// </summary>
        public static double GetEdgeArea<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var twin = hedge.Twin;
            var p0 = getPosition(hedge.Start);
            var p1 = getPosition(twin.Start);
            var w = 0.0;

            if (hedge.Face != null)
            {
                var p2 = getPosition(hedge.Previous.Start);
                w += Vector3d.Cotangent(p0 - p2, p1 - p2);
            }

            if (twin.Face != null)
            {
                var p3 = getPosition(twin.Previous.Start);
                w += Vector3d.Cotangent(p0 - p3, p1 - p3);
            }

            return p0.SquareDistanceTo(p1) * w * 0.25;
        }


        /// <summary>
        /// Calcuated as the signed angle between adjacent face normals where convex is positive.
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<F, Vector3d> getNormal)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetDihedralAngle(hedge, Position3d<V>.Get, getNormal);
        }


        /// <summary>
        /// Calcuated as the signed angle between adjacent face normals where convex is positive.
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this HeStructure<V, E, F>.Halfedge hedge, Func<V, Vector3d> getPosition, Func<F, Vector3d> getNormal)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var twin = hedge.Twin;
            var axis = getPosition(twin.Start) - getPosition(hedge.Start);
            return Geometry.GetDihedralAngle(axis.Unit, getNormal(hedge.Face), getNormal(twin.Face));
        }
    }
}
