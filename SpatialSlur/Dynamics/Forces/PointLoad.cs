
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
    /// 
    /// </summary>
    [Serializable]
    public class PointLoad : IForce
    {
        private ParticleHandle _h0;
        private Vector3d _d0;
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="loadForce"></param>
        public PointLoad(ParticleHandle handle, Vector3d loadForce)
        {
            _h0 = handle;
            _d0 = loadForce;
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle Handle
        {
            get { return _h0; }
            set { _h0 = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d LoadForce
        {
            get { return _d0; }
            set { _d0 = value; }
        }


        /// <inheritdoc />
        public void Calculate(ParticleBuffer particles)
        {
        }


        /// <inheritdoc />
        public void Accumulate(
            ArrayView<Vector3d> forceSum,
            ArrayView<Vector3d> torqueSum)
        {
            forceSum[_h0.PositionIndex] += _d0;
        }


        #region Explicit interface implementations

        void IInfluence.Initialize(ParticleBuffer particles) { }

        #endregion
    }
}
