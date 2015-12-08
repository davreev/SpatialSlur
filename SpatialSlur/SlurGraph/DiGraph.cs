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
        protected override IEnumerable<Node> GetConnectedNodes(Node node)
        {
            var edges = node.Edges;

            for (int i = 0; i < edges.Count; i++)
            {
                Edge e = edges[i];
                Node n = e.End;

                if (!e.IsRemoved && !n.IsRemoved)
                    yield return n;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected override Edge FindEdge(Node ni, Node nj)
        {
            return ni.FindEdgeTo(nj);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vi"></param>
        /// <param name="vj"></param>
        protected override void AddEdge(Edge edge)
        {
            Node ni = edge.Start;
            ni.Edges.Add(edge);
            ni.Degree++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        protected override void RemoveEdge(Edge edge)
        {
            edge.Index = -1;
            edge.Start.Degree--;
        }
    }
}
