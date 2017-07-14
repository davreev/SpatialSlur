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


        /// <summary>
        /// The first halfedge in each pair is passed to the given delegate.
        /// </summary>
        public sealed override void Sort<K>(Func<E, K> getKey)
        {
            // sort 2nd halfedge from each pair
            {
                int i = 0;
                foreach (var he0 in this.TakeEveryNth(2).OrderBy(getKey))
                {
                    var he1 = he0.Twin;
                    this[++i] = he1;
                    he1.Index = i++;
                }
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
