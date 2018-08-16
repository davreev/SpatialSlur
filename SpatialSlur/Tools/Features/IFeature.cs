
/*
 * Notes
 */

using SpatialSlur;

namespace SpatialSlur.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFeature
    {
        /// <summary>
        /// Determines priority during assignment
        /// </summary>
        int Rank { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Vector3d ClosestPoint(Vector3d point);
    }
}
