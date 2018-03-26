
/*
 * Notes
 */

#if USING_UNITY

using System;
using UnityEngine;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ArrayExtensions
    {
        #region Color[]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Color Lerp(this Color[] colors, float factor)
        {
            int last = colors.Length - 1;
            factor = SlurMath.Fract(factor * last, out int i);

            if (i < 0)
                return colors[0];
            else if (i >= last)
                return colors[last];

            return Color.LerpUnclamped(colors[i], colors[i + 1], factor);
        }

        #endregion
    }
}

#endif
