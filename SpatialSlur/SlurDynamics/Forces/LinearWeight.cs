using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Forces
{
    using H = ParticleHandle;

    /// <summary>
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    [Serializable]
    public class LinearWeight : Force, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private Vec3d _acceleration;
        private double _massPerLength;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="acceleration"></param>
        /// <param name="massPerLength"></param>
        /// <param name="strength"></param>
        public LinearWeight(int i0, int i1, Vec3d acceleration, double massPerLength = 1.0, double strength = 1.0)
        {
            _h0.Index = i0;
            _h1.Index = i1;

            _acceleration = acceleration;
            MassPerLength = massPerLength;
            Strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle0
        {
            get { return _h0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle1
        {
            get { return _h1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double MassPerLength
        {
            get { return _massPerLength; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Mass must be greater than zero.");

                _massPerLength = value;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            _h0.Delta = _h1.Delta = _acceleration * (bodies[_h0].Position.DistanceTo(bodies[_h1].Position) * _massPerLength * Strength * 0.5);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_h0].ApplyForce(_h0.Delta);
            bodies[_h1].ApplyForce(_h1.Delta);
        }


        #region Explicit interface implementations

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
            }
        }

        #endregion
    }
}
