
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
        /// Applies calculated deltas to the affected particles.
        /// </summary>
        /// <param name="particles"></param>
        void Apply(ArrayView<Vector3d> forceSum, ArrayView<Vector3d> torqueSum);
    }
}
