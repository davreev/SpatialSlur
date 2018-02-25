using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class HeNodeList<T, E> : HeElementList<T>
        where T : HeNode<T, E>
        where E : Halfedge<E>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HeNodeList(int capacity)
            :base(capacity)
        {
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountUnused()
        {
            int result = 0;

            for (int i = 0; i < Count; i++)
                if (Items[i].IsUnused) result++;

            return result;
        }


        /// <summary>
        /// Removes all unused elements in the list and re-indexes the remaining.
        /// Does not change the capacity of the list.
        /// If the list has any associated attributes, be sure to compact those first.
        /// </summary>
        public void Compact()
        {
            int marker = 0;

            for (int i = 0; i < Count; i++)
            {
                T element = Items[i];
                if (element.IsUnused) continue; // skip unused elements

                element.Index = marker;
                Items[marker++] = element;
            }

            AfterCompact(marker);
        }


        /// <summary>
        /// Removes all attributes corresponding with unused elements.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<U>(List<U> attributes)
        {
            int marker = SwimAttributes(attributes);
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// Moves attributes corresponding with used elements to the front of the given list.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public int SwimAttributes<U>(IList<U> attributes)
        {
            int marker = 0;

            for (int i = 0; i < Count; i++)
            {
                if (Items[i].IsUnused) continue; // skip unused elements
                attributes[marker++] = attributes[i];
            }

            return marker;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="getKey"></param>
        public void Sort<K>(Func<T, K> getKey)
            where K : IComparable<K>
        {
            int index = 0;

            // sort first
            foreach (var t in this.OrderBy(getKey))
                Items[index++] = t;
            
            // re-index after since indices may be used to fetch keys
            for (int i = 0; i < Count; i++)
                Items[i].Index = i;
        }
    }
}
