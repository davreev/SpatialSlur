using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EdgeList : GraphElementList<Graph, Edge>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="capacity"></param>
        internal EdgeList(Graph graph, int capacity = 2)
            :base(graph, capacity)
        {
        }

  
        /// <summary>
        /// Adds a new edge between the given nodes.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <returns></returns>
        public Edge Add(Node n0, Node n1)
        {
            var nodes = Graph.Nodes;
            nodes.OwnsCheck(n0);
            nodes.OwnsCheck(n1);

            return AddImpl(n0, n1);
        }


        /// <summary>
        /// Adds a new edge between nodes at the given indices.
        /// </summary>
        /// <param name="ni0"></param>
        /// <param name="ni1"></param>
        /// <returns></returns>
        public Edge Add(int ni0, int ni1)
        {
            var nodes = Graph.Nodes;
            return AddImpl(nodes[ni0], nodes[ni1]);
        }


        /// <summary>
        /// 
        /// </summary>
        internal Edge AddImpl(Node n0, Node n1)
        {
            Edge e = new Edge(n0, n1);
            Add(e);
            n0.AddEdge(e);
            n1.AddEdge(e);
            return e;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        public override void Remove(Edge edge)
        {
            OwnsCheck(edge);
            edge.UsedCheck(); // can't remove an unused edge
            edge.OnRemove();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index)
        {
            Edge e = this[index];
            if (!e.IsUnused) e.OnRemove();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void CollapseEdge(int index)
        {
            // TODO
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void SplitEdge(int index)
        {
            // TODO
        }
    }
}
