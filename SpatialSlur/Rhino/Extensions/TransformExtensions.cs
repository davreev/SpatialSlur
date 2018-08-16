
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
    public static class TransformExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector4d Apply(this Transform xform, Vector4d vector)
        {
            return new Vector4d(
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
        public static Vector3d Apply(this Transform xform, Vector3d vector)
        {
            return new Vector3d(
             vector.X * xform.M00 + vector.Y * xform.M01 + vector.Z * xform.M02,
             vector.X * xform.M10 + vector.Y * xform.M11 + vector.Z * xform.M12,
             vector.X * xform.M20 + vector.Y * xform.M21 + vector.Z * xform.M22
             );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector3d ApplyToPoint(this Transform xform, Vector3d point)
        {
            return new Vector3d(
             point.X * xform.M00 + point.Y * xform.M01 + point.Z * xform.M02 + xform.M03,
             point.X * xform.M10 + point.Y * xform.M11 + point.Z * xform.M12 + xform.M13,
             point.X * xform.M20 + point.Y * xform.M21 + point.Z * xform.M22 + xform.M23
             );
        }
    }
}

#endif
