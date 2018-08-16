
/*
 * Notes
 */

#if USING_RHINO

using System;

using SpatialSlur.Collections;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class PriorityQueueExtensions
    {
        /// <summary>
        /// Workaround for lack of support for ValueTuple in Grasshopper script components
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="queue"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void GetMin<K, V>(this PriorityQueue<K,V> queue, out K key, out V value)
            where K : IComparable<K>
        {
            (key, value) = queue.Min;
        }


        /// <summary>
        /// Workaround for lack of support for ValueTuple in Grasshopper script components
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="queue"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void RemoveMin<K, V>(this PriorityQueue<K, V> queue, out K key, out V value)
            where K : IComparable<K>
        {
            (key, value) = queue.RemoveMin();
        }
    }
}

#endif