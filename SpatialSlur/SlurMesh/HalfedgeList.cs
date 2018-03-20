

/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public class HalfedgeList<E> : HeElementList<E>
        where E : Halfedge<E>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HalfedgeList(int capacity)
            : base(capacity)
        {
        }


        /// <inheritdoc/>
        public override int CountUnused()
        {
            int result = 0;

            for (int i = 0; i < Count; i += 2)
                if (Items[i].IsUnused) result += 2;

            return result;
        }


        /// <inheritdoc/>
        public override void Compact()
        {
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                var he = Items[i];
                if (he.IsUnused) continue; // skip unused halfedge pairs

                he.Index = marker;
                Items[marker++] = he;

                he = he.Twin;

                he.Index = marker;
                Items[marker++] = he;
            }

            AfterCompact(marker);
        }


        /// <inheritdoc/>
        public override void CompactAttributes<A>(List<A> attributes)
        {
            int marker = SwimAttributes(attributes);
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <inheritdoc/>
        public override int SwimAttributes<A>(IList<A> attributes)
        {
            if (attributes is A[] arr)
                return SwimAttributes(arr);

            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                if (Items[i].IsUnused) continue; // skip unused halfedge pairs
                attributes[marker++] = attributes[i];
                attributes[marker++] = attributes[i + 1];
            }

            return marker;
        }


        /// <inheritdoc/>
        public override int SwimAttributes<A>(A[] attributes)
        {
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                if (Items[i].IsUnused) continue; // skip unused halfedge pairs
                attributes[marker++] = attributes[i];
                attributes[marker++] = attributes[i + 1];
            }

            return marker;
        }


        /// <summary>
        /// Reorders halfedge pairs based on the given key.
        /// Note that the key is taken from the first halfedge in each pair.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="getKey"></param>
        public override void Sort<K>(Func<E, K> getKey)
        {
            int index = 0;

            // sort in pairs
            foreach (var he0 in Edges().OrderBy(getKey))
            {
                Items[index++] = he0;
                Items[index++] = he0.Twin;
            }

            // re-index after since indices may be used to fetch keys
            for (int i = 0; i < Count; i++)
                Items[i].Index = i;

            IEnumerable<E> Edges()
            {
                for (int i = 0; i < Count; i += 2)
                    yield return Items[i];
            }
        }
    }
}
