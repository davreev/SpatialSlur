using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    public class DiGraph:GraphBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public DiGraph()
            :base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCount"></param>
        public DiGraph(int nodeCapacity, int edgeCapacity)
            :base(nodeCapacity, edgeCapacity)
        {
        }


        /// <summary>
        /// Creates a copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        public DiGraph(DiGraph other)
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
            var edges = Nodes[nodeIndex].Edges;

            for (int i = 0; i < edges.Count; i++)
                yield return edges[i].End;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public override Edge FindEdge(int i, int j)
        {
            return Nodes[i].FindEdgeTo(Nodes[j]);
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
            Edges.Add(e);
   
            return e;
        }
    }
}
