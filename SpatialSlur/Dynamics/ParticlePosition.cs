
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
    public struct ParticlePosition
    {
        #region Static

        /// <summary></summary>
        public static readonly ParticlePosition Default = new ParticlePosition
        {
            Current = Vector3d.Zero,
            Velocity = Vector3d.Zero,
            InverseMass = 1.0
        };

        #endregion


        /// <summary>Current position of the particle</summary>
        public Vector3d Current;

        /// <summary>Current velocity of the particle</summary>
        public Vector3d Velocity;
        
        /// <summary>Inverse mass of the particle</summary>
        public double InverseMass;


        /// <summary>
        /// 
        /// </summary>
        public ParticlePosition(Vector3d current)
            : this(current, 1.0)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticlePosition(Vector3d current, double inverseMass)
        {
            Current = current;
            Velocity = Vector3d.Zero;
            InverseMass = inverseMass;
        }
    }
}
