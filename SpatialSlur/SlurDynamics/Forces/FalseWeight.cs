using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = FalseWeight.CustomHandle;

    /// <summary>
    /// Applies a force proportional to the mass defined on each handle.
    /// </summary>
    [Serializable]
    public class FalseWeight : Force, IConstraint
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class CustomHandle : ParticleHandle
        {
            private double _mass = 1.0;


            /// <summary>
            /// 
            /// </summary>
            public double Mass
            {
                get { return _mass; }
                set
                {
                    if (value < 0.0)
                        throw new ArgumentOutOfRangeException("The value can not be negative.");

                    _mass = value;
                }
            }
        }

        #endregion


        private H _handle = new H();
        private Vec3d _acceleration;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="capacity"></param>
        /// <param name="strength"></param>
        public FalseWeight(int index, Vec3d acceleration, double strength = 1.0)
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
            _handle.Delta = _acceleration * (_handle.Mass * Strength);
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
