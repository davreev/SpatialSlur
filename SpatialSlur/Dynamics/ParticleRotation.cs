
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
            InverseInertia = new Vector3d(1.0)
        };

        #endregion


        /// <summary>Current rotation of the particle</summary>
        public Quaterniond Current;

        /// <summary>Current angular velocity of the particle</summary>
        public Vector3d Velocity;
        
        /// <summary>Inverse rotational mass of the particle</summary>
        public Vector3d InverseInertia;


        /// <summary>
        /// 
        /// </summary>
        public ParticleRotation(Quaterniond current)
            :this(current, new Vector3d(1.0))
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleRotation(Quaterniond current, Vector3d inverseInertia)
        {
            Current = current;
            Velocity = Vector3d.Zero;
            InverseInertia = inverseInertia;
        }
    }
}
