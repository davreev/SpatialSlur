
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
            ArrayView<(Vector3d Delta, double Weight)> linearSum, 
            ArrayView<(Vector3d Delta, double Weight)> angularSum)
        {
            var handles = Handles;
            var deltas = Deltas;
            var weight = Weight;

            for (int i = 0; i < handles.Count; i++)
            {
                ref var dr = ref angularSum[handles[i].RotationIndex];
                dr.Delta += deltas[i] * weight;
                dr.Weight += weight;
            }
        }
    }
}
