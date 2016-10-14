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
    public partial class DiEdgeList : GraphElementList<DiGraph, DiEdge>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="capacity"></param>
        internal DiEdgeList(DiGraph graph, int capacity = 2)
            :base(graph, capacity)
        {
        }

  
        /// <summary>
        /// Adds a new edge between the given nodes.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <returns></returns>
        public DiEdge Add(DiNode n0, DiNode n1)
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
        public DiEdge Add(int ni0, int ni1)
        {
            var nodes = Graph.Nodes;
            return AddImpl(nodes[ni0], nodes[ni1]);
        }


        /// <summary>
        /// 
        /// </summary>
        internal DiEdge AddImpl(DiNode n0, DiNode n1)
        {
            DiEdge e = new DiEdge(n0, n1);
            Add(e);
            n0.AddOutEdge(e);
            n1.AddInEdge(e);
            return e;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        public override void Remove(DiEdge edge)
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
            DiEdge e = this[index];
            if (!e.IsUnused) e.OnRemove();
        }
    }
}
