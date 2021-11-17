/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.CompilerServices;

namespace SpatialSlur
{
    /// <summary>
    /// Static class for stray utility methods that don't yet fit anywhere else (boo).
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap<U>(ref U a, ref U b)
        {
            var c = a;
            a = b;
            b = c;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public static void BoundsCheck(int index, int count)
        {
            if ((uint)index >= (uint)count)
                throw new IndexOutOfRangeException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public static void BoundsCheck(long index, long count)
        {
            if ((ulong)index >= (ulong)count)
                throw new IndexOutOfRangeException();
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> Yield<T>(T item)
        {
            yield return item;
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> Yield<T>(T t0, T t1)
        {
            yield return t0;
            yield return t1;
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> Yield<T>(T t0, T t1, T t2)
        {
            yield return t0;
            yield return t1;
            yield return t2;
        }
    }
}
