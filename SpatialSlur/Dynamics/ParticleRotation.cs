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

        /// <summary>
        /// 
        /// </summary>
        public static readonly ParticleRotation Default = new ParticleRotation
        {
            Current = Quaterniond.Identity,
            Velocity = Vector3d.Zero,
            InverseMass = new Vector3d(1.0)
        };

        #endregion


        /// <summary>Current rotation of the particle</summary>
        public Quaterniond Current;

        /// <summary>Current rotational velocity of the particle</summary>
        public Vector3d Velocity;
        
        /// <summary>
        /// Inverse rotational mass of the particle (i.e. the diagonal elements of the inverse inertia tensor)
        /// Note that this assumes that the current rotation represents the particle's principle axes of inertia
        /// </summary>
        public Vector3d InverseMass;


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
        public ParticleRotation(Quaterniond current, Vector3d inverseMass)
        {
            Current = current;
            Velocity = Vector3d.Zero;
            InverseMass = inverseMass;
        }
    }
}
