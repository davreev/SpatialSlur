
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force along the normal of the triangle between 3 particles with a magnitude proportional to the area.
    /// </summary>
    [Serializable]
    public class Pressure : IForce
    {
        private Vector3d _d0;
        private int _i0, _i1, _i2;

        private double _strength;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="strength"></param>
        public Pressure(int index0, int index1, int index2, double strength = 1.0)
        {
            _i0 = index0;
            _i1 = index1;
            _i2 = index2;
            _strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index0
        {
            get { return _i0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index1
        {
            get { return _i1; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index2
        {
            get { return _i2; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }


        /// <inheritdoc />
        /// 
        public void Calculate(ReadOnlyArrayView<Particle> particles)
        {
            Vector3d p0 = particles[_i0].Position.Current;
            Vector3d p1 = particles[_i1].Position.Current;
            Vector3d p2 = particles[_i2].Position.Current;

            const double inv6 = 1.0 / 6.0;
            _d0 = Vector3d.Cross(p1 - p0, p2 - p1) * (inv6 * _strength); // force is proportional to 1/3 area of tri
        }


        /// <inheritdoc />
        /// 
        public void Apply(ReadOnlyArrayView<Particle> particles)
        {
            particles[_i0].Position.ForceSum += _d0;
            particles[_i1].Position.ForceSum += _d0;
            particles[_i2].Position.ForceSum += _d0;

        }


        #region Explicit interface implementations

        void IInfluence.Initialize(ReadOnlyArrayView<Particle> particles) { }

        #endregion
    }
}
