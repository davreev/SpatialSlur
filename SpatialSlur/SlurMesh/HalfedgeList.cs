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
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public class HalfedgeList<E> : HeElementList<E>
        where E : HeElement, IHalfedge<E>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        internal HalfedgeList(int capacity)
            : base(capacity)
        {
        }


        /// <inheritdoc/>
        /// <summary>
        /// Halfedges are sorted in pairs where the key is taken from the first halfedge in each pair.
        /// </summary>
        public sealed override void Sort<K>(Func<E, K> getKey, IComparer<K> keyComparer)
        {
            // sort first halfedge in each pair
            int index = 0;
            foreach (var he0 in this.TakeEveryNth(2).OrderBy(getKey, keyComparer))
            {
                this[index] = he0;
                index += 2;
            }

            // reset index of first halfedge in each pair
            for (int i = 0; i < Count; i += 2)
            {
                var he0 = this[i];
                var he1 = he0.Twin;

                // re-index pair
                he0.Index = i;
                he1.Index = i + 1;

                // set twin
                this[i + 1] = he1;
            }
        }
    }
}
