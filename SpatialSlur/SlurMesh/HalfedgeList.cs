using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class HalfedgeList<V, E> : HeElementList<E>
        where V : HeElement, IHeVertex<V, E>
        where E : HeElement, IHalfedge<V, E>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HalfedgeList(int capacity)
            : base(capacity)
        {
        }


        /// <summary>
        /// Sorts halfedges by the first in each pair.
        /// </summary>
        public sealed override void Sort<K>(Func<E, K> getKey)
        {
            int index = 1;
            foreach (var he0 in this.TakeEveryNth(2).OrderBy(getKey))
            {
                var he1 = he0.Twin;
                this[index] = he1;
                he1.Index = index;
                index += 2;
            }

            // reset index of first halfedge in each pair
            for (int i = 0; i < Count; i += 2)
            {
                var he0 = this[i + 1].Twin;
                this[i] = he0;
                he0.Index = i;
            }
        }
    }
}
