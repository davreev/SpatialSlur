/*
 * Notes
 */

using System;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// Base class for forces and constraints that act on a group of particles
    /// </summary>
    [Serializable]
    public abstract class Influence<TDelta> : IInfluence
        where TDelta : struct
    {
        private SlurList<Particle> _particles = new SlurList<Particle>();
        private TDelta[] _deltas = Array.Empty<TDelta>();
        private bool _parallel;


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
        public ArrayView<TDelta> Deltas
        {
            get { return _deltas.AsView(_particles.Count); }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Parallel
        {
            get => _parallel;
            set => _parallel = value;
        }


        /// <inheritdoc />
        public virtual void Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }


        /// <inheritdoc />
        public virtual void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            if (_deltas.Length < _particles.Count)
                _deltas = new TDelta[_particles.Capacity];
        }
    }
}
