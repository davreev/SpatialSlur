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
    public class DiEdge : GraphElement
    {
        private DiNode _start;
        private DiNode _end;
        //private E _data;


        /// <summary>
        /// 
        /// </summary>
        internal DiEdge()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal DiEdge(DiNode start, DiNode end)
        {
            _start = start;
            _end = end;
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
        /// Return true if the edge has been flagged for removal.
        /// </summary>
        public override bool IsUnused
        {
            get { return _start == null; }
        }


        /// <summary>
        /// Flags the edge for removal.
        /// </summary>
        internal void OnRemove()
        {
            _start.OutDegree--;
            _end.InDegree--;
            _start = null;
        }
    }
}
