using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
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
        /// <returns></returns>
        public static IEnumerable<int> CountFrom(int start, int step)
        {
            while (true)
            {
                yield return start;
                start += step;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
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
    }
}
