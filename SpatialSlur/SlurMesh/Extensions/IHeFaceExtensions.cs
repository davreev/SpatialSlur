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
