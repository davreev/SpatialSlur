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
    public class Edge
    {
        private Node _start;
        private Node _end;
        //private E _data;
        private int _index = -1;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal Edge(Node start, Node end, int index)
        {
            _start = start;
            _end = end;
            _index = index;
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
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsRemoved
        {
            get { return _index == -1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            _index = -1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node GetOther(Node node)
        {
            if (node == _start)
                return _end;
            else if (node == _end)
                return _start;

            return null;
        }
    }
}
