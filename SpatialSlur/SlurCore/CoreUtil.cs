using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Utility class for stray constants and static methods.
    /// </summary>
    public static class CoreUtil
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
        /// Trick to simplify bounds check
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        public static void BoundsCheck(int index, int range)
        {
            if ((uint)index >= (uint)range)
                throw new IndexOutOfRangeException();
        }
    }
}
