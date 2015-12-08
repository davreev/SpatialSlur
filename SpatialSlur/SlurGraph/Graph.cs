using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// Undirected graph class.
    /// </summary>
    public class Graph : IGraph
    {
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
        /// Creates a copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        public Graph(Graph other)
            : this(other.NodeCount, other.EdgeCount)
        {
            var otherNodes = other._nodes;
            var otherEdges = other._edges;

            // add all nodes (don't flag removed nodes yet as it will interfere with the creation of edges below)
            for (int i = 0; i < otherNodes.Count; i++)
                AddNode();

            // add all edges
            for (int i = 0; i < otherEdges.Count; i++)
            {
                Edge e0 = otherEdges[i];
                Edge e1 = AddEdge(e0.Start.Index, e0.End.Index); // returns a null if nodes are flagged for removal
                if (e0.IsRemoved) e1.Remove();
            }

            // flag nodes for removal after creating edges
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
        /// Skips edges which have been flagged for removal.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<Edge> GetIncidentEdges(int nodeIndex)
        {
            return _nodes[nodeIndex].IncidentEdges;
        }


        /// <summary>
        /// Skips nodes which have been flagged for removal.
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        public IEnumerable<Node> GetConnectedNodes(int nodeIndex)
        {
            return _nodes[nodeIndex].ConnectedNodes;
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
            Node n = new Node(NodeCount);
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
                _nodes.Add(new Node(NodeCount));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveNode(int index)
        {
            _nodes[index].Remove();
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

            Edge e = new Edge(ni, nj, EdgeCount);
            _edges.Add(e);
            ni.AddEdge(e);
            nj.AddEdge(e);

            return e;
        }


        /// <summary>
        /// Flags an edge between nodes i and j for removal.
        /// Returns false if no edge exists.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="vj"></param>
        /// <returns></returns>
        public bool RemoveEdge(int i, int j)
        {
            Edge e = FindEdge(i, j);
            if (e == null) 
                return false;

            e.Remove();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeIndex"></param>
        public void RemoveEdge(int edgeIndex)
        {
            _edges[edgeIndex].Remove();
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
                Node v = _nodes[i];
                if (!v.IsRemoved)
                {
                    v.Compact(); // compact adjacency list
                    v.Index = marker;
                    _nodes[marker++] = v;
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
            return String.Format("EdgeGraph (N:{0} E:{1})", _nodes.Count, _edges.Count);
        }
    }
}
