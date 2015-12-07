using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    // TODO make generic so nodes and edges can store attributes.
    public interface IGraph
    {
        /// <summary>
        /// 
        /// </summary>
        int NodeCount { get; }


        /// <summary>
        /// 
        /// </summary>
        int EdgeCount { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Node GetNode(int index);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Edge GetEdge(int index);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        IEnumerable<Node> GetConnectedNodes(int nodeIndex);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        IEnumerable<Edge> GetIncidentEdges(int nodeIndex);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        Edge FindEdge(int i, int j);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        bool HasEdge(int i, int j);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Node AddNode();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        void AddNodes(int quantity);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        Edge AddEdge(int i, int j);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        bool RemoveEdge(int i, int j);


        /// <summary>
        /// 
        /// </summary>
        void Compact();
    }
}
