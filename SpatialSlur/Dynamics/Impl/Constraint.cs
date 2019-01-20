/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for constraints that act on both positions and rotations of a group of particles
    /// </summary>
    public abstract class Constraint : Influence<(Vector4d Position, Vector4d Rotation)>, IConstraint
    {
        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector4d> linearCorrectSums, 
            ArrayView<Vector4d> angularCorrectSums)
        {
            var handles = Particles;
            var deltas = Deltas;

            for (int i = 0; i < handles.Count; i++)
            {
                ref var h = ref handles[i];
                ref var d = ref deltas[i];
                linearCorrectSums[h.PositionIndex] += d.Position;
                angularCorrectSums[h.RotationIndex] += d.Rotation;
            }
        }
    }
}
