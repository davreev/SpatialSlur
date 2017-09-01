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
    public static class IHeFaceExtensions
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
        public static F NearestMin<V, E, F, K>(this F start, Func<F, K> getKey)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start, f => f.AdjacentFaces, getKey);
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
        public static F NearestMax<V, E, F, K>(this F start, Func<F, K> getKey)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start, f => f.AdjacentFaces, getKey);
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
        public static IEnumerable<F> WalkToMin<V, E, F, K>(this F start, Func<F, K> getKey)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start, f => f.AdjacentFaces, getKey);
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
        public static IEnumerable<F> WalkToMax<V, E, F, K>(this F start, Func<F, K> getKey)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start, f => f.AdjacentFaces, getKey);
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
            if (face.IsDegree3)
                return face.First.GetNormal(getPosition).Direction;
            else
                return face.Halfedges.Sum(he => he.GetNormal(getPosition)).Direction;
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
            if (face.IsDegree3)
                return getNormal(face.First).Direction;
            else
                return face.Halfedges.Sum(getNormal).Direction;
        }


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
            return GeometryUtil.GetCircumcenter(
                getPosition(he.PrevInFace.Start),
                getPosition(he.Start),
                getPosition(he.NextInFace.Start)
                );
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
            return GeometryUtil.GetIncenter(
                getPosition(he.PrevInFace.Start), 
                getPosition(he.Start), 
                getPosition(he.NextInFace.Start)
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
        public static double GetPlanarity<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            var he0 = face.First;
            var he1 = he0;

            var p0 = getPosition(he1.Start);
            he1 = he1.NextInFace;

            var p1 = getPosition(he1.Start);
            he1 = he1.NextInFace;

            var p2 = getPosition(he1.Start);
            he1 = he1.NextInFace;

            // tri case
            if (ReferenceEquals(he1, he0))
                return 0.0;

            // quad case
            if (ReferenceEquals(he1.NextInFace, he0))
                return GeometryUtil.LineLineShortestVector(p0, p2, p1, getPosition(he1.Start)).Length;

            // general case
            var he2 = he1;
            double max = 0.0;

            do
            {
                var p3 = getPosition(he2.Start);
                max = Math.Max(GeometryUtil.LineLineShortestVector(p0, p2, p1, p3).Length, max);
    
                he2 = he2.NextInFace;
                p0 = p1; p1 = p2; p2 = p3;
            } while (!ReferenceEquals(he2, he1));

            return max;
        }
    }
}
