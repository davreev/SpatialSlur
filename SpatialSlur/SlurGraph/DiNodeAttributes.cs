using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// 
    /// </summary>
    public static class DiNodeAttributes
    {
        /// <summary>
        /// Gets the topological depth of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static int[] GetNodeDepths(this DiNodeList nodes, IEnumerable<DiNode> sources)
        {
            int[] result = new int[nodes.Count];
            nodes.UpdateNodeDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public static void UpdateNodeDepths(this DiNodeList nodes, IEnumerable<DiNode> sources, IList<int> result)
        {
            nodes.SizeCheck(result);

            var queue = new Queue<DiNode>();
            result.Set(int.MaxValue);

            // set sources to zero and enqueue
            foreach (DiNode n in sources)
            {
                nodes.OwnsCheck(n);
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                DiNode n0 = queue.Dequeue();
                int t0 = result[n0.Index] + 1;

                foreach (DiNode n1 in n0.OutNeighbours)
                {
                    int i1 = n1.Index;

                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        queue.Enqueue(n1);
                    }
                }
            }
        }


        /// <summary>
        /// Gets the topological distance of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public static double[] GetNodeDistances(this DiNodeList nodes, IEnumerable<DiNode> sources, IList<double> edgeWeights)
        {
            double[] result = new double[nodes.Count];
            nodes.UpdateNodeDistances(sources, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public static void UpdateNodeDistances(this DiNodeList nodes, IEnumerable<DiNode> sources, IList<double> edgeWeights, IList<double> result)
        {
            // TODO switch to pq implementation
            nodes.SizeCheck(result);
            nodes.Graph.Edges.SizeCheck(edgeWeights);

            var queue = new Queue<DiNode>();
            result.Set(Double.PositiveInfinity);

            // set sources to zero and enqueue
            foreach (DiNode n in sources)
            {
                nodes.OwnsCheck(n);
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                DiNode n0 = queue.Dequeue();
                double t0 = result[n0.Index];

                foreach (DiEdge e in n0.OutEdges)
                {
                    DiNode n1 = e.End;
                    int i1 = n1.Index;
                    double t1 = t0 + edgeWeights[e.Index];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(n1);
                    }
                }
            }
        }


        /// <summary>
        /// Computes the Laplacian using a normalized umbrella weighting scheme.
        /// </summary>
        /// <returns></returns>
        public static double[] GetLaplacianOut(this DiNodeList nodes, IList<double> nodeValues, bool parallel = false)
        {
            double[] result = new double[nodes.Count];
            nodes.UpdateLaplacianOut(nodeValues, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="nodeValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateLaplacianOut(this DiNodeList nodes, IList<double> nodeValues, IList<double> result, bool parallel = false)
        {
            nodes.SizeCheck(nodeValues);
            nodes.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, nodes.Count), range =>
                    nodes.UpdateLaplacianOut(nodeValues, result, range.Item1, range.Item2));
            else
                nodes.UpdateLaplacianOut(nodeValues, result, 0, nodes.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateLaplacianOut(this DiNodeList nodes, IList<double> nodeValues, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var ni = nodes[i];
                if (ni.IsUnused) continue;

                double sum = 0.0;
                foreach (var nj in ni.OutNeighbours)
                    sum += nodeValues[nj.Index];

                result[i] = sum / ni.OutDegree - nodeValues[i];
            }
        }


        /// <summary>
        /// Computes the Laplacian using a given set of edge weights.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetLaplacianOut(this DiNodeList nodes, IList<double> nodeValues, IList<double> edgeWeights, bool parallel = false)
        {
            double[] result = new double[nodes.Count];
            nodes.UpdateLaplacianOut(nodeValues, edgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateLaplacianOut(this DiNodeList nodes, IList<double> nodeValues, IList<double> edgeWeights, IList<double> result, bool parallel = false)
        {
            nodes.SizeCheck(nodeValues);
            nodes.SizeCheck(result);
            nodes.Graph.Edges.SizeCheck(edgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, nodes.Count), range =>
                    nodes.UpdateLaplacianOut(nodeValues, edgeWeights, result, range.Item1, range.Item2));
            else
                nodes.UpdateLaplacianOut(nodeValues, edgeWeights, result, 0, nodes.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateLaplacianOut(this DiNodeList nodes, IList<double> nodeValues, IList<double> edgeWeights, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var ni = nodes[i];
                if (ni.IsUnused) continue;

                double val = nodeValues[i];
                double sum = 0.0;
                foreach (var e in ni.OutEdges)
                    sum += (nodeValues[e.End.Index] - val) * edgeWeights[e.Index];

                result[i] = sum;
            }
        }
    }
}
