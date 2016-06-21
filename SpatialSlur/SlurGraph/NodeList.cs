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
    public class NodeList : GraphElementList<Graph, Node>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="capacity"></param>
        internal NodeList(Graph graph, int capacity = 2)
            :base(graph,capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public Node Add(int capacity = 2)
        {
            Node n = new Node(capacity);
            Add(n);
            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="capacity"></param>
        public void AddMany(int quantity, int capacity = 2)
        {
            for (int i = 0; i < quantity; i++)
                Add(new Node(capacity));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public override void Remove(Node node)
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
        public int CountCommonNeighbours(Node n0, Node n1)
        {
            OwnsCheck(n0);
            OwnsCheck(n1);

            if (n0.IsUnused || n1.IsUnused)
                return 0;

            return CountCommonNeighboursImpl(n0, n1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        internal int CountCommonNeighboursImpl(Node n0, Node n1)
        {
            int currTag = NextTag;

            // flag neighbours of n1
            foreach (Node n in n1.Neighbours)
                n.Tag = currTag;

            // count flagged neighbours of n0
            int count = 0;
            foreach (Node n in n0.Neighbours)
                if (n.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns all common neigbours shared between n0 and n1.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <returns></returns>
        public List<Node> GetCommonNeighbours(Node n0, Node n1)
        {
            OwnsCheck(n0);
            OwnsCheck(n1);

            if (n0.IsUnused || n1.IsUnused)
                return new List<Node>();

            return GetCommonNeighboursImpl(n0, n1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        /// <returns></returns>
        internal List<Node> GetCommonNeighboursImpl(Node n0, Node n1)
        {
            int currTag = NextTag;

            // flag neighbours of n1
            foreach (Node n in n1.Neighbours)
                n.Tag = currTag;

            // collect flagged neighbours of v0
            List<Node> result = new List<Node>();
            foreach (Node n in n0.Neighbours)
                if (n.Tag == currTag) result.Add(n);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="n0"></param>
        /// <param name="n1"></param>
        public void MergeNodes(Node n0, Node n1)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void ExpandNode(Node node)
        {
            // TODO
            throw new NotImplementedException();

            // i.e. chamfer
            // split all incident edges
            // reconnect between new nodes
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void SplitNode(Node node)
        {
            // TODO
            throw new NotImplementedException();

            // for mask, true edges are transferred to new vertex
        }
    }
}
