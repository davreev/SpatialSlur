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
                if (n0.IsRemoved) n1.Remove(); // set removed status
            }

            // add all edges
            for (int i = 0; i < other.EdgeCount; i++)
            {
                Edge e0 = other._edges[i];
                Edge e1 = AddEdge(e0.Start.Index, e0.End.Index);
                if (e0.IsRemoved) e1.Remove(); // set removed status
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected List<Node> Nodes
        {
            get { return _nodes; }
        }


        /// <summary>
        /// 
        /// </summary>
        protected List<Edge> Edges
        {
            get { return _edges; }
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
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        public abstract IEnumerable<Node> GetConnectedNodes(int nodeIndex);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<Edge> GetIncidentEdges(int nodeIndex)
        {
            var edges = _nodes[nodeIndex].Edges;

            for (int i = 0; i < edges.Count; i++)
                yield return edges[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public abstract Edge FindEdge(int i, int j);


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
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public abstract Edge AddEdge(int i, int j);


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
            if (e == null) return false;

            e.Remove();
            return true;
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
