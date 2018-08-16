
/*
 * Notes
 */

using System;
using System.Runtime.CompilerServices;

namespace SpatialSlur
{
    /// <summary>
    /// Static class for stray methods that don't yet fit anywhere else (boo).
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
    }
}
