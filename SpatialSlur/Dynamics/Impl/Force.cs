
/*
 * Notes
 */

using System;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for a force that acts on the positions and rotations of a dynamic collection of particles.
    /// </summary>
    public abstract class Force : InfluenceBase<(Vector3d, Vector3d)>, IForce
    {
        /// <inheritdoc />
        public virtual void Accumulate(ArrayView<Vector3d> forceSum, ArrayView<Vector3d> torqueSum)
        {
            var handles = Handles;
            var deltas = Deltas;

            for(int i = 0; i < handles.Count; i++)
            {
                forceSum[handles[i].PositionIndex] += deltas[i].Item1;
                torqueSum[handles[i].RotationIndex] += deltas[i].Item2;
            }
        }
    }
}
