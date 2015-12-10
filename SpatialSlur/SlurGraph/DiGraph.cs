using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// Adjacency list implementation of a directed graph.
    /// </summary>
    public class DiGraph
    {
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
            return String.Format("EdgeGraph (N:{0} E:{1})", _nodes.Count, _edges.Count);
        }
    }
}
