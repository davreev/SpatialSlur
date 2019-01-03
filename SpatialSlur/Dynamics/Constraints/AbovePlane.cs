
/*
 * Notes
 */

using System;
using System.Linq;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AbovePlane : IConstraint
    {
        private ParticleHandle _h0;
        private Vector3d _d0;

        private Vector3d _origin;
        private Vector3d _normal;
        private double _weight;
        private bool _accumulate;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="weight"></param>
        public AbovePlane(ParticleHandle handle, Vector3d origin, Vector3d normal, double weight = 1.0)
        {
            _h0 = handle;
            _origin = origin;
            _normal = normal;
            Weight = weight;
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
        public Vector3d Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Weight
        {
            get { return _weight; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _weight = value;
            }
        }


        /// <inheritdoc />
        public void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            double d = Vector3d.Dot(_origin - positions[_h0.PositionIndex].Current, _normal);

            if (d <= 0.0)
            {
                _d0 = Vector3d.Zero;
                _accumulate = false;
                return;
            }

            _d0 = _normal * (d / _normal.SquareLength);
            _accumulate = true;
        }


        /// <inheritdoc />
        public void Accumulate(
            ArrayView<Vector4d> linearCorrectSums,
            ArrayView<Vector4d> angularCorrectSums)
        {
            if (!_accumulate)
                return;
            
            linearCorrectSums[_h0.PositionIndex] += new Vector4d(_d0 * Weight, Weight);
        }


        #region Explicit interface implementations

        void IInfluence.Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }

        #endregion
    }
}
