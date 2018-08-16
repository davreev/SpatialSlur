
/*
 * Notes 
 */

#if USING_RHINO

using Rhino.Geometry;
using SpatialSlur;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Interval3dExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Interval3d interval)
        {
            var x = interval.X;
            var y = interval.Y;
            var z = interval.Z;
            return new BoundingBox(x.A, y.A, z.A, x.B, y.B, z.B);
        }
    }
}

#endif