
/*
 * Notes
 */

using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurTools
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
        Vec3d ClosestPoint(Vec3d point);
    }
}
