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
    public class Edge : GraphElement
    {
        private Node _start;
        private Node _end;
        //private E _data;


        /// <summary>
        /// 
        /// </summary>
        internal Edge()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal Edge(Node start, Node end)
        {
            _start = start;
            _end = end;
        }


        /// <summary>
        /// 
        /// </summary>
        public Node Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Node End
        {
            get { return _end; }
            internal set { _end = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node Other(Node node)
        {
            if (node == _start)
                return _end;
            else if (node == _end)
                return _start;

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsUnused
        {
            get { return _start == null; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void OnRemove()
        {
            _start.Degree--;
            _end.Degree--;
            _start = null; // flag as unused
        }


        /*
        /// <summary>
        /// This edge and its start node are flagged as unused
        /// Edges from start node are transferred to end node
        /// </summary>
        internal void Collapse()
        {
            if (IsRemoved) return; // can't collapse a removed edge

            // flag edge for removal
            _index = -1;
            //_start.Degree--;
            _end.Degree--;

            // flag start node for removal
            _start.Index = -1;

            // transfer edges from start node to the end node
            foreach (Edge e in _start.Edges)
                _end.AddEdge(e);
            
            this.Remove();
        }
        */
    }
}
