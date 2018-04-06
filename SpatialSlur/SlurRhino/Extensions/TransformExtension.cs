
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
    public static class TransformExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vec4d Apply(this Transform xform, Vec4d vector)
        {
            return new Vec4d(
             vector.X * xform.M00 + vector.Y * xform.M01 + vector.Z * xform.M02 + vector.W * xform.M03,
             vector.X * xform.M10 + vector.Y * xform.M11 + vector.Z * xform.M12 + vector.W * xform.M13,
             vector.X * xform.M20 + vector.Y * xform.M21 + vector.Z * xform.M22 + vector.W * xform.M23,
             vector.W
             );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vec3d Apply(this Transform xform, Vec3d vector)
        {
            return new Vec3d(
             vector.X * xform.M00 + vector.Y * xform.M01 + vector.Z * xform.M02,
             vector.X * xform.M10 + vector.Y * xform.M11 + vector.Z * xform.M12,
             vector.X * xform.M20 + vector.Y * xform.M21 + vector.Z * xform.M22
             );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vec3d ApplyToPoint(this Transform xform, Vec3d point)
        {
            return new Vec3d(
             point.X * xform.M00 + point.Y * xform.M01 + point.Z * xform.M02 + xform.M03,
             point.X * xform.M10 + point.Y * xform.M11 + point.Z * xform.M12 + xform.M13,
             point.X * xform.M20 + point.Y * xform.M21 + point.Z * xform.M22 + xform.M23
             );
        }
    }
}

#endif
