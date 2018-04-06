
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeElementListExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="structure"></param>
        /// <param name="getPosition"></param>
        /// <param name="setResult"></param>
        /// <param name="parallel"></param>
        public static void GetEdgeLines<V, E>(this HeStructure<V, E> structure, Func<V, Vec3d> getPosition, Action<E, Line> setResult, bool parallel = false)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var edges = structure.Edges;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, edges.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var he = edges[i];
                    if (he.IsUnused) continue;
                    setResult(he, he.ToLine(getPosition));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="structure"></param>
        /// <param name="getPosition"></param>
        /// <param name="setResult"></param>
        /// <param name="parallel"></param>
        public static void GetFacePolylines<V, E, F>(this HeStructure<V, E, F> structure, Func<V, Vec3d> getPosition, Action<F, Polyline> setResult, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = structure.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setResult(f, f.ToPolyline(getPosition));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="structure"></param>
        /// <param name="getPosition"></param>
        /// <param name="setResult"></param>
        /// <param name="parallel"></param>
        public static void GetFaceCircumcircles<V, E, F>(this HeStructure<V, E, F> structure, Func<V, Vec3d> getPosition, Action<F, Circle> setResult, bool parallel = false)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = structure.Faces;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, faces.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var f = faces[i];
                    if (f.IsUnused) continue;
                    setResult(f, f.GetCircumcircle(getPosition));
                }
            }
        }
    }
}

#endif