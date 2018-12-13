
/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for a constraint that acts on the rotations of a dynamic collection of particles.
    /// </summary>
    public abstract class RotationConstraint : WeightedInfluence<(Vector3d, Vector3d)>, IConstraint
    {
        /// <inheritdoc />
        public virtual void Accumulate(ArrayView<(Vector3d, double)> linearDeltaSum, ArrayView<(Vector3d, double)> angularDeltaSum)
        {
            var handles = Handles;
            var deltas = Deltas;
            var weight = Weight;

            for (int i = 0; i < handles.Count; i++)
            {
                ref var dr = ref angularDeltaSum[handles[i].RotationIndex];
                dr.Item1 += deltas[i].Item2 * weight;
                dr.Item2 += weight;
            }
        }
    }
}
