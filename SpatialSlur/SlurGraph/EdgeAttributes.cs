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
    public partial class EdgeList
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodePositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetEdgeLengths(IList<Vec3d> nodePositions, bool parallel = false)
        {
            var result = new double[Count];
            GetEdgeLengths(nodePositions, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodePositions"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetEdgeLengths(IList<Vec3d> nodePositions, IList<double> result, bool parallel = false)
        {
            SizeCheck(result);
            Graph.Nodes.SizeCheck(nodePositions);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetEdgeLengths(nodePositions, result, range.Item1, range.Item2));
            else
                GetEdgeLengths(nodePositions, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetEdgeLengths(IList<Vec3d> nodePositions, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var e = this[i];
                if (e.IsUnused) continue;
                result[i] = nodePositions[e.Start.Index].DistanceTo(nodePositions[e.End.Index]);
            }
        }
    }
}
