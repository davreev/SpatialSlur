
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
        /// Accumulates the calculated forces with respect to the affected particles.
        /// </summary>
        /// <param name="particles"></param>
        void Accumulate(ArrayView<Vector3d> forceSum, ArrayView<Vector3d> torqueSum);
    }
}
