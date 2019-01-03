
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
        public void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }


        /// <inheritdoc />
        public void Accumulate(
            ArrayView<Vector3d> forceSums,
            ArrayView<Vector3d> torqueSums)
        {
            forceSums[_h0.PositionIndex] += _d0;
        }


        #region Explicit interface implementations

        void IInfluence.Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }

        #endregion
    }
}
