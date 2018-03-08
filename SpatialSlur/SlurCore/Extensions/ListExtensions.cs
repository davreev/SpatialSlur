using System;
using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ListExtensions
    {
        #region List<T>

        /// <summary>
        /// Moves true elements to the front of the list and removes remaining elements from the list.
        /// </summary>
        public static void Compact<T>(this List<T> list, Predicate<T> match)
        {
            int marker = list.Swim(match);
            list.RemoveRange(marker, list.Count - marker);
        }


        /// <summary>
        /// Fills list to capacity with the default value of T.
        /// </summary>
        public static void Fill<T>(this List<T> list)
        {
            list.FillTo(list.Capacity);
        }


        /// <summary>
        /// Fills list to capacity with the given value of T.
        /// </summary>
        public static void Fill<T>(this List<T> list, T value)
        {
            list.FillTo(list.Capacity, value);
        }


        /// <summary>
        /// Fills list to the specified count with the default value of T.
        /// </summary>
        public static void FillTo<T>(this List<T> list, int count)
        {
            while (list.Count < count)
                list.Add(default(T));
        }


        /// <summary>
        /// Fills list to the specified count with the given value of T.
        /// </summary>
        public static void FillTo<T>(this List<T> list, int count, T value)
        {
            while (list.Count < count)
                list.Add(value);
        }

        #endregion
    }
}
