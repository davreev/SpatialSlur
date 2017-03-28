using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtensions
    {
        #region List<T>

        /// <summary>
        /// Removes elements from the list for which the given predicate returns false.
        /// </summary>
        public static void Compact<T>(this List<T> list, Predicate<T> include)
        {
            int marker = 0;

            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                if (include(t))
                    list[marker++] = t;
            }

            list.RemoveRange(marker, list.Count - marker); // trim list to include only used elements
        }


        /// <summary>
        /// Fills list to capacity with the default value of T.
        /// </summary>
        public static void Fill<T>(this List<T> list)
        {
            while (list.Count < list.Capacity)
                list.Add(default(T));
        }


        /// <summary>
        /// Fills list to capacity with the given value of T.
        /// </summary>
        public static void Fill<T>(this List<T> list, T value)
        {
            while (list.Count < list.Capacity)
                list.Add(value);
        }

        #endregion
    }
}
