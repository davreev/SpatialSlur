
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;
using System.Threading;

using static SpatialSlur.Geometry;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PlanarQuad : IConstraint
    {
        private ParticleHandle _h0, _h1, _h2, _h3;
        private Vector3d _d0, _d1;
        private double _weight;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hande0"></param>
        /// <param name="handle1"></param>
        /// <param name="handle2"></param>
        /// <param name="handle3"></param>
        /// <param name="weight"></param>
        public PlanarQuad(ParticleHandle hande0, ParticleHandle handle1, ParticleHandle handle2, ParticleHandle handle3, double weight = 1.0)
        {
            _h0 = hande0;
            _h1 = handle1;
            _h2 = handle2;
            _h3 = handle3;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle Handle0
        {
            get => _h0;
            set => _h0 = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle Handle1
        {
            get => _h1;
            set => _h1 = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle Handle2
        {
            get => _h2;
            set => _h2 = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public ParticleHandle Handle3
        {
            get => _h3;
            set => _h3 = value;
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
            ref var p2 = ref positions[_h2.PositionIndex];
            ref var p3 = ref positions[_h3.PositionIndex];
            
            var d = LineLineShortestVector(p0.Current, p2.Current, p1.Current, p3.Current);

            var w0 = p0.InverseMass + p2.InverseMass;
            var w1 = p1.InverseMass + p3.InverseMass;
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

            var d0 = new Vector4d(_d0 * w, w);
            linearCorrectSums[_h0.PositionIndex] += d0;
            linearCorrectSums[_h2.PositionIndex] += d0;

            var d1 = new Vector4d(_d1 * w, w);
            linearCorrectSums[_h1.PositionIndex] += d1;
            linearCorrectSums[_h3.PositionIndex] += d1;
        }


        #region Explicit interface implementations

        void IInfluence.Initialize(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        { }

        #endregion
    }
}
