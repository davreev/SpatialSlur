
/*
 * Notes
 */

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IForce : IInfluence
    {
        /// <summary>
        /// 
        /// </summary>
        double Strength { get; set; }
    }
}
