using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes 
 */ 

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NodeList
    {
        /// <summary>
        ///  Gets the topological depth of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetNodeDepths(IEnumerable<Node> sources)
        {
            var result = new int[Count];
            GetNodeDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetNodeDepths(IEnumerable<Node> sources, IList<int> result)
        {
            SizeCheck(result);

            var queue = new Queue<Node>();
            result.Set(int.MaxValue);

            // set sources to zero and enqueue
            foreach (Node n in sources)
            {
                OwnsCheck(n);
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0;
            }

            GetNodeDepths(queue, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetNodeDepths(IEnumerable<int> sources)
        {
            var result = new int[Count];
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

            var queue = new Queue<Node>();
            result.Set(int.MaxValue);

            // set sources to zero and enqueue
            foreach (int ni in sources)
            {
                Node n = this[ni];
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0;
            }

            GetNodeDepths(queue, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetNodeDepths(Queue<Node> queue, IList<int> result)
        {
            // breadth first search from sources
            while (queue.Count > 0)
            {
                Node n0 = queue.Dequeue();
                int t0 = result[n0.Index] + 1;

                foreach (Node n1 in n0.Neighbours)
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
        public double[] GetNodeDistances(IEnumerable<Node> sources, IList<double> edgeWeights)
        {
            var result = new double[Count];
            GetNodeDistances(sources, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void GetNodeDistances(IEnumerable<Node> sources, IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Graph.Edges.SizeCheck(edgeWeights);

            var queue = new Queue<Node>();
            result.Set(Double.PositiveInfinity);

            // set sources to zero and enqueue
            foreach (Node n in sources)
            {
                OwnsCheck(n);
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0.0;
            }

            GetNodeDistances(queue, edgeWeights, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetNodeDistances(Queue<Node> queue, IList<double> edgeWeights, IList<double> result)
        {
            // TODO switch to pq implementation
            // breadth first search from sources
            while (queue.Count > 0)
            {
                Node n0 = queue.Dequeue();
                double t0 = result[n0.Index];

                foreach (Edge e in n0.Edges)
                {
                    Node n1 = e.Other(n0);
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
        /// <param name="nodeValues"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetLaplacian(IList<double> nodeValues, bool parallel = false)
        {
            var result = new double[Count];
            GetLaplacian(nodeValues, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IList<double> nodeValues, IList<double> result, bool parallel = false)
        {
            SizeCheck(nodeValues);
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacian(nodeValues, result, range.Item1, range.Item2));
            else
                GetLaplacian(nodeValues, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacian(IList<double> nodeValues, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var n0 = this[i];
                if (n0.IsUnused) continue;

                double sum = 0.0;
                foreach (var n1 in n0.Neighbours)
                    sum += nodeValues[n1.Index];

                result[i] = sum / n0.Degree - nodeValues[i];
            }
        }


        /// <summary>
        /// Computes the Laplacian using a given set of edge weights.
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public double[] GetLaplacian2(IList<double> nodeValues, IList<double> edgeWeights, bool parallel = false)
        {
            var result = new double[Count];
            GetLaplacian2(nodeValues, edgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian2(IList<double> nodeValues, IList<double> edgeWeights, IList<double> result, bool parallel = false)
        {
            SizeCheck(nodeValues);
            SizeCheck(result);
            Graph.Edges.SizeCheck(edgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacian2(nodeValues, edgeWeights, result, range.Item1, range.Item2));
            else
                GetLaplacian2(nodeValues, edgeWeights, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacian2(IList<double> nodeValues, IList<double> edgeWeights, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var n0 = this[i];
                if (n0.IsUnused) continue;

                double val = nodeValues[i];
                double sum = 0.0;
                foreach (var e in n0.Edges)
                {
                    Node n1 = e.Other(n0);
                    sum += (nodeValues[n1.Index] - val) * edgeWeights[e.Index];
                }

                result[i] = sum;
            }
        }


        /// <summary>
        /// Computes the Laplacian using a normalized umbrella weighting scheme.
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetLaplacian(IList<Vec3d> nodeValues, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetLaplacian(nodeValues, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IList<Vec3d> nodeValues, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(nodeValues);
            SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacian(nodeValues, result, range.Item1, range.Item2));
            else
                GetLaplacian(nodeValues, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacian(IList<Vec3d> nodeValues, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var n0 = this[i];
                if (n0.IsUnused) continue;

                Vec3d sum = new Vec3d();
                foreach (var n1 in n0.Neighbours)
                    sum += nodeValues[n1.Index];

                result[i] = sum / n0.Degree - nodeValues[i];
            }
        }


        /// <summary>
        /// Computes the Laplacian using a given set of edge weights.
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetLaplacian2(IList<Vec3d> nodeValues, IList<double> edgeWeights, bool parallel = false)
        {
            var result = new Vec3d[Count];
            GetLaplacian2(nodeValues, edgeWeights, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian2(IList<Vec3d> nodeValues, IList<double> edgeWeights, IList<Vec3d> result, bool parallel = false)
        {
            SizeCheck(nodeValues);
            SizeCheck(result);
            Graph.Edges.SizeCheck(edgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                    GetLaplacian2(nodeValues, edgeWeights, result, range.Item1, range.Item2));
            else
                GetLaplacian2(nodeValues, edgeWeights, result, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetLaplacian2(IList<Vec3d> nodeValues, IList<double> edgeWeights, IList<Vec3d> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var n0 = this[i];
                if (n0.IsUnused) continue;

                Vec3d val = nodeValues[i];
                Vec3d sum = new Vec3d();
                foreach (var e in n0.Edges)
                {
                    Node n1 = e.Other(n0);
                    sum += (nodeValues[n1.Index] - val) * edgeWeights[e.Index];
                }

                result[i] = sum;
            }
        }
    }
}
