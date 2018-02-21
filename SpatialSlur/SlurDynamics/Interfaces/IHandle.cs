
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHandle
    {
        /// <summary>
        /// 
        /// </summary>
        Vec3d Delta { get; }


        /// <summary>
        /// 
        /// </summary>
        Vec3d AngleDelta { get; }
        

        /// <summary>
        /// 
        /// </summary>
        int Index { get; set; }
    }
}
