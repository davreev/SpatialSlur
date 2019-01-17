
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
        Particle _h0, _h1, _h2;
        private Vector3d _d0;

        private double _strength;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle0"></param>
        /// <param name="handle1"></param>
        /// <param name="handle2"></param>
        /// <param name="strength"></param>
        public Pressure(
            Particle handle0,
            Particle handle1,
            Particle handle2,
            double strength)
        {
            _h0 = handle0;
            _h1 = handle1;
            _h2 = handle2;
            _strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        public Particle Handle0
        {
            get { return _h0; }
            set { _h0 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Particle Handle1
        {
            get { return _h1; }
            set { _h1 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Particle Handle2
        {
            get { return _h2; }
            set { _h2 = value; }
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
        public void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            Vector3d p0 = positions[_h0.PositionIndex].Current;
            Vector3d p1 = positions[_h1.PositionIndex].Current;
            Vector3d p2 = positions[_h2.PositionIndex].Current;

            const double inv6 = 1.0 / 6.0;
            _d0 = Vector3d.Cross(p1 - p0, p2 - p1) * (inv6 * _strength); // force is proportional to 1/3 area of tri
        }


        /// <inheritdoc />
        public void Accumulate(
            ArrayView<Vector3d> forceSums,
            ArrayView<Vector3d> torqueSums)
        {
            forceSums[_h0.PositionIndex] += _d0;
            forceSums[_h1.PositionIndex] += _d0;
            forceSums[_h2.PositionIndex] += _d0;
        }


        #region Explicit interface implementations

        void IInfluence.Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }

        #endregion
    }
}
