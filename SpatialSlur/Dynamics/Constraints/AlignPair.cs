
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class AlignPair : IConstraint
    {
        private ParticleHandle _h0, _h1;
        private Vector3d _d0, _d1;
        private Vector3d _target;
        private double _weight;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle0"></param>
        /// <param name="handle1"></param>
        /// <param name="target"></param>
        /// <param name="weight"></param>
        public AlignPair(ParticleHandle handle0, ParticleHandle handle1, Vector3d target, double weight = 1.0)
        {
            _h0 = handle0;
            _h1 = handle1;
            _target = target;
            Weight = weight;
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
        public Vector3d Target
        {
            get { return _target; }
            set { _target = value; }
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
            ref var p0 = ref positions[_h0.PositionIndex];
            ref var p1 = ref positions[_h1.PositionIndex];

            var d = Vector3d.Reject(p1.Current - p0.Current, _target);

            var w0 = p0.InverseMass;
            var w1 = p1.InverseMass;
            var invSum = 1.0 / (w0 + w1);

            _d0 = d * (w0 * invSum);
            _d1 = d * (-w1 * invSum);
        }


        /// <inheritdoc />
        public void Accumulate(
            ArrayView<Vector4d> linearCorrectSums,
            ArrayView<Vector4d> angularCorrectSums)
        {
            var w = Weight;
            linearCorrectSums[_h0.PositionIndex] += new Vector4d(_d0 * w, w);
            linearCorrectSums[_h1.PositionIndex] += new Vector4d(_d1 * w, w);
        }


        #region Explicit interface implementations

        void IInfluence.Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }

        #endregion
    }
}
