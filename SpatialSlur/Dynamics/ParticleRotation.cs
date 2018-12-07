
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public class ParticleRotation
    {
        /// <summary>Current rotation</summary>
        public Quaterniond Current = Quaterniond.Identity;

        /// <summary>Angular velocity</summary>
        public Vector3d Velocity;

        /// <summary></summary>
        internal Vector3d TorqueSum;

        /// <summary></summary>
        internal Vector3d DeltaSum;

        /// <summary></summary>
        internal double WeightSum;
        
        /// <summary></summary>
        private Vector3d _inertiaInv = new Vector3d(1.0);

        /// <summary>
        /// 
        /// </summary>
        public ParticleRotation()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public ParticleRotation(Quaterniond current)
        {
            Current = current;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private ParticleRotation(ParticleRotation other)
        {
            Current = other.Current;
            Velocity = other.Velocity;
            _inertiaInv = other._inertiaInv;
        }

        /// <summary>
        /// Returns a deep copy of this object.
        /// </summary>
        /// <returns></returns>
        public ParticleRotation Duplicate()
        {
            return new ParticleRotation(this);
        }

        /// <summary>
        /// Gets/sets the rotational mass of the body.
        /// Assumes the current rotation is aligned with the body's principal axes of inertia.
        /// </summary>
        public Vector3d Inertia
        {
            set
            {
                if (value.ComponentMin > 0.0)
                    _inertiaInv = 1.0 / value;
                else
                    throw new ArgumentException("The value must be greater than zero.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3d InertiaInv
        {
            get => _inertiaInv;
            set
            {
                if (value.ComponentMin < 0.0)
                    throw new ArgumentException("The value cannot be negative");

                _inertiaInv = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="torque"></param>
        public void AddTorque(Vector3d torque)
        {
            TorqueSum += torque;
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
        public void ClearTorque()
        {
            TorqueSum = Vector3d.Zero;
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
