
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
    public abstract class ForceBase : Impl.InfluenceBase<(Vector3d, Vector3d)>, IForce
    {
        /// <summary>
        /// 
        /// </summary>
        public ForceBase()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        public ForceBase(IEnumerable<int> indices)
            : base(indices)
        {
        }


        /// <inheritdoc />
        /// 
        protected override void Apply(
            ReadOnlyArrayView<Particle> particles, 
            ReadOnlyArrayView<int> indices, 
            ReadOnlyArrayView<(Vector3d, Vector3d)> deltas)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                (var p, var r) = particles[indices[i]];
                p.ForceSum += deltas[i].Item1;
                r.TorqueSum += deltas[i].Item2;
            }
        }
    }
}
