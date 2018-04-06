
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
    public static class HeVertexExtension
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
        public static V NearestMin<V, E, K>(this HeVertex<V, E> start, Func<V, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start.Self, v => v.ConnectedVertices, getKey);
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
        public static V NearestMax<V, E, K>(this HeVertex<V, E> start, Func<V, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start.Self, v => v.ConnectedVertices, getKey);
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
        public static IEnumerable<V> WalkToMin<V, E, K>(this HeVertex<V, E> start, Func<V, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start.Self, v => v.ConnectedVertices, getKey);
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
        public static IEnumerable<V> WalkToMax<V, E, K>(this HeVertex<V, E> start, Func<V, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start.Self, v => v.ConnectedVertices, getKey);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeVertex<V, E, F> vertex)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetNormal(vertex, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeVertex<V, E, F> vertex, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return vertex.OutgoingHalfedges.Where(he => !he.IsHole).Sum(he => he.GetNormal(getPosition)).Unit;
        }
    }
}
