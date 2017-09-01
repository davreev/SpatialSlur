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
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static V NearestMin<V, E, K>(this V start, Func<V, K> getKey)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start, v => v.ConnectedVertices, getKey);
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
        public static V NearestMax<V, E, K>(this V start, Func<V, K> getKey)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start, v => v.ConnectedVertices, getKey);
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
        public static IEnumerable<V> WalkToMin<V, E, K>(this V start, Func<V, K> getKey)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start, v => v.ConnectedVertices, getKey);
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
        public static IEnumerable<V> WalkToMax<V, E, K>(this V start, Func<V, K> getKey)
            where V : HeElement, IHeVertex<V, E>
            where E : HeElement, IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start, v => v.ConnectedVertices, getKey);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeVertex<V, E, F> vertex, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return vertex.OutgoingHalfedges.Where(he => !he.IsHole).Sum(he => he.GetNormal(getPosition)).Direction;   
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
