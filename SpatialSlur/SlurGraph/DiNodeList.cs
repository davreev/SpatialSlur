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
    public partial class DiNodeList : GraphElementList<DiGraph, DiNode>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="capacity"></param>
        internal DiNodeList(DiGraph graph, int capacity = 2)
            :base(graph, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public DiNode Add(int inCapacity = 2, int outCapacity = 2)
        {
            DiNode n = new DiNode(inCapacity, outCapacity);
            Add(n);
            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="inCapacity"></param>
        /// <param name="outCapacity"></param>
        public void AddMany(int quantity, int inCapacity = 2, int outCapacity = 2)
        {
            for (int i = 0; i < quantity; i++)
                Add(new DiNode(inCapacity, outCapacity));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public override void Remove(DiNode node)
        {
            OwnsCheck(node);
            node.OnRemove();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index)
        {
            this[index].OnRemove();
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Compact()
        {
            base.Compact();

            // compact edge lists for remaining nodes
            for (int i = 0; i < Count; i++)
                this[i].Compact();
        }


        /// <summary>
        /// Returns the number of common neigbours shared between n0 and n1.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <returns></returns>
        public int CountCommonOutNeighbours(DiNode n0, DiNode n1)
        {
            OwnsCheck(n0);
            OwnsCheck(n1);

            if (n0.IsUnused || n1.IsUnused)
                return 0;

            return CountCommonOutNeighboursImpl(n0, n1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        internal int CountCommonOutNeighboursImpl(DiNode n0, DiNode n1)
        {
            int currTag = NextTag;

            // flag neighbours of n1
            foreach (DiNode n in n1.OutNeighbours)
                n.Tag = currTag;

            // count flagged neighbours of n0
            int count = 0;
            foreach (DiNode n in n0.OutNeighbours)
                if (n.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns all common neigbours shared between n0 and n1.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <returns></returns>
        public List<DiNode> GetCommonOutNeighbours(DiNode n0, DiNode n1)
        {
            OwnsCheck(n0);
            OwnsCheck(n1);

            if (n0.IsUnused || n1.IsUnused)
                return new List<DiNode>();

            return GetCommonOutNeighboursImpl(n0, n1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <returns></returns>
        internal List<DiNode> GetCommonOutNeighboursImpl(DiNode n0, DiNode n1)
        {
            int currTag = NextTag;

            // flag neighbours of n1
            foreach (DiNode n in n1.OutNeighbours)
                n.Tag = currTag;

            // collect flagged neighbours of v0
            var result = new List<DiNode>();
            foreach (DiNode n in n0.OutNeighbours)
                if (n.Tag == currTag) result.Add(n);

            return result;
        }
    }
}
