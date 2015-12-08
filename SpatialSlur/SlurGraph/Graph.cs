using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    public class Graph:GraphBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public Graph()
            :base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCount"></param>
        public Graph(int nodeCapacity, int edgeCapacity)
            :base(nodeCapacity, edgeCapacity)
        {
        }


        /// <summary>
        /// Creates a copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        public Graph(Graph other)
            :base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ni"></param>
        /// <param name="nj"></param>
        /// <returns></returns>
        protected override Edge FindEdge(Node ni, Node nj)
        {
            if (nj.Degree < ni.Degree) // search from the node with the smaller degree
                return nj.FindEdgeTo(nj);
            else
                return ni.FindEdgeTo(nj);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ni"></param>
        /// <param name="nj"></param>
        /// <returns></returns>
        protected override Edge AddEdge(Node ni, Node nj)
        {
            Edge e = base.AddEdge(ni, nj);
            if (e != null)
            {
                ni.AddEdge(e);
                nj.AddEdge(e);
            }

            return e;
        }
    }
}
