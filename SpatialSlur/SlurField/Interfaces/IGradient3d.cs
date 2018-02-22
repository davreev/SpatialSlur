
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGradient3d<T>
    {
        /// <summary>
        /// Returns the the gradient at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        void GradientAt(Vec3d point, out T gx, out T gy, out T gz);
    }
}
