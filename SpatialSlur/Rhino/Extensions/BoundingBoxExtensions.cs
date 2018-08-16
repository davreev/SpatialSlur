
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
    public static class BoundingBoxExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public static Interval2d ToInterval2d(this BoundingBox bbox)
        {
            Vector3d p0 = bbox.Min;
            Vector3d p1 = bbox.Max;
            return new Interval2d(p0.XY, p1.XY);
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
