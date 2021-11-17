
/*
 * Notes 
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Grid
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Func<int, int, int> GetWrapFunction(WrapMode mode)
        {
            switch (mode)
            {
                case WrapMode.Clamp:
                    return Clamp;
                case WrapMode.Repeat:
                    return Repeat;
                case WrapMode.Mirror:
                    return Mirror;
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static int Wrap(int index, int count, WrapMode mode)
        {
            return 
                mode == WrapMode.Mirror ? Mirror(index, count) :
                mode == WrapMode.Repeat ? Repeat(index, count) :
                Clamp(index, count);

#if OBSOLETE
            // This implementation doesn't inline
            switch (mode)
            {
                case WrapMode.Clamp:
                    return Clamp(index, range);
                case WrapMode.Repeat:
                    return Repeat(index, range);
                case WrapMode.Mirror:
                    return Mirror(index, range);
            }

            throw new NotSupportedException();
#endif
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Clamp(int i, int n)
        {
            return (i < 0) ? 0 : (i < n) ? i : n - 1;
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Repeat(int i, int n)
        {
            i %= n;
            return (i < 0) ? i + n : i;
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Mirror(int i, int n)
        {
            int n2 = n << 1;
            i %= n2;
            i = (i < 0) ? i + n2 : i;
            return n - Math.Abs(i - n);
        }
    }
}
