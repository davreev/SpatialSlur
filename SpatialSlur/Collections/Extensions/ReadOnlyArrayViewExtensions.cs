
/*
 * Notes
 */

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.Collections;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ReadOnlyArrayViewExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void Convert<T, U>(this ReadOnlyArrayView<T> source, Func<T, U> converter, ArrayView<U> result, bool parallel = false)
        {
            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, source.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = converter(source[i]);
                });
            }
            else
            {
                for (int i = 0; i < source.Count; i++)
                    result[i] = converter(source[i]);
            }
        }
    }
}
