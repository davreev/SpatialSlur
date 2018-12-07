
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public class ParticlePosition
    {
        /// <summary>Current position</summary>
        public Vector3d Current;

        /// <summary>Linear velocity</summary>
        public Vector3d Velocity;

        /// <summary></summary>
        internal Vector3d ForceSum;

        /// <summary></summary>
        internal Vector3d DeltaSum;

        /// <summary></summary>
        internal double WeightSum;
        
        /// <summary></summary>
        private double _massInv = 1.0;

        /// <summary>
        /// 
        /// </summary>
        public ParticlePosition()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public ParticlePosition(Vector3d current)
        {
            Current = current;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private ParticlePosition(ParticlePosition other)
        {
            Current = other.Current;
            Velocity = other.Velocity;
            _massInv = other._massInv;
        }

        /// <summary>
        /// Returns a deep copy of this object.
        /// </summary>
        /// <returns></returns>
        public ParticlePosition Duplicate()
        {
            return new ParticlePosition(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public double Mass
        {
            set
            {
                if (value > 0.0)
                    _massInv = 1.0 / value;
                else
                    throw new ArgumentException("The value must be greater than zero.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double MassInv
        {
            get => _massInv;
            set
            {
                if (value < 0.0)
                    throw new ArgumentException("The value cannot be negative.");

                _massInv = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        public void AddForce(Vector3d force)
        {
            ForceSum += force;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        public void AddDelta(Vector3d delta, double weight)
        {
            DeltaSum += delta * weight;
            WeightSum += weight;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearForces()
        {
            ForceSum = Vector3d.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearDeltas()
        {
            DeltaSum = Vector3d.Zero;
            WeightSum = 0.0;
        }
    }
}
