using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Node : GraphElement
    {
        private Edge[] _edges;
        private int _n;
        private int _degree; // explicitly store degree to keep up with edge removal


        /// <summary>
        /// 
        /// </summary>
        internal Node(int edgeCapacity = 2)
        {
            _edges = new Edge[edgeCapacity];
        }


        /// <summary>
        /// Iterates over nodes connected to this one (i.e. those at the opposite end of incident edges).
        /// Skips nodes connected by unused edges.
        /// </summary>
        public IEnumerable<Node> Neighbours
        {
            get
            {
                for (int i = 0; i < _n; i++)
                {
                    Edge e = _edges[i];
                    if (!e.IsUnused) yield return e.Other(this);
                }
            }
        }


        /// <summary>
        /// Iterates over edges incident to this node.
        /// Skips unused edges.
        /// </summary>
        public IEnumerable<Edge> Edges
        {
            get
            {
                for (int i = 0; i < _n; i++)
                {
                    Edge e = _edges[i];
                    if (!e.IsUnused) yield return e;
                }
            }
        }


        /// <summary>
        /// Returns the number of used edges incident to this node.
        /// </summary>
        public int Degree
        {
            get { return _degree; }
            internal set { _degree = value; }
        }


        /// <summary>
        /// Returns the number of edges incident to this node.
        /// Note that this includes unused edges.
        /// </summary>
        public int EdgeCount
        {
            get { return _n; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int EdgeCapacity
        {
            get { return _edges.Length; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsUnused
        {
            get { return _degree == 0; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void OnRemove()
        {
            // remove edges (flags node as unused)
            for(int i = 0; i < _n; i++)
            {
                Edge e = _edges[i];
                if (!e.IsUnused) e.OnRemove();
            }

            _n = 0; // reset edge list
        }
   

        /// <summary>
        /// Removes all flagged edges from the edge list.
        /// </summary>
        public void Compact()
        {
            int marker = 0;

            for (int i = 0; i < _n; i++)
            {
                Edge e = _edges[i];
                if (!e.IsUnused)
                    _edges[marker++] = e;
            }

            _n = marker;

            // trim array if length is greater than twice _n
            int maxLength = Math.Max(_n << 1, 2);
            if (_edges.Length > maxLength)
                Array.Resize(ref _edges, maxLength);

            // prevent object loitering
            Array.Clear(_edges, _n, _edges.Length - _n);
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
            for (int i = 0; i < _n; i++)
            {
                Edge e = _edges[i];
                if (!e.IsUnused && e.Other(this) == other)
                    return e;
            }

            return null;
        }


        /// <summary>
        /// Returns the incident edge at the given index.
        /// Note that this may return unused edges.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Edge EdgeAt(int index)
        {
            return _edges[index];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        internal void AddEdge(Edge edge)
        {
            // resize if necessary
            if (_n == _edges.Length)
                Array.Resize(ref _edges, _edges.Length << 1);

            _edges[_n++] = edge;
            _degree++;
        }
    }
}
