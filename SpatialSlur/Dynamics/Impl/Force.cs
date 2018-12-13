
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
    public abstract class Force : InfluenceBase<(Vector3d Position, Vector3d Rotation)>, IForce
    {
        /// <summary>
        /// 
        /// </summary>
        public Force() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        public Force(IEnumerable<ParticleHandle> handles) 
            : base(handles) { }


        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector3d> forceSum, 
            ArrayView<Vector3d> torqueSum)
        {
            var handles = Handles;
            var deltas = Deltas;

            for(int i = 0; i < handles.Count; i++)
            {
                forceSum[handles[i].PositionIndex] += deltas[i].Position;
                torqueSum[handles[i].RotationIndex] += deltas[i].Rotation;
            }
        }
    }
}
