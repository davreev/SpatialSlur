
/*
 * Notes
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurData;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ListExtension
    {
        #region List<T>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ListView<T> GetView<T>(this List<T> list, int count)
        {
            return new ListView<T>(list, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ListView<T> GetView<T>(this List<T> list, int index, int count)
        {
            return new ListView<T>(list, index, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ReadOnlyListView<T> GetReadOnlyView<T>(this List<T> list, int count)
        {
            return new ReadOnlyListView<T>(list, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ReadOnlyListView<T> GetReadOnlyView<T>(this List<T> list, int index, int count)
        {
            return new ReadOnlyListView<T>(list, index, count);
        }


        /// <summary>
        /// Allows enumeration over indexable segments of the given list.
        /// </summary>
        public static IEnumerable<ReadOnlyListView<T>> Batch<T>(this List<T> source, IEnumerable<int> sizes)
        {
            int marker = 0;
            foreach (int n in sizes)
            {
                yield return source.GetReadOnlyView(marker, n);
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
                yield return source.GetSubView(marker, n);
                marker += n;
            }
        }

        #endregion
    }
}
