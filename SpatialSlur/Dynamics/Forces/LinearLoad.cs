
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
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    [Serializable]
    public class LinearLoad : IForce
    {
        private ParticleHandle _h0, _h1;
        private Vector3d _d0;

        private Vector3d _loadForce;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle0"></param>
        /// <param name="handle1"></param>
        /// <param name="loadForce"></param>
        public LinearLoad(
            ParticleHandle handle0, 
            ParticleHandle handle1, 
            Vector3d loadForce)
        {
            _h0 = handle0;
            _h1 = handle1;
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
        public Vector3d LoadForce
        {
            get { return _loadForce; }
            set { _loadForce = value; }
        }


        /// <inheritdoc />
        public void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            var d = positions[_h1.PositionIndex].Current - positions[_h0.PositionIndex].Current;
            _d0 = _loadForce * (d.Length * 0.5);
        }


        /// <inheritdoc />
        public void Accumulate(
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums)
        {
            forceSums[_h0.PositionIndex] += _d0;
            forceSums[_h1.PositionIndex] += _d0;
        }


        #region Explicit interface implementations

        void IInfluence.Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }

        #endregion
    }
}
