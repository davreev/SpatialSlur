using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    /// <summary>
    /// TODO
    /// Consolidate necessary functionality of Graph and Digraph for use in various graph search algorithms.
    /// </summary>
    public interface IGraph<N, E>
        where N : INode<N, E>
        where E : IEdge<N, E>
    {
        IEnumerable<N> Nodes { get; }
        IEnumerable<E> Edges { get; }
        N GetNode(int index);
        E GetEdge(int index);
        int NodeCount { get; }
        int EdgeCount { get; }
    }
}
