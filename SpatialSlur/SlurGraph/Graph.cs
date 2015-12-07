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
        public override IEnumerable<Node> GetConnectedNodes(int nodeIndex)
        {
            Node node = Nodes[nodeIndex];
            var edges = node.Edges;

            for (int i = 0; i < edges.Count; i++)
                yield return edges[i].GetOther(node);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public override Edge FindEdge(int i, int j)
        {
            Node ni = Nodes[i];
            Node nj = Nodes[j];

            // search from the node with the smaller degree
            if (nj.Degree < ni.Degree)
                return nj.FindEdgeTo(nj);
            else
                return ni.FindEdgeTo(nj);
        }


        /// <summary>
        /// Adds an edge between nodes i and j.
        /// </summary>
        /// <param name="vi"></param>
        /// <param name="vj"></param>
        public override Edge AddEdge(int i, int j)
        {
            Node ni = Nodes[i];
            Node nj = Nodes[j];

            Edge e = new Edge(ni, nj, EdgeCount);
            ni.Edges.Add(e);
            nj.Edges.Add(e);
            Edges.Add(e);
  
            return e;
        }
    }
}
