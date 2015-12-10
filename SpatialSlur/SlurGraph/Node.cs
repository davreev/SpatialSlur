using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// TODO make generic for attaching attributes
    /// </summary>
    public class Node
    {
        private readonly List<Edge> _edges;
        //private N _data;
        private int _index = -1;
        private int _degree; // explicitly store degree to keep up with edge/node removal
  

        /// <summary>
        /// 
        /// </summary>
        internal Node(int index)
        {
            _edges = new List<Edge>();
            _index = index;
        }


        /// <summary>
        /// Skips edges which have been flagged for removal.
        /// </summary>
        public IEnumerable<Edge> IncidentEdges
        {
            get
            {
                for(int i = 0; i < _edges.Count; i++)
                {
                    Edge e = _edges[i];
                    if (!e.IsRemoved) yield return e;
                }
            }
        }


        /// <summary>
        /// Skips nodes which have been flagged for removal.
        /// </summary>
        public IEnumerable<Node> ConnectedNodes
        {
            get
            {
                for (int i = 0; i < _edges.Count; i++)
                {
                    Edge e = _edges[i];
                    Node n = e.GetOther(this);
                    if (!e.IsRemoved && !n.IsRemoved) yield return n;
                }
            }
        }


        /// <summary>
        /// Returns the number of edges incident to this node.
        /// </summary>
        public int Degree
        {
            get { return _degree; }
            internal set { _degree = value; }
        }


        /// <summary>
        /// Returns the index of the node within the graph's node list.
        /// This will be set to -1 if the node is removed.
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }


        /// <summary>
        /// Returns true if this node has been flagged for removal.
        /// </summary>
        public bool IsRemoved
        {
            get { return _index == -1; }
        }


        /// <summary>
        /// Flags the node and all its incident edges for removal.
        /// </summary>
        public void Remove()
        {
            if (IsRemoved) return; // check if already flagged
            _index = -1; 

            for (int i = 0; i < _edges.Count; i++)
                _edges[i].Remove();
        }


        /// <summary>
        /// Removes all flagged edges from the edge list.
        /// </summary>
        internal void Compact()
        {
            int marker = 0;

            for (int i = 0; i < _edges.Count; i++)
            {
                Edge e = _edges[i];
                if (!e.IsRemoved)
                    _edges[marker++] = e;
            }

            _edges.RemoveRange(marker, _edges.Count - marker); // trim list to include only used elements
        }


        /// <summary>
        /// Returns true if an edge exists between this node and the given node.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsConnectedTo(Node other)
        {
            return FindEdgeTo(other) != null;
        }


        /// <summary>
        /// Searched for an edge between this node and the given node.
        /// Returns null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Edge FindEdgeTo(Node other)
        {
            for(int i = 0; i < _edges.Count; i++)
            {
                Edge e = _edges[i];
                if (e.GetOther(this) == other && !e.IsRemoved)
                    return e;
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        internal void AddEdge(Edge edge)
        {
            _edges.Add(edge);
            _degree++;
        }
    }
}
