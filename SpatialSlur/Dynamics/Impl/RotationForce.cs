
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
    public abstract class RotationForce : InfluenceBase<Vector3d>, IForce
    {
        /// <summary>
        /// 
        /// </summary>
        public RotationForce() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        public RotationForce(IEnumerable<ParticleHandle> handles)
            : base(handles) { }


        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector3d> forceSum, 
            ArrayView<Vector3d> torqueSum)
        {
            var handles = Handles;
            var deltas = Deltas;

            for (int i = 0; i < handles.Count; i++)
                torqueSum[handles[i].RotationIndex] += deltas[i];
        }
    }
}
