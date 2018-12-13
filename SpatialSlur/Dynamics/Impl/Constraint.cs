
/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for a constraint that acts on the positions and rotations of a dynamic collection of particles.
    /// </summary>
    public abstract class Constraint : WeightedInfluence<(Vector3d Position, Vector3d Rotation)>, IConstraint
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
                ref var p = ref linearSum[handles[i].PositionIndex];
                p.Delta += deltas[i].Position * weight;
                p.Weight += weight;

                ref var r = ref angularSum[handles[i].RotationIndex];
                r.Delta += deltas[i].Rotation * weight;
                r.Weight += weight;
            }
        }
    }
}
