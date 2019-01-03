
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for a force that acts on the positions and rotations of a dynamic collection of particles.
    /// </summary>
    public abstract class Force : Influence<(Vector3d Position, Vector3d Rotation)>, IForce
    {
        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums)
        {
            var handles = Handles;
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
