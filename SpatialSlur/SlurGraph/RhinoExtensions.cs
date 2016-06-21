using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// 
    /// </summary>
    public static class RhinoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Line[] GetEdgeLines(this EdgeList edges, IList<Vec3d> nodePositions, bool parallel = false)
        {
            Line[] result = new Line[edges.Count >> 1];
            edges.UpdateEdgeLines(nodePositions, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateEdgeLines(this EdgeList edges, IList<Vec3d> nodePositions, IList<Line> result, bool parallel = false)
        {
            edges.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count >> 1), range =>
                    edges.UpdateEdgeLines(nodePositions, result, range.Item1, range.Item2));
            else
                edges.UpdateEdgeLines(nodePositions, result, 0, edges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateEdgeLines(this EdgeList edges, IList<Vec3d> nodePositions, IList<Line> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var e = edges[i << 1];
                if (e.IsUnused) continue;

                Vec3d p0 = nodePositions[e.Start.Index];
                Vec3d p1 = nodePositions[e.End.Index];
                result[i] = new Line(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Line[] GetEdgeLines(this DiEdgeList edges, IList<Vec3d> nodePositions, bool parallel = false)
        {
            Line[] result = new Line[edges.Count >> 1];
            edges.UpdateEdgeLines(nodePositions, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateEdgeLines(this DiEdgeList edges, IList<Vec3d> nodePositions, IList<Line> result, bool parallel = false)
        {
            edges.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count >> 1), range =>
                    edges.UpdateEdgeLines(nodePositions, result, range.Item1, range.Item2));
            else
                edges.UpdateEdgeLines(nodePositions, result, 0, edges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateEdgeLines(this DiEdgeList edges, IList<Vec3d> nodePositions, IList<Line> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var e = edges[i << 1];
                if (e.IsUnused) continue;

                Vec3d p0 = nodePositions[e.Start.Index];
                Vec3d p1 = nodePositions[e.End.Index];
                result[i] = new Line(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
            }
        }
    }
}
