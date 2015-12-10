using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurGraph
{
    public static class GraphUtil
    {
      /*
      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="pointPairs"></param>
      /// <param name="epsilon"></param>
      /// <param name="nodePositions"></param>
      /// <returns></returns>
      public static T CreateFromLineSegments<T>(IList<Vec3d> pointPairs, double epsilon, out List<Vec3d> nodePositions)
      {
          throw new NotImplementedException();

          int[] indexMap;
          nodePositions = Vec3d.RemoveDuplicates(pointPairs, epsilon, out indexMap);

          //T result = new T(nodePositions.Count);
          T result; // TODO cannot construct with arguments?

          for (int i = 0; i < indexMap.Length; i += 2)
              result.AddEdge(indexMap[i], indexMap[i + 1]);

          return result;
      }
      */

        /// <summary>
        /// Computes the graph laplacian using a normalized umbrella weighting scheme (Tutte scheme).
        /// </summary>
        /// <returns></returns>
        public static double[] GetLaplacian(Graph graph, IList<double> vertexValues)
        {
            double[] result = new double[graph.NodeCount];
            UpdateLaplacian(graph, vertexValues, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public static void UpdateLaplacian(Graph graph, IList<double> nodeValues, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, graph.NodeCount), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Node ni = graph.GetNode(i);
                    if (ni.IsRemoved) continue;

                    double sum = 0.0;

                    foreach (Node nj in ni.ConnectedNodes)
                        sum += nodeValues[nj.Index];
                  
                    result[i] = sum / ni.Degree - nodeValues[i];
                }
            });
        }


        /// <summary>
        /// Computes the graph laplacian using custom edge weights.
        /// </summary>
        /// <returns></returns>
        public static double[] GetLaplacian(Graph graph, IList<double> nodeValues, IList<double> edgeWeights)
        {
            double[] result = new double[graph.NodeCount];
            UpdateLaplacian(graph, nodeValues, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public static void UpdateLaplacian(Graph graph, IList<double> nodeValues, IList<double> edgeWeights, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, graph.NodeCount), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Node ni = graph.GetNode(i);
                    if (ni.IsRemoved) continue;

                    double val = nodeValues[i];
                    double sum = 0.0;

                    foreach (Edge e in ni.IncidentEdges)
                    {
                        Node nj = e.GetOther(ni);
                        sum += (nodeValues[nj.Index] - val) * edgeWeights[e.Index];
                    }
            
                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// Gets the minimum topological depth of each node from a given set of source nodes.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static int[] GetNodeDepths(Graph graph, IList<int> sources)
        {
            int[] result = new int[graph.NodeCount];
            UpdateNodeDepths(graph, sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public static void UpdateNodeDepths(Graph graph, IList<int> sources, IList<int> result)
        {
            // set values to infinity
            result.Set(int.MaxValue);

            // set sources to zero and enqueue
            Queue<int> queue = new Queue<int>();
            foreach (int i in sources)
            {
                result[i] = 0;
                queue.Enqueue(i);
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                int i = queue.Dequeue();
                Node ni = graph.GetNode(i);
                int tj = result[i] + 1;

                foreach (Node nj in ni.ConnectedNodes)
                {
                    int j = nj.Index;

                    if (tj < result[j])
                    {
                        result[j] = tj;
                        queue.Enqueue(j);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <returns></returns>
        public static double[] GetNodeDistances(Graph graph, IList<int> sources, IList<double> edgeLengths)
        {
            double[] result = new double[graph.NodeCount];
            UpdateNodeDistances(graph, sources, edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        public static void UpdateNodeDistances(Graph graph, IList<int> sources, IList<double> edgeLengths, IList<double> result)
        {
            result.Set(Double.PositiveInfinity);

            // set sources to zero and enqueue
            Queue<int> queue = new Queue<int>();
            foreach (int i in sources)
            {
                result[i] = 0.0;
                queue.Enqueue(i);
            }

            // conduct distance-aware breadth first search
            while (queue.Count > 0)
            {
                int i = queue.Dequeue();
                Node ni = graph.GetNode(i);
                double ti = result[i];
         
                foreach (Edge e in graph.GetNode(i).IncidentEdges)
                {
                    int j =  e.GetOther(ni).Index;
                    double tj = ti + edgeLengths[e.Index];

                    if (tj < result[j])
                    {
                        result[j] = tj;
                        queue.Enqueue(j);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="nodeCoords"></param>
        /// <returns></returns>
        public static double[] GetEdgeLengths(Graph graph, IList<Vec3d> nodeCoords)
        {
            double[] result = new double[graph.EdgeCount];
            UpdateEdgeLengths(graph, nodeCoords, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeCoords"></param>
        /// <param name="result"></param>
        public static void UpdateEdgeLengths(Graph graph, IList<Vec3d> nodeCoords, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, graph.EdgeCount), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Edge e = graph.GetEdge(i);
                    if (e.IsRemoved) continue;
                    result[i] = nodeCoords[e.Start.Index].DistanceTo(nodeCoords[e.End.Index]);
                }
            });
        }
    }
}
