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
    public class DiNode : GraphElement
    {
        private DiEdge[] _inEdges, _outEdges;
        private int _inCount, _outCount;
        private int _inDegree, _outDegree; // explicitly store degree to keep up with edge removal
    
  
        /// <summary>
        /// 
        /// </summary>
        internal DiNode(int inCapacity, int outCapacity)
        {
            _inEdges = new DiEdge[inCapacity];
            _outEdges = new DiEdge[outCapacity];
        }


        /// <summary>
        /// Iterates over all nodes that connect to this node (i.e. those at the start of incoming edges).
        /// Skips nodes connected by unused edges.
        /// </summary>
        public IEnumerable<DiNode> InNeighbours
        {
            get
            {
                for (int i = 0; i < _inCount; i++)
                {
                    DiEdge e = _inEdges[i];
                    if (!e.IsUnused) yield return e.Start;
                }
            }
        }


        /// <summary>
        /// Iterates over all nodes that this node connects to (i.e. those at the end of outgoing edges).
        /// Skips nodes connected by unused edges.
        /// </summary>
        public IEnumerable<DiNode> OutNeighbours
        {
            get
            {
                for (int i = 0; i < _outCount; i++)
                {
                    DiEdge e = _outEdges[i];
                    if (!e.IsUnused) yield return e.End;
                }
            }
        }


        /// <summary>
        /// Iterates edges ending at this node.
        /// Skips unused edges.
        /// </summary>
        public IEnumerable<DiEdge> InEdges
        {
            get
            {
                for (int i = 0; i < _inCount; i++)
                {
                    DiEdge e = _inEdges[i];
                    if (!e.IsUnused) yield return e;
                }
            }
        }


        /// <summary>
        /// Iterates edges starting at this node.
        /// Skips unused edges.
        /// </summary>
        public IEnumerable<DiEdge> OutEdges
        {
            get
            {
                for (int i = 0; i < _outCount; i++)
                {
                    DiEdge e = _outEdges[i];
                    if (!e.IsUnused) yield return e;
                }
            }
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
        /// Returns the number of edges starting at this node.
        /// </summary>
        public int OutDegree
        {
            get { return _outDegree; }
            internal set { _outDegree = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int InEdgeCount
        {
            get { return _inCount; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int OutEdgeCount
        {
            get { return _outCount; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int InEdgeCapacity
        {
            get { return _inEdges.Length; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int OutEdgeCapacity
        {
            get { return _outEdges.Length; }
        }


        /// <summary>
        /// Returns true if this node has been flagged for removal.
        /// </summary>
        public override bool IsUnused
        {
            get { return _outDegree == 0 && _inDegree == 0; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void OnRemove()
        {
            // remove all in edges
            for (int i = 0; i < _inCount; i++)
            {
                DiEdge e = _inEdges[i];
                if (!e.IsUnused) e.OnRemove();
            }

            // remove all out edges
            for (int i = 0; i < _outCount; i++)
            {
                DiEdge e = _outEdges[i];
                if (!e.IsUnused) e.OnRemove();
            }

            // reset edge lists
            _outCount = _inCount = 0;
        }


        /// <summary>
        /// Removes all flagged edges from incoming and outgoing edges lists.
        /// </summary>
        internal void Compact()
        {
            Compact(ref _inEdges, ref _inCount);
            Compact(ref _outEdges, ref _outCount);
        }


        /// <summary>
        /// 
        /// </summary>
        private void Compact(ref DiEdge[] edges, ref int count)
        {
            int marker = 0;

            for (int i = 0; i < count; i++)
            {
                DiEdge e = edges[i];
                if (!e.IsUnused)
                    edges[marker++] = e;
            }

            count = marker;

            // trim array if length is greater than twice _n
            int maxLength = Math.Max(count << 1, 2);
            if (edges.Length > maxLength)
                Array.Resize(ref edges, maxLength);

            // prevent object loitering
            Array.Clear(edges, count, edges.Length - count);
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
        /// Searched for an edge from this node to another.
        /// Returns null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public DiEdge FindEdgeTo(DiNode other)
        {
            for (int i = 0; i < _outCount; i++)
            {
                DiEdge e = _outEdges[i];
                if (!e.IsUnused && e.End == other)
                    return e;
            }

            return null;
        }


        /// <summary>
        /// Returns the outgoing edge at the given index.
        /// Note that this may return unused edges.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DiEdge OutEdgeAt(int index)
        {
            return _outEdges[index];
        }


        /// <summary>
        /// Returns the incoming edge at the given index.
        /// Note that this may return unused edges.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DiEdge InEdgeAt(int index)
        {
            return _inEdges[index];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        internal void AddInEdge(DiEdge edge)
        {
            // resize if necessary
            if (_inCount == _inEdges.Length)
                Array.Resize(ref _inEdges, _inEdges.Length << 1);

            _inEdges[_inCount++] = edge;
            _inDegree++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        internal void AddOutEdge(DiEdge edge)
        {
            // resize if necessary
            if (_outCount == _outEdges.Length)
                Array.Resize(ref _outEdges, _outEdges.Length << 1);

            _outEdges[_outCount++] = edge;
            _outDegree++;
        }
    }
}
