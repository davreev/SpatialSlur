using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// Applies a force proportional to the mass of each particle.
    /// </summary>
    [Serializable]
    public class Weight : Force, IConstraint
    {
        private H _handle = new H();
        private Vec3d _acceleration;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="capacity"></param>
        /// <param name="strength"></param>
        public Weight(int index, Vec3d acceleration, double strength = 1.0)
        {
            _handle.Index = index;
            _acceleration = acceleration;
            Strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle
        {
            get { return _handle; }
            set { _handle = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            _handle.Delta = _acceleration * (bodies[_handle].Mass * Strength);
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_handle].ApplyForce(_handle.Delta);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { yield return _handle; }
        }

        #endregion
    }
}
