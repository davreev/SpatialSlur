using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurGraph
{
    public interface IEdge<N, E>
        where N : INode<N, E>
        where E : IEdge<N, E>
    {
        N Start { get; }
        N End { get; }
        int Index { get; }
        bool IsRemoved { get; }
    }
}
