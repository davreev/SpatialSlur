using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// 
    /// </summary>
    public class DiNode
    {
        private readonly List<DiEdge> _outEdges;
        private readonly List<DiEdge> _inEdges;
        //private N _data;
        private int _index = -1;
        private int _outDegree; // explicitly store degree to keep up with edge/node removal
        private int _inDegree;


        /// <summary>
        /// 
        /// </summary>
        internal DiNode(int index)
        {
            _outEdges = new List<DiEdge>();
            _inEdges = new List<DiEdge>();
            _index = index;
        }


        /// <summary>
        /// Iterates over all nodes that this node connects to (i.e. those at the end of outgoing edges).
        /// Skips nodes which have been flagged for removal.
        /// </summary>
        public IEnumerable<DiNode> ConnectedTo
        {
            get
            {
                for (int i = 0; i < _outEdges.Count; i++)
                {
                    DiEdge e = _outEdges[i];
                    if (!e.IsRemoved) yield return e.End;
                }
            }
        }


        /// <summary>
        /// Iterates over all nodes that connect to this node (i.e. those at the start of incoming edges).
        /// Skips nodes which have been flagged for removal.
        /// </summary>
        public IEnumerable<DiNode> ConnectedFrom
        {
            get
            {
                for (int i = 0; i < _inEdges.Count; i++)
                {
                    DiEdge e = _inEdges[i];
                    if (!e.IsRemoved) yield return e.Start;
                }
            }
        }


        /// <summary>
        /// Iterates edges starting at this node.
        /// Skips edges which have been flagged for removal.
        /// </summary>
        public IEnumerable<DiEdge> OutgoingEdges
        {
            get
            {
                for (int i = 0; i < _outEdges.Count; i++)
                {
                    DiEdge e = _outEdges[i];
                    if (!e.IsRemoved) yield return e;
                }
            }
        }


        /// <summary>
        /// Iterates edges starting at this node.
        /// Skips edges which have been flagged for removal.
        /// </summary>
        public IEnumerable<DiEdge> IncomingEdges
        {
            get
            {
                for (int i = 0; i < _inEdges.Count; i++)
                {
                    DiEdge e = _inEdges[i];
                    if (!e.IsRemoved) yield return e;
                }
            }
        }


        /// <summary>
        /// Returns the number of edges starting at this node.
        /// </summary>
        public int OutDegree
        {
            get { return _outDegree; }
            internal set { _outDegree = value; }
        }



        /// <summary>
        /// Returns the number of edges ending at this node.
        /// </summary>
        public int InDegree
        {
            get { return _inDegree; }
            internal set { _inDegree = value; }
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
        /// Flags the node and all incoming and outgoing edges for removal.
        /// </summary>
        public void Remove()
        {
            if (IsRemoved) return; // check if already flagged
            _index = -1;

            for (int i = 0; i < _outEdges.Count; i++)
                _outEdges[i].Remove();

            for (int i = 0; i < _inEdges.Count; i++)
                _inEdges[i].Remove();
        }


        /// <summary>
        /// Removes all flagged edges from incoming and outgoing edges lists.
        /// </summary>
        internal void Compact()
        {
            Compact(_outEdges);
            Compact(_inEdges);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        private void Compact(List<DiEdge> edges)
        {
            int marker = 0;

            for (int i = 0; i < edges.Count; i++)
            {
                DiEdge e = edges[i];
                if (!e.IsRemoved)
                    edges[marker++] = e;
            }

            edges.RemoveRange(marker, edges.Count - marker); // trim list to include only used elements
        }


        /// <summary>
        /// Returns true if there is an edge connecting this node to another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsConnectedTo(DiNode other)
        {
            return FindEdgeTo(other) != null;
        }


        /// <summary>
        /// Searched for an edge from this node no another.
        /// Returns null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public DiEdge FindEdgeTo(DiNode other)
        {
            for (int i = 0; i < _outEdges.Count; i++)
            {
                DiEdge e = _outEdges[i];
                if (e.End == other && !e.IsRemoved)
                    return e;
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        internal void AddOutEdge(DiEdge edge)
        {
            _outEdges.Add(edge);
            _outDegree++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        internal void AddInEdge(DiEdge edge)
        {
            _inEdges.Add(edge);
            _inDegree++;
        }
    }
}
