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
    public static class GraphAttributes
    {
        #region Node Attributes

        /// <summary>
        ///  Gets the topological depth of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static int[] GetNodeDepths(this NodeList nodes, IEnumerable<Node> sources)
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
        public static void UpdateNodeDepths(this NodeList nodes, IEnumerable<Node> sources, IList<int> result)
        {
            nodes.SizeCheck(result);

            var queue = new Queue<Node>();
            result.Set(int.MaxValue);

            // set sources to zero and enqueue
            foreach (Node n in sources)
            {
                nodes.OwnsCheck(n);
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0;
            }

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
        /// <param name="nodes"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <returns></returns>
        public static double[] GetNodeDepths(this NodeList nodes, IEnumerable<Node> sources, IList<double> edgeLengths)
        {
            double[] result = new double[nodes.Count];
            nodes.UpdateNodeDepths(sources, edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        public static void UpdateNodeDepths(this NodeList nodes, IEnumerable<Node> sources, IList<double> edgeLengths, IList<double> result)
        {
            nodes.SizeCheck(result);
            nodes.Graph.Edges.SizeCheck(edgeLengths);

            var queue = new Queue<Node>();
            result.Set(Double.PositiveInfinity);

            // set sources to zero and enqueue
            foreach (Node n in sources)
            {
                nodes.OwnsCheck(n);
                if (n.IsUnused) continue;

                queue.Enqueue(n);
                result[n.Index] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                Node n0 = queue.Dequeue();
                double t0 = result[n0.Index];

                foreach (Edge e in n0.Edges)
                {
                    Node n1 = e.Other(n0);
                    int i1 = n1.Index;
                    double t1 = t0 + edgeLengths[e.Index];

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
        /// <param name="nodes"></param>
        /// <param name="nodeValues"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetLaplacian(this NodeList nodes, IList<double> nodeValues, bool parallel = false)
        {
            double[] result = new double[nodes.Count];
            nodes.UpdateLaplacian(nodeValues, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="nodeValues"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateLaplacian(this NodeList nodes, IList<double> nodeValues, IList<double> result, bool parallel = false)
        {
            nodes.SizeCheck(nodeValues);
            nodes.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, nodes.Count), range =>
                    nodes.UpdateLaplacian(nodeValues, result, range.Item1, range.Item2));
            else
                nodes.UpdateLaplacian(nodeValues, result, 0, nodes.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateLaplacian(this NodeList nodes, IList<double> nodeValues, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var n0 = nodes[i];
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
        /// <param name="nodes"></param>
        /// <param name="nodeValues"></param>
        /// <param name="edgeWeights"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetLaplacian(this NodeList nodes, IList<double> nodeValues, IList<double> edgeWeights, bool parallel = false)
        {
            double[] result = new double[nodes.Count];
            nodes.UpdateLaplacian(nodeValues, edgeWeights, result, parallel);
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
        public static void UpdateLaplacian(this NodeList nodes, IList<double> nodeValues, IList<double> edgeWeights, IList<double> result, bool parallel = false)
        {
            nodes.SizeCheck(nodeValues);
            nodes.SizeCheck(result);
            nodes.Graph.Edges.SizeCheck(edgeWeights);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, nodes.Count), range =>
                    nodes.UpdateLaplacian(nodeValues, edgeWeights, result, range.Item1, range.Item2));
            else
                nodes.UpdateLaplacian(nodeValues, edgeWeights, result, 0, nodes.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateLaplacian(this NodeList nodes, IList<double> nodeValues, IList<double> edgeWeights, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var n0 = nodes[i];
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

        #endregion


        #region EdgeAttributes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="nodePositions"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static double[] GetEdgeLengths(this EdgeList edges, IList<Vec3d> nodePositions, bool parallel = false)
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
        public static void UpdateEdgeLengths(this EdgeList edges, IList<Vec3d> nodePositions, IList<double> result, bool parallel = false)
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
        private static void UpdateEdgeLengths(this EdgeList edges, IList<Vec3d> nodePositions, IList<double> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                var e = edges[i];
                if (e.IsUnused) continue;
                result[i] = nodePositions[e.Start.Index].DistanceTo(nodePositions[e.End.Index]);
            }
        }

        #endregion


        #region DiNode Attributes

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
        /// <param name="edgeLengths"></param>
        /// <returns></returns>
        public static double[] GetNodeDepths(this DiNodeList nodes, IEnumerable<DiNode> sources, IList<double> edgeLengths)
        {
            double[] result = new double[nodes.Count];
            nodes.UpdateNodeDepths(sources, edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        public static void UpdateNodeDepths(this DiNodeList nodes, IEnumerable<DiNode> sources, IList<double> edgeLengths, IList<double> result)
        {
            nodes.SizeCheck(result);
            nodes.Graph.Edges.SizeCheck(edgeLengths);

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
                    double t1 = t0 + edgeLengths[e.Index];

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

        #endregion


        #region DiEdge Attributes

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

        #endregion
    }
}
