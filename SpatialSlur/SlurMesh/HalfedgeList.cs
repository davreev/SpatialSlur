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
        /// The first halfedge in each pair is passed to the given delegate.
        /// </summary>
        public sealed override void Sort<K>(Func<E, K> getKey)
        {
            int index = 0;
            foreach (var he0 in this.TakeEveryNth(2).OrderBy(getKey))
            {
                var he1 = he0.Twin;
                this[++index] = he1;
                he1.Index = index++;
            }

            // reset index of first halfedge in each pair
            for (int i = 0; i < Count; i += 2)
            {
                var he0 = this[i + 1].Twin;
                this[i] = he0;
                he0.Index = i;
            }
        }


        /// <summary>
        /// Removes all attributes corresponding with flagged halfedge pairs.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public void CompactEdgeAttributes<U>(List<U> attributes)
        {
            int marker = SwimEdgeAttributes(attributes);
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// Moves attributes that correspond with unflagged halfedge pairs to the front of the given list.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public int SwimEdgeAttributes<U>(IList<U> attributes)
        {
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                if (this[i].IsRemoved) continue; // skip unused elements
                attributes[marker++] = attributes[i >> 1];
            }

            return marker;
        }
    }
}
