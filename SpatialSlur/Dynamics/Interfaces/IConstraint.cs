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
        /// Accumulates the calculated deltas onto the affected particles.
        /// </summary>
        void Accumulate(
            ArrayView<Vector4d> linearCorrectSums, 
            ArrayView<Vector4d> angularCorrectSums);
    }
}
