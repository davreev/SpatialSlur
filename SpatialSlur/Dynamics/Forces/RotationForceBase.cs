
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Base class for a force that acts on both positions and rotations of a collection of particles.
    /// </summary>
    [Serializable]
    public abstract class RotationForceBase : Impl.InfluenceBase<Vector3d>, IForce
    {
        /// <summary>
        /// 
        /// </summary>
        public RotationForceBase()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        public RotationForceBase(IEnumerable<int> indices)
            : base(indices)
        {
        }


        /// <inheritdoc />
        /// 
        protected override void Apply(
            ReadOnlyArrayView<Particle> particles,
            ReadOnlyArrayView<int> indices,
            ReadOnlyArrayView<Vector3d> deltas)
        {
            for (int i = 0; i < indices.Count; i++)
                particles[indices[i]].Rotation.TorqueSum += deltas[i];
        }
    }
}
