#if USING_UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurUnity
{
    /// <summary>
    /// 
    /// </summary>
    public static class SlurMathf
    {
        /// <summary></summary>
        public const float ZeroTolerance = 1.0e-6f;


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