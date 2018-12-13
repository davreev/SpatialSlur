
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
        private ParticleHandle _h0, _h1, _h2;
        private Vector3d _d0;

        private Vector3d _loadForce;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle0"></param>
        /// <param name="handle1"></param>
        /// <param name="handle2"></param>
        /// <param name="loadForce"></param>
        public AreaLoad(
            ParticleHandle handle0, 
            ParticleHandle handle1, 
            ParticleHandle handle2, 
            Vector3d loadForce)
        {
            _h0 = handle0;
            _h1 = handle1;
            _h2 = handle2;
            _loadForce = loadForce;
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle Handle0
        {
            get { return _h0; }
            set { _h0 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle Handle1
        {
            get { return _h1; }
            set { _h1 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle Handle2
        {
            get { return _h2; }
            set { _h2 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d LoadForce
        {
            get { return _loadForce; }
            set { _loadForce = value; }
        }


        /// <inheritdoc />
        public void Calculate(ParticleBuffer particles)
        {
            var pp = particles.Positions;
            Vector3d p0 = pp[_h0.PositionIndex].Current;
            Vector3d p1 = pp[_h1.PositionIndex].Current;
            Vector3d p2 = pp[_h2.PositionIndex].Current;

            const double inv6 = 1.0 / 6.0;
            _d0 = _loadForce * (Vector3d.Cross(p1 - p0, p2 - p1).Length * inv6);
        }


        /// <inheritdoc />
        public void Accumulate(
            ArrayView<Vector3d> forceSum, 
            ArrayView<Vector3d> torqueSum)
        {
            forceSum[_h0.PositionIndex] += _d0;
            forceSum[_h1.PositionIndex] += _d0;
            forceSum[_h2.PositionIndex] += _d0;
        }


        #region Explicit interface implementations
        
        void IInfluence.Initialize(ParticleBuffer particles) { }

        #endregion
    }
}
