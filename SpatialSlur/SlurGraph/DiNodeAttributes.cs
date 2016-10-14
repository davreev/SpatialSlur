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
    public partial class DiNodeList
    {
        /// <summary>
        /// Gets the topological depth of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetNodeDepths(IEnumerable<DiNode> sources)
        {
            int[] result = new int[Count];
            GetNodeDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetNodeDepths(IEnumerable<DiNode> sources, IList<int> result)
        {
            SizeCheck(result);

            var queue = new Queue<DiNode>();
            result.Set(int.MaxValue);

            // set sources to zero and enqueue
            foreach (DiNode n in sources)
            {
                OwnsCheck(n);
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0;
            }

            GetNodeDepths(queue, result);
        }


        /// <summary>
        /// Gets the topological depth of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetNodeDepths(IEnumerable<int> sources)
        {
            int[] result = new int[Count];
            GetNodeDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetNodeDepths(IEnumerable<int> sources, IList<int> result)
        {
            SizeCheck(result);

            var queue = new Queue<DiNode>();
            result.Set(int.MaxValue);

            // set sources to zero and enqueue
            foreach (int ni in sources)
            {
                DiNode n = this[ni];
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0;
            }

            GetNodeDepths(queue, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetNodeDepths(Queue<DiNode> queue, IList<int> result)
        {
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
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public double[] GetNodeDistances(IEnumerable<DiNode> sources, IList<double> edgeWeights)
        {
            double[] result = new double[Count];
            GetNodeDistances(sources, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void GetNodeDistances(IEnumerable<DiNode> sources, IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Graph.Edges.SizeCheck(edgeWeights);

            var queue = new Queue<DiNode>();
            result.Set(Double.PositiveInfinity);

            // set sources to zero and enqueue
            foreach (DiNode n in sources)
            {
                OwnsCheck(n);
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0.0;
            }

            GetNodeDistances(queue, edgeWeights, result);
        }


        /// <summary>
        /// Gets the topological distance of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public double[] GetNodeDistances(IEnumerable<int> sources, IList<double> edgeWeights)
        {
            double[] result = new double[Count];
            GetNodeDistances(sources, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void GetNodeDistances(IEnumerable<int> sources, IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Graph.Edges.SizeCheck(edgeWeights);

            var queue = new Queue<DiNode>();
            result.Set(Double.PositiveInfinity);

            // set sources to zero and enqueue
            foreach (int ni in sources)
            {
                DiNode n = this[ni];
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0.0;
            }

            GetNodeDistances(queue, edgeWeights, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetNodeDistances(Queue<DiNode> queue, IList<double> edgeWeights, IList<double> result)
        {
            // TODO switch to pq implementation
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
        public double[] GetLaplacianOut(IList<double> nodeValues, bool parallel = false)
        {
            double[] result = new double[Count];
            GetLaplacianOut(nodeValues, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacianOut(IList<double> nodeValues, IList<double> result, bool parallel = false)
        {
            SizeCheck(nodeValues);
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacianOut(nodeValues, result, range.Item1, range.Item2));
            else
                GetLaplacianOut(nodeValues, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacianOut(IList<double> nodeValues, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var ni = this[i];
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
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetLaplacianOut2(IList<double> nodeValues, IList<double> edgeWeights, bool parallel = false)
        {
            double[] result = new double[Count];
            GetLaplacianOut2(nodeValues, edgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacianOut2(IList<double> nodeValues, IList<double> edgeWeights, IList<double> result, bool parallel = false)
        {
            SizeCheck(nodeValues);
            SizeCheck(result);
            Graph.Edges.SizeCheck(edgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacianOut2(nodeValues, edgeWeights, result, range.Item1, range.Item2));
            else
                GetLaplacianOut2(nodeValues, edgeWeights, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacianOut2(IList<double> nodeValues, IList<double> edgeWeights, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var ni = this[i];
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
