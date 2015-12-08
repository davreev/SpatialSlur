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
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        protected override IEnumerable<Node> GetConnectedNodes(Node node)
        {
            foreach (Edge e in node.IncidentEdges)
            {
                Node n = e.GetOther(node);
                if (!e.IsRemoved && !n.IsRemoved)
                    yield return n;
            }
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
            ni.AddEdge(e);
            nj.AddEdge(e);
            return e;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        protected override void RemoveEdge(Edge edge)
        {
            if (edge.IsRemoved) return; // make sure the edge hasn't already been removed
            edge.Remove(); // flag for removal
        }
    }
}
