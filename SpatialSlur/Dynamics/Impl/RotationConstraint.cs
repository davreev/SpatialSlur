
/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for a constraint that acts on the rotations of a dynamic collection of particles.
    /// </summary>
    public abstract class RotationConstraint : WeightedInfluence<Vector3d>, IConstraint
    {
        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector4d> linearCorrectSums,
            ArrayView<Vector4d> angularCorrectSums)
        {
            var handles = Handles;
            var deltas = Deltas;
            var w = Weight;

            for (int i = 0; i < handles.Count; i++)
                angularCorrectSums[handles[i].RotationIndex] += new Vector4d(deltas[i] * w, w);
        }
    }
}
