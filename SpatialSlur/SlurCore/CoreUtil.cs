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
    /// Utility class for stray methods.
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
    }
}
