using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    public abstract class GraphBase : IGraph
    {
        private readonly List<Node> _nodes;
        private readonly List<Edge> _edges;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        protected GraphBase()
        {
            _nodes = new List<Node>();
            _edges = new List<Edge>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCount"></param>
        protected GraphBase(int nodeCapacity, int edgeCapacity)
        {
            _nodes = new List<Node>(nodeCapacity);
            _edges = new List<Edge>(edgeCapacity);
        }


        /// <summary>
        /// Creates a copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        protected GraphBase(GraphBase other)
            : this(other.NodeCount, other.EdgeCount)
        {
            // add all nodes
            for (int i = 0; i < other.NodeCount; i++)
            {
                Node n0 = other._nodes[i];
                Node n1 = AddNode();
                n1.Index = n0.Index; // set removed status
            }

            // add all edges
            for (int i = 0; i < other.EdgeCount; i++)
            {
                Edge e0 = other._edges[i];
                Edge e1 = AddEdge(e0.Start.Index, e0.End.Index);
                if (e0.IsRemoved) RemoveEdge(e1);
            }
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
        /// Skips nodes which have been flagged for removal.
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        public IEnumerable<Node> GetConnectedNodes(int nodeIndex)
        {
            return GetConnectedNodes(_nodes[nodeIndex]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected abstract IEnumerable<Node> GetConnectedNodes(Node node);


        /// <summary>
        /// Skips edges which have been flagged for removal.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<Edge> GetIncidentEdges(int nodeIndex)
        {
            return GetIncidentEdges(_nodes[nodeIndex]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<Edge> GetIncidentEdges(Node node)
        {
            return node.IncidentEdges;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Edge FindEdge(int i, int j)
        {
            return FindEdge(_nodes[i], _nodes[j]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected abstract Edge FindEdge(Node ni, Node nj);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public bool HasEdge(int i, int j)
        {
            return FindEdge(_nodes[i], _nodes[j]) != null;
        }


        /// <summary>
        /// 
        /// </summary>
        public Node AddNode()
        {
            Node v = new Node(NodeCount);
            _nodes.Add(v);

            return v;
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
            RemoveNode(_nodes[index]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void RemoveNode(Node node)
        {
            if (node.IsRemoved) return; // exit if already removed
            node.Remove(); // flag for removal

            foreach (Edge e in node.IncidentEdges)
                RemoveEdge(e);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Edge AddEdge(int i, int j)
        {
            return AddEdge(_nodes[i], _nodes[j]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ni"></param>
        /// <param name="nj"></param>
        /// <returns></returns>
        private Edge AddEdge(Node ni, Node nj)
        {
            Edge e = new Edge(ni, nj, EdgeCount);
            _edges.Add(e);

            AddEdge(e);
            return e;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        protected abstract void AddEdge(Edge edge);
   

        /// <summary>
        /// Flags an edge between nodes i and j for removal.
        /// Returns false if no edge exists.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="vj"></param>
        /// <returns></returns>
        public bool RemoveEdge(int i, int j)
        {
            return RemoveEdge(_nodes[i], _nodes[j]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ni"></param>
        /// <param name="nj"></param>
        /// <returns></returns>
        private bool RemoveEdge(Node ni, Node nj)
        {
            Edge e = FindEdge(ni, nj);
            if (e == null) return false;

            RemoveEdge(e);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeIndex"></param>
        public void RemoveEdge(int edgeIndex)
        {
            RemoveEdge(_edges[edgeIndex]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        protected abstract void RemoveEdge(Edge edge);


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
