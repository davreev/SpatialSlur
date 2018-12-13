
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
    public struct ParticleRotation
    {
        #region Static

        /// <summary></summary>
        public static readonly ParticleRotation Default = new ParticleRotation
        {
            Current = Quaterniond.Identity,
            Velocity = Vector3d.Zero,
            InertiaInv = new Vector3d(1.0)
        };

        #endregion


        /// <summary>Current rotation of the particle</summary>
        public Quaterniond Current;

        /// <summary>Current angular velocity of the particle</summary>
        public Vector3d Velocity;
        
        /// <summary>Inverse rotational mass of the particle</summary>
        public Vector3d InertiaInv;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public ParticleRotation(Quaterniond current)
        {
            Current = current;
            Velocity = Vector3d.Zero;
            InertiaInv = new Vector3d(1.0);
        }


        /// <summary>
        /// Sets the rotational mass of the particle.
        /// Assumes the current rotation is aligned with the particle's principal axes of inertia.
        /// </summary>
        public Vector3d Inertia
        {
            set
            {
                if (value.ComponentMin > 0.0)
                    InertiaInv = 1.0 / value;
                else
                    throw new ArgumentException("The value must be greater than zero.");
            }
        }


        /// <summary>
        /// Returns true if the current rotation quaternion is unit length i.e. it represents a valid rotation
        /// </summary>
        public bool IsValid(double epsilon = SlurMath.Constantsd.ZeroTolerance)
        {
            return Current.IsUnit(epsilon);
        }
    }
}
