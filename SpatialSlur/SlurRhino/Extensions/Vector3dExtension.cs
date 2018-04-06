
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
    public static class Vector3dExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Vector3d LerpTo(this Vector3d vector, Vector3d other, double factor)
        {
            return vector + (other - vector) * factor;
        }
    }
}

#endif
