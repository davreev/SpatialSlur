
/*
 * Notes 
 */

#if USING_RHINO

using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Interval2dExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Interval2d interval)
        {
            var x = interval.X;
            var y = interval.Y;
            return new BoundingBox(x.A, y.A, 0.0, x.B, y.B, 0.0);
        }
    }
}

#endif