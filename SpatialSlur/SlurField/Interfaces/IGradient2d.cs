
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
    public interface IGradient2d<T>
    {
        /// <summary>
        /// Returns the the gradient at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        void GradientAt(Vec2d point, out T gx, out T gy);
    }
}
