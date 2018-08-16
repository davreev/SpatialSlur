
/*
 * Notes
 */

#if USING_UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class SlurMath
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="whole"></param>
        /// <returns></returns>
        public static float Fract(float value, out int whole)
        {
            whole = (int)Mathf.Floor(value);
            return value - whole;
        }
    }
}

#endif