using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Interface for a spatially varying function in 3 dimensions.
    /// </summary>
    public interface IField3d<TValue>
    {
        /// <summary>
        /// Returns the value at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        TValue ValueAt(Vec3d point);
    }
}
