
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force proportional to the mass of each particle.
    /// </summary>
    [Serializable]
    public class Weight : IForce
    {
        private Vector3d _d0;
        private int _i0;

        private Vector3d _acceleration;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acceleration"></param>
        public Weight(int index, Vector3d acceleration)
        {
            _i0 = index;
            _acceleration = acceleration;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get { return _i0; }
            set { _i0 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }


        /// <inheritdoc />
        /// 
        public void Calculate(ReadOnlyArrayView<Particle> particles)
        {
            _d0 = _acceleration * (1.0 / particles[_i0].Position.MassInv);
        }


        /// <inheritdoc />
        /// 
        public void Apply(ReadOnlyArrayView<Particle> particles)
        {
            particles[_i0].Position.ForceSum += _d0;
        }


        #region Explicit interface implementations

        void IInfluence.Initialize(ReadOnlyArrayView<Particle> particles) { }

        #endregion
    }
}
