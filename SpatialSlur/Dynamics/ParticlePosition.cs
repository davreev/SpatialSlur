
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public struct ParticlePosition
    {
        #region Static

        /// <summary></summary>
        public static readonly ParticlePosition Default = new ParticlePosition
        {
            Current = Vector3d.Zero,
            Velocity = Vector3d.Zero,
            MassInv = 1.0
        };

        #endregion


        /// <summary>Current position of the particle</summary>
        public Vector3d Current;

        /// <summary>Current velocity of the particle</summary>
        public Vector3d Velocity;
        
        /// <summary>Inverse mass of the particle</summary>
        public double MassInv;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public ParticlePosition(Vector3d current)
        {
            Current = current;
            Velocity = Vector3d.Zero;
            MassInv = 1.0;
        }


        /// <summary>
        /// 
        /// </summary>
        public double Mass
        {
            set
            {
                if (value > 0.0)
                    MassInv = 1.0 / value;
                else
                    throw new ArgumentException("The value must be greater than zero.");
            }
        }
    }
}
