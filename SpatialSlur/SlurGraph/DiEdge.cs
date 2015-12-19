using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    public class DiEdge
    {
        private DiNode _start;
        private DiNode _end;
        //private E _data;
        private int _index = -1;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal DiEdge(DiNode start, DiNode end, int index)
        {
            _start = start;
            _end = end;
            _index = index;
        }


        /// <summary>
        /// Returns the node at the start of the edge.
        /// </summary>
        public DiNode Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// Returns the node at the end of the edge.
        /// </summary>
        public DiNode End
        {
            get { return _end; }
            internal set { _end = value; }
        }


        /// <summary>
        /// Returns the index of the edge within the graph's edge list.
        /// This will be set to -1 if the edge is removed.
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }


        /// <summary>
        /// Return true if the edge has been flagged for removal.
        /// </summary>
        public bool IsRemoved
        {
            get { return _index == -1; }
        }


        /// <summary>
        /// Flags the edge for removal.
        /// </summary>
        public void Remove()
        {
            if (IsRemoved) return; // check if already flagged
            _index = -1;
            _start.OutDegree--;
            _end.InDegree--;
        }
    }
}
