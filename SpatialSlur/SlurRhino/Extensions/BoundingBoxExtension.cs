
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
    public static class BoundingBoxExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public static Interval2d ToInterval2d(this BoundingBox bbox)
        {
            Vec3d p0 = bbox.Min;
            Vec3d p1 = bbox.Max;
            return new Interval2d(p0, p1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public static Interval3d ToInterval3d(this BoundingBox bbox)
        {
            return new Interval3d(bbox.Min, bbox.Max);
        }
    }
}

#endif
