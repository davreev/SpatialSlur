
/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IForce : IInfluence
    {
        /// <summary>
        /// Accumulates the calculated forces onto the affected particles.
        /// </summary>
        void Accumulate(
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums);
    }
}
