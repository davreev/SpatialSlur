
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for a force that acts on the rotations of a dynamic collection of particles.
    /// </summary>
    public abstract class RotationForce : Influence<Vector3d>, IForce
    {
        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums)
        {
            var handles = Handles;
            var deltas = Deltas;

            for (int i = 0; i < handles.Count; i++)
                torqueSums[handles[i].RotationIndex] += deltas[i];
        }
    }
}
