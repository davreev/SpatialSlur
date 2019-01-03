
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
            ArrayView<Vector4d> linearCorrectSums, 
            ArrayView<Vector4d> angularCorrectSums)
        {
            var handles = Handles;
            var deltas = Deltas;
            var w = Weight;

            for (int i = 0; i < handles.Count; i++)
            {
                ref var h = ref handles[i];
                ref var d = ref deltas[i];

                linearCorrectSums[h.PositionIndex] += new Vector4d(d.Position * w, w);
                angularCorrectSums[h.RotationIndex] += new Vector4d(d.Rotation * w, w);
            }
        }
    }
}
