
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public readonly struct Particle
    {
        /// <summary></summary>
        public readonly ParticlePosition Position;

        /// <summary></summary>
        public readonly ParticleRotation Rotation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public Particle(Vector3d position)
        {
            Position = new ParticlePosition(position);
            Rotation = null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public Particle(Quaterniond rotation)
        {
            Position = null;
            Rotation = new ParticleRotation(rotation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Particle(Vector3d position, Quaterniond rotation)
        {
            Position = new ParticlePosition(position);
            Rotation = new ParticleRotation(rotation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Particle(ParticlePosition position, ParticleRotation rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private Particle(Particle other)
        {
            Position = other.Position?.Duplicate();
            Rotation = other.Rotation?.Duplicate();
        }

        /// <summary>
        /// Returns a deep copy of this particle
        /// </summary>
        /// <returns></returns>
        public Particle Duplicate()
        {
            return new Particle(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void Deconstruct(out ParticlePosition position, out ParticleRotation rotation)
        {
            position = Position;
            rotation = Rotation;
        }
    }
}
