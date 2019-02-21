/*
 * Notes
 */ 
 
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// 
    /// </summary>
    public class ConstantForce : IForce
    {
        private SlurList<Particle> _particles = new SlurList<Particle>();
        private Vector3d _force;


        /// <summary>
        /// 
        /// </summary>
        public SlurList<Particle> Particles
        {
            get => _particles;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Force
        {
            get => _force;
            set => _force = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSums"></param>
        /// <param name="torqueSums"></param>
        public void Accumulate(
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums)
        {
            var handles = Particles;
            var force = _force;

            for (int i = 0; i < handles.Count; i++)
                forceSums[handles[i].PositionIndex] += force;
        }


        #region Explicit interface impl

        /// <inheritdoc />
        void IInfluence.Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }


        /// <inheritdoc />
        void IInfluence.Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }

        #endregion
    }
}
