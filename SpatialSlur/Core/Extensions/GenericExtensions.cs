
/*
 * Notes
 */

using System.Collections.Generic;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
