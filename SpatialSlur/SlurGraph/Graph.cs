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
            var edges = node.Edges;

            for (int i = 0; i < edges.Count; i++)
            {
                Edge e = edges[i];
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
        /// <param name="vi"></param>
        /// <param name="vj"></param>
        protected override void AddEdge(Edge edge)
        {
            Node n = edge.Start;
            n.Edges.Add(edge);
            n.Degree++;

            n = edge.End;
            n.Edges.Add(edge);
            n.Degree++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        protected override void RemoveEdge(Edge edge)
        {
            edge.Index = -1;
            edge.Start.Degree--;
            edge.End.Degree--;
        }
    }
}
