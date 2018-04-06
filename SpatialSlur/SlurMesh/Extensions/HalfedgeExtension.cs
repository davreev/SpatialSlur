
/*
 * Notes
 */
 
using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class HalfedgeExtension
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
        public static E NearestMin<V, E, K>(this Halfedge<V, E> start, Func<E, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start.Self, he => he.ConnectedEdges, getKey);
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
        public static E NearestMax<V, E, K>(this Halfedge<V, E> start, Func<E, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start.Self, he => he.ConnectedEdges, getKey);
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
        public static IEnumerable<E> WalkToMin<V, E, K>(this Halfedge<V, E> start, Func<E, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start.Self, he => he.ConnectedEdges, getKey);
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
        public static IEnumerable<E> WalkToMax<V, E, K>(this Halfedge<V, E> start, Func<E, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start.Self, he => he.ConnectedEdges, getKey);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        public static double GetLength<V, E>(this Halfedge<V, E> hedge)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            return GetLength(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        /// <returns></returns>
        public static double GetLength<V, E>(this Halfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            return getPosition(hedge.Start).DistanceTo(getPosition(hedge.End));
        }


        /// <summary>
        /// Returns the angle between this halfedge and the previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this Halfedge<V, E> hedge)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            return GetAngle(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this Halfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var p = getPosition(hedge.Start);

            return Vec3d.Angle(
                getPosition(hedge.Previous.Start) - p,
                getPosition(hedge.Next.Start) - p
                );
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static double GetCotangent<V, E, F>(this Halfedge<V, E, F> hedge)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetCotangent(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static double GetCotangent<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Vec3d p = getPosition(hedge.Previous.Start);

            return Vec3d.Cotangent(
                getPosition(hedge.Start) - p,
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vec3d GetNormal<V, E, F>(this Halfedge<V, E, F> hedge)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetNormal(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vec3d GetNormal<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Vec3d p = getPosition(hedge.Start);

            return Vec3d.Cross(
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
        public static Orient3d GetFrame<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return Orient3d.CreateFromPoints(
                getPosition(hedge.Start),
                getPosition(hedge.Next.Start),
                getPosition(hedge.Previous.Start)
                );
        }


        /// <summary>
        /// Calculates the unit length edge normal.
        /// </summary>
        public static Vec3d GetEdgeNormal<V, E, F>(this Halfedge<V, E, F> hedge)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetEdgeNormal(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Calculates the unit length edge normal.
        /// </summary>
        public static Vec3d GetEdgeNormal<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Vec3d p0 = getPosition(hedge.Start);
            Vec3d d1 = (getPosition(hedge.Previous.Start) + getPosition(hedge.Next.End)) * 0.5 - p0;

            hedge = hedge.Twin;

            Vec3d d2 = getPosition(hedge.Start) - p0;
            Vec3d d3 = (getPosition(hedge.Previous.Start) + getPosition(hedge.Next.End)) * 0.5 - p0;

            return (Vec3d.Cross(d1, d2) + Vec3d.Cross(d2, d3)).Unit;
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and the given face center.
        /// See V in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static double GetArea<V, E, F>(this Halfedge<V, E, F> hedge, Vec3d faceCenter)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetArea(hedge, IPosition3d<V>.Get, faceCenter);
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and the given face center.
        /// See V in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static double GetArea<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Vec3d faceCenter)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Vec3d p0 = getPosition(hedge.Start);
            return Vec3d.Cross(getPosition(hedge.End) - p0, faceCenter - p0).Length * 0.5;
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this Halfedge<V, E, F> hedge, Func<F, Vec3d> getNormal)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetDihedralAngle(hedge, IPosition3d<V>.Get, getNormal);
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Func<F, Vec3d> getNormal)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var twin = hedge.Twin;
            Vec3d axis = getPosition(twin.Start) - getPosition(hedge.Start);
            return GeometryUtil.GetDihedralAngle(axis.Unit, getNormal(hedge.Face), getNormal(twin.Face));
        }
    }
}
