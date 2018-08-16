
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
    public interface IGradient3d<T>
    {
        /// <summary>
        /// Returns the the gradient at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        void GradientAt(Vector3d point, out T dx, out T dy, out T dz);
    }
}
