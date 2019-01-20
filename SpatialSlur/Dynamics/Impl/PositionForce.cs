/*
 * Notes
 */

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for forces that act on the positions of a group of particles
    /// </summary>
    public abstract class PositionForce : Influence<Vector3d>, IForce
    {
        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums)
        {
            var handles = Particles;
            var deltas = Deltas;

            for (int i = 0; i < handles.Count; i++)
                forceSums[handles[i].PositionIndex] += deltas[i];
        }
    }
}
