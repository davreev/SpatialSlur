/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PointLoad : IForce
    {
        private DynamicArray<Particle> _particles = new DynamicArray<Particle>();
        private DynamicArray<Vector3d> _loadForces;


        /// <summary>
        /// 
        /// </summary>
        public DynamicArray<Particle> Particles
        {
            get => _particles;
        }


        /// <summary>
        /// Per-particle load forces
        /// </summary>
        public DynamicArray<Vector3d> LoadForces
        {
            get => _loadForces;
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
            var particles = Particles;
            var loadForces = _loadForces;

            for (int i = 0; i < particles.Count; i++)
                forceSums[particles[i].PositionIndex] += loadForces[i];
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
