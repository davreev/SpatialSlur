
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeFaceExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static F NearestMin<V, E, F, K>(this HeFace<V, E, F> start, Func<F, K> getKey)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start.Self, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static F NearestMax<V, E, F, K>(this HeFace<V, E, F> start, Func<F, K> getKey)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start.Self, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<F> WalkToMin<V, E, F, K>(this HeFace<V, E, F> start, Func<F, K> getKey)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start.Self, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<F> WalkToMax<V, E, F, K>(this HeFace<V, E, F> start, Func<F, K> getKey)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start.Self, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static double GetArea<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetArea(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the area of this face.
        /// Note that this method assumes the face is planar.
        /// </summary>
        /// <returns></returns>
        public static double GetArea<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            if (face.IsDegree(3))
                return face.First.GetNormal(getPosition).Length * 0.5;
            else
                return face.Halfedges.Sum(he => he.GetNormal(getPosition)).Length * 0.5;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetNormal(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            if (face.IsDegree(3))
                return face.First.GetNormal(getPosition).Unit;
            else
                return face.Halfedges.Sum(he => he.GetNormal(getPosition)).Unit;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeFace<V, E, F> face, Func<E, Vec3d> getNormal)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            if (face.IsDegree(3))
                return getNormal(face.First).Unit;
            else
                return face.Halfedges.Sum(getNormal).Unit;
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetBarycenter<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return face.Vertices.Mean(IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetBarycenter<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return face.Vertices.Mean(getPosition);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetCircumcenter<V, E, F>(this HeFace<V, E, F> face)
        where V : HeVertex<V, E, F>, IPosition3d
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            return GetCircumcenter(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetCircumcenter<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            var he = face.First;
            return GeometryUtil.GetCircumcenter(
                getPosition(he.Previous.Start),
                getPosition(he.Start),
                getPosition(he.Next.Start)
                );
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetIncenter<V, E, F>(this HeFace<V, E, F> face)
        where V : HeVertex<V, E, F>, IPosition3d
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            return GetIncenter(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetIncenter<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            var he = face.First;
            return GeometryUtil.GetIncenter(
                getPosition(he.Previous.Start),
                getPosition(he.Start),
                getPosition(he.Next.Start)
                );
        }


        /// <summary>
        /// Returns the planar deviation of this face
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetPlanarity<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetPlanarity(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the planar deviation of this face.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetPlanarity<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var he0 = face.First;
            var he1 = he0;

            var p0 = getPosition(he1.Start);
            he1 = he1.Next;

            var p1 = getPosition(he1.Start);
            he1 = he1.Next;

            var p2 = getPosition(he1.Start);
            he1 = he1.Next;

            // tri case
            if (ReferenceEquals(he1, he0))
                return 0.0;

            var p3 = getPosition(he1.Start);
            he1 = he1.Next;

            // quad case
            if (ReferenceEquals(he1, he0))
                return GeometryUtil.GetQuadPlanarity(p0, p1, p2, p3);

            // general case
            double sum = 0.0;
            int count = 0;

            foreach (var he in he1.Circulate)
            {
                sum += GeometryUtil.GetQuadPlanarity(p0, p1, p2, p3);
                p0 = p1; p1 = p2; p2 = p3;
                p3 = getPosition(he.Start);
            }

            return sum / count;
        }
    }
}
