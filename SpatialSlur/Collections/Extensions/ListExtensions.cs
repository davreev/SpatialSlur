
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using SpatialSlur.Collections;

namespace SpatialSlur
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
                list.Add(default);
        }


        /// <summary>
        /// Fills list to the specified count with the given value of T.
        /// </summary>
        public static void FillTo<T>(this List<T> list, int count, T value)
        {
            while (list.Count < count)
                list.Add(value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ListView<T> AsView<T>(this List<T> list)
        {
            return new ListView<T>(list, 0, list.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ListView<T> AsView<T>(this List<T> list, int count)
        {
            return new ListView<T>(list, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ListView<T> AsView<T>(this List<T> list, int start, int count)
        {
            return new ListView<T>(list, start, count);
        }


        /// <summary>
        /// Allows enumeration over indexable segments of the given list.
        /// </summary>
        public static IEnumerable<ReadOnlyListView<T>> Batch<T>(this List<T> source, IEnumerable<int> sizes)
        {
            int marker = 0;
            foreach (int n in sizes)
            {
                yield return source.AsView(marker, n);
                marker += n;
            }
        }


        /// <summary>
        /// Allows enumeration over indexable segments of the given list.
        /// </summary>
        public static IEnumerable<ReadOnlyListView<T>> Batch<T>(this ReadOnlyListView<T> source, IEnumerable<int> sizes)
        {
            int marker = 0;
            foreach (int n in sizes)
            {
                yield return source.Subview(marker, n);
                marker += n;
            }
        }

        #endregion
    }
}
