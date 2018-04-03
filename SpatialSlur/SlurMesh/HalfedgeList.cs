
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
            var items = Items;
            int result = 0;

            for (int i = 0; i < Count; i += 2)
                if (items[i].IsUnused) result += 2;

            return result;
        }


        /// <inheritdoc/>
        public override void Compact()
        {
            var items = Items;
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                var he = items[i];
                if (he.IsUnused) continue; // skip unused halfedge pairs

                he.Index = marker;
                items[marker++] = he;

                he = items[i + 1];

                he.Index = marker;
                items[marker++] = he;
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

            var items = Items;
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                if (items[i].IsUnused) continue; // skip unused halfedge pairs
                attributes[marker++] = attributes[i];
                attributes[marker++] = attributes[i + 1];
            }

            return marker;
        }


        /// <inheritdoc/>
        public override int SwimAttributes<A>(A[] attributes)
        {
            var items = Items;
            int marker = 0;

            for (int i = 0; i < Count; i += 2)
            {
                if (items[i].IsUnused) continue; // skip unused halfedge pairs
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
            var items = Items;
            int index = 0;

            // sort in pairs
            foreach (var he0 in Edges().OrderBy(getKey))
            {
                items[index++] = he0;
                items[index++] = he0.Twin;
            }

            // re-index after since indices may be used to fetch sort keys
            for (int i = 0; i < Count; i++)
                items[i].Index = i;

            IEnumerable<E> Edges()
            {
                for (int i = 0; i < Count; i += 2)
                    yield return items[i];
            }
        }
    }
}
