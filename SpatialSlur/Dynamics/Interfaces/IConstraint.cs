
/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConstraint : IInfluence
    {
        /// <summary>
        /// 
        /// </summary>
        double Weight { get; set; }


        /// <summary>
        /// Accumulates the calculated deltas onto the affected particles.
        /// </summary>
        /// <param name="particles"></param>
        void Accumulate(ArrayView<(Vector3d, double)> linearDeltaSum, ArrayView<(Vector3d, double)> angularDeltaSum);
    }
}
