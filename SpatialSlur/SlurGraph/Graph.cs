using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// Adjacency list implementation of an undirected graph.
    /// </summary>
    public class Graph
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoints"></param>
        /// <param name="epsilon"></param>
        /// <param name="nodePositions"></param>
        /// <returns></returns>
        public static Graph CreateFromLineSegments(IList<Vec3d> endPoints, double epsilon, bool allowDupEdges, out List<Vec3d> nodePositions)
        {
            int[] indexMap;
            nodePositions = Vec3d.RemoveDuplicates(endPoints, epsilon, out indexMap);
            Graph result = new Graph(nodePositions.Count, endPoints.Count >> 1);

            // add nodes
            result.AddNodes(indexMap.Length);

            // add edges
            if (allowDupEdges)
            {
                for (int i = 0; i < indexMap.Length; i += 2)
                    result.AddEdge(indexMap[i], indexMap[i + 1]);
            }
            else
            {
                for (int i = 0; i < indexMap.Length; i += 2)
                {
                    int i0 = indexMap[i];
                    int i1 = indexMap[i + 1];
                    if (!result.HasEdge(i0, i1)) result.AddEdge(i0, i1);
                }
            }

            return result;
        }
   

        private readonly List<Node> _nodes;
        private readonly List<Edge> _edges;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public Graph()
        {
            _nodes = new List<Node>();
            _edges = new List<Edge>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCount"></param>
        public Graph(int nodeCapacity, int edgeCapacity)
        {
            _nodes = new List<Node>(nodeCapacity);
            _edges = new List<Edge>(edgeCapacity);
        }


        /// <summary>
        /// Creates a deep copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        public Graph(Graph other)
            : this(other.NodeCount, other.EdgeCount)
        {
            var otherNodes = other._nodes;
            var otherEdges = other._edges;
  
            // add all nodes
            for (int i = 0; i < otherNodes.Count; i++)
                AddNode();

            // add all edges
            for (int i = 0; i < otherEdges.Count; i++)
            {
                Edge e = otherEdges[i];
                AddEdge(e.Start.Index, e.End.Index);
            }
      
            // flag removed edges
            for (int i = 0; i < otherEdges.Count; i++)
                if (otherEdges[i].IsRemoved) _edges[i].Remove();

            // flag removed nodes
            for (int i = 0; i < otherNodes.Count; i++)
                if (otherNodes[i].IsRemoved) _nodes[i].Index = -1;
        }


        /// <summary>
        /// Skips nodes which have been flagged for removal.
        /// </summary>
        public IEnumerable<Node> Nodes
        {
            get
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    Node n = _nodes[i];
                    if (!n.IsRemoved) yield return n;
                }
            }
        }


        /// <summary>
        /// Skips edges which have been flagged for removal.
        /// </summary>
        public IEnumerable<Edge> Edges
        {
            get
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    Edge e = _edges[i];
                    if (!e.IsRemoved) yield return e;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int NodeCount
        {
            get { return _nodes.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int EdgeCount
        {
            get { return _edges.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Node GetNode(int index)
        {
            return _nodes[index];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Edge GetEdge(int index)
        {
            return _edges[index];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Edge FindEdge(int i, int j)
        {
            Node ni = _nodes[i];
            Node nj = _nodes[j];

            if (nj.Degree < ni.Degree) // search from the node with the smaller degree
                return nj.FindEdgeTo(nj);
            else
                return ni.FindEdgeTo(nj);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public bool HasEdge(int i, int j)
        {
            return FindEdge(i, j) != null;
        }


        /// <summary>
        /// 
        /// </summary>
        public Node AddNode()
        {
            Node n = new Node(_nodes.Count);
            _nodes.Add(n);
            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantity"></param>
        public void AddNodes(int quantity)
        {
            for (int i = 0; i < quantity; i++)
                _nodes.Add(new Node(_nodes.Count));
        }


        /// <summary>
        /// Adds a new edge between nodes i and j.
        /// Note that if node i or j is flagged for removal, no new edge is added and null is returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Edge AddEdge(int i, int j)
        {
            Node ni = _nodes[i];
            Node nj = _nodes[j];

            if (ni.IsRemoved || nj.IsRemoved)
                return null;

            Edge e = new Edge(ni, nj, _edges.Count);
            _edges.Add(e);

            ni.AddEdge(e);
            nj.AddEdge(e);

            return e;
        }


        /// <summary>
        /// Removes all flagged nodes and edges from the graph.
        /// </summary>
        public void Compact()
        {
            CompactNodes();
            CompactEdges();
        }


        /// <summary>
        /// Removes all flagged nodes.
        /// </summary>
        /// <param name="edges"></param>
        private void CompactNodes()
        {
            int marker = 0;
            for (int i = 0; i < _nodes.Count; i++)
            {
                Node n = _nodes[i];
                if (!n.IsRemoved)
                {
                    n.Compact(); // compact adjacency list
                    n.Index = marker;
                    _nodes[marker++] = n;
                }
            }

            _nodes.RemoveRange(marker, _nodes.Count - marker);
        }


        /// <summary>
        /// Removes all flagged edges.
        /// </summary>
        /// <param name="edges"></param>
        private void CompactEdges()
        {
            int marker = 0;
            for (int i = 0; i < _edges.Count; i++)
            {
                Edge e = _edges[i];
                if (!e.IsRemoved)
                {
                    e.Index = marker;
                    _edges[marker++] = e;
                }
            }

            _edges.RemoveRange(marker, _edges.Count - marker);
        }


        /// <summary>
        /// Removes all attributes corresponding with nodes which have been flagged for removal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributes"></param>
        public void CompactNodeAttributes<T>(List<T> attributes)
        {
            int marker = 0;
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (!_nodes[i].IsRemoved)
                    attributes[marker++] = attributes[i];
            }

            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// Removes all attributes corresponding with edges which have been flagged for removal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributes"></param>
        public void CompactEdgeAttributes<T>(List<T> attributes)
        {
            int marker = 0;
            for (int i = 0; i < _edges.Count; i++)
            {
                if (!_edges[i].IsRemoved)
                    attributes[marker++] = attributes[i];
            }

            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Graph (N:{0} E:{1})", _nodes.Count, _edges.Count);
        }


        /// <summary>
        /// Gets the topological depth of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns></returns>
        public int[] GetNodeDepths(IList<int> sources)
        {
            int[] result = new int[_nodes.Count];
            UpdateNodeDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void UpdateNodeDepths( IList<int> sources, IList<int> result)
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
                var ni = _nodes[i];
                int tj = result[i] + 1;

                foreach (var nj in ni.ConnectedNodes)
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
        /// Gets the topological distance of each node from a given set of source nodes via breadth first search.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <returns></returns>
        public double[] GetNodeDistances(IList<int> sources, IList<double> edgeLengths)
        {
            double[] result = new double[_nodes.Count];
            UpdateNodeDistances(sources, edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="sources"></param>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        public void UpdateNodeDistances(IList<int> sources, IList<double> edgeLengths, IList<double> result)
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
                var ni = _nodes[i];
                double ti = result[i];

                foreach (var e in ni.IncidentEdges)
                {
                    int j = e.Other(ni).Index;
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
        /// Computes the laplacian using a normalized umbrella weighting scheme.
        /// </summary>
        /// <returns></returns>
        public  double[] GetLaplacian(IList<double> nodeValues)
        {
            double[] result = new double[_nodes.Count];
            UpdateLaplacian(nodeValues, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<double> nodeValues, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, _nodes.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var ni = _nodes[i];
                    if (ni.IsRemoved) continue;

                    double sum = 0.0;
                    foreach (var nj in ni.ConnectedNodes)
                        sum += nodeValues[nj.Index];

                    result[i] = sum / ni.Degree - nodeValues[i];
                }
            });
        }


        /// <summary>
        /// Computes the laplacian using custom edge weights.
        /// </summary>
        /// <returns></returns>
        public double[] GetLaplacian(IList<double> nodeValues, IList<double> edgeWeights)
        {
            double[] result = new double[_nodes.Count];
            UpdateLaplacian(nodeValues, edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateLaplacian(IList<double> nodeValues, IList<double> edgeWeights, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, _nodes.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var ni = _nodes[i];
                    if (ni.IsRemoved) continue;

                    double val = nodeValues[i];
                    double sum = 0.0;
                    foreach (var e in ni.IncidentEdges)
                    {
                        Node nj = e.Other(ni);
                        sum += (nodeValues[nj.Index] - val) * edgeWeights[e.Index];
                    }

                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="nodeCoords"></param>
        /// <returns></returns>
        public double[] GetEdgeLengths(IList<Vec3d> nodeCoords)
        {
            double[] result = new double[_edges.Count];
            UpdateEdgeLengths(nodeCoords, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeCoords"></param>
        /// <param name="result"></param>
        public void UpdateEdgeLengths(IList<Vec3d> nodeCoords, IList<double> result)
        {
            Parallel.ForEach(Partitioner.Create(0, _edges.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var e = _edges[i];
                    if (e.IsRemoved) continue;
                    result[i] = nodeCoords[e.Start.Index].DistanceTo(nodeCoords[e.End.Index]);
                }
            });
        }
    }
}
