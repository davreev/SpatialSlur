using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * 
 */ 

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// 
    /// </summary>
    public static class DiEdgeAttributes
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetEdgeLengths(this DiEdgeList edges, IList<Vec3d> nodePositions, bool parallel = false)
        {
            double[] result = new double[edges.Count];
            edges.UpdateEdgeLengths(nodePositions, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateEdgeLengths(this DiEdgeList edges, IList<Vec3d> nodePositions, IList<double> result, bool parallel = false)
        {
            edges.SizeCheck(result);
            edges.Graph.Nodes.SizeCheck(nodePositions);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, edges.Count), range =>
                    edges.UpdateEdgeLengths(nodePositions, result, range.Item1, range.Item2));
            else
                edges.UpdateEdgeLengths(nodePositions, result, 0, edges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateEdgeLengths(this DiEdgeList edges, IList<Vec3d> nodePositions, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var e = edges[i];
                if (e.IsUnused) continue;
                result[i] = nodePositions[e.Start.Index].DistanceTo(nodePositions[e.End.Index]);
            }
        }
    }
}
