
/*
 * Notes
 */

using System;
using System.Collections.Generic;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class Sequences
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static IEnumerable<int> CountFrom(int start)
        {
            while (true)
                yield return start++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stride"></param>
        /// <returns></returns>
        public static IEnumerable<int> CountFrom(int start, int stride)
        {
            while (true)
            {
                yield return start;
                start += stride;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<int> Fibonacci()
        {
            int n0 = 0;
            int n1 = 1;

            yield return n0;
            yield return n1;

            while (true)
            {
                int n2 = n0 + n1;
                yield return n2;
                n0 = n1;
                n1 = n2;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="digits"></param>
        /// <param name="radix"></param>
        /// <returns></returns>
        public static IEnumerable<ReadOnlyArrayView<int>> CountInBase(int digits, int radix)
        {
            var curr = new int[digits];
            var n = Pow(radix, digits);

            yield return curr;

            while (--n > 0L)
            {
                int i = 0;

                // increment
                curr[i]++;

                // carry
                while (curr[i] == radix)
                {
                    curr[i++] = 0;
                    curr[i]++;
                }

                yield return curr;
            }

            long Pow(long x, long y)
            {
                long result = 1;

                for (long i = 0; i < y; i++)
                    result *= x;

                return result;
            }
        }
    }
}
