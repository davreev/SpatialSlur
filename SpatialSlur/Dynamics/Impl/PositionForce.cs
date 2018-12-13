
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for a force that acts on the positions of a dynamic collection of particles.
    /// </summary>
    public abstract class PositionForce : InfluenceBase<Vector3d>, IForce
    {
        /// <summary>
        /// 
        /// </summary>
        public PositionForce() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        public PositionForce(IEnumerable<ParticleHandle> handles)
            : base(handles) { }


        /// <inheritdoc />
        public virtual void Accumulate(
            ArrayView<Vector3d> forceSum, 
            ArrayView<Vector3d> torqueSum)
        {
            var handles = Handles;
            var deltas = Deltas;

            for (int i = 0; i < handles.Count; i++)
                forceSum[handles[i].PositionIndex] += deltas[i];
        }
    }
}
