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
    /// Adjacency list implementation of a directed graph.
    /// </summary>
    public class DiGraph
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointPairs"></param>
        /// <param name="epsilon"></param>
        /// <param name="nodePositions"></param>
        /// <returns></returns>
        public static DiGraph CreateFromLineSegments(IList<Vec3d> pointPairs, double epsilon, bool allowDupEdges, out List<Vec3d> nodePositions)
        {
            int[] indexMap;
            nodePositions = Vec3d.RemoveDuplicates(pointPairs, epsilon, out indexMap);
            DiGraph result = new DiGraph(nodePositions.Count, pointPairs.Count >> 1);

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
                    if(!result.HasEdge(i0, i1)) result.AddEdge(i0, i1);
                }
            }

            return result;
        }


        private readonly List<DiNode> _nodes;
        private readonly List<DiEdge> _edges;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public DiGraph()
        {
            _nodes = new List<DiNode>();
            _edges = new List<DiEdge>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCount"></param>
        public DiGraph(int nodeCapacity, int edgeCapacity)
        {
            _nodes = new List<DiNode>(nodeCapacity);
            _edges = new List<DiEdge>(edgeCapacity);
        }


        /// <summary>
        /// Creates a deep copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        public DiGraph(DiGraph other)
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
                DiEdge e = otherEdges[i];
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
        public IEnumerable<DiNode> Nodes
        {
            get
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    DiNode n = _nodes[i];
                    if (!n.IsRemoved) yield return n;
                }
            }
        }


        /// <summary>
        /// Skips edges which have been flagged for removal.
        /// </summary>
        public IEnumerable<DiEdge> Edges
        {
            get
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    DiEdge e = _edges[i];
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
        public DiNode GetNode(int index)
        {
            return _nodes[index];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DiEdge GetEdge(int index)
        {
            return _edges[index];
        }


        /// <summary>
        /// Returns an edge spanning from i to j.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public DiEdge FindEdge(int i, int j)
        {
            return _nodes[i].FindEdgeTo(_nodes[j]);
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
        public DiNode AddNode()
        {
            DiNode n = new DiNode(_nodes.Count);
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
                _nodes.Add(new DiNode(_nodes.Count));
        }


        /// <summary>
        /// Adds a new edge from node i to node j.
        /// Note that if node i or j is flagged for removal, no new edge is added and null is returned.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public DiEdge AddEdge(int i, int j)
        {
            DiNode ni = _nodes[i];
            DiNode nj = _nodes[j];

            if (ni.IsRemoved || nj.IsRemoved)
                return null;

            DiEdge e = new DiEdge(ni, nj, _edges.Count);
            _edges.Add(e);

            ni.AddOutEdge(e);
            nj.AddInEdge(e);

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
                DiNode n = _nodes[i];
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
                DiEdge e = _edges[i];
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
            return String.Format("DiGraph (N:{0} E:{1})", _nodes.Count, _edges.Count);
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
        public void UpdateNodeDepths(IList<int> sources, IList<int> result)
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

                foreach (var nj in ni.ConnectedTo)
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

                foreach (var e in ni.OutgoingEdges)
                {
                    int j = e.End.Index;
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
        public double[] GetLaplacian(IList<double> nodeValues)
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
                    foreach (var nj in ni.ConnectedTo)
                        sum += nodeValues[nj.Index];

                    result[i] = sum / ni.OutDegree - nodeValues[i];
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
                    foreach (var e in ni.OutgoingEdges)
                        sum += (nodeValues[e.End.Index] - val) * edgeWeights[e.Index];

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
