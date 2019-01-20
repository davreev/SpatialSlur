/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for forces that act on both positions and rotations of a group of particles
    /// </summary>
    public abstract class Force : Influence<(Vector3d Position, Vector3d Rotation)>, IForce
    {
        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums)
        {
            var handles = Particles;
            var deltas = Deltas;

            for (int i = 0; i < handles.Count; i++)
            {
                ref var h = ref handles[i];
                ref var d = ref deltas[i];
                forceSums[h.PositionIndex] += d.Position;
                torqueSums[h.RotationIndex] += d.Rotation;
            }
        }
    }
}
