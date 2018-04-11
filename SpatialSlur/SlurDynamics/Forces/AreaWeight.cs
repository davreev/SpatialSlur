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
    /// Applies a force proportional to the area of the triangle defined by 3 particles.
    /// </summary>
    [Serializable]
    public class AreaWeight : Force, IConstraint
    {
        private H _h0 = new H();
        private H _h1 = new H();
        private H _h2 = new H();
        private Vec3d _acceleration;
        private double _massPerArea = 1.0;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="acceleration"></param>
        /// <param name="massPerArea"></param>
        /// <param name="weight"></param>
        public AreaWeight(int i0, int i1, int i2, Vec3d acceleration, double massPerArea = 1.0, double strength = 1.0)
        {
            _h0.Index = i0;
            _h1.Index = i1;
            _h2.Index = i2;

            _acceleration = acceleration;
            MassPerArea = massPerArea;
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
        public H Handle2
        {
            get { return _h2; }
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
        public double MassPerArea
        {
            get { return _massPerArea; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Mass must be greater than zero.");

                _massPerArea = value;
            }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            Vec3d p0 = bodies[_h0].Position;
            Vec3d p1 = bodies[_h1].Position;
            Vec3d p2 = bodies[_h2].Position;

            const double inv6 = 1.0 / 6.0;
            _h0.Delta = _h1.Delta = _h2.Delta = _acceleration * (Vec3d.Cross(p1 - p0, p2 - p1).Length * _massPerArea * Strength * inv6);
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_h0].ApplyForce(_h0.Delta);
            bodies[_h1].ApplyForce(_h1.Delta);
            bodies[_h2].ApplyForce(_h2.Delta);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _h0;
                yield return _h1;
                yield return _h2;
            }
        }

        #endregion
    }
}
