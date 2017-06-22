using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Interface for a spatially varying function in 2 dimensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IField2d<T>
    {
        /// <summary>
        /// Returns the value at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T ValueAt(Vec2d point);
    }
}
