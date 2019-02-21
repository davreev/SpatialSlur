/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for constraints that act on the rotations of a group a particles
    /// </summary>
    public abstract class RotationConstraint : Influence<Vector4d>, IConstraint
    {
        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector4d> linearCorrectSums,
            ArrayView<Vector4d> angularCorrectSums)
        {
            var handles = Particles;
            var deltas = Deltas;

            for (int i = 0; i < handles.Count; i++)
                angularCorrectSums[handles[i].RotationIndex] += deltas[i];
        }
    }
}
