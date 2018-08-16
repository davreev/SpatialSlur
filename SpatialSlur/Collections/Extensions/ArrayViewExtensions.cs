
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
    public static partial class ArrayViewExtensions
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
        public static void Convert<T, U>(this ArrayView<T> source, Func<T, U> converter, ArrayView<U> result, bool parallel = false)
        {
            ReadOnlyArrayViewExtensions.Convert(source, converter, result, parallel);
        }
    }
}
