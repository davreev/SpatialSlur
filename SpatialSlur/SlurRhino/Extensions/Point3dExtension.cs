
/*
 * Notes
 */

#if USING_RHINO


using Rhino.Geometry;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class Point3dExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Point3d LerpTo(this Point3d point, Point3d other, double factor)
        {
            return point + (other - point) * factor;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double SquareDistanceTo(this Point3d point, Point3d other)
        {
            Vector3d v = other - point;
            return v.SquareLength;
        }
    }
}

#endif
