/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for forces that act on the rotations of a group of particles.
    /// </summary>
    public abstract class RotationForce : Influence<Vector3d>, IForce
    {
        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums)
        {
            var handles = Particles;
            var deltas = Deltas;

            for (int i = 0; i < handles.Count; i++)
                torqueSums[handles[i].RotationIndex] += deltas[i];
        }
    }
}
