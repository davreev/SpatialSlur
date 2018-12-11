
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force proportional to the area of the triangle defined by 3 particles.
    /// </summary>
    [Serializable]
    public class AreaLoad : IForce
    {
        private Vector3d _d0;
        private int _i0, _i1, _i2;

        private Vector3d _force;


        /// <summary>
        /// 
        /// </summary>
        public int Index0
        {
            get { return _i0; }
            set { _i0 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index1
        {
            get { return _i1; }
            set { _i1 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index2
        {
            get { return _i2; }
            set { _i2 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Force
        {
            get { return _force; }
            set { _force = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="force"></param>
        /// <param name="massPerArea"></param>
        /// <param name="strength"></param>
        public AreaLoad(int index0, int index1, int index2, Vector3d force)
        {
            _i0 = index0;
            _i1 = index1;
            _i2 = index2;
            _force = force;
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Particle> particles)
        {
            Vector3d p0 = particles[_i0].Position.Current;
            Vector3d p1 = particles[_i1].Position.Current;
            Vector3d p2 = particles[_i2].Position.Current;

            const double inv6 = 1.0 / 6.0;
            _d0 = _force * (Vector3d.Cross(p1 - p0, p2 - p1).Length * inv6);
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Particle> particles)
        {
            particles[_i0].Position.ForceSum += _d0;
            particles[_i1].Position.ForceSum += _d0;
            particles[_i2].Position.ForceSum += _d0;
        }


        #region Explicit Interface Implementations

        void IInfluence.Initialize(ReadOnlyArrayView<Particle> particles) { }

        #endregion
    }
}
