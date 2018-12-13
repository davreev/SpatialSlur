
/*
 * Notes
 */

using System;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for a constraint that acts on the positions of a dynamic collection of particles.
    /// </summary>
    public abstract class PositionConstraint : WeightedInfluence<Vector3d>, IConstraint
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
                ref var dp = ref linearSum[handles[i].PositionIndex];
                dp.Delta += deltas[i] * weight;
                dp.Weight += weight;
            }
        }
    }
}
