
/*
 * Notes
 */

#if USING_RHINO

using System.Collections.Concurrent;
using System.Threading.Tasks;

using Rhino.Geometry;
using SpatialSlur.SlurMesh;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class IHeStructureExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="xform"></param>
        /// <param name="parallel"></param>
        public static void Transform<V, E>(this HeStructure<V, E> graph, Transform xform, bool parallel = false)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            var verts = graph.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    v.Position = xform.ApplyToPoint(v.Position);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="graph"></param>
        /// <param name="xmorph"></param>
        /// <param name="parallel"></param>
        public static void SpaceMorph<V, E>(this HeStructure<V, E> graph, SpaceMorph xmorph, bool parallel = false)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            var verts = graph.Vertices;

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, verts.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, verts.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    v.Position = xmorph.MorphPoint(v.Position);
                }
            }
        }
    }
}

#endif