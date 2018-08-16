

/*
 * Notes
 */

using SpatialSlur;

namespace SpatialSlur.Fields
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
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        void GradientAt(Vector2d point, out T dx, out T dy);
    }
}
