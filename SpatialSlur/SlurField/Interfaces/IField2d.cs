using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Interface for a spatially varying function in 2 dimensions.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IField2d<TValue>
    {
        /// <summary>
        /// Returns the value at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        TValue ValueAt(Vec2d point);
    }
}
