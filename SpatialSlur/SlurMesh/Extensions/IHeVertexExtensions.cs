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
    public static class IHeVertexExtensions
    {
        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeVertex<V, E, F> vertex, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d result = new Vec3d();

            foreach (var he in vertex.OutgoingHalfedges)
            {
                if (he.Face == null) continue;
                result += he.GetNormal(getPosition);
            }

            result.Unitize();
            return result;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeVertex<V, E, F> vertex, Func<E, Vec3d> getNormal)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d result = vertex.OutgoingHalfedges.Sum(getNormal);
            result.Unitize();
            return result;
        }
    }
}
