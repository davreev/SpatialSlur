using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    public interface INode<N, E>
        where N : INode<N, E>
        where E : IEdge<N, E>
    {
        IEnumerable<E> Adjacency { get; }
        int Index { get; }
        int Degree { get; }
    }
}
